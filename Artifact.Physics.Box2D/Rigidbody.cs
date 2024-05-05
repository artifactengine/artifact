using Artifact.Plugins.ECS;
using Artifact.Plugins.Rendering;
using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Common;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Physics.Box2D
{
    public enum RigidbodyType
    {
        Static,
        Dynamic
    }

    public class Rigidbody : Component
    {
        private Body body;

        public RigidbodyType Type { get; set; }
        public Vector2 Velocity
        {
            get => body.GetLinearVelocity();
            set => body.SetLinearVelocity(value);
        }

        private Vector2 boxSize;

        private RenderingPlugin renderer;

        public Rigidbody(RigidbodyType type)
        {
            Type = type;
            

            renderer = Application.current.GetPlugin<RenderingPlugin>();
        }

        public override void OnLoad()
        {
            base.OnLoad();

            BodyDef bodyDef = new BodyDef();
            bodyDef.type = Type == RigidbodyType.Dynamic ? BodyType.Dynamic : BodyType.Static;
            bodyDef.position = new Vector2((Transform.Position.X - Transform.Scale.X / 2.0f), (Transform.Position.Y - Transform.Scale.Y / 2.0f));
            bodyDef.bullet = true;

            body = PhysicsPlugin.world.CreateBody(bodyDef);

            PolygonShape shape = new PolygonShape();
            boxSize = new Vector2(Transform.Scale.X, Transform.Scale.Y);
            shape.SetAsBox(Transform.Scale.X * 4.0f, Transform.Scale.Y * 4.0f);

            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = 1.0f;
            fixtureDef.friction = 0.3f;

            body.CreateFixture(in fixtureDef);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (body.Type() == BodyType.Dynamic)
            {
                Transform.Position = new Vector3(body.Position.X + (Transform.Scale.X / 2.0f), body.Position.Y + (Transform.Scale.X / 2.0f), -5);
            }
            
        }
    }
}