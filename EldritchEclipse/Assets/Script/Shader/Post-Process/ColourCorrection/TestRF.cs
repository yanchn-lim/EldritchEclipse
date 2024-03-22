using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TestRF : ScriptableRendererFeature
{
    class TestRenderPass : ScriptableRenderPass
    {
        ProfilingSampler cc_PS = new("Test Blit");
        Material mat;
        RTHandle textureHandle;
        RenderTextureDescriptor td;

        public TestRenderPass(Material material)
        {
            mat = material;
            td = new(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
        }

        //public void SetTarget(RTHandle colorHandle)
        //{
        //    textureHandle = colorHandle;
        //}

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            td.width = cameraTextureDescriptor.width;
            td.height = cameraTextureDescriptor.height;

            RenderingUtils.ReAllocateIfNeeded(ref textureHandle, td);
        }

        //public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        //{
        //    ConfigureTarget(textureHandle);
        //}

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            if (cameraData.camera.cameraType != CameraType.Game) return;

            if (mat == null) return;

            CommandBuffer cmd = CommandBufferPool.Get();

            RTHandle cTH = renderingData.cameraData.renderer.cameraColorTargetHandle;

            using (new ProfilingScope(cmd, cc_PS))
            {
                Blitter.BlitCameraTexture(cmd, cTH, textureHandle, mat, 0);
                Blitter.BlitCameraTexture(cmd, textureHandle, cTH);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    TestRenderPass m_ScriptablePass;
    [SerializeField]
    RenderPassEvent InjectionPoint;
    [SerializeField]
    Shader shader;
    Material mat;
    public override void Create()
    {
        if (shader == null) return;

        mat = CoreUtils.CreateEngineMaterial(shader);

        m_ScriptablePass = new(mat);
        m_ScriptablePass.renderPassEvent = InjectionPoint;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game) //make it only render in game
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
            //m_ScriptablePass.SetTarget(renderer.cameraColorTargetHandle);
        }
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(mat);
    }
}


