using System;
using System.Timers;

namespace Snake.Server.GameClasses
{
    public class Ammo : Position
    {
        public event ElapsedEventHandler Died;

        private Timer t;

        public Ammo(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
            t = new Timer(Config.data.AMMO_DURATION);
            t.Elapsed += t_Elapsed;
            t.Start();
        }

        public void Die()
        {
            t.Stop();
            t.Dispose();

            if (Died != null)
                Died(this, null);
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Die();
        }
    }
}
