using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCity
{
    /// <summary>Result of parsing an ASCII level map. Pure data, EditMode-testable.</summary>
    public class ParsedLevel
    {
        public int Width;
        public int Height;
        /// <summary>Indexed [x, y] with y = 0 at the BOTTOM (world coordinates).</summary>
        public TileType[,] Tiles;
        public Vector2 PlayerSpawn;
        public List<Vector2> EnemySpawns = new List<Vector2>();
        public Vector2 EaglePosition;
    }

    /// <summary>
    /// Pure C# parser for LevelDefinition maps. World convention: tile (x, y) sits at
    /// world position (x, y); row 0 of the string array is the top row, so its tiles
    /// get y = Height - 1.
    /// </summary>
    public static class LevelParser
    {
        public static ParsedLevel Parse(string[] rows)
        {
            if (rows == null || rows.Length == 0)
                throw new ArgumentException("Level map is empty");

            int height = rows.Length;
            int width = rows[0].Length;
            var level = new ParsedLevel
            {
                Width = width,
                Height = height,
                Tiles = new TileType[width, height],
            };

            bool foundPlayer = false, foundEagle = false;
            for (int row = 0; row < height; row++)
            {
                if (rows[row].Length != width)
                    throw new ArgumentException($"Row {row} has length {rows[row].Length}, expected {width}");

                int y = height - 1 - row;
                for (int x = 0; x < width; x++)
                {
                    char c = rows[row][x];
                    level.Tiles[x, y] = LevelDefinition.CharToTile(c);
                    var pos = new Vector2(x, y);
                    switch (c)
                    {
                        case 'P':
                            if (foundPlayer) throw new ArgumentException("Multiple player spawns ('P') in map");
                            level.PlayerSpawn = pos;
                            foundPlayer = true;
                            break;
                        case '1':
                        case '2':
                        case '3':
                            level.EnemySpawns.Add(pos);
                            break;
                        case 'E':
                            if (foundEagle) throw new ArgumentException("Multiple eagles ('E') in map");
                            level.EaglePosition = pos;
                            foundEagle = true;
                            break;
                    }
                }
            }

            if (!foundPlayer) throw new ArgumentException("No player spawn ('P') in map");
            if (!foundEagle) throw new ArgumentException("No eagle ('E') in map");
            if (level.EnemySpawns.Count == 0) throw new ArgumentException("No enemy spawns ('1'-'3') in map");

            return level;
        }
    }
}
