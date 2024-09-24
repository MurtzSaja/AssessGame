using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssessGame
{
    internal delegate void OnScoreChange(int newScore);

    internal class ScoreManager : MonoBehaviour
    {
        internal OnScoreChange onHighScoreChange, onCurrentScoreChange, onTriesChange;
        internal static ScoreManager Instance { get => instance; private set => instance = value; }

        private static ScoreManager instance;
        private int currentScore, currentTries;
        private int contineousMatchCount = 0;

        private int awardScore, awardComboScore, matchCountForCombo;

        internal void Init(int awardScore, int awardComboScore, int matchCountForCombo)
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            this.awardScore = awardScore;
            this.awardComboScore = awardComboScore;
            this.matchCountForCombo = matchCountForCombo;
        }
        internal void AddCurrentScore()
        {
            SetCurrentTries();
            if (contineousMatchCount < matchCountForCombo)
            {
                currentScore += awardScore;
            }
            else
            {
                currentScore += awardComboScore;
            }
            contineousMatchCount++;
            onCurrentScoreChange?.Invoke(currentScore);
            SetHighScore(currentScore);
        }

        internal void SetCurrentTries(bool isMismatch = false)
        {
            currentTries++;
            if (isMismatch)
            {
                contineousMatchCount = 0;
            }
            onTriesChange?.Invoke(currentTries);
        }

        internal void ResetCurrentStats()
        {
            currentScore = 0;
            currentTries = 0;
            onCurrentScoreChange?.Invoke(currentScore);
            onTriesChange?.Invoke(currentTries);
        }

        internal int GetHighScore()
        {
            if (PlayerPrefs.HasKey("highScore"))
            {
                return PlayerPrefs.GetInt("highScore");
            }
            return 0;
        }


        private void SetHighScore(int newScore)
        {
            int oldScore = 0;
            if (PlayerPrefs.HasKey("highScore"))
            {
                oldScore = PlayerPrefs.GetInt("highScore");    
            }
            if (oldScore < newScore)
            {
                PlayerPrefs.SetInt("highScore", newScore);
                onHighScoreChange?.Invoke(newScore);
            }
            else
            {
                onHighScoreChange?.Invoke(oldScore);
            }
        }
    }
}
