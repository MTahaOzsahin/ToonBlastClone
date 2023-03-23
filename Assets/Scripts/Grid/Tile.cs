using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grid
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
    public class Tile : MonoBehaviour
    {
        [Header("Tile Colors")] 
        [SerializeField] private GameObject blueTile;
        [SerializeField] private GameObject greenTile;
        [SerializeField] private GameObject pinkTile;
        [SerializeField] private GameObject purpleTile;
        [SerializeField] private GameObject redTile;
        [SerializeField] private GameObject yellowTile;

        [Header("Selected Color")]
        public SelectedColor selectedColor = SelectedColor.None;

        public void Init(SelectedColor color)
        {
            switch (color)
            {
                case SelectedColor.Blue:
                    blueTile.SetActive(true);
                    break;
                case SelectedColor.Green:
                    greenTile.SetActive(true);
                    break;
                case SelectedColor.Pink:
                    pinkTile.SetActive(true);
                    break;
                case SelectedColor.Purple:
                    purpleTile.SetActive(true);
                    break;
                case SelectedColor.Red:
                    redTile.SetActive(true);
                    break;
                case SelectedColor.Yellow:
                    yellowTile.SetActive(true);
                    break;
                case SelectedColor.None:
                break;
            }
        }

        // public void Init()
        // {
        //     var random = Random.Range(0, 5);
        //     switch (random)
        //     {
        //         case 0:
        //             blueTile.SetActive(true);
        //             break;
        //         case 1:
        //             greenTile.SetActive(true);
        //             break;
        //         case 2:
        //             pinkTile.SetActive(true);
        //             break;
        //         case 3:
        //             purpleTile.SetActive(true);
        //             break;
        //         case 4:
        //             redTile.SetActive(true);
        //             break;
        //         case 5:
        //             yellowTile.SetActive(true);
        //             break;
        //     }
        // }
    }
}
