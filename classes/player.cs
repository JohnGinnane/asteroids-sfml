using SFML.Graphics;
using SFML.System;
using static asteroids.util;

namespace asteroids {
    public class player {
        public polybody ship;

        private float fireCooldown = 100f;
        DateTime lastFire;
        private float maxMoveSpeed = 600f;
        private float plyMoveSpeed = 200f;
        private float maxTurnSpeed = 100f;
        private float plyTurnSpeed = 50f;
        private float torpedoSpeed = 400f;

        public player() {
            this.ship = new polybody(rotate(models.SpaceShip, (float)Math.PI/2f));
            this.ship.Parent = this;
            this.ship.Position = Global.ScreenSize / 2f;
            this.ship.Drag = 0.2f;
            this.ship.AngularDrag = 8f;
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
            Vector2f fireVel = this.ship.Velocity;
            fireVel += vector2f(this.ship.Angle) * this.torpedoSpeed;

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
            // draw the little thrust model
            if (DateTime.Now.Millisecond % 200 < 100 && Global.Keyboard["w"].isPressed) {
                VertexArray thrustVa = transform(rotate(models.SpaceShipThrust, this.ship.Angle - (float)Math.PI/2f), this.ship.Position);
                window.Draw(thrustVa);
            }
        }
    }
}