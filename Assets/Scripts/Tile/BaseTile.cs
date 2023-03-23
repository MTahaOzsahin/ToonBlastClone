using UnityEngine;

namespace Tile
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
    public class BaseTile : MonoBehaviour
    {
        [Header("Tile Colors")] 
        [SerializeField] private GameObject blueTile;
        [SerializeField] private GameObject greenTile;
        [SerializeField] private GameObject pinkTile;
        [SerializeField] private GameObject purpleTile;
        [SerializeField] private GameObject redTile;
        [SerializeField] private GameObject yellowTile;

        [Header("Selected Color")]
        [SerializeField] private SelectedColor selectedColor = SelectedColor.None;
        

        public void Init(SelectedColor color)
        {
            switch (color)
            {
                case SelectedColor.Blue:
                    selectedColor = SelectedColor.Blue;
                    blueTile.SetActive(true);
                    break;
                case SelectedColor.Green:
                    selectedColor = SelectedColor.Green;
                    greenTile.SetActive(true);
                    break;
                case SelectedColor.Pink:
                    selectedColor = SelectedColor.Pink;
                    pinkTile.SetActive(true);
                    break;
                case SelectedColor.Purple:
                    selectedColor = SelectedColor.Purple;
                    purpleTile.SetActive(true);
                    break;
                case SelectedColor.Red:
                    selectedColor = SelectedColor.Red;
                    redTile.SetActive(true);
                    break;
                case SelectedColor.Yellow:
                    selectedColor = SelectedColor.Yellow;
                    yellowTile.SetActive(true);
                    break;
                case SelectedColor.None:
                    break;
            }
        }
    }
}
