using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Enemy : DrawableGameComponent
    {
        private List<HexEntity> Path;
        private int PathIndex;
        private float PathProgress;

        public EnemyStats Stats;
        public Vector2 ScreenPosition { get; private set; }

        public Enemy(Game game, List<HexEntity> Path, EnemyStats Stats, Vector2 ScreenPosition) : base(game)
        {
            this.Path = Path;
            this.Stats = Stats;
            this.ScreenPosition = ScreenPosition;

            PathProgress = PathIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            ScreenPosition = (Path[PathIndex + 1].ScreenPosition - Path[PathIndex].ScreenPosition) * PathProgress;

            PathProgress += Stats.speed;
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
    }
}
