using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceRace.Managers
{
    public class CameraManager
    {
        public int ScaleFactor { get; private set; }
        public Vector2 Offset { get; private set; }
        public int BaseWidth { get; }
        public int BaseHeight { get; }

        private Vector2 cameraPosition;
        public Vector2 CameraPosition 
        { 
            get {  return cameraPosition; }
            set
            {
                cameraPosition = value;
                cameraPosition = Vector2.Clamp(cameraPosition, 
                    CenterPosition, 
                    WorldSize - CenterPosition);
            } 
        }

        public Vector2 WorldSize { get; set; }

        public Vector2 CenterPosition => new Vector2(BaseWidth / 2, BaseHeight / 2);
        public Vector2 DrawOffset => CameraPosition - CenterPosition;

        public Vector2 TransformWorldToScreen(Vector2 pos)
        {
            return (pos - DrawOffset) * ScaleFactor + Offset;
        }

        public Vector2 TransformToScreen(Vector2 pos)
        {
            return pos * ScaleFactor + Offset;
        }

        public Vector2 CenterOnScreen(Texture2D texture)
        {
            var pos = CenterPosition - new Vector2(texture.Width / 2, texture.Height / 2);
            return pos * ScaleFactor + Offset;
        }

        public CameraManager(int baseWidth, int baseHeight)
        {
            BaseWidth = baseWidth;
            BaseHeight = baseHeight;
            Offset = Vector2.Zero;
        }

        public void CalculateScaleFactor(int screenWidth, int screenHeight)
        {
            int scaleWidth = screenWidth / BaseWidth;
            int scaleHeight = screenHeight / BaseHeight;
            ScaleFactor = Math.Min(scaleWidth, scaleHeight);
            Offset = new Vector2((screenWidth - BaseWidth * ScaleFactor) / 2, (screenHeight - BaseHeight * ScaleFactor) / 2);

        }
    }
}
