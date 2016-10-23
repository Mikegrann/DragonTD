using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DragonTD
{
    class Projectile : DrawableGameComponent
    {
        public Tower.TowerStats Stats;
        public int MultiHit;

        // Velocity in pixels per second
        Vector2 Velocity;
        public Vector2 Position;
        Vector2 StartPosition;
        Vector2 Target;

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
            MultiHit = Stats.MultiHit;

            StartPosition = Position = position;
            Target = target;

            Vector2 direction = target - position;
            direction.Normalize();
            Velocity = direction * stats.ProjectileSpeed;
            spriteBatch = game.Services.GetService<SpriteBatch>();
        }

        public override void Update(GameTime gameTime)
        {
            Position += (float)gameTime.ElapsedGameTime.TotalSeconds * Velocity;

            // Signal death
            if (Util.Distance(StartPosition, Position) > Stats.Range)
            {
                MultiHit = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, GetRotation(Velocity), new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0);
        }

        public static float GetRotation(Vector2 velocity)
        {
            return (float)Math.PI / 2f + (float)System.Math.Atan2(velocity.Y, velocity.X);
        }

        public void ApplyEffect(Enemy Other)
        {
            // Apply Basic
            if (Other.Stats.Shields > 0)
            {
                Other.Stats.Shields -= Stats.BasicDamage;
            }
            else
            {
                Other.Stats.Health -= Stats.BasicDamage;
            }

            // Piercing Ignores Shields
            Other.Stats.Health -= Stats.PiercingDamage;

            // Set up a Poison DoT
            Other.ApplyPoison(Stats.PoisonDamage, Stats.PoisonDuration);
        }
    }

    
}