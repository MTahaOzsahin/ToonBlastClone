using System.Collections.Generic;
using GameManager;
using Placeables;
using Placeables.Interactables.BasicColors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grid
{
    public class GridOperator : SingletonMB<GridOperator>
    {
        public List<BasePlaceable> createdPlaceableList;

        private void OnEnable()
        {
            createdPlaceableList = new List<BasePlaceable>();
        }

        public void CreateWantedItems()
        {
            foreach (var tile in GridManager.Instance.tilesInGrid)
            {
                if (tile.Key.y < GridManager.Instance.gridHeight)
                {
                    var random = Random.Range(1, GridManager.Instance.wantedItemsList.Count);
                    var spawnedItem = Instantiate(GridManager.Instance.wantedItemsList[random], tile.Value.transform.position,
                        Quaternion.identity, tile.Value.gameObject.transform);
                    spawnedItem.name = $"{GridManager.Instance.wantedItemsList[random].name}";
                    spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)tile.Key.y;
                    tile.Value.occupiedPrefab = spawnedItem.GetComponent<BasePlaceable>();
                    createdPlaceableList.Add(spawnedItem);
                    // columns[(int)tile.Key.x][(int)tile.Key.y] = spawnedItem;
                    // rows[(int)tile.Key.y][(int)tile.Key.x] = spawnedItem;
                }
            }
        }

        public void RePlacedWantedItemsToGrid()
        {
            
        }
        
        /// <summary>
        /// Destroy all placeable at once.
        /// </summary>
        /// <param name="basePlaceables"></param>
        public void DestroyPlaceable(List<BasicColor> basePlaceables)
        {
            GameManager.GameManager.Instance.ChangeState(GameState.OperatingGrid);
            foreach (var placeableItem in basePlaceables)
            {
                var position = placeableItem.transform.position;
                var adjustedPosition = new Vector2(position.x, Mathf.Round(position.y));
                if (GridManager.Instance.tilesInGrid.ContainsKey(adjustedPosition))
                {
                    GridManager.Instance.tilesInGrid.TryGetValue(adjustedPosition, out var tile);
                    if (tile != null)
                    {
                        var placeablePosition = tile.transform.position;
                        GridManager.Instance.matchedPlaceableItemsList.Remove(tile.occupiedPrefab.GetComponent<BasePlaceable>()); 
                        Destroy(tile.GetComponentInChildren<BasePlaceable>().gameObject);
                        tile.occupiedPrefab = null;
                        CreateWantedItemsAtPosition(new Vector2((int)adjustedPosition.x,(int)adjustedPosition.y + 10));
                    }
                }
            }
            GameManager.GameManager.Instance.ChangeState(GameState.CheckForCombos);
        }

        private void CreateWantedItemsAtPosition(Vector2 position)
        {
            var newPosition = new Vector2(position.x, position.y);
            var random = Random.Range(1, GridManager.Instance.wantedItemsList.Count);
            var spawnedItem = Instantiate(GridManager.Instance.wantedItemsList[random], newPosition, Quaternion.identity);
            spawnedItem.name = $"{GridManager.Instance.wantedItemsList[random].name}";
            spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)position.y;
        }
    }
}
