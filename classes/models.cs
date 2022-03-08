using SFML.Graphics;
using SFML.System;

namespace asteroids {
    public static class models {
        private static VertexArray? spaceShip;
        public static VertexArray SpaceShip {
            get {
                if (spaceShip == null) {
                    float offsetx = 0f;
                    float offsety = 2f;
                    float scale = 0.5f;

                    VertexArray va = new VertexArray(PrimitiveType.LineStrip, 5);
                    va[0] = new Vertex(new Vector2f(offsetx,      offsety - 24) * scale, Color.White);
                    va[1] = new Vertex(new Vector2f(offsetx + 18, offsety + 24) * scale, Color.White);
                    va[2] = new Vertex(new Vector2f(offsetx,      offsety + 18) * scale, Color.White);
                    va[3] = new Vertex(new Vector2f(offsetx - 18, offsety + 24) * scale, Color.White);
                    va[4] = new Vertex(new Vector2f(offsetx,      offsety - 24) * scale, Color.White);

                    spaceShip = va;
                }

                return spaceShip;                
            }
        }

        private static VertexArray? spaceShipThrust;
        public static VertexArray SpaceShipThrust {
            get {
                if (spaceShipThrust == null) {
                    float offsetx = 0f;
                    float offsety = 2f;
                    float scale = 0.5f;

                    VertexArray va = new VertexArray(PrimitiveType.LineStrip, 3);
                    va[0] = new Vertex(new Vector2f(offsetx - 9, offsety - 24) * scale, Color.White);
                    va[1] = new Vertex(new Vector2f(offsetx,     offsety - 45) * scale, Color.White);
                    va[2] = new Vertex(new Vector2f(offsetx + 9, offsety - 24) * scale, Color.White);

                    spaceShipThrust = va;
                }

                return spaceShipThrust;
            }
        }
    }
}