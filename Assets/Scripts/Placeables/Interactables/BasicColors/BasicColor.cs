using System.Collections.Generic;
using System.Linq;
using GameManager;
using Grid;
using UnityEngine;

namespace Placeables.Interactables.BasicColors
{
    public enum SelectedColor
    {
        Blue,
        Green,
        Pink,
        Purple,
        Red,
        Yellow,
        None
    }
    public class BasicColor : Interactable
    {
        //This for basic colors.
        [Header("Selected Color")]
        public SelectedColor selectedColor;
        
        [Header("Matched Neighbour Tile List")]
        public List<BasicColor> matchedNeighbourItems;

        [Header("Sprite List for combos")] 
        [SerializeField] private List<Sprite> combosSpritesList;

        private SpriteRenderer spriteRenderer;
        public List<BasicColor> destroyableItemsList;
        
        private void OnEnable()
        {
            destroyableItemsList = new List<BasicColor> { this };
            spriteRenderer = GetComponent<SpriteRenderer>();
            matchedNeighbourItems = new List<BasicColor>();
        }

        private void Update()
        {
            CheckMatches();
        }

        private void OnMouseDown()
        {
            if (GameManager.GameManager.Instance.gameState != GameState.CheckForCombos) return;
            if (GridManager.Instance.matchedPlaceableItemsList.Contains(this))
            {
                GameManager.GameManager.Instance.ChangeState(GameState.OperatingGrid);
                foreach (var placeableToDestroy in destroyableItemsList)
                {
                    if (matchedNeighbourItems.Contains(placeableToDestroy))
                        matchedNeighbourItems.Remove(placeableToDestroy);
                    if (GridManager.Instance.matchedPlaceableItemsList.Contains(placeableToDestroy))
                        GridManager.Instance.matchedPlaceableItemsList.Remove(placeableToDestroy);
                }
                GridManager.Instance.DestroyPlaceable(destroyableItemsList);
            }
            destroyableItemsList.Clear();
        }

        private void CheckMatches()
        {
            if (GameManager.GameManager.Instance.gameState != GameState.CheckForCombos) return;
            if (GridManager.Instance.matchedPlaceableItemsList.Contains(this))
            {
                if (matchedNeighbourItems.Count == 0) return;
                foreach (var basicColor in matchedNeighbourItems)
                {
                    if (!destroyableItemsList.Contains(basicColor)) destroyableItemsList.Add(basicColor);
                    if (basicColor.matchedNeighbourItems.Count == 0) return;
                    foreach (var basicColor2 in basicColor.matchedNeighbourItems)
                    {
                        if (!destroyableItemsList.Contains(basicColor2)) destroyableItemsList.Add(basicColor2);
                        if (basicColor2.matchedNeighbourItems.Count == 0) return;
                        foreach (var basicColor3 in basicColor2.matchedNeighbourItems)
                        {
                            if (!destroyableItemsList.Contains(basicColor3)) destroyableItemsList.Add(basicColor3);
                        }
                    }
                }

                if (matchedNeighbourItems.Count == 0)
                {
                    destroyableItemsList.Clear();
                }
            }
            CheckSprite();
        }

        private void CheckSprite()
        {
            switch (destroyableItemsList.Count)
            {
                case < 5:
                    spriteRenderer.sprite = combosSpritesList[0];
                    break;
                case >= 5 and < 8:
                    spriteRenderer.sprite = combosSpritesList[1];
                    break;
                case >= 8 and < 10:
                    spriteRenderer.sprite = combosSpritesList[2];
                    break;
                case > 10:
                    spriteRenderer.sprite = combosSpritesList[3];
                    break;
            }
        }
    }
}
