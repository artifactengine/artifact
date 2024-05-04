using Artifact.Plugins.ECS;
using FreeTypeSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Pong.Components
{
    public class Ball : Component
    {
        private const float SPEED = 1500f;

        private Vector3 velocity = new Vector3(SPEED * 1.2f, SPEED / 2.0f, 0);

        public RectangleF PaddleRightRect { get; set; } = new RectangleF();
        public RectangleF PaddleLeftRect { get; set; } = new RectangleF();
        public RectangleF BallRect { get; set; } = new RectangleF();

        public override void OnLoad()
        {
            Transform.Position = new Vector3(0, -500, 0);
            base.OnLoad();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            BallRect = new RectangleF(Transform.Position.X - (Transform.Scale.X / 2), Transform.Position.Y - (Transform.Scale.Y / 2), Transform.Scale.X, Transform.Scale.Y);

            Transform.Position += velocity * dt;

            if (Transform.Position.X > 2300)
            {
                velocity = new Vector3(Vector2.Reflect(new Vector2(velocity.X, velocity.Y), new Vector2(-1, 0)), 0);
                Transform.Position += new Vector3(-50, 0, 0);
            } else if (Transform.Position.X < -2300)
            {
                velocity = new Vector3(Vector2.Reflect(new Vector2(velocity.X, velocity.Y), new Vector2(1, 0)), 0);
                Transform.Position += new Vector3(50, 0, 0);
            }

            if (Transform.Position.Y > 360)
            {
                velocity = new Vector3(Vector2.Reflect(new Vector2(velocity.X, velocity.Y), new Vector2(0, -1)), 0);
                Transform.Position += new Vector3(0, -50, 0);

            }

            if (Transform.Position.Y < -360)
            {
                velocity = new Vector3(Vector2.Reflect(new Vector2(velocity.X, velocity.Y), new Vector2(0, 1)), 0);
                Transform.Position += new Vector3(0, 50, 0);

            }


            if (BallRect.IntersectsWith(PaddleRightRect))
            {
                velocity = new Vector3(Vector2.Reflect(new Vector2(velocity.X, velocity.Y), new Vector2(-1, 0)), 0);
                Transform.Position += new Vector3(5, 0, 0);
            }

            if (BallRect.IntersectsWith(PaddleLeftRect))
            {
                velocity = new Vector3(Vector2.Reflect(new Vector2(velocity.X, velocity.Y), new Vector2(1, 0)), 0);
                Transform.Position += new Vector3(5, 0, 0);
            }
        }
    }
}
