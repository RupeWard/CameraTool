using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CX.CamTool.UI
{
	public class GyroPanel : MonoBehaviour
	{
		public GameObject noGyroPanel;
		public GameObject yesGyroPanel;

		public UnityEngine.UI.Text attXDegText;
		public UnityEngine.UI.Text attYDegText;
		public UnityEngine.UI.Text attZDegText;

		private bool _isShowing = false;

		private Gyroscope _gyro = null;
		private bool _isAvailable = false;

		public RectTransform cachedRT
		{
			get;
			private set;
		}

		private float _size = 0f;

		private void Awake()
		{
			cachedRT = GetComponent<RectTransform>( );
		}

		public void Init()
		{
			RectTransform parentRT = transform.parent.GetComponent<RectTransform>( );
			_size = parentRT.sizeDelta.y;
            cachedRT.sizeDelta = new Vector2( _size, _size );

			_isAvailable = SystemInfo.supportsGyroscope;
            if (_isAvailable)
			{
				_gyro = Input.gyro;

				_gyro.enabled = false;
				_gyro.enabled = true;
			}
			yesGyroPanel.SetActive( _isAvailable);
			noGyroPanel.SetActive( !_isAvailable );

			Hide( true );
		}

		private void Update()
		{
			if (_isAvailable && _isShowing)
			{
				Vector3 attEuler = Input.gyro.attitude.eulerAngles;

				attXDegText.text = attEuler.x.ToString("0.00");
				attYDegText.text = attEuler.y.ToString( "0.00" );
				attZDegText.text = attEuler.z.ToString( "0.00" );
			}
		}

		public void Hide(bool immediate = false)
		{
			cachedRT.DOAnchorPosX( -1f * _size, (immediate) ? (0f) : (UIManager.Instance.tweenTime) );
			_isShowing = false;
        }

		public void Show( bool immediate = false )
		{
			cachedRT.DOAnchorPosX( 0f, (immediate) ? (0f) : (UIManager.Instance.tweenTime) );
			_isShowing = true;
		}

		public void HandleButton_Hide()
		{
			Hide( );
		}
	}
}

