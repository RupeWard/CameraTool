using UnityEngine;
using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using RJWS.Core.DebugDescribable;

namespace RJWS.Core.Data
{
	public partial class SqliteUtils : RJWS.Core.Singleton.SingletonApplicationLifetimeLazy<SqliteUtils>
	{
		public static readonly bool DEBUG_SQL = true;

		private Dictionary<string, SqliteConnection> storedConnections_ = new Dictionary<string, SqliteConnection>( );
		static private string databaseFolder_ = "";
		static public string databaseFolder
		{
			get
			{
				if (databaseFolder_.Length == 0)
				{
					databaseFolder_ = Application.persistentDataPath + "/Data";
				}
				return databaseFolder_;
			}
		}

		public Action databaseLoadComplete;

		private List<string> databaseList_;
		private int numLoadedDatabases_;
		//	private static string language;

		private RJWS.Core.Version.Version.VersionNumber previousVersionNumber_ = null;

		protected override void PostAwake( )
		{
		}

		private static string getDatabaseFilename( string databaseName )
		{
			return databaseName + ".db";
		}

		public static string getDatabasePath( string databaseName )
		{
			return databaseFolder + "/" + getDatabaseFilename( databaseName );
		}

		public static string streamingAssetsPath
		{
			get
			{
				if (Application.platform == RuntimePlatform.Android)
				{
					return "jar:file://" + Application.dataPath + "!/assets";
				}
				else
				{
					return "file://" + Application.streamingAssetsPath;
				}
			}
		}

		protected override void PostOnDestroy( )
		{
			foreach (KeyValuePair<string, SqliteConnection> pair in storedConnections_)
			{
				if (DEBUG_SQL)
				{
					Debug.Log( "SQL: closing DB " + pair.Key );
				}
				pair.Value.Close( );
				pair.Value.Dispose( );
			}
			storedConnections_.Clear( );
		}


		public static SqliteParameter newSqlParameter( object value )
		{
			SqliteParameter sqlParameter = new SqliteParameter( );
			sqlParameter.Value = value;
			return sqlParameter;
		}

		public SqliteConnection getConnection( string databaseName )
		{
			if (storedConnections_.ContainsKey( databaseName ))
			{
				if (DEBUG_SQL)
				{
					Debug.Log( "SqliteUtils found connection to '" + databaseName + "'" );
				}
				return storedConnections_[databaseName];
			}

			SqliteConnection connection;
			connection = new SqliteConnection( "URI=file:" + getDatabasePath( databaseName ) );
			connection.Open( );
			storedConnections_[databaseName] = connection;
			if (DEBUG_SQL)
			{
				Debug.Log( "SqliteUtils created connection to '" + databaseName + "'" );
			}
			return connection;
		}

		static public bool DeleteDB(string dbName)
		{
			bool success = false;
			string fName = getDatabasePath(dbName);

			System.IO.FileInfo fileInfo = new System.IO.FileInfo( fName );
			if (fileInfo.Exists)
			{
				fileInfo.Delete( );
				success = true;
				Debug.Log( "Deleted DB file " + fName );
			}
			else
			{
				Debug.LogWarning( "Delete DB file couldn't find " + fName );
			}
			return success;
		}

		public bool doesTableExist( string dbName, string tableName )
		{
			SqliteConnection connection = getConnection( dbName );

			SqliteCommand existsCommand = connection.CreateCommand( );
			existsCommand.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name = '" + tableName + "'";
			bool exists = (existsCommand.ExecuteScalar( ) != null);
			existsCommand.Dispose( );
			return exists;
		}

		public void initialiseDatabases( string language )
		{
			if (DEBUG_SQL)
			{
				Debug.Log( "SQL: initialiseDatabases( " + language + " ): Language not implemented" );
			}

			if (!Directory.Exists( databaseFolder ))
			{
				if (DEBUG_SQL)
				{
					Debug.Log( "Creating database folder '" + databaseFolder + "'" );
				}
				Directory.CreateDirectory( databaseFolder );
			}

			//		SqliteUtils.language = language;
			getOriginalSettings( );

			if (previousVersionNumber_ == null)
			{
				Debug.LogError( "null previous version number" );
			}
			if (RJWS.Core.Version.Version.DEBUG_VERSION)
			{
				Debug.Log( "THIS VERSION = " + RJWS.Core.Version.Version.versionNumber.DebugDescribe( )
						  + "\nPREVIOUS = " + previousVersionNumber_.DebugDescribe( )
						  + " (BEFORE = " + previousVersionNumber_.Before( RJWS.Core.Version.Version.versionNumber ) + " )" );
			}

			databaseList_ = new List<string>( );

			if (previousVersionNumber_.Before( RJWS.Core.Version.Version.versionNumber ))
			{
				//			databaseList.Add( language );
				if (DEBUG_SQL)
				{
					Debug.Log( "Previous version is older so copying DBs from StreamingAssets" );
				}
				databaseList_.Add( "CoreData" );
			}
			else
			{
				if (!File.Exists( getDatabasePath( "CoreData" ) ))
				{
					if (DEBUG_SQL)
					{
						Debug.Log( "CoreData does not exist so copying DBs from StreamingAssets" );
					}
					databaseList_.Add( "CoreData" );
				}
				else
				{
					if (DEBUG_SQL)
					{
						Debug.Log( "DB file '" + getDatabasePath( "CoreData" ) + "' exists and version is not new" );
					}
				}
				/*
				if (!File.Exists( Application.persistentDataPath + "/Data/" + language + ".db" ))
				{
					databaseList.Add( language );
				}
				*/
			}

			if (databaseList_.Count > 0)
			{
				// replace databases with those in bundle
				numLoadedDatabases_ = 0;
				if (DEBUG_SQL)
				{
					Debug.Log( "Copying " + databaseList_.Count + " DBs from StreamingAssets" );
				}

				for (int i = 0; i < databaseList_.Count; i++)
				{
					StartCoroutine( copyDatabaseFromBundle( databaseList_[i] ) );
				}
			}
			else
			{
				databaseFilesCopied( );
			}
		}

		private void getOriginalSettings( )
		{
			if (DEBUG_SQL)
			{
				Debug.Log( "SQL: getOriginalSettings()" );
			}

			// check if settings exists

			string progressPath = getDatabasePath( "Progress" );
			if (File.Exists( progressPath ))
			{
				SqliteConnection connection = new SqliteConnection( "URI=file:" + progressPath );
				connection.Open( );
				SqliteCommand query = connection.CreateCommand( );

				query.CommandText = "SELECT value FROM settings WHERE id=?";
				SqliteParameter valueSql = new SqliteParameter( );
				query.Parameters.Add( valueSql );

				SqliteDataReader reader = null;

				valueSql.Value = SettingsIds.versionNumber;
				reader = query.ExecuteReader( );

				string previousVersionString = string.Empty;

				if (reader.Read( ))
				{
					previousVersionString = reader.GetString( 0 );
					previousVersionNumber_ = new RJWS.Core.Version.Version.VersionNumber( previousVersionString );
				}
				reader.Close( );

				query.Dispose( );

				checkProgressTableDefaults( );
			}
			else
			{
				Debug.LogWarning( "No settings table in getOriginalSettings" );
				prepareProgressTable( );
			}
			if (previousVersionNumber_ == null)
			{
				previousVersionNumber_ = new RJWS.Core.Version.Version.VersionNumber( 0, 0, 0, 0 );
				Debug.Log( "No previousVersionNumber, defaulting to " + previousVersionNumber_.ToString( ) );
			}
		}

		private IEnumerator copyDatabaseFromBundle( string databaseName )
		{
			if (DEBUG_SQL)
			{
				Debug.Log( "SQL: copyDatabaseFromBundle( " + databaseName + " )" );
			}

			string dbPath = getDatabasePath( databaseName );

			if (File.Exists( dbPath ))
			{
				File.Delete( dbPath );
			}

			WWW wwwFile = new WWW( streamingAssetsPath + "/Data/" + getDatabaseFilename( databaseName ) );
			yield return wwwFile;

			//Save to persistent data path
			File.WriteAllBytes( dbPath, wwwFile.bytes );

#if UNITY_IPHONE
		iPhone.SetNoBackupFlag( dataBasePath );
#endif

			//Check if we have loaded all databases
			numLoadedDatabases_++;
			if (numLoadedDatabases_ == databaseList_.Count)
			{
				databaseFilesCopied( );
			}
		}

		private void databaseFilesCopied( )
		{
			numLoadedDatabases_ = 0;
			onOpenConnections( );
		}

		private void onOpenConnections( )
		{
			// Debug.Log ("onOpenConnections");

			//		SqliteUtils.Instance.getConnection( language );
			SqliteUtils.Instance.getConnection( "CoreData" );
			SqliteUtils.Instance.getConnection( "Progress" );

			prepareProgressTable( );

			if (databaseLoadComplete != null)
			{
				databaseLoadComplete( );
			}
		}




	}

}
