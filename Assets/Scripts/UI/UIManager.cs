using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CX.CamTool
{
	public class UIManager : Core.Singleton.SingletonSceneLifetime< UIManager>
	{
		public RectTransform uIPanel;
		public RectTransform mainButtonsPanel;
		public RectTransform sidePanel;
		public GameObject showMainButtonsButton;

		public float tweenTime = 0.5f;

		protected override void PostAwake()
		{
			sidePanel.sizeDelta = new Vector2( uIPanel.rect.width * 0.1f, uIPanel.rect.height );
			mainButtonsPanel.sizeDelta = sidePanel.sizeDelta;
            ShowMainButtons( true );
		}

		private void ShowMainButtons(bool immediate = false)
		{
			HideShowMainButtonsButton( );
			mainButtonsPanel.DOAnchorPosY( 0f, (immediate)?(0f):(tweenTime) );
		}

		private void HideMainButtons( bool immediate = false )
		{
			mainButtonsPanel.DOAnchorPosY( mainButtonsPanel.sizeDelta.y, (immediate) ? (0f) : (tweenTime) ).OnComplete(ShowShowMainButtonsButton);
		}

		private void ShowShowMainButtonsButton( )
		{
			ShowShowMainButtonsButton( true );
        }

		private void HideShowMainButtonsButton( )
		{
			ShowShowMainButtonsButton( false );
		}

		private void ShowShowMainButtonsButton(bool b)
		{
			showMainButtonsButton.gameObject.SetActive( b );
		}

		public void HandleButton_ShowMainButtons()
		{
			ShowMainButtons( );
		}

		public void HandleButton_HideMainButtons( )
		{
			HideMainButtons( );
		}

		public void HandleButton_Quit( )
		{
			Application.Quit( );
		}

	}
}
