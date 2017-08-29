﻿using UnityEngine;
using System.Collections;

namespace Core.Version
{
	public static partial class Version
	{
		// Change this to match whenever updating version number in build settings (and vice versa)
		// Put a 'D' at the start to make a Dev version 
		public static VersionNumber versionNumber = new VersionNumber( "D0.0.1 (23)" );
	}
}