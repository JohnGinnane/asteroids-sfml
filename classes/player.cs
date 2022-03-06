using SFML.Graphics;
using SFML.System;
using static asteroids.util;

namespace asteroids {
    public class player {
        public polybody ship;

        private float fireCooldown = 250f;
        DateTime lastFire;
        private float maxMoveSpeed = 400f;
        private float plyMoveSpeed = 200f;
        private float maxTurnSpeed = 50f;
        private float plyTurnSpeed = 25f;

        public player() {
            float offsetx = 0f;
            float offsety = 2f;

            VertexArray va = new VertexArray(PrimitiveType.LineStrip, 5);
            va[0] = new Vertex(new Vector2f(offsetx,      offsety - 24), Color.White);
            va[1] = new Vertex(new Vector2f(offsetx + 18, offsety + 24), Color.White);
            va[2] = new Vertex(new Vector2f(offsetx,      offsety + 18), Color.White);
            va[3] = new Vertex(new Vector2f(offsetx - 18, offsety + 24), Color.White);
            va[4] = new Vertex(new Vector2f(offsetx,      offsety - 24), Color.White);
            
            this.ship = new polybody(rotate(va, (float)Math.PI/2f));
            this.ship.Position = new Vector2f(400, 400);
            this.ship.Drag = 0.5f;
            this.ship.AngularDrag = 4f;
            this.ship.Debug = true;
        }

        public torpedo? fire() {
            if ((DateTime.Now - lastFire).TotalMilliseconds < fireCooldown) {
                return null;
            }

            lastFire = DateTime.Now;
            Global.sfx["fire"].play();

            // from from in front of the ship
            Vector2f firePos = this.ship.Position;
            firePos += vector2f(this.ship.Angle) * (this.ship.BoundingCircleRadius + 2f);
            Vector2f fireVel = this.ship.Velocity * 2f;
            fireVel += vector2f(this.ship.Angle) * this.maxMoveSpeed;

            torpedo t = new torpedo(firePos, fireVel);
            return t;
        }

        public void update(float delta) {
            
            float plySpeed = magnitude(ship.Velocity);

            // Player movement
            if (plySpeed <= maxMoveSpeed) {
                if (Global.Keyboard["w"].isPressed) {
                    ship.AddVelocity(vector2f(ship.Angle) * plyMoveSpeed * delta);
                }

                if (Global.Keyboard["s"].isPressed) {
                    //ship.AddYVelocity(plyMoveSpeed * delta);
                }
            }

            if (this.ship.AnglularVelocity > -maxTurnSpeed && Global.Keyboard["a"].isPressed) {
                ship.AnglularVelocity -= plyTurnSpeed * delta;
            }

            if (this.ship.AnglularVelocity < maxTurnSpeed && Global.Keyboard["d"].isPressed) {
                ship.AnglularVelocity += plyTurnSpeed * delta;
            }

            if (ship.Position.X < 0) { ship.SetXPosition(Global.ScreenSize.X); }
            if (ship.Position.X > Global.ScreenSize.X) { ship.SetXPosition(0); }
            if (ship.Position.Y < 0) { ship.SetYPosition(Global.ScreenSize.Y); }
            if (ship.Position.Y > Global.ScreenSize.Y) { ship.SetYPosition(0); }   

            ship.update(delta);         
        }

        public void draw(RenderWindow window) {
            ship.draw(window);
        }
    }
}