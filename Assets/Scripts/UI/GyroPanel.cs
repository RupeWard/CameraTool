using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CX.CamTool.UI
{
	public class GyroPanel : MonoBehaviour
	{
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
			Hide( true );
		}

		public void Hide(bool immediate = false)
		{
			cachedRT.DOAnchorPosX( -1f * _size, (immediate) ? (0f) : (UIManager.Instance.tweenTime) );
        }

		public void Show( bool immediate = false )
		{
			cachedRT.DOAnchorPosX( 0f, (immediate) ? (0f) : (UIManager.Instance.tweenTime) );
		}

		public void HandleButton_Hide()
		{
			Hide( );
		}
	}
}

