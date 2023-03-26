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

        private void OnTriggerStay2D(Collider2D other)
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
            if (GridManager.Instance.matchedPlaceablesList.Contains(this))
            {
                var tempList = new List<BasePlaceable> { this };
                foreach (var tile in matchedNeighbourTiles)
                {
                    if (!tempList.Contains(tile)) tempList.Add(tile);
                }
            
                for (int i = 0; i < tempList.Count; i++)
                {
                    foreach (var newTileMatchedNeighbourTile in tempList[i].GetComponent<BasicColor>().matchedNeighbourTiles)
                    {
                        if (!tempList.Contains(newTileMatchedNeighbourTile)) tempList.Add(newTileMatchedNeighbourTile);
                    }
                }
            
                foreach (var placeableToDestroy in tempList)
                {
                    GridManager.Instance.DestroyPlaceable(placeableToDestroy.transform.position);
                    if (matchedNeighbourTiles.Contains(placeableToDestroy))
                        matchedNeighbourTiles.Remove(placeableToDestroy);
                }
            }
        }
    }
}
