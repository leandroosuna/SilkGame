using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SilkGame.Components.Cameras;
using SilkGame.Components.Input;
using SilkGame.Components.Internal;
using SilkGame.Components.Internal.RT;
using SilkGame.Components.Internal.Gui;

using System.Numerics;
namespace SilkGame
{
    class Program
    {
        private static IWindow window;
        private static GL GL;
        
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

            libtest.TestLib.Method();
                
            Console.WriteLine("Running game");
            window.Run(); 
            //thread blocked here until the window is closed.
            window.Dispose();
        }

        
        private static unsafe void OnLoad()
        {
            window.Center();
            GL = GL.GetApi(window);

            InputHelper.Init(window);
            GUIManager.Init(GL, window, InputHelper.GetInputContext());
            FullScreenQuad.Init(GL);
            RTManager.Init(GL, window.FramebufferSize);
            //RTManager.SetAsActive("screen");

            BasicShader = new Shader(GL, "mrt-test");
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



            //RTManager.CreateRenderTarget("rt1");
            RTManager.CreateRenderTarget("mrt", ["red", "green", "blue"]);

        }

        private static unsafe void OnUpdate(double deltaTime)
        {
            InputHelper.Update();
            Camera.Update(deltaTime);

            if (InputHelper.KeyDown(Key.Escape))
                window.Close();

            GUIManager.Update(deltaTime);

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

            RTManager.SetAsActive("MRT"); // multiple render target test
            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.PolygonMode(GLEnum.FrontAndBack, PolygonMode.Line);

            spin += (float)deltaTime;
            
            BasicShader.SetAsCurrentGLProgram();
            var world = Matrix4x4.CreateScale(10f) * Matrix4x4.CreateFromYawPitchRoll(MathF.PI * 0.5f, -MathF.PI * 0.5f, spin);

            BasicShader.SetUniform("uUseTexture", 0);
            BasicShader.SetUniform("uWorld", world);
            BasicShader.SetUniform("uView", Camera.View);
            BasicShader.SetUniform("uProjection", Camera.Projection);

            BasicShader.SetUniform("uColor", new Vector3(0, .75f, 1f));
            BasicShader.SetUniform("uLightPos", new Vector3(50, 50, 50));
            BasicShader.SetUniform("uViewPos", Camera.Position);

            //Draw every mesh with the same uniforms
            ModelLogo.DrawMeshes();
            
            
            world = Matrix4x4.CreateScale(20) * Matrix4x4.CreateTranslation(new Vector3(0, -5, 0));

            BasicShader.SetUniform("uUseTexture", 1);
            BasicShader.SetTextureUniform(TextureFloor, name: "uTex", slot: 0);

            BasicShader.SetUniform("uWorld", world);
            BasicShader.SetUniform("uView", Camera.View);
            BasicShader.SetUniform("uProjection", Camera.Projection);

            BasicShader.SetUniform("uColor", new Vector3(1, 1, 1));
            
            //Draw every mesh with a variance
            foreach (var mesh in ModelFloor.Meshes)
            {
                mesh.Draw();
            }
            //GL.PolygonMode(GLEnum.FrontAndBack, PolygonMode.Fill);
            RTManager.SetAsActive("screen");
            GL.ClearDepth(1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            PostProcessShader.SetAsCurrentGLProgram();
            //PostProcessShader.SetUniform("uTime", (float)time);

            var tex = RTManager.GetTargetTextureID("mrt", "red");
            PostProcessShader.SetTextureUniform(tex, "uR", 0);
            tex = RTManager.GetTargetTextureID("mrt", "green");
            PostProcessShader.SetTextureUniform(tex, "uG", 1);
            tex = RTManager.GetTargetTextureID("mrt", "blue");
            PostProcessShader.SetTextureUniform(tex, "uB", 2);


            FullScreenQuad.Draw();
            
            GUIManager.Draw(deltaTime);
            
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
