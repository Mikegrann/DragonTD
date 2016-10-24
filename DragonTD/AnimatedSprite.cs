using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DragonTD
{
    class AnimatedSprite
    {
        TimeSpan ElapsedTime;
        TimeSpan TimeBetweenFrames;
        public int CurrentFrame { get; private set; }
        bool Animating = true;

        Texture2D[] Textures;
        public Color Color;

        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>
        /// Animated sprite.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="color"></param>
        /// <param name="timeBetweenFrames">Time between frames in seconds</param>
        public AnimatedSprite(Texture2D[] textures, Color color, float timeBetweenFrames)
        {
            CurrentFrame = 0;
            TimeBetweenFrames = new TimeSpan(0, 0, 0, 0, (int)(timeBetweenFrames * 1000));
            Color = color;
            Textures = textures;
            Width = Textures[0].Width;
            Height = Textures[0].Height;
        }

        public void StartAnimation()
        {
            Animating = true;
        }
        public void PauseAnimation()
        {
            Animating = false;
        }
        public void NextFrame()
        {
            CurrentFrame++;
            if (CurrentFrame >= Textures.Length)
                CurrentFrame = 0;
        }
        public void PreviousFrame()
        {
            CurrentFrame--;
            if (CurrentFrame < 0)
                CurrentFrame = Textures.Length - 1;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 origin, float rotation = 0f, float scale = 1f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f )
        {
            if (Animating)
                UpdateFrame(gameTime);

            spriteBatch.Draw(Textures[CurrentFrame], position, null, Color, rotation, origin, scale, effects, layerDepth);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Vector2 origin, float rotation = 0f, float scale = 1f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            if (Animating)
                UpdateFrame(gameTime);

            spriteBatch.Draw(Textures[CurrentFrame], position, null, Color, rotation, origin, scale, effects, layerDepth);
            spriteBatch.DrawString(font, position.ToString(), position, Color.Black);
            spriteBatch.DrawString(font, Textures.Length.ToString(), position + new Vector2(0, 16), Color.Black);
            spriteBatch.DrawString(font, Textures[0].Bounds.ToString(), position + new Vector2(0, 32), Color.Black);
        }

        private void UpdateFrame(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime;

            while(ElapsedTime > TimeBetweenFrames)
            {
                ElapsedTime -= TimeBetweenFrames;
                NextFrame();
            }
        }
    }
}
