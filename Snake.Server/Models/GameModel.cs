using Snake.Server.GameClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.Models
{
    public class GameModel
    {
        public List<SnakeModel> Snakes { get; set; }
        public List<List<Position>> Borders { get; set; }
        public List<Position> Food { get; set; }
        public List<Position> Shots { get; set; }
        public List<Position> Ammo { get; set; }
        public List<Position> Armor { get; set; }
        public int Score { get; set; }
        public string Name { get; set; }
        public int AmmoCount { get; set; }
        public int BoostStored { get; set; }
        public bool PlayerArmor { get; set; }

        public GameModel()
        {
            Snakes = new List<SnakeModel>();
            Borders = new List<List<Position>>();
            Food = new List<Position>();
            Shots = new List<Position>();
            Ammo = new List<Position>();
            Armor = new List<Position>();
        }
    }
}
