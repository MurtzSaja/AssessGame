using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AssessGame
{
    internal class SideMenuController : MonoBehaviour
    {
        private static SideMenuController instance;

        [SerializeField]
        private GameObject sidebar, cardGrid;
        [SerializeField]
        private TextMeshProUGUI currentScoreTxt, currentTriesTxt;
        [SerializeField]
        private Button homeBtn, nextGameBtn;
        internal static SideMenuController Instance { get => instance;private set => instance = value; }

        internal void Init(Action onHomeBtnClick, Action onNextGame, ScoreManager scoreManager)
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            homeBtn.onClick.AddListener(()=> { onHomeBtnClick(); });
            nextGameBtn.onClick.AddListener(() => { onNextGame(); });
            scoreManager.onCurrentScoreChange += ( (currentScore) => 
            {
                currentScoreTxt.text = currentScore.ToString();
            });
            scoreManager.onTriesChange += ((currenTries) =>
            {
                currentTriesTxt.text = currenTries.ToString();
            });
        }

        internal void ShowGameOver()
        {
            sidebar.SetActive(false);
            cardGrid.SetActive(false);
            nextGameBtn.gameObject.SetActive(true);
        }

        internal void StartGame()
        {
            sidebar.SetActive(true);
            cardGrid.SetActive(true);
            nextGameBtn.gameObject.SetActive(false);
        }

        internal void ResetControls()
        {
           currentTriesTxt.text = currentScoreTxt.text = "0";
        }

    }
}