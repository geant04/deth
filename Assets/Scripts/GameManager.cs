using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private int rounds;
    public GameObject normalEnemy;
    public static List<GameObject> enemies = new List<GameObject>();
    
    public Tilemap tilemap;
    public TileBase ruleTile;
    
    private Vector2Int start = new Vector2Int(1, 1);  
    private Vector2Int end = new Vector2Int(3, 3);    
    private List<Vector2Int> solutionPath;
    
    private int[][] currentMap;
    int getRounds() {
        return rounds;
    }
    

    private static int[][] a1 = {
        new int[] {1,1,1,1,1},
        new int[] {1,0,1,0,1},
        new int[] {1,0,1,0,1},
        new int[] {1,0,0,0,1},
        new int[] {1,1,1,1,1}
    };

    private static int[][] a2 = {
        new int[] {1,1,1,1,1},
        new int[] {1,0,0,0,1},
        new int[] {1,0,1,1,1},
        new int[] {1,0,0,0,1},
        new int[] {1,1,1,1,1}
    };

    private static int[][] a3 = {
        new int[] {1,1,1,1,1},
        new int[] {1,0,0,0,1},
        new int[] {1,0,1,0,1},
        new int[] {1,0,1,0,1},
        new int[] {1,1,1,1,1}
    };

    private static int[][] a4 = {
        new int[] {1,1,1,1,1},
        new int[] {1,0,0,0,1},
        new int[] {1,1,1,0,1},
        new int[] {1,0,0,0,1},
        new int[] {1,1,1,1,1}
    };
    
    public static Tuple<List<int[]>, List<int>> RandomMapGeneration(int size)
    {
        List<int[][]> maps = new List<int[][]> { a1, a2, a3, a4 };
        List<int> exits = new List<int>();

        int[][] L1 = maps[new Random().Next(maps.Count)];
        for (int i = 0; i < size - 1; i++)
            L1 = AppendXGeneration(new List<int[]>(L1), new List<int[]>(maps[new Random().Next(maps.Count)])).ToArray();

        List<int[]> final_map = new List<int[]>(L1);
        for (int i = 0; i < size - 1; i++)
        {
            int[][] L2 = maps[new Random().Next(maps.Count)];
            for (int j = 0; j < size - 1; j++)
                L2 = AppendXGeneration(new List<int[]>(L2), new List<int[]>(maps[new Random().Next(maps.Count)])).ToArray();

            Tuple<int, List<int[]>> result = AppendYGeneration(final_map, new List<int[]>(L2));
            exits.Add(result.Item1);
            final_map = result.Item2;
        }

        return new Tuple<List<int[]>, List<int>>(final_map, exits);
    }

    private void MapToTile()
    {
        var width = currentMap[0].Length;
        var height = currentMap.Length;
        
        Vector2Int offset = new Vector2Int(-Mathf.FloorToInt(width / 2f), -Mathf.FloorToInt(height / 2f));
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (currentMap[y][x] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), ruleTile);
                }
            }
        }
    }

    
    void generateRound()
    {
        rounds++;
        
        Debug.Log("Round " + rounds);

        // Generate a random map of size 5x5 (or any other size)
        Tuple<List<int[]>, List<int>> result = RandomMapGeneration(5);
        currentMap = result.Item1.ToArray();

        tilemap.ClearAllTiles();
        MapToTile();
        
        for(int i = 0; i < rounds; i++)
        {
            GameObject enemy = Instantiate(normalEnemy, new Vector3(Random.Range(-5.0f, 5.0f), 
                                            Random.Range(-5.0f, 5.0f), 0.0f), Quaternion.identity) as GameObject;
            enemies.Add(enemy);
        }
    }

    void Start()
    {
        generateRound();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemies.Count > 0) {
            return;
        }
        
        generateRound();
    }
}
