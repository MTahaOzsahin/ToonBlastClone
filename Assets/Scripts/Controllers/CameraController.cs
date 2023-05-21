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
