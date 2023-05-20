using System;
using Grid;

namespace GameManager
{
    public class GameManager : SingletonMB<GameManager>
    {
        public GameState gameState;
        public Action checkForCombos;

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
                    GridManager.Instance.GenerateGrid();
                    break;
                case GameState.OperatingGrid:
                    break;
                case GameState.WaitForInput:
                    break;
                case GameState.CreateBonusItem:
                    break;
                case GameState.CheckForCombos:
                    checkForCombos?.Invoke();
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
        WaitForInput,
        CreateBonusItem,
        CheckForCombos
    }
}
