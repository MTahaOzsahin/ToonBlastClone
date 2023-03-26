using System.Collections.Generic;
using Tile.Interactables.BasicColors;
using UnityEngine;

namespace Tile
{
   
    public class BaseTile : MonoBehaviour
    {
        [Header("Neighbour Tile List")] 
        public List<BaseTile> neighbourTiles;

        [Header("Occupied prefab")]
        public BaseTile occupiedPrefab;
        

        public BaseTile()
        {
            neighbourTiles = new List<BaseTile>();
        }
        public virtual void Init(){}

        private void OnTriggerEnter2D(Collider2D colliderInfo)
        {
            var thisTileTransform = transform;
            var directionToCollidedTile = thisTileTransform.position - colliderInfo.gameObject.transform.position;
            var directionToCollidedTileNormalized = directionToCollidedTile.normalized;
            var direction =Vector3.Dot(thisTileTransform.up.normalized, directionToCollidedTileNormalized);
            var collidedTile = colliderInfo.GetComponent<BaseTile>();
            if(collidedTile.GetComponent<BasicColor>()) return;
            if (Mathf.Approximately(direction,1f)) //Up
            {
                neighbourTiles.Add(collidedTile);
            }
            else if (Mathf.Approximately(direction,-1f)) //Down
            {
                neighbourTiles.Add(collidedTile);
            }
            else if (Mathf.Approximately(direction,0f)) //Right or Left
            {
                
                neighbourTiles.Add(collidedTile);
            }
        }
    }
}
