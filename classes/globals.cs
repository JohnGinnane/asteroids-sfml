using SFML.Graphics;
using SFML.System;

namespace asteroids {
    public static class Global {            
        private static Vector2f screenSize;
        public static Vector2f ScreenSize {
            get { return screenSize; }
            set { screenSize = value; }
        }

        private static keyboard kb = new keyboard();
        public static keyboard Keyboard {
            get { return kb; }
        }

        public static Dictionary<string, sound> sfx = new Dictionary<string, sound>();

        public static List<particle>particles = new List<particle>();
    }
}