using System;
using Grid;
using UnityEngine;

namespace GameManager
{
    public class GameManager : SingletonMB<GameManager>
    {
        public GameState gameState;

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
                case GameState.CheckForCombos:
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
        CheckForCombos
    }
}
