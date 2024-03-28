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
        RTHandle tempTexture, gauss1,gauss2, color;
        RTHandle structTensor;
        RTHandle eigen1, eigen2;
        RTHandle DoG;
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

            mat.SetFloat("_K", settings.StdDevScale);
            mat.SetFloat("_Tau", settings.Sharpness);
            mat.SetFloat("_Phi", settings.SoftThreshold);

            mat.SetFloat("_SigmaC", settings.StructureTensorDeviation);
            mat.SetFloat("_SigmaE", settings.DifferenceOfGaussianDeviation);
            mat.SetFloat("_SigmaA", settings.EdgeSmoothDeviation);
            mat.SetFloat("_SigmaM", settings.LineIntegralDeviation);

            mat.SetFloat("_Threshold",settings.WhitePoint_1);
            mat.SetFloat("_Threshold2", settings.WhitePoint_2);
            mat.SetFloat("_Threshold3", settings.WhitePoint_3);
            mat.SetFloat("_Threshold4", settings.WhitePoint_4);
            mat.SetFloat("_Thresholds", settings.QuantizerSteps);

            mat.SetFloat("_DoGStrength",settings.DoGStrength);
            mat.SetFloat("_BlendStrength",settings.BlendStrength);

            Vector4 stepSize = new(settings.LineConvolutionStepSize.x, settings.LineConvolutionStepSize.y, settings.EdgeSmoothStepSize.x, settings.EdgeSmoothStepSize.y);
            mat.SetVector("_IntegralConvolutionStepSizes",stepSize);

            SetThresholdType();
            mat.SetKeyword(new(mat.shader, "INVERT"), settings.INVERT);
            mat.SetKeyword(new(mat.shader, "CALCDIFFBEFORECONVOLUTION"), settings.CALCDIFFBEFORECONVOLUTION);

            SetBlendMode();
            mat.SetColor("_MinColor", settings.MinColor);
            mat.SetColor("_MaxColor", settings.MaxColor);
            /*
            mat.SetColor("_Colour", settings.EdgeColour);
            mat.SetFloat("_Sigma", settings.Sigma);
            mat.SetInt("_GridSize", settings.KernelSize);
            mat.SetFloat("_Threshold", settings.Threshold);

            mat.SetKeyword(new(mat.shader,"THRESHOLDING"),settings.THRESHOLDING);
            mat.SetKeyword(new(mat.shader, "TANH"), settings.TANH);
            */
        }

        void SetThresholdType()
        {
            switch (settings.THRESHOLDING)
            {
                case EdgeSetting.ThresholdType.TANH:
                    mat.EnableKeyword("THRESHOLDING_1");
                    mat.DisableKeyword("THRESHOLDING_2");
                    mat.DisableKeyword("THRESHOLDING_3");
                    mat.DisableKeyword("THRESHOLDING_DEFAULT");

                    break;
                case EdgeSetting.ThresholdType.QUANTIZATION:
                    mat.EnableKeyword("THRESHOLDING_2");
                    mat.DisableKeyword("THRESHOLDING_1");
                    mat.DisableKeyword("THRESHOLDING_3");
                    mat.DisableKeyword("THRESHOLDING_DEFAULT");
                    break;
                case EdgeSetting.ThresholdType.SMOOTHQUANTIZATION:
                    mat.EnableKeyword("THRESHOLDING_3");
                    mat.DisableKeyword("THRESHOLDING_2");
                    mat.DisableKeyword("THRESHOLDING_1");
                    mat.DisableKeyword("THRESHOLDING_DEFAULT");
                    break;
                case EdgeSetting.ThresholdType.NO_THRESHOLD:
                    mat.EnableKeyword("THRESHOLDING_DEFAULT");
                    mat.DisableKeyword("THRESHOLDING_2");
                    mat.DisableKeyword("THRESHOLDING_3");
                    mat.DisableKeyword("THRESHOLDING_1");
                    break;
                default:
                    Debug.Log("COUDLNT GET A THRESHOLD TYPE");
                    break;
            }
        }

        void SetBlendMode()
        {
            switch (settings.BlendType)
            {
                case EdgeSetting.BlendMode.NO_BLEND:
                    mat.EnableKeyword("BLEND_NONE");
                    mat.DisableKeyword("BLEND_INTERPOLATE");
                    mat.DisableKeyword("BLEND_TWO_POINT_INTERPOLATE");
                    break;
                case EdgeSetting.BlendMode.INTERPOLATE:
                    mat.DisableKeyword("BLEND_NONE");
                    mat.EnableKeyword("BLEND_INTERPOLATE");
                    mat.DisableKeyword("BLEND_TWO_POINT_INTERPOLATE");
                    break;
                case EdgeSetting.BlendMode.TWO_POINT_INTERPOLATE:
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
            RenderingUtils.ReAllocateIfNeeded(ref color, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_ColorTex");
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, tempTextDesc,FilterMode.Point,TextureWrapMode.Clamp, name :"_XDoGTemp");
            RenderingUtils.ReAllocateIfNeeded(ref gauss1, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_XDoGTex1");
            RenderingUtils.ReAllocateIfNeeded(ref gauss2, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_XDoGTex2");

            RenderingUtils.ReAllocateIfNeeded(ref structTensor, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_StructureTensorTex");
            RenderingUtils.ReAllocateIfNeeded(ref eigen1, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_EigenTex1");
            RenderingUtils.ReAllocateIfNeeded(ref eigen2, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_EigenTex2");

            RenderingUtils.ReAllocateIfNeeded(ref DoG, tempTextDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_DOG");
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

                if (settings.ConvertColor)
                {
                    Blitter.BlitCameraTexture(cmd, cameraColourTexture, color, mat, 6);
                }
                else
                {
                    Blitter.BlitCameraTexture(cmd, cameraColourTexture, color);
                }


                Blitter.BlitCameraTexture(cmd, color, structTensor, mat, 0);
                Blitter.BlitCameraTexture(cmd, structTensor, eigen1, mat, 1);
                Blitter.BlitCameraTexture(cmd, eigen1, eigen2, mat, 2);
                mat.SetTexture("_TFM", eigen2);
                Blitter.BlitCameraTexture(cmd,color,gauss1,mat,3);
                Blitter.BlitCameraTexture(cmd, gauss1, gauss2, mat, 4);

                Blitter.BlitCameraTexture(cmd, gauss2, DoG);
                mat.SetTexture("_DoGTex", DoG);
                Blitter.BlitCameraTexture(cmd, color, tempTexture,mat,5);
                Blitter.BlitCameraTexture(cmd, tempTexture, cameraColourTexture);

                if(settings.ViewEdges)
                    Blitter.BlitCameraTexture(cmd, DoG, cameraColourTexture);           
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

    //sigmas
    [Header("DEVIATIONS")]
    [Range(0,5)]public float StructureTensorDeviation;
    [Range(0,10)]public float DifferenceOfGaussianDeviation;
    [Range(0,20)]public float LineIntegralDeviation;
    [Range(0,10)]public float EdgeSmoothDeviation;
    [HideInInspector]public Vector2 LineConvolutionStepSize = Vector2.one;
    [HideInInspector]public Vector2 EdgeSmoothStepSize = Vector2.one;

    //thresholds
    [Header("THRESHOLD")]

    [Range(0, 100)]public float WhitePoint_1;
    [Range(0, 100)]public float WhitePoint_2, WhitePoint_3, WhitePoint_4;
    [Range(1,16)]public int QuantizerSteps;
    public ThresholdType THRESHOLDING;

    [Header("EDGE VALUES")]
    [Range(0.1f,5f)]public float StdDevScale;
    [Range(0,100)]public float Sharpness;
    [Range(0,10)]public float SoftThreshold;

    [Header("SHADER KEYWORDS")]
    public bool CALCDIFFBEFORECONVOLUTION;
    public bool INVERT;

    [Header("BLEND")]
    public BlendMode BlendType;
    public float DoGStrength;
    public float BlendStrength;
    public Color MinColor;
    public Color MaxColor;

    [Header("DEBUG")]
    public bool ViewEdges;
    public bool ConvertColor;

    public enum ThresholdType
    {
        TANH,
        QUANTIZATION,
        SMOOTHQUANTIZATION,
        NO_THRESHOLD
    }

    public enum BlendMode
    {
        NO_BLEND,
        INTERPOLATE,
        TWO_POINT_INTERPOLATE
    }
}
