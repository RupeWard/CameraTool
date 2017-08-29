﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CX.CamTool.UI
{
    abstract public class SlideOutUIPanel : MonoBehaviour
    {
        public RectTransform cachedRT
        {
            get;
            private set;
        }

        private float _size = 0f;
        public bool isShowing
        {
            get;
            private set;
        }

        protected virtual void Awake()
        {
            cachedRT = GetComponent<RectTransform>();
        }

        public void Init(bool showing = false)
        {
            RectTransform parentRT = transform.parent.GetComponent<RectTransform>();
            _size = parentRT.sizeDelta.y;
            cachedRT.sizeDelta = new Vector2(_size, _size);

            DoInit(showing);
            Show(showing, true);
        }

        abstract protected void DoInit(bool showing);

        public void Show(bool showing, bool immediate)
        {
            float xPos = (showing) ? (0f) : (-1f * _size);
            float duration = (immediate) ? (0f) : (UIManager.Instance.tweenTime);
            cachedRT.DOAnchorPosX(xPos, duration).SetEase(Ease.InOutQuad);
            isShowing = showing;
        }

        public void ShowImmediate()
        {
            Show(true, true);
        }

        public void ShowTween()
        {
            Show(true, false);
        }

        public void HideImmediate()
        {
            Show(false, true);
        }

        public void HideTween()
        {
            Show(false, false);
        }

        public void HandleButton_Hide()
        {
            HideTween();
        }

    }
}
