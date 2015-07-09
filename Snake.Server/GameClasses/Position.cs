using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.GameClasses
{
    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Position() { }
        public Position(double X, double Y) { this.X = X; this.Y = Y; }

        public double DistanceTo(Position position)
        {
            if (position == null) return double.MaxValue;
            return Math.Sqrt((position.X - X) * (position.X - X) + (position.Y - Y) * (position.Y - Y));
        }

        public int HeadingTo(Position pos)
        {
            var dY = (pos.Y - this.Y);
            var dX = (pos.X - this.X);

            int Heading;

            if (dX != 0)
            {
                Heading = (int)(Math.Atan(dY / dX) * 180 / Math.PI);
                if (dX > 0) Heading += 180;
            }
            else if (dY > 0) Heading = 90;
            else Heading = 270;

            return Heading;
        }

        public bool Equals(Object o)
        {
            if (o is Position)
                return this == (Position)o;
            else return false;
        }

        public static bool operator ==(Position one, Position two)
        {
            if (System.Object.ReferenceEquals(one, two))
            {
                return true;
            }
            if (((object)one == null) || ((object)two == null))
            {
                return false;
            }
            return (one.X == two.X && one.Y == two.Y);
        }

        public static bool operator !=(Position one, Position two)
        {
            if (System.Object.ReferenceEquals(one, two))
            {
                return false;
            }
            if (((object)one == null) || ((object)two == null))
            {
                return true;
            }
            return !(one.X == two.X && one.Y == two.Y);
        }

        public static Position operator -(Position one, Position two)
        {
            return new Position(one.X - two.X, one.Y - two.Y);
        }
    }
}
