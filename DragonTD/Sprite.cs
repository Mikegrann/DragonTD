using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DragonTD
{
    class Sprite
    {
        public Texture2D Texture;
        public Color Color;

        public Sprite(Texture2D texture, Color color)
        {
            Texture = texture;
            Color = color;
        }
        //making this virtual allows the subclass's overridden draw method to be called even if it gets referenced as the superclass.
        //https://msdn.microsoft.com/en-us/library/aa645767(v=vs.71).aspx
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 origin, float rotation = 0f, float scale = 0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            spriteBatch.Draw(Texture, position, null, Color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
