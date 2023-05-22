using System.Collections;
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
        public float cameraTopBound;

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
                    var random = Random.Range(0, GridManager.Instance.wantedItemsList.Count);
                    var spawnedItem = Instantiate(GridManager.Instance.wantedItemsList[random], tile.Value.transform.position,
                        Quaternion.identity, tile.Value.gameObject.transform);
                    spawnedItem.name = $"{GridManager.Instance.wantedItemsList[random].name}";
                    spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)tile.Key.y;
                    tile.Value.occupiedPrefab = spawnedItem.GetComponent<BasePlaceable>();
                    createdPlaceableList.Add(spawnedItem);
                }
            }
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
                var adjustedPosition = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
                if (GridManager.Instance.tilesInGrid.ContainsKey(adjustedPosition))
                {
                    GridManager.Instance.tilesInGrid.TryGetValue(adjustedPosition, out var tile);
                    if (tile != null)
                    {
                        GridManager.Instance.matchedPlaceableItemsList.Remove(tile.occupiedPrefab.GetComponent<BasePlaceable>()); 
                        createdPlaceableList.Remove(placeableItem);
                        Destroy(tile.GetComponentInChildren<BasePlaceable>().gameObject);
                        tile.occupiedPrefab = null;
                        if (cameraTopBound == 0f)
                        {
                            var spawnedItem = CreateWantedItemsAtPosition(new Vector2((int)adjustedPosition.x,(int)adjustedPosition.y + 10));
                            createdPlaceableList.Add(spawnedItem);
                        }
                        else
                        {
                            var spawnedItem = CreateWantedItemsAtPosition(new Vector2((int)adjustedPosition.x,(int)adjustedPosition.y + cameraTopBound + 1));
                            createdPlaceableList.Add(spawnedItem);
                        }
                    }
                }
            }
            StartCoroutine(RePlacedWantedItemsToGrid());
        }
        
        private IEnumerator RePlacedWantedItemsToGrid()
        {
            yield return new WaitForSeconds(0.8f);
            foreach (var basePlaceable in createdPlaceableList)
            {
                var basePlaceablePosition = basePlaceable.transform.position;
                var roundedPosition = new Vector2(Mathf.Round(basePlaceablePosition.x), Mathf.Round(basePlaceablePosition.y));
                GridManager.Instance.tilesInGrid.TryGetValue(roundedPosition, out var tile);
                if (tile != null)
                {
                    tile.occupiedPrefab = basePlaceable;
                    basePlaceable.transform.parent = tile.transform;
                    basePlaceable.GetComponent<SpriteRenderer>().sortingOrder = (int)(roundedPosition.y);
                }
                else
                {
                    yield break;
                }
                basePlaceable.transform.localPosition = Vector2.zero;
            }
            GameManager.GameManager.Instance.ChangeState(GameState.CheckForCombos);
        }

        private BasePlaceable CreateWantedItemsAtPosition(Vector2 position)
        {
            var newPosition = new Vector2(position.x, position.y);
            var random = Random.Range(0, GridManager.Instance.wantedItemsList.Count);
            var spawnedItem = Instantiate(GridManager.Instance.wantedItemsList[random], newPosition, Quaternion.identity);
            spawnedItem.name = $"{GridManager.Instance.wantedItemsList[random].name}";
            spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)position.y;
            return spawnedItem;
        }
    }
}
