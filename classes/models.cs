using SFML.Graphics;
using SFML.System;
using static asteroids.util;

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

        private static VertexArray? flyingSaucer;
        public static VertexArray FlyingSaucer {
            get {
                if (flyingSaucer == null) {
                    float offsetx = 0f;
                    float offsety = 0f;
                    Vector2f scale = new Vector2f(5, 5);
                    Color c = Color.White;

                    VertexArray va = new VertexArray(PrimitiveType.LineStrip, 12);
                    va[0]  = new Vertex(multi(new Vector2f(offsetx,     offsety),        scale), c);
                    va[1]  = new Vertex(multi(new Vector2f(offsetx + 1, offsety - 1.5f), scale), c);
                    va[2]  = new Vertex(multi(new Vector2f(offsetx + 2, offsety - 1.5f), scale), c);
                    va[3]  = new Vertex(multi(new Vector2f(offsetx + 3, offsety),        scale), c);
                    va[4]  = new Vertex(multi(new Vector2f(offsetx,     offsety),        scale), c);
                    va[5]  = new Vertex(multi(new Vector2f(offsetx - 3, offsety + 1),    scale), c);
                    va[6]  = new Vertex(multi(new Vector2f(offsetx,     offsety + 2),    scale), c);
                    va[7]  = new Vertex(multi(new Vector2f(offsetx + 3, offsety + 2),    scale), c);
                    va[8]  = new Vertex(multi(new Vector2f(offsetx + 6, offsety + 1),    scale), c);
                    va[9]  = new Vertex(multi(new Vector2f(offsetx + 3, offsety),        scale), c);
                    va[10] = new Vertex(multi(new Vector2f(offsetx + 6, offsety + 1),    scale), c);
                    va[11] = new Vertex(multi(new Vector2f(offsetx - 3, offsety + 1),    scale), c);

                    Vector2f avgPos = new Vector2f();
                    for (uint i = 0; i < va.VertexCount; i++) {
                        avgPos += va[i].Position;
                    }

                    avgPos /= va.VertexCount;
                    va = transform(va, -avgPos);

                    flyingSaucer = va;
                }

                return flyingSaucer;
            }
        }
    }
}