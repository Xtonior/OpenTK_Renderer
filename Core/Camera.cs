using System;
using OpenTK.Mathematics;

namespace Engine.Core
{
    public class Camera
    {
        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        public Vector3 Position { get; set; }

        public float AspectRatio { private get; set; }

        public Vector3 Front = -Vector3.UnitX;

        public Vector3 Up = Vector3.UnitY;

        public Vector3 Right = Vector3.UnitZ;

        public Matrix3 Rotation;

        public float Pitch;
        public float Yaw = -MathHelper.PiOver2;

        public float PitchDeg { get => MathHelper.RadiansToDegrees(Pitch); }
        public float YawDeg { get => MathHelper.RadiansToDegrees(Yaw); }

        public float Fov = MathHelper.PiOver2;

        public void UpdateVectors()
        {
            float p = MathHelper.Clamp(PitchDeg, -89.0f, 89.0f);
            Pitch = MathHelper.DegreesToRadians(p);

            Rotation = Matrix3.CreateRotationY(Pitch) * Matrix3.CreateRotationZ(Yaw);

            Front.X = Rotation.M11;
            Front.Y = Rotation.M12; 
            Front.Z = Rotation.M13; 

            Front = Vector3.Normalize(Front);

            Right = Vector3.Normalize(Vector3.Cross(Front, -Vector3.UnitZ));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}