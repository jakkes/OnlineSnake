using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Snake.Server.Models;

namespace Snake.Server.GameClasses
{
    public class Player
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public double X { get { return Head.X; } }
        public double Y { get { return Head.Y; } }
        public Position Head { get { return Points.FirstOrDefault(); } }
        public TurnState Turn { get; set; }
        public int Length { get; set; }
        public int Score
        {
            get { return _score; }
            set
            {
                if (value < 0)
                    _score = 0;
                else
                    _score = value;
                if (ScoreChanged != null)
                    ScoreChanged(this,null);
            }
        }
        public bool Invulnerable
        {
            get { return _invulnerable; }
        }
        public bool RealPlayer { get; set; }
        public Position[] Points
        {
            get { lock (_points) { return _points.ToArray(); } }
        }
        public int Heading { get; set; }
        public int TurnSpeed { get; set; }
        public int MovementSpeed { get; set; }
        public int MovementLength { get; set; }
        public int Ammo { get; set; }
        public string Color { get; set; }
        public int BoostStored { get; set; }
        public bool Boost
        {
            get { return _boost; }
            set
            {
                _boost = value;
                if (_boost)
                {
                    BoostStored -= 100;
                    if (BoostStored > 0)
                    {
                        boostReplenishTimer.Stop();
                        MovementLength = (int)(Config.data.BASE_MOVEMENT_LENGTH * 1.5);
                        boostTimer.Start();
                    }
                }
                else
                {
                    boostTimer.Stop();
                    MovementLength = Config.data.BASE_MOVEMENT_LENGTH;
                    boostReplenishTimer.Start();
                }
            }
        }
        public bool Armor { get { return _armor; } set { _armor = value; } }

        public delegate void FireHandler(Player sender, Shot shot);
        public delegate void SplitHandler(Player sender, DeadPlayer deadPart);

        public event SplitHandler Splitted;
        public event FireHandler Fire;
        public event EventHandler Grew;
        public event EventHandler Died;
        public event EventHandler ScoreChanged;

        protected List<Position> _points;

        protected Timer moveTimer;
        protected Timer turnTimer;
        protected Timer invulnerableTimer;
        protected Timer boostReplenishTimer;
        protected Timer boostTimer;

        private int _score = 0;
        private bool _boost;
        private bool _armor = false;
        private bool _invulnerable = false;

        public Player() { }

        public Player(string Token, string Name, double X, double Y)
        {
            var r = new Random();

            _points = new List<Position>();
            _points.Add(new Position(X,Y));

            BoostStored = 0;
            TurnSpeed = Config.data.BASE_TURN_SPEED;
            MovementSpeed = Config.data.BASE_MOVEMENT_SPEED;
            MovementLength = Config.data.BASE_MOVEMENT_LENGTH;
            Ammo = 0;
            Heading = r.Next(360);
            this.Token = Token;
            this.Name = Name;
            Length = Config.data.BASE_LENGTH;
            RealPlayer = true;
            Score = 0;

            setMoveTimer();
            setTurnTimer();
            setBoostTimer();
        }

        protected void setBoostTimer()
        {
            boostReplenishTimer = new Timer(100);
            boostReplenishTimer.Elapsed += (o, e) =>
            {
                BoostStored += 25;
                if (BoostStored >= Config.data.BOOST_CAP)
                {
                    BoostStored = Config.data.BOOST_CAP;
                    boostReplenishTimer.Stop();
                }
            };

            boostTimer = new Timer(100);
            boostTimer.Elapsed += (o, e) =>
            {
                BoostStored -= 100;
                if (BoostStored <= 0)
                {
                    BoostStored = 0;
                    Boost = false;
                }
            };

            Boost = false;
        }

        protected void setMoveTimer()
        {
            moveTimer = new Timer(1000 / Config.data.BASE_MOVEMENT_SPEED);
            moveTimer.Elapsed += moveTimer_Elapsed;
            moveTimer.Start();
        }

        protected void setTurnTimer()
        {
            turnTimer = new Timer(1000 / Config.data.BASE_TURN_SPEED);
            turnTimer.Elapsed += (o, e) =>
            {
                if (Turn == TurnState.Right)
                    Heading++;
                else if (Turn == TurnState.Left)
                    Heading--;
            };
            turnTimer.Start();
        }

        public void Shoot()
        {
            if (Ammo <= 0) return;

            Ammo--;
            if (Fire != null)
                Fire(this, new Shot(this));
        }

        public void Hit(int index, Shot src)
        {
            if(index == Points.Length - 1 || index == 0) return;
            if (!Armor)
            {
                if (index < Config.data.BASE_LENGTH)
                {
                    Die(src.Source);
                    src.Die();
                    return;
                }

                var d = new DeadPlayer(Points.Skip(index).ToArray(), this);

                Score -= (int)Math.Floor((double)(Length - index - 1) / 2 / Config.data.FOOD_GROW);
                src.Source.Score += (int)Math.Floor((double)(Length - index - 1) / 3 / Config.data.FOOD_GROW);
                Length = index + 1;

                src.Die();

                if (Splitted != null)
                    Splitted(this, d);
            }
            else
            {
                int h = Points[index - 1].HeadingTo(Points[index + 1]);
                src.Heading = h - (src.Heading - h);
                Armor = false;
            }
        }

        public void Grow()
        {
            Grow(1);
        }

        public void Grow(int Amount)
        {
            Length += Amount;
            if (Grew != null)
                Grew(this, null);
        }

        public void MakeInvulnerable(int Time)
        {
            invulnerableTimer = new Timer(Time);
            invulnerableTimer.Elapsed += (o, e) =>
            {
                _invulnerable = false;
                invulnerableTimer.Stop();
                invulnerableTimer.Dispose();
            };
            _invulnerable = true;
            invulnerableTimer.Start();
        }

        public void Die(Player source)
        {
            if (Die())
            {
                if(RealPlayer)
                    source.Score += 5;
                else
                {
                    if (((DeadPlayer)this).Source != this)
                    {
                        source.Score += 5;
                    }
                }
            }
        }

        public bool Die()
        {
            if (Invulnerable) return false;

            if (moveTimer != null)
            {
                moveTimer.Stop();
                moveTimer.Dispose();
            }
            if (turnTimer != null)
            {
                turnTimer.Stop();
                turnTimer.Dispose();
            }

            if (Died != null)
                Died(this, null);

            return true;
        }

        private void moveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_points.Count == 0) { Die(); return; }
            lock (_points)
            {
                _points.Insert(0, new Position(Head.X + MovementLength * Math.Cos(Math.PI / 180 * Heading), Head.Y + MovementLength * Math.Sin(Math.PI / 180 * Heading)));
                if (_points.Count > Length)
                    _points.RemoveRange(Length, _points.Count - Length);
            }
            for (int i = 10; i < Points.Length; i++)
            {
                if (Head.DistanceTo(Points[i]) < Config.data.SNAKE_RADIUS * 2)
                {
                    Die();
                    return;
                }
            }
        }

        public enum TurnState
        {
            None, 
            Left, 
            Right
        }
    }
}