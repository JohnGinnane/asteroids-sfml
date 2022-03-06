using SFML.Audio;

namespace asteroids {
    public class sound {
        private string name;
        public string Name {
            get { return name; }
        }

        private SoundBuffer buffer;
        private Sound sfmlSound;
        
        public sound(string name, string filename) {
            this.name = name;
            buffer = new SoundBuffer(filename);
            sfmlSound = new Sound(buffer);
        }

        public void play(bool loop = false) {
            sfmlSound.Loop = loop;
            sfmlSound.Play();
        }

        public void stop() {
            sfmlSound.Stop();
        }

        public void pause() {
            sfmlSound.Pause();
        }
    }
}