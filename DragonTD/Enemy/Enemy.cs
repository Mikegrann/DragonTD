using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.Enemy
{
    public enum EnemyType { Trash, Basic, Flying, Fast, Mid, Heavy, Buff };

    abstract class Enemy : DrawableGameComponent
    {
        public AnimatedSprite Texture;
        SpriteBatch spriteBatch;

        public Color Color { get { return Texture.Color; } set { Texture.Color = value; } }
        public float Rotation;

        public bool Dead;
        
        public EnemyStats Stats;
        public Vector2 ScreenPosition { get;  set; }

        private int MaxHealth, MaxShield;

        private int PoisonDamage;
        private float PoisonTimer;


        public float FreezeTime;

        public float SpeedDebuff = 1f;
        public float SpeedDebuffTimer = 0f;

        public Enemy(Game game, EnemyStats Stats, Vector2 ScreenPosition, AnimatedSprite texture) : base(game)
        { 
            this.Stats = Stats;
            MaxHealth = Stats.Health;
            MaxShield = Stats.Shields;

            this.ScreenPosition = ScreenPosition;

            this.Texture = texture;
            spriteBatch = game.Services.GetService<SpriteBatch>();

            Dead = false;

            PoisonTimer = PoisonDamage = 0;
        }

        // TODO: Set EnemyStats (maybe implement reads from config files?)
        public static EnemyStats GetEnemyStats(EnemyType type)
        {
            switch (type)
            {
                default:
                case EnemyType.Basic:
                    return new EnemyStats(20, 0, 100, 5, 20);
                case EnemyType.Flying:
                    return new EnemyStats(10, 0, 50, 15, 25);
            }
        }


        static String TexDir = "Textures/Mobs/";
        static float AnimTime = 0.1f;
        public static AnimatedSprite GetEnemyTexture(Game game, EnemyType type)
        {
            switch (type)
            {
                default:
                case EnemyType.Trash:
                    return new AnimatedSprite(new Texture2D[] 
                    {
                        game.Content.Load<Texture2D>(TexDir + "Peasant/Peasant1"),
                        game.Content.Load<Texture2D>(TexDir + "Peasant/Peasant2"),
                        game.Content.Load<Texture2D>(TexDir + "Peasant/Peasant3"),
                        game.Content.Load<Texture2D>(TexDir + "Peasant/Peasant2")
                    }, Color.White, AnimTime);
                case EnemyType.Basic:
                    return new AnimatedSprite(new Texture2D[] 
                    {
                        game.Content.Load<Texture2D>(TexDir + "Spearman/Spearman1"),
                        game.Content.Load<Texture2D>(TexDir + "Spearman/Spearman2"),
                        game.Content.Load<Texture2D>(TexDir + "Spearman/Spearman3"),
                        game.Content.Load<Texture2D>(TexDir + "Spearman/Spearman2")
                    }, Color.White, AnimTime);
                case EnemyType.Flying:
                    return new AnimatedSprite(new Texture2D[]
                    {
                        game.Content.Load<Texture2D>(TexDir + "Griffin/Griffin1"),
                        game.Content.Load<Texture2D>(TexDir + "Griffin/Griffin2"),
                        game.Content.Load<Texture2D>(TexDir + "Griffin/Griffin3and4and7"),
                        game.Content.Load<Texture2D>(TexDir + "Griffin/Griffin3and4and7"),
                        game.Content.Load<Texture2D>(TexDir + "Griffin/Griffin5"),
                        game.Content.Load<Texture2D>(TexDir + "Griffin/Griffin6"),
                        game.Content.Load<Texture2D>(TexDir + "Griffin/Griffin3and4and7")
                    }, Color.White, AnimTime);
                case EnemyType.Fast:
                    return new AnimatedSprite(new Texture2D[]
                    {
                        game.Content.Load<Texture2D>(TexDir + "Mounted/Mounted1"),
                        game.Content.Load<Texture2D>(TexDir + "Mounted/Mounted2"),
                        game.Content.Load<Texture2D>(TexDir + "Mounted/Mounted3"),
                        game.Content.Load<Texture2D>(TexDir + "Mounted/Mounted4"),
                        game.Content.Load<Texture2D>(TexDir + "Mounted/Mounted5"),
                        game.Content.Load<Texture2D>(TexDir + "Mounted/Mounted6"),
                    }, Color.White, AnimTime);
                case EnemyType.Mid:
                    return new AnimatedSprite(new Texture2D[]
                    {
                        game.Content.Load<Texture2D>(TexDir + "Knight/Knight1"),
                        game.Content.Load<Texture2D>(TexDir + "Knight/Knight2"),
                        game.Content.Load<Texture2D>(TexDir + "Knight/Knight3"),
                        game.Content.Load<Texture2D>(TexDir + "Knight/Knight2")
                    }, Color.White, AnimTime);
                case EnemyType.Heavy:
                    return new AnimatedSprite(new Texture2D[]
                    {
                        game.Content.Load<Texture2D>(TexDir + "Elephant/Elephant1"),
                        game.Content.Load<Texture2D>(TexDir + "Elephant/Elephant2"),
                        game.Content.Load<Texture2D>(TexDir + "Elephant/Elephant3"),
                        game.Content.Load<Texture2D>(TexDir + "Elephant/Elephant4"),
                        game.Content.Load<Texture2D>(TexDir + "Elephant/Elephant5"),
                        game.Content.Load<Texture2D>(TexDir + "Elephant/Elephant6"),
                        game.Content.Load<Texture2D>(TexDir + "Elephant/Elephant7"),
                        game.Content.Load<Texture2D>(TexDir + "Elephant/Elephant8"),
                    }, Color.White, AnimTime);
                case EnemyType.Buff:
                    return new AnimatedSprite(new Texture2D[]
                    {
                        game.Content.Load<Texture2D>(TexDir + "Wizard/Wizard1"),
                        game.Content.Load<Texture2D>(TexDir + "Wizard/Wizard2"),
                        game.Content.Load<Texture2D>(TexDir + "Wizard/Wizard3"),
                        game.Content.Load<Texture2D>(TexDir + "Wizard/Wizard2")
                    }, Color.White, AnimTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Texture != null)
                //spriteBatch.Draw(Texture, ScreenPosition, null, Color, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
                Texture.Draw(gameTime, spriteBatch, ScreenPosition, new Vector2(Texture.Width / 2, Texture.Height / 2), Rotation);

            // TODO: Redo logic for new health bars
            int healthRatio = Stats.Health * 50 / MaxHealth;
            spriteBatch.Draw(Game.Content.Load<Texture2D>("Textures/UI/HealthBars"), 
                new Rectangle((int)ScreenPosition.X, (int)ScreenPosition.Y - 40, 40, 8), 
                new Rectangle(0, 5 * healthRatio, 52, 5), 
                Color.White, 0f, new Vector2(25, 0), SpriteEffects.None, 0f);

            if (Stats.Shields > 0) {
                int shieldRatio = Stats.Shields * 50 / MaxShield;
                spriteBatch.Draw(Game.Content.Load<Texture2D>("Textures/UI/HealthBars"),
                    new Rectangle((int)ScreenPosition.X, (int)ScreenPosition.Y - 45, 40, 8),
                    new Rectangle(0, 5 * shieldRatio, 52, 5),
                    Color.White, 0f, new Vector2(25, 0), SpriteEffects.None, 0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Apply Poison DoT
            if (PoisonTimer > 0.0)
            {
                Stats.Health -= (int)(PoisonDamage * gameTime.ElapsedGameTime.TotalSeconds);
                PoisonTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                Color = Color.Green;
            }
            else if (SpeedDebuffTimer > 0)
            {
                SpeedDebuffTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                Color = Color.Cyan;
            }
            else
            {
                Color = Color.White;
                SpeedDebuff = 1f;
            }

            if (Stats.Health <= 0) { Dead = true; }
        }

        public abstract float GetDistanceFromGoal();

        public void ApplyPoison(int dps, float duration)
        {
            PoisonDamage = dps;
            PoisonTimer = duration;
            Color = Color.Green;
        }
    }
}
