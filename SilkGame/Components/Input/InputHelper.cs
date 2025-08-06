using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SilkGame.Components.Cameras;

namespace SilkGame.Components.Input
{
    public static class InputHelper
    {
        static IWindow window;
        static IInputContext input;
        static IKeyboard keyboard;
        static IMouse mouse;
        static Vector2 lastMousePosition;
        static Camera camera;

        public static float MouseSensitivity = .001f;
        public static Vector2 MouseDelta = Vector2.Zero;
        public static float WheelValue = 0;
        public static void Init(IWindow context)
        {
            window = context;
            input = window.CreateInput();
           
            //TODO: handle all mice/keyboard inputs
            keyboard = input.Keyboards.FirstOrDefault();
            //for (int i = 0; i < input.Mice.Count; i++)
            //{
            //    input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
            //    input.Mice[i].MouseMove += OnMouseMove;
            //    input.Mice[i].Scroll += OnMouseWheel;
            //}
            mouse = input.Mice.FirstOrDefault();
            mouse.Cursor.CursorMode = CursorMode.Raw;
            mouse.Position = (Vector2)window.Position + (Vector2)window.Size / 2;

            lastMousePosition = mouse.Position;
            MouseDelta = Vector2.Zero;

        }
        public static void SetCamera(Camera cam)
        {
            camera = cam;
        }
        public static void SetMouseMode(CursorMode mode)
        {
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = mode;
            }
        }
       

        public static void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            WheelValue = Math.Clamp(WheelValue - scrollWheel.Y, 1.0f, 45f);
            if (camera != null)
            {
                camera.Update(WheelValue);
            }
        }

        static List<Key> keysDown = new List<Key>();
        public static void Update()
        {
            keysDown.RemoveAll(k => !keyboard.IsKeyPressed(k));

            MouseDelta.X = (float)(mouse.Position.X - lastMousePosition.X) * MouseSensitivity;
            MouseDelta.Y = (float)(mouse.Position.Y - lastMousePosition.Y) * MouseSensitivity;
            lastMousePosition = mouse.Position;
           
        }
        public static bool KeyDown(Key key)
        {
            return keyboard.IsKeyPressed(key);
        }

        public static bool KeyDownOnce(Key key)
        {
            if(keyboard.IsKeyPressed(key) && !keysDown.Contains(key))
            {
                keysDown.Add(key);
                return true;
            }
            return false;
        }
    }
}
