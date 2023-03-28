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
        [Header("Grid Width and Height"),Tooltip("Please do not change during playMode!")]
        [SerializeField, Range(2, 10)] private int width;
        [SerializeField, Range(2, 10)] private int height;

        [Header("Tile Prefab")] 
        [SerializeField] private BaseTile tilePrefab;

        [Header("Wanted Items")] 
        [SerializeField] private List<BasePlaceable> wantedItemsList;
        
        [Header("Wanted Colors. If none selected will be all colors"),Tooltip("For test only")]
        [SerializeField] private bool blue;
        [SerializeField] private bool green;
        [SerializeField] private bool pink;
        [SerializeField] private bool purple;
        [SerializeField] private bool red;
        [SerializeField] private bool yellow;

        [Header("Matched Tiles")]
        public List<BasePlaceable> matchedPlaceableItemsList;

        public System.Action<int, int> OnGameStart;

        private Dictionary<Vector2, BaseTile> tilesInGrid;
        private List<BasePlaceable[]> columns; // If needed.
        private List<BasePlaceable[]> rows; // If needed.
        
        private int count;
        private List<Vector2> itemsWillMoveOldPositionList;
        private List<Vector2> itemsWillMoveNewPositionList;

        private void OnEnable()
        {
            DOTween.SetTweensCapacity(500,50); //For 10x10 grid.
        }

        private void Update()
        {
            OperateGrid();
            CombineMatchedLists();
        }

        public void GenerateGrid()
        {
            ClearTiles();
            CheckWantedColors();
            matchedPlaceableItemsList = new List<BasePlaceable>();
            columns = new List<BasePlaceable[]>();
            rows = new List<BasePlaceable[]>();
            tilesInGrid = new Dictionary<Vector2, BaseTile>();
            for (int i = 0; i < width; i++)
            {
                columns.Add(new BasePlaceable[height * 3]);
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
            OnGameStart?.Invoke(width,height);
            GameManager.GameManager.Instance.ChangeState(GameState.CheckForCombos);
        }

        private void CheckWantedColors() //For test use.
        {
            if (!blue && !green && !pink && !purple && !red && !yellow) return;
            if (!blue)
            {
                wantedItemsList.Remove(wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Blue));
            }
            if (!green)
            {
                wantedItemsList.Remove(wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Green));
            }
            if (!pink)
            {
                wantedItemsList.Remove(wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Pink));
            }
            if (!purple)
            {
                wantedItemsList.Remove(wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Purple));
            }
            if (!red)
            {
                wantedItemsList.Remove(wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Red));
            }
            if (!yellow)
            {
                wantedItemsList.Remove(wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Yellow));
            }
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
        
        
        /// <summary>
        /// Destroy any placeable one by one.
        /// </summary>
        /// <param name="position"></param>
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
                    columns[(int)placeablePosition.x][(int)placeablePosition.y] = null;
                    rows[(int)placeablePosition.y][(int)placeablePosition.x] = null;
                    CreateWantedItemsAtPosition(new Vector2((int)position.x,(int)position.y + height * 2));
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
                if (tilesInGrid.ContainsKey(position))
                {
                    tilesInGrid.TryGetValue(position, out var tile);
                    if (tile != null)
                    {
                        var placeablePosition = tile.transform.position;
                        matchedPlaceableItemsList.Remove(tile.occupiedPrefab.GetComponent<BasePlaceable>()); 
                        Destroy(tile.GetComponentInChildren<BasePlaceable>().gameObject);
                        tile.occupiedPrefab = null;
                        columns[(int)placeablePosition.x][(int)placeablePosition.y] = null;
                        rows[(int)placeablePosition.y][(int)placeablePosition.x] = null;
                        CreateWantedItemsAtPosition(new Vector2((int)position.x,(height * 3 ) - (height - (int)position.y)));
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
            columns[(int)position.x][(int)position.y] = spawnedItem;
            rows[(int)position.y][(int)position.x] = spawnedItem;
        }
        

        private void ClearTiles()
        {
            if(tilesInGrid == null || tilesInGrid.Count == 0) return;
            foreach (var tile in tilesInGrid)
            {
                Destroy(tile.Value.gameObject);
            }
        }

        /// <summary>
        /// Checking tiles if the tile that under is empty. If so move.
        /// </summary>
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
                newItemTransform.DOLocalMove(Vector3.zero, 20f * Time.fixedDeltaTime, true);
                newItem.GetComponent<SpriteRenderer>().sortingOrder = (int)newPosition[i].y;
                GetTileAtPosition(newPosition[i]).occupiedPrefab = newItem;
            }
            yield return new WaitForSeconds(22f* Time.fixedDeltaTime);
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    tile.Value.occupiedPrefab.GetComponent<BasicColor>().matchedNeighbourItems.Clear();
                }
            }
            GameManager.GameManager.Instance.ChangeState(GameState.CheckForCombos);
        }

        /// <summary>
        /// Checking all rows from start position and at end position to find matched items.
        /// </summary>
        private void CheckForRows()
        {
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    for (int i = 1; i < width; i++)
                    {
                        if ( tile.Key.x + i >= width) break;
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
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checking all columns from start position and end position to find matched items.
        /// </summary>
        private void CheckForColumns()
        {
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
                {
                    for (int i = 1; i < height; i++)
                    {
                        if (tile.Key.y + i >= height) break;
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
            if (GameManager.GameManager.Instance.gameState != GameState.CheckForCombos) return;
            CheckForColumns();
            CheckForRows();
            var tempList = new List<BasePlaceable>();
            foreach (var tile in tilesInGrid)
            {
                if (tile.Value.occupiedPrefab != null && tile.Key.y < height)
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

            matchedPlaceableItemsList.Clear();
            foreach (var placeableItem in tempList)
            {
                if (!matchedPlaceableItemsList.Contains(placeableItem)) matchedPlaceableItemsList.Add(placeableItem);
            }
            matchedPlaceableItemsList = matchedPlaceableItemsList.Where(i => i != null).OrderBy(t => t.GetComponent<BasicColor>().selectedColor)
                .ThenBy(x => x.transform.position.x).ThenBy(y => y.transform.position.y).ToList();
            CheckForShuffle();
        }

        private void CheckForShuffle()
        {
            if (GameManager.GameManager.Instance.gameState != GameState.CheckForCombos) return;
            if (matchedPlaceableItemsList.Count == 0)
            {
                GameManager.GameManager.Instance.ChangeState(GameState.OperatingGrid);
                var tempList = new List<BasePlaceable>();
                var random = new System.Random();
                foreach (var baseTile in tilesInGrid)
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
                foreach (var baseTile in tilesInGrid)
                {
                    if (baseTile.Key.y >= height) continue;
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
