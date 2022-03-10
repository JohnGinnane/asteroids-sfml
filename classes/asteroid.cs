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
                    radius = 20f;
                    velMulti = 1f;
                    this.points = 20;
                    break;
                case enumSize.medium:
                default:
                    numPoints = 10;
                    radius = 14f;
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
            this.Velocity = randvec2(-1, 1) * randfloat(60, 80) * velMulti;
        }

        public override void update(float delta)
        {
            base.update(delta);

            if (this.Position.X < 0) { this.SetXPosition(Global.ScreenSize.X); }
            if (this.Position.X > Global.ScreenSize.X) { this.SetXPosition(0); }
            if (this.Position.Y < 0) { this.SetYPosition(Global.ScreenSize.Y); }
            if (this.Position.Y > Global.ScreenSize.Y) { this.SetYPosition(0); }
        }

        public List<asteroid> breakup(int numNewAsteroids) {
            List<asteroid> newAsteroids = new List<asteroid>();

            // larger asteroids break into smaller ones
            string bangSound = "bangMedium";
            
            switch (this.size) {
                case asteroid.enumSize.small:
                    bangSound = "bangSmall";
                    break;
                case asteroid.enumSize.medium:
                    bangSound = "bangMedium";
                    break;
                case asteroid.enumSize.large:
                    bangSound = "bangLarge";
                    break;
            }

            Global.sfx[bangSound].play();

            if (this.size > enumSize.small) {
                for (int i = 0; i < numNewAsteroids; i++) {
                    asteroid newAsteroid = new asteroid((enumSize)((int)this.size - 1));
                    newAsteroid.Position = this.Position + randvec2(-this.BoundingCircleRadius, this.BoundingCircleRadius);
                    newAsteroids.Add(newAsteroid);
                }
            }

            return newAsteroids;
        }

        public static List<asteroid> spawnAsteroids(int numAsteroids) {
            List<asteroid> newAsteroids = new List<asteroid>();

            // Spawn them on the edge of the screen
            for (int i = numAsteroids; i > 0; i--) {
                asteroid a = new asteroid(enumSize.large);
                int side = (int)Math.Round(randfloat(0f, 1f));
                if (side == 0) {
                    a.Position = new Vector2f(randint(0, 1) * Global.ScreenSize.X,
                                              randfloat(0, Global.ScreenSize.Y));
                } else {
                    a.Position = new Vector2f(randfloat(0, Global.ScreenSize.X),
                                              randint(0, 1) * Global.ScreenSize.Y);
                }
                
                newAsteroids.Add(a);
            }

            return newAsteroids;
        }

        // if the asteroid is hit by a torpedo it should break up
        public override void resolve(collision c)
        {
            base.resolve(c);
        }
    }
}