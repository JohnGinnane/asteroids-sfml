using SFML.System;
using SFML.Graphics;
using static asteroids.util;

namespace asteroids {
    public abstract class body {
#region "Properties"
        private bool debug;
        public bool Debug {
            get { return debug; }
            set { debug = value; }
        }

        private Color debugColour;
        public Color DebugColour {
            get { return debugColour; }
            set { debugColour = value; }
        }

        // Movement properties
        private Vector2f position;
        public Vector2f Position {
            get { return position; }
            set {
                position = value;

                if (shapeType == typeof(CircleShape) && Shape != null) {
                    ((CircleShape)Shape).Position = value + shapeOffset;
                    return;
                }

                if (shapeType == typeof(RectangleShape) && Shape != null) {
                    ((RectangleShape)Shape).Position = value + shapeOffset;
                    return;
                }
            }
        }
        
        private Vector2f velocity;
        public Vector2f Velocity {
            get { return velocity; }
            set { velocity = value; }
        }

        private float drag = 0.01f;
        public float Drag {
            get { return drag; }
            set { drag = value; }
        }

        public bool isStatic = false;

        // Collision properties
        private float mass = 100f;
        public float Mass {
            get { return mass; }
            set { mass = value; }
        }

        private float bounciness = 1f;
        public float Bounciness {
            get { return bounciness; }
            set { bounciness = value; }
        }

        private float boundingCircleRadius;
        public float BoundingCircleRadius {
            get { return boundingCircleRadius; }
        }

        internal Type? noCollideType;
        public Type? NoCollideType {
            get { return noCollideType; }
        }
        
        // Drawing properties
        internal Drawable? shape;
        public Drawable? Shape {
            get { return shape; }
            set {
                if (value == null) { 
                    this.shape = null;
                    this.shapeType = null;
                    this.boundingCircleRadius = 0f;
                } else {
                    this.shape = value;
                    this.shapeType = value.GetType();
                    this.boundingCircleRadius = calculateBoundingCircleRadius();
                }
            }
        }

        internal Type? shapeType;
        public Type? ShapeType { get { return shapeType; } }

        internal Vector2f shapeOffset;

        private Color fillColour;
        public Color FillColour {
            get { return fillColour; }
            set {
                fillColour = value;

                if (shapeType == typeof(CircleShape) && Shape != null) {
                    ((CircleShape)Shape).FillColor = value;
                    return;
                }

                if (shapeType == typeof(RectangleShape) && Shape != null) {
                    ((RectangleShape)Shape).FillColor = value;
                    return;
                }
            }
        }

        private Color outlineColour = Color.White;
        public Color OutlineColour {
            get { return outlineColour; }
            set {
                outlineColour = value;
                
                if (shapeType == typeof(CircleShape) && Shape != null) {
                    ((CircleShape)Shape).OutlineColor = value;
                    return;
                }

                if (shapeType == typeof(RectangleShape) && Shape != null) {
                    ((RectangleShape)Shape).OutlineColor = value;
                    return;
                }
            }
        }

        private float outlineThickness = 0f;
        public float OutlineThickness {
            get { return outlineThickness; }
            set {
                outlineThickness = value;
                
                if (shapeType == typeof(CircleShape) && Shape != null) {
                    ((CircleShape)Shape).OutlineThickness = value;
                    return;
                }

                if (shapeType == typeof(RectangleShape) && Shape != null) {
                    ((RectangleShape)Shape).OutlineThickness = value;
                    return;
                }
            }
        }
#endregion

#region "Methods"
        private float calculateBoundingCircleRadius() {
            if (this.GetType() == typeof(circlebody)) {
                return ((circlebody)this).Radius;
            }

            if (this.GetType() == typeof(rectbody)) {
                return magnitude(((rectbody)this).Size)/2f;
            }
            
            // for polygonal shapes iterate over all points
            // and use the furthest point
            if (this.GetType() == typeof(polybody) ||
                this.GetType().IsSubclassOf(typeof(polybody))) {
                float furthestPoint = 0f;
                VertexArray? va = (VertexArray?)((polybody)this).Shape;
                if (va == null) { return 0f; }

                for (uint i = 0; i < va.VertexCount; i++) {
                    float curDist = magnitude(this.Position - va[i].Position);

                    if (curDist > furthestPoint) { furthestPoint = curDist; }
                }

                return furthestPoint;
            }
            
            return 0f;
        }

        public void SetPosition(Vector2i position) {
            this.Position = (Vector2f)position;
        }

        public void SetPosition(Vector2f position) {
            this.Position = position;
        }

        public void SetXPosition(float x) {
            this.Position = new Vector2f(x, this.Position.Y);
        }

        public void SetYPosition(float y) {
            this.Position = new Vector2f(this.Position.X, y);
        }

        public void SetVelocity(Vector2f velocity) {
            this.Velocity = velocity;
        }

        public void AddVelocity(Vector2f velocity) {
            this.Velocity = this.Velocity + velocity;
        }

        public void SetXVelocity(float x) {
            this.Velocity = new Vector2f(x, this.Velocity.Y);
        }

        public void AddXVelocity(float x) {
            this.Velocity = this.Velocity + new Vector2f(x, 0);
        }

        public void SetYVelocity(float y) {
            this.Velocity = new Vector2f(this.Velocity.X, y);
        }

        public void AddYVelocity(float y) {
            this.Velocity = this.Velocity + new Vector2f(0, y);
        }

        public virtual void update(float delta) {
            if (!isStatic) {
                this.Position += this.Velocity * delta;
                if (this.Drag != 0) {
                    this.Velocity *= (1f - this.Drag * delta);
                }
            }
        }

        public virtual void resolve(collision c) {
            
        }

        public virtual void draw(RenderWindow window) { 
            if (this.Shape == null) { return; }

            if (shapeType == typeof(CircleShape)) {
                window.Draw(this.Shape);
            } else if (shapeType == typeof(RectangleShape)) {
                window.Draw(this.Shape);
            }

            if (this.Debug) {
                // draw direction we are facing
                VertexArray va = new VertexArray(PrimitiveType.Lines, 2);
                va[0] = new Vertex(this.Position, this.DebugColour);
                va[1] = new Vertex(this.Position + this.Velocity, this.DebugColour);
                window.Draw(va, RenderStates.Default);

                // TODO: draw bounding circle
                CircleShape cs = new CircleShape(this.BoundingCircleRadius);
                cs.Position = this.Position - new Vector2f(this.BoundingCircleRadius, this.BoundingCircleRadius);
                cs.FillColor = Color.Transparent;
                cs.OutlineThickness = 1f;
                cs.OutlineColor = this.DebugColour;
                window.Draw(cs);
            }
        }

        public bool collide(body otherbody) {
            if (otherbody == null) { return false; }
            if (this.isStatic) { return false; } // our body is static, we don't care about collisions right now
            
            if (otherbody.GetType() == typeof(circlebody) &&
                    this.GetType() == typeof(circlebody)) {
                // https://en.wikipedia.org/wiki/Elastic_collision#Two-dimensional_collision_with_two_moving_objects
                circlebody A = (circlebody)this;
                circlebody B = (circlebody)Convert.ChangeType(otherbody, typeof(circlebody));
                
                Vector2f x1 = A.Position;
                Vector2f x2 = B.Position;
                Vector2f dir = normalise(x2 - x1);
                float dist = distance(x1, x2);

                // If we are too far from the other body then just exit
                if (dist > A.Radius + B.Radius) {
                    return false;
                }
                
                // move the circle away from the other body so it no longer overlaps
                A.SetPosition(A.Position - dir * (A.Radius + B.Radius - dist));

                Vector2f v1 = A.Velocity;
                Vector2f v2 = B.Velocity;

                float m1 = A.mass;
                float m2 = B.mass;
                float bounceFactor = (A.Bounciness + B.Bounciness) / 2f;

                if (!B.isStatic) {
                    Vector2f u1 = v1 - ((2*m2) / (m1+m2)) * dot(v1-v2, x1-x2) / (float)(Math.Pow(magnitude(x1-x2), 2)) * (x1 - x2);
                    Vector2f u2 = v2 - ((2*m1) / (m1+m2)) * dot(v2-v1, x2-x1) / (float)(Math.Pow(magnitude(x2-x1), 2)) * (x2 - x1);

                    A.Velocity = u1 * bounceFactor;
                    B.Velocity = u2 * bounceFactor;
                } else {
                    Vector2f hn = normalise(A.Position - B.Position);
                    Vector2f vnew = -2f * dot(A.Velocity, hn) * hn + A.Velocity;
                    A.Velocity = vnew * bounceFactor;
                }

                return true;
            }

            if (otherbody.GetType() == typeof(rectbody) &&
                    this.GetType() == typeof(rectbody)) {
                // TO DO: Implement this in another project
                return false;
            }

            if (otherbody.GetType() == typeof(rectbody) &&
                    this.GetType() == typeof(circlebody)) {
                // https://www.geeksforgeeks.org/check-if-any-point-overlaps-the-given-circle-and-rectangle/

                rectbody B = (rectbody)Convert.ChangeType(otherbody, typeof(rectbody));
                circlebody C = (circlebody)this;
                
                Vector2f topLeft = B.Position - B.Size/2f;
                Vector2f bottomRight = B.Position + B.Size/2f;

                Vector2f closestPoint = new Vector2f(Math.Clamp(C.Position.X, topLeft.X, bottomRight.X),
                                                    Math.Clamp(C.Position.Y, topLeft.Y, bottomRight.Y));

                float D = C.Radius - distance(closestPoint, C.Position);

                if (D < 0) {
                    return false;
                }

                Vector2f dir = normalise(closestPoint - C.Position);

                if (dir.X is float.NaN || dir.Y is float.NaN) {
                    dir = normalise(B.Position - closestPoint);
                }
                
                // Correct our position and place it outside the rectangle
                C.SetPosition(C.Position - (dir * D));

                // find what side the circle is touching
                Vector2f hn = new Vector2f(dir.X / B.Size.X, dir.Y / B.Size.Y);
                if (Math.Abs(hn.X) > Math.Abs(hn.Y)) {
                    hn = new Vector2f(Math.Sign(hn.X), 0);
                } else if (Math.Abs(hn.Y) > Math.Abs(hn.X)) {
                    hn = new Vector2f(0, Math.Sign(hn.Y));
                } else {
                    hn = new Vector2f(Math.Sign(hn.X), Math.Sign(hn.Y));
                }

                float bounceFactor = (C.Bounciness + B.Bounciness) / 2f;

                // https://www.3dkingdoms.com/weekly/weekly.php?a=2
                Vector2f vnew = -2f * dot(C.Velocity, hn) * hn + C.Velocity;
                C.Velocity = vnew * bounceFactor;

                return true;
            }

            return false;
        }
    }
#endregion
}