using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.GameClasses
{
    public class TranslatedPosition
    {

        public int X { get; set; }
        public int Y { get; set; }

        public TranslatedPosition(int x, int y, Position reference) : this(new Position(x, y), reference)
        {
            
        }

        public TranslatedPosition(Position p, Position reference)
        {
            X = (int)(reference.X - p.X);
            Y = (int)(reference.Y - p.Y);
        }
    }
}
