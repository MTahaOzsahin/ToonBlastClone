using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;

namespace Tile
{
   
    public class BaseTile : MonoBehaviour
    {
        [Header("Neighbour Tile List")] 
        public List<BaseTile> neighbourTiles;
        public List<BaseTile> matchedNeighbourTiles;
    }
}
