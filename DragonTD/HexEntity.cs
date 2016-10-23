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

        /// <summary>
        /// How much the tile costs to place
        /// </summary>
        public int Cost = 0;

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

        public override string ToString()
        {
            return this.GetType().Name + " " + this.Position.X + ", " + this.Position.Y;
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
                spriteBatch.Draw(Texture, ScreenPosition, null, Color, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
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
        public static List<HexEntity> GetNeighbors(HexEntity ThisHex, HexEntity[,] EntityArray)
        {
            // Differs based on row count due to even-r coordinates
            Point[] offsetsEven = { new Point(1, 0), new Point(1, -1), new Point(0, -1),
                new Point(-1, 0), new Point(0, 1), new Point(1, 1) };
            Point[] offsetsOdd = { new Point(1, 0), new Point(0, -1), new Point(-1, -1),
                new Point(-1, 0), new Point(-1, 1), new Point(0, 1) };

            List<HexEntity> neighbors = new List<HexEntity>();

            if (ThisHex.Position.Y % 2 == 0) // Even row
            {
                foreach (Point p in offsetsEven)
                {
                    Point ndx = p + ThisHex.Position;

                    // Avoid out-of-bounds
                    if (ndx.Y >= 0 && ndx.Y <= EntityArray.GetUpperBound(0) &&
                        ndx.X >= 0 && ndx.X <= EntityArray.GetUpperBound(1) &&
                        EntityArray[ndx.Y, ndx.X].Passable)
                    {
                        neighbors.Add(EntityArray[ndx.Y, ndx.X]);
                    }
                }
            }
            else // Odd row
            {
                foreach (Point p in offsetsOdd)
                {
                    Point ndx = p + ThisHex.Position;

                    // Avoid out-of-bounds
                    if (ndx.Y >= 0 && ndx.Y <= EntityArray.GetUpperBound(0) &&
                        ndx.X >= 0 && ndx.X <= EntityArray.GetUpperBound(1) &&
                        EntityArray[ndx.Y, ndx.X].Passable)
                    {
                        neighbors.Add(EntityArray[ndx.Y, ndx.X]);
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
            Point3D a = OffsetToCube(start.Position);
            Point3D b = OffsetToCube(goal.Position);

            return (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z)) / 2;
        }


        public static Point3D OffsetToCube(Point OffsetCoords)
        {
            Point3D NewCoords = new Point3D();
            NewCoords.X = OffsetCoords.X - (OffsetCoords.Y + (OffsetCoords.Y % 2)) / 2;
            NewCoords.Z = OffsetCoords.Y;
            NewCoords.Y = -NewCoords.X - NewCoords.Z;

            return NewCoords;
        }

        public static Vector3 AxialToCube(Vector2 AxialCoords)
        {
            Vector3 NewCoords = new Vector3();
            NewCoords.X = AxialCoords.X;
            NewCoords.Z = AxialCoords.Y;
            NewCoords.Y = -NewCoords.X - NewCoords.Z;

            return NewCoords;
        }

        public static Point CubeToOffset(Point3D CubeCoords)
        {
            Point NewCoords = new Point();
            NewCoords.X = CubeCoords.X + (CubeCoords.Z + (CubeCoords.Z % 2)) / 2;
            NewCoords.Y = CubeCoords.Z;

            return NewCoords;
        }

        public static Point3D PixelToCube(Vector2 PixelCoords)
        {
            Vector2 NewCoords = new Vector2();
            NewCoords.X = (float)(PixelCoords.X * Math.Sqrt(3.0) / 3.0 - PixelCoords.Y / 3.0) / 74.0f;
            NewCoords.Y = (PixelCoords.Y * 2.0f / 3.0f) / 74.0f;

            return CubeRound(AxialToCube(NewCoords));
        }

        public static Point PixelToHex(Vector2 PixelCoords)
        {
            return CubeToOffset(PixelToCube(PixelCoords));
        }

        public static Point3D CubeRound(Vector3 CubeCoords)
        {
            Point3D NewCoords = new Point3D();
            NewCoords.X = (int)Math.Round(CubeCoords.X);
            NewCoords.Y = (int)Math.Round(CubeCoords.Y);
            NewCoords.Z = (int)Math.Round(CubeCoords.Z);

            Vector3 DiffCoords = new Vector3();
            DiffCoords.X = Math.Abs(NewCoords.X - CubeCoords.X);
            DiffCoords.Y = Math.Abs(NewCoords.Y - CubeCoords.Y);
            DiffCoords.Z = Math.Abs(NewCoords.Z - CubeCoords.Z);

            if (DiffCoords.X > DiffCoords.Y && DiffCoords.X > DiffCoords.Z)
            {
                NewCoords.X = -NewCoords.Y - NewCoords.Z;
            }
            else if (DiffCoords.Y > DiffCoords.Z)
            {
                NewCoords.Y = -NewCoords.X - NewCoords.Z;
            }
            else
            {
                NewCoords.Z = -NewCoords.X - NewCoords.Y;
            }

            return NewCoords;
        }
    }
}
