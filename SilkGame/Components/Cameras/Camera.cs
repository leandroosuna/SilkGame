using Silk.NET.Input;
using Silk.NET.Windowing;
using SilkGame.Components.Input;
using System.Numerics;

namespace SilkGame.Components.Cameras
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Front;
        public Vector3 Up;
        public Vector3 Right;
        public float Yaw;
        public float Pitch;
        public float FOV;
        public float AspectRatio;
        public float NearPlane;
        public float FarPlane;
        public float MoveSpeed = 10f;
        public Matrix4x4 View;
        public Matrix4x4 Projection;

        Vector3 tempFront;

        public Camera(Vector3 position, float yaw, float pitch, float fov, float nearPlane, float farPlane, float aspectRatio)
        {
            Position = position;
            Yaw = yaw;
            Pitch = pitch;
            FOV = fov;
            NearPlane = nearPlane;
            FarPlane = farPlane;
            AspectRatio = aspectRatio;
            Up = Vector3.UnitY;
            CalculateVectors();
            CalculateView();
            CalculateProjection();
        }
        void CalculateVectors()
        {
            tempFront.X = MathF.Cos(Yaw) * MathF.Cos(Pitch);
            tempFront.Y = MathF.Sin(Pitch);
            tempFront.Z = MathF.Sin(Yaw) * MathF.Cos(Pitch);

            Front = Vector3.Normalize(tempFront);

            var flatFront = new Vector3(tempFront.X, 0, tempFront.Z);

            flatFront = Vector3.Normalize(flatFront);
            
            Right = Vector3.Normalize(Vector3.Cross(flatFront, Up));
        }
        void CalculateView()
        {
            View = Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }
        void CalculateProjection()
        {
            Projection = Matrix4x4.CreatePerspectiveFieldOfView(FOV, AspectRatio, NearPlane, FarPlane);
        }
        public void Update(Vector2 mouseDelta)
        {
            Yaw += mouseDelta.X;
            Pitch += mouseDelta.Y;

            if (Pitch > MathF.PI)
                Pitch = MathF.PI;
            else if(Pitch < 0)
                Pitch = 0;

            CalculateVectors();
            CalculateView();
            
        }
        public void Update(double deltaTime)
        {
            var mouseDelta = InputHelper.MouseDelta;

            var updateView = false;
            if(mouseDelta != Vector2.Zero)
            {
                Yaw += mouseDelta.X;
                Pitch -= mouseDelta.Y;
                Pitch = Math.Clamp(Pitch, -((MathF.PI * 0.5f) - 0.05f), (MathF.PI * 0.5f) - 0.05f);
               
                CalculateVectors();
                updateView = true;
            }

            var dir = Vector3.Zero;
            if(InputHelper.KeyDown(Key.W))
            {
                dir += Front;    
            }
            if (InputHelper.KeyDown(Key.S))
            {
                dir -= Front;    
            }
            if (InputHelper.KeyDown(Key.A))
            {
                dir -= Right;
            }
            if (InputHelper.KeyDown(Key.D))
            {
                dir += Right;
            }
            if (InputHelper.KeyDown(Key.Space))
            {
                dir += Up;
            }
            if (InputHelper.KeyDown(Key.ControlLeft))
            {
                dir -= Up;
            }

            if (dir != Vector3.Zero)
            {
                dir = Vector3.Normalize(dir);

                Position += dir * MoveSpeed * (float)deltaTime;
                updateView = true;
            }
            if(updateView)
                CalculateView();
        }
        public void Update(float mouseWheel)
        {

        }
        
        

    }
}
