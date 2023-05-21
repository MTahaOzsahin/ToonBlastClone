using Grid;
using UnityEngine;

namespace Controllers
{
    [DefaultExecutionOrder(-10)]
    public class CameraController : MonoBehaviour
    {
        [Header("Game Camera")] 
        [SerializeField] private Camera gameCamera;

        private void OnEnable()
        {
            GridCreator.Instance.onGameStart += CenterCamera;
        }

        private void OnDisable()
        {
            if (GridCreator.Instance == null) return; 
            GridCreator.Instance.onGameStart -= CenterCamera;
        }

        private void CenterCamera(int width,int height)
        {
            gameCamera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 + 0.5f, -10f);
            gameCamera.orthographicSize = width switch
            {
                3 => 4,
                4 => 4.5f,
                5 => 5.5f,
                6 => 6f,
                7 => 7,
                8 => 8,
                9 => 9,
                10 => 10,
                _ => 4
            };
            GridOperator.Instance.cameraTopBound = (height / 2f) + 0.5f +gameCamera.orthographicSize;
        }
    }
}
