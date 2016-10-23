using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class FlyingEnemy : Enemy
    {
        HexEntity Target;
        Vector2 Velocity;

        public FlyingEnemy(Game game, EnemyStats Stats, Vector2 ScreenPosition, Texture2D texture, HexEntity target) : base(game, Stats, ScreenPosition, texture)
        {
            Target = target;

            Vector2 direction = target.ScreenPosition - ScreenPosition;
            direction.Normalize();
            Velocity = direction * Stats.Speed;
            Rotation = (float)Math.PI / 2 + (float)Math.Atan2(Velocity.Y, Velocity.X);
        }

        public override void Update(GameTime gameTime)
        {
            if (FreezeTime <= 0)
            {
                // Reached end of path
                if (GetDistanceFromGoal() < (float)gameTime.ElapsedGameTime.TotalSeconds * Velocity.Length())
                {
                    ScreenPosition = Target.ScreenPosition;
                    Dead = true;
                }
                else
                {
                    ScreenPosition += (float)gameTime.ElapsedGameTime.TotalSeconds * Velocity * SpeedDebuff;
                }
            }
            else
            {
                FreezeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        public override float GetDistanceFromGoal()
        {
            return Util.Distance(Target.ScreenPosition, ScreenPosition);
        }
    }
}
