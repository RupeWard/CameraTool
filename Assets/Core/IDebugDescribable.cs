using System;
using System.Collections;
using System.Collections.Generic;

namespace RJWS.Core.DebugDescribable
{
	public interface IDebugDescribable
	{
		void DebugDescribe( System.Text.StringBuilder sb );
	}

	public static class DebugDescribeExtensions
	{
		public static string DebugDescribe( this object obj)
		{
			if (obj == null)
			{
				return "[null]";
			}

			IDebugDescribable idd = obj as IDebugDescribable;
			if (idd != null)
			{
				return idd.DebugDescribe( );
			}
			return "["+obj.ToString( )+"]";
		}

		public static string DebugDescribe( this IDebugDescribable obj )
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder( );
			obj.DebugDescribe( sb );
			return sb.ToString( );
		}

		public static System.Text.StringBuilder DebugDescribe( this System.Text.StringBuilder sb, IDebugDescribable dd )
		{
			sb.Append( (dd == null) ? ("NULL") : (dd.DebugDescribe( )) );
			return sb;
		}

		public static void DebugDescribe<T>( this IEnumerable<T> coll, System.Text.StringBuilder sb )
		{
			sb.Append( "(" );
			bool first = true;
			foreach (T t in coll)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					sb.Append( ", " );
				}
				sb.Append( t.ToString( ) );
			}
			sb.Append( ")" );
		}
	}

}


