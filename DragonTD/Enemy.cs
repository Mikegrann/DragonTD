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

        public Texture2D Texture;
        SpriteBatch spriteBatch;

        public Color Color = Color.White;
        public float Rotation;

        public EnemyStats Stats;
        public Vector2 ScreenPosition { get; private set; }

        private int PoisonDamage;
        private float PoisonTimer;

        public enum EnemyType { Trash, Basic, Flying, Fast, Mid, Heavy, Buff };

        public float FreezeTime;

        public Enemy(Game game, List<HexEntity> Path, EnemyStats Stats, Vector2 ScreenPosition, Texture2D texture) : base(game)
        {
            this.Path = Path;
            this.Stats = Stats;
            this.ScreenPosition = ScreenPosition;

            this.Texture = texture;
            spriteBatch = game.Services.GetService<SpriteBatch>();

            PathProgress = PathIndex = 0;
            PoisonTimer = PoisonDamage = 0;
        }

        // TODO: Set EnemyStats (maybe implement reads from config files?)
        public static EnemyStats GetEnemyStats(EnemyType type)
        {
            switch (type)
            {
                default:
                case EnemyType.Basic:
                    return new EnemyStats(20, 0, 3);
                    break;
            }
        }

        // TODO: Set EnemyTextures
        public static Texture2D GetEnemyTexture(Game game, EnemyType type)
        {
            switch (type)
            {
                default:
                case EnemyType.Basic:
                    return game.Content.Load<Texture2D>("textures/enemies/basic");
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, ScreenPosition, null, Color, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
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

                    PathProgress += Stats.Speed * 0.01f;
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

            // Apply Poison DoT
            if (PoisonTimer > 0.0)
            {
                Stats.Health -= (int)(PoisonDamage * gameTime.ElapsedGameTime.TotalSeconds);
                PoisonTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public float GetProgress()
        {
            return PathIndex + PathProgress;
        }

        public void ApplyPoison(int dps, float duration)
        {
            PoisonDamage = dps;
            PoisonTimer = duration;
        }
    }
}
