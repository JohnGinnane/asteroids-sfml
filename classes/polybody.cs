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

        private float angularDrag = 0.1f;
        public float AngularDrag {
            get { return angularDrag; }
            set { angularDrag = value; }
        }
        public polybody() {
            VertexArray va = new VertexArray(PrimitiveType.LineStrip, 4);
            for (uint i = 0; i < 4; i++) {
                float radius = 20f;
                Vector2f point = new Vector2f();
                point.X = (float)Math.Sin(Math.PI/180f * 120 * i) * radius;
                point.Y = (float)Math.Cos(Math.PI/180f * 120 * i) * radius;

                va[i] = new Vertex(this.Position + point);
            }
            this.Shape = va;
            this.OutlineColour = Color.White;
        }

        public polybody(VertexArray va) {
            this.Shape = va;
        }

        public override void update(float delta)
        {
            if (!isStatic) {
                this.Angle += this.AnglularVelocity * delta;
                if (this.AngularDrag != 0) {
                    this.AnglularVelocity *= (1f - this.AngularDrag * delta);
                }
            }

            base.update(delta);
        }

        public override void draw(RenderWindow window) {    
            if (this.Shape == null) { return; }
            if (this.shapeType  != typeof(VertexArray)) { return; }
                    
            VertexArray ogva = (VertexArray)Shape;
            VertexArray va = new VertexArray(ogva);
            for (uint i = 0; i < va.VertexCount; i++) {
                if (angle == 0) {
                    va[i] = new Vertex(this.Position + ogva[i].Position);
                } else {
                    float s = (float)Math.Sin(this.Angle);
                    float c = (float)Math.Cos(this.Angle);

                    float x2 = va[i].Position.X * c - va[i].Position.Y * s;
                    float y2 = va[i].Position.X * s + va[i].Position.Y * c;

                    va[i] = new Vertex(this.Position + new Vector2f(x2, y2), this.OutlineColour);
                }
            }
            
            window.Draw(va, RenderStates.Default);

            base.draw(window);
        }
    }
}