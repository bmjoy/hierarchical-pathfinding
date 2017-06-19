﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Map
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Boundaries Boundaries { get; set; }

    public int FreeTiles { get; set; }

    //Consider storing obstacles in a Hashset to save memory on large maps
    //Obstacles are stores with the y value in the first array and the x value in the second array
    public bool[][] Obstacles { get; set; }

    //Original characters that forms the whole map
    //Tiles are stored with the y value in the first array and the x value in the second array
    public char[][] Tiles { get; set; }


    private Map() {}


    //Returns whether the tile is a valid free tile in the map or not
    public bool IsFreeTile(GridTile tile)
    {
        return tile.x >= 0 && tile.x < Width &&
            tile.y >= 0 && tile.y < Height &&
            !Obstacles[tile.y][tile.x];
    }


    public static List<FileInfo> GetMaps()
    {
        string BaseMapDirectory = GetBaseMapDirectory();
        DirectoryInfo d = new DirectoryInfo(BaseMapDirectory);
        return new List<FileInfo>(d.GetFiles("*.map"));
    }

    /// <summary>
    /// Loads a map from the base map directory
    /// </summary>
    /// <param name="MapName">File from which to load the map</param>
    public static Map LoadMap(string MapName)
    {
        string BaseMapDirectory = GetBaseMapDirectory();
        FileInfo f = new FileInfo(Path.Combine(BaseMapDirectory, MapName));

        return ReadMap(f);
    }

    /// <summary>
    /// Gets the base map directory
    /// </summary>
    private static string GetBaseMapDirectory()
    {
        return Path.Combine(Application.dataPath, "../Maps/map");
    }

    /// <summary>
    /// Reads map and returns a map object 
    /// </summary>
    private static Map ReadMap(FileInfo file)
    {
        Map map = new Map();

        using (FileStream fs = file.OpenRead())
        using (StreamReader sr = new StreamReader(fs))
        {

            //Line 1 : type octile
            ReadLine(sr, "type octile");

            //Line 2 : height
            map.Height = ReadIntegerValue(sr, "height");

            //Line 3 : width
            map.Width = ReadIntegerValue(sr, "width");

            //Set boundaries according to width and height
            map.Boundaries = new Boundaries
            {
                Min = new GridTile(0, 0),
                Max = new GridTile(map.Width - 1, map.Height - 1)
            };

            //Line 4 to end : map
            ReadLine(sr, "map");

            map.Obstacles = new bool[map.Height][];
            map.FreeTiles = 0;

            //Store the array of characters that makes up the map
            map.Tiles = new char[map.Height][];

            //Read tiles section
            ReadTiles(sr, map);

            return map;
        }
    }

    /// <summary>
    /// Read a line and expect the line to be the value passed in arguments
    /// </summary>
    private static void ReadLine(StreamReader sr, string value)
    {
        string line = sr.ReadLine();
        if (line != value) throw new Exception(
                string.Format("Invalid format. Expected: {0}, Actual: {1}", value, line));
    }

    /// <summary>
    /// Returns an integer value from the streamreader that comes
    /// right after a key separated by a space.
    /// I.E. width 5
    /// </summary>
    private static int ReadIntegerValue(StreamReader sr, string key)
    {
        string[] block = sr.ReadLine().Split(null);
        if(block[0] != key) throw new Exception(
                string.Format("Invalid format. Expected: {0}, Actual: {1}", key, block[0]));

        return int.Parse(block[1]);
    }

    /// <summary>
    /// Read tiles from the map file, adding tiles and filling obstacles in the array
    /// </summary>
    private static void ReadTiles(StreamReader sr, Map map)
    {
        char c;
        string line;

        for (int i = 0; i < map.Height; ++i)
        {
            line = sr.ReadLine();
            map.Obstacles[i] = new bool[map.Width];
            map.Tiles[i] = new char[map.Width];

            for (int j = 0; j < map.Width; ++j)
            {
                c = line[j];
                map.Tiles[i][j] = c;

                switch (c)
                {
                    case '@':
                    case 'O':
                        map.Obstacles[i][j] = true;
                        break;
                    case 'T':
                        map.Obstacles[i][j] = true;
                        break;
                    case '.':
                    case 'G':
                        map.Obstacles[i][j] = false;
                        map.FreeTiles++;
                        break;
                    default:
                        throw new Exception("Character not recognized");
                }
            }
        }
    }

}
