using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssessGame
{
    [Serializable]
    public class DifficultyGrid
    {
        public int toMatchCount;
        public DifficultyType DifficultyType;
        public List<Vector2Int> supportedGridSizes;
    }

    [CreateAssetMenu(fileName = "GameGenDataSCRO",menuName = "ScriptableObjects/GameGenData",order = 0)]
    public class GameGenDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<DifficultyGrid> difficultyGrids;
        [SerializeField]
        private Sprite cardBack;
        [SerializeField]
        private List<Sprite> cardFrontSprites;

        public List<DifficultyGrid> DifficultyGrids { get => difficultyGrids;private set => difficultyGrids = value; }
        public Sprite CardBack { get => cardBack;private set => cardBack = value; }
        public List<Sprite> CardFrontSprites { get => cardFrontSprites;private set => cardFrontSprites = value; }
    }
}
