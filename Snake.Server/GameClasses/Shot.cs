using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Snake.Server.GameClasses
{
    public class Shot : Position
    {

        public int Heading { get; set; }
        public Player Source { get; set; }

        public event EventHandler Died;

        private Timer movementTimer;

        public Shot(Player shooter)
        {
            Source = shooter;
            Heading = shooter.Heading;
            X = shooter.X + 2 * (Config.data.SNAKE_RADIUS + Config.data.SHOT_RADIUS) * Math.Cos(Math.PI * Heading / 180);
            Y = shooter.Y + 2 * (Config.data.SNAKE_RADIUS + Config.data.SHOT_RADIUS + 1) * Math.Sin(Math.PI * Heading / 180);

            movementTimer = new Timer(1000 / Config.data.SHOT_MOVEMENT_SPEED);
            movementTimer.Elapsed += movementTimer_Elapsed;
            movementTimer.Start();
        }

        public void Die()
        {
            movementTimer.Stop();
            movementTimer.Dispose();

            if (Died != null)
                Died(this, null);
        }

        private void movementTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.X += Config.data.SHOT_MOVEMENT_LENGTH * Math.Cos(Heading * Math.PI / 180);
            this.Y += Config.data.SHOT_MOVEMENT_LENGTH * Math.Sin(Heading * Math.PI / 180);
        }
    }
}
