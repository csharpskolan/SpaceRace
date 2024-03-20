using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceRace
{
    class Utility
    {
        public static Vector2 FindPixel(Texture2D texture, uint color)
        {
            if (texture == null)
                return Vector2.Zero;

            //Kopiera pixeldata som uint
            uint[] bits = new uint[texture.Width * texture.Height];
            texture.GetData<uint>(bits);

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    //Har vi hittat det vi söker?
                    if (bits[x + y * texture.Width] == color)
                        return new Vector2(x, y);
                }
            }
            //Om inget hittades
            return Vector2.Zero;
        }

        public static List<Point> FindHull(Texture2D gfx)
        {
            var _hull = new List<Point>();
            //Läs in pixeldata från spriten
            uint[] bits = new uint[gfx.Width * gfx.Height];
            gfx.GetData<uint>(bits);

            for (int x = 0; x < gfx.Width; x++)
            {
                for (int y = 0; y < gfx.Height; y++)
                {
                    //Skippa genomskinliga pixlar
                    if ((bits[x + y * gfx.Width] & 0xFF000000) >> 24 <= 20)
                        continue;

                    //Pixlar på kanten av texturen?
                    if (x == 0 || y == 0 || x == gfx.Width - 1 || y == gfx.Height - 1)
                    {
                        _hull.Add(new Point(x, y));
                        continue;
                    }

                    //Kant i spriten?
                    if (((bits[x + 1 + y * gfx.Width] & 0xFF000000) >> 24 <= 20) ||
                        ((bits[x - 1 + y * gfx.Width] & 0xFF000000) >> 24 <= 20) ||
                        ((bits[x + (y + 1) * gfx.Width] & 0xFF000000) >> 24 <= 20) ||
                        ((bits[x + (y - 1) * gfx.Width] & 0xFF000000) >> 24 <= 20))
                        _hull.Add(new Point(x, y));
                }
            }
            return _hull;
        }
    }
}
