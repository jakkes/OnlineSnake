using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Snake.Server.GameClasses
{
    public class Armor : Position
    {
        public int Remaining { get { return (int)(DateTime.Now - _startTime).TotalSeconds; } }

        public event ElapsedEventHandler ArmorDied;

        private DateTime _startTime;
        private Timer lastingTimer;

        public Armor(double X, double Y)
        {
            base.X = X;
            base.Y = Y;

            lastingTimer = new Timer(Config.data.ARMOR_DURATION);
            lastingTimer.Elapsed += lastingTimer_Elapsed;
            lastingTimer.Start();
            _startTime = DateTime.Now;
        }

        public void Die()
        {
            lastingTimer.Stop();
            lastingTimer.Dispose();

            if (ArmorDied != null)
                ArmorDied(this, null);
        }

        private void lastingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Die();
        }
    }
}
