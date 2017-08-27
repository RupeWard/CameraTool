using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.DebugDescribable;

namespace CX.CamTool
{
	public class CamManager: IDebugDescribable
	{
		private WebCamTexture _webCamTexture = null;
		private int _currentDeviceIndex = -1;
		private UnityEngine.UI.RawImage _rawImage = null;
		Material _material = null;

		public CamManager( UnityEngine.UI.RawImage ri )
		{
			_rawImage = ri;
			_material = ri.material;

			_webCamTexture = new WebCamTexture( );

			for (int i = 0; i < WebCamTexture.devices.Length; i++)
			{
				WebCamDevice device = WebCamTexture.devices[i];
				if (device.isFrontFacing)
				{
//					_webCamTexture.deviceName = device.name;
					_currentDeviceIndex = i;
				}
			}
			Debug.Log( this.DebugDescribe( ) );

			PlayCamera( true );
		}

		public bool IsPlaying
		{
			get { return (_webCamTexture != null && _webCamTexture.isPlaying); }
		}

		public void DoUpdate( )
		{
			if (_rawImage != null)
			{
				if (IsPlaying)
				{
					_material.mainTexture = _webCamTexture;
				}
			}
		}

		public void PlayCamera( bool b)
		{
			if (b)
			{
				_webCamTexture.Play( );
			}
			else
			{
				_webCamTexture.Pause( );
			}
		}

		#region IDebugDescribable

		public void DebugDescribe( System.Text.StringBuilder sb)
		{
			sb.Append( "WebCamManager: " + WebCamTexture.devices.Length + " cameras" );
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

