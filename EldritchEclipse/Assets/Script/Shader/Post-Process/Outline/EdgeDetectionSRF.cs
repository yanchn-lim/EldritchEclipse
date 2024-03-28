using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;

//THIS IS A TEMPLATE MADE FROM READING UNITY DOCS AND RENDERER FEATURES SCRIPTS
//FOR USE IN URP ONLY!
public class EdgeDetectionRendererFeature : ScriptableRendererFeature
{
    EdgeDetectionPass pass;
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
        bool setup = pass.SetUp(ref mat, ref settings);

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

    class EdgeDetectionPass : ScriptableRenderPass
    {
        ProfilingSampler profileSampler;
        EdgeSetting settings;
        Material mat;
        RTHandle tempTexture, gauss1,gauss2,gauss3;
        RTHandle structTensor;
        RTHandle eigen1, eigen2;
        RenderTextureDescriptor tempTextDesc;

        //DECLARE ANY VARIABLES YOU NEED HERE

        #region HELPER METHODS
        public bool SetUp(ref Material material, ref EdgeSetting setting) //setup the render pass with all the data
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
            mat.SetVector("_TexelSize", new Vector2(tempTexture.rt.texelSize.x, tempTexture.rt.texelSize.y));

            mat.SetFloat("_SigmaC", settings.SigmaC);
            mat.SetFloat("_SigmaE", settings.SigmaE);
            mat.SetFloat("_SigmaA", settings.SigmaA);
            mat.SetFloat("_SigmaM", settings.SigmaM);

            /*
            mat.SetColor("_Colour", settings.EdgeColour);
            mat.SetFloat("_Sigma", settings.Sigma);
            mat.SetInt("_GridSize", settings.KernelSize);
            mat.SetFloat("_K", settings.K);
            mat.SetFloat("_Tau", settings.Tau);
            mat.SetFloat("_Threshold", settings.Threshold);
            mat.SetFloat("_Phi", settings.Phi);

            mat.SetKeyword(new(mat.shader,"THRESHOLDING"),settings.THRESHOLDING);
            mat.SetKeyword(new(mat.shader, "TANH"), settings.TANH);
            mat.SetKeyword(new(mat.shader, "INVERT"), settings.INVERT);
            */
        }

        #endregion
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //assign the correct size to the texture descriptor
            tempTextDesc.width = cameraTextureDescriptor.width;
            tempTextDesc.height = cameraTextureDescriptor.height;

            //re allocate the texture and assign a name so it can be identified in frame debugger / memory profiler
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, tempTextDesc,FilterMode.Point,TextureWrapMode.Clamp, name :"_GaussianTexTemp");
            RenderingUtils.ReAllocateIfNeeded(ref gauss1, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_GaussianTex1");
            RenderingUtils.ReAllocateIfNeeded(ref gauss2, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_GaussianTex2");
            RenderingUtils.ReAllocateIfNeeded(ref gauss3, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_GaussianTex3");

            RenderingUtils.ReAllocateIfNeeded(ref structTensor, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_StructureTensorTex");
            RenderingUtils.ReAllocateIfNeeded(ref eigen1, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_EigenTex1");
            RenderingUtils.ReAllocateIfNeeded(ref eigen2, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_EigenTex2");



        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //uncomment this if u want the effect to take place only in game
            //if (cameraData.camera.cameraType != CameraType.Game) return;

            if (mat == null) return; //another material check

            CommandBuffer cmd = CommandBufferPool.Get(); // get a command buffer from the pool
            RTHandle cameraColourTexture = renderingData.cameraData.renderer.cameraColorTargetHandle; //get the camera texture
            RTHandle cameraDepthTexture = renderingData.cameraData.renderer.cameraDepthTargetHandle;
            UpdateShaderSettings();

            //assigns a identification scope so it can be identified in the frame debugger
            using (new ProfilingScope(cmd, profileSampler))
            {
                //Blitter.BlitCameraTexture(cmd, cameraColourTexture, gauss1, mat, 1);
                //mat.SetTexture("_GaussianTex", gauss1);
                //Blitter.BlitTexture(cmd,cameraColourTexture ,gauss2 ,mat,2);
                //mat.SetTexture("_GaussianTex2", gauss2);
                //Blitter.BlitTexture(cmd, gauss2, gauss3, mat, 3);
                //mat.SetTexture("_GaussianTex3", gauss3);
                //Blitter.BlitCameraTexture(cmd, cameraColourTexture, tempTexture,mat,4);
                //mat.SetTexture("_TempTex", tempTexture);
                //Blitter.BlitCameraTexture(cmd, tempTexture, cameraColourTexture);
                //if (settings.ViewEdges)
                //    Blitter.BlitCameraTexture(cmd, gauss3, cameraColourTexture);

                Blitter.BlitCameraTexture(cmd, cameraColourTexture, structTensor, mat, 0);
                Blitter.BlitCameraTexture(cmd, structTensor, eigen1, mat, 1);
                Blitter.BlitCameraTexture(cmd, eigen1, eigen2, mat, 2);
                mat.SetTexture("_EigenTex", eigen2);
                Blitter.BlitCameraTexture(cmd, eigen2, cameraColourTexture);

            }

            context.ExecuteCommandBuffer(cmd); //execute the shader
            cmd.Clear();
            CommandBufferPool.Release(cmd); //release the command buffer
        }
    }
}

[System.Serializable]
public class EdgeSetting
{
    public RenderPassEvent InjectionPoint; //this is where the shader will be injected for post-processing
    public ScriptableRenderPassInput Requirements; //this is the buffer the pass requires
    public string ProfilerName = "EDGE_DETECTION_BLIT";

    public float SigmaC, SigmaE, SigmaA, SigmaM;
    public ThresholdType THRESHOLDING;

    //[Header("EDGE VALUES")]
    ////put your settings here
    //public Color EdgeColour;
    //[Range(0,20)]public int KernelSize;
    //[Range(0.1f,20)]public float Sigma;
    //[Range(-1f,1f)]public float K;
    //public float Tau;
    //[Range(0,1)]public float Threshold;
    //public float Phi;

    //[Header("SHADER KEYWORDS")]
    ////keywords
    //public bool THRESHOLDING;
    //public bool TANH;
    //public bool INVERT;

    //[Header("DEBUG")]
    //public bool ViewEdges;

    public enum ThresholdType
    {
        THRESHOLD1,
        THRESHOLD2,
        THRESHOLD3,
        DEFAULT
    }

}
