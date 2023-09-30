using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Materials
{
    public class ClearDepthRenderPass : ScriptableRenderPass
    {
        public ClearDepthRenderPass() {
            renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            Debug.Log("Executing!");
            CommandBuffer cmd = CommandBufferPool.Get("CLEARING DEPTH BUFFER");
            cmd.ClearRenderTarget(true, false, Color.black);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
