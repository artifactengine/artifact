using Artifact;
using Artifact.Plugins.ECS;
using Artifact.Plugins.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace Pong.Components
{
    public class BotBat : Component
    {
        private InputPlugin input;

        private const float SPEED = 600f;

        private Ball ball;

        public BotBat(Ball ball)
        {
            this.ball = ball;
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (Transform.Position.Y > ball.Transform.Position.Y)
            {
                Transform.Position -= new Vector3(0, SPEED * dt, 0);
            }
            if (Transform.Position.Y < ball.Transform.Position.Y)
            {
                Transform.Position += new Vector3(0, SPEED * dt, 0);
            }

            ball.PaddleLeftRect = new RectangleF(Transform.Position.X - (Transform.Scale.X / 2), Transform.Position.Y - (Transform.Scale.Y / 2), Transform.Scale.X, Transform.Scale.Y);
        }
    }
}
