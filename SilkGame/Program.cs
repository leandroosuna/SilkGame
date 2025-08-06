using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.Assimp;
using SilkGame.Components.Input;
using SilkGame.Components.Cameras;
namespace SilkGame
{
    class Program
    {
        private static IWindow window;
        private static GL Gl;
        private static IKeyboard primaryKeyboard;
        private static IMouse primaryMouse;
        private static Shader BasicShader;

        private static Model ModelLogo;

        private static Model ModelFloor;
        private static Texture TextureFloor;


        private static Components.Cameras.Camera Camera;

        private static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1600, 900);
            options.Title = "SILK.NET OPENGL";
            options.VSync = true;
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(4, 1));

            window = Window.Create(options);
            
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.FramebufferResize += OnFramebufferResize;
            window.Closing += OnClose;
            
            window.Run();
            window.Dispose();

        }

        private static void OnLoad()
        {
            window.Center();

            InputHelper.Init(window);
            
            Gl = GL.GetApi(window);

            BasicShader = new Shader(Gl, "basic-model");
            ModelFloor = new Model(Gl, "Models/plane.obj");
            ModelLogo = new Model(Gl, "Models/tgc-logo.fbx");
            TextureFloor = new Texture(Gl, "Models/metalfloor.png");

            Camera = new Components.Cameras.Camera(
               position: new Vector3(8.0f, 0.0f, 0.0f),
               yaw: -MathF.PI,
               pitch: 0f,
               fov: MathF.PI * 0.55f,
               nearPlane: 0.1f,
               farPlane: 100f,
               aspectRatio: (float)window.FramebufferSize.X / window.FramebufferSize.Y);

            InputHelper.SetCamera(Camera);
        }

        private static unsafe void OnUpdate(double deltaTime)
        {
            InputHelper.Update();
            Camera.Update(deltaTime);

            if (InputHelper.KeyDown(Key.Escape))
                window.Close();

        }
        static float spin = 0;
        private static unsafe void OnRender(double deltaTime)
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.ClearDepth(1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            spin += (float)deltaTime;

            var world = Matrix4x4.CreateScale(10f) * Matrix4x4.CreateFromYawPitchRoll(MathF.PI * 0.5f, -MathF.PI * 0.5f, spin);
            foreach (var mesh in ModelLogo.Meshes)
            {
                mesh.Bind();
                BasicShader.SetAsCurrentGLProgram();
                BasicShader.SetUniform("uUseTexture", 0);
                BasicShader.SetUniform("uWorld", world);
                BasicShader.SetUniform("uView", Camera.View);
                BasicShader.SetUniform("uProjection", Camera.Projection);

                BasicShader.SetUniform("uColor", new Vector3(0, 0.75f, 1f));
                BasicShader.SetUniform("uLightPos", new Vector3(50, 50, 50));
                BasicShader.SetUniform("uViewPos", Camera.Position);

                Gl.DrawElements(Silk.NET.OpenGL.PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            }


            world = Matrix4x4.CreateScale(20) * Matrix4x4.CreateTranslation(new Vector3(0,-5,0));
            
            foreach (var mesh in ModelFloor.Meshes)
            {
                mesh.Bind();
                BasicShader.SetAsCurrentGLProgram();
                BasicShader.SetUniform("uUseTexture", 1);
                TextureFloor.Bind();
                BasicShader.SetUniform("uTex", 0);

                BasicShader.SetUniform("uWorld", world);
                BasicShader.SetUniform("uView", Camera.View);
                BasicShader.SetUniform("uProjection", Camera.Projection);

                BasicShader.SetUniform("uColor", new Vector3(1, 1, 1f));
                BasicShader.SetUniform("uLightPos", new Vector3(50, 50, 50));
                BasicShader.SetUniform("uViewPos", Camera.Position);
                
                Gl.DrawElements(Silk.NET.OpenGL.PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            }
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            Gl.Viewport(newSize);
        }

        private static void OnClose()
        {
            BasicShader.Dispose();
            ModelLogo.Dispose();
            TextureFloor.Dispose();
        }

        
    }
}
