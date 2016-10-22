using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    abstract class HexEntity : DrawableGameComponent
    {
        ///2D position in array for pathfinding
        public Point Position;
        
        ///2D position of entity on screen for drawing
        public Vector2 ScreenPosition;

        public HexEntity(Game game) : base(game)
        {

        }
        
    }
}
