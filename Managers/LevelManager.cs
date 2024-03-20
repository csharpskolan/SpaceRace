using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceRace.Messages;

namespace SpaceRace.Managers
{

    public class LevelManager : ManagerBase
    {
        SpriteBatch spriteBatch;
        
        private CameraManager cameraManager;
        private SpaceShipManager spaceShipManager;

        float backgroundIntensity = 0;

        public LevelManager(Game game)
            : base(game)
        {
        }

        #region Properties

        public static TimeSpan LevelTime;

        public Texture2D Level { get; set; }
        public Texture2D LevelMasked { get; set; }

        int levelIndex = 0;

        public Vector2 StartPosition { get { return Utility.FindPixel(Level, 0xFF0000FF); }}

        #endregion

        #region Overrides

        public override GameState SupportedStates
        {
            get { return GameState.GAMEOVER | GameState.PLAYING | GameState.READY | GameState.WON; }
        }

        public override void Initialize()
        {
            cameraManager = ServiceLocator.GetService<CameraManager>();
            spaceShipManager = ServiceLocator.GetService<SpaceShipManager>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadNextLevel();

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            byte i = (byte)((0.5f + Math.Abs(Math.Cos(backgroundIntensity) / 2.0f)) * 255);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(Level, cameraManager.TransformWorldToScreen(Vector2.Zero), null, new Color(i,i,i,i), 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
            spriteBatch.Draw(LevelMasked, cameraManager.TransformWorldToScreen(Vector2.Zero), null, Color.White, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            backgroundIntensity += 0.03f;

            if (Physics.GameState != GameState.PLAYING)
                return;

            LevelTime -= gameTime.ElapsedGameTime;
            if (LevelTime.TotalSeconds <= 0)
            {
                Messenger.Instance.Send(GameState.GAMEOVER);
                return;
            }

            base.Update(gameTime);
        }

        #endregion

        #region Public Methods

        public void LoadNextLevel()
        {
            levelIndex++;
            LevelTime = TimeSpan.FromMinutes(2);

            string levelFile = "Level" + levelIndex.ToString("00");
            string levelPath = "Content/" + levelFile + ".xnb";
            if (File.Exists(levelPath))
            {
                Level = this.Game.Content.Load<Texture2D>(levelFile);
                LevelMasked = this.Game.Content.Load<Texture2D>($"{levelFile}_masked");
            }
            else
            {
                levelIndex = 1;
                Level = this.Game.Content.Load<Texture2D>("Level01");
                LevelMasked = this.Game.Content.Load<Texture2D>("Level01_masked");
            }

            cameraManager.WorldSize = new Vector2(Level.Width, Level.Height);    
        }

        public void ResetLevel()
        {
            LevelTime = TimeSpan.FromMinutes(2);
        }

        #endregion
    }
}
