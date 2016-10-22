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

        //TODO: implement
        public static Vector2 CalculateScreenPosition()
        {
            return Vector2.Zero;
        }
        
    }
}
