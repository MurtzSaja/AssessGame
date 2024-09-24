using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AssessGame
{
    internal class MainGameController : MonoBehaviour
    {
        [SerializeField]
        private GridController gridController;
        [SerializeField]
        private MatchingController matchingController;
        [SerializeField]
        private GameObject cardPrefab;
        private int matchCombinationCount = 0;
        internal async void InitGame(DifficultyGrid selectedDifficultyGrid, List<Sprite> frontFaceSprites, Sprite back)
        {
            matchingController.Init(selectedDifficultyGrid.toMatchCount, OnMatchSuccess);
            Vector2Int selectedGridSize = await gridController.Init(cardPrefab, selectedDifficultyGrid, frontFaceSprites, matchingController.CheckMatch);
            matchCombinationCount = (selectedGridSize.x * selectedGridSize.y) / selectedDifficultyGrid.toMatchCount;
            SideMenuController.Instance.StartGame();
            gameObject.SetActive(true);
            await Task.Delay((int)Time.deltaTime * 1000);
            gridController.ShowHint();
        }
        internal void ResetGameBoard()
        {
            SideMenuController.Instance.ResetControls();
            gridController.ResetGrid();
            ScoreManager.Instance.ResetCurrentStats();
        }

        private void OnMatchSuccess()
        {
            matchCombinationCount--;
            if (matchCombinationCount == 0)
            {
                AudioManager.Instance.PlaySFXOnShot(AudioManager.AudioType.GameOver);
                SideMenuController.Instance.ShowGameOver();
                ResetGameBoard();
            }
        }
    }
}