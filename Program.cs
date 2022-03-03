using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static asteroids.circlebody;
using static asteroids.util;

namespace asteroids {
    static class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hi :-)");
            asteroids game = new asteroids();
            game.run();
        }
    }

    public class asteroids {
        RenderWindow window;
        View view;

        keyboard kb;
        Vector2f screenSize;
        
        DateTime lastTime;
        double runTime = 0;
        float timeStep = 1/60f;
        float timeScale = 1f;
        
        player ply;

        public asteroids() {
            screenSize =new Vector2f(800, 800);
            kb = new keyboard();
            ply = new player();
            
            window = new RenderWindow(new VideoMode((uint)screenSize.X, (uint)screenSize.Y),
                                      "Asteroids",
                                      Styles.Default);

            view = new View(screenSize/2f, screenSize);
            window.SetView(view);
            window.SetKeyRepeatEnabled(false);
            window.Closed += window_CloseWindow;
        }

        private void window_CloseWindow(object? sender, System.EventArgs? e) {
            if (sender == null) { return; }
            ((RenderWindow)sender).Close();
        }

        private void input() {
            kb.update();
        }

        private void update(float delta) {
            kb.update();

            if (kb["escape"].isPressed) {
                window.Close();
            }
            
            ply.update(delta, kb, screenSize);
        }

        private void draw() {
            window.Clear();

            ply.draw(window);

            window.Display();
        }

        public void run() {
            // get player input
            // do updates
            // draw!

            while (window.IsOpen) {
                if (!window.HasFocus()) { continue; }

                if ((float)(DateTime.Now - lastTime).TotalMilliseconds < timeStep) { continue; }
                float delta = timeStep * timeScale;
                lastTime = DateTime.Now;
                runTime += delta;

                window.DispatchEvents();
                update(delta);
                draw();
            }
        }
    }
}