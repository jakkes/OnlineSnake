using Snake.Server.GameClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.Extensions
{
    public static class ExtensionMethods
    {
        public static List<TranslatedPosition> TranslateList(this List<Position> list, Position Reference)
        {
            var l = new List<TranslatedPosition>();
            foreach (var p in list)
            {
                l.Add(p.TranslatePosition(Reference));
            }
            return l;
        }

        public static TranslatedPosition TranslatePosition(this Position pos, Position Reference)
        {
            return new TranslatedPosition(pos,Reference);
        }
    }
}
