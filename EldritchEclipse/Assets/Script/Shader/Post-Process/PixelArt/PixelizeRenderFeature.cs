using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelizeRenderFeature : ScriptableRendererFeature
{
    class PixelizePass : ScriptableRenderPass
    {
        CustomPassSettings settings;
        RTHandle colorBuffer, pixelBuffer;
        int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

        Material material;
        int pixelScreenHeight, pixelScreenWidth;

        public PixelizePass(CustomPassSettings settings)
        {
            this.settings = settings;
            renderPassEvent = settings.renderPassEvent;
            if (material == null)
                material = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

            pixelScreenHeight = settings.screenHeight;
            pixelScreenWidth = (int)(pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

            material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
            material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
            material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

            descriptor.height = pixelScreenHeight;
            descriptor.width = pixelScreenWidth;

            cmd.GetTemporaryRT(pixelBufferID,descriptor,FilterMode.Point);
            pixelBuffer = RTHandles.Alloc(
                        new RenderTargetIdentifier(pixelBufferID));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new("Pixelize Pass")))
            {
                Blit(cmd, colorBuffer, pixelBuffer, material);
                //Blit(cmd, pixelBuffer, colorBuffer);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) throw new System.ArgumentNullException("cmd");

            cmd.ReleaseTemporaryRT(pixelBufferID);
        }
    }

    #region RENDER FEATURE
    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public int screenHeight = 144;
    }

    [SerializeField]
    CustomPassSettings settings;
    PixelizePass customPass;

    /// <inheritdoc/>
    public override void Create()
    {
        customPass = new(settings);

        // Configures where the render pass should be injected.
        customPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(customPass);

    }
    #endregion
}


