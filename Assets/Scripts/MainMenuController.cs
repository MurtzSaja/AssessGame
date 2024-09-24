using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AssessGame
{
    internal delegate void OnChangeDifficulty(DifficultyType difficultyType);
    internal enum DifficultyType
    {
        Easy,
        Medium,
        Hard
    }

    [Serializable]
    internal class DifficultyData
    {
        public Toggle toggleControl;
        public DifficultyType difficultyType;
    }
    internal class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private List<DifficultyData> difficultyToggles;
        [SerializeField]
        private Button playBtn;
        [SerializeField]
        private TextMeshProUGUI highScoreTxt;
        private OnChangeDifficulty OnChangeDifficulty;
        internal void Init(DifficultyType lastDifficulty, OnChangeDifficulty onChangeDifficulty, Action OnPlay)
        {
            for (int i = 0; i < difficultyToggles.Count; ++i)
            {
                if (difficultyToggles[i].difficultyType == lastDifficulty)
                {
                    difficultyToggles[i].toggleControl.SetIsOnWithoutNotify(true);
                }

                difficultyToggles[i].toggleControl.onValueChanged.AddListener(OnTogglevalueChanged);
            }
            this.OnChangeDifficulty = onChangeDifficulty;
            playBtn.onClick.RemoveAllListeners();
            playBtn.onClick.AddListener(() => { OnPlay(); });
            highScoreTxt.text = ScoreManager.Instance.GetHighScore().ToString();
            ScoreManager.Instance.onHighScoreChange += (highScore) =>
            {
                highScoreTxt.text = highScore.ToString();
            };
        }

        private void OnTogglevalueChanged(bool state)
        {
            if (!state)
            {
                return;
            }

            for (int i = 0; i < difficultyToggles.Count; ++i)
            {
                if (difficultyToggles[i].toggleControl.isOn)
                {
                    OnChangeDifficulty?.Invoke(difficultyToggles[i].difficultyType);
                    break;
                }
            }
        }
    }

}