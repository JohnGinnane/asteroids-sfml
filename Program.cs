using SFML.Graphics;
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
            asteroidsGame game = new asteroidsGame();
            game.run();
        }
    }

    public class asteroidsGame {
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

        List<body?> bodies;
        DateTime playerSpawnTime;

        DateTime? nextLevelTime;
        int level = 0;

        private Text pointsText;
        int totalPoints;
        int TotalPoints {
            get { return totalPoints; }
            set {
                totalPoints = value;
                pointsText.DisplayedString = value.ToString();
            }
        }

        public asteroidsGame() {            
            // using these lists makes it easier
            // to update and draw all items            
            bodies = new List<body?>();

            updateLastTime = DateTime.Now;
            frameLastTime = DateTime.Now;
            playerSpawnTime = DateTime.Now.AddSeconds(1);

            window = new RenderWindow(new VideoMode((uint)Global.ScreenSize.X, (uint)Global.ScreenSize.Y),
                                      "Asteroids",
                                      Styles.Default);

            view = new View(Global.ScreenSize/2f, Global.ScreenSize);
            window.SetView(view);
            window.SetKeyRepeatEnabled(false);
            window.Closed += window_CloseWindow;

            totalPoints = 0;
            pointsText = new Text(this.TotalPoints.ToString(), Fonts.Hyperspace);
            pointsText.CharacterSize = 32;
            pointsText.FillColor = Color.White;
            pointsText.Position = new Vector2f(10, 10);

            // Load sounds
            Global.sfx.Add("fire",       new sound("fire",       "sound/fire.wav"));
            Global.sfx.Add("thrust",     new sound("thrust",     "sound/thrust.wav"));
            Global.sfx.Add("bangSmall",  new sound("bangSmall",  "sound/bangSmall.wav"));
            Global.sfx.Add("bangMedium", new sound("bangMedium", "sound/bangMedium.wav"));
            Global.sfx.Add("bangLarge",  new sound("bang3",      "sound/bangLarge.wav"));

            nextLevelTime = DateTime.Now.AddSeconds(1);
        }

        private void window_CloseWindow(object? sender, System.EventArgs? e) {
            if (sender == null) { return; }
            ((RenderWindow)sender).Close();
        }

        public void nextLevel() {
            level++;
            bodies.AddRange(asteroid.spawnAsteroids(level));
            nextLevelTime = null;
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

            // Check if we cleared the level
            if (DateTime.Now > nextLevelTime) {
                nextLevel();
            }

            // Update all our particles
            for (int k = Global.particles.Count - 1; k >= 0; k--) {
                particle p = Global.particles[k];

                p.update(delta);

                if (DateTime.Now > p.DestroyTime) {
                    Global.particles.RemoveAt(k);
                    break;
                }
            }

            Global.Keyboard.update();

            if (Global.Keyboard["escape"].isPressed) {
                window.Close();
            }

            if (ply == null) {
                if (DateTime.Now > playerSpawnTime) {
                    ply = new player();
                    bodies.Add(ply.ship);
                }
            } else {
                ply.update(delta);
            }

            if (Global.Keyboard["space"].isPressed && ply != null) {
                torpedo? newTorpedo = ply.fire();

                if (newTorpedo != null) { bodies.Add(newTorpedo); }
            }

            if (Global.Keyboard["w"].justPressed && ply != null) {
                Global.sfx["thrust"].play(true);
            }

            if (Global.Keyboard["w"].justReleased) {
                Global.sfx["thrust"].stop();
            }

            for (int k = bodies.Count - 1; k >= 0; k--) {
                body? b = bodies[k];
                if (b == null) { continue; }

                // handle homing
                if (b.GetType() == typeof(torpedo)) {
                    torpedo t = (torpedo)b;

                    if (t.Target == null) {
                        // find the nearest asteroid
                        asteroid? nearest = null;
                        float nearestDot = -1;

                        for (int j = bodies.Count - 1; j >= 0; j--) {
                            if (k == j) { continue; }
                            if (bodies[j] == null) { continue; }
                            if (bodies[j].GetType() != typeof(asteroid)) { continue; }
                            
                            Vector2f astDir = normalise(bodies[j].Position - ply.ship.Position);
                            float astDot = dot(vector2f(ply.ship.Angle), astDir);
                            if (astDot > nearestDot) {
                                nearestDot = astDot;
                                nearest = (asteroid?)bodies[j];
                            }
                        }

                        if (nearest != null) {
                            t.Target = nearest;
                        }
                    }
                }

                // handle collisions
                if (b.CheckCollisions) {
                    for (int j = bodies.Count - 1; j >= 0; j--) {
                        if (k == j) { continue; }
                        body? a = bodies[j];
                        if (a == null) { continue; }

                        collision? c = collide(a, b);

                        if (c == null) { continue; }

                        if (b.GetType() == typeof(asteroid)) {
                            // if the player is hit by an asteroid then destroy the ship
                            if (ply != null) {
                                if (a == ply.ship) {
                                    Global.sfx["bangMedium"].play();
                                    Global.sfx["thrust"].stop();
                                    bodies.RemoveAt(j--);
                                    Global.particles.AddRange(particle.convertToParticles(ply.ship, DateTime.Now.AddSeconds(5)));
                                    ply = null;
                                    playerSpawnTime = DateTime.Now.AddSeconds(3);
                                }
                            }

                            // if torpedo hits asteroid then destroy it                     
                            asteroid ast = (asteroid)b;
                            for (int i = 0; i < 6; i++) {
                                particle p = new particle(DateTime.Now.AddSeconds(3));
                                p.Position = ast.Position;
                                p.Velocity = randvec2(-50, 50);
                                VertexArray va = new VertexArray(PrimitiveType.Points, 1);
                                va[0] = new Vertex(new Vector2f(), Color.White);
                                p.Shape = va;                            
                                Global.particles.Add(p);
                            }

                            TotalPoints += ast.Points;
                            bodies.AddRange(ast.breakup(2));

                            try {
                                if (k > j) {
                                    bodies.RemoveAt(k--);
                                    bodies.RemoveAt(j--);
                                } else {
                                    bodies.RemoveAt(j--);
                                    bodies.RemoveAt(k--);
                                }
                            } catch (Exception e) {
                                Console.WriteLine(e.Message);
                            }

                            break;
                        }
                    }
                }

                b.update(delta);

                if (b.GetType() == typeof(torpedo)) {
                    torpedo t = (torpedo)b;
                    if (DateTime.Now > t.DestroyTime) {
                        bodies.RemoveAt(k);
                    }
                }
            }

            // If no asteroids or saucers are left then end level
            if (nextLevelTime == null) {
                int targets = 0;
                foreach (body? b in bodies) {
                    if (b == null) { continue; }
                    if (b.GetType() == typeof(asteroid)) { targets++; }
                }

                if (targets == 0) { 
                    nextLevelTime = DateTime.Now.AddSeconds(1);
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

            foreach (particle p in Global.particles) {
                p.draw(window);
            }

            window.Draw(pointsText);

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