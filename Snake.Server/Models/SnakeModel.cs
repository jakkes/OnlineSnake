using Snake.Server.GameClasses;
using System.Collections.Generic;

namespace Snake.Server.Models
{
    public class SnakeModel
    {
        public List<TranslatedPosition> Points { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
    }
}
