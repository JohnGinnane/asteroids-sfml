using SFML.System;
using static asteroids.util;

namespace asteroids {
    public class saucer {
        private polybody saucerShip;
        public polybody SaucerShip {
            get { return saucerShip; }
            set { saucerShip = value; }
        }

        private float fireCooldown = 1000f;
        DateTime lastFire;
        private float maxMoveSpeed = 30f;
        private float torpedoSpeed = 200f;
        private DateTime nextMove;

        public enum enumSaucerType {
            small,
            large
        }

        private enumSaucerType saucerType;
        public enumSaucerType SaucerType { get { return saucerType; } }

        public saucer() {
            saucerType = (enumSaucerType)randint(0, 1);
            
            if (saucerType == enumSaucerType.small) {
                this.SaucerShip = new polybody(scale(models.FlyingSaucer, 0.5f));
                this.SaucerShip.Position = new Vector2f(Global.ScreenSize.X - 1,
                                                        randfloat(10f, Global.ScreenSize.Y - 10f));
                this.SaucerShip.Velocity = new Vector2f(-maxMoveSpeed, 0);
            } else {
                this.SaucerShip = new polybody(models.FlyingSaucer);
                this.SaucerShip.Position = new Vector2f(1, randfloat(10f, Global.ScreenSize.Y - 10f));
                this.SaucerShip.Velocity = new Vector2f(maxMoveSpeed, 0);
            }

            this.SaucerShip.Parent = this;
            this.SaucerShip.Drag = 0f;
            this.lastFire = DateTime.Now;
        }

        public void update(float delta) {
            this.SaucerShip.update(delta);

            // only wrap from top to bottom
            if (this.SaucerShip.Position.Y < 0) { this.SaucerShip.SetYPosition(Global.ScreenSize.Y); }
            if (this.SaucerShip.Position.Y > Global.ScreenSize.Y) { this.SaucerShip.SetYPosition(0); }

            if (DateTime.Now > nextMove) {
                nextMove = DateTime.Now.AddSeconds(randint(1, 3));

                this.SaucerShip.SetYVelocity(randfloat(10, 30) * Math.Sign(randfloat(-1, 1)));
            }
        }

        public torpedo? fire(body? target = null) {
            if (lastFire.AddMilliseconds(fireCooldown) > DateTime.Now) {
                return null;
            }

            lastFire = DateTime.Now;
            Global.sfx["fire"].play();

            // fire directly at the player
            Vector2f dir;

            if (target == null) {
                dir = normalise(randvec2(-1, 1));
            } else {
                dir = normalise(target.Position - this.SaucerShip.Position);
            }

            Vector2f firePos = this.SaucerShip.Position + dir * (this.SaucerShip.BoundingCircleRadius + 5f);
            Vector2f fireVel = this.SaucerShip.Velocity + dir * torpedoSpeed;

            torpedo t = new torpedo(firePos, fireVel);
            return t;
        }
    }
}