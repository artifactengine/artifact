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
    public class PlayerBat : Component
    {
        private InputPlugin input;

        private const float SPEED = 400f;

        private Ball ball;

        public PlayerBat(Ball ball)
        {
            this.ball = ball;
        }

        public override void OnLoad()
        {
            base.OnLoad();

            input = Application.current.GetPlugin<InputPlugin>();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (input.IsKeyDown(Key.Up) || input.IsKeyDown(Key.W))
            {
                Transform.Position += new Vector3(0, SPEED * dt, 0);
            }
            if (input.IsKeyDown(Key.Down) || input.IsKeyDown(Key.S))
            {
                Transform.Position -= new Vector3(0, SPEED * dt, 0);
            }

            ball.PaddleRightRect = new RectangleF(Transform.Position.X - (Transform.Scale.X / 2), Transform.Position.Y - (Transform.Scale.Y / 2), Transform.Scale.X, Transform.Scale.Y);
        }
    }
}
