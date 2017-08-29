using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.DebugDescribable;

namespace CX.CamTool
{
    public class CamManager : MonoBehaviour, IDebugDescribable
    {
        static public readonly bool DEBUG_CAMMANAGER = true;

        public System.Action<WebCamTexture> onCamUpdate;
        public System.Action onCamImageSizeChanged;

        public int[] requestedResolution = new int[2] { 2000, 2000 };

        public WebCamTexture webCamTexture
        {
            get;
            private set;
        }

        public enum ECameraInitialisationState
        {
            PENDING,
            READY,
            FAILED
        }

        public ECameraInitialisationState eCameraInitialisationState
        {
            get;
            private set;
        }

        public int[] _doNotAccessCamImageSize = null;

        public int[] camImageSize
        {
            get
            {
                if (_doNotAccessCamImageSize == null)
                {
                    _doNotAccessCamImageSize = new int[2];
                }
                return _doNotAccessCamImageSize;
            }
        }

        public float camImageWidth
        {
            get 
            {
                return camImageSize[0];
            }
        }

        public float camImageHeight
        {
            get
            {
                return camImageSize[1];
            }
        }

        private bool SetCamImageSize(int w, int h)
        {
            bool changed = false;
            if (camImageSize[0] != w)
            {
                camImageSize[0] = w;
                changed = true;
            }
            if (camImageSize[1] != h)
            {
                camImageSize[1] = h;
                changed = true;
            }
            return changed;
        }

        private int _currentDeviceIndex = -1;

        private void Awake()
        {
        }

        public bool Init(float timeOutSecs, System.Action<bool> onCamInitialised)
        {
            bool success = true;

            webCamTexture = new WebCamTexture();
            _currentDeviceIndex = -1;

            eCameraInitialisationState = ECameraInitialisationState.PENDING;

            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                WebCamDevice device = WebCamTexture.devices[i];
                if (!device.isFrontFacing)
                {
                    webCamTexture.deviceName = device.name;
                    _currentDeviceIndex = i;
                }
            }

            if (_currentDeviceIndex == -1)
            {
                if (WebCamTexture.devices.Length == 1)
                {
#if UNITY_EDITOR
                    if (DEBUG_CAMMANAGER)
                    {
                        Debug.Log("CamManager using front camera: expected in editor)");
                    }
#else
                    Debug.LogWarning("CamManager only found one camera (front-facing), using it regardless!");
#endif
                    WebCamDevice device = WebCamTexture.devices[0];
                    webCamTexture.deviceName = device.name;
                    _currentDeviceIndex = 0;
                }
                else
                {
                    Debug.LogError("CamManager FOUND NO CAMERA");
                    success = false;
                }
            }

            if (success)
            {
                webCamTexture.requestedWidth = requestedResolution[0];
                webCamTexture.requestedHeight = requestedResolution[1];
                if (DEBUG_CAMMANAGER)
                {
                    Debug.Log("CamPanel requesting " + requestedResolution[0] + "x" + requestedResolution[1] + "\n");
                }
                PlayCamera(true);
                camImageSize[0] = webCamTexture.width;
                camImageSize[1] = webCamTexture.height;
                StartCoroutine(WaitForCameraInitialisationCR(timeOutSecs, onCamInitialised));
            }
            return success;
		}

        private IEnumerator WaitForCameraInitialisationCR(float timeOutSecs, System.Action<bool> onCamInitialised)
        {
            MessageBus.Instance.SendConsoleMessage("Initialising Camera...", true, true);

            Debug.Log("CamManager: waiting for "+timeOutSecs+"s");
            float startTime = Time.time;

            bool bReady = false;

            while (!bReady && (Time.time - startTime) < timeOutSecs)
            {
                yield return new WaitForSeconds(1f);
                if (webCamTexture.width > 16)
                {
                    bReady = true;
                }
            }
            if (bReady)
            {
                eCameraInitialisationState = ECameraInitialisationState.READY;
                Debug.Log("CamManager: camera initialised after "+(Time.time-startTime)+"s at "+webCamTexture.width+"x"+webCamTexture.height
                          +"\nWith VVM="+webCamTexture.videoVerticallyMirrored+", VRA="+webCamTexture.videoRotationAngle);
                SetCamImageSize(webCamTexture.width, webCamTexture.height);
                MessageBus.Instance.SendConsoleMessage("Camera initialised at "+ webCamTexture.width + "x" + webCamTexture.height, true, true);
            }
            else
            {
                eCameraInitialisationState = ECameraInitialisationState.FAILED;
                Debug.LogError("CamManager: camera failed to initialise after " + (Time.time - startTime) + "s");
                MessageBus.Instance.SendConsoleMessage("CAMERA FAILED TO INITIALISE", true, true);
            }
            if (onCamInitialised != null)
            {
                onCamInitialised(bReady);
            }
            else
            {
                if (DEBUG_CAMMANAGER)
                {
                    Debug.Log("CamManager has no onCamInitialised callback defined");
                }
            }
            if (DEBUG_CAMMANAGER)
            {
                Debug.Log(this.DebugDescribe());
            }
        }

		public bool IsPlaying
		{
			get { return (webCamTexture != null && webCamTexture.isPlaying); }
		}

		public void Update( )
		{
            if (eCameraInitialisationState == ECameraInitialisationState.READY && IsPlaying && webCamTexture.didUpdateThisFrame)
			{
                if (SetCamImageSize(webCamTexture.width,webCamTexture.height)) 
				{
                    Debug.LogWarning( "WebCamTexture size changed while playing, to " + camImageWidth+"x"+camImageHeight);
                    MessageBus.Instance.SendConsoleMessage("WebCamTexture size changed while playing, to " 
                                                           +camImageWidth + "x" + camImageHeight, true, false);
					if (onCamImageSizeChanged != null)
					{
						onCamImageSizeChanged(  );
					}
				}
				if (onCamUpdate != null)
				{
					onCamUpdate( webCamTexture );
				}
			}
		}

		public void PlayCamera( bool b)
		{
			if (b)
			{
				webCamTexture.Play( );
			}
			else
			{
				webCamTexture.Pause( );
			}
		}

#region IDebugDescribable

		public void DebugDescribe( System.Text.StringBuilder sb)
		{
			sb.Append( "\nWebCamManager: " + WebCamTexture.devices.Length + " cameras" );
			if (WebCamTexture.devices.Length > 0)
			{
				sb.Append( ":" );
				for (int i = 0; i < WebCamTexture.devices.Length; i++)
				{
					WebCamDevice device = WebCamTexture.devices[i];
					sb.Append("\n").Append( i ).Append( ": '" ).Append( device.name ).Append( "'" );
					if (device.isFrontFacing)
					{
						sb.Append( " FRONT" );
					}
					else
					{
						sb.Append( " BACK" );
					}
					if (_currentDeviceIndex == i)
					{
						sb.Append( " (CURRENT)" );
					}
				}
				if (_currentDeviceIndex == -1)
				{
					sb.Append( "\nNO CURRENT DEVICE" );
	            }
			}
			sb.Append( "\n" );
		}
#endregion IDebugDescribable

	}
}

