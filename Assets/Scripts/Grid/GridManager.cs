using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Width and Height")]
        [SerializeField, Range(2, 10)] private int width;
        [SerializeField, Range(2, 10)] private int height;

        [Header("Tile Prefab")] 
        [SerializeField] private Tile tilePrefab;

        [Header("Wanted Colors"),Tooltip("If none selected will be all 6 colors")] 
        [SerializeField] private bool isWantedBlue;
        [SerializeField] private bool isWantedGreen;
        [SerializeField] private bool isWantedPink;
        [SerializeField] private bool isWantedPurple;
        [SerializeField] private bool isWantedRed;
        [SerializeField] private bool isWantedYellow;
        
        [Header("Game Camera")] 
        [SerializeField] private Camera gameCamera;

        private Dictionary<Vector2, Tile> tiles;
        private List<SelectedColor> selectedTileColor;
        private int wantedColorNumber;
        public void Start()
        {
            CenterCamera();
            GenerateGrid(true);
        }

        private void GenerateGrid(bool isSpecificColor)
        {
            ClearTiles();
            GetWantedColor();
            tiles = new Dictionary<Vector2, Tile>();

            if (isSpecificColor)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity,transform);
                        var random = Random.Range(0, selectedTileColor.Count);
                        spawnedTile.Init(selectedTileColor[random]);
                        spawnedTile.name = $"Tile {x} {y}";
                        spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
                        tiles[new Vector2(x, y)] = spawnedTile;
                    }
                }
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity,transform);
                        spawnedTile.name = $"Tile {x} {y}";
                        spawnedTile.GetComponent<SpriteRenderer>().sortingOrder = y;
                        tiles[new Vector2(x, y)] = spawnedTile;
                    }
                }
            }
        }

        public Tile GetTileAtPosition(Vector2 position)
        {
            if (tiles.TryGetValue(position, out var tile))
            {
                return tile;
            }
            else
            {
                return null;
            }
        }

        private void CenterCamera()
        {
            gameCamera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10f);
            gameCamera.fieldOfView = width switch
            {
                7 => 68,
                8 => 76,
                9 => 84,
                10 => 92,
                _ => 60
            };
        }

        private void GetWantedColor()
        {
            selectedTileColor = new List<SelectedColor>();
            var blueColor = SelectedColor.Blue;
            var greenColor = SelectedColor.Green;
            var pinkColor = SelectedColor.Pink;
            var purpleColor = SelectedColor.Purple;
            var redColor = SelectedColor.Red;
            var yellowColor = SelectedColor.Yellow;
            
            if (isWantedBlue)
            {
                selectedTileColor.Add(blueColor);
            }
            if (isWantedGreen)
            {
                selectedTileColor.Add(greenColor);
            }
            if (isWantedPink)
            {
                selectedTileColor.Add(pinkColor);
            }
            if (isWantedPurple)
            {
                selectedTileColor.Add(purpleColor);
            }
            if (isWantedRed)
            {
                selectedTileColor.Add(redColor);
            }
            if (isWantedYellow)
            {
                selectedTileColor.Add(yellowColor);
            }
            if (selectedTileColor.Count == 0)
            {
                selectedTileColor.Add(blueColor);
                selectedTileColor.Add(greenColor);
                selectedTileColor.Add(pinkColor);
                selectedTileColor.Add(purpleColor);
                selectedTileColor.Add(redColor);
                selectedTileColor.Add(yellowColor);
            }
        }

        private void ClearTiles()
        {
            if(tiles == null || tiles.Count == 0) return;
            foreach (var tile in tiles)
            {
                Destroy(tile.Value.gameObject);
            }
        }
    }
}
