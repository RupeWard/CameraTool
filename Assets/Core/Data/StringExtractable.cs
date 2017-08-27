using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Core.DebugDescribable;
using System;
using System.Text;

namespace Core.Data
{
	public abstract class AbstractStringExtractable< T >
	{
		static protected bool DEBUG_AbstractStringExtractable = true;

		abstract protected bool DebugType( );

		protected bool _extractFromString( ref string str)
		{
			return _extractFromString( ref str, ref Value );
		}

		abstract protected bool _extractFromString( ref string str, ref T result);

		protected bool _addToString( System.Text.StringBuilder sb )
		{
			return _addToString( Value, sb );
		}
		abstract protected bool _addToString( T target, System.Text.StringBuilder sb );

		public T Value;

		private string _sep = "";
		private string sep
		{
			get { return _sep; }
			set
			{
				if (value.Length < 2)
				{
					throw new System.Exception( "Can't make seps from single character " + value );
				}
				else
				{
					_sep = value;
				}
			}
		}

		public bool hasBeenSet
		{
			get;
			protected set;
		}

		public void Reset()
		{
			hasBeenSet = false;
		}

		private int sepLength
		{
			get
			{
				if (_sep.Length > 2)
				{
					if ( (_sep.Length & 1) == 0)
					{
//						Debug.LogWarning( "SepLength for " + _sep + " halved = " + _sep.Length/2 );
						return _sep.Length / 2;
					}
					else
					{
//						Debug.LogWarning( "SepLength for " + _sep + " = " + _sep.Length );
						return _sep.Length;
					}
				}
				else if (_sep.Length == 2)
				{
//					Debug.LogWarning( "SepLength for " + _sep + " = 1");
					return 1;
				}
				else
				{
					throw new System.Exception( "Invalid seps = '" + _sep + "'" );
				}
			}
		}

		protected string sep0
		{
			get
			{
				return _sep.Substring( 0, sepLength );
			}
		}

		protected string sep1
		{
			get
			{
				string result = _sep.Substring( _sep.Length - sepLength, sepLength );
				if (_sep.Length == sepLength)
				{
					char[] arr = result.ToCharArray( );
					System.Array.Reverse( arr );
					result = new string( arr );
				}
				return result;
			}
		}

		protected AbstractStringExtractable( string s )
		{
			sep = s;
		}

		protected AbstractStringExtractable( ) // using default separators
		{
			sep = "{}";
		}

		/*
		private string ENDS_REGEX = string.Empty;
		
		private void HandleSepsSet( )
		{
			Sep1StartRegex = @"^"+sep0;
		}
		
		private string Sep1StartRegex;
		*/

		public void AddToString( System.Text.StringBuilder sb )
		{
			sb.Append( sep0 );
			_addToString( sb );
			sb.Append( sep1 );
		}

		public bool ExtractOptionalFromString( ref string str)
		{
			return (ExtractFromString( ref str, false ));
		}

		public bool ExtractRequiredFromString( ref string str)
		{
			return (ExtractFromString( ref str, true));
		}

		private static readonly bool DEBUG_Parsing = false;

		public bool ExtractFromString( ref string str, bool required )
		{
			bool success = false;

			// FIXME find matching!

			if (str.Length < sep0.Length+sep1.Length)
			{
				Debug.Log( "Can't extract with seps " + sep0 + " " + sep1 + " as str too short '" + str + "'" );
			}
			else
			{
				//			string regexStr = @"^(" + ENDS_REGEX + @")";
				//            System.Text.RegularExpressions.Regex regex =
				//				new System.Text.RegularExpressions.Regex( Sep1StartRegex );
				//			System.Text.RegularExpressions.Match match = regex.Match( str );
				if (str.StartsWith( sep0 ))
				{
					string prevStr = str;

					//Found first. Now search forward for matching

					if (DEBUG_Parsing)
					{
						Debug.Log( "Found " + sep0 + " at start of " + str );
					}
	
					int index = sep0.Length;

					string contentString = string.Empty;

					int numNestedNotMatched = 0;
					while (!success && index < (str.Length - sep1.Length + 1))
					{
						string testString = str.Substring( index, sep1.Length );
						if (testString.StartsWith( sep0 ))
						{
							if (DEBUG_Parsing)
							{
								Debug.Log( "Found " + sep0 + " at " + index + " (" + numNestedNotMatched + ")" );
							}
							numNestedNotMatched++;
							index = index + sep0.Length;
						}
						else if (testString.StartsWith( sep1 ))
						{
							if (numNestedNotMatched > 0)
							{
								if (DEBUG_Parsing)
								{
									Debug.Log( "Found " + sep1 + " at " + index + " (" + numNestedNotMatched + ")" );
								}
								numNestedNotMatched--;
								index = index + sep1.Length;
							}
							else
							{
								contentString = str.Substring( sep0.Length, index - sep0.Length );
								string msg = "Found matching " + sep1 + " for initial " + sep0 + " in '" + str + "'"
									+ "\nContent = '" + contentString + "'";
								int contentLength = contentString.Length;
								if (_extractFromString( ref contentString ))
								{
									int deleteLength = sep0.Length + contentLength + sep1.Length;
									if (deleteLength > str.Length)
									{
										Debug.LogError( "deleteLength " + deleteLength + " strLen=" + str.Length );
										str = string.Empty;
									}
									else if (deleteLength == str.Length)
									{
										str = string.Empty;
									}
									else
									{
										str = str.Substring( deleteLength );
									}
									msg = msg + "'\nRemaining = '" + str + "'";

									msg = msg + "\n" + GetType( ) + " Extracted from '" + prevStr;
									IDebugDescribable dd = this as IDebugDescribable;
									if (dd != null)
									{
										msg = msg + "\n" + dd.DebugDescribe( );
									}
									if (DEBUG_Parsing)
									{
										Debug.Log( msg );
									}
									success = true;
								}
								else
								{
									Debug.LogWarning( "Failed to extract from " + contentString );
									index = str.Length;
								}

							}
						}
						else
						{
							index = index + 1;
						}
					}

					if (!success)
					{
						string warningStr = GetType( ) + " Failed to read " + typeof( T ) + " from \n" + str;
						if (required)
						{
							throw new System.Exception( warningStr );
						}
						else
						{
							if (DEBUG_AbstractStringExtractable || DebugType( ))
							{
								Debug.LogWarning( warningStr );
							}
						}

					}
				}
				else
				{
					Debug.LogWarning( "Doesn't start with " + sep0 + " : " + str );
				}

			}
			return success;
		}

	}

	/*
	public class ListExtractable< TExtractableType >: AbstractStringExtractable<ListExtractable<TExtractableType>>
	{
		private string _separator = ",";
		private int _num = 0;

		public List< >
	}
	*/

	public class ListExtractable<T, TEmbedded >: AbstractStringExtractable< ListExtractable <T, TEmbedded> > where T : AbstractStringExtractable<TEmbedded>, new()
	{
		private static readonly bool DEBUG_ListExtractable = true;

		private List<TEmbedded> _list = new List<TEmbedded>( );

		private IntExtractable numExtractable = new IntExtractable( );
		private T itemExtractable = new T( );

		public ListExtractable( ) : base(  )
		{
		}

		public ListExtractable(string s ): base(s)
		{
		}

		public ListExtractable(IEnumerable<TEmbedded> ts):base()
		{
			_list.AddRange( ts );
		}

		public void ConsumeList( List<TEmbedded> dest)
		{
			dest.AddRange( _list );
			_list.Clear( );
		}

		protected override bool DebugType( )
		{
			return DEBUG_ListExtractable;
		}

		protected override bool _addToString( ListExtractable<T, TEmbedded> target, StringBuilder sb )
		{
			bool success = false;
			if (target == null)
			{
				target = this;
			}
			numExtractable.Value = target._list.Count;
			numExtractable.AddToString( sb );
			if (target._list.Count > 0)
			{
				foreach (TEmbedded t in target._list)
				{
					itemExtractable = new T( );
					itemExtractable.Value = t;
					itemExtractable.AddToString( sb );
				}
			}
			success = true;

			return success;
		}

		protected override bool _extractFromString( ref string str, ref ListExtractable<T, TEmbedded> result )
		{
			bool success = false;

			if (result == null)
			{
				result = this;
			}

			int num = 0;
			if (numExtractable.ExtractOptionalFromString(ref str))
			{
				success = true;
				num = numExtractable.Value;
				List<TEmbedded> newList = new List<TEmbedded>( );
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						itemExtractable = new T( );
						if (itemExtractable.ExtractOptionalFromString(ref str))
						{
							newList.Add( itemExtractable.Value );
							string msg = string.Empty;
							if (itemExtractable is IDebugDescribable)
							{
								msg = (itemExtractable as IDebugDescribable).DebugDescribe( );
                            }
							else
							{
								msg = itemExtractable.Value.ToString( );
							}
							Debug.LogWarning( "Read item " + i + " in list of " + num + " of type " + typeof( T )+ " = "+msg );
						}
						else
						{
							Debug.LogWarning( "Failed to read item " + i + " in list of " + num + " of type " + typeof( T ) );
						}
					}
				}
				result._list = newList;
				Debug.Log( "Extracted list of type " + typeof( T ) + " with " + result._list.Count + " loaded of " + num );
			}
			return success;
		}
	}

	public class TextExtractable: AbstractStringExtractable<string>
	{
		private static readonly bool DEBUG_TextExtractable = true;

		/*
		public string Value
		{
			get;
			set;
		}
		*/

		public TextExtractable( string v, string sep ) : base( sep )
		{
			Value = v;
		}

		public TextExtractable( string v) : base( )
		{
			Value = v;
		}

		public TextExtractable( ) : base()
		{
		}

		override protected bool DebugType( )
		{
			return DEBUG_TextExtractable || DEBUG_AbstractStringExtractable;
		}
	
		// Not - this one assumes the whole string is needed. The bounds of the string are determined at a higher level
		override protected bool _extractFromString( ref string str, ref string result)
		{
			Debug.Log( "TextExtractor.extractFromString (" + str+ ")" );
			result = str;
			Debug.Log( "TextExtractor got '" + Value + "'" );
			return true;
		}

		override protected bool _addToString( string target, System.Text.StringBuilder sb )
		{
			sb.Append( target );
			return true;
		}


	}

	public class FloatExtractable : AbstractStringExtractable<float>
	{
		private static readonly bool DEBUG_FloatExtractable = true;

		public FloatExtractable( float v, string sep ) : base( sep )
		{
			Value = v;
		}

		public FloatExtractable( float v ) : base(  )
		{
			Value = v;
		}

		public FloatExtractable( ) : base(  )
		{
		}

		override protected bool DebugType( )
		{
			return DEBUG_FloatExtractable || DEBUG_AbstractStringExtractable;
		}

		override protected bool _extractFromString( ref string str, ref float result )
		{
			string prevStr = str;
			bool success = false;
			float parseVal=0f;

			if (DataHelpers.extractFloat(ref str, ref parseVal, false))
			{
				Debug.Log( "Got float "+parseVal+ " from '" + prevStr + "' leaving '"+str+"'" );
				result = parseVal;
//				str = string.Empty;
				success = true;
			}
			else
			{
				Debug.LogError( "Couldn't parse float from '" + str + "'" );
			}
			return success;
		}

		override protected bool _addToString(float target, System.Text.StringBuilder sb )
		{
			sb.Append( target );
			return true;
		}


	}

	public class IntExtractable : AbstractStringExtractable<int>
	{
		private static readonly bool DEBUG_IntExtractable = true;

		public IntExtractable( int v, string sep ) : base( sep )
		{
			Value = v;
		}

		public IntExtractable( int v ) : base( )
		{
			Value = v;
		}

		public IntExtractable( ) : base( )
		{
		}

		override protected bool DebugType( )
		{
			return DEBUG_IntExtractable || DEBUG_AbstractStringExtractable;
		}

		override protected bool _extractFromString( ref string str, ref int result )
		{
			string prevStr = str;
			bool success = false;
			int parseVal = 0;

			if (DataHelpers.extractInt( ref str, ref parseVal, false ))
			{
				Debug.Log( "Got int " + parseVal + " from "+prevStr+" leaving '" + str + "'" );
				result = parseVal;
				//				str = string.Empty;
				success = true;
			}
			else
			{
				Debug.LogError( "Couldn't parse int from '" + str + "'" );
			}
			return success;
		}

		override protected bool _addToString( int target, System.Text.StringBuilder sb )
		{
			sb.Append( target );
			return true;
		}


	}

	static public class StringExtractableHelpers
	{

	}
}
