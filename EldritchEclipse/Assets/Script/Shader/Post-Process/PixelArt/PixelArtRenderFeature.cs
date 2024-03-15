using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class PixelArtRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    Material material;

    class PixelArtRenderPass : ScriptableRenderPass
    {
        [SerializeField]
        Material material;
        RTHandle tempTexture, sourceTexture;

        public PixelArtRenderPass(Material material) : base()
        {
            this.material = material;
        }


        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            sourceTexture = renderingData.cameraData.renderer.cameraColorTargetHandle; //pull texture from the camera
            tempTexture = RTHandles.Alloc(new("_TempTexture"),name : "_TempTexture"); //allocate memory to this texture
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer buffer = CommandBufferPool.Get("PixelArtRenderFeature"); //assign a name to this buffer
            RenderTextureDescriptor targetDescriptor = renderingData.cameraData.cameraTargetDescriptor; //get the information to create a temporary render texture
            targetDescriptor.depthBufferBits = 0;

            buffer.GetTemporaryRT(Shader.PropertyToID(tempTexture.name),targetDescriptor,FilterMode.Point); //creates a temp render texture

            Blit(buffer, sourceTexture, tempTexture, material); //copy texture data from the camera into our temp texture
            Blit(buffer, tempTexture, sourceTexture); //copy the texture data back to the camera after processing

            context.ExecuteCommandBuffer(buffer); //execute the commands
            CommandBufferPool.Release(buffer); //release the buffer

        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            tempTexture.Release(); //release the temp texture memory
        }
    }

    PixelArtRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new(material);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing; //make sure this processing pass is before post-processing
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass); //queue the pass into the renderer
    }
}


