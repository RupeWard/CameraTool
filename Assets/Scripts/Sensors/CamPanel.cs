using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CX.CamTool.UI
{
	public class CamPanel : MonoBehaviour
	{
		public UnityEngine.UI.RawImage camImage;

		private RectTransform _camImageRT;
		private Material _camImageMaterial;

		public RectTransform cachedRT
		{
			get;
			private set;
		}

		private void Awake()
		{
			cachedRT = GetComponent<RectTransform>( );
			_camImageRT = camImage.GetComponent<RectTransform>( );
			_camImageMaterial = new Material( camImage.material );
			camImage.material = _camImageMaterial;
		}

		private void Start()
		{
			SetOrientation( UIManager.Instance.screenOrientation );
			UIManager.Instance.onScreenOrientationChanged += SetOrientation;
			AppManager.Instance.camManager.onCamUpdate += HandleCamUpdate;
			AppManager.Instance.camManager.onCamImageSizeChanged += HandleCamSizeChanged;
			UIManager.Instance.onTestButtonClicked += ToggleCamImageSize;
		}

		private void SetOrientation(ScreenOrientation orientation)
		{
			int rightAngles = UIManager.ScreenOrientation2RightAngles( orientation );
			Debug.Log( "\nCamPanel.SetOrientation( "+orientation+") = "+ rightAngles + " right angles"
				+ "\nWCT: vra=" + AppManager.Instance.camManager.webCamTexture.videoRotationAngle 
				+ "\n     vvm="+ AppManager.Instance.camManager.webCamTexture.videoVerticallyMirrored+"\n" );
						
#if UNITY_EDITOR
			rightAngles++;
#endif
			Vector2 size = new Vector2( UIManager.Instance._screenMaxDim, UIManager.Instance._screenMinDim );
			Vector2 imSize = AppManager.Instance.camManager.camImageSize;

			if (size.x/size.y > imSize.x/imSize.y)
			{
				size.x = size.y * imSize.x / imSize.y;
			}
			else
			{
				size.y = size.x * imSize.y / imSize.x;
			}
			Vector3 rotEuler = new Vector3( 0f, 0f, 90f * rightAngles );

			_camImageRT.sizeDelta = size;
			_camImageRT.localRotation = Quaternion.Euler( rotEuler );

			Debug.Log( "\nSet camImage size = " + _camImageRT.sizeDelta
				+ ", cam texture size = " + AppManager.Instance.camManager.webCamTexture.width + ", " + AppManager.Instance.camManager.webCamTexture.height );
        }

		public void HandleCamUpdate(WebCamTexture webCamTexture)
		{
			_camImageMaterial.mainTexture = webCamTexture;
		}

		public void HandleCamSizeChanged()
		{
			SetOrientation( UIManager.Instance.screenOrientation );
		}

		public void ToggleCamImageSize()
		{
			if (_camImageRT.transform.localScale.x != 1f)
			{
				_camImageRT.transform.localScale = Vector3.one;
            }
			else
			{
				_camImageRT.transform.localScale = new Vector3(0.5f,0.5f,1f);
			}
		}
	}
}
