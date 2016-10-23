﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.Enemy
{
    public enum EnemyType { Trash, Basic, Flying, Fast, Mid, Heavy, Buff };

    abstract class Enemy : DrawableGameComponent
    {
        public Texture2D Texture;
        SpriteBatch spriteBatch;

        public Color Color = Color.White;
        public float Rotation;

        public bool Dead;
        
        public EnemyStats Stats;
        public Vector2 ScreenPosition { get;  set; }

        private int PoisonDamage;
        private float PoisonTimer;


        public float FreezeTime;

        public float SpeedDebuff = 1f;
        public float SpeedDebuffTimer = 0f;

        public Enemy(Game game, EnemyStats Stats, Vector2 ScreenPosition, Texture2D texture) : base(game)
        { 
            this.Stats = Stats;
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

        // TODO: Set EnemyTextures
        public static Texture2D GetEnemyTexture(Game game, EnemyType type)
        {
            switch (type)
            {
                default:
                case EnemyType.Basic:
                    return game.Content.Load<Texture2D>("textures/enemies/basic");
                case EnemyType.Flying:
                    return game.Content.Load<Texture2D>("textures/enemies/flying");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, ScreenPosition, null, Color, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            // Apply Poison DoT
            if (PoisonTimer > 0.0)
            {
                Stats.Health -= (int)(PoisonDamage * gameTime.ElapsedGameTime.TotalSeconds);
                PoisonTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(SpeedDebuffTimer > 0)
            {
                SpeedDebuffTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                Color = Color.Cyan;
            }
            else
            {
                Color = Color.White;
                SpeedDebuff = 1f;
            }
        }

        public abstract float GetDistanceFromGoal();

        public void ApplyPoison(int dps, float duration)
        {
            PoisonDamage = dps;
            PoisonTimer = duration;
        }
    }
}