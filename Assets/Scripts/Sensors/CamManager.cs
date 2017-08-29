using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.DebugDescribable;

namespace CX.CamTool
{
    public class CamManager: MonoBehaviour, IDebugDescribable
	{
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

		private int _currentDeviceIndex = -1;

        private void Awake()
        {
        }

        public bool Init( float maxWaitTimeSecs, System.Action<bool> onCamInitialised)
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
                    Debug.LogWarning("CamManager Only found one camera, using it!");
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
                Debug.Log("\nCamPanel requesting " + requestedResolution[0] + "x" + requestedResolution[1] + "\n");

                Debug.Log(this.DebugDescribe());

                PlayCamera(true);
                camImageSize = new Vector2(webCamTexture.width, webCamTexture.height);
                StartCoroutine(WaitForCameraInitialisationCR(maxWaitTimeSecs, onCamInitialised));
            }
            return success;
		}

        private IEnumerator WaitForCameraInitialisationCR(float maxWaitTimeSecs, System.Action<bool> onCamInitialised)
        {
            Debug.Log("CamManager: waiting for "+maxWaitTimeSecs+"s");
            float startTime = Time.time;

            bool bReady = false;

            while (!bReady && (Time.time - startTime) < maxWaitTimeSecs)
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
                Debug.Log("CamManager: camera initialised after "+(Time.time-startTime)+"s at "+webCamTexture.width+"x"+webCamTexture.height);
            }
            else
            {
                eCameraInitialisationState = ECameraInitialisationState.FAILED;
                Debug.LogError("CamManager: camera failed to initialise after " + (Time.time - startTime) + "s");
            }
            if (onCamInitialised != null)
            {
                onCamInitialised(bReady);
            }
            else
            {
                Debug.LogWarning("CamManager has no onCamInitialised callback defined");
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
				if (camImageSize.x != webCamTexture.width || camImageSize.y != webCamTexture.height)
				{
					Vector2 newSize = new Vector2( webCamTexture.width, webCamTexture.height );
					Debug.LogWarning( "\nWebCamTexture size changed while playing from " + camImageSize
						+ " to " + newSize );
					camImageSize = newSize;
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

		public Vector2 camImageSize
		{
			get;
			private set;
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

