using Microsoft.Xna.Framework;

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

        ///Calculate screen position of hex enity based on 2d array coordinate grid.
        ///using even-r horizontal layout, offset coordinates
        ///assumes 128px x 148px hex tile
        ///http://www.redblobgames.com/grids/hexagons/#coordinates
        public static Vector2 CalculateScreenPosition(Point position)
        {
            //the tip of the next row is inset, so to avoid math I just subract it here
            Vector2 sPosition = position.ToVector2() * new Vector2(128, 112);

            if (position.Y % 2 == 0) //if row is even, offset
                sPosition.X += 64;

            return sPosition;
        }
        
    }
}
