using Placeables;
using UnityEngine;

namespace Tile
{
   
    public class BaseTile : MonoBehaviour
    {
        [Header("Occupied prefab")]
        public BasePlaceable occupiedPrefab;

        private void Update()
        {
            ClapPosition();
        }

        private void ClapPosition()
        {
            var originPosition = transform.position;
            var roundedX = Mathf.Round(originPosition.x);
            var roundedY = Mathf.Round(originPosition.y);
            transform.position = new Vector2(roundedX, roundedY);
        }
        
    }
}
