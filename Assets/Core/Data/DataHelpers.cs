using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWS.Core.Data
{
	public static class DataHelpers
	{
		private static bool DEBUG_LOCAL = false;

		public static readonly string FLOAT_REGEX = @"[-+]?[0-9]*\.?[0-9]+";


		#region Vector4

		static public List<Vector4> extractVector4List( ref string str )
		{
			List<Vector4> result = new List<Vector4>( );
			Vector4 v = Vector4.zero;

			while (extractOptionalVector4( ref str, ref v ))
			{
				result.Add( v );
			}
			return result;
		}

		static public bool extractRequiredVector4( ref string str, ref Vector4 v )
		{
			return extractVector4( ref str, ref v, true );
		}

		static public bool extractOptionalVector4( ref string str, ref Vector4 v )
		{
			return extractVector4( ref str, ref v, false );
		}

		static public bool extractVector4( ref string str, ref Vector4 v, bool required )
		{
			bool success = false;
			System.Text.RegularExpressions.Regex regex =
				new System.Text.RegularExpressions.Regex( @"^(\((" + FLOAT_REGEX + @"),\s*(" + FLOAT_REGEX + @"),\s*(" + FLOAT_REGEX + @"),\s*(" + FLOAT_REGEX + @")\))" );
			System.Text.RegularExpressions.Match match = regex.Match( str );
			if (match.Success && match.Groups.Count == 6)
			{
				string matched = match.Groups[1].Value;
				string sx = match.Groups[2].Value;
				string sy = match.Groups[3].Value;
				string sz = match.Groups[4].Value;
				string ss = match.Groups[5].Value;
				//			string remainder = match.Groups[4].Value;
				float x, y, z, s;
				if (float.TryParse( sx, out x ) && float.TryParse( sy, out y ) && float.TryParse( sz, out z ) && float.TryParse( ss, out s ))
				{
					v = new Vector4( x, y, z, s );
					string prevStr = str;
					str = str.Replace( matched, "" );
					if (DEBUG_LOCAL)
					{
						Debug.Log( "Extracted vector 4 from '" + prevStr + "' = " + v + "\nRemainder = '" + str + "'" );
					}
					success = true;
				}
				else
				{
					Debug.LogError( "Couldn't parse floats from '" + sx + "' '" + sy + "' '" + sz + "' '" + ss + "'" );
				}
			}
			else
			{
				if (required)
				{
					Debug.LogError( "Furniture: Couldn't parse Vector4 from '" + str + "'" );
				}
			}
			return success;
		}

		#endregion Vector4



		#region Vector3

		static public List<Vector3> extractVector3List( ref string str )
		{
			List<Vector3> result = new List<Vector3>( );
			Vector3 v = Vector3.zero;

			while (extractOptionalVector3( ref str, ref v ))
			{
				result.Add( v );
			}
			return result;
		}

		static public bool extractRequiredVector3( ref string str, ref Vector3 v )
		{
			return extractVector3( ref str, ref v, true );
		}

		static public bool extractOptionalVector3( ref string str, ref Vector3 v )
		{
			return extractVector3( ref str, ref v, false );
		}

		static public bool extractVector3( ref string str, ref Vector3 v, bool required )
		{
			bool success = false;
			System.Text.RegularExpressions.Regex regex =
				new System.Text.RegularExpressions.Regex( @"^(\((" + FLOAT_REGEX + @"),\s*(" + FLOAT_REGEX + @"),\s*(" + FLOAT_REGEX + @")\))" );
			System.Text.RegularExpressions.Match match = regex.Match( str );
			if (match.Success && match.Groups.Count == 5)
			{
				string matched = match.Groups[1].Value;
				string sx = match.Groups[2].Value;
				string sy = match.Groups[3].Value;
				string sz = match.Groups[4].Value;
				//			string remainder = match.Groups[4].Value;
				float x, y, z;
				if (float.TryParse( sx, out x ) && float.TryParse( sy, out y ) && float.TryParse( sz, out z ))
				{
					v = new Vector3( x, y, z );
					string prevStr = str;
					str = str.Replace( matched, "" );
					if (DEBUG_LOCAL)
					{
						Debug.Log( "Extracted vector 3 from '" + prevStr + "' = " + v + "\nRemainder = '" + str + "'" );
					}
					success = true;
				}
				else
				{
					Debug.LogError( "Couldn't parse floats from '" + sx + "' '" + sy + "' '" + sz + "'" );
				}
			}
			else
			{
				if (required)
				{
					Debug.LogError( "Furniture: Couldn't parse Vector3 from '" + str + "'" );
				}
			}
			return success;
		}

		#endregion Vector3

		#region Vector2

		static public List<Vector2> extractVector2List( ref string str )
		{
			List<Vector2> result = new List<Vector2>( );
			Vector2 v = Vector3.zero;

			while (extractOptionalVector2( ref str, ref v ))
			{
				result.Add( v );
			}
			return result;
		}

		static public bool extractRequiredVector2( ref string str, ref Vector2 v )
		{
			return extractVector2( ref str, ref v, true );
		}

		static public bool extractOptionalVector2( ref string str, ref Vector2 v )
		{
			return extractVector2( ref str, ref v, false );
		}

		static public bool extractVector2( ref string str, ref Vector2 v, bool required )
		{
			bool success = false;
			System.Text.RegularExpressions.Regex regex =
				new System.Text.RegularExpressions.Regex( @"^(\((" + FLOAT_REGEX + @"),\s*(" + FLOAT_REGEX + @")\))" );
			System.Text.RegularExpressions.Match match = regex.Match( str );
			if (match.Success && match.Groups.Count == 4)
			{
				string matched = match.Groups[1].Value;
				string sx = match.Groups[2].Value;
				string sy = match.Groups[3].Value;
				//			string remainder = match.Groups[3].Value;
				float x, y;
				if (float.TryParse( sx, out x ) && float.TryParse( sy, out y ))
				{
					string prevStr = str;
					str = str.Replace( matched, "" );
					v = new Vector2( x, y );
					if (DEBUG_LOCAL)
					{
						Debug.Log( "Extracted Vector2 from '" + prevStr + " = " + v + "\nRemainder = '" + str + "'" );
					}
					success = true;
				}
				else
				{
					Debug.LogError( "Couldn't parse floats from '" + sx + "' '" + sy + "'" );
				}
			}
			else
			{
				if (required)
				{
					Debug.LogError( "Furniture: Couldn't parse Vector2 from '" + str + "'" );
				}
			}
			return success;
		}

		#endregion Vector2

		#region Float

		static public List< float > extractFloatList( ref string str )
		{
			List< float > result = new List< float >( );
			float v = 0f;

			while (extractOptionalFloat( ref str, ref v ))
			{
				result.Add( v );
				if (str.StartsWith(","))
				{
					str = str.Replace( ",","" );
				}
				else
				{
					break;
				}
			}
			return result;
		}


		static public bool extractRequiredFloat( ref string str, ref float v )
		{
			return extractFloat( ref str, ref v, true );
		}

		static public bool extractOptionalFloat( ref string str, ref float v )
		{
			return extractFloat( ref str, ref v, false );
		}

		static public bool extractFloat( ref string str, ref float v, bool required )
		{
			bool success = false;
			System.Text.RegularExpressions.Regex regex =
				new System.Text.RegularExpressions.Regex( @"^(" + FLOAT_REGEX + @")" );
			System.Text.RegularExpressions.Match match = regex.Match( str );
			if (match.Success && match.Groups.Count == 2)
			{
				string fStr = match.Groups[1].Value;
				float f;
				if (float.TryParse( fStr, out f ) )
				{
					string prevStr = str;
					str = str.Replace( fStr, "" );
					v = f;
					if (DEBUG_LOCAL)
					{
						Debug.Log( "Extracted Float "+v+" from '" + prevStr + " = " + v + "\nRemainder = '" + str + "'" );
					}
					success = true;
				}
				else
				{
					Debug.LogError( "Couldn't parse float from '" + fStr + "' '" );
				}
			}
			else
			{
				if (required)
				{
					Debug.LogError( "Couldn't parse float from '" + str + "'" );
				}
			}
			return success;
		}

		#endregion Float

		#region Int

		static public List<int> extractIntList( ref string str )
		{
			List<int> result = new List<int>( );
			int v = 0;

			while (extractOptionalInt( ref str, ref v ))
			{
				result.Add( v );
				if (str.StartsWith( "," ))
				{
					str = str.Replace( ",", "" );
				}
				else
				{
					break;
				}
			}
			return result;
		}


		static public bool extractRequiredInt( ref string str, ref int v )
		{
			return extractInt( ref str, ref v, true );
		}

		static public bool extractOptionalInt( ref string str, ref int v )
		{
			return extractInt( ref str, ref v, false );
		}

		static public bool extractInt( ref string str, ref int v, bool required )
		{
			bool success = false;
			System.Text.RegularExpressions.Regex regex =
				new System.Text.RegularExpressions.Regex( @"^(\d+)" );
			System.Text.RegularExpressions.Match match = regex.Match( str );
			if (match.Success && match.Groups.Count == 2)
			{
				string iStr = match.Groups[1].Value;
				int f;
				if (int.TryParse( iStr, out f ))
				{
					string prevStr = str;
					str = str.Replace( iStr, "" );
					v = f;
					if (DEBUG_LOCAL)
					{
						Debug.Log( "Extracted Int "+v+" from '" + prevStr + " = " + v + "\nRemainder = '" + str + "'" );
					}
					success = true;
				}
				else
				{
					Debug.LogError( "Couldn't parse float from '" + iStr + "' '" );
				}
			}
			else
			{
				if (required)
				{
					Debug.LogError( "Couldn't parse float from '" + str + "'" );
				}
			}
			return success;
		}

		#endregion int

		#region bool

		static public bool extractRequiredBool( ref string str, ref bool v )
		{
			return extractBool( ref str, ref v, true );
		}

		static public bool extractOptionalBool( ref string str, ref bool v )
		{
			return extractBool( ref str, ref v, false );
		}

		static public bool extractBool( ref string str, ref bool v, bool required )
		{
			bool success = false;
			if (str.StartsWith(true.ToString()))
			{
				v = true;
				string prevStr = str;
				str = str.Replace( true.ToString(),"" );
				success = true;
				if (DEBUG_LOCAL)
				{
					Debug.Log( "Extracted Bool " + v + " from '" + prevStr + "\nRemainder = '" + str + "'" );
				}
			}
			else if (str.StartsWith( false.ToString() ))
			{
				v = true;
				string prevStr = str;
				str = str.Replace( false.ToString(), "" );
				success = true;
				if (DEBUG_LOCAL)
				{
					Debug.Log( "Extracted Bool " + v + " from '" + prevStr + "\nRemainder = '" + str + "'" );
				}
			}
			else
			{
				if (required)
				{
					Debug.LogError( "Couldn't parse bool from '" + str + "'" );
				}
			}
			return success;
		}

		#endregion bool

		#region text

		static public bool extractRequiredText( ref string str, ref string v )
		{
			return extractText( ref str, ref v, true );
		}

		static public bool extractOptionalText( ref string str, ref string v )
		{
			return extractText( ref str, ref v, false );
		}

		static public bool extractText( ref string str, ref string v, bool required )
		{
			bool success = false;

			v = str;
			str = string.Empty;
			success = true;

			return success;
		}

#endregion text


	}
}
