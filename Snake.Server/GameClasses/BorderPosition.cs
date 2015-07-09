using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.GameClasses
{
    public class BorderPosition : Position
    {
        public BorderPosition(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
