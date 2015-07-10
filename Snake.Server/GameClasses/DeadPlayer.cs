using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Snake.Server.GameClasses
{
    public class DeadPlayer : Player
    {

        public Player Source { get; set; }

        private Random r;
        private bool turningLeft;

        public DeadPlayer(Position[] points, Player src)
        {
            Source = src;
            _points = points.ToList();
            Alive = true;
            Length = points.Length;
            MovementLength = Config.data.BASE_MOVEMENT_LENGTH;
            MakeInvulnerable(1000);

            r = new Random();

            if (r.Next(2) == 1)
                turningLeft = true;
            else turningLeft = false;
            Color = Config.data.DEAD_SNAKE_COLOR;
            RealPlayer = false;

            if (Length > 1)
            {
                var dY = (points[1].Y - points[0].Y);
                var dX = (points[1].X - points[0].X);

                if (dX != 0)
                {
                    Heading = (int)(Math.Atan(dY / dX) * 180 / Math.PI);
                    if (dX > 0) Heading += 180;
                }
                else if (dY > 0) Heading = 90;
                else Heading = 270;
            }
            else Heading = 0;
            startTurn();
            setMoveTimer();
        }

        private void startTurn()
        {
            turnTimer = new Timer(1000 / Config.data.BASE_TURN_SPEED);
            turnTimer.Elapsed += (o, e) =>
            {
                if (turningLeft)
                    Heading--;
                else Heading++;

                if (r.NextDouble() < Config.data.DEAD_SNAKE_TURN_PROBABILITY)
                    turningLeft = !turningLeft;
            };
            turnTimer.Start();
        }
    }
}
