using static asteroids.util;
using SFML.System;
using SFML.Graphics;

namespace asteroids {
    public class asteroid : circlebody {
        private bool spawning;
        
        public asteroid() {
            //new(radius, position, colour, mass);
            
            this.spawning = true;
            Vector2f spawnDir = normalise(randvec2(-10, 10));
            this.Debug = true;
            this.Position = Global.ScreenSize / 2f + (spawnDir * magnitude(Global.ScreenSize) * 2f);
            float randAng = (float)Math.PI / 36f; // 5 degrees
            this.Velocity = rotate(normalise(Global.ScreenSize / 2f - this.Position), randfloat(-randAng, randAng));
            this.Velocity *= randfloat(3, 6);
            this.Drag = 0f;
        }

        public override void update(float delta)
        {
            base.update(delta);

            // TODO: Asteroids still spawn on screen (or off screen but 
            // move on screen same/next frame)

            if (this.spawning) {
                Console.WriteLine("spawning");                  
                if (this.Position.X > 0 &&
                    this.Position.X < Global.ScreenSize.X &&
                    this.Position.Y > 0 &&
                    this.Position.Y < Global.ScreenSize.Y) {
                    this.spawning = false;
                }
            } else {                    
                if (this.Position.X < 0) { this.SetXPosition(Global.ScreenSize.X); }
                if (this.Position.X > Global.ScreenSize.X) { this.SetXPosition(0); }
                if (this.Position.Y < 0) { this.SetYPosition(Global.ScreenSize.Y); }
                if (this.Position.Y > Global.ScreenSize.Y) { this.SetYPosition(0); }   
            }
        }
    }
}