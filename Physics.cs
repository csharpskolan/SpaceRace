using System;
using Microsoft.Xna.Framework;

namespace SpaceRace
{
    class Physics
    {
        public static Vector2 Gravity = new Vector2(0, 0.01f);
        public static bool Landed { get; set; }
        public static GameState GameState = GameState.LOADING;

        public static bool PointingUp(float rotation, float degrees)
        {
            // Vi kollar om skeppet pekar uppåt med +/- degrees grader
            float min = (float)Math.Cos(0 + degrees * Math.PI / 180.0f);
            return Math.Cos(rotation) >= min;
        }
    }
}
