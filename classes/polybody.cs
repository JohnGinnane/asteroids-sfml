using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace asteroids {
    public class polybody : body {
        private float angle = 0f;
        public float Angle {
            get { return angle; }
            set {
                angle = value;
                if (angle < 0) { angle = (float)Math.PI * 2f; }
                if (angle > (float)Math.PI * 2) { angle = 0; }
            }
        }

        private float angularVelocity = 0f;
        public float AnglularVelocity {
            get { return angularVelocity; }
            set { angularVelocity = value; }
        }

        public polybody(VertexArray va) {
            this.Shape = va;
        }

        public override void update(float delta)
        {
            if (!isStatic) {
                this.Angle += this.AnglularVelocity * delta;
                this.AnglularVelocity *= (1f - this.Drag * delta);
            }

            base.update(delta);
        }

        public override void draw(RenderWindow window) {            
            VertexArray ogva = (VertexArray)shape;
            VertexArray va = new VertexArray(ogva);
            for (uint i = 0; i < va.VertexCount; i++) {
                if (angle == 0) {
                    va[i] = new Vertex(this.Position + ogva[i].Position, ogva[i].Color);
                } else {
                    float s = (float)Math.Sin(this.Angle);
                    float c = (float)Math.Cos(this.Angle);

                    float x2 = va[i].Position.X * c - va[i].Position.Y * s;
                    float y2 = va[i].Position.X * s + va[i].Position.Y * c;

                    va[i] = new Vertex(this.Position + new Vector2f(x2, y2), va[i].Color);
                }
            }
            
            window.Draw(va, RenderStates.Default);
        }
    }
}