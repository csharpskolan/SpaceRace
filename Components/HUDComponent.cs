using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceRace.Managers;

namespace SpaceRace
{
    class HUDComponent : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D fuel_full, fuel_empty;
        SpriteFont hudFont;
        Texture2D overlay_gameover, overlay_win, overlay_ready;
        CameraManager cameraManager;

        public HUDComponent(Game game) : base(game)
        {
            
        }

        public SpaceShip Ship { get; set; }

        protected override void LoadContent()
        {
            base.LoadContent();

            cameraManager = ServiceLocator.GetService<CameraManager>();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hudFont = this.Game.Content.Load<SpriteFont>("small_font");

            fuel_full = this.Game.Content.Load<Texture2D>("Fuel_full");
            fuel_empty = this.Game.Content.Load<Texture2D>("Fuel_empty");
            overlay_gameover = this.Game.Content.Load<Texture2D>("splash_gameover");
            overlay_win = this.Game.Content.Load<Texture2D>("splash_win");
            overlay_ready = this.Game.Content.Load<Texture2D>("splash_ready");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            TimeSpan warningTime = TimeSpan.FromSeconds(30);

            switch (Physics.GameState)
            {
                case GameState.READY:
                case GameState.PLAYING:

                    string timeString = $"{LevelManager.LevelTime:mm\\:ss\\.fff}";
                    Color timeColor;
                    if (LevelManager.LevelTime > warningTime ||
                        Physics.Landed || (int)LevelManager.LevelTime.TotalSeconds % 2 == 0)
                        timeColor = Color.Yellow;
                    else
                        timeColor = Color.Red;

                    spriteBatch.DrawString(hudFont, timeString, cameraManager.TransformToScreen(new Vector2(4, 7)), Color.Black, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(hudFont, timeString, cameraManager.TransformToScreen(new Vector2(2, 5)), timeColor, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);

                    spriteBatch.Draw(fuel_full, new Vector2(5, 150) + cameraManager.Offset, null, Color.White, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                    spriteBatch.Draw(fuel_empty, new Vector2(5, 150) + cameraManager.Offset, new Rectangle(0, 0, 14, (int)(130*(100-Ship.Fuel)/100.0f)), Color.White, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);


                    if (Physics.GameState == GameState.READY)
                    {
                        spriteBatch.Draw(overlay_ready, cameraManager.CenterOnScreen(overlay_ready) + new Vector2(2,2)*cameraManager.ScaleFactor, null, Color.Black, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                        spriteBatch.Draw(overlay_ready, cameraManager.CenterOnScreen(overlay_ready), null, Color.White, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);

                        if(gameTime.TotalGameTime.Seconds % 2 == 0)
                        {
                            var size = hudFont.MeasureString("PRESS SPACE");
                            var pos = new Vector2(cameraManager.BaseWidth / 2 - size.X / 2, cameraManager.BaseHeight / 2 + 60);
                            spriteBatch.DrawString(hudFont, "PRESS SPACE", cameraManager.TransformToScreen(pos), Color.Yellow, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                        }
                    }

                    break;
                case GameState.WON:
                    spriteBatch.Draw(overlay_win, cameraManager.CenterOnScreen(overlay_win), null, Color.White, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);

                    if (gameTime.TotalGameTime.Seconds % 2 == 0)
                    {
                        timeString = $"{LevelManager.LevelTime:mm\\:ss\\.fff}";
                        var size = hudFont.MeasureString(timeString);
                        var pos = new Vector2(cameraManager.BaseWidth / 2 - size.X / 2, cameraManager.BaseHeight / 2 + 60);
                        spriteBatch.DrawString(hudFont, timeString, cameraManager.TransformToScreen(pos), Color.Yellow, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);

                        size = hudFont.MeasureString("PRESS SPACE");
                        pos = new Vector2(cameraManager.BaseWidth / 2 - size.X / 2, cameraManager.BaseHeight / 2 + 74);
                        spriteBatch.DrawString(hudFont, "PRESS SPACE", cameraManager.TransformToScreen(pos), Color.Yellow, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                    }
                    //int totalScore = (int)((Ship.Fuel + Ship.Shield + LevelManager.LevelTime.TotalSeconds) * 10);

                    break;
                case GameState.GAMEOVER:
                    spriteBatch.Draw(overlay_gameover, cameraManager.CenterOnScreen(overlay_gameover), null, Color.White, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                    if (gameTime.TotalGameTime.Seconds % 2 == 0)
                    {
                        var size = hudFont.MeasureString("PRESS SPACE");
                        var pos = new Vector2(cameraManager.BaseWidth / 2 - size.X / 2, cameraManager.BaseHeight / 2 + 60);
                        spriteBatch.DrawString(hudFont, "PRESS SPACE", cameraManager.TransformToScreen(pos), Color.Yellow, 0f, Vector2.Zero, cameraManager.ScaleFactor, SpriteEffects.None, 0f);
                    }
                    break;
            }

            spriteBatch.End();
        }
    }
}
