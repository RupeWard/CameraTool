using System.Collections;
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

        public float size
        {
            get;
            private set;
        }

        public bool isShowing
        {
            get;
            private set;
        }

        protected virtual void Awake()
        {
            cachedRT = GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {
            UIManager.Instance.RegisterSlideOutPanel(this);
        }

        public void Init(bool showing = false)
        {
            RectTransform parentRT = transform.parent.GetComponent<RectTransform>();
            size = parentRT.sizeDelta.y;
            cachedRT.sizeDelta = new Vector2(size, size);

            DoInit(showing);
            Show(showing, true);
        }

        abstract protected void DoInit(bool showing);

        public void Show(bool showing, bool immediate)
        {
            cachedRT.DOKill();
            float xPos = (showing) ? (0f) : (-1f * size);
            float duration = (immediate) ? (0f) : (UIManager.Instance.tweenTime);
            cachedRT.DOAnchorPosX(xPos, duration).SetEase(Ease.InOutQuad);
            isShowing = showing;
            UIManager.Instance.HandleSlideOutPanelIsShowing(this, showing);
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
