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
    internal class GridController : MonoBehaviour
    {
        public delegate void OnHint();
        private OnHint onHint;
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

        private Sprite backSprite;
        private Vector2 frontSpriteSize;

        internal async Task<Vector2Int> Init(GameObject cardPrefab, DifficultyGrid selectedDifficultyGrid, List<Sprite> frontFaceSprites, Action<CardItemController> CheckMatch)
        {
            Vector2Int gridSize = selectedDifficultyGrid.supportedGridSizes[Random.Range(0, selectedDifficultyGrid.supportedGridSizes.Count)];
            await GridItemCreator(cardPrefab, gridSize, selectedDifficultyGrid.toMatchCount, selectedDifficultyGrid.hintDuration, frontFaceSprites, CheckMatch);
            return gridSize;
        }

        internal void ResetGrid()
        {
            List<CardItemController> cards = new List<CardItemController>(grid.GetComponentsInChildren<CardItemController>());
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].ResetCard();
            }
            onHint = null;
        }

        internal void ShowHint()
        {
            onHint?.Invoke();
        }

        private async Task GridItemCreator(GameObject cardPrefab, Vector2Int gridSize, int toMatchCount, float hintDuration, List<Sprite> frontFaceSprites, Action<CardItemController> CheckMatch)
        {
            int itemsCount = gridSize.x * gridSize.y;
            int spriteForCombination = itemsCount / toMatchCount;

            int startIndex = Random.Range(0, frontFaceSprites.Count);
            int endIndex = startIndex + spriteForCombination;

            if (endIndex >= frontFaceSprites.Count - 1)
            {
                startIndex -= endIndex - frontFaceSprites.Count;
            }

            Debug.Log(startIndex + "," + spriteForCombination);

            List<FrontImageMatchData> selectedRange = new List<FrontImageMatchData>();
            frontFaceSprites.GetRange(startIndex, spriteForCombination).ForEach(x =>
            {
                selectedRange.Add(new FrontImageMatchData(x, toMatchCount));
            });
            List<CardItemController> selectedTransform = SpawnGridCards(itemsCount, cardPrefab, grid.transform);
            backSprite = selectedTransform[0].BackImage;
            for (int i = 0; i < selectedTransform.Count;)
            {
                FrontImageMatchData frontImageMatchData = selectedRange.Count >= 1 ? selectedRange[Random.Range(0, selectedRange.Count)] : selectedRange.Last();
                selectedTransform[i].Init(frontImageMatchData.sprite, CheckMatch, hintDuration,ref onHint);
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
            List<CardItemController> gridCards = new List<CardItemController>(grid.GetComponentsInChildren<CardItemController>());
            for (int i = 0; i < itemCountToSpawn; i++)
            {
                CardItemController temp = null;
                if (i >= gridCards.Count)
                {
                    temp = Instantiate(prefab, gridParent).GetComponent<CardItemController>();
                    gridCards.Add(temp);
                }
                else
                {
                    temp = gridCards[i];
                }
            }
            return gridCards;
        }
    }
}