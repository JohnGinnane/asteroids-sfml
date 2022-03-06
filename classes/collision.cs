namespace asteroids {
    public class collision {
        private DateTime collideTime;
        private body? a;
        public body? A { get { return a; }}
        private body? b;
        public body? B { get { return b; }}

        internal collision(body a, body b) {
            this.a = a;
            this.b = b;
            collideTime  = DateTime.Now;
        }

        public static collision? collide(body a, body b) {
            // check if the bounding circle overlaps
            // otherwise ignore

            // this is "cheaper" than the entire distance check 
            float possq = (float)(Math.Pow(a.Position.X - b.Position.X, 2) + Math.Pow(a.Position.Y - b.Position.Y, 2));
            float radsq = (float)(Math.Pow(a.BoundingCircleRadius + b.BoundingCircleRadius, 2));

            if (radsq < possq) { return null; }

            collision c = new collision(a, b);

            return c;
        }

        public static void resolve(collision c) {
            
        }
    }
}