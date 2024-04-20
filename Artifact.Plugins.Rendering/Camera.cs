using Artifact.Plugins.Windowing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public enum MatrixMode
    {
        Perspective,
        Orthographic
    }

    public static class Camera
    {
        public static MatrixMode Mode { get; set; } = MatrixMode.Orthographic;

        public static Vector3 Position { get; set; } = Vector3.Zero;

        public static float orthoWidth => 1.0f * (1280f / 720f);
        public static float orthoHeight => 1.0f;

        public static float perspectiveAspectRatio = 16f / 9f;

        public static float nearPlane = 0.1f;
        public static float farPlane = 100f;

        public static Matrix4x4 ProjectionMatrix { get; set; }
        public static Matrix4x4 ViewMatrix { get; set; }

        public static void UpdateMatrices()
        {
            switch (Mode)
            {
                case MatrixMode.Orthographic:
                    ProjectionMatrix = Matrix4x4.CreateOrthographic(orthoWidth, orthoHeight, nearPlane, farPlane);
                    ViewMatrix = Matrix4x4.CreateTranslation(Position);
                    break;
                case MatrixMode.Perspective:
                    break;
            }
        }
    }
}
