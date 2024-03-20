using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpaceRace.Managers;
using SpaceRace.Messages;
using static System.Net.Mime.MediaTypeNames;


namespace SpaceRace
{
    public class MenuManager : ManagerBase
    {
        private SpriteFont Font;
        private SpriteBatch spriteBatch;
        private List<MenuChoice> Choices;
        private Texture2D titleScreen;
        private Texture2D logo;
        private CameraManager cameraManager;
        private MenuState menuState;
        private SpriteFont hudFont;

        public MenuManager(Game game)
            : base(game)
        {
        }

        public override GameState SupportedStates
        {
            get { return GameState.MAINMENU; }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            hudFont = this.Game.Content.Load<SpriteFont>("small_font");
            Font = Game.Content.Load<SpriteFont>("menufont");
            titleScreen = Game.Content.Load<Texture2D>("titlescreen");
            logo = Game.Content.Load<Texture2D>("logo");

            base.LoadContent();
        }

        public override void Initialize()
        {
            menuState = MenuState.INTRO;
            cameraManager = ServiceLocator.GetService<CameraManager>();
            Choices = new List<MenuChoice>();
            Choices.Add(new MenuChoice() { Title = "START", Selected = true, PressedAction = MenuStartPressed});
            Choices.Add(new MenuChoice() { Title = "SELECT LEVEL" });
            Choices.Add(new MenuChoice() { Title = "OPTIONS" });
            Choices.Add(new MenuChoice() { Title = "QUIT", PressedAction = MenuQuitPressed });

            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            if(menuState == MenuState.INTRO)
            {
                if(KeyBoardComponent.KeyPressed(Keys.Space))
                    menuState = MenuState.MAINMENU;
            }
            if (menuState.HasFlag(MenuState.MAINMENU))
            {
                if (KeyBoardComponent.KeyPressed(Keys.Escape))
                    Messenger.Instance.Send(GameState.MAINMENU);

                if (KeyBoardComponent.KeyPressed(Keys.Up))
                    PreviousMenuChoice();
                if (KeyBoardComponent.KeyPressed(Keys.Down))
                    NextMenuChoice();
                if (KeyBoardComponent.KeyPressed(Keys.Enter))
                    SelectMenuChoice();
            }
            base.Update(gameTime);
        }

        private void MenuStartPressed()
        {
            Messenger.Instance.Send<GameState>(GameState.LOADING);
        }

        private void MenuQuitPressed()
        {
            this.Game.Exit(); 
        }

        private void SelectMenuChoice()
        {
            var selected = Choices.FirstOrDefault(c => c.Selected);

            if (selected == null || selected.PressedAction == null)
                return;

            selected.PressedAction.Invoke();
        }

        private void NextMenuChoice()
        {
            var selected = Choices.FirstOrDefault(c => c.Selected);

            if (selected == null)
            {
                Choices[0].Selected = true;
                return;
            }

            int index = Choices.IndexOf(selected) + 1;
            selected.Selected = false;
            Choices[(index >= Choices.Count) ? 0 : index].Selected = true;
        }

        private void PreviousMenuChoice()
        {
            var selected = Choices.FirstOrDefault(c => c.Selected);

            if (selected == null)
            {
                Choices[0].Selected = true;
                return;
            }

            int index = Choices.IndexOf(selected) - 1;
            selected.Selected = false;
            Choices[(index < 0) ? Choices.Count - 1 : index].Selected = true;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(titleScreen, cameraManager.Offset, null, Color.White, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);

            if (menuState == MenuState.INTRO)
            {
                spriteBatch.Draw(logo, cameraManager.CenterOnScreen(logo), null, Color.White, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                if (gameTime.TotalGameTime.Seconds % 2 == 0)
                {
                    var size = hudFont.MeasureString("PRESS SPACE");
                    var pos = new Vector2(cameraManager.BaseWidth / 2 - size.X / 2, cameraManager.BaseHeight / 2 + 60);
                    spriteBatch.DrawString(hudFont, "PRESS SPACE", cameraManager.TransformToScreen(pos), Color.Yellow, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                }
            }
            if (menuState.HasFlag(MenuState.MAINMENU))
            {
                float startY = 60;

                foreach (var choice in Choices)
                {
                    var size = Font.MeasureString(choice.Title);
                    var x = cameraManager.BaseWidth / 2 - size.X / 2;
                    spriteBatch.DrawString(Font, choice.Title, cameraManager.TransformToScreen(new Vector2(x, startY)), choice.Selected ? Color.White : new Color(127, 127, 127, 255), 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                    startY += 35;
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
