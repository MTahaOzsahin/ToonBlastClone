using System.Collections.Generic;
using GameManager;
using Placeables;
using Placeables.Interactables.BasicColors;
using Tile;
using UnityEngine;

namespace Grid
{
    public class GridCreator : SingletonMB<GridCreator>
    {
        [Header("Wanted Colors. If none selected will be all colors"),Tooltip("For test only")]
        [SerializeField] private bool blue;
        [SerializeField] private bool green;
        [SerializeField] private bool pink;
        [SerializeField] private bool purple;
        [SerializeField] private bool red;
        [SerializeField] private bool yellow;
        
        public System.Action<int, int> onGameStart;
        
        public void GenerateGrid()
        {
            ClearTiles();
            CheckWantedColors();
            GridManager.Instance.matchedPlaceableItemsList = new List<BasePlaceable>();
            GridManager.Instance.tilesInGrid = new Dictionary<Vector2, BaseTile>();
            for (int x = 0; x < GridManager.Instance.gridWidth; x++)
            {
                for (int y = 0; y < GridManager.Instance.gridHeight; y++)
                {
                    var spawnedTile = Instantiate(GridManager.Instance.tilePrefab, new Vector3(x, y), Quaternion.identity,transform);
                    spawnedTile.name = $"Tile {x} {y}";
                    spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
                    GridManager.Instance.tilesInGrid[new Vector2(x, y)] = spawnedTile;
                }
            }
            GridOperator.Instance.CreateWantedItems();
            onGameStart?.Invoke(GridManager.Instance.gridWidth,GridManager.Instance.gridHeight);
            GameManager.GameManager.Instance.ChangeState(GameState.CheckForCombos);
        }
        
        private void CheckWantedColors() //For test use.
        {
            if (!blue && !green && !pink && !purple && !red && !yellow) return;
            if (!blue)
            {
                GridManager.Instance.wantedItemsList.Remove(GridManager.Instance.wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Blue));
            }
            if (!green)
            {
                GridManager.Instance.wantedItemsList.Remove(GridManager.Instance.wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Green));
            }
            if (!pink)
            {
                GridManager.Instance.wantedItemsList.Remove(GridManager.Instance.wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Pink));
            }
            if (!purple)
            {
                GridManager.Instance.wantedItemsList.Remove(GridManager.Instance.wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Purple));
            }
            if (!red)
            {
                GridManager.Instance.wantedItemsList.Remove(GridManager.Instance.wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Red));
            }
            if (!yellow)
            {
                GridManager.Instance.wantedItemsList.Remove(GridManager.Instance.wantedItemsList.Find(x => x.GetComponent<BasicColor>().selectedColor == SelectedColor.Yellow));
            }
        }
        
        private void ClearTiles()
        {
            if (GridManager.Instance.tilesInGrid == null || GridManager.Instance.tilesInGrid.Count == 0) return;
            foreach (var tile in GridManager.Instance.tilesInGrid)
            {
                Destroy(tile.Value.gameObject);
            }
        }
    }
}
