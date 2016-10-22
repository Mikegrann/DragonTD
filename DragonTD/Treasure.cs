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
        public Treasure(Game game, Point position, Texture2D texture) : base(game, position, texture, true)
        {

        }
    }
}
