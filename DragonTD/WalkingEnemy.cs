using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class WalkingEnemy : Enemy {
        private List<HexEntity> Path;
        private int PathIndex;
        private float PathProgress;

        public WalkingEnemy(Game game, EnemyStats Stats, Vector2 ScreenPosition, Texture2D texture, List<HexEntity> Path) : base(game, Stats, ScreenPosition, texture) 
        {
            this.Path = Path;
            PathProgress = PathIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (PathIndex < Path.Count - 1)
            {
                if (FreezeTime <= 0)
                {
                    ScreenPosition = Path[PathIndex].ScreenPosition + (Path[PathIndex + 1].ScreenPosition - Path[PathIndex].ScreenPosition) * PathProgress;

                    Rotation = (float)Math.PI / 2 +
                        (float)Math.Atan2(Path[PathIndex + 1].ScreenPosition.Y - Path[PathIndex].ScreenPosition.Y,
                        Path[PathIndex + 1].ScreenPosition.X - Path[PathIndex].ScreenPosition.X);

                    PathProgress += (float)gameTime.ElapsedGameTime.TotalSeconds * Stats.Speed / (Path[PathIndex + 1].ScreenPosition - Path[PathIndex].ScreenPosition).Length();
                    if (PathProgress > 1.0f)
                    {
                        PathProgress -= 1.0f;
                        PathIndex++;
                    }

                    // Reached end of path
                    if (PathIndex == Path.Count)
                    {
                        // TODO: Enemy reaches end - decrease treasure resource
                    }
                }
                else
                {
                    FreezeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            base.Update(gameTime);
        }

        public override float GetDistanceFromGoal()
        {
            return Path.Count - (PathIndex + PathProgress);
        }
    }
}
