using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssessGame
{
    [Serializable]
    internal class DifficultyGrid
    {
        public float hintDuration;
        public int toMatchCount;
        public DifficultyType DifficultyType;
        public List<Vector2Int> supportedGridSizes;
    }

    [CreateAssetMenu(fileName = "GameGenDataSCRO",menuName = "ScriptableObjects/GameGenData",order = 0)]
    internal class GameGenDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<DifficultyGrid> difficultyGrids;
        [SerializeField]
        private Sprite cardBack;
        [SerializeField]
        private List<Sprite> cardFrontSprites;
        [SerializeField]
        private int matchAwardScore = 1, matchComboAwardScore = 4, matchCountForCombo = 2;

        internal List<DifficultyGrid> DifficultyGrids { get => difficultyGrids;private set => difficultyGrids = value; }
        internal Sprite CardBack { get => cardBack;private set => cardBack = value; }
        internal List<Sprite> CardFrontSprites { get => cardFrontSprites;private set => cardFrontSprites = value; }
        internal int MatchAwardScore { get => matchAwardScore;private set => matchAwardScore = value; }
        internal int MatchComboAwardScore { get => matchComboAwardScore;private set => matchComboAwardScore = value; }
        internal int MatchCountForCombo { get => matchCountForCombo;private set => matchCountForCombo = value; }
    }
}
