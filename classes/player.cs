using SFML.Graphics;
using SFML.System;
using static asteroids.util;

namespace asteroids {
    public class player {
        private polybody ship;
        
        private float fireCooldown = 0.25f;
        private float lastFire = 0f;
        private float maxMoveSpeed = 20f;
        private float plyMoveSpeed = 2f;
        private float maxTurnSpeed = 0.4f;
        private float plyTurnSpeed = 0.1f; // radians

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
            this.ship.Drag = 0.01f;
        }

        public void update(float delta, keyboard kb, Vector2f screenSize) {
            
            float plySpeed = magnitude(ship.Velocity);

            // Player movement
            if (plySpeed <= maxMoveSpeed) {
                if (kb["w"].isPressed) {
                    ship.AddVelocity(vector2f(ship.Angle) * plyMoveSpeed * delta);
                }

                if (kb["s"].isPressed) {
                    //ship.AddYVelocity(plyMoveSpeed * delta);
                }
            }

            if (this.ship.AnglularVelocity > -maxTurnSpeed && kb["a"].isPressed) {
                ship.AnglularVelocity -= plyTurnSpeed * delta;
            }

            if (this.ship.AnglularVelocity < maxTurnSpeed && kb["d"].isPressed) {
                ship.AnglularVelocity += plyTurnSpeed * delta;
            }

            if (ship.Position.X < 0) { ship.SetXPosition(screenSize.X); }
            if (ship.Position.X > screenSize.X) { ship.SetXPosition(0); }
            if (ship.Position.Y < 0) { ship.SetYPosition(screenSize.Y); }
            if (ship.Position.Y > screenSize.Y) { ship.SetYPosition(0); }   

            ship.update(delta);         
        }

        public void draw(RenderWindow window) {
            ship.draw(window);
        }
    }
}