using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    // TODO: Figure out why this is its own class again?
    class Treasure : HexEntity
    {
        public Treasure(Game game, Level level, Point position) : base(game, level, position, (AnimatedSprite)null, true)
        {
            Texture = new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>("Textures/Start and End/TreasurePile") }, Color.White, 1f);
        }
    }
}
