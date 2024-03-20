using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceRace.Managers;
using SpaceRace.Messages;

namespace SpaceRace
{
    public class SpaceRaceGame : Game
    {
        GraphicsDeviceManager graphics;

        SpaceShipManager ShipManager;
        LevelManager LevelManager;
        MenuManager MenuManager;
        CameraManager cameraManager;

        int startWidth = 960;
        int startHeight = 540;
        public SpaceRaceGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = startWidth;
            graphics.PreferredBackBufferHeight = startHeight;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            cameraManager = new CameraManager(480, 270);
            cameraManager.CalculateScaleFactor(startWidth, startHeight);
            ServiceLocator.AddService(cameraManager);

            Components.Add(new KeyBoardComponent(this));
            Components.Add(new SoundManager(this));

            LevelManager = new LevelManager(this);
            Components.Add(LevelManager);

            ShipManager = new SpaceShipManager(this);
            Components.Add(ShipManager);
            ServiceLocator.AddService(ShipManager);

            MenuManager = new MenuManager(this);
            Components.Add(MenuManager);

            base.Initialize();

            RegisterMessages();
            Messenger.Instance.Send<GameState>(GameState.MAINMENU);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            if (KeyBoardComponent.KeyPressed(Keys.Escape))
                Messenger.Instance.Send(Physics.GameState | GameState.MAINMENU);
            
            if (KeyBoardComponent.KeyPressed(Keys.F))
            {
                this.graphics.IsFullScreen = !this.graphics.IsFullScreen;
                this.graphics.ApplyChanges();
            }

            switch (Physics.GameState)
            {
                case GameState.LOADING:
                    ShipManager.ResetShip(LevelManager.StartPosition);
                    Messenger.Instance.Send(GameState.READY);
                    break;
                case GameState.PLAYING:
                    //Kolla kollision
                    Physics.Landed = ShipManager.CheckCollision(LevelManager.LevelMasked, LevelManager.Level);
                    if (Physics.Landed)
                    {
                        //SoundManager.PlayCue("win");
                        Messenger.Instance.Send(GameState.WON);
                    }
                    if (ShipManager.Ship.Dead)
                        Messenger.Instance.Send(GameState.GAMEOVER);
                    break;
                case GameState.WON:
                    if (KeyBoardComponent.KeyPressed(Keys.Space))
                    {
                        LevelManager.LoadNextLevel();
                        ShipManager.ResetShip(LevelManager.StartPosition);
                        Messenger.Instance.Send(GameState.READY);
                    }
                    break;
                case GameState.GAMEOVER:
                    if (KeyBoardComponent.KeyPressed(Keys.Space))
                    {
                        LevelManager.ResetLevel();
                        ShipManager.ResetShip(LevelManager.StartPosition);
                        Messenger.Instance.Send(GameState.READY);
                    }
                    break;
                case GameState.READY:
                    if (KeyBoardComponent.KeyPressed(Keys.Space))
                    {
                        LevelManager.ResetLevel();
                        ShipManager.ResetShip(LevelManager.StartPosition);
                        Messenger.Instance.Send(GameState.PLAYING);
                    }
                    break;
            }

            if(KeyBoardComponent.KeyPressed(Keys.D1))
            {
                graphics.PreferredBackBufferWidth = 480;
                graphics.PreferredBackBufferHeight = 270;
                graphics.ApplyChanges();
            }
            if (KeyBoardComponent.KeyPressed(Keys.D2))
            {
                graphics.PreferredBackBufferWidth = 960;
                graphics.PreferredBackBufferHeight = 540;
                graphics.ApplyChanges();
            }
            if (KeyBoardComponent.KeyPressed(Keys.D3))
            {
                graphics.PreferredBackBufferWidth = 1440;
                graphics.PreferredBackBufferHeight = 810;
                graphics.ApplyChanges();
            }
            if (KeyBoardComponent.KeyPressed(Keys.D4))
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
                graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            cameraManager.CalculateScaleFactor(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            base.Draw(gameTime);
        }

        void RegisterMessages()
        {
            Messenger.Instance.Register<GameState>(this, OnGameStateChanged);
        }

        private void OnGameStateChanged(GameState state)
        {
            if (Physics.GameState == state)
                return;

            Physics.GameState = state;

            foreach (var component in Components)
            {
                ManagerBase manager = component as ManagerBase;
                if (manager == null)
                    continue;

                manager.Enabled = (manager.SupportedStates & state) > 0;
                manager.Visible = manager.Enabled;
            }
        }
    }
}
