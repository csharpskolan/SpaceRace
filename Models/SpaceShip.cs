using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceRace
{
    class SpaceShip
    {
        public SpaceShip()
        {
            Fuel = 100;
            Shield = 100;
        }

        #region Properties

        #region Shield
        private float _shield;

        public float Shield
        {
            get { return _shield; }
            set
            {
                _shield = value;
                if (_shield < 0)
                    _shield = 0;
            }
        }
        #endregion

        #region Fuel
        private float _fuel;

        public float Fuel
        {
            get { return _fuel; }
            set
            {
                _fuel = value;
                if (_fuel < 0)
                    _fuel = 0;
            }
        }
        #endregion

        public int Width { get; set; }
        public int Height { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }

        public float TurnRate { get; set; }

        private float _rotation;

        public bool RecalcHull { get; set; }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; RecalcHull = true; }
        }

        public bool Accelerating { get; set; }
        public bool Dead { get; set; }

        #endregion

        #region Public Methods

       public Dictionary<Point, bool> UpdateHull(List<Point> _hull)
        {
            //Initiera med beräknad längd för ökad prestanda
            var _hullTransformed = new Dictionary<Point, bool>(_hull.Count);

            //Beräkna rotationen
            float cos = (float)Math.Cos(Rotation);
            float sin = (float)Math.Sin(Rotation);
            //Center för skeppet
            int width = Width / 2;
            int height = Height / 2;

            foreach (Point p in _hull)
            {
                //Beräkna nytt x o y kring centrum
                int newX = (int)((p.X - width) * cos - (p.Y - height) * sin);
                int newY = (int)((p.Y - height) * cos + (p.X - width) * sin);

                Point newP = new Point(newX, newY);
                //Punkten kan redan finnas p.g.a avrundning
                if (!_hullTransformed.ContainsKey(newP))
                    _hullTransformed.Add(newP, true);
            }

            RecalcHull = false;
            return _hullTransformed;
        }


        #endregion
    }
}
