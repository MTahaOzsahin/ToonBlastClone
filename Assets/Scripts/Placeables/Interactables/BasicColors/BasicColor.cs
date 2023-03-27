using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<BasePlaceable> matchedNeighbourTiles;

        [Header("Sprite List for combos")] 
        [SerializeField] private List<Sprite> combosSpritesList;

        private SpriteRenderer spriteRenderer;
        private List<BasePlaceable> destroyableItemsList;
        
        private void OnEnable()
        {
            destroyableItemsList = new List<BasePlaceable> { this };
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            CheckMatches();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            var thisTileTransform = transform;
            var directionToCollidedTile = thisTileTransform.position - other.gameObject.transform.position;
            var directionToCollidedTileNormalized = directionToCollidedTile.normalized;
            var direction =Vector3.Dot(thisTileTransform.up.normalized, directionToCollidedTileNormalized);
            if (other.gameObject.GetComponent<BasicColor>() != null && other.gameObject.layer != LayerMask.NameToLayer("NotShow"))
            {
                var collidedTile = other.gameObject.GetComponent<BasicColor>();
                if (Mathf.Approximately(direction,1f)) //Up
                {
                    if (collidedTile.selectedColor == selectedColor)
                    {
                        if(!matchedNeighbourTiles.Contains(collidedTile)) matchedNeighbourTiles.Add(collidedTile);
                        if (!GridManager.Instance.matchedPlaceablesList.Contains(collidedTile)) GridManager.Instance.matchedPlaceablesList.Add(collidedTile);
                    }
                }
                else if (Mathf.Approximately(direction,-1f)) //Down
                {
                    if (collidedTile.selectedColor == selectedColor)
                    {
                        if(!matchedNeighbourTiles.Contains(collidedTile)) matchedNeighbourTiles.Add(collidedTile);
                        if (!GridManager.Instance.matchedPlaceablesList.Contains(collidedTile)) GridManager.Instance.matchedPlaceablesList.Add(collidedTile);
                    }
                }
                else if (Mathf.Approximately(direction,0f)) //Right or Left
                {
                    if (collidedTile.selectedColor == selectedColor)
                    {
                        if(!matchedNeighbourTiles.Contains(collidedTile)) matchedNeighbourTiles.Add(collidedTile);
                        if (!GridManager.Instance.matchedPlaceablesList.Contains(collidedTile)) GridManager.Instance.matchedPlaceablesList.Add(collidedTile);
                    }
                }

                GridManager.Instance.matchedPlaceablesList = GridManager.Instance.matchedPlaceablesList.Where(p => p != null).OrderBy(t => t.GetComponent<BasicColor>().selectedColor).
                    ThenBy(x => x.transform.position.x).ThenBy(y => y.transform.position.y).ToList();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var thisTileTransform = transform;
            var directionToCollidedTile = thisTileTransform.position - other.gameObject.transform.position;
            var directionToCollidedTileNormalized = directionToCollidedTile.normalized;
            var direction =Vector3.Dot(thisTileTransform.up.normalized, directionToCollidedTileNormalized);
            if (other.gameObject.GetComponent<BasicColor>() != null)
            {
                var collidedTile = other.gameObject.GetComponent<BasicColor>();
                if (Mathf.Approximately(direction,1f)) //Up
                {
                    if (collidedTile.selectedColor == selectedColor)
                    {
                        if(matchedNeighbourTiles.Contains(collidedTile)) matchedNeighbourTiles.Remove(collidedTile);
                        if (GridManager.Instance.matchedPlaceablesList.Contains(collidedTile)) GridManager.Instance.matchedPlaceablesList.Remove(collidedTile);
                    }
                }
                else if (Mathf.Approximately(direction,-1f)) //Down
                {
                    if (collidedTile.selectedColor == selectedColor)
                    {
                        if(matchedNeighbourTiles.Contains(collidedTile)) matchedNeighbourTiles.Remove(collidedTile);
                        if (GridManager.Instance.matchedPlaceablesList.Contains(collidedTile)) GridManager.Instance.matchedPlaceablesList.Remove(collidedTile);
                    }
                }
                else if (Mathf.Approximately(direction,0f)) //Right or Left
                {
                    if (collidedTile.selectedColor == selectedColor)
                    {
                        if(matchedNeighbourTiles.Contains(collidedTile)) matchedNeighbourTiles.Remove(collidedTile);
                        if (GridManager.Instance.matchedPlaceablesList.Contains(collidedTile)) GridManager.Instance.matchedPlaceablesList.Remove(collidedTile);
                    }
                }

                GridManager.Instance.matchedPlaceablesList = GridManager.Instance.matchedPlaceablesList.Where(p => p != null).OrderBy(t => t.GetComponent<BasicColor>().selectedColor).
                    ThenBy(x => x.transform.position.x).ThenBy(y => y.transform.position.y).ToList();
            }
        }

        private void OnMouseDown()
        {
            foreach (var placeableToDestroy in destroyableItemsList)
            {
                GridManager.Instance.DestroyPlaceable(placeableToDestroy.transform.position);
                if (matchedNeighbourTiles.Contains(placeableToDestroy))
                    matchedNeighbourTiles.Remove(placeableToDestroy);
            }
        }

        private void CheckMatches()
        {
            if (GridManager.Instance.matchedPlaceablesList.Contains(this))
            { 
                foreach (var tile in matchedNeighbourTiles)
                {
                    if (!destroyableItemsList.Contains(tile)) destroyableItemsList.Add(tile);
                }
            
                for (int i = 0; i < destroyableItemsList.Count; i++)
                {
                    foreach (var newTileMatchedNeighbourTile in destroyableItemsList[i].GetComponent<BasicColor>().matchedNeighbourTiles)
                    {
                        if (!destroyableItemsList.Contains(newTileMatchedNeighbourTile)) destroyableItemsList.Add(newTileMatchedNeighbourTile);
                    }
                }
            }

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
