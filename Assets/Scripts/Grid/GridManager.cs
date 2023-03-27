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
            ClearTiles();
            matchedPlaceablesList = new List<BasePlaceable>();
            columns = new List<BasePlaceable[]>();
            rows = new List<BasePlaceable[]>();
            tilesInGrid = new Dictionary<Vector2, BaseTile>();
            for (int i = 0; i < width; i++)
            {
                columns.Add(new BasePlaceable[height * 2]);
            }
            for (int j = 0; j < height * 2; j++)
            {
                rows.Add(new BasePlaceable[width]);
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height * 2; y++)
                {
                    var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity,transform);
                    spawnedTile.name = $"Tile {x} {y}";
                    spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
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
                var spawnedItem = Instantiate(wantedItemsList[random], tile.Value.transform.position,
                    Quaternion.identity, tile.Value.gameObject.transform);
                spawnedItem.name = $"{wantedItemsList[random].name}";
                spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)tile.Key.y;
                tile.Value.occupiedPrefab = spawnedItem.GetComponent<BasePlaceable>();
                columns[(int)tile.Key.x][(int)tile.Key.y] = spawnedItem;
                rows[(int)tile.Key.y][(int)tile.Key.x] = spawnedItem;
                if (spawnedItem.transform.position.y >= height)
                {
                    spawnedItem.gameObject.layer = LayerMask.NameToLayer("NotShow");
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
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.GetComponentInChildren<BasePlaceable>() == null)
                {
                    CreateWantedItemsAtPosition(tile.Key);
                }
                var item = tile.Value.GetComponentInChildren<BasePlaceable>();
                item.gameObject.layer = LayerMask.NameToLayer(item.gameObject.transform.position.y >= height ? "NotShow" : "Default");
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
            var newItem = GetTileAtPosition(oldPosition).GetComponentInChildren<BasePlaceable>();
            var newItemTransform = newItem.transform;
            newItemTransform.parent = GetTileAtPosition(newPosition).transform;
            newItemTransform.DOLocalMove(Vector3.zero, 1f, true);
            newItem.GetComponent<SpriteRenderer>().sortingOrder = (int)newPosition.y;
            if (matchedPlaceablesList.Contains(newItem)) matchedPlaceablesList.Remove(newItem);
            GetTileAtPosition(newPosition).occupiedPrefab = newItem;
        }

        private void Update()
        {
            OperateGrid();
        }

    }
}
