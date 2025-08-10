using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkGame.Components.Internal
{
    public class RenderTarget
    {
        
        public uint FrameBuffer;
        public Dictionary<string, uint> TextureColorBuffers;
        public uint ColorTargetCount;
        public RenderTarget()
        {
        
        }
    }
}
