using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceRace
{
    public class ManagerBase : DrawableGameComponent
    {
        public ManagerBase(Game game) : base(game)
        {        
        }

        #region Properties

        public DrawableGameComponent[] Components { get; set; }

        public virtual GameState SupportedStates { get { return GameState.ALL; }}

        #endregion

        #region Overrides

        public override void Initialize()
        {
            if (Components != null)
            {
                foreach (var component in Components)
                    this.Game.Components.Add(component);
            }

            base.Initialize();
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);

            if (Components == null)
                return;

            foreach (var component in Components)
                component.Enabled = this.Enabled;
        }

        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            base.OnVisibleChanged(sender, args);

            if (Components == null)
                return;

            foreach (var component in Components)
                component.Visible = this.Visible;
        }

        #endregion
    }
}
