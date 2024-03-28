using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


//THIS IS A TEMPLATE MADE FROM READING UNITY DOCS AND RENDERER FEATURES SCRIPTS
//FOR USE IN URP ONLY!
internal class EdgeDetectionRendererFeature : ScriptableRendererFeature
{
    EdgeDetectionRenderPass pass;
    Material mat;

    //exposed to be tweaked in the renderer settings
    [SerializeField] Shader shader;
    [SerializeField] EdgeSetting settings;

    #region HELPER METHODS
    private bool GetMaterials()
    {
        if (mat == null && shader != null)
            mat = CoreUtils.CreateEngineMaterial(shader);
        return mat != null;
    }
    #endregion

    #region RENDERER METHODS
    public override void Create()
    {
        if (pass == null)
            pass = new();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!GetMaterials()) //checks if there is a valid material to use
        {
            Debug.LogErrorFormat("{0}.AddRenderPasses(): Missing material. {1} render pass will not be added.", GetType().Name, name);
            return;
        }
        bool setup = pass.SetUp(ref mat,ref settings);

        if (setup)
            renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        pass?.Dispose();
        pass = null;
        CoreUtils.Destroy(mat);
    }
    #endregion

    class EdgeDetectionRenderPass : ScriptableRenderPass
    {
        ProfilingSampler profileSampler;
        EdgeSetting settings;
        Material mat;
        RTHandle tempTexture, gauss1, gauss2, gauss3;
        RenderTextureDescriptor tempTextDesc;
        
        //DECLARE ANY VARIABLES YOU NEED HERE

        #region HELPER METHODS
        public bool SetUp(ref Material material,ref EdgeSetting setting) //setup the render pass with all the data
        {
            mat = material;
            settings = setting;

            ConfigureInput(settings.Requirements);
            tempTextDesc = new(Screen.width, Screen.height, RenderTextureFormat.RGB111110Float, 0);
            profileSampler = new(settings.ProfilerName); //assign a name to the profiler to be identified in frame debugger
            renderPassEvent = settings.InjectionPoint;

            return material != null;
        }

        public void Dispose()
        {
            //dispose of all the unused assets here

            //tempTexture.Release();// release the temporary texture
        }

        private void UpdateShaderSettings()
        {
            //SET MATERIAL VALUES HERE
            //E.G.
            //mat.SetFloat("_YourValue", settings.YourValue);
            mat.SetFloat("_Sigma",settings.Sigma);
            mat.SetFloat("_K", settings.K);
            mat.SetFloat("_Tau", settings.Tau);
            mat.SetFloat("_Phi", settings.Phi);
            mat.SetFloat("_Threshold", settings.Threshold);
            mat.SetFloat("_DoGStrength", settings.DoGStrength);
            mat.SetFloat("_BlendStrength", settings.BlendStrength);
            mat.SetInt("_GridSize", settings.KernelSize);
            mat.SetVector("_TexelSize",new Vector2(tempTexture.rt.texelSize.x,tempTexture.rt.texelSize.y));

            mat.SetKeyword(new(mat.shader,"THRESHOLDING"), settings.THRESHOLDING);
            mat.SetKeyword(new(mat.shader, "TANH"), settings.TANH);
            mat.SetKeyword(new(mat.shader, "INVERT"), settings.INVERT);
            SetBlendMode();

            mat.SetColor("_MinColor", settings.MinColor);
            mat.SetColor("_MaxColor", settings.MaxColor);
        }

        void SetBlendMode()
        {
            switch (settings.BlendMode)
            {
                case EdgeSetting.BlendType.NO_BLEND:
                    mat.EnableKeyword("BLEND_NONE");
                    mat.DisableKeyword("BLEND_INTERPOLATE");
                    mat.DisableKeyword("BLEND_TWO_POINT_INTERPOLATE");
                    break;
                case EdgeSetting.BlendType.INTERPOLATE:
                    mat.DisableKeyword("BLEND_NONE");
                    mat.EnableKeyword("BLEND_INTERPOLATE");
                    mat.DisableKeyword("BLEND_TWO_POINT_INTERPOLATE");
                    break;
                case EdgeSetting.BlendType.TWO_POINT_INTERPOLATE:
                    mat.DisableKeyword("BLEND_NONE");
                    mat.DisableKeyword("BLEND_INTERPOLATE");
                    mat.EnableKeyword("BLEND_TWO_POINT_INTERPOLATE");
                    break;
                default:
                    Debug.Log("Could not get a blend mode");
                    break;
            }
        }
        #endregion

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //assign the correct size to the texture descriptor
            tempTextDesc.width = cameraTextureDescriptor.width;
            tempTextDesc.height = cameraTextureDescriptor.height;

            //re allocate the texture and assign a name so it can be identified in frame debugger / memory profiler
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_TempTexture");
            RenderingUtils.ReAllocateIfNeeded(ref gauss1, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_GaussTex1");
            RenderingUtils.ReAllocateIfNeeded(ref gauss2, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_GaussTex2");
            RenderingUtils.ReAllocateIfNeeded(ref gauss3, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_GaussTex3");
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //uncomment this if u want the effect to take place only in game
            //if (cameraData.camera.cameraType != CameraType.Game) return;

            if (mat == null) return; //another material check

            CommandBuffer cmd = CommandBufferPool.Get(); // get a command buffer from the pool
            RTHandle cameraColourTexture = renderingData.cameraData.renderer.cameraColorTargetHandle; //get the camera texture

            //assigns a identification scope so it can be identified in the frame debugger
            using (new ProfilingScope(cmd, profileSampler))
            {
                UpdateShaderSettings();

                Blitter.BlitCameraTexture(cmd, cameraColourTexture, gauss1, mat, 0); //hori blur
                mat.SetTexture("_GaussianTex", gauss1);
                Blitter.BlitTexture(cmd, cameraColourTexture, gauss2, mat, 1); //vert blur
                mat.SetTexture("_GaussianTex2", gauss2);
                Blitter.BlitTexture(cmd, gauss2, gauss3, mat, 2); //DoG
                mat.SetTexture("_DoGTex", gauss3);
                Blitter.BlitCameraTexture(cmd, cameraColourTexture, tempTexture,mat,3);
                Blitter.BlitCameraTexture(cmd, tempTexture, cameraColourTexture);

                if (settings.ViewEdges)
                    Blitter.BlitCameraTexture(cmd, gauss3, cameraColourTexture);
            }
            context.ExecuteCommandBuffer(cmd); //execute the shader
            cmd.Clear();
            CommandBufferPool.Release(cmd); //release the command buffer
        }
    }
}

[System.Serializable]
internal class EdgeSetting
{
    public RenderPassEvent InjectionPoint; //this is where the shader will be injected for post-processing
    public ScriptableRenderPassInput Requirements; //this is the buffer the pass requires
    public string ProfilerName = "EDGE_BLIT";

    //put your settings here
    [Range(1, 10)]public int KernelSize;
    [Range(0.1f, 5.0f)]public float Sigma;
    [Range(0.1f,5f)]public float K;
    [Range(0.01f,100)]public float Phi;
    [Range(0.01f,5.0f)]public float Tau;
    [Range(-1.0f, 1.0f)]public float Threshold;

    public BlendType BlendMode;
    public float DoGStrength;
    public float BlendStrength;
    public Color MinColor, MaxColor;


    public bool THRESHOLDING, TANH, INVERT;
    public bool ViewEdges;

    public enum BlendType
    {
        NO_BLEND,
        INTERPOLATE,
        TWO_POINT_INTERPOLATE
    }
}
