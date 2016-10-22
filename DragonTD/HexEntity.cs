using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class HexEntity : DrawableGameComponent
    {
        /// <summary>
        ///2D position in array for pathfinding
        /// </summary>
        public Point Position;

        /// <summary>
        /// Rotation in radians
        /// </summary>
        public float Rotation;

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

        public Color Color = Color.White;

        /// <summary>
        /// The level on which the Hex exists
        /// </summary>
        public Level Level;

        SpriteBatch spriteBatch;

        public HexEntity(Game game, Level level, Point position, Texture2D texture, bool passable) : base(game)
        {
            Position = position;
            ScreenPosition = CalculateScreenPosition(Position);
            Texture = texture;
            Passable = passable;
            spriteBatch = game.Services.GetService<SpriteBatch>();
            Level = level;
        }

        /// <summary>
        /// Draw method for all Hex Entities. Can be Overridden if entity needs to be rotated, or treated specially.
        /// Assumes spriteBatch.Begin has already been called!!!!!
        /// Origin moved to middle of texture to make rotating tower sprites easier.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, ScreenPosition, null, Color, Rotation, new Vector2(64, 74), 1f, SpriteEffects.None, 0f);
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
            Vector2 sPosition = position.ToVector2() * new Vector2(128, 111);

            if (position.Y % 2 == 0) //if row is even, offset
                sPosition.X += 64;

            return sPosition;
        }

        /// <summary>
        /// Creates a list of all neighboring HexEntities
        /// where entities off the edges are not included.
        /// </summary>
        /// <param name="ThisHex">hex to find its neighbors</param>
        /// <param name="EntityArray">array of all hexes</param>
        /// <returns>list of neighbors</returns>
        public static List<HexEntity> GetNeighbors(HexEntity ThisHex, HexEntity[][] EntityArray)
        {
            // Differs based on row count due to even-r coordinates
            Point[] offsetsOdd = { new Point(1, 0), new Point(1, -1), new Point(0, -1),
                new Point(-1, 0), new Point(0, 1), new Point(1, 1) };
            Point[] offsetsEven = { new Point(1, 0), new Point(0, -1), new Point(-1, -1),
                new Point(-1, 0), new Point(-1, 1), new Point(0, 1) };

            List<HexEntity> neighbors = new List<HexEntity>();

            if (ThisHex.Position.Y % 2 == 0) // Even row
            {
                foreach (Point p in offsetsEven)
                {
                    Point ndx = p + ThisHex.Position;

                    // Avoid out-of-bounds
                    if (ndx.Y >= 0 && ndx.Y < EntityArray.Length &&
                        ndx.X >= 0 && ndx.X < EntityArray[ndx.Y].Length &&
                        EntityArray[ndx.Y][ndx.X].Passable)
                    {
                        neighbors.Add(EntityArray[ndx.Y][ndx.X]);
                    }
                }
            }
            else // Odd row
            {
                foreach (Point p in offsetsOdd)
                {
                    Point ndx = p + ThisHex.Position;

                    // Avoid out-of-bounds
                    if (ndx.Y >= 0 && ndx.Y < EntityArray.Length &&
                        ndx.X >= 0 && ndx.X < EntityArray[ndx.Y].Length &&
                        EntityArray[ndx.Y][ndx.X].Passable)
                    {
                        neighbors.Add(EntityArray[ndx.Y][ndx.X]);
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Calculates the "Manhattan" distance
        /// </summary>
        /// <param name="start">hex1</param>
        /// <param name="goal">hex2</param>
        /// <returns>Integer number of "steps"</returns>
        public static int Distance(HexEntity start, HexEntity goal)
        {
            Point3D a = CubeCoords(start.Position);
            Point3D b = CubeCoords(goal.Position);

            return (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z)) / 2;
        }

        /// <summary>
        /// Converts to Cube-based coordinates
        /// </summary>
        /// <param name="OffsetCoords">Offset-based coordinates</param>
        /// <returns>Cube-based coordinates</returns>
        public static Point3D CubeCoords(Point OffsetCoords)
        {
            Point3D NewCoords = new Point3D();
            NewCoords.X = OffsetCoords.X - (OffsetCoords.Y + (OffsetCoords.Y % 2)) / 2;
            NewCoords.Z = OffsetCoords.Y;
            NewCoords.Y = -NewCoords.X - NewCoords.Z;

            return NewCoords;
        }
    }
}
