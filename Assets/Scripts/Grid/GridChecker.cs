using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameManager;
using Placeables;
using Placeables.Interactables.BasicColors;
using Tile;
using UnityEngine;

namespace Grid
{
    public class GridChecker : SingletonMB<GridChecker>
    {
        private void OnEnable()
        {
            GameManager.GameManager.Instance.getGridData += CombineMatchedLists;
        }

        private void OnDisable()
        {
            GameManager.GameManager.Instance.getGridData -= CombineMatchedLists;
        }
        
        private BaseTile GetTileAtPosition(Vector2 position) //If needed.
        {
            return GridManager.Instance.tilesInGrid.TryGetValue(position, out var tile) ? tile : null;
        }

        private void CheckForRows()
        {
            foreach (var tile in GridManager.Instance.tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < GridManager.Instance.gridHeight)
                {
                    for (int i = 1; i < GridManager.Instance.gridWidth; i++)
                    {
                        if ( tile.Key.x + i >= GridManager.Instance.gridWidth) break;
                        var originItem = tile.Value.occupiedPrefab.GetComponent<BasicColor>();
                        var neighbourTile = GetTileAtPosition(new Vector2(tile.Key.x + i, tile.Key.y)).occupiedPrefab.GetComponent<BasicColor>();
                        if (neighbourTile.selectedColor == originItem.selectedColor)
                        {
                            if (!originItem.matchedNeighbourItems.Contains(neighbourTile))
                            {
                                originItem.matchedNeighbourItems.Add(neighbourTile);
                            }
                            if (!originItem.matchedNeighbourItems.Contains(originItem))
                            {
                                originItem.matchedNeighbourItems.Add(originItem);
                            }
                        }
                        else
                        {
                            if (originItem.matchedNeighbourItems.Contains(neighbourTile))
                            {
                                originItem.matchedNeighbourItems.Remove(neighbourTile);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checking all rows from end position until start position to find matched items.
        /// </summary>
        private void CheckForRowsReverse()
        {
            var reverseTilesInGrid = GridManager.Instance.tilesInGrid.Reverse();
            foreach (var tile in reverseTilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < GridManager.Instance.gridHeight)
                {
                    for (int j = 1; j < GridManager.Instance.gridWidth; j++)
                    {
                        if ( tile.Key.x - j < 0) break;
                        var originItem = tile.Value.occupiedPrefab.GetComponent<BasicColor>();
                        var neighbourTile = GetTileAtPosition(new Vector2(tile.Key.x - j, tile.Key.y)).occupiedPrefab.GetComponent<BasicColor>();
                        if (neighbourTile.selectedColor == originItem.selectedColor)
                        {
                            if (!originItem.matchedNeighbourItems.Contains(neighbourTile))
                            {
                                originItem.matchedNeighbourItems.Add(neighbourTile);
                            }
                            if (!originItem.matchedNeighbourItems.Contains(originItem))
                            {
                                originItem.matchedNeighbourItems.Add(originItem);
                            }
                        }
                        else
                        {
                            if (originItem.matchedNeighbourItems.Contains(neighbourTile))
                            {
                                originItem.matchedNeighbourItems.Remove(neighbourTile);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checking all columns from start position until end position to find matched items.
        /// </summary>
        private void CheckForColumns()
        {
            foreach (var tile in GridManager.Instance.tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < GridManager.Instance.gridHeight)
                {
                    for (int i = 1; i < GridManager.Instance.gridHeight; i++)
                    {
                        if (tile.Key.y + i >= GridManager.Instance.gridHeight) break;
                        var originItem = tile.Value.occupiedPrefab.GetComponent<BasicColor>();
                        var neighbourTile = GetTileAtPosition(new Vector2(tile.Key.x, tile.Key.y + i)).occupiedPrefab.GetComponent<BasicColor>();
                        if (neighbourTile.selectedColor == originItem.selectedColor)
                        {
                            if (!originItem.matchedNeighbourItems.Contains(neighbourTile))
                            {
                                originItem.matchedNeighbourItems.Add(neighbourTile);
                            }
                            if (!originItem.matchedNeighbourItems.Contains(originItem))
                            {
                                originItem.matchedNeighbourItems.Add(originItem);
                            }
                        }
                        else
                        {
                            if (originItem.matchedNeighbourItems.Contains(neighbourTile))
                            {
                                originItem.matchedNeighbourItems.Remove(neighbourTile);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checking all columns from end position until start position to find matched items.
        /// </summary>
        private void CheckForColumnsReverse()
        {
            var reverseTilesInGrid = GridManager.Instance.tilesInGrid.Reverse();
            foreach (var tile in reverseTilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < GridManager.Instance.gridHeight)
                {
                    for (int j = 1; j < GridManager.Instance.gridHeight; j++)
                    {
                        if (tile.Key.y - j < 0) break;
                        var originItem = tile.Value.occupiedPrefab.GetComponent<BasicColor>();
                        var neighbourTile = GetTileAtPosition(new Vector2(tile.Key.x, tile.Key.y - j)).occupiedPrefab.GetComponent<BasicColor>();
                        if (neighbourTile.selectedColor == originItem.selectedColor)
                        {
                            if (!originItem.matchedNeighbourItems.Contains(neighbourTile))
                            {
                                originItem.matchedNeighbourItems.Add(neighbourTile);
                            }
                            if (!originItem.matchedNeighbourItems.Contains(originItem))
                            {
                                originItem.matchedNeighbourItems.Add(originItem);
                            }
                        }
                        else
                        {
                            if (originItem.matchedNeighbourItems.Contains(neighbourTile))
                            {
                                originItem.matchedNeighbourItems.Remove(neighbourTile);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// After checking all rows and columns combine it together.
        /// </summary>
        private void CombineMatchedLists()
        {
            CheckForColumns();
            CheckForRows();
            CheckForColumnsReverse();
            CheckForRowsReverse();
            var tempList = new List<BasePlaceable>();
            foreach (var tile in GridManager.Instance.tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < GridManager.Instance.gridHeight)
                {
                    var originItem = tile.Value.occupiedPrefab.GetComponent<BasicColor>();
                    if (originItem.matchedNeighbourItems.Count == 0) continue;
                    foreach (var matchedNeighbourItem in originItem.matchedNeighbourItems)
                    {
                        tempList.Add(matchedNeighbourItem);
                        if (originItem.matchedNeighbourItems.Count != matchedNeighbourItem.matchedNeighbourItems.Count )
                        {
                            for (int i = 0; i < matchedNeighbourItem.matchedNeighbourItems.Count; i++)
                            {
                                if (!matchedNeighbourItem.matchedNeighbourItems[i].matchedNeighbourItems.Contains(originItem)) matchedNeighbourItem.matchedNeighbourItems[i].matchedNeighbourItems.Add(originItem);
                            }
                        }
                    }
                }
            }

            GridManager.Instance.matchedPlaceableItemsList.Clear();
            foreach (var placeableItem in tempList)
            {
                if (!GridManager.Instance.matchedPlaceableItemsList.Contains(placeableItem)) GridManager.Instance.matchedPlaceableItemsList.Add(placeableItem);
            }
            GridManager.Instance.matchedPlaceableItemsList = GridManager.Instance.matchedPlaceableItemsList
                .Where(i => i != null)
                .OrderBy(t => t.GetComponent<BasicColor>().selectedColor)
                .ThenBy(x => Mathf.Round(x.transform.position.x))
                .ThenBy(y => Mathf.Round(y.transform.position.y)).ToList();
            GameManager.GameManager.Instance.ChangeState(GameState.WaitForInput);
            // CheckForShuffle();
        }

        private void CheckForShuffle()
        {
            if (GameManager.GameManager.Instance.gameState != GameState.CheckForCombos) return;
            if (GridManager.Instance.matchedPlaceableItemsList.Count == 0)
            {
                GameManager.GameManager.Instance.ChangeState(GameState.OperatingGrid);
                var tempList = new List<BasePlaceable>();
                var random = new System.Random();
                foreach (var baseTile in GridManager.Instance.tilesInGrid)
                {
                    if (baseTile.Value.occupiedPrefab != null)
                    {
                        Tween shakeTween = baseTile.Value.transform.DOShakePosition(0.2f, 0.4f, 3, 10, true);
                        shakeTween.OnComplete(() =>
                            Destroy(baseTile.Value.GetComponentInChildren<BasicColor>().gameObject));
                        tempList.Add(baseTile.Value.occupiedPrefab);
                        baseTile.Value.occupiedPrefab = null;
                    }
                }
                tempList = tempList.OrderBy(x => random.Next()).ToList();
                foreach (var baseTile in GridManager.Instance.tilesInGrid)
                {
                    if (baseTile.Key.y >= GridManager.Instance.gridHeight) continue;
                    if (tempList.Count != 0)
                    {
                        var spawnedItem = Instantiate(tempList.First(), baseTile.Key, Quaternion.identity,
                            baseTile.Value.transform);
                        baseTile.Value.occupiedPrefab = spawnedItem;
                        spawnedItem.name = $"{baseTile.Value.occupiedPrefab.name}";
                        spawnedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)baseTile.Key.y;
                        spawnedItem.GetComponent<BasicColor>().enabled = true;
                        spawnedItem.GetComponent<BoxCollider2D>().enabled = true;
                        tempList.Remove(tempList.First());
                    }
                }
                GameManager.GameManager.Instance.ChangeState(GameState.CheckForCombos);
            }
        }
    }
}
