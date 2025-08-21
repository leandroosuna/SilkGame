using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SilkGame.Components.Internal.Gui
{
    internal static class GUIManager
    {
        static ImGuiController controller;
        static GL GL;
        const string fontPath = "CascadiaMono.ttf";
        const float fontSize = 18;
        static ImFontPtr font;
        public static void Init(GL gl, IWindow window, IInputContext input)
        {
            GL = gl;

            controller = new ImGuiController(gl, window, input);

            LoadCustomFont();
            
        }

        static unsafe void LoadCustomFont()
        {
            var io = ImGui.GetIO();
            io.Fonts.AddFontFromFileTTF(fontPath, fontSize);

            io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out int width, out int height, out int bytesPerPixel);

            // Upload to GL
            uint fontTex;
            GL.GenTextures(1, out fontTex);
            GL.BindTexture(TextureTarget.Texture2D, fontTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                          (uint)width, (uint)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            io.Fonts.SetTexID((nint)fontTex);
            io.Fonts.ClearTexData();

            font = io.Fonts.Fonts[1];
        }
        
        
        
        public static void Update(double delta)
        {
            controller.Update((float)delta);
            ImGui.PushFont(font);
        }
        public static unsafe void Draw(double delta)
        {

            ImGui.Begin("test",ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
            
            ImGui.Text("text");
            //ImGui.GetStyle();
            if (ImGui.Button("a"))
            {
                Console.WriteLine("btn a");
            }
                //ImGuiNET.ImGui.ShowDemoWindow();

            controller.Render();
        }
        
    }
}
