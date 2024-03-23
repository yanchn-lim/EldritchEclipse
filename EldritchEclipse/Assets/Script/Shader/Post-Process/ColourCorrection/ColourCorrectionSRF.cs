using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor.ShaderKeywordFilter;

public class ColourCorrectionSRF : ScriptableRendererFeature
{
    ColourCorrectionRenderPass m_ScriptablePass;
    [SerializeField] ColourCorrectionSetting setting;
    [SerializeField]RenderPassEvent InjectionPoint;
    public Shader shader;
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
            m_ScriptablePass = new(mat);

        m_ScriptablePass.renderPassEvent = InjectionPoint;
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

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if(renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
        }   
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
        m_ScriptablePass = null;
        CoreUtils.Destroy(mat);
    }

    class ColourCorrectionRenderPass : ScriptableRenderPass
    {
        ProfilingSampler cc_PS = new("Color Correction Blit");
        ColourCorrectionSetting setting;
        Material mat;
        RTHandle textureHandle;
        RenderTextureDescriptor textDesc;

        public ColourCorrectionRenderPass(Material material)
        {
            //mat = material;
            textDesc = new(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
        }

        public bool SetUp(ref Material material,ColourCorrectionSetting setting)
        {
            ConfigureInput(ScriptableRenderPassInput.Color);
            mat = material;
            this.setting = setting;

            return mat != null;
        }

        private void UpdateShaderSetting()
        {
            //SET MATERIAL VALUES HERE
            mat.SetFloat("_Contrast", setting.Contrast);
            mat.SetFloat("_Brightness", setting.Brightness);
            mat.SetFloat("_Saturation", setting.Saturation);
            mat.SetFloat("_Gamma", setting.Gamma);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            textDesc.width = cameraTextureDescriptor.width;
            textDesc.height = cameraTextureDescriptor.height;
            RenderingUtils.ReAllocateIfNeeded(ref textureHandle, textDesc, name: "_ColourCorrection_Texture");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            //RenderTextureDescriptor camTargetDesc = renderingData.cameraData.cameraTargetDescriptor;
            //UpdateShaderSetting();
            //textDesc = camTargetDesc;
            //RenderingUtils.ReAllocateIfNeeded(ref textureHandle, textDesc,name : "_ColourCorrection_Texture");
            //ConfigureTarget(textureHandle);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            //if (cameraData.camera.cameraType != CameraType.Game) return;

            if (mat == null) return;

            CommandBuffer cmd = CommandBufferPool.Get();            
            RTHandle cameraTexture = renderingData.cameraData.renderer.cameraColorTargetHandle;
            
            using (new ProfilingScope(cmd, cc_PS))
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
            textureHandle.Release();
        }

    }

}

[System.Serializable]
public class ColourCorrectionSetting
{
    [Range(0, 2f)] public float Contrast;
    [Range(-1, 1f)] public float Brightness;
    [Range(0, 3f)] public float Saturation;
    [Range(0, 3f)] public float Gamma;
}


