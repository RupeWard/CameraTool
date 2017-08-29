using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CX.CamTool.UI
{
	public class CamPanel : MonoBehaviour
	{
        static private readonly bool DEBUG_LOCAL = true;

		public UnityEngine.UI.RawImage camImage;

		private RectTransform _camImageRT;
        private Transform _camImageTransform;
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
            _camImageTransform = camImage.transform;
            _camImageMaterial = new Material( camImage.material );
			camImage.material = _camImageMaterial;
		}

        // These look the wrong way round but they're working
        private readonly Rect normalUVRect = new Rect(0f, 1f, 1f, -1f);
        private readonly Rect flippedYUVRect = new Rect(0f, 0f, 1f, 1f);

        private void Start()
        {
			StartCoroutine(WaitForCameraReadyCR());
        }

		private void Init()
		{
			SetOrientation( UIManager.Instance.screenOrientation );
            RegisterListeners();
		}

        private void RegisterListeners()
        {
            UnregisterListeners(); // just in case

            UIManager.Instance.onScreenOrientationChanged += SetOrientation;
            AppManager.Instance.camManager.onCamUpdate += HandleCamUpdate;
            AppManager.Instance.camManager.onCamImageSizeChanged += HandleCamSizeChanged;
            UIManager.Instance.onTestButtonClicked += ToggleCamImageSize;
        }

        private void UnregisterListeners()
        {
            UIManager.Instance.onScreenOrientationChanged -= SetOrientation;
            AppManager.Instance.camManager.onCamUpdate -= HandleCamUpdate;
            AppManager.Instance.camManager.onCamImageSizeChanged -= HandleCamSizeChanged;
            UIManager.Instance.onTestButtonClicked -= ToggleCamImageSize;
        }

        private IEnumerator WaitForCameraReadyCR()
        {
            // Don't need a timeout here as CamManager has one
            while (AppManager.Instance.camManager.eCameraInitialisationState == CamManager.ECameraInitialisationState.PENDING)
            {
            	yield return new WaitForSeconds(0.5f);
            }
			if (AppManager.Instance.camManager.eCameraInitialisationState == CamManager.ECameraInitialisationState.READY)
			{
				Init();
			}
        }

		private void SetOrientation(ScreenOrientation orientation)
		{
			int rightAngles = UIManager.ScreenOrientation2RightAngles( orientation );

            if (DEBUG_LOCAL)
            {
                Debug.Log("\nCamPanel.SetOrientation( " + orientation + ") = " + rightAngles + " right angles"
                    + "\nWCT: vra=" + AppManager.Instance.camManager.webCamTexture.videoRotationAngle
                    + "\n     vvm=" + AppManager.Instance.camManager.webCamTexture.videoVerticallyMirrored + "\n");
            }
						
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

            _camImageTransform.localScale = Vector3.one;

            if (AppManager.Instance.camManager.webCamTexture.videoVerticallyMirrored)
            {
                camImage.uvRect = normalUVRect;
            }
            else
            {
                camImage.uvRect = flippedYUVRect;
            }

            if (DEBUG_LOCAL)
            {
                Debug.Log("\nSet camImage size = " + _camImageRT.sizeDelta
                    + ", cam texture size = " + AppManager.Instance.camManager.webCamTexture.width + ", " + AppManager.Instance.camManager.webCamTexture.height);
            }
        }

		public void HandleCamUpdate(WebCamTexture webCamTexture)
		{
            camImage.texture = webCamTexture;
		}

		public void HandleCamSizeChanged()
		{
			SetOrientation( UIManager.Instance.screenOrientation );
		}

		public void ToggleCamImageSize()
		{
			if (_camImageRT.transform.localScale.x != 1f) // fp comparison ok here because we set it
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
