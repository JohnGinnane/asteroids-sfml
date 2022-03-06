using static asteroids.util;
using SFML.System;
using SFML.Graphics;

namespace asteroids {
    public class asteroid : polybody {
        public enum enumSize {
            small,
            medium,
            large
        }

        private int points;
        public int Points { get { return points; } }

        public enumSize size;

        public asteroid(enumSize size) {
            this.noCollideType = typeof(asteroid);
            uint numPoints = 8;
            float radius = 10f;
            float velMulti = 2f;
            this.size = size;

            switch (size) {
                case enumSize.small:
                    numPoints = 8;
                    radius = 8f;
                    velMulti = 2f;
                    this.points = 100;
                    break;
                case enumSize.large:
                    numPoints = 12;
                    radius = 16f;
                    velMulti = 1f;
                    this.points = 20;
                    break;
                case enumSize.medium:
                default:
                    numPoints = 10;
                    radius = 12f;
                    velMulti = 1.5f;
                    this.points = 50;
                    break;
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
            this.Velocity = randvec2(-1, 1) * randfloat(40, 60) * velMulti;
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