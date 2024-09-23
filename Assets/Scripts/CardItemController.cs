using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AssessGame
{
    public enum CardFlipState
    {
        Front,
        Back
    }

    public class CardItemController : MonoBehaviour, IPointerClickHandler
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
        [SerializeField]
        private CardFlipState flipState = CardFlipState.Back;

        private Coroutine flipRoutine;
        private Action<CardItemController> checkCard;
        public string ImageToSetName { get => imageToSetName; private set => imageToSetName = value; }

        public void Init(Sprite imageToSet, Action<CardItemController> checkCard)
        {
            frontImage.sprite = imageToSet;
            imageToSetName = imageToSet.name;
            this.checkCard = checkCard;
            holder.gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (flipRoutine != null || flipState != CardFlipState.Front)
            {
                return;
            }

            flipRoutine = StartCoroutine(Flip(flipState, rotateTime));
        }

        IEnumerator Flip(CardFlipState cardFlipState, float rotateTime)
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
                            case CardFlipState.Front:
                                frontImageHolder.SetActive(true);
                                backImage.gameObject.SetActive(false);
                                flipState = CardFlipState.Back;
                                break;
                            case CardFlipState.Back:
                                frontImageHolder.SetActive(false);
                                backImage.gameObject.SetActive(true);
                                flipState = CardFlipState.Front;
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
            if (cardFlipState == CardFlipState.Front)
            {
                checkCard?.Invoke(this);
            }
            yield break;
        }

        public void ResetFlip(bool snapRotateToDeactivate = false)
        {
            if (flipState == CardFlipState.Back)
            {
                flipRoutine = StartCoroutine(Flip(flipState, snapRotateToDeactivate ? 0 : rotateTime));
                holder.gameObject.SetActive(!snapRotateToDeactivate);
            }
        }
    }
}
