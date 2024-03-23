using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class SharpenRendererFeature : ScriptableRendererFeature
{
    SharpenRenderPass pass;
    Material mat;

    //exposed to be tweaked in the renderer settings
    [SerializeField] Shader shader;
    [SerializeField] SharpenSetting settings;

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

    class SharpenRenderPass : ScriptableRenderPass
    {
        ProfilingSampler profileSampler;
        SharpenSetting settings;
        Material mat;
        RTHandle tempTexture;
        RenderTextureDescriptor tempTextDesc;

        //DECLARE ANY VARIABLES YOU NEED HERE

        #region HELPER METHODS
        public bool SetUp(ref Material material, ref SharpenSetting setting) //setup the render pass with all the data
        {
            mat = material;
            settings = setting;

            ConfigureInput(settings.Requirements);
            tempTextDesc = new(Screen.width, Screen.height, RenderTextureFormat.RGB111110Float, 0);
            UpdateShaderSettings();
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
            mat.SetFloat("_Sharpness", settings.Sharpen);


        }
        #endregion

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //assign the correct size to the texture descriptor
            tempTextDesc.width = cameraTextureDescriptor.width;
            tempTextDesc.height = cameraTextureDescriptor.height;

            //re allocate the texture and assign a name so it can be identified in frame debugger / memory profiler
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, tempTextDesc, name: "_SHARPEN_BLIT");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //uncomment this if u want the effect to take place only in game
            //if (cameraData.camera.cameraType != CameraType.Game) return;

            if (mat == null) return; //another material check

            CommandBuffer cmd = CommandBufferPool.Get(); // get a command buffer from the pool
            RTHandle cameraTexture = renderingData.cameraData.renderer.cameraColorTargetHandle; //get the camera texture
            mat.SetVector("_TextureSize",new Vector2(tempTextDesc.width, tempTextDesc.height));
            
            //assigns ator2("_ScreenSize") identification scope so it can be identified in the frame debugger
            using (new ProfilingScope(cmd, profileSampler))
            {
                Blitter.BlitCameraTexture(cmd, cameraTexture, tempTexture, mat, 0); //copies the camera texture into our temporary texture and applies our shader on it
                Blitter.BlitCameraTexture(cmd, tempTexture, cameraTexture); //copies the texture back into the camera to be displayed
            }

            context.ExecuteCommandBuffer(cmd); //execute the shader
            cmd.Clear();
            CommandBufferPool.Release(cmd); //release the command buffer
        }
    }
}

[System.Serializable]
public class SharpenSetting
{
    public RenderPassEvent InjectionPoint; //this is where the shader will be injected for post-processing
    public ScriptableRenderPassInput Requirements; //this is the buffer the pass requires
    public string ProfilerName = "SHARPEN_BLIT";

    //put your settings here
    public float Sharpen;
    
}
