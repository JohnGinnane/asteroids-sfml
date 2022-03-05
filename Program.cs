﻿using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static asteroids.circlebody;
using static asteroids.util;

namespace asteroids {
    static class Program {
        
        static void Main(string[] args) {
            Console.WriteLine("Hi :-)");
            Global.ScreenSize = new Vector2f(800, 800);

            asteroids game = new asteroids();
            game.run();
        }
    }

    public class asteroids {
        RenderWindow window;
        View view;

        DateTime lastTime;
        double runTime = 0;
        float timeStep = 1/60f;
        float timeScale = 1f;
        
        player ply;
        List<asteroid> listAsteroids;

        public asteroids() {
            ply = new player();
            
            window = new RenderWindow(new VideoMode((uint)Global.ScreenSize.X, (uint)Global.ScreenSize.Y),
                                      "Asteroids",
                                      Styles.Default);

            view = new View(Global.ScreenSize/2f, Global.ScreenSize);
            window.SetView(view);
            window.SetKeyRepeatEnabled(false);
            window.Closed += window_CloseWindow;
            listAsteroids = new List<asteroid>();

            for (int i = 5; i > 0; i--) {
                asteroid a= new asteroid();
                a.Position = randvec2(0, Global.ScreenSize.X, 0, Global.ScreenSize.Y);
                a.Debug = true;
                a.Drag = 0f;
                a.Radius = 20f;
                a.FillColour = Color.Red; 
                listAsteroids.Add(a);
            }
        }

        private void window_CloseWindow(object? sender, System.EventArgs? e) {
            if (sender == null) { return; }
            ((RenderWindow)sender).Close();
        }

        private void input() {
            Global.Keyboard.update();
        }

        private void update(float delta) {
            Global.Keyboard.update();

            if (Global.Keyboard["escape"].isPressed) {
                window.Close();
            }
            
            ply.update(delta);

            foreach (asteroid a in listAsteroids) {
                a.update(delta);
            }
        }

        private void draw() {
            window.Clear();

            ply.draw(window);

            foreach (asteroid a in listAsteroids) {
                a.draw(window);
            }

            window.Display();
        }

        public void run() {
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