using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject normalEnemy;
    public GameObject railGunEnemy;
    public GameObject player;
    public Tilemap tilemap;
    public TileBase ruleTile;

    public Tilemap bgTilemap;
    public TileBase borderTile;
    public TileBase lightTile;
    public TileBase darkTile;

    private List<GameObject> shadowCasters = new List<GameObject>();
    public GameObject ShadowCasterPrefab;
    
    public static List<GameObject> EnemyList = new();
    public static int[][] CurrentMap;

    public static float Px;
    public static float Py;

    public TMP_Text bigRoundText;
    public TMP_Text roundText;
    public TMP_Text enemiesText;

    private float roundStartTime;
    
    private int _roundNum = 1;

    private static readonly List<int[,]> CellsList = new() {
        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,1,1,0,0,0,1,0,1},
            {1,0,1,1,0,1,0,1,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,1,1,0,0,0,1},
            {1,0,0,0,0,1,1,1,1,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,1,1,0,0,0,1},
            {1,0,0,0,1,1,0,0,0,1},
            {1,1,1,0,0,0,0,1,0,1},
            {1,0,0,0,0,0,0,1,0,1},
            {1,0,1,1,1,1,0,0,0,1},
            {1,0,1,0,0,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },

        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,0,0,0,1,0,0,1},
            {1,0,0,1,1,1,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,0,0,1,0,0,1,1,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },

        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,1,0,0,1},
            {1,0,0,0,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,0,0,1,1,1,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },
        
        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,1,1,1,1,1,1,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,0,0,1,1,1,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,1,1,1,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1,1,1,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,0,0,1,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,1,1,0,0,0,1},
            {1,0,0,0,1,1,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,1,0,1,1,1,1},
            {1,0,0,0,1,0,0,0,0,1},
            {1,0,0,0,1,1,1,0,0,1},
            {1,0,0,0,0,0,1,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1}
        }

    };

    private static List<int> GetRow(int[,] mat, int rowNum)
    {
        return Enumerable.Range(0, mat.GetLength(1)).Select(idx => mat[rowNum, idx]).ToList();
    }

    private static int[,] RotateMatrixClockwise(int[,] originalMatrix)
{
    int totalRows = originalMatrix.GetLength(0);
    int totalCols = originalMatrix.GetLength(1);
    int[,] rotatedMatrix = new int[totalCols, totalRows];

    for (int i = 0; i < totalRows; i++)
    {
        for (int j = 0; j < totalCols; j++)
        {
            rotatedMatrix[j, totalRows - i - 1] = originalMatrix[i, j];
        }
    }

    return rotatedMatrix;
}

    private static List<List<int>> GenerateMapRow(int size)
    {
        var mapRow = new List<List<int>>();
        var firstRandCell = CellsList[Random.Range(0, CellsList.Count)];

        for (var row = 0; row < firstRandCell.GetLength(0); row++)
        {
            mapRow.Add(GetRow(firstRandCell, row));
        }

        for (var i = 0; i < size - 1; i++)
        {
            var rotationcount = Random.Range(0,4);
            var randCell = CellsList[Random.Range(0, CellsList.Count)];
            for (var j = 0; j < rotationcount; j++)
            {
                randCell = RotateMatrixClockwise(randCell);
            }

            

            // 0 for top, 1 for below
            var rand = Random.Range(0, 3);
            var width = mapRow[0].Count;
                
            if (rand == 0)
            {
                mapRow[1][width - 1] = 0;
                mapRow[2][width - 1] = 0;
            }
            else if (rand == 1)
            {
                mapRow[4][width - 1] = 0;
                mapRow[5][width - 1] = 0;
            }
            else 
            {
                mapRow[7][width - 1] = 0;
                mapRow[8][width - 1] = 0;
            }

            for (var row = 0; row < mapRow.Count; row++)
            {
                mapRow[row].AddRange(GetRow(randCell, row).GetRange(1, randCell.GetLength(1) - 1));
            }
        }

        return mapRow;
    }

    private static int[][] GenerateFullMap(int w, int h)
    {
        var fullMap = new List<List<int>>();
        fullMap.AddRange(GenerateMapRow(w));

        for (var i = 0; i < h - 1; i++)
        {
            var mapRow = GenerateMapRow(w);
            mapRow.RemoveRange(0, 1);

            var width = fullMap[0].Count;
            var height = fullMap.Count;

            for (var col = 0; col < width; col += 6)
            {
                if (col + 1 == width) break;
                
                // 0 for left, 1 for right
                var rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    fullMap[height - 1][col + 1] = 0;
                    fullMap[height - 1][col + 2] = 0;
                }
                else if (rand == 1)
                {
                    fullMap[height - 1][col + 4] = 0;
                    fullMap[height - 1][col + 5] = 0;
                }
                else 
                {
                    fullMap[height - 1][col + 7] = 0;
                    fullMap[height - 1][col + 8] = 0;
                }

            }
            
            fullMap.AddRange(mapRow);
        }
        
        return fullMap.Select(row => row.ToArray()).ToArray();
    }

    private void MapToTile()
    {
        var width = CurrentMap[0].Length;
        var height = CurrentMap.Length;

        var offset = new Vector2Int(-Mathf.FloorToInt(width / 2f), -Mathf.FloorToInt(height / 2f));

        Vector3Int pos;
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                pos = new Vector3Int(x + offset.x - 1, y + offset.y - 1, 0);

                var row = y + 1;
                var col = x + 1;
                
                if (col % 2 == 1)
                {
                    if (row % 2 == 1)
                    {
                        bgTilemap.SetTile(pos, darkTile);
                    }
                    else
                    {
                        bgTilemap.SetTile(pos, lightTile);
                    }
                }
                else
                {
                    if (row % 2 == 1)
                    {
                        bgTilemap.SetTile(pos, lightTile);
                    }
                    else
                    {
                        bgTilemap.SetTile(pos, darkTile);
                    }
                }
                
                if (CurrentMap[y][x] == 1)
                {
                    tilemap.SetTile(pos, ruleTile);
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            pos = new Vector3Int(x + offset.x - 1, 0 - 1 + offset.y - 1, 0);
            bgTilemap.SetTile(pos, borderTile);
            pos = new Vector3Int(x + offset.x - 1, height + offset.y - 1, 0);
            bgTilemap.SetTile(pos, borderTile);
        }

        for (int y = 0; y < height; y++)
        {
            pos = new Vector3Int(0 - 1 + offset.x - 1, y + offset.y - 1, 0);
            bgTilemap.SetTile(pos, borderTile);
            pos = new Vector3Int(width + offset.x - 1, y + offset.y - 1, 0);
            bgTilemap.SetTile(pos, borderTile);
        }
        
        pos = new Vector3Int(0 - 1 + offset.x - 1, 0 - 1 + offset.y - 1, 0);
        bgTilemap.SetTile(pos, borderTile);
        pos = new Vector3Int(width + offset.x - 1, 0 - 1 + offset.y - 1, 0);
        bgTilemap.SetTile(pos, borderTile);
        pos = new Vector3Int(0 - 1 + offset.x - 1, height + offset.y - 1, 0);
        bgTilemap.SetTile(pos, borderTile);
        pos = new Vector3Int(width + offset.x - 1, height + offset.y - 1, 0);
        bgTilemap.SetTile(pos, borderTile);
    }

    private void GenerateTilemapShadowCasters() { 
        for(int i = 0; i < shadowCasters.Count; i++) {
            Destroy(shadowCasters[i]);
        }

        GameObject shadowCastersContainer = GameObject.Find("ShadowCasters");

        for(int i = 0; i < CurrentMap.Length; i++) {
            for(int j = 0; j < CurrentMap[0].Length; j++) {
                if(CurrentMap[i][j] == 1) {
                    GameObject ShadowCasterOBJ = Instantiate(ShadowCasterPrefab, new Vector3(j - (CurrentMap[0].Length / 2.0f), 
                                                             i - (CurrentMap.Length / 2.0f), 0.0f), Quaternion.identity, shadowCastersContainer.transform);
                    shadowCasters.Add(ShadowCasterOBJ);
                }
            }
        }
    }

    private void GenerateRound()
    {
        roundText.text = "Round " + _roundNum;
        bigRoundText.text = _roundNum.ToString();
        
        CurrentMap = GenerateFullMap(5, 3);
        tilemap.ClearAllTiles();
        bgTilemap.ClearAllTiles();
        MapToTile();
        GenerateTilemapShadowCasters();

        bool debug = true;

        for (var i = 0; i < Mathf.Min(20, _roundNum + 2); i++)
        {
            int y = Random.Range(1, CurrentMap.Length - 1);
            int x = Random.Range(1, CurrentMap[0].Length - 1);

            while(CurrentMap[y][x] == 1 || (Mathf.Abs(Py - (float)(y)) < 7.0f && Mathf.Abs(Px - (float)(x)) < 7.0f))
            {
                y = Random.Range(1, CurrentMap.Length - 1);
                x = Random.Range(1, CurrentMap[0].Length - 1);
            }

            float ey = (float) y - (CurrentMap.Length / 2.0f);
            float ex = (float) x - (CurrentMap[0].Length / 2.0f);

            var enemy = null as GameObject;
            
            enemy = Instantiate(debug && Random.Range(0,2) == 1 ? railGunEnemy : normalEnemy, new Vector3(ex, ey, 0.0f), Quaternion.identity);
            
            EnemyList.Add(enemy);
        }
        
        _roundNum++;
        roundStartTime = 1f;
    }

    private void Start()
    {
        EnemyList = new List<GameObject>();
        CurrentMap = new int[][] {};

        Px = 0f;
        Py = 0f;
        
        enemiesText.text = "Enemies left: " + EnemyList.Count;
        GenerateRound();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
        
        if (roundStartTime != 0f) 
        {
            roundStartTime = Mathf.Max(0.0f, roundStartTime - Time.deltaTime);
            Color textCol = bigRoundText.color;
            textCol.a = roundStartTime / 1f;
            bigRoundText.color = textCol;
        }
        
        if (EnemyList.Count > 0)
        {
            enemiesText.text = "Enemies left: " + EnemyList.Count;
            return;
        }

        GenerateRound();
    }
}
