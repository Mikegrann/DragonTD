using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DragonTD
{
    class Projectile : DrawableGameComponent
    {
        Tower.TowerStats Stats;
        // Velocity in pixels per second
        Vector2 Velocity;
        Vector2 Position, Target;
        Texture2D Texture;
        Color Color;

        SpriteBatch spriteBatch;

        /// <summary>
        /// Projectile fired my projectile towers
        /// </summary>
        /// <param name="game">Game object</param>
        /// <param name="texture">Texture of projectile</param>
        /// <param name="color">Color of projectile. If null - draws white</param>
        /// <param name="position">ScreenPosition</param>
        /// <param name="velocity">Velocity of projectile in pixels per second.</param>
        /// <param name="stats">stats of the tower. includes damage values.</param>
        public Projectile(Game game, Texture2D texture, Nullable<Color> color, Tower.TowerStats stats, Vector2 position, Vector2 target) : base(game)
        {
            if (color.HasValue)
                Color = color.Value;
            else
                Color = Color.White;
            Texture = texture;
            Stats = stats;
            Position = position;
            Target = target;
            Velocity = (target - position) * stats.ProjectileSpeed;
            spriteBatch = game.Services.GetService<SpriteBatch>();
        }

        public override void Update(GameTime gameTime)
        {
            Position += (float)gameTime.ElapsedGameTime.TotalSeconds * Velocity;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, GetRotation(Velocity), new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0);
        }

        public static float GetRotation(Vector2 velocity)
        {
            return (float)Math.PI / 2f + (float)System.Math.Atan2(velocity.Y, velocity.X);
        }

        
    }

    
}