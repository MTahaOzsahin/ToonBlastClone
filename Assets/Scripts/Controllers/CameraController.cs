using System;
using Grid;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        [Header("Game Camera")] 
        [SerializeField] private Camera gameCamera;

        private void OnEnable()
        {
            GridManager.Instance.OnGameStart += CenterCamera;
        }

        private void OnDisable()
        {
            GridManager.Instance.OnGameStart -= CenterCamera;
        }

        private void CenterCamera(int width,int height)
        {
            gameCamera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 + 1.5f, -10f);
            gameCamera.fieldOfView = width switch
            {
                7 => 68,
                8 => 76,
                9 => 84,
                10 => 92,
                _ => 60
            };
        }
    }
}
