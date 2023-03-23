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
                case GameState.CheckForMatch:
                    break;
                case GameState.CollapseMatch:
                    break;
                case GameState.OperateNewGrid:
                    break;
                case GameState.Wait:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
    }

    public enum GameState
    {
        GenerateGrid,
        CheckForMatch,
        CollapseMatch,
        OperateNewGrid,
        Wait
    }
}
