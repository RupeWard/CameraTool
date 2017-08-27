using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.DebugDescribable;

namespace CX.CamTool
{
	public class AppManager : Core.Singleton.SingletonSceneLifetime< AppManager >
	{
		public CamManager camManager
		{
			get;
			private set;
		} 

		protected override void PostAwake()
		{
			camManager = new CamManager( );
		}

		private void Update()
		{
			camManager.DoUpdate( );
		}

		
	}

}
