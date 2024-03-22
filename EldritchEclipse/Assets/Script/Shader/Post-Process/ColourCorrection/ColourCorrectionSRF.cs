using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColourCorrectionSRF : ScriptableRendererFeature
{
    class ColourCorrectionRenderPass : ScriptableRenderPass
    {
        ColorCorrectionSetting setting;
        Material mat;
        RTHandle textureHandle;
        RenderTextureDescriptor textureDescriptor;
        static readonly int contrastId = Shader.PropertyToID("_Contrast");

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            textureDescriptor.width = cameraTextureDescriptor.width;
            textureDescriptor.height = cameraTextureDescriptor.height;

            RenderingUtils.ReAllocateIfNeeded(ref textureHandle, textureDescriptor);
        }

        private void UpdateShaderSetting()
        {
            if (mat == null)
                return;

            mat.SetFloat(contrastId,setting.Contrast);
            //set material values

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            RTHandle cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

            UpdateShaderSetting();

            Blit(cmd,cameraTargetHandle,textureHandle,mat,0); //(command,camera texture,temp texture,material,pass number)
            Blit(cmd, textureHandle, cameraTargetHandle, mat); // pass the texture back into the camera

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }

        public void Dispose()
        {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Object.Destroy(mat);
            }
            else
            {
                Object.DestroyImmediate(mat);
            }
#else
            Object.Destroy(mat);

#endif
            if (textureHandle != null)
                textureHandle.Release();
        }

        public ColourCorrectionRenderPass(Material material,ColorCorrectionSetting setting)
        {
            mat = material;
            this.setting = setting;
            textureDescriptor = new RenderTextureDescriptor(Screen.width,Screen.height,RenderTextureFormat.Default,0);
        }
    }

    ColourCorrectionRenderPass m_ScriptablePass;
    [SerializeField] ColorCorrectionSetting setting;
    [SerializeField]RenderPassEvent InjectionPoint;
    public Shader shader;
    Material mat;

    public override void Create()
    {
        if (shader == null) return;

        mat = new(shader);

        m_ScriptablePass = new(mat,setting);
        m_ScriptablePass.renderPassEvent = InjectionPoint;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(renderingData.cameraData.cameraType == CameraType.Game) //make it only render in game
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass.Dispose();
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            Destroy(mat);
        }
        else
        {
            DestroyImmediate(mat);
        }
#else
                Destroy(mat);
#endif
    }
}

[System.Serializable]
public class ColorCorrectionSetting
{
    [Range(0, 2f)] public float Contrast;
}


