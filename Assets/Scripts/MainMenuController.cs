using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssessGame
{
    public delegate void OnChangeDifficulty(DifficultyType difficultyType);
    public enum DifficultyType
    {
        Easy,
        Medium,
        Hard
    }

    [Serializable]
    public class DifficultyData
    {
        public Toggle toggleControl;
        public DifficultyType difficultyType;
    }
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private List<DifficultyData> difficultyToggles;
        private OnChangeDifficulty OnChangeDifficulty;
        public void Init(DifficultyType lastDifficulty,OnChangeDifficulty onChangeDifficulty)
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