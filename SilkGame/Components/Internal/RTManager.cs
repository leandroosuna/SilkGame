using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace SilkGame.Components.Internal
{
    public static class RTManager
    {
        static Dictionary<string, RenderTarget> targets = new Dictionary<string, RenderTarget>();
        static GL GL;
        static Vector2D<int> windowSize;

        public static void Init(GL gl, Vector2D<int> size)
        {
            GL = gl;
            windowSize = size;
        }

        public static void CreateRenderTarget(string name)
        {
            CreateRenderTarget(name, windowSize, ["color"]);
        }
        public static unsafe void CreateRenderTarget(string name, Vector2D<int> size, string[] targetNames)
        {
            var n = name.ToLower();
            if (targets.TryGetValue(n, out var existingRt))
            {
                throw new Exception($"Render target {n} already exists");
            }
            
            RenderTarget rt = new RenderTarget();
            targets[n] = rt;
            rt.ColorTargetCount = (uint)targetNames.Length;

            GL.GenFramebuffers(1, out rt.FrameBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, rt.FrameBuffer);

            var buffers = new Dictionary<string, uint>();

            uint tcb;
            for(int i = 0; i < rt.ColorTargetCount; i++)
            {
                // create color texture (RGBA8)
                GL.GenTextures(1, out tcb);
                buffers.Add(targetNames[i], tcb);

                GL.BindTexture(TextureTarget.Texture2D, tcb);
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    (int)InternalFormat.Rgba8,
                    (uint)size.X,
                    (uint)size.Y,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    null);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, 
                    (FramebufferAttachment)((int)FramebufferAttachment.ColorAttachment0 + i), TextureTarget.Texture2D, tcb, 0);

            }
            rt.TextureColorBuffers = buffers;
            // create and attach a depth+stencil renderbuffer
            GL.GenRenderbuffers(1, out uint rboDepth);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
            GL.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, (uint)size.X, (uint)size.Y);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

            // unbind and tidy
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

        }

        public static void SetAsActive(string name)
        {
            var n = name.ToLower();
            if (n == "screen")
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                return;
            }

            if(targets.TryGetValue(n, out var rt))
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, rt.FrameBuffer);
            else
                throw new Exception($"target {n} not found.");

        }

        public static uint GetTargetTextureID(string rtName, string colorBufferName)
        {
            var n = rtName.ToLower();
            if (!targets.ContainsKey(n))
            {
                throw new Exception($"target {n} not found.");
            }
            var rt = targets[n];
            var cbn = colorBufferName.ToLower();
            if (!rt.TextureColorBuffers.ContainsKey(cbn))
            {
                throw new Exception($"color buffer {cbn} not found.");
            }

            return rt.TextureColorBuffers[cbn];
        }
    }
}
