using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CX.CamTool.UI
{
	public class UIManager : Core.Singleton.SingletonSceneLifetime< UIManager>
	{
		public System.Action<float> onButtonSizeChanged;
		public System.Action<ScreenOrientation> onScreenOrientationChanged;
		public System.Action onTestButtonClicked;

		public RectTransform uIPanel;
		public RectTransform mainButtonsPanel;
		public RectTransform sidePanel;
		public GameObject showMainButtonsButton;
		public GameObject versionPanel;

		public GameObject reorientButton;

		public GyroPanel gyroPanel;

		public UnityEngine.UI.Text versionText;

		public float tweenTime = 0.5f;
		public float versionShowTime = 5f;

		public ScreenOrientation screenOrientation
		{
			get;
			private set;
		}

		public float _screenMinDim, _screenMaxDim;
		private float _buttonSize = 120f;
		public float buttonSize
		{
			get
			{
				return _buttonSize;
			}
			private set
			{
				if (value != _buttonSize)
				{
					_buttonSize = value;
					if (onButtonSizeChanged != null)
					{
						onButtonSizeChanged( _buttonSize );
					}
				}

			}
		}

		protected override void PostAwake()
		{
			screenOrientation = Screen.orientation;

			_screenMinDim = Mathf.Min( uIPanel.rect.width, uIPanel.rect.height );
			_screenMaxDim = Mathf.Max( uIPanel.rect.width, uIPanel.rect.height );

			gyroPanel.gameObject.SetActive( true );

			reorientButton.SetActive( false );
			sidePanel.sizeDelta = new Vector2( _buttonSize, _screenMinDim );
			mainButtonsPanel.sizeDelta = sidePanel.sizeDelta;
            ShowMainButtons( true );

			versionText.text = Core.Version.Version.versionNumber.ToString( );

			gyroPanel.Init( );

#if UNITY_EDITOR
			if (uIPanel.rect.width > uIPanel.rect.height)
			{
				screenOrientation = ScreenOrientation.LandscapeLeft;
			}
			else
			{
				screenOrientation = ScreenOrientation.Portrait;
			}
			Debug.LogWarning( "Editor screen orientation = " + screenOrientation );
#endif
		}

		private void Start()
		{
			SetScreenOrientation( screenOrientation );
			HandleScreenOrientationSet( );
			versionPanel.SetActive( true );
			StartCoroutine( HideVersionCR( ) );
		}

		private IEnumerator HideVersionCR()
		{
			yield return new WaitForSeconds( 5f );
			versionPanel.SetActive( false );
		}

		private void Update()
		{
			if (Screen.orientation != screenOrientation)
			{
				SetScreenOrientation( Screen.orientation );
				reorientButton.SetActive( true );
			}
		}

		private void SetScreenOrientation(ScreenOrientation orientation)
		{
			screenOrientation = orientation;
//			HandleScreenOrientationSet( );
		}

		public static int ScreenOrientation2RightAngles( ScreenOrientation orientation )
		{
			int rightAngles = 0;
			switch (orientation)
			{
				case ScreenOrientation.LandscapeLeft:
					{
						rightAngles = 0;
						break;
					}
				case ScreenOrientation.PortraitUpsideDown:
					{
						rightAngles = 1;
						break;
					}
				case ScreenOrientation.LandscapeRight:
					{
						rightAngles = 2;
						break;
					}
				case ScreenOrientation.Portrait:
					{
						rightAngles = 3;
						break;
					}
			}
			return rightAngles;
		}

		private void HandleScreenOrientationSet()
		{
			Debug.Log( "\nHandleScreenOrientationSet: " + screenOrientation + "\n" );
			if (onScreenOrientationChanged != null)
			{
				onScreenOrientationChanged( screenOrientation );
			}
		}

		public void ShowMainButtons(bool immediate = false)
		{
			HideShowMainButtonsButton( );
			mainButtonsPanel.DOAnchorPosY( 0f, (immediate)?(0f):(tweenTime) );
		}

		public void HideMainButtons( bool immediate = false )
		{
			mainButtonsPanel.DOAnchorPosY( mainButtonsPanel.sizeDelta.y, 
				(immediate) ? (0f) : (tweenTime) ).OnComplete(ShowShowMainButtonsButton);
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

		public void HandleButton_Version( )
		{
			versionPanel.SetActive( !versionPanel.activeSelf );
		}

		public void HandleButton_Test()
		{
			if (onTestButtonClicked != null)
			{
				onTestButtonClicked( );
			}
		}

		public void HandleButton_ShowGyro()
		{
			gyroPanel.Show( );
		}

		public void HandleButton_Reorient()
		{
			reorientButton.SetActive( false );
			HandleScreenOrientationSet( );
		}
	}
}
