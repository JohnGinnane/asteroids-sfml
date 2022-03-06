using static asteroids.util;
using SFML.System;
using SFML.Graphics;

namespace asteroids {
    public class asteroid : polybody {
        public enum enumSize {
            small,
            large
        }

        public enumSize size;

        public asteroid(enumSize size) {
            this.noCollideType = typeof(asteroid);
            uint numPoints = 12;
            float radius = 20f;
            this.size = size;

            if (size == enumSize.small)
            {
                numPoints = 8;
                radius = 10f;
            }

            float angOffset = (float)Math.PI/180f * (360f / numPoints);

            VertexArray va = new VertexArray(PrimitiveType.LineStrip, numPoints + 1);
            for (uint i = 0; i < numPoints; i++) {
                Vector2f point = new Vector2f();
                float thisRadius = radius + randfloat(0, radius);

                point.X = (float)Math.Sin(angOffset * i) * thisRadius;
                point.Y = (float)Math.Cos(angOffset * i) * thisRadius;

                va[i] = new Vertex(point, Color.White);
            }

            va[numPoints] = new Vertex(va[0].Position, va[0].Color);

            this.Shape = va;
            this.Drag = 0f;
            this.Velocity = randvec2(-80, 100, -80, 100);

            if (size == enumSize.small) {
                this.Velocity *= 2f;
            }
        }

        public override void update(float delta)
        {
            base.update(delta);

            if (this.Position.X < 0) { this.SetXPosition(Global.ScreenSize.X); }
            if (this.Position.X > Global.ScreenSize.X) { this.SetXPosition(0); }
            if (this.Position.Y < 0) { this.SetYPosition(Global.ScreenSize.Y); }
            if (this.Position.Y > Global.ScreenSize.Y) { this.SetYPosition(0); }
        }

        // if the asteroid is hit by a torpedo it should break up
        public override void resolve(collision c)
        {
            base.resolve(c);
        }
    }
}