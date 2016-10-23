﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class WaveManager : GameComponent
    {
        public int WaveNumber { get; private set; }
        public List<Wave> Waves { get; private set; }

        public Level Level;

        public WaveManager(Game game, Level level) : base(game)
        {
            this.Level = level;

            WaveNumber = 0;
            Waves = new List<Wave>();

            // TODO: Restructure waves to read from files?
            Wave tempWave = new Wave(game);
            tempWave.AddEnemies(5, Enemy.EnemyType.Basic, 0.5f);
            tempWave.AddEnemies(2, Enemy.EnemyType.Flying, 0.8f);
            Waves.Add(tempWave);
        }

        /// <summary>
        /// Starts the spawns of the current wave's enemies with a known path
        /// </summary>
        /// <param name="EntityArray"></param>
        public void StartWave(Spawn start, Treasure goal, HexEntity[,] EntityArray)
        {
            List<HexEntity> Path = CreatePath(start, goal, EntityArray);

            foreach (EnemyWave desc in Waves[WaveNumber].EnemyDepictions)
            {
                for (int i = 0; i < desc.Count; i++)
                {
                    Enemy tmpEnemy = start.CreateEnemy(desc.Type, goal, Path);
                    tmpEnemy.FreezeTime = desc.Delay * i;
                    Level.EnemyList.Add(tmpEnemy);
                }
            }
        }

        /// <summary>
        /// Returns a List of every hex along the path 
        /// between the start and goal hexes (A* pathfinding)
        /// </summary>
        /// <param name="start">hex to start from</param>
        /// <param name="goal">hex to end on</param>
        /// <param name="EntityArray">array of all hexes</param>
        /// <returns>list leading with "steps" from start to goal (null if no such path)</returns>
        public static List<HexEntity> CreatePath(HexEntity start, HexEntity goal, HexEntity[,] EntityArray)
        {
            // Store list of farthest extents of explored region
            SortedSet<Tuple<HexEntity, int>> frontier = new SortedSet<Tuple<HexEntity, int>>(new HexTupleCompare());
            frontier.Add(new Tuple<HexEntity, int>(start, 0));

            // Store details of known distances
            Dictionary<Point, HexEntity> came_from = new Dictionary<Point, HexEntity>();
            Dictionary<Point, int> cost_to = new Dictionary<Point, int>();

            came_from[start.Position] = null;
            cost_to[start.Position] = 0;

            while (frontier.Count > 0)
            {
                // Grab the minimal-distance known hex
                HexEntity current = frontier.Min.Item1;
                frontier.Remove(frontier.Min);

                if (current.Position == goal.Position) break;

                // Check neighbors for progress toward the end
                foreach (HexEntity next in HexEntity.GetNeighbors(current, EntityArray))
                {
                    int cost_here = cost_to[current.Position] + 1; // Neighbors are 1 step away
                    if (!came_from.ContainsKey(next.Position) || cost_here < cost_to[next.Position])
                    {
                        cost_to[next.Position] = cost_here; // Update to new path for this location
                        int priority = cost_here + HexEntity.Distance(next, goal);
                        frontier.Add(new Tuple<HexEntity, int>(next, priority));
                        came_from[next.Position] = current;
                    }
                }
            }

            // No Path found
            if (!came_from.ContainsKey(goal.Position))
            {
                return null;
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

    class HexTupleCompare : IComparer<Tuple<HexEntity, int>>
    {
        public int Compare(Tuple<HexEntity, int> v1, Tuple<HexEntity, int> v2)
        {
            if (v1.Item2 != v2.Item2)
            {
                return v1.Item2 - v2.Item2;
            }
            else if (v1.Item1 == v2.Item1)
            {
                return 0;
            }
            else {
                return -1;
            }
        }
    }
}
