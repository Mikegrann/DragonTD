using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    abstract class HexEntity : DrawableGameComponent
    {
        /// <summary>
        ///2D position in array for pathfinding
        /// </summary>
        public Point Position;

        /// <summary>
        ///2D position of entity on screen for drawing
        /// </summary>
        public Vector2 ScreenPosition;

        /// <summary>
        /// Is this tile traversable for pathfinding.
        /// </summary>
        public bool Passable;

        /// <summary>
        /// Hex Tile Texture
        /// </summary>
        public Texture2D Texture;

        SpriteBatch spriteBatch;

        public HexEntity(Game game, Point position, Texture2D texture, bool passable) : base(game)
        {
            Position = position;
            ScreenPosition = CalculateScreenPosition(Position);
            Texture = texture;
            Passable = passable;
            spriteBatch = game.Services.GetService<SpriteBatch>();
        }

        /// <summary>
        /// Draw method for all Hex Entities. Can be Overridden if entity needs to be rotated, or treated specially.
        /// Assumes spriteBatch.Begin has already been called!!!!!
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(Texture, ScreenPosition, Color.White);
        }


        /// <summary>
        /// Calculate Screen position from 2d array position
        /// using even-r horizontal layout, offset coordinates
        /// assumes 128px by 148px hex tile
        /// </summary>
        /// <param name="position">2d array position</param>
        /// <returns>ScreenPosition</returns>
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
