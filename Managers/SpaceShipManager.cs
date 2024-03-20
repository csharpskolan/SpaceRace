using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using SpaceRace.Messages;
using SpaceRace.Managers;

namespace SpaceRace
{
    class SpaceShipManager : ManagerBase
    {
        SpriteBatch spriteBatch;
        Texture2D _gfx, _gfx_acc, _gfx_explosion;
        List<Point> _hull;
        Dictionary<Point, bool> _hullTransformed;
        KeyboardState prev_ks;
        private CameraManager cameraManager;
        SoundEffectInstance engineSound;

        bool _collision = false;
        bool _takingDamage = false;
        bool exploding = false;
        TimeSpan anim_time = new TimeSpan();
        int anim_frame = 0;

        #region Properties

        public Vector2 StartPositionOffset
        {
            get { return new Vector2(0, _gfx.Height / 2 + 3); }
        }

        public SpaceShip Ship { get; set; }

        #endregion

        public SpaceShipManager(Game game) : base(game)
        {
            Ship = new SpaceShip();
        }

        #region Overrides

        public override GameState SupportedStates
        {
            get { return GameState.GAMEOVER | GameState.PLAYING | GameState.READY | GameState.WON; }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _gfx = Game.Content.Load<Texture2D>("ship");
            _gfx_acc = Game.Content.Load<Texture2D>("ship_acc");
            _gfx_explosion = Game.Content.Load<Texture2D>("explosion");

            var sound = Game.Content.Load<SoundEffect>("Sounds\\throttle");
            engineSound = sound.CreateInstance();
            engineSound.IsLooped = true;
            
            Ship.Width = _gfx.Width;
            Ship.Height = _gfx.Height;

            _hull = Utility.FindHull(_gfx);
            _hullTransformed = Ship.UpdateHull(_hull);
            base.LoadContent();
        }

        public override void Initialize()
        {
            cameraManager = ServiceLocator.GetService<CameraManager>();
            Components = new[] { new HUDComponent(Game) { Ship = Ship } };

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (Physics.GameState != GameState.PLAYING)
                return;

            KeyboardState ks = Keyboard.GetState();

            //Tryckte vi ned några knappar?
            if ((ks.IsKeyDown(Keys.A) && !prev_ks.IsKeyDown(Keys.A))
                || (ks.IsKeyDown(Keys.A) && !ks.IsKeyDown(Keys.D)))
                Ship.TurnRate = -0.03f;
            else if ((ks.IsKeyDown(Keys.D) && !prev_ks.IsKeyDown(Keys.D))
                || (ks.IsKeyDown(Keys.D) && !ks.IsKeyDown(Keys.A)))
                Ship.TurnRate = 0.03f;
            else if ((!ks.IsKeyDown(Keys.D) && prev_ks.IsKeyDown(Keys.D)) ||
                (!ks.IsKeyDown(Keys.A) && prev_ks.IsKeyDown(Keys.A)))
                Ship.TurnRate = 0;

            Ship.Accelerating = ks.IsKeyDown(Keys.W);

            if (!exploding)
            {
                if (Ship.Accelerating && Ship.Fuel > 0)
                {
                    Ship.Speed += Physics.Gravity + Vector2.Transform(new Vector2(0, -0.03f), Matrix.CreateRotationZ(Ship.Rotation));
                    Ship.Fuel -= 0.1f;

                    //Update sound
                    if (engineSound.State == SoundState.Stopped)
                        engineSound.Play();
                    else if (engineSound.State == SoundState.Paused)
                        engineSound.Resume();
                }
                else
                {
                    if (engineSound.State == SoundState.Playing)
                        engineSound.Pause();
                    Ship.Accelerating = false;
                    Ship.Speed += Physics.Gravity;
                }

                if (!_collision)
                    Ship.Rotation += Ship.TurnRate;

                if (_takingDamage)
                    Ship.Shield -= Ship.Speed.Length() * 30.0f;

                if (Ship.Shield <= 0)
                {
                    //SoundManager.PlayCue("explosion");
                    exploding = true;
                    return;
                }

                Ship.Position += Ship.Speed;

                if (Ship.RecalcHull)
                    _hullTransformed = Ship.UpdateHull(_hull);
            }
            else
            {
                if (engineSound.State == SoundState.Playing)
                    engineSound.Pause();

                anim_time += gameTime.ElapsedGameTime;
                if (anim_time.TotalMilliseconds > 50)
                {
                    anim_time = new TimeSpan();
                    anim_frame++;
                    if (anim_frame > 15)
                        Ship.Dead = true;
                }
            }

            cameraManager.CameraPosition = Ship.Position;
            prev_ks = ks;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Physics.GameState != GameState.READY && Physics.GameState != GameState.PLAYING)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            if (!exploding)
            {
                if (Ship.Accelerating)
                    //spriteBatch.Draw(_gfx_acc, Ship.Position, null, Color.White, Ship.Rotation,
                    //    new Vector2(_gfx.Width / 2, _gfx.Height / 2), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(_gfx_acc, cameraManager.TransformWorldToScreen(Ship.Position), null, Color.White, Ship.Rotation, new Vector2(_gfx.Width / 2, _gfx.Height / 2), cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                else
                    //spriteBatch.Draw(_gfx, Ship.Position, null, Color.White, Ship.Rotation,
                    //    new Vector2(_gfx.Width / 2, _gfx.Height / 2), 1.0f, SpriteEffects.None, 0);
                    spriteBatch.Draw(_gfx, cameraManager.TransformWorldToScreen(Ship.Position), null, Color.White, Ship.Rotation, new Vector2(_gfx.Width / 2, _gfx.Height / 2), cameraManager.ScaleFactor, SpriteEffects.None, 0f);
            }
            else
            {
                Rectangle tmp = new Rectangle((anim_frame % 4) * 64, (anim_frame / 4) * 64, 64, 64);
                spriteBatch.Draw(_gfx_explosion, Ship.Position, tmp, new Color(255, 255, 255, 127), 0,
                        new Vector2(64 / 2, 64 / 2), 1.0f, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        public void ResetShip(Vector2 position)
        {
            anim_frame = 0;
            exploding = false;
            Ship.Shield = 100;
            Ship.Fuel = 100;
            Ship.Dead = false;
            Ship.Rotation = 0;
            Ship.TurnRate = 0;
            _hullTransformed = Ship.UpdateHull(_hull);
            Ship.Speed = new Vector2();
            Ship.Position = position - this.StartPositionOffset;
            cameraManager.CameraPosition = Ship.Position;
        }

        const UInt32 _black = 0xFF000000;
        const UInt32 _red = 0xFF0000FF;
        const UInt32 _green = 0xFF00FF00;
        const UInt32 _blue = 0xFFFF0000;
        const UInt32 _trans = 0x00000000;

        public bool CheckCollision(Texture2D level, Texture2D background)
        {
            uint[] levelPixels = new uint[level.Width * level.Height];
            uint[] backgroundPixels = new uint[background.Width * background.Height];

            level.GetData<uint>(levelPixels);
            background.GetData<uint>(backgroundPixels);
            Dictionary<Point, bool>.Enumerator enumer = _hullTransformed.GetEnumerator();

            //Eftersom vi kommer att gå igenom alla pixlar som kolliderar
            //så behöver extra temporära variabler för att kunna räkna ut
            //ett medelvärde längre fram
            int num_found = 0;
            Vector2 delta_pos = new Vector2();
            float delta_angle = 0;

            bool landed = false;
            _takingDamage = false;

            while (enumer.MoveNext())
            {
                //Beräkna pixelpositon för kantpixeln
                Point p = enumer.Current.Key;
                int x = p.X + (int)Ship.Position.X;
                int y = p.Y + (int)Ship.Position.Y;

                //Pixlar utanför banan ignoreras
                if (x < 0 || y < 0 || x >= level.Width || y >= level.Height)
                    continue;

                ////Endast svarta, gröna och blåa pixlar kan vi kollidera mot
                if ((levelPixels[x + y * level.Width] != _trans))
                {

                    //Ska vi ta skada? Jo om vi kolliderar med svart
                    _takingDamage = (levelPixels[x + y * level.Width] == _black);

                    //Vektor från centrum (hävstång)
                    Vector2 c = new Vector2(Ship.Position.X - x, Ship.Position.Y - y);
                    //Roterad vektor 90 grader moturs
                    Vector2 h = new Vector2(-c.Y, c.X);
                    //Beräkna rotation (skalad skalärprodukt)
                    float rot = (h.X * Ship.Speed.X + h.Y * Ship.Speed.Y) / 600.0f;
                    delta_angle += rot;

                    //Flytta tillbaka skeppet vid första tecken på kollision
                    //för att förhindra att skeppet fastar
                    if (num_found == 0)
                        Ship.Position -= Ship.Speed;

                    //Vektor som pekar mot centrum för skeppet
                    //se artikelbild för bättre förklaring
                    Vector2 D = new Vector2(x - (Ship.Position.X), y - (Ship.Position.Y));
                    D.Normalize();
                    Vector2 V = Ship.Speed;
                    float length = Vector2.Dot(D, V);
                    Vector2 pl = length * D;
                    Vector2 np = V - pl;
                    //Flytta isär lite extra i riktning från kollisionen
                    delta_pos -= (V.Length() + 0.05f) * D;

                    //Beräkna dämpning beroende på pixelfärg
                    float dampening = 0.8f;
                    if ((backgroundPixels[x + y * level.Width] == _green) ||
                    (backgroundPixels[x + y * level.Width] == _blue))
                        dampening = 0.4f;

                    //Ny hastighet
                    Ship.Speed = (-pl + np) * dampening;
                    num_found++;

                    //Kolla om vi har landat på mål
                    if ((backgroundPixels[x + y * level.Width] == _blue)
                        && Ship.Speed.Length() < 0.03f && Physics.PointingUp(Ship.Rotation, 10))
                        landed = true;
                }
            }
            _collision = num_found > 0;
            //Beräkna medelvärdet för positions/rotations ändring samt rotationshastighet
            //om vi har kolliderar
            if (num_found > 0)
            {
                Ship.Position += delta_pos / num_found;
                Ship.Rotation += delta_angle / num_found;
                Ship.TurnRate = delta_angle / num_found;
            }
            return landed;
        }

    }
}
