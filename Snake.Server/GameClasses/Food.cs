using Snake.Server.Models;
using System;
using System.Timers;

namespace Snake.Server.GameClasses
{
    public class Food : Position
    {

        public bool Alive { get; set; }
        public int Remaining { get { return (int)(DateTime.Now - _startTime).TotalSeconds; } }

        public event ElapsedEventHandler FoodDied;

        private DateTime _startTime;
        private Timer lastingTimer;

        public Food(double X, double Y)
        {
            Alive = true;
            base.X = X;
            base.Y = Y;

            lastingTimer = new Timer(Config.data.FOOD_DURATION);
            lastingTimer.Elapsed += lastingTimer_Elapsed;
            lastingTimer.Start();
            _startTime = DateTime.Now;
        }

        public void Die()
        {
            Alive = false;

            lastingTimer.Stop();
            lastingTimer.Dispose();

            if (FoodDied != null)
                FoodDied(this, null);
        }

        private void lastingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Die();
        }
    }
}
