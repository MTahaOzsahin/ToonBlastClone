using System.Collections.Generic;
using Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grid
{
    public class GridManager : SingletonMB<GridManager>
    {
        [Header("Grid Width and Height")]
        [SerializeField, Range(2, 10)] private int width;
        [SerializeField, Range(2, 10)] private int height;

        [Header("Tile Prefab")] 
        [SerializeField] private List<BaseTile> tilesPrefab;

        [Header("Wanted Colors"),Tooltip("If none selected will be all 6 colors")] 
        [SerializeField] private bool isWantedBlue;
        [SerializeField] private bool isWantedGreen;
        [SerializeField] private bool isWantedPink;
        [SerializeField] private bool isWantedPurple;
        [SerializeField] private bool isWantedRed;
        [SerializeField] private bool isWantedYellow;
        
        [Header("Game Camera")] 
        [SerializeField] private Camera gameCamera;
        
        [Header("Matched Tiles")]
        public List<BaseTile> matchedTileList;

        private Dictionary<Vector2, BaseTile> tiles;
        private List<SelectedColor> selectedTileColor;
        private List<BaseTile[]> columns; //If needed.
        private List<BaseTile[]> rows; //If needed.



        public void GenerateGrid()
        {
            matchedTileList = new List<BaseTile>();
            columns = new List<BaseTile[]>();
            rows = new List<BaseTile[]>();
            for (int i = 0; i < width; i++)
            {
                columns.Add(new BaseTile[height]);
            }

            for (int j = 0; j < height; j++)
            {
                rows.Add(new BaseTile[width]);
            }
            ClearTiles();
            GetWantedColor();
            tiles = new Dictionary<Vector2, BaseTile>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var random = Random.Range(0, selectedTileColor.Count);
                    var spawnedTile = Instantiate(tilesPrefab[random], new Vector3(x, y), Quaternion.identity,transform);
                    spawnedTile.Init(selectedTileColor[random]);
                    spawnedTile.name = $"Tile {x} {y} {spawnedTile.selectedColor.ToString()}";
                    spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
                    tiles[new Vector2(x, y)] = spawnedTile;
                    columns[x][y] = spawnedTile;
                    rows[y][x] = spawnedTile;
                }
            }
            CenterCamera();
            // GenerateReserveGrid();
        }

        // private void GenerateReserveGrid()
        // {
        //     for (int x = 0; x < width; x++)
        //     {
        //         for (int y = 16; y < height + 16; y++)  // +16 is to set above camera view.
        //         {
        //             var random = Random.Range(0, selectedTileColor.Count);
        //             var spawnedTile = Instantiate(tilesPrefab[random], new Vector3(x, y), Quaternion.identity,transform);
        //             spawnedTile.Init(selectedTileColor[random]);
        //             spawnedTile.name = $"Tile {x} {y} {spawnedTile.selectedColor.ToString()}";
        //             spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
        //             tiles[new Vector2(x, y)] = spawnedTile;
        //             // columns[x][y] = spawnedTile;
        //             // rows[y][x] = spawnedTile;
        //         }
        //     }
        // }

        public BaseTile GetTileAtPosition(Vector2 position) //If needed.
        {
            if (tiles.TryGetValue(position, out var tile))
            {
                return tile;
            }
            else
            {
                return null;
            }
        }

        private void CenterCamera()
        {
            gameCamera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10f);
            gameCamera.fieldOfView = width switch
            {
                7 => 68,
                8 => 76,
                9 => 84,
                10 => 92,
                _ => 60
            };
        }

        private void GetWantedColor()
        {
            selectedTileColor = new List<SelectedColor>();
            var blueColor = SelectedColor.Blue;
            var greenColor = SelectedColor.Green;
            var pinkColor = SelectedColor.Pink;
            var purpleColor = SelectedColor.Purple;
            var redColor = SelectedColor.Red;
            var yellowColor = SelectedColor.Yellow;
            
            if (isWantedBlue)
            {
                selectedTileColor.Add(blueColor);
            }
            if (isWantedGreen)
            {
                selectedTileColor.Add(greenColor);
            }
            if (isWantedPink)
            {
                selectedTileColor.Add(pinkColor);
            }
            if (isWantedPurple)
            {
                selectedTileColor.Add(purpleColor);
            }
            if (isWantedRed)
            {
                selectedTileColor.Add(redColor);
            }
            if (isWantedYellow)
            {
                selectedTileColor.Add(yellowColor);
            }
            if (selectedTileColor.Count == 0)
            {
                selectedTileColor.Add(blueColor);
                selectedTileColor.Add(greenColor);
                selectedTileColor.Add(pinkColor);
                selectedTileColor.Add(purpleColor);
                selectedTileColor.Add(redColor);
                selectedTileColor.Add(yellowColor);
            }
        }

        public void DestroyTiles(Vector2 position)
        {
            if (tiles.ContainsKey(position))
            {
                tiles.Remove(position);
            }

            if (matchedTileList.Count == 0)
            {
                GenerateGrid();
            }
        }

        private void ClearTiles()
        {
            if(tiles == null || tiles.Count == 0) return;
            foreach (var tile in tiles)
            {
                if ( tile.Value.gameObject != null) Destroy(tile.Value.gameObject);
            }
        }
    }
}
