using System.Collections.Generic;
using DG.Tweening;
using Placeables;
using Tile;
using UnityEngine;

namespace Grid
{
    public class GridManager : SingletonMB<GridManager>
    {
        [Header("Grid Width and Height"),Tooltip("Please do not change during playMode!")]
        [Range(2, 10)] public int gridWidth;
        [Range(2, 10)] public int gridHeight;

        [Header("Tile Prefab")] 
        public BaseTile tilePrefab;

        [Header("Wanted Items")] 
        public List<BasePlaceable> wantedItemsList;

        [Header("Matched Tiles")]
        public List<BasePlaceable> matchedPlaceableItemsList;
        
        public Dictionary<Vector2, BaseTile> tilesInGrid;

        private void OnEnable()
        {
            DOTween.SetTweensCapacity(500,50); //For 10x10 grid.
        }
    }
}
