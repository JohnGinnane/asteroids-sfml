using SFML.Graphics;
using SFML.System;
using static asteroids.util;

namespace asteroids {
    public class particle : polybody {
        private DateTime destroyTime;
        public DateTime DestroyTime {
            get { return destroyTime; }
            set { destroyTime = value; }
        }

        public particle(DateTime destroyTime) {
            this.destroyTime = destroyTime;
            
        }

        public static List<particle> createParticles(uint num, Vector2f position) {
            List<particle> newParticles = new List<particle>();

            for (int i = 0; i < num; i++) {
                particle p = new particle(DateTime.Now.AddSeconds(3));
                p.Position = position;
                p.Velocity = randvec2(-50, 50);
                VertexArray va = new VertexArray(PrimitiveType.Points, 1);
                va[0] = new Vertex(new Vector2f(), Color.White);
                p.Shape = va;                            
                Global.particles.Add(p);
            }

            return newParticles;
        }

        public static List<particle> convertToParticles(body b, DateTime destroyTime) {
            List<particle> newParticles = new List<particle>();

            // take a vertex array and convert to lines
            if (b.ShapeType == typeof(VertexArray) && b.Shape != null) {
                VertexArray va = (VertexArray)b.Shape;

                for (uint i = 1; i < va.VertexCount; i++) {
                    VertexArray newVa = new VertexArray(PrimitiveType.LineStrip, 2);
                    Vector2f position = (va[i].Position + va[i-1].Position) / 2f;

                    newVa[0] = new Vertex(va[i-1].Position, va[i-1].Color);
                    newVa[1] = new Vertex(va[i].Position, va[i].Color);
                    particle newP = new particle(destroyTime);
                    newP.Shape = newVa;
                    if (b.GetType() == typeof(polybody)) {
                        newP.Angle = ((polybody)b).Angle;
                    }
                    float velMag = magnitude(b.Velocity) / 2f;
                    newP.Velocity = b.Velocity / 2f + randvec2(-velMag, velMag);
                    newP.Position = b.Position;
                    newP.AnglularVelocity = randfloat(-1, 1);
                    newP.AngularDrag = 0f;
                    
                    newParticles.Add(newP);
                }
            }

            return newParticles;
        }
    }
}