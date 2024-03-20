using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace SpaceRace
{
    public class SoundManager : DrawableGameComponent
    {
        public SoundManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.05f;
            MediaPlayer.Play(this.Game.Content.Load<Song>("Sounds\\music"));
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
