using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CX.CamTool.UI
{
    public class LogPanel : SlideOutUIPanel
	{
		public UnityEngine.UI.Text titleText;
		public UnityEngine.UI.Text versionText;

		protected override void DoInit(bool showing)
		{
            titleText.text = "Log: version " + Core.Version.Version.versionNumber.ToString();
		}

		private void Update()
		{
		}

	}
}

