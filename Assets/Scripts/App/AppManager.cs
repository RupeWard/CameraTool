using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.DebugDescribable;

namespace CX.CamTool
{
	public class AppManager : Core.Singleton.SingletonSceneLifetime< AppManager >
	{
		public float initialisationFailTime = 20f;
		public CamManager camManager
		{
			get;
			private set;
		} 

		public GyroManager gyroManager
		{
			get;
			private set;
		}

		protected override void PostAwake()
		{
			GameObject camManagerGO = new GameObject("CamManager");
			camManagerGO.transform.SetParent(this.transform);
			camManager = camManagerGO.AddComponent<CamManager>();

			gyroManager = new GyroManager( );

			camManager.Init(initialisationFailTime, null);
		}
	}

}
