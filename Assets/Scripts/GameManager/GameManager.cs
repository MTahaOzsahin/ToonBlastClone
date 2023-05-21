using System;
using Grid;

namespace GameManager
{
    public class GameManager : SingletonMB<GameManager>
    {
        public GameState gameState;
        public Action getGridData;

        private void Start()
        {
            ChangeState(GameState.GenerateGrid);
        }

        public void ChangeState(GameState newState)
        {
            gameState = newState;
            switch (newState)
            {
                case GameState.GenerateGrid:
                    GridCreator.Instance.GenerateGrid();
                    break;
                case GameState.GetGridData:
                    getGridData?.Invoke();
                    break;
                case GameState.OperatingGrid:
                    break;
                case GameState.WaitForInput:
                    break;
                case GameState.CheckForCombos:
                    getGridData?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
    }

    public enum GameState
    {
        GenerateGrid,
        OperatingGrid,
        GetGridData,
        WaitForInput,
        CheckForCombos
    }
}
