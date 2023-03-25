using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;

namespace Tile.Interactables.BasicColors
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
        public SelectedColor selectedColor = SelectedColor.None;
        
        public void Init(SelectedColor color)
        {
            neighbourTiles = new List<BaseTile>();
            matchedNeighbourTiles = new List<BaseTile>();
            switch (color)
            {
                case SelectedColor.Blue:
                    selectedColor = SelectedColor.Blue;
                    break;
                case SelectedColor.Green:
                    selectedColor = SelectedColor.Green;
                    break;
                case SelectedColor.Pink:
                    selectedColor = SelectedColor.Pink;
                    break;
                case SelectedColor.Purple:
                    selectedColor = SelectedColor.Purple;
                    break;
                case SelectedColor.Red:
                    selectedColor = SelectedColor.Red;
                    break;
                case SelectedColor.Yellow:
                    selectedColor = SelectedColor.Yellow;
                    break;
                case SelectedColor.None:
                    break;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D colliderInfo)
        {
            var thisTileTransform = transform;
            var directionToCollidedTile = thisTileTransform.position - colliderInfo.gameObject.transform.position;
            var directionToCollidedTileNormalized = directionToCollidedTile.normalized;
            var direction =Vector3.Dot(thisTileTransform.up.normalized, directionToCollidedTileNormalized);
            var collidedTile = colliderInfo.GetComponent<BasicColor>();
            if (Mathf.Approximately(direction,1f)) //Up
            {
                neighbourTiles.Add(collidedTile);
                if (collidedTile.selectedColor == selectedColor)
                {
                    matchedNeighbourTiles.Add(collidedTile);
                    if (!GridManager.Instance.matchedTileList.Contains(collidedTile)) GridManager.Instance.matchedTileList.Add(collidedTile);
                }
            }
            else if (Mathf.Approximately(direction,-1f)) //Down
            {
                neighbourTiles.Add(collidedTile);
                if (collidedTile.selectedColor == selectedColor)
                {
                    matchedNeighbourTiles.Add(colliderInfo.GetComponent<BasicColor>());
                    if (!GridManager.Instance.matchedTileList.Contains(collidedTile)) GridManager.Instance.matchedTileList.Add(collidedTile);
                }
            }
            else if (Mathf.Approximately(direction,0f)) //Right or Left
            {
                
                neighbourTiles.Add(collidedTile);
                if (collidedTile.selectedColor == selectedColor)
                {
                    matchedNeighbourTiles.Add(collidedTile);
                    if (!GridManager.Instance.matchedTileList.Contains(collidedTile)) GridManager.Instance.matchedTileList.Add(collidedTile);
                }
            }

            GridManager.Instance.matchedTileList = GridManager.Instance.matchedTileList.OrderBy(t => t.GetComponent<BasicColor>()).
                ThenBy(x => x.transform.position.x).ThenBy(y => y.transform.position.y).ToList();
        }
        
        private void OnMouseDown()
        {
            if (GridManager.Instance.matchedTileList.Contains(this))
            {
                var tempList = new List<BaseTile>();
                tempList.Add(this);
                foreach (var tile in matchedNeighbourTiles)
                {
                    if (!tempList.Contains(tile)) tempList.Add(tile);
                }

                for (int i = 0; i < tempList.Count; i++)
                {
                    foreach (var newTileMatchedNeighbourTile in tempList[i].matchedNeighbourTiles)
                    {
                        if (!tempList.Contains(newTileMatchedNeighbourTile)) tempList.Add(newTileMatchedNeighbourTile);
                    }
                }

                foreach (var tileToDestroy in tempList)
                {
                    if (tileToDestroy.gameObject != null)
                    {
                        GridManager.Instance.matchedTileList.Remove(tileToDestroy);
                        GridManager.Instance.DestroyTiles(tileToDestroy.transform.position);
                        Destroy(tileToDestroy.gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("No");
            }
        }
    }
}
