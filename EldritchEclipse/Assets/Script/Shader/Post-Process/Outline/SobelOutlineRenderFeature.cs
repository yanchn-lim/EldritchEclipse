using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelOutlineRenderFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        Material mat;
        RenderTextureDescriptor descriptor;
        RTHandle cameraColorTarget;
        RTHandle cameraDepthTarget;
        RTHandle destTarget;
        public CustomRenderPass(Material material)
        {
            mat = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetTarget(RTHandle colorTarget, RTHandle depthTarget)
        {
            cameraColorTarget = colorTarget;
            cameraDepthTarget = depthTarget;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            descriptor = renderingData.cameraData.cameraTargetDescriptor;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            //using (new ProfilingScope(cmd, new("Sobel Outline Effect")))
            //{
            //    //set the stuff for the material

            //    destTarget = RTHandles.Alloc(descriptor);
            //    Blitter.BlitCameraTexture(cmd, cameraColorTarget, destTarget, mat, 0);
            //    Blitter.BlitCameraTexture(cmd, destTarget, cameraColorTarget);
            //}
            destTarget = RTHandles.Alloc(descriptor);
            Blitter.BlitCameraTexture(cmd, cameraColorTarget, destTarget, mat, 0);
            Blitter.BlitCameraTexture(cmd, destTarget, cameraColorTarget);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            destTarget.Release();
            cameraColorTarget.Release();
            cameraDepthTarget.Release();
            cmd.Release();
        }
    }

    CustomRenderPass m_ScriptablePass;

    //list shaders
    public Shader SobelShader;
    Material sobelMaterial;

    public override void Create()
    {
        sobelMaterial = CoreUtils.CreateEngineMaterial(SobelShader);

        m_ScriptablePass = new CustomRenderPass(sobelMaterial);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Depth);
            m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_ScriptablePass.SetTarget(renderer.cameraColorTargetHandle,renderer.cameraDepthTargetHandle);

        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(sobelMaterial);
    }
}


