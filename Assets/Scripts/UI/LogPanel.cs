using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CX.CamTool.UI
{
    public class LogPanel : SlideOutUIPanel
	{
		public UnityEngine.UI.Text titleText;
        public UnityEngine.UI.Text logText;
        public UnityEngine.UI.ScrollRect scrollRect;
        public UnityEngine.UI.Text lockButtonText;

        private RectTransform _logtextRT;

        private List<string> _lines = new List<string>();
        public float heightFactor = 10f;
        public int maxLines = 500;

        private bool _isLocked = false;

        private System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

        protected override void Awake()
        {
            base.Awake();
            _logtextRT = logText.GetComponent<RectTransform>();
        }

        protected override void DoInit(bool showing)
		{
            titleText.text = "Log: version " + Core.Version.Version.versionNumber.ToString();

            RectTransform viewportRT = logText.transform.parent.GetComponent<RectTransform>();
            RectTransform logTextRT = logText.GetComponent<RectTransform>();
            logTextRT.sizeDelta = new Vector2(size - 40f, viewportRT.rect.height * 10f);

            RegisterListeners();

            /*
            // testing
            for (int i = 0; i < 500; i++)
            {
                AddMessage(i.ToString()+" -------- "+i.ToString(), true, true);
            }
            */
		}

        private void RegisterListeners()
        {
            UnregisterListeners();
            MessageBus.Instance.addToConsole += AddMessage;
        }

        private void UnregisterListeners()
        {
            MessageBus.Instance.addToConsole -= AddMessage;
        }

        public void AddMessage(string s, bool scrollTo, bool forceShow)
        {
            if (forceShow)
            {
                ShowTween();
            }

            bool remake = false;
            string[] newLines = s.Split('\n');
            for (int i = 0; i < newLines.Length; i++)
            {
                _lines.Add(newLines[i]);
            }
            while (_lines.Count > maxLines)
            {
                remake = true;
                _lines.RemoveAt(0);
            }
            if (remake)
            {
                stringBuilder.Length = 0;
                for (int i = 0; i < _lines.Count; i++)
                {
                    stringBuilder.Append(_lines[i]).Append("\n");
                }
                logText.text = stringBuilder.ToString();
            }
            else
            {
                logText.text = logText.text + "\n" + s;
            }
            if (scrollTo && !_isLocked)
            {
                ScrollToBottom();
            }
        }

        private void ScrollToBottom()
        {
            ScrollToBottom(0.5f);
        }

        private void ScrollToBottomImmediate()
        {
            ScrollToBottom(0f);
        }

        private void ScrollToBottom(float f)
        {
            _logtextRT.DOAnchorPosY(0f, f);
        }

        private void SetLockButtonText()
        {
            lockButtonText.text = (_isLocked) ? ("UNLOCK") : ("LOCK");
        }

        private void SetLock(bool l)
        {
            _isLocked = l;
            SetLockButtonText();
        }

        public void HandleButton_Lock()
        {
            SetLock(!_isLocked);
        }
    
        public void HandleButton_Bottom()
        {
            SetLock(false);
            ScrollToBottom();
        }


    }
}

