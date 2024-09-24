using AssessGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssessGame
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public static GameManager Instance
        { get { return instance; } }
        [SerializeField]
        private GameGenDataScriptableObject gameGenData;
        [SerializeField]
        private MainMenuController mainMenuController;
        [SerializeField]
        private MainGameController mainGameController;
        [SerializeField]
        private AudioManager audioManager;
        [SerializeField]
        private ScoreManager scoreManager;
        [SerializeField]
        private SideMenuController sideMenuController;
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
            scoreManager.Init(gameGenData.MatchAwardScore, gameGenData.MatchComboAwardScore, gameGenData.MatchCountForCombo);
            sideMenuController.Init(() =>
            {
                mainGameController.ResetGameBoard();
                mainMenuController.gameObject.SetActive(true);
                mainGameController.gameObject.SetActive(false);
            }, () => 
            { 
                mainGameController.InitGame(gameGenData.DifficultyGrids.Where(x => x.DifficultyType == gameDifficulty).FirstOrDefault(), gameGenData.CardFrontSprites, gameGenData.CardBack); mainMenuController.gameObject.SetActive(false); 
            }, scoreManager);
            audioManager.Init();
            mainMenuController.Init(gameDifficulty, (diffculty) => { gameDifficulty = diffculty; PlayerPrefs.SetInt("difficulty", (int)diffculty); }, () => { mainGameController.InitGame(gameGenData.DifficultyGrids.Where(x => x.DifficultyType == gameDifficulty).FirstOrDefault(), gameGenData.CardFrontSprites, gameGenData.CardBack); mainMenuController.gameObject.SetActive(false); });
        }
    }
}
