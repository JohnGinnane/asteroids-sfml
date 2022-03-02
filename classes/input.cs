using SFML.System;
using SFML.Window;

namespace asteroids {
    public class key {
        public string name;
        public int code;
        public bool isPressed;
        public DateTime timePressed;
        public DateTime timeReleased;

        public bool justPressed;
        public bool justReleased;

        public key(string name, int code) {
            this.name = name;
            this.code = code;
        }
    }
    public class keyboard {
        private List<key> keys;

        public keyboard() {
            keys = new List<key>();

            for (int k = (int)Keyboard.Key.A; k < (int)Keyboard.Key.KeyCount; k++) {
                key v = new key(((Keyboard.Key)k).ToString(), k);
                keys.Add(v);
            }
        }

        public void update() {
            for (int k = (int)Keyboard.Key.A; k < (int)Keyboard.Key.KeyCount; k++) {
                
                bool lastPressed = keys[k].isPressed;

                keys[k].justPressed = false;
                keys[k].justReleased = false;
                
                keys[k].isPressed = Keyboard.IsKeyPressed((Keyboard.Key)k);

                if (!lastPressed && keys[k].isPressed) {
                    keys[k].justPressed = true;
                }

                if (lastPressed && !keys[k].isPressed) {
                    keys[k].justReleased = true;
                }
            }
        }

        public key this[string name] => FindKeyIndex(name);

        private key FindKeyIndex(string name) {
            key? output;

            output = keys.Find(x => x.name.ToLower() == name.ToLower());

            if (output == null) {
                return new key("Unknown", -1);
            }

            return (key)output;
        }
    }
    public class input {
        public input() {

        }

        public void update() {

        }
    }
}