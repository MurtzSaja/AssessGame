using AssessGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssessGame
{
    public class GameManager : MonoBehaviour
    {
        private GameManager instance;

        public GameManager Instance
        { get { return instance; } }

        [SerializeField]
        private MainMenuController mainMenuController;
        [SerializeField]
        private DifficultyType gameDifficulty;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this);
            Application.targetFrameRate = 1000;
            Init();
        }

        private void Init()
        {
            if (PlayerPrefs.HasKey("difficulty"))
            {
                gameDifficulty = (DifficultyType)PlayerPrefs.GetInt("difficulty");
            }
            else
            {
                gameDifficulty = DifficultyType.Easy;
            }
            mainMenuController.Init(gameDifficulty, (diffculty) => { gameDifficulty = diffculty; PlayerPrefs.SetInt("difficulty", (int)diffculty); });
        }


    }
}
