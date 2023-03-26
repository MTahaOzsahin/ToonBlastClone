using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tile;
using Tile.Interactables.BasicColors;
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
        [SerializeField] private BaseTile tilePrefab;

        [Header("Wanted Items")] 
        [SerializeField] private List<GameObject> wantedItemsList;
        
        
        [Header("Game Camera")] 
        [SerializeField] private Camera gameCamera;
        
        [Header("Matched Tiles")]
        public List<BaseTile> matchedTileList;

        private Dictionary<Vector2, BaseTile> tilesInGrid;
        private List<SelectedColor> selectedTileColor;
        private List<BaseTile[]> columns;
        private List<BaseTile[]> rows;
        private List<BaseTile> tilesToDestroy;


        public void GenerateGrid()
        {
            matchedTileList = new List<BaseTile>();
            columns = new List<BaseTile[]>();
            rows = new List<BaseTile[]>();
            tilesToDestroy = new List<BaseTile>();
            for (int i = 0; i < width; i++)
            {
                columns.Add(new BaseTile[height]);
            }

            for (int j = 0; j < height; j++)
            {
                rows.Add(new BaseTile[width]);
            }
            ClearTiles();
            tilesInGrid = new Dictionary<Vector2, BaseTile>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity,transform);
                    spawnedTile.name = $"Tile {x} {y}";
                    spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
                    tilesInGrid[new Vector2(x, y)] = spawnedTile;
                    columns[x][y] = spawnedTile;
                    rows[y][x] = spawnedTile;
                }
            }
            PlaceWantedItems();
            CenterCamera();
        }

        private void PlaceWantedItems()
        {
            foreach (var tile in tilesInGrid)
            {
                var random = Random.Range(1, wantedItemsList.Count);
                tile.Value.GetComponent<SpriteRenderer>().enabled = false;
                var spawnedItem = Instantiate(wantedItemsList[random], tile.Value.transform.position,
                    Quaternion.identity, tile.Value.gameObject.transform);
                spawnedItem.name = $"{wantedItemsList[random].name}";
                spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)tile.Key.y;
                tile.Value.occupiedPrefab = spawnedItem.GetComponent<BasicColor>();
            }
        }
        
        public BaseTile GetTileAtPosition(Vector2 position) //If needed.
        {
            if (tilesInGrid.TryGetValue(position, out var tile))
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
        

        public void DestroyTiles(Vector2 position)
        {
            if (tilesInGrid.ContainsKey(position))
            {
                tilesInGrid.TryGetValue(position, out var tile);
                tilesToDestroy.Add(tile);
                matchedTileList.Remove(tile);
                if (tile != null) Destroy(tile.gameObject);
                tilesInGrid.Remove(position);
            }
        }

        private void ClearTiles()
        {
            if(tilesInGrid == null || tilesInGrid.Count == 0) return;
            foreach (var tile in tilesInGrid)
            {
                Destroy(tile.Value.gameObject);
            }
        }

        public IEnumerator OperateGrid()
        {
            var clearedTileToDestroyList = tilesToDestroy.Where(x => x != null).GroupBy(t => t.transform.position.x)
                .Select(grp => grp.Last()).OrderBy(h => h.transform.position.x).ToList();
            // var clearedTileToDestroyList = tilesToDestroy.Where(x => x != null).ToList();
            foreach (var destroyedTile in clearedTileToDestroyList)
            {
                var destroyedTilePosition = destroyedTile.transform.position;
                var destroyedTileY = destroyedTilePosition.y;
                for (int i = (int)destroyedTileY + 1; i < height; i++)
                {
                    if (columns[(int)destroyedTilePosition.x][(int)destroyedTilePosition.y + 1] != null)
                    {
                        columns[(int)destroyedTilePosition.x][(int)destroyedTilePosition.y + 1].transform.position =
                            new Vector3(destroyedTilePosition.x, destroyedTilePosition.y);
                        columns[(int)destroyedTilePosition.x][(int)destroyedTilePosition.y + 1].name =
                            $"Tile {destroyedTilePosition.x} {destroyedTilePosition.y} {columns[(int)destroyedTilePosition.x][(int)destroyedTilePosition.y + 1].GetComponent<BasicColor>().selectedColor.ToString()}";
                        columns[(int)destroyedTilePosition.x][(int)destroyedTilePosition.y + 1]
                            .GetComponent<SpriteRenderer>().sortingOrder = (int)destroyedTilePosition.y + 1;
                        destroyedTilePosition = new Vector3(destroyedTilePosition.x, destroyedTilePosition.y + 1);
                    }
                }
                var newColumnOrder = tilesInGrid.Values.Where(x => (int)x.transform.position.x == (int)destroyedTilePosition.x).ToArray();
                columns[(int)destroyedTilePosition.x] = newColumnOrder;
                
                // if (columns[(int)tilePosition.x][(int)tilePosition.y + 1] != null)
                // {
                //     columns[(int)tilePosition.x][(int)tilePosition.y + 1].transform.position =
                //         new Vector3(tilePosition.x, tilePosition.y);
                //     columns[(int)tilePosition.x][(int)tilePosition.y + 1].name =
                //         $"Tile {tilePosition.x} {tilePosition.y} {columns[(int)tilePosition.x][(int)tilePosition.y + 1].GetComponent<BasicColor>().selectedColor.ToString()}";
                //     var newColumnOrder = tilesInGrid.Values.Where(x => (int)x.transform.position.x == (int)tilePosition.x).ToArray();
                //     columns[(int)tilePosition.x] = newColumnOrder;
                // }
            }

            yield return null;

            // for (int i = 0; i < columns.Count; i++)
            // {
            //     for (int j = 0; j < columns[i].Length; j++)
            //     {
            //         var notDestroyedTiles = columns[i].Where(x => x != null).ToList();
            //         foreach (var tile in notDestroyedTiles)
            //         {
            //             var originPosition = tile.transform.position;
            //             var modifiedPosition = new Vector2(originPosition.x, originPosition.y - 1);
            //             tile.transform.position = new Vector3(modifiedPosition.x, modifiedPosition.y);
            //         }
            //     }
            // }
        }
    }
}
