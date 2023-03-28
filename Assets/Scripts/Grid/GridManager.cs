using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameManager;
using Placeables;
using Placeables.Interactables.BasicColors;
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
        [SerializeField] private BaseTile tilePrefab;

        [Header("Wanted Items")] 
        [SerializeField] private List<BasePlaceable> wantedItemsList;
        
        
        [Header("Game Camera")] 
        [SerializeField] private Camera gameCamera;
        
        [Header("Matched Tiles")]
        public List<BasePlaceable> matchedPlaceableItemsList;

        private Dictionary<Vector2, BaseTile> tilesInGrid;
        private List<BasePlaceable[]> columns; // If needed.
        private List<BasePlaceable[]> rows; // If needed.
        
        private int count = 0;
        private List<Vector2> itemsWillMoveOldPositionList;
        private List<Vector2> itemsWillMoveNewPositionList;

        private void Update()
        {
            OperateGrid();
            CombineMatchedLists();
        }

        public void GenerateGrid()
        {
            ClearTiles();
            matchedPlaceableItemsList = new List<BasePlaceable>();
            columns = new List<BasePlaceable[]>();
            rows = new List<BasePlaceable[]>();
            tilesInGrid = new Dictionary<Vector2, BaseTile>();
            for (int i = 0; i < width; i++)
            {
                columns.Add(new BasePlaceable[height * 2]);
            }
            for (int j = 0; j < height * 3; j++)
            {
                rows.Add(new BasePlaceable[width]);
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height * 3; y++)
                {
                    var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity,transform);
                    spawnedTile.name = $"Tile {x} {y}";
                    spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
                    tilesInGrid[new Vector2(x, y)] = spawnedTile;
                }
            }
            PlaceWantedItems();
            CenterCamera();
            GameManager.GameManager.Instance.ChangeState(GameState.CheckForCombos);
        }

        private void PlaceWantedItems()
        {
            foreach (var tile in tilesInGrid)
            {
                if (tile.Key.y < height)
                {
                    var random = Random.Range(1, wantedItemsList.Count);
                    var spawnedItem = Instantiate(wantedItemsList[random], tile.Value.transform.position,
                        Quaternion.identity, tile.Value.gameObject.transform);
                    spawnedItem.name = $"{wantedItemsList[random].name}";
                    spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)tile.Key.y;
                    tile.Value.occupiedPrefab = spawnedItem.GetComponent<BasePlaceable>();
                    columns[(int)tile.Key.x][(int)tile.Key.y] = spawnedItem;
                    rows[(int)tile.Key.y][(int)tile.Key.x] = spawnedItem;
                }
            }
        }
        
        private BaseTile GetTileAtPosition(Vector2 position) //If needed.
        {
            return tilesInGrid.TryGetValue(position, out var tile) ? tile : null;
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
        

        public void DestroyPlaceable(Vector2 position)
        {
            GameManager.GameManager.Instance.ChangeState(GameState.OperatingGrid);
            if (tilesInGrid.ContainsKey(position))
            {
                tilesInGrid.TryGetValue(position, out var tile);
                if (tile != null)
                {
                    var placeablePosition = tile.transform.position;
                    matchedPlaceableItemsList.Remove(tile.occupiedPrefab.GetComponent<BasePlaceable>()); 
                    Destroy(tile.GetComponentInChildren<BasePlaceable>().gameObject);
                    tile.occupiedPrefab = null;
                    // columns[(int)placeablePosition.x][(int)placeablePosition.y] = null;
                    CreateWantedItemsAtPosition(new Vector2((int)position.x,(int)position.y + height * 2));
                }
            }
        }

        public void DestroyPlaceable(List<BasicColor> basePlaceables)
        {
            GameManager.GameManager.Instance.ChangeState(GameState.OperatingGrid);
            foreach (var placeableItem in basePlaceables)
            {
                var position = placeableItem.transform.position;
                if (tilesInGrid.ContainsKey(position))
                {
                    tilesInGrid.TryGetValue(position, out var tile);
                    if (tile != null)
                    {
                        var placeablePosition = tile.transform.position;
                        matchedPlaceableItemsList.Remove(tile.occupiedPrefab.GetComponent<BasePlaceable>()); 
                        Destroy(tile.GetComponentInChildren<BasePlaceable>().gameObject);
                        tile.occupiedPrefab = null;
                        // columns[(int)placeablePosition.x][(int)placeablePosition.y] = null;
                        CreateWantedItemsAtPosition(new Vector2((int)position.x,(int)position.y + height * 2));
                    }
                }
            }
            
        }

        private void CreateWantedItemsAtPosition(Vector2 position)
        {
            var newPosition = new Vector2(position.x, position.y);
            var random = Random.Range(1, wantedItemsList.Count);
            var spawnedItem = Instantiate(wantedItemsList[random], newPosition,
                Quaternion.identity,GetTileAtPosition(position).transform);
            spawnedItem.name = $"{wantedItemsList[random].name}";
            spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)position.y;
            GetTileAtPosition(position).occupiedPrefab = spawnedItem;
        }
        

        private void ClearTiles()
        {
            if(tilesInGrid == null || tilesInGrid.Count == 0) return;
            foreach (var tile in tilesInGrid)
            {
                Destroy(tile.Value.gameObject);
            }
        }

        private void OperateGrid()
        {
            count = 0;
            itemsWillMoveOldPositionList = new List<Vector2>();
            itemsWillMoveNewPositionList = new List<Vector2>();
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y - 1 >= 0)
                {
                    var originPosition = tile.Key;
                    if (GetTileAtPosition(new Vector2(originPosition.x,originPosition.y - 1)).occupiedPrefab == null)
                    {
                        count++;
                        itemsWillMoveOldPositionList.Add(originPosition);
                        itemsWillMoveNewPositionList.Add(new Vector2(originPosition.x,originPosition.y - 1));
                    }
                }
            }

            if (count != 0)
            {
                StartCoroutine(MovePlaceableItem(itemsWillMoveOldPositionList,itemsWillMoveNewPositionList));
            }
        }

        private IEnumerator MovePlaceableItem(List<Vector2> oldPosition , List<Vector2> newPosition)
        {
            GameManager.GameManager.Instance.ChangeState(GameState.OperatingGrid);
            for (int i = 0; i < oldPosition.Count; i++)
            {
                GetTileAtPosition(oldPosition[i]).occupiedPrefab = null;
                var newItem = GetTileAtPosition(oldPosition[i]).GetComponentInChildren<BasePlaceable>();
                var newItemTransform = newItem.transform;
                newItemTransform.parent = GetTileAtPosition(newPosition[i]).transform;
                newItemTransform.DOLocalMove(Vector3.zero, 0.5f * Time.fixedDeltaTime, true);
                newItem.GetComponent<SpriteRenderer>().sortingOrder = (int)newPosition[i].y;
                GetTileAtPosition(newPosition[i]).occupiedPrefab = newItem;
            }
            yield return new WaitForSeconds(10f* Time.fixedDeltaTime);
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Clear();
                }
            }
            GameManager.GameManager.Instance.ChangeState(GameState.CheckForCombos);
        }

        private void CheckForRows()
        {
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    for (int i = 1; i < width; i++)
                    {
                        if ( tile.Key.x + i >= width) break;
                        var neighbourTile = GetTileAtPosition(new Vector2(tile.Key.x + i, tile.Key.y));
                        if (neighbourTile.occupiedPrefab.GetComponent<BasicColor>().selectedColor == tile.Value.occupiedPrefab.GetComponent<BasicColor>().selectedColor)
                        {
                            if (!tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Contains(neighbourTile.occupiedPrefab.GetComponent<BasicColor>()))
                            {
                                tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Add(neighbourTile.occupiedPrefab.GetComponent<BasicColor>());
                            }
                            if (!tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Contains(tile.Value.occupiedPrefab.GetComponent<BasicColor>()))
                            {
                                tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Add(tile.Value.occupiedPrefab.GetComponent<BasicColor>());
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            var reverseTilesInGrid = tilesInGrid.Reverse();
            foreach (var tile in reverseTilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    for (int j = 1; j < width; j++)
                    {
                        if ( tile.Key.x - j < 0) break;
                        var neighbourTile = GetTileAtPosition(new Vector2(tile.Key.x - j, tile.Key.y));
                        if (neighbourTile.occupiedPrefab.GetComponent<BasicColor>().selectedColor == tile.Value.occupiedPrefab.GetComponent<BasicColor>().selectedColor)
                        {
                            if (!tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Contains(neighbourTile.occupiedPrefab.GetComponent<BasicColor>()))
                            {
                                tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Add(neighbourTile.occupiedPrefab.GetComponent<BasicColor>());
                            }
                            if (!tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Contains(tile.Value.occupiedPrefab.GetComponent<BasicColor>()))
                            {
                                tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Add(tile.Value.occupiedPrefab.GetComponent<BasicColor>());
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void CheckForColumns()
        {
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    for (int i = 1; i < height; i++)
                    {
                        if (tile.Key.y + i >= height) break;
                        var neighbourTile = GetTileAtPosition(new Vector2(tile.Key.x, tile.Key.y + i));
                        if (neighbourTile.occupiedPrefab.GetComponent<BasicColor>().selectedColor == tile.Value.occupiedPrefab.GetComponent<BasicColor>().selectedColor)
                        {
                            if (!tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Contains(neighbourTile.occupiedPrefab.GetComponent<BasicColor>()))
                            {
                                tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Add(neighbourTile.occupiedPrefab.GetComponent<BasicColor>());
                            }
                            if (!tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Contains(tile.Value.occupiedPrefab.GetComponent<BasicColor>()))
                            {
                                tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Add(tile.Value.occupiedPrefab.GetComponent<BasicColor>());
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            var reverseTilesInGrid = tilesInGrid.Reverse();
            foreach (var tile in reverseTilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    for (int j = 1; j < height; j++)
                    {
                        if (tile.Key.y - j < 0) break;
                        var neighbourTile = GetTileAtPosition(new Vector2(tile.Key.x, tile.Key.y - j));
                        if (neighbourTile.occupiedPrefab.GetComponent<BasicColor>().selectedColor == tile.Value.occupiedPrefab.GetComponent<BasicColor>().selectedColor)
                        {
                            if (!tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Contains(neighbourTile.occupiedPrefab.GetComponent<BasicColor>()))
                            {
                                tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Add(neighbourTile.occupiedPrefab.GetComponent<BasicColor>());
                            }
                            if (!tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Contains(tile.Value.occupiedPrefab.GetComponent<BasicColor>()))
                            {
                                tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Add(tile.Value.occupiedPrefab.GetComponent<BasicColor>());
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void CombineMatchedLists()
        {
            if (GameManager.GameManager.Instance.gameState != GameState.CheckForCombos) return;
            CheckForColumns();
            CheckForRows();
            var tempList = new List<BasePlaceable>();
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    if (tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Count != 0)
                    {
                        foreach (var matchedItem in tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems)
                        {
                            tempList.Add(matchedItem);
                        }
                    }
                }
            }
            foreach (var placeableItem in tempList)
            {
                if (!matchedPlaceableItemsList.Contains(placeableItem)) matchedPlaceableItemsList.Add(placeableItem);
            }

            matchedPlaceableItemsList = matchedPlaceableItemsList.Where(x => x != null).ToList();
        }
    }
}
