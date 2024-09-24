using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AssessGame
{
    internal class MatchingController : MonoBehaviour
    {
        [Serializable]
        private class MatchProcessor
        {
            private Queue<CardItemController> cardItemControllers;
            private int size;
            private float delayTimer;
            private bool processing = false;
            private Action<MatchProcessor> onComplete;
            private Action onMatchSuccess;
            internal MatchProcessor(Queue<CardItemController> toAddCards, int size, float delayTimer, Action<MatchProcessor> OnComplete, Action OnMatchSuccess)
            {
                this.onComplete = OnComplete;
                this.onMatchSuccess = OnMatchSuccess;
                cardItemControllers = new Queue<CardItemController>(size);
                this.delayTimer = delayTimer;
                this.size = size;
                AddItems(toAddCards);
            }

            internal int QueueCount()
            {
                return cardItemControllers.Count;
            }

            internal void AddItems(Queue<CardItemController> cardItemController)
            {
                cardItemControllers = new Queue<CardItemController>(cardItemController);
            }

            internal async void ProcessMatch()
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
                        AudioManager.Instance.PlaySFXOnShot(AudioManager.AudioType.Matching);
                        ScoreManager.Instance.AddCurrentScore();
                        onMatchSuccess?.Invoke();
                        foreach (CardItemController temp in cardItemControllers)
                        {
                            temp.ResetFlip(true);
                        }
                    }
                    else
                    {
                        ScoreManager.Instance.SetCurrentTries(true);
                        AudioManager.Instance.PlaySFXOnShot(AudioManager.AudioType.MisMatch);
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


        private List<MatchProcessor> matchQueue;
        private Queue<CardItemController> localQueue;
        private bool isDisposing = false;
        private int toMatchCount;
        private Action onMatchSuccess;
        internal void Init(int toMatchCount, Action OnMatchSuccess)
        {
            localQueue = new Queue<CardItemController>(toMatchCount);
            matchQueue = new List<MatchProcessor>();
            this.toMatchCount = toMatchCount;
            this.onMatchSuccess = OnMatchSuccess;
        }
        internal void CheckMatch(CardItemController toMatchCard)
        {
            localQueue.Enqueue(toMatchCard);
            if (localQueue.Count >= toMatchCount)
            {
                matchQueue.Add(new MatchProcessor(localQueue, toMatchCount, 0.5f, DisposeProcess, onMatchSuccess));
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
