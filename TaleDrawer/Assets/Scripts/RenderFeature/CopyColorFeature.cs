using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class CopyColorFeature : ScriptableRendererFeature
{
    class CopyPass : ScriptableRenderPass
    {
        class PassData
        {
            public TextureHandle source;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resources = frameData.Get<UniversalResourceData>();

            using var builder = renderGraph.AddRasterRenderPass<PassData>(
                "Expose Scene Color",
                out var passData);

            // URP 2D usa cameraColor
            passData.source = resources.cameraColor;

            builder.UseTexture(passData.source);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                context.cmd.SetGlobalTexture("_SceneColorTexture", data.source);
            });
        }
    }

    CopyPass pass;

    public override void Create()
    {
        pass = new CopyPass();

        pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}