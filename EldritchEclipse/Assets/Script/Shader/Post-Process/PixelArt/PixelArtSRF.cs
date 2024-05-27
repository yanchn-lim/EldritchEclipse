using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;

//THIS IS A TEMPLATE MADE FROM READING UNITY DOCS AND RENDERER FEATURES SCRIPTS
//FOR USE IN URP ONLY!
internal class PixelArtRendererFeature : ScriptableRendererFeature
{
    PixelArtRenderPass pass;

    [SerializeField] PixelArtSetting settings;

    #region RENDERER METHODS
    public override void Create()
    {
        if (pass == null)
            pass = new();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        pass.SetUp(ref settings);
        renderer.EnqueuePass(pass);
    }

    protected override void Dispose(bool disposing)
    {
        pass = null;
    }

    #endregion

    class PixelArtRenderPass : ScriptableRenderPass
    {
        ProfilingSampler profileSampler;
        PixelArtSetting settings;
        RTHandle tempTexture;
        RenderTextureDescriptor tempTextDesc;

        //DECLARE ANY VARIABLES YOU NEED HERE

        #region HELPER METHODS
        public void SetUp(ref PixelArtSetting setting) //setup the render pass with all the data
        {
            settings = setting;

            ConfigureInput(settings.Requirements);
            tempTextDesc = new(Screen.width, Screen.height, RenderTextureFormat.RGB111110Float, 0);
            profileSampler = new(settings.ProfilerName); //assign a name to the profiler to be identified in frame debugger
            renderPassEvent = settings.InjectionPoint;
        }
        #endregion

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //assign the correct size to the texture descriptor
            int height = (int)math.pow(2, settings.Steps);
            tempTextDesc.width = cameraTextureDescriptor.width / settings.Steps ;
            tempTextDesc.height = cameraTextureDescriptor.height / settings.Steps;

            //re allocate the texture and assign a name so it can be identified in frame debugger / memory profiler
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, tempTextDesc,FilterMode.Point,TextureWrapMode.Clamp, name: "_PIXEL_ART");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //uncomment this if u want the effect to take place only in game
            //if (cameraData.camera.cameraType != CameraType.Game) return;

            CommandBuffer cmd = CommandBufferPool.Get(); // get a command buffer from the pool
            RTHandle cameraTexture = renderingData.cameraData.renderer.cameraColorTargetHandle; //get the camera texture

            //assigns a identification scope so it can be identified in the frame debugger
            using (new ProfilingScope(cmd, profileSampler))
            {
                Blitter.BlitCameraTexture(cmd, cameraTexture, tempTexture); //copies the camera texture into our temporary texture and applies our shader on it
                Blitter.BlitCameraTexture(cmd, tempTexture, cameraTexture); //copies the texture back into the camera to be displayed
            }

            context.ExecuteCommandBuffer(cmd); //execute the shader
            cmd.Clear();
            CommandBufferPool.Release(cmd); //release the command buffer
        }
    }
}

[System.Serializable]
internal class PixelArtSetting
{
    public RenderPassEvent InjectionPoint; //this is where the shader will be injected for post-processing
    public ScriptableRenderPassInput Requirements; //this is the buffer the pass requires
    public string ProfilerName = "PIXEL_ART_BLIT";

    //put your settings here
    [Range(1,32)]public int Steps;
}
