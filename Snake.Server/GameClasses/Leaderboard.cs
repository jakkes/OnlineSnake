using Newtonsoft.Json;
using Snake.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Server.GameClasses
{
    public class Highscores
    {

        public static List<LeaderModel> Leaderboard
        {
            get
            {
                if (_board == null)
                    _board = JsonConvert.DeserializeObject<List<LeaderModel>>(File.ReadAllText(Config.LeaderboardFile));
                return _board;
            }
            set
            {
                _board = value;
                try
                {
                    File.WriteAllText(Config.LeaderboardFile,JsonConvert.SerializeObject(value));
                }
                catch
                {

                }
            }
        }
        public static List<LeaderModel> PersonalHighscores
        {
            get
            {
                if (_personal == null)
                    _personal = JsonConvert.DeserializeObject<List<LeaderModel>>(File.ReadAllText(Config.PersonalHighscoreFile));
                return _personal;
            }
            set
            {
                _personal = value;
                try { File.WriteAllText(Config.PersonalHighscoreFile, JsonConvert.SerializeObject(value)); }
                catch { }
            }
        }

        private static List<LeaderModel> _personal;
        private static List<LeaderModel> _board;
        
        public static void Add(LeaderModel model)
        {
            var l = Leaderboard;
            if (l.Any(x => x.Score < model.Score))
            {
                for (int i = 0; i < l.Count; i++)
                    if (l.ElementAt(i).Score < model.Score)
                    {
                        l.Insert(i, model);
                        break;
                    }
                while (l.Count > 10)
                    l.RemoveAt(10);
                Leaderboard = l;
            }
            l = PersonalHighscores;
            LeaderModel a = l.FirstOrDefault(x => x.Name == model.Name);
            if (a != null && a.Score < model.Score)
            {
                l.Remove(a);
                l.Add(model);
                PersonalHighscores = l;
            }
            else if (a == null) { l.Add(model); PersonalHighscores = l; }
        }

        public static LeaderModel PersonalHighscore(string Name)
        {
            foreach (var p in PersonalHighscores)
                if (p.Name == Name)
                    return p;
            return new LeaderModel() { Name = Name, Score = 0 };
        }
    }
}
