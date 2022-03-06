﻿using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using static asteroids.util;
using static asteroids.collision;

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

        // performance data
        uint updatesAcc;
        DateTime updateLastTime;
        float updatesPerSecond;
        float totalUpdateTime;
        float avgUpdateTime;

        uint framesAcc;
        DateTime frameLastTime;
        float framesPerSecond;
        
        player? ply;
        List<asteroid> listAsteroids;

        List<body?> bodies;

        public asteroids() {
            
            // using these lists makes it easier
            // to update and draw all items            
            bodies = new List<body?>();

            updateLastTime = DateTime.Now;
            frameLastTime = DateTime.Now;

            ply = new player();
            bodies.Add(ply.ship);
            
            window = new RenderWindow(new VideoMode((uint)Global.ScreenSize.X, (uint)Global.ScreenSize.Y),
                                      "Asteroids",
                                      Styles.Default);

            view = new View(Global.ScreenSize/2f, Global.ScreenSize);
            window.SetView(view);
            window.SetKeyRepeatEnabled(false);
            window.Closed += window_CloseWindow;
            listAsteroids = new List<asteroid>();
            
            for (int i = 12; i > 0; i--) {
                asteroid a = new asteroid(asteroid.enumSize.large);
                a.Position = randvec2(0, Global.ScreenSize.X, 0, Global.ScreenSize.Y);
                listAsteroids.Add(a);
                bodies.Add(a);
            }

            // Load sounds
            Global.sfx.Add("fire", new sound("fire", "sound/fire.wav"));
            Global.sfx.Add("thrust", new sound("thrust", "sound/thrust.wav"));
            Global.sfx.Add("bang1", new sound("bang1", "sound/bangSmall.wav"));
            Global.sfx.Add("bang2", new sound("bang2", "sound/bangMedium.wav"));
            Global.sfx.Add("bang3", new sound("bang3", "sound/bangLarge.wav"));
        }

        private void window_CloseWindow(object? sender, System.EventArgs? e) {
            if (sender == null) { return; }
            ((RenderWindow)sender).Close();
        }

        private void update(float delta) {
            DateTime thisUpdateTime = DateTime.Now;
            updatesAcc++;
            if ((DateTime.Now - updateLastTime).TotalMilliseconds >= 1000) {
                if (updatesAcc != 0) { avgUpdateTime = totalUpdateTime / updatesAcc; }
                totalUpdateTime = 0;
                updatesPerSecond = updatesAcc;
                updateLastTime = DateTime.Now;
                updatesAcc = 0;
            }

            Global.Keyboard.update();

            if (Global.Keyboard["escape"].isPressed) {
                window.Close();
            }

            if (Global.Keyboard["space"].isPressed && ply != null) {
                torpedo? newTorpedo = ply.fire();

                if (newTorpedo != null) { bodies.Add(newTorpedo); }
            }

            if (Global.Keyboard["w"].justPressed) {
                Global.sfx["thrust"].play(true);
            }

            if (Global.Keyboard["w"].justReleased) {
                Global.sfx["thrust"].stop();
            }

            // player is handled separately because of inputs
            if (ply != null) {
                ply.update(delta);
            }

            for (int k = bodies.Count - 1; k >= 0; k--) {
                body? b = bodies[k];
                if (b == null) { continue; }

                Color debugColour = Colour.Grey;

                // handle collisions
                for (int j = bodies.Count - 1; j >= 0; j--) {
                    if (k == j) { continue; }
                    body? a = bodies[j];
                    if (a == null) { continue; }

                    collision? c = collide(a, b);

                    if (c == null) { continue; }

                    debugColour = Colour.Orange;

                    // destroy the player
                    // if (b.GetType() == typeof(player) &&
                    //     c.GetType() == typeof(asteroid) &&
                    //     ply != null) {
                    //     ply.destroy();
                    //     ply = null;
                    // }

                    // delete torpedoes;
                    if (a.GetType() == typeof(torpedo)) {                        
                        if (b.GetType() == typeof(asteroid)) {
                            // larger asteroids break into smaller ones
                            Global.sfx["bang" + randint(1, 3).ToString()].play();
                            
                            if (((asteroid)b).size == asteroid.enumSize.large) {
                                int numNewAsteroids = 2;

                                for (int l = 0; l < numNewAsteroids; l++) {
                                    asteroid newAsteroid = new asteroid(asteroid.enumSize.small);
                                    newAsteroid.Position = a.Position + randvec2(0, a.BoundingCircleRadius);
                                    bodies.Add(newAsteroid);
                                }
                            }

                            bodies.RemoveAt(j--);
                            bodies.RemoveAt(k--);

                            break;
                        }
                    }
                }

                b.DebugColour = debugColour;
                
                if (b == ply.ship) { continue; }
                b.update(delta);

                if (b.GetType() == typeof(torpedo)) {
                    torpedo t = (torpedo)b;
                    if ((DateTime.Now - t.SpawnTime).TotalSeconds > t.LifeTime) {
                        bodies.RemoveAt(k);
                    }
                }
            }

            totalUpdateTime += (float)(DateTime.Now - thisUpdateTime).TotalMilliseconds;
        }

        private void draw() {
            framesAcc++;
            if ((DateTime.Now - frameLastTime).TotalMilliseconds >= 1000) {
                framesPerSecond = framesAcc;
                frameLastTime = DateTime.Now;
                framesAcc = 0;
            }

            window.Clear();

            foreach (body? b in bodies) {
                if (b == null) { continue; }
                b.draw(window);
            }

            window.Display();
        }

        public void run() {
            while (window.IsOpen) {
                if (!window.HasFocus()) { continue; }

                if ((float)(DateTime.Now - lastTime).TotalMilliseconds / 1000f >= timeStep) {
                    float delta = timeStep * timeScale;
                    lastTime = DateTime.Now;
                    runTime += delta;

                    window.DispatchEvents();
                    update(delta);

                    window.SetTitle(String.Format("Asteroids (FPS: {0}, UPS: {1} (avg time: {2}))",
                                    framesPerSecond.ToString(),
                                    updatesPerSecond.ToString(),
                                    avgUpdateTime.ToString()));
                }

                draw();
            }
        }
    }
}