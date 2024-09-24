using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AssessGame
{
    internal enum CardFlipState
    {
        Front,
        Back
    }

    internal class CardItemController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private string imageToSetName;
        [SerializeField]
        private Transform holder;
        [SerializeField]
        private Image backImage, frontImage;
        [SerializeField]
        private GameObject frontImageHolder;
        [SerializeField]
        private float rotateTime;

        private CardFlipState flipState = CardFlipState.Back;

        private Coroutine flipRoutine;
        private Action<CardItemController> checkCard;
        private bool hinting;
        private float hintDuration;
        internal string ImageToSetName { get => imageToSetName; private set => imageToSetName = value; }
        internal Sprite BackImage { get => backImage.sprite; }

        internal void Init(Sprite imageToSet, Action<CardItemController> checkCard, float hintDuration,ref GridController.OnHint OnShowHint)
        {
            this.hintDuration = hintDuration;
            frontImage.sprite = imageToSet;
            imageToSetName = imageToSet.name;
            this.checkCard = checkCard;
            holder.gameObject.SetActive(true);
            OnShowHint += FlipForHint;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (flipRoutine != null || flipState != CardFlipState.Back || hinting)
            {
                return;
            }

            flipRoutine = StartCoroutine(Flip(flipState, rotateTime));
        }

        internal void ResetFlip(bool snapRotateToDeactivate = false)
        {
            if (flipState == CardFlipState.Front && gameObject.activeInHierarchy)
            {
                flipRoutine = StartCoroutine(Flip(flipState, snapRotateToDeactivate ? 0 : rotateTime));
                holder.gameObject.SetActive(!snapRotateToDeactivate);
            }
        }

        internal void ResetCard()
        {
            frontImageHolder.SetActive(false);
            backImage.gameObject.SetActive(true);
            flipState = CardFlipState.Front;
            holder.rotation = Quaternion.identity;
            gameObject.SetActive(false);
        }

        private IEnumerator Flip(CardFlipState cardFlipState, float rotateTime, bool isForHint = false)
        {
            float timer = 0;
            Quaternion orgRot = transform.rotation;
            Quaternion targetRot = Quaternion.Euler(0, 90, 0);
            while (true)
            {
                if (timer < rotateTime)
                {
                    timer += Time.deltaTime;
                    holder.rotation = Quaternion.Lerp(orgRot, targetRot, timer / rotateTime);
                }
                else
                {
                    if (cardFlipState == flipState)
                    {
                        switch (cardFlipState)
                        {
                            case CardFlipState.Back:
                                frontImageHolder.SetActive(true);
                                backImage.gameObject.SetActive(false);
                                flipState = CardFlipState.Front;
                                break;
                            case CardFlipState.Front:
                                frontImageHolder.SetActive(false);
                                backImage.gameObject.SetActive(true);
                                flipState = CardFlipState.Back;
                                break;
                            default:
                                break;
                        }
                        timer = 0;
                        orgRot = targetRot;
                        targetRot = Quaternion.identity;
                    }
                    else
                    {
                        break;
                    }
                }
                yield return null;
            }
            flipRoutine = null;
            if (!isForHint && cardFlipState == CardFlipState.Back)
            {
                checkCard?.Invoke(this);
            }
            yield break;
        }

        private void FlipForHint()
        {
            IEnumerator HintFilp()
            {
                hinting = true;
                flipRoutine = StartCoroutine(Flip(flipState, rotateTime, true));
                yield return flipRoutine;
                yield return new WaitForSeconds(hintDuration);
                flipRoutine = StartCoroutine(Flip(flipState, rotateTime, true));
                yield return flipRoutine;
                hinting = false;
            }
            StartCoroutine(HintFilp());
        }
    }
}
