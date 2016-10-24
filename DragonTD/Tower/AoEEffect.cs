using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DragonTD.Tower
{
    class AoEEffect : DrawableGameComponent
    {
        public enum EffectType { Freeze, Lightning, Explosion };

        AnimatedSprite Texture;
        Vector2 ScreenPosition;

        public bool Done;

        public AoEEffect(Game game, EffectType type, Vector2 location) : base(game)
        {
            Done = false;
            ScreenPosition = location;
            Texture = GetEffectTexture(Game, type);
            Texture.NextFrame();
        }

        static string TTexDir = "Textures/AOE/";
        static float TAnimTime = 0.15f;
        public static AnimatedSprite GetEffectTexture(Game game, EffectType type)
        {
            switch (type)
            {
                default:
                case EffectType.Freeze:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Freeze/Snowflake1"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Freeze/Snowflake1"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Freeze/Snowflake2"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Freeze/Snowflake3"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Freeze/Snowflake4"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Freeze/Snowflake5"), }, Color.White, TAnimTime);
                case EffectType.Lightning:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Lightning/Lightning1"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Lightning/Lightning1"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Lightning/Lightning2"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Lightning/Lightning3"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Lightning/Lightning4"),
                                                                game.Content.Load<Texture2D>(TTexDir + "Lightning/Lightning5"), }, Color.White, TAnimTime);
                    //case EffectType.Explosion:
                    //    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Red Dragon/Redfinal12"), game.Content.Load<Texture2D>(TTexDir + "Red Dragon/Redfinal3478"), game.Content.Load<Texture2D>(TTexDir + "Red Dragon/Redfinal56"), game.Content.Load<Texture2D>(TTexDir + "Red Dragon/Redfinal3478") }, Color.White, TAnimTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Texture.CurrentFrame == 0)
            {
                Done = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Texture.Draw(gameTime, Game.Services.GetService<SpriteBatch>(), ScreenPosition, new Vector2(Texture.Width / 2, Texture.Height / 2), 0f);
        }
    }
}
