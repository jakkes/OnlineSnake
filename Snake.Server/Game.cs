using System;
using System.Collections.Generic;
using System.Timers;
using Snake.Server.GameClasses;
using Snake.Server.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Snake.Server
{
    public partial class Game
    {
        public Player[] Snakes
        {
            get { lock (_snakes) { return _snakes.ToArray(); } }
        }
        public Food[] Food
        {
            get { lock (_food) { return _food.ToArray(); } }
        }
        public Shot[] Shots
        {
            get { lock (_shots) return _shots.ToArray(); }
        }
        public Ammo[] Ammo
        {
            get { lock (_ammo) return _ammo.ToArray(); }
        }
        public Armor[] Armor
        {
            get { lock (_armor) return _armor.ToArray(); }
        }
        
        public int OnlineCount
        {
            get
            {
                return _onlineCount;
            }
            set
            {
                _onlineCount = value;
            }
        }

        public int BoardWidth
        {
            get
            {
                return (int)_boardWidth;
            }
        }
        public int BoardHeight
        {
            get
            {
                return (int)_boardHeight;
            }
        }

        public Config Settings { get; set; }

        private double SpawnTimeAdjustmentConstant
        {
            get
            {
                if (OnlineCount <= 1) return 1;
                return (1.0 / Math.Pow(OnlineCount,2));
            }
        }
        private int DesiredBoardWidth
        {
            get
            {
                if (OnlineCount <= 0) return Config.data.BASE_BOARD_WIDTH;
                return (int)(Config.data.BASE_BOARD_WIDTH * Math.Sqrt(OnlineCount));
            }
        }
        private int DesiredBoardHeight
        {
            get
            {
                if (OnlineCount <= 0) return Config.data.BASE_BOARD_HEIGHT;
                return (int)(Config.data.BASE_BOARD_HEIGHT * Math.Sqrt(OnlineCount));
            }
        }

        private double _boardWidth;
        private double _boardHeight;
        private int _onlineCount = 0;

        private Timer collisionTimer;
        private Timer foodTimer;
        private Timer ammoTimer;
        private Timer armorTimer;
        private Timer boardTimer;
        private Timer settingsCheckTimer;

        private List<Food> _food;
        private List<Player> _snakes;
        private List<Shot> _shots;
        private List<Ammo> _ammo;
        private List<Armor> _armor;

        private Random r = new Random();

        public Game()
        {
            Config.data = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Config.BaseFilePath + "Config.json"));
            Settings = Config.data;

            _snakes = new List<Player>();
            _food = new List<Food>();
            _shots = new List<Shot>();
            _ammo = new List<Ammo>();
            _armor = new List<Armor>();

            _boardHeight = Config.data.BASE_BOARD_HEIGHT;
            _boardWidth = Config.data.BASE_BOARD_WIDTH;

            SetFoodTimer();
            SetCollisionTimer();
            SetAmmoTimer();
            SetArmorTimer();
            SetBoardTimer();
            SetSettingsCheckTimer();
        }

        public void ReloadSettings()
        {
            Config.data = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Config.BaseFilePath + "Config.json"));
            Settings = Config.data;
        }

        #region SettingsCheck
        private void SetSettingsCheckTimer()
        {
            settingsCheckTimer = new Timer(2000);
            settingsCheckTimer.Elapsed += settingsCheckTimer_Elapsed;
            settingsCheckTimer.Start();
        }

        private void settingsCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _onlineCount = Snakes.Count(x => x.RealPlayer);
        }
        #endregion

        #region Snake
        private void AddSnake(Player s)
        {
            if (string.IsNullOrEmpty(s.Color))
                s.Color = Settings.COLORS[r.Next(Settings.COLORS.Length)];

            s.Died += snake_Died;
            s.Fire += s_Fire;
            s.Splitted += s_Splitted;

            lock (_snakes)
                _snakes.Add(s);
            if (s.RealPlayer)
                OnlineCount++;
        }

        private void snake_Died(object sender, EventArgs e)
        {
            lock (_snakes)
                _snakes.Remove((Player)sender);

            if (((Player)sender).RealPlayer)
            {
                Highscores.Add(new LeaderModel() { Name = ((Player)sender).Name, Score = ((Player)sender).Score });
                OnlineCount--;
            }
        }

        private void s_Fire(Player sender, Shot shot)
        {
            shot.Died += shot_Died;
            lock (_shots)
                _shots.Add(shot);
        }

        private void s_Splitted(Player sender, DeadPlayer deadPart)
        {
            AddSnake(deadPart);
        }
        #endregion

        #region Shots
        private void shot_Died(object sender, EventArgs e)
        {
            lock (_shots)
                _shots.Remove((Shot)sender);
        }
        #endregion

        #region Armor
        private void SetArmorTimer()
        {
            if (armorTimer == null)
            {
                armorTimer = new Timer(Config.data.ARMOR_SPAWN_TIME * SpawnTimeAdjustmentConstant);
                armorTimer.Elapsed += SpawnArmor;
            }
            armorTimer.Start();
        }

        private void SpawnArmor(object sender, ElapsedEventArgs e)
        {
            armorTimer.Interval = Config.data.ARMOR_SPAWN_TIME * SpawnTimeAdjustmentConstant;
            var a = new Armor(r.Next(DesiredBoardWidth), r.Next(DesiredBoardHeight));
            a.ArmorDied += armor_Died;
            lock (_armor)
                _armor.Add(a);
        }

        private void armor_Died(object sender, ElapsedEventArgs e)
        {
            lock (_armor)
                _armor.Remove((Armor)sender);
        }
        #endregion

        #region Food
        private void SetFoodTimer()
        {
            if (foodTimer == null)
            {
                foodTimer = new Timer(Config.data.FOOD_SPAWN_TIME);
                foodTimer.Elapsed += SpawnFood;
            }
            foodTimer.Start();
        }

        private void SpawnFood(object sender, ElapsedEventArgs e)
        {
            foodTimer.Interval = Config.data.FOOD_SPAWN_TIME * SpawnTimeAdjustmentConstant;
            var f = new Food(r.Next(DesiredBoardWidth), r.Next(DesiredBoardHeight));
            f.FoodDied += food_Died;
            lock (_food)
            {
                _food.Add(f);
            }
        }

        private void food_Died(object sender, ElapsedEventArgs e)
        {
            lock (_food)
                _food.Remove((Food)sender);
        }
        #endregion

        #region Ammo
        private void SetAmmoTimer()
        {
            ammoTimer = new Timer(Config.data.AMMO_SPAWN_TIME);
            ammoTimer.Elapsed += SpawnAmmo;
            ammoTimer.Start();
        }

        private void SpawnAmmo(object sender, ElapsedEventArgs e)
        {
            ammoTimer.Interval = Config.data.AMMO_SPAWN_TIME * SpawnTimeAdjustmentConstant;
            var ammo = new Ammo(r.Next(DesiredBoardWidth), r.Next(DesiredBoardHeight));
            ammo.Died += ammo_Died;
            lock (_ammo)
                _ammo.Add(ammo);
        }

        private void ammo_Died(object sender, ElapsedEventArgs e)
        {
            lock (_ammo)
                _ammo.Remove((Ammo)sender);
        }
        #endregion

        private void SetBoardTimer()
        {
            boardTimer = new Timer(100);
            boardTimer.Elapsed += (o, e) =>
            {
                if (_boardHeight < DesiredBoardHeight)
                    _boardHeight += 5;
                else if (_boardHeight > DesiredBoardHeight)
                    _boardHeight -= 1;

                if (_boardWidth < DesiredBoardWidth)
                    _boardWidth += 5;
                else if (_boardWidth > DesiredBoardWidth)
                    _boardWidth -= 1;

            };
            boardTimer.Start();
        }

        private void SetCollisionTimer()
        {
            if (collisionTimer == null)
            {
                collisionTimer = new Timer(Config.data.FOOD_SPAWN_TIME);
                collisionTimer = new Timer(1000 / Config.data.BASE_MOVEMENT_SPEED);
                collisionTimer.Elapsed += (o, e) => CollisionCheck();
            }
            collisionTimer.Start();
        }

        private void CollisionCheck()
        {
            var Snakes = this.Snakes;
            foreach (var s in Shots)
            {
                if (s.X < 0 || s.X > BoardWidth || s.Y < 0 || s.Y > BoardHeight)
                    s.Die();
            }

            foreach (var s in Snakes)
            {
                var points = s.Points;
                var head = s.Points[0];

                if (head.X < 0 || head.X > BoardWidth || head.Y < 0 || head.Y > BoardHeight)
                {
                    s.Die();
                    continue;
                }

                foreach (var o in Snakes)
                {
                    if (s == o) continue;
                    var op = o.Points;
                    for (int i = 0; i < op.Length; i++)
                    {
                        if (head.DistanceTo(op[i]) < Settings.SNAKE_RADIUS * 2)
                        {
                            s.Die(o);
                            continue;
                        }
                    }
                }

                foreach (var f in Food)
                {
                    if (head.DistanceTo(f) < Config.data.SNAKE_RADIUS + Config.data.FOOD_RADIUS)
                    {
                        f.Die();
                        s.Score++;
                        s.Grow(Config.data.FOOD_GROW);
                    }
                }

                foreach (var sh in Shots)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        if (sh.DistanceTo(points[i]) < Config.data.SHOT_RADIUS + Config.data.SNAKE_RADIUS)
                        {
                            s.Hit(i,sh);
                            break;
                        }
                    }
                }

                foreach (var ammo in Ammo)
                {
                    if (head.DistanceTo(ammo) < Config.data.AMMO_RADIUS + Config.data.SNAKE_RADIUS)
                    {
                        s.Ammo++;
                        ammo.Die();
                    }
                }

                foreach (var armor in Armor)
                {
                    if (head.DistanceTo(armor) < Config.data.ARMOR_RADIUS + Config.data.SNAKE_RADIUS)
                    {
                        s.Armor = true;
                        armor.Die();
                    }
                }
            }
        }

        private void Stop()
        {
            collisionTimer.Stop();
            collisionTimer.Dispose();

            foodTimer.Stop();
            foodTimer.Dispose();

            ammoTimer.Stop();
            ammoTimer.Dispose();
        }
    }

    public class GameException : Exception
    {

        public ExceptionType Type { get; set; }

        public GameException(ExceptionType type)
        {
            Type = type;
        }

        public enum ExceptionType
        {
            InvalidToken,
            Unknown,
            GameEmpty,
            GameFull
        }
    }
}
