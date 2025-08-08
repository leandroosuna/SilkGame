using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SilkGame.Components.Cameras;
using SilkGame.Components.Input;
using SilkGame.Components.Internal;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
namespace SilkGame
{
    class Program
    {
        private static IWindow window;
        private static GL GL;
        private static IKeyboard primaryKeyboard;
        private static IMouse primaryMouse;
        private static Shader BasicShader;
        private static Shader PostProcessShader;

        private static Model ModelLogo;

        private static Model ModelFloor;
        private static Texture TextureFloor;


        private static Components.Cameras.Camera Camera;

        private static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1600, 900);
            options.Title = "Silk.NET OPENGL";
            options.VSync = true;
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(4, 1));

            window = Window.Create(options);
            
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += OnClose;

            Console.WriteLine("Running game");
            window.Run(); 
            //thread blocked here until the window is closed.
            window.Dispose();
            
        }
        static uint frameBufferAlt;
        static uint TextureColorBuffer;
        private static unsafe void OnLoad()
        {
            window.Center();

            InputHelper.Init(window);
            GL = GL.GetApi(window);
            FullScreenQuad.Init(GL);

            BasicShader = new Shader(GL, "basic-model");
            PostProcessShader = new Shader(GL, "basic-post-process");
            
            ModelFloor = new Model(GL, "Models/plane.obj");
            ModelLogo = new Model(GL, "Models/tgc-logo.fbx");
            TextureFloor = new Texture(GL, "Models/metalfloor.png");

            Camera = new Components.Cameras.Camera(
               position: new Vector3(8.0f, 0.0f, 0.0f),
               yaw: -MathF.PI,
               pitch: 0f,
               fov: MathF.PI * 0.55f,
               nearPlane: 0.1f,
               farPlane: 100f,
               aspectRatio: (float)window.FramebufferSize.X / window.FramebufferSize.Y);

            InputHelper.SetCamera(Camera);

            GL.GenFramebuffers(1, out frameBufferAlt);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferAlt);

            int fbWidth = window.FramebufferSize.X;
            int fbHeight = window.FramebufferSize.Y;
            
            // create color texture (RGBA8)
            GL.GenTextures(1, out TextureColorBuffer);
            GL.BindTexture(TextureTarget.Texture2D, TextureColorBuffer);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                (int)InternalFormat.Rgba8,
                (uint)fbWidth,
                (uint)fbHeight,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                null);
            int texW, texH;
            GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GLEnum.TextureWidth, out texW);
            GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GLEnum.TextureHeight, out texH);
           
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);

            // attach texture to FBO
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureColorBuffer, 0);

            // create and attach a depth+stencil renderbuffer
            GL.GenRenderbuffers(1, out uint rboDepth);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
            GL.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, (uint)fbWidth, (uint)fbHeight);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

            // unbind and tidy
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }
        
        private static unsafe void OnUpdate(double deltaTime)
        {
            InputHelper.Update();
            Camera.Update(deltaTime);

            if (InputHelper.KeyDown(Key.Escape))
                window.Close();

        }
        static float spin = 0;
        static double time = 0;
        
        private static unsafe void OnRender(double deltaTime)
        {
            time += deltaTime;

            if(time % 0.1f < 0.01)
            {
                var fps= (1.0 / deltaTime).ToString("F0");
                window.Title = $"Silk.NET OPENGL - FPS {fps}";
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferAlt);
            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            spin += (float)deltaTime;
            
            BasicShader.SetAsCurrentGLProgram();
            var world = Matrix4x4.CreateScale(10f) * Matrix4x4.CreateFromYawPitchRoll(MathF.PI * 0.5f, -MathF.PI * 0.5f, spin);
            foreach (var mesh in ModelLogo.Meshes)
            {
                mesh.Bind();
                BasicShader.SetUniform("uUseTexture", 0);
                BasicShader.SetUniform("uWorld", world);
                BasicShader.SetUniform("uView", Camera.View);
                BasicShader.SetUniform("uProjection", Camera.Projection);

                BasicShader.SetUniform("uColor", new Vector3(0, .75f, 1f));
                BasicShader.SetUniform("uLightPos", new Vector3(50, 50, 50));
                BasicShader.SetUniform("uViewPos", Camera.Position);

                GL.DrawElements(Silk.NET.OpenGL.PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            }


            world = Matrix4x4.CreateScale(20) * Matrix4x4.CreateTranslation(new Vector3(0, -5, 0));

            foreach (var mesh in ModelFloor.Meshes)
            {
                mesh.Bind();
                BasicShader.SetUniform("uUseTexture", 1);
                BasicShader.SetTextureUniform(TextureFloor, name: "uTex", slot: 0);

                BasicShader.SetUniform("uWorld", world);
                BasicShader.SetUniform("uView", Camera.View);
                BasicShader.SetUniform("uProjection", Camera.Projection);

                BasicShader.SetUniform("uColor", new Vector3(1, 1, 1));
                BasicShader.SetUniform("uLightPos", new Vector3(50, 50, 50));
                BasicShader.SetUniform("uViewPos", Camera.Position);

                GL.DrawElements(Silk.NET.OpenGL.PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            }


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            PostProcessShader.SetAsCurrentGLProgram();
            PostProcessShader.SetUniform("uTime", (float)time);
            PostProcessShader.SetTextureUniform(TextureColorBuffer, "uTexture", 0);
            FullScreenQuad.Draw();
            
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            GL.Viewport(newSize);
        }

        private static void OnClose()
        {
            BasicShader.Dispose();
            PostProcessShader.Dispose();
            ModelLogo.Dispose();
            TextureFloor.Dispose();
        }
    }
}
