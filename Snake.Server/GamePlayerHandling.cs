using Snake.Server.GameClasses;
using Snake.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Snake.Server.Extensions;
using System.IO;
using Newtonsoft.Json;

namespace Snake.Server
{
    public partial class Game
    {
        public void Shoot(string Token)
        {
            GetPlayer(Token).Shoot();
        }

        public void Join(string Token, string Name)
        {
            if (Snakes.Any(x => x.Token == Token))
                return;
            var r = new Random();
            AddSnake(new Player(Token, Name, r.Next(DesiredBoardWidth),r.Next(DesiredBoardHeight)));
        }

        public GameLoopModel GetLoopData(string Token, LoopRequestModel model)
        {
            Player snake = GetPlayer(Token);

            snake.Turn = model.Turn;
            if (snake.Boost != model.Boost)
                snake.Boost = model.Boost;

            var Head = snake.Head;
            GameLoopModel r = new GameLoopModel();
            r.Score = snake.Score;
            r.Name = snake.Name;
            r.AmmoCount = snake.Ammo;
            r.BoostStored = snake.BoostStored;
            r.PlayerArmor = snake.Armor;

            #region Snakes
            foreach (var s in Snakes)
            {
                SnakeModel m = new SnakeModel() { Color = s.Color, Name = s.Name };
                m.Points = new List<TranslatedPosition>();
                bool linked = true;
                foreach (var p in s.Points)
                {
                    if (!linked)
                    {
                        r.Snakes.Add(m);
                        m = new SnakeModel() { Color = s.Color, Name = s.Name };
                        m.Points = new List<TranslatedPosition>();
                        linked = true;
                    }
                    if (p.DistanceTo(Head) < Config.data.VISIBILITY_RADIUS)
                        m.Points.Add(p.TranslatePosition(Head));
                    else if(m.Points.Count != 0) linked = false;
                }

                if (m.Points.Count != 0)
                    r.Snakes.Add(m);
            }
            #endregion

            #region Borders
            if (Head.X + Config.data.VISIBILITY_RADIUS > BoardWidth)
            {
                r.Borders.Add(new List<Position>()
                {
                    new BorderPosition(BoardWidth, Head.Y + Math.Sqrt(Math.Pow(Config.data.VISIBILITY_RADIUS, 2) - Math.Pow(BoardWidth - Head.X,2))),
                    new BorderPosition(BoardWidth, Head.Y - Math.Sqrt(Math.Pow(Config.data.VISIBILITY_RADIUS, 2) - Math.Pow(BoardWidth - Head.X,2)))
                }.TranslateList(Head));
            }
            if (Head.X - Config.data.VISIBILITY_RADIUS < 0)
            {
                r.Borders.Add(new List<Position>()
                {
                    new BorderPosition(0, Head.Y + Math.Sqrt(Math.Pow(Config.data.VISIBILITY_RADIUS, 2) - Math.Pow(Head.X,2))),
                    new BorderPosition(0, Head.Y - Math.Sqrt(Math.Pow(Config.data.VISIBILITY_RADIUS, 2) - Math.Pow(Head.X,2)))
                }.TranslateList(Head));
            }
            if (Head.Y + Config.data.VISIBILITY_RADIUS > BoardHeight)
            {
                r.Borders.Add(new List<Position>()
                {
                    new BorderPosition(Head.X + Math.Sqrt(Math.Pow(Config.data.VISIBILITY_RADIUS, 2) - Math.Pow(BoardHeight - Head.Y, 2)), BoardHeight),
                    new BorderPosition(Head.X - Math.Sqrt(Math.Pow(Config.data.VISIBILITY_RADIUS, 2) - Math.Pow(BoardHeight - Head.Y, 2)), BoardHeight)
                }.TranslateList(Head));
            }
            if (Head.Y - Config.data.VISIBILITY_RADIUS < 0)
            {
                r.Borders.Add(new List<Position>()
                {
                    new BorderPosition(Head.X + Math.Sqrt(Math.Pow(Config.data.VISIBILITY_RADIUS, 2) - Math.Pow(Head.Y, 2)), 0),
                    new BorderPosition(Head.X - Math.Sqrt(Math.Pow(Config.data.VISIBILITY_RADIUS, 2) - Math.Pow(Head.Y, 2)), 0)
                }.TranslateList(Head));
            }
            #endregion

            #region Food
            foreach (var f in Food)
                if (Head.DistanceTo(f) < Config.data.VISIBILITY_RADIUS)
                    r.Food.Add(f.TranslatePosition(Head));
            #endregion

            #region Shots
            foreach (var s in Shots)
                if (Head.DistanceTo(s) < Config.data.VISIBILITY_RADIUS)
                    r.Shots.Add(s.TranslatePosition(Head));
            #endregion

            #region Ammo
            foreach (var ammo in Ammo)
                if (ammo.DistanceTo(Head) < Config.data.VISIBILITY_RADIUS)
                    r.Ammo.Add(ammo.TranslatePosition(Head));
            #endregion

            #region Armor
            foreach (var ammo in Armor)
                if (ammo.DistanceTo(Head) < Config.data.VISIBILITY_RADIUS)
                    r.Armor.Add(ammo.TranslatePosition(Head));
            #endregion
            return r;
        }

        public OnlineDataModel GetOnlineData(string token)
        {
            return new OnlineDataModel()
            {
                Leaderboard = Highscores.Leaderboard,
                CurrentLeader = new LeaderModel()
                {
                    Score = Snakes.Max(x => x.Score),
                    Name = Snakes.First(x => x.Score == Snakes.Max(c => c.Score)).Name
                },
                OnlineCount = OnlineCount,
                Players = Snakes.Where(x => x.RealPlayer == true).Select(x => new PlayerModel() { Name = x.Name, Color = x.Color }).ToList(),
                PersonalHighscore = Highscores.PersonalHighscore(GetPlayer(token).Name).Score
            };
        }

        private Player GetPlayer(string Token)
        {
            var p = Snakes.FirstOrDefault(x => x.Token == Token);
            if (p != null)
                return p;
            else
                throw new GameException(GameException.ExceptionType.InvalidToken);
        }
    }
}
