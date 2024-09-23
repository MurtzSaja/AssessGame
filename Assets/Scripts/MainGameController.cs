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
    public class MainGameController : MonoBehaviour
    {
        [Serializable]
        private class MatchProcessor
        {
            private Queue<CardItemController> cardItemControllers;
            private int size;
            private float delayTimer;
            private bool processing = false;
            private Action<MatchProcessor> onComplete;
            public MatchProcessor(Queue<CardItemController> toAddCards, int size, float delayTimer, Action<MatchProcessor> OnComplete)
            {
                this.onComplete = OnComplete;
                cardItemControllers = new Queue<CardItemController>(size);
                this.delayTimer = delayTimer;
                this.size = size;
                AddItems(toAddCards);
            }

            public int QueCount()
            {
                return cardItemControllers.Count;
            }

            public void AddItems(Queue<CardItemController> cardItemController)
            {
                cardItemControllers = new Queue<CardItemController>(cardItemController);
            }

            public async void ProcessMatch()
            {
                if (processing)
                {
                    return;
                }
                processing = true;
                if (cardItemControllers.Count >= size)
                {
                    if (cardItemControllers.Select(x => x.ImageToSetName).Distinct().ToList().Count == 1)
                    {
                        foreach (CardItemController temp in cardItemControllers)
                        {
                            temp.ResetFlip(true);
                        }
                    }
                    else
                    {
                        await Task.Delay((int)(delayTimer * 1000));

                        foreach (CardItemController temp in cardItemControllers)
                        {
                            temp.ResetFlip();
                        }
                    }
                    cardItemControllers.Clear();
                }
                processing = false;
                onComplete?.Invoke(this);
            }
        }

        private class FrontImageMatchData
        {
            public Sprite sprite;
            public int matchCount;

            public FrontImageMatchData(Sprite sprite, int matchCount)
            {
                this.sprite = sprite;
                this.matchCount = matchCount;
            }
        }

        [SerializeField]
        private GridLayoutGroup grid;
        [SerializeField]
        private GameObject cardPrefab;

        [SerializeField]
        DifficultyGrid selectedDifficultyGrid;
        [SerializeField]
        List<Sprite> frontFaceSprites;


        private Sprite backSprite;
        private Vector2 frontSpriteSize;
        private List<MatchProcessor> matchQueue;
        private Queue<CardItemController> localQueue = new Queue<CardItemController>(2);
        private bool isDisposing = false;

        public void InitGame(DifficultyGrid selectedDifficultyGrid, List<Sprite> frontFaceSprites, Sprite back)
        {
            matchQueue = new List<MatchProcessor>();
            this.backSprite = back;
            this.selectedDifficultyGrid = selectedDifficultyGrid;
            this.frontFaceSprites = frontFaceSprites;
            GridItemCreator();
            gameObject.SetActive(true);
        }

        private async void GridItemCreator()
        {
            Vector2Int gridSize = selectedDifficultyGrid.supportedGridSizes[Random.Range(0, selectedDifficultyGrid.supportedGridSizes.Count)];
            int itemsCount = gridSize.x * gridSize.y;
            int spriteForCombination = itemsCount / 2;

            int startIndex = Random.Range(0, frontFaceSprites.Count);
            int endIndex = startIndex + spriteForCombination;

            Debug.Log(startIndex + "," + endIndex + "," + spriteForCombination);

            if (endIndex >= frontFaceSprites.Count - 1)
            {
                startIndex -= endIndex - frontFaceSprites.Count;
            }

            Debug.Log(startIndex + "," + spriteForCombination);

            List<FrontImageMatchData> selectedRange = new List<FrontImageMatchData>();
            frontFaceSprites.GetRange(startIndex, spriteForCombination).ForEach(x =>
            {
                selectedRange.Add(new FrontImageMatchData(x, 2));
            });
            List<CardItemController> selectedTransform = SpawnGridCards(itemsCount, cardPrefab, grid.transform);
            for (int i = 0; i < selectedTransform.Count;)
            {
                FrontImageMatchData frontImageMatchData = selectedRange.Count >= 1 ? selectedRange[Random.Range(0, selectedRange.Count)] : selectedRange.Last();
                selectedTransform[i].Init(frontImageMatchData.sprite, CheckMatch);
                frontImageMatchData.matchCount--;
                if (frontImageMatchData.matchCount <= 0)
                {
                    selectedRange.Remove(frontImageMatchData);
                }
                i++;
            }
            await Task.Delay((int)Time.deltaTime * 1000);
            ResizeGrid(gridSize);
        }

        private void ResizeGrid(Vector2Int gridSize)
        {
            float frontImageAspect = backSprite.textureRect.size.x / backSprite.textureRect.size.y;

            RectTransform gridTransform = grid.transform as RectTransform;
            float containerWidth = gridTransform.rect.width;
            float containerHeight = gridTransform.rect.height;

            float totalAvailableWidth = containerWidth - (gridSize.x - 1) * grid.spacing.x;
            float totalAvailableHeight = containerHeight - (gridSize.y - 1) * grid.spacing.y;

            float maxCellWidth = totalAvailableWidth / gridSize.x;
            float maxCellHeight = totalAvailableHeight / gridSize.y;


            float cellWidth, cellHeight;

            if (maxCellWidth / maxCellHeight > frontImageAspect)
            {
                cellHeight = maxCellHeight;
                cellWidth = cellHeight * frontImageAspect;
            }
            else
            {
                cellWidth = maxCellWidth;
                cellHeight = cellWidth / frontImageAspect;
            }

            float scaleFactorWidth = totalAvailableWidth / (cellWidth * gridSize.x);
            float scaleFactorHeight = totalAvailableHeight / (cellHeight * gridSize.y);

            float scaleFactor = Mathf.Min(scaleFactorWidth, scaleFactorHeight);

            cellWidth *= scaleFactor;
            cellHeight *= scaleFactor;

            if (containerWidth > containerHeight)
            {
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = gridSize.x;
            }
            else
            {
                grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                grid.constraintCount = gridSize.y;
            }

            grid.cellSize = new Vector2(cellWidth, cellHeight);
        }


        private List<CardItemController> SpawnGridCards(int itemCountToSpawn, GameObject prefab, Transform gridParent)
        {
            List<CardItemController> gridCards = new List<CardItemController>();
            for (int i = 0; i < itemCountToSpawn; i++)
            {
                CardItemController temp = null;
                if (i >= gridParent.childCount)
                {
                    temp = Instantiate(prefab, gridParent).GetComponent<CardItemController>();
                }
                else
                {
                    temp = gridParent.GetChild(i).GetComponent<CardItemController>();
                }
                gridCards.Add(temp);
            }
            return gridCards;
        }

        private void CheckMatch(CardItemController toMatchCard)
        {
            localQueue.Enqueue(toMatchCard);
            if (localQueue.Count >= selectedDifficultyGrid.toMatchCount)
            {
                matchQueue.Add(new MatchProcessor(localQueue, selectedDifficultyGrid.toMatchCount, 0.5f, DisposeProcess));
                localQueue.Clear();
            }
        }

        private void Update()
        {
            if (matchQueue != null && !isDisposing)
            {
                for (int i = 0; i < matchQueue.Count; i++) 
                {
                    matchQueue[i].ProcessMatch();
                }
            }
        }

        private void DisposeProcess(MatchProcessor toDispose)
        {
            isDisposing = true;
            matchQueue.Remove(toDispose);
            isDisposing = false;
        }
    }
}