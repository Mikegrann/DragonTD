using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DragonTD
{
    class AnimatedSprite : Sprite
    {
        TimeSpan ElapsedTime;
        TimeSpan TimeBetweenFrames;
        int CurrentFrame = 0;
        int CellCount;
        Rectangle[] FrameBoundaries;
        bool Animating = true;

        public AnimatedSprite(Texture2D texture, Color color, Vector2 rowsAndColumns, int cellCount) : base(texture, color)
        {
            CellCount = cellCount;
            Vector2 frameSize = new Vector2(Texture.Width, Texture.Height) / rowsAndColumns;
            FrameBoundaries = new Rectangle[CellCount];
            int i = 0;
            for(int y = 0; y < rowsAndColumns.Y; y++)
            {
                for(int x = 0; x < rowsAndColumns.X; x++)
                {
                    if (i >= CellCount)
                        break;

                    FrameBoundaries[i++] = new Rectangle((int)(x * frameSize.X), (int)(y * frameSize.Y), (int)frameSize.X, (int)frameSize.Y);
                }
            }
        }

        public void StartAnimation()
        {
            Animating = true;
        }
        public void PauseAnimation()
        {
            Animating = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 origin, float rotation = 0f, float scale = 0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f )
        {
            if (Animating)
                UpdateFrame(gameTime);

            spriteBatch.Draw(Texture, position, FrameBoundaries[CurrentFrame], Color, rotation, origin, scale, effects, layerDepth);
        }

        private void UpdateFrame(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime;

            if (ElapsedTime > TimeBetweenFrames)
            {
                ElapsedTime -= TimeBetweenFrames;

                CurrentFrame++;
                if (CurrentFrame >= CellCount)
                    CurrentFrame = 0;
            }
        }
    }
}
