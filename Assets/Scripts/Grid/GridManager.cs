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
        [SerializeField] private Transform gameCamera;

        private Dictionary<Vector2, Tile> _tiles;
        private List<SelectedColor> _selectedTileColor;
        private int _wantedColorNumber;
        public void Start()
        {
            CenterCamera();
            GenerateGrid(true);
        }

        private void GenerateGrid(bool isSpecificColor)
        {
            ClearTiles();
            GetWantedColor();
            _tiles = new Dictionary<Vector2, Tile>();

            if (isSpecificColor)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity,transform);
                        var random = Random.Range(0, _selectedTileColor.Count);
                        spawnedTile.Init(_selectedTileColor[random]);
                        spawnedTile.name = $"Tile {x} {y}";
                        spawnedTile.GetComponentInChildren<SpriteRenderer>().sortingOrder = y;
                        _tiles[new Vector2(x, y)] = spawnedTile;
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
                        _tiles[new Vector2(x, y)] = spawnedTile;
                    }
                }
            }
        }

        public Tile GetTileAtPosition(Vector2 position)
        {
            if (_tiles.TryGetValue(position, out var tile))
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
        }

        private void GetColorVariant()
        {
            if (isWantedBlue) _wantedColorNumber++;
            if (isWantedGreen) _wantedColorNumber++;
            if (isWantedPink) _wantedColorNumber++;
            if (isWantedPurple) _wantedColorNumber++;
            if (isWantedRed) _wantedColorNumber++;
            if (isWantedYellow) _wantedColorNumber++;
            if (_wantedColorNumber == 0) _wantedColorNumber = 6;
        }

        private void GetWantedColor()
        {
            _selectedTileColor = new List<SelectedColor>();
            var blueColor = SelectedColor.Blue;
            var greenColor = SelectedColor.Green;
            var pinkColor = SelectedColor.Pink;
            var purpleColor = SelectedColor.Purple;
            var redColor = SelectedColor.Red;
            var yellowColor = SelectedColor.Yellow;
            
            if (isWantedBlue)
            {
                _selectedTileColor.Add(blueColor);
            }
            if (isWantedGreen)
            {
                _selectedTileColor.Add(greenColor);
            }
            if (isWantedPink)
            {
                _selectedTileColor.Add(pinkColor);
            }
            if (isWantedPurple)
            {
                _selectedTileColor.Add(purpleColor);
            }
            if (isWantedRed)
            {
                _selectedTileColor.Add(redColor);
            }
            if (isWantedYellow)
            {
                _selectedTileColor.Add(yellowColor);
            }
            if (_selectedTileColor.Count == 0)
            {
                _selectedTileColor.Add(blueColor);
                _selectedTileColor.Add(greenColor);
                _selectedTileColor.Add(pinkColor);
                _selectedTileColor.Add(purpleColor);
                _selectedTileColor.Add(redColor);
                _selectedTileColor.Add(yellowColor);
            }
        }

        private void ClearTiles()
        {
            if(_tiles == null || _tiles.Count == 0) return;
            foreach (var tile in _tiles)
            {
                Destroy(tile.Value.gameObject);
            }
        }
    }
}
