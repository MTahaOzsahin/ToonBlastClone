using System;
using System.Collections.Generic;
using Placeables;
using Placeables.Interactables.BasicColors;
using UnityEngine;

namespace Tile
{
   
    public class BaseTile : MonoBehaviour
    {
        [Header("Neighbour Tile List")] 
        public List<BaseTile> neighbourTiles;

        [Header("Occupied prefab")]
        public BasePlaceable occupiedPrefab;

        public BaseTile()
        {
            neighbourTiles = new List<BaseTile>();
        }

        private void OnTriggerEnter2D(Collider2D colliderInfo)
        {
            var thisTileTransform = transform;
            var directionToCollidedTile = thisTileTransform.position - colliderInfo.gameObject.transform.position;
            var directionToCollidedTileNormalized = directionToCollidedTile.normalized;
            var direction =Vector3.Dot(thisTileTransform.up.normalized, directionToCollidedTileNormalized);
            if (colliderInfo.GetComponent<BaseTile>() != null)
            {
                var collidedTile = colliderInfo.GetComponent<BaseTile>();
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
}
