using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor.ShaderKeywordFilter;

public class ColourCorrectionSRF : ScriptableRendererFeature
{
    ColourCorrectionRenderPass m_ScriptablePass;
    [SerializeField] ColourCorrectionSetting setting;
    [SerializeField] Shader shader;
    Material mat;

    private bool GetMaterials()
    {
        if (mat == null && shader != null)
            mat = CoreUtils.CreateEngineMaterial(shader);
        return mat != null;
    }

    public override void Create()
    {
        if(m_ScriptablePass == null)
            m_ScriptablePass = new();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!GetMaterials())
        {
            Debug.LogErrorFormat("{0}.AddRenderPasses(): Missing material. {1} render pass will not be added.", GetType().Name, name);
            return;
        }

        bool setup = m_ScriptablePass.SetUp(ref mat,setting);

        if(setup)
            renderer.EnqueuePass(m_ScriptablePass);
        
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
        m_ScriptablePass = null;
        CoreUtils.Destroy(mat);
    }

    class ColourCorrectionRenderPass : ScriptableRenderPass
    {
        ProfilingSampler profileSampler;
        ColourCorrectionSetting settings;
        Material mat;
        RTHandle textureHandle;
        RenderTextureDescriptor tempTextDesc;

        public bool SetUp(ref Material material,ColourCorrectionSetting setting)
        {

            mat = material;
            settings = setting;

            ConfigureInput(settings.Requirements);
            tempTextDesc = new(Screen.width, Screen.height, RenderTextureFormat.RGB111110Float, 0);
            UpdateShaderSetting();
            profileSampler = new(settings.ProfilerName);
            renderPassEvent = settings.InjectionPoint;

            return mat != null;
        }

        private void UpdateShaderSetting()
        {
            //SET MATERIAL VALUES HERE
            mat.SetFloat("_Contrast", settings.Contrast);
            mat.SetFloat("_Brightness", settings.Brightness);
            mat.SetFloat("_Saturation", settings.Saturation);
            mat.SetFloat("_Gamma", settings.Gamma);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            tempTextDesc.width = cameraTextureDescriptor.width;
            tempTextDesc.height = cameraTextureDescriptor.height;
            RenderingUtils.ReAllocateIfNeeded(ref textureHandle, tempTextDesc, name: "_ColourCorrection_Texture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //if (cameraData.camera.cameraType != CameraType.Game) return;

            if (mat == null) return;

            CommandBuffer cmd = CommandBufferPool.Get();            
            RTHandle cameraTexture = renderingData.cameraData.renderer.cameraColorTargetHandle;
            using (new ProfilingScope(cmd, profileSampler))
            {
                UpdateShaderSetting();
                //Blitter.BlitCameraTexture(cmd, textureHandle, textureHandle, mat, 0);
                Blitter.BlitCameraTexture(cmd, cameraTexture, textureHandle, mat, 0);
                Blitter.BlitCameraTexture(cmd, textureHandle, cameraTexture);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            //textureHandle.Release();
        }

    }

}

[System.Serializable]
public class ColourCorrectionSetting
{
    public RenderPassEvent InjectionPoint; //this is where the shader will be injected for post-processing
    public ScriptableRenderPassInput Requirements; //this is the buffer the pass requires
    public string ProfilerName = "COLOUR_CORRECTION_BLIT";

    [Range(0, 2f)] public float Contrast;
    [Range(-1, 1f)] public float Brightness;
    [Range(0, 3f)] public float Saturation;
    [Range(0, 3f)] public float Gamma;
}


