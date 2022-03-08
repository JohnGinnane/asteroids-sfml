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
        DateTime? playerSpawnTime;

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

        // used for playing beats
        int maxTargetsThisLevel = 0;
        DateTime nextBeat;
        uint lastBeat;

        public enum enumGameState {
            start,
            running,
            end
        }

        public enumGameState gameState = enumGameState.start;

        int lives;

        public asteroidsGame() {            
            // using these lists makes it easier
            // to update and draw all items            
            bodies = new List<body?>();

            updateLastTime = DateTime.Now;
            frameLastTime = DateTime.Now;

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

            nextLevelTime = DateTime.Now.AddSeconds(1);

            // Load sounds
            Global.sfx.Add("fire",       new sound("fire",       "sound/fire.wav"));
            Global.sfx.Add("thrust",     new sound("thrust",     "sound/thrust.wav"));
            Global.sfx.Add("bangSmall",  new sound("bangSmall",  "sound/bangSmall.wav"));
            Global.sfx.Add("bangMedium", new sound("bangMedium", "sound/bangMedium.wav"));
            Global.sfx.Add("bangLarge",  new sound("bang3",      "sound/bangLarge.wav"));
            Global.sfx.Add("beat1",      new sound("beat1",      "sound/beat1.wav"));
            Global.sfx.Add("beat2",      new sound("beat2",      "sound/beat2.wav"));
        }

        private void window_CloseWindow(object? sender, System.EventArgs? e) {
            if (sender == null) { return; }
            ((RenderWindow)sender).Close();
        }

        public void nextLevel() {
            level++;
            bodies.AddRange(asteroid.spawnAsteroids(level + 3));
            nextLevelTime = null;
            
            maxTargetsThisLevel = countTargets();
        }

        public void clearLevel() {
            for (int k = bodies.Count - 1; k >= 0; k--) {
                if (bodies[k] == null) { continue; }
                bodies.RemoveAt(k);
            }

            maxTargetsThisLevel = 0;
        }

        public int countTargets() {
            int c = 0;

            foreach (body? b in bodies) {
                if (b == null) { continue; }
                if (b.GetType() != typeof(asteroid)) { continue; }
                asteroid a = (asteroid)b;

                switch (a.size) {
                    case asteroid.enumSize.small:
                        c++;
                        break;
                    case asteroid.enumSize.medium:
                        c+=2;
                        break;
                    case asteroid.enumSize.large:
                        c+=4;
                        break;
                }
            }

            return c;
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

            // Update all our particles
            for (int k = Global.particles.Count - 1; k >= 0; k--) {
                particle p = Global.particles[k];

                p.update(delta);

                if (DateTime.Now > p.DestroyTime) {
                    Global.particles.RemoveAt(k);
                    break;
                }
            }

            if (Global.Keyboard["escape"].isPressed) {
                window.Close();
            }

            if (Global.Keyboard["space"].justPressed) {
                switch (gameState) {
                    case enumGameState.start:
                        gameState = enumGameState.running;
                        playerSpawnTime = DateTime.Now.AddSeconds(1);
                        level = 0;
                        TotalPoints = 0;
                        lives = 3;
                        clearLevel();
                        nextLevelTime = DateTime.Now.AddSeconds(1);
                        break;
                    case enumGameState.running:
                        if (ply != null) { bodies.Add(ply.fire()); }
                        break;
                    case enumGameState.end:
                        gameState = enumGameState.start;
                        level = 0;
                        TotalPoints = 0;
                        lives = 3;
                        break;
                }
            }

            if (Global.Keyboard["w"].justReleased) {
                Global.sfx["thrust"].stop();
            }

            // Check if we cleared the level
            if (gameState < enumGameState.end) {
                if (nextLevelTime != null) {
                    if (DateTime.Now > nextLevelTime) {
                        nextLevel();
                        nextLevelTime = null;
                    }
                }

                // Update all bodies
                for (int k = bodies.Count - 1; k >= 0; k--) {
                    body? b = bodies[k];
                    if (b == null) { continue; }

                    // handle collisions
                    if (b.CheckCollisions) {
                        for (int j = bodies.Count - 1; j >= 0; j--) {
                            if (k == j) { continue; }
                            body? a = bodies[j];
                            if (a == null) { continue; }

                            collision? c = collide(a, b);

                            if (c == null) { continue; }

                            if (b.GetType() == typeof(asteroid)) {
                                // the asteroid was hit by a player or torpedo
                                asteroid ast = (asteroid)b;

                                TotalPoints += ast.Points;
                                bodies.AddRange(ast.breakup(2));

                                // if the player is hit by an asteroid then destroy the ship
                                if (ply != null && a == ply.ship) {
                                    Global.sfx["bangMedium"].play();
                                    Global.sfx["thrust"].stop();
                                    Global.particles.AddRange(particle.convertToParticles(ply.ship, DateTime.Now.AddSeconds(5)));
                                    ply = null;
                                    lives--;

                                    if (lives > 0) {
                                        playerSpawnTime = DateTime.Now.AddSeconds(3);
                                    } else {
                                        gameState = enumGameState.end;
                                        clearLevel();
                                        k = -1; // skip to the end of the outer loop
                                        break;
                                    }
                                } else {
                                    if (a.GetType() == typeof(torpedo)) {
                                        // if torpedo hits asteroid then destroy it
                                        for (int i = 0; i < 6; i++) {
                                            particle p = new particle(DateTime.Now.AddSeconds(3));
                                            p.Position = ast.Position;
                                            p.Velocity = randvec2(-50, 50);
                                            VertexArray va = new VertexArray(PrimitiveType.Points, 1);
                                            va[0] = new Vertex(new Vector2f(), Color.White);
                                            p.Shape = va;                            
                                            Global.particles.Add(p);
                                        }
                                    }
                                }

                                try {
                                    if (k > j) {
                                        bodies.RemoveAt(k);
                                        bodies.RemoveAt(j);
                                    } else {
                                        bodies.RemoveAt(j);
                                        bodies.RemoveAt(k);
                                    }
                                    
                                    k--;
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
                
            }

            if (gameState == enumGameState.running) {
                if (ply != null) {
                    ply.update(delta);

                    if (Global.Keyboard["w"].justPressed) {
                        Global.sfx["thrust"].play(true);
                    }
                } else {
                    if (playerSpawnTime != null) {
                        if (DateTime.Now > playerSpawnTime) {
                            ply = new player();
                            bodies.Add(ply.ship);
                            playerSpawnTime = null;
                        }
                    }
                }

                // play the beat
                if (DateTime.Now > nextBeat && maxTargetsThisLevel > 0) {
                    int targets = countTargets();

                    nextBeat = DateTime.Now.AddSeconds(0.5f + ((float)targets / (float)maxTargetsThisLevel) * 0.5f);
                    if (lastBeat == 0) {
                        Global.sfx["beat2"].play();
                        lastBeat = 1;
                    } else {
                        Global.sfx["beat1"].play();
                        lastBeat = 0;
                    }
                }
                
                // If no asteroids or saucers are left then end level
                if (nextLevelTime == null) {
                    if (countTargets() == 0) { 
                        nextLevelTime = DateTime.Now.AddSeconds(1);
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

            foreach (particle p in Global.particles) {
                p.draw(window);
            }

            window.Draw(pointsText);

            // Display lives
            Vector2f livesStartOffset = new Vector2f(20, 60);
            Vector2f livesOffset = new Vector2f(24, 0);

            if (gameState == enumGameState.running) {
                for (int i = 0; i < lives; i++) {
                    VertexArray va = new VertexArray(models.SpaceShip);
                    for (uint j = 0; j < va.VertexCount; j++) {
                        va[j] = new Vertex(livesStartOffset + livesOffset * i + va[j].Position, Color.White);
                    }

                    window.Draw(va);
                }
            }

            // Display text for start and end game
            if (gameState == enumGameState.start) {
                Text textStart =new Text("Press space to start", Fonts.Hyperspace);
                textStart.Position = Global.ScreenSize / 2f;
                textStart.CharacterSize = 36;
                FloatRect textBounds = textStart.GetLocalBounds();
                textStart.Origin = new Vector2f(textBounds.Left + textBounds.Width / 2f,
                                                textBounds.Top + textBounds.Height / 2f);
                window.Draw(textStart);
            } else if (gameState == enumGameState.end) {
                Text textGameOver = new Text("Game Over", Fonts.Hyperspace);
                textGameOver.Position = Global.ScreenSize / 2f;
                textGameOver.CharacterSize = 36;
                FloatRect textBounds = textGameOver.GetLocalBounds();
                textGameOver.Origin = new Vector2f(textBounds.Left + textBounds.Width / 2f,
                                                   textBounds.Top + textBounds.Height / 2f);
                window.Draw(textGameOver);
            }

            // draw the ship's thrust
            if (ply != null) {
                ply.draw(window);
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