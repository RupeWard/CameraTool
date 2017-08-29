using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CX.CamTool.UI
{
    public class GyroPanel : SlideOutUIPanel
	{
		public GameObject noGyroPanel;
		public GameObject yesGyroPanel;

		public UnityEngine.UI.Text attXDegText;
		public UnityEngine.UI.Text attYDegText;
		public UnityEngine.UI.Text attZDegText;

		private Gyroscope _gyro = null;
		private bool _isAvailable = false;

		protected override void DoInit(bool showing)
		{
			_isAvailable = SystemInfo.supportsGyroscope;
            if (_isAvailable)
			{
				_gyro = Input.gyro;

				_gyro.enabled = false;
				_gyro.enabled = true;
			}
			yesGyroPanel.SetActive( _isAvailable);
			noGyroPanel.SetActive( !_isAvailable );
		}

		private void Update()
		{
			if (_isAvailable && isShowing)
			{
				Vector3 attEuler = Input.gyro.attitude.eulerAngles;

				attXDegText.text = attEuler.x.ToString("0.00");
				attYDegText.text = attEuler.y.ToString( "0.00" );
				attZDegText.text = attEuler.z.ToString( "0.00" );
			}
		}

	}
}

