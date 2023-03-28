using System.Collections.Generic;
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
        
        private void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            matchedNeighbourItems = new List<BasicColor>();
        }

        private void Update()
        {
            CheckSprite();
        }

        private void OnMouseDown()
        {
            if (GameManager.GameManager.Instance.gameState != GameState.CheckForCombos) return;
            if (GridManager.Instance.matchedPlaceableItemsList.Contains(this))
            {
                foreach (var placeableToDestroy in matchedNeighbourItems)
                {
                    if (GridManager.Instance.matchedPlaceableItemsList.Contains(placeableToDestroy))
                        GridManager.Instance.matchedPlaceableItemsList.Remove(placeableToDestroy);
                }
                GridManager.Instance.DestroyPlaceable(matchedNeighbourItems);
            }
            matchedNeighbourItems.Clear();
        }

        private void CheckSprite()
        {
            switch (matchedNeighbourItems.Count)
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
                case >= 10:
                    spriteRenderer.sprite = combosSpritesList[3];
                    break;
            }
        }
    }
}
