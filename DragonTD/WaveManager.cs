using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class WaveManager : GameComponent
    {
        public int WaveNumber { get; private set; }
        public List<Wave> Waves { get; private set; }

        public WaveManager(Game game) : base(game)
        {
            WaveNumber = 0;

            // TODO: Restructure waves to read from files?
            Wave tempWave = new Wave(game);
            tempWave.addWave(5, Enemy.EnemyType.Basic);
        }

        /// <summary>
        /// Starts the spawns of the current wave's enemies with a known path
        /// </summary>
        /// <param name="EntityArray"></param>
        public void StartWave(Spawn start, Treasure goal, HexEntity[,] EntityArray)
        {
            List<HexEntity> Path = CreatePath(start, goal, EntityArray);
        }

        /// <summary>
        /// Returns a List of every hex along the path 
        /// between the start and goal hexes (A* pathfinding)
        /// </summary>
        /// <param name="start">hex to start from</param>
        /// <param name="goal">hex to end on</param>
        /// <param name="EntityArray">array of all hexes</param>
        /// <returns>list leading with "steps" from start to goal</returns>
        public static List<HexEntity> CreatePath(HexEntity start, HexEntity goal, HexEntity[,] EntityArray)
        {
            // Store list of farthest extents of explored region
            SortedList<int, HexEntity> frontier = new SortedList<int, HexEntity>();
            frontier.Add(0, start);

            // Store details of known distances
            Dictionary<Point, HexEntity> came_from = new Dictionary<Point, HexEntity>();
            Dictionary<Point, int> cost_to = new Dictionary<Point, int>();

            came_from[start.Position] = null;
            cost_to[start.Position] = 0;

            while (frontier.Count > 0)
            {
                // Grab the minimal-distance known hex
                HexEntity current = frontier.GetEnumerator().Current.Value;

                if (current.Position == goal.Position) break;

                // Check neighbors for progress toward the end
                foreach (HexEntity next in HexEntity.GetNeighbors(current, EntityArray))
                {
                    int cost_here = cost_to[current.Position] + 1; // Neighbors are 1 step away
                    if (!came_from.ContainsKey(next.Position) || cost_here < cost_to[next.Position])
                    {
                        cost_to[next.Position] = cost_here; // Update to new path for this location
                        int priority = cost_here + HexEntity.Distance(next, goal);
                        frontier.Add(priority, next);
                        came_from[next.Position] = current;
                    }
                }
            }

            // Use A* output to build back to the desired hex path
            List<HexEntity> Path = new List<HexEntity>();
            for (HexEntity temp = goal; temp != null; temp = came_from[temp.Position])
            {
                Path.Insert(0, temp); // Insert at front
            }

            return Path;
        }
    }
}
