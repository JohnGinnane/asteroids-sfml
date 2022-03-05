using SFML.System;
using SFML.Graphics;
using static asteroids.util;

namespace asteroids {
    public class circlebody : body {
        private float radius;
        public float Radius {
            get { return radius; }
            set {
                radius = value;
                ((CircleShape)Shape).Radius = value;
                shapeOffset = new Vector2f(-radius, -radius);
            }
        }

        internal circlebody() : this(10, Global.ScreenSize / 2f) {}

        public circlebody(float radius, Vector2f pos) {
            this.Shape = new CircleShape();
            this.Radius = radius;
            this.FillColour = Color.White;
            this.Position = pos;
            this.Mass = 100f;
        }
    }
}