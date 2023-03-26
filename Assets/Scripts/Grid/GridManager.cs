using System.Collections.Generic;
using DG.Tweening;
using Placeables;
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
        public List<BasePlaceable> matchedPlaceablesList;

        private Dictionary<Vector2, BaseTile> tilesInGrid;
        private List<BasePlaceable[]> columns; // If needed.
        private List<BasePlaceable[]> rows; // If needed.
        
        public void GenerateGrid()
        {
            matchedPlaceablesList = new List<BasePlaceable>();
            columns = new List<BasePlaceable[]>();
            rows = new List<BasePlaceable[]>();
            tilesInGrid = new Dictionary<Vector2, BaseTile>();
            for (int i = 0; i < width; i++)
            {
                columns.Add(new BasePlaceable[height]);
            }
            for (int j = 0; j < height; j++)
            {
                rows.Add(new BasePlaceable[width]);
            }
            ClearTiles();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity,transform);
                    spawnedTile.name = $"Tile {x} {y}";
                    spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
                    spawnedTile.GetComponent<BoxCollider2D>().enabled = true;
                    tilesInGrid[new Vector2(x, y)] = spawnedTile;
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
                tile.Value.GetComponent<BoxCollider2D>().enabled = false;
                var spawnedItem = Instantiate(wantedItemsList[random], tile.Value.transform.position,
                    Quaternion.identity, tile.Value.gameObject.transform);
                spawnedItem.name = $"{wantedItemsList[random].name}";
                spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)tile.Key.y;
                tile.Value.occupiedPrefab = spawnedItem.GetComponent<BasePlaceable>();
                columns[(int)tile.Key.x][(int)tile.Key.y] = spawnedItem;
                rows[(int)tile.Key.y][(int)tile.Key.x] = spawnedItem;
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
            var castedPosition = new Vector2((int)position.x, (int)position.y);
            if (tilesInGrid.ContainsKey(castedPosition))
            {
                tilesInGrid.TryGetValue(castedPosition, out var tile);
                if (tile != null)
                {
                    var placeablePosition = tile.transform.position;
                    matchedPlaceablesList.Remove(tile.GetComponent<BasePlaceable>()); 
                    Destroy(tile.GetComponentInChildren<BasePlaceable>().gameObject);
                    tile.occupiedPrefab = null;
                    columns[(int)placeablePosition.x][(int)placeablePosition.y] = null;
                    ReCreateWantedItems(position);
                }
            }
        }

        private void ReCreateWantedItems(Vector2 position)
        {
            var newPosition = new Vector2(position.x, position.y + 16);
            var random = Random.Range(1, wantedItemsList.Count);
            var spawnedItem = Instantiate(wantedItemsList[random], newPosition,
                Quaternion.identity);
            spawnedItem.name = $"{wantedItemsList[random].name}";
            spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)position.y;
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
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y - 1 >= 0)
                {
                    var originPosition = tile.Key;
                    if (GetTileAtPosition(new Vector2(originPosition.x,originPosition.y - 1)).occupiedPrefab == null)
                    {
                        MovePlaceableItem(originPosition,new Vector2(originPosition.x,originPosition.y -1));
                    }
                }
            }
        }

        private void MovePlaceableItem(Vector2 oldPosition , Vector2 newPosition)
        {
            GetTileAtPosition(oldPosition).occupiedPrefab = null;
            var oldItem = GetTileAtPosition(oldPosition).GetComponentInChildren<BasePlaceable>();
            var oldItemTransform = oldItem.transform;
            oldItemTransform.parent = GetTileAtPosition(newPosition).transform;
            oldItemTransform.DOLocalMove(Vector3.zero, 0.1f, true);
            oldItem.GetComponent<SpriteRenderer>().sortingOrder = (int)newPosition.y;
            if (matchedPlaceablesList.Contains(oldItem)) matchedPlaceablesList.Remove(oldItem);
            GetTileAtPosition(newPosition).occupiedPrefab = oldItem;
        }

        private void Update()
        {
            OperateGrid();
        }

    }
}
