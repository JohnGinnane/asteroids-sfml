using SFML.System;
using SFML.Graphics;

namespace asteroids {
    public class torpedo : circlebody {
        public torpedo(Vector2f position, Vector2f velocity) {
            this.Radius = 2f;
            this.Shape = new CircleShape(this.Radius);
            this.Position = position;
            this.Velocity = velocity;
            this.FillColour = Color.Transparent;
            this.OutlineColour = Color.White;
            this.OutlineThickness = 1f;
        }

        public override void draw(RenderWindow window)
        {
            base.draw(window);

            if (this.Position.X < 0) { this.SetXPosition(Global.ScreenSize.X); }
            if (this.Position.X > Global.ScreenSize.X) { this.SetXPosition(0); }
            if (this.Position.Y < 0) { this.SetYPosition(Global.ScreenSize.Y); }
            if (this.Position.Y > Global.ScreenSize.Y) { this.SetYPosition(0); }
        }
    }
}