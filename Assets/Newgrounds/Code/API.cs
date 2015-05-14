#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newgrounds;
#endregion

//Code written by Adam R. Vierra
//adam@adamvierra.com
//http://coolboyman.newgrounds.com/

//namespace Newgrounds
//{
	public class API : MonoBehaviour 
	{	
		#region Constant Fields
		private const string m_newgroundsGateway = "http://www.ngads.com/gateway_v2.php";
		private const string m_testVars = "http://uploads.ungrounded.net/tmp/797000/797004/file/alternate/alternate_4.unity3d?35013&NewgroundsAPI_PublisherID=1&NewgroundsAPI_SandboxID=542b703459ca8&NewgroundsAPI_SessionID=SN5TILXOORGpB3asN0WHab965c0282e88d40b36fde4daff8cfb9c6f086f7IocC&NewgroundsAPI_UserName=Coolboyman&NewgroundsAPI_UserID=3617592&ng_username=Coolboyman";
		private const int m_seedSize = 20;
		#endregion
		 
		#region Private Fields
		private enum ErrorCodes
		{
			UNKNOWN_ERROR = 0,
			INVALID_API_ID,
			MISSING_PARAM,
			INVALID_STAT_ID,
			INVALID_COMMAND_ID,
			FLASH_ADS_NOT_APPROVED,
			PERMISSION_DENIED,
			IDENTIFICATION_REQUIRED,
			INVALID_EMAIL_ADDRESS,
			BANNED_USER,
			SESSION_EXPIRED,
			INVALID_SCORE,
			INVALID_MEDAL,
			FILE_NOT_FOUND,
			INVALID_FOLDER,
			SITE_ID_REQUIRED,
			UPLOAD_IN_PROGRESS,
			USER_CANCELLED,
			CONFIRM_REQUEST,
			ILLEGAL_REQUEST,
			CONNECTION_FAILED,
			QUERY_INCOMPLETE,
			SAVE_FILE_ERROR,
			INVALID_VALUE,
			SERVER_ERROR
		}

		private static bool m_loadedAPI = false;
		[SerializeField]
		private string m_apiID;
		[SerializeField]
		private string m_encryptionKey;
		[SerializeField]
		private string m_backupSessionID; //This is for local testing. Load your game on Newgrounds and get the sessionID which is placed in the game's url (NewgroundsAPI_SessionID).
		private Dictionary<string, string> m_headers;
		private int m_publisherID;
		private string m_sandboxID;
		private string m_sessionID;
		private string m_userName;
		private int m_userID;
		private string m_ngUsername;
		private string m_currentHost = "www.newgrounds.com";
		private Queue<JSONCollection> m_commands;

		#endregion

		#region Public Fields
		public static string m_output = "";
		public bool m_connecting = false;
		public string m_gameName = "";
		private Dictionary<string, Medal> m_medals;
		private Dictionary<string, Scoreboard> m_scoreboards;
		private Dictionary<string, SaveFile> m_saveFiles;
		#endregion

		#region Constructors
		void Start() 
		{
			if (m_loadedAPI)
			{
				Destroy (gameObject);
			}
			m_commands = new Queue<JSONCollection>();
			m_headers = new Dictionary<string, string>();
			m_headers.Add("Content-Type", "application/x-www-form-urlencoded");
			m_headers.Add("Accept","*/*");
			
			NGVars(Application.absoluteURL == "" ? m_testVars : Application.absoluteURL);
			
			DontDestroyOnLoad (gameObject);
			m_loadedAPI = true;
		}
		#endregion

		#region Properties
		public string GetID
		{
			get
			{
				return m_apiID;
			}
		}
		
		public int GetUserID
		{
			get
			{
				return m_userID;
			}
		}
		
		public string GetUserName
		{
			get
			{
				return m_userName;
			}
		}
		#endregion

		#region Private Functions
		/// <summary>
		/// Parses the URL variables.
		/// </summary>
		/// <returns>The URL variables.</returns>
		/// <param name="data">The string to be parsed</param>
		/// <exception cref="ArgumentException">Thrown when 'data' is not a valid 'application/x-www-form-urlencoded' string.</exception>
		private Dictionary<string, string> ParseURLVars (string url)
		{
			Dictionary<string, string> values = new Dictionary<string, string> ();
			
			int indexOfQuestionMark = url.IndexOf ('?');
			if (indexOfQuestionMark > -1)
			{
				string[] fields = url.Substring (indexOfQuestionMark + 1).Split ('&');
				
				foreach (var field in fields)
				{
					string[] kv = field.Split ('=');
					int kvlen = kv.Length;
					
					if (kvlen == 0 || kvlen > 2)
					{
						throw new System.ArgumentException ("The given string is not in the valid format.", "data");
					}
					
					values [WWW.UnEscapeURL (kv [0])] = (kvlen == 2) ? WWW.UnEscapeURL (kv [1]) : null;
				}
			}
			
			return values;
		}

		private void NGVars(string input)
		{
			Dictionary<string, string> vars = ParseURLVars (input);

			m_publisherID = System.Convert.ToInt32 (vars["NewgroundsAPI_PublisherID"]);
			m_sandboxID = vars["NewgroundsAPI_SandboxID"];
			m_userName = vars["NewgroundsAPI_UserName"];
			
			if (m_userName == "&lt;deleted7gt;")
			{
				m_userName = "Logged-out";
			}
			
			if (m_userName != "Logged-out")
			{
				m_sessionID = (input == m_testVars)? m_backupSessionID : vars["NewgroundsAPI_SessionID"];
			}
			m_userID = System.Convert.ToInt32(vars["NewgroundsAPI_UserID"]);
			m_ngUsername = vars["ng_username"];
			if (m_userName == "Logged-out")
			{	
				m_ngUsername = m_userName;
			}

      // output useful data
			m_output += input + '\n';
			m_output += "Publisher ID is " + m_publisherID.ToString() + '\n';
			m_output += "Sandbox ID is " + m_sandboxID + '\n';
			m_output += "Username is " + m_userName + '\n';
			m_output += "NG Username is " + m_ngUsername + '\n';
			m_output += "User ID is " + m_userID.ToString() + '\n';
			if (!string.IsNullOrEmpty (m_sessionID))
			{
				m_output += "Session ID is " + m_sessionID + '\n';
			}
		}

		private string Encrypt(JSONCollection data, string seed)
		{
			return Encrypt (data.JSONString(), seed);
		}
		
		private string Encrypt(string JSON, string seed)
		{
			m_output += "Encrypting data: " + JSON  + '\n';
			string hash = MD5Hash.GetHash(seed); //Getting the hash of the randomized string
			RC4 packetKey = new RC4(m_encryptionKey); //Setting the key for the RC4
			StringBuilder combinedHash = new StringBuilder();
			combinedHash.Append(hash);
			combinedHash.Append(packetKey.Convert(JSON)); //Gets the proper RC4 string then Combine the MD5 and the RC4.
			return BaseN.ProduceString(combinedHash.ToString());
		}
		
		private IEnumerator PostData(string final, string seed)	
		{
			SendString sendData = new SendString("securePacket");
			sendData.AddCommand("tracker_id", m_apiID);
			sendData.AddCommand("seed", seed);
			sendData.AddCommand("secure", final);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		private static string RandomString(int size) //Generates a random string for the seed
		{
			StringBuilder s = new StringBuilder();
			
			for (int i = 0; i < size; i++)
			{
				char random = (char)Random.Range(0, 62); //All uppercase, lowercase and numbers
				char toWrite = '0';
				
				if (random < 10) //Number
				{
					toWrite = (char)('0' + random);
				}
				else if (random < 36) //Uppercase
				{
					toWrite = (char)('A' + (random - 10));
				}
				else //Lowercase
				{
					toWrite = (char)('a' + (random - 36));
				}
				s.Append(toWrite);
			}
			
			return s.ToString();
		}
		#endregion

		#region Public Functions
		public void Update()
		{
			while (m_commands.Count > 0)
			{
				JSONCollection data = m_commands.Dequeue();
				string commandName = data.Find ("command_id");
				string contents = data.JSONString();
				int i = 0;

				if (data.Find ("error_msg") != "")
				{
					ErrorCodes errorCode = (ErrorCodes)data.FindInt("error_code");

					m_output = "";
					m_output += ("Error Message: " + data.Find ("error_msg")) + '\n';
					m_output += ("Error type: " + errorCode.ToString ()) + '\n';
					m_output += ("JSON String: " + contents);
					Debug.LogError (m_output);
					return;
				}

				switch (commandName)
				{
					case "connectMovie":
						m_output += "\nConnect to NG received data: " +  data.JSONString() + '\n';
						m_gameName = data.Find("movie_name");
						
						APIEvent.Activate(APIEvent.EventNames.API_CONNECTED, null);
					break;
					case "getMedals": 
						m_medals = new Dictionary<string, Medal>();

						List<JSONCollection> medals = data.GetArray("medals");

						for (i = 0; i < medals.Count; i++)
						{
							m_output += "Registered medal " + medals[i].Find("medal_name") + '\n';
							m_medals[medals[i].Find("medal_name")] = new Medal(medals[i]);
						}

						APIEvent.Activate(APIEvent.EventNames.MEDALS_LOADED, m_medals);
					break;
					case "unlockMedal":
						Medal med = m_medals[data.Find ("medal_name")];
						if (!med.m_unlocked)
						{
							Debug.Log ("Unlocked medal " + data.Find ("medal_name"));
						}
						else
						{
							Debug.Log ("Unlocking medal " + data.Find ("medal_name") + " again.");
						}
						med.Unlock ();
						APIEvent.Activate(APIEvent.EventNames.MEDAL_UNLOCKED, med);
					break;
					case "preloadSettings":
						m_scoreboards = new Dictionary<string, Scoreboard>();
						List<JSONCollection> scores = data.GetArray ("score_boards");
						
						for (i = 0; i < scores.Count; i++)
						{
							m_scoreboards.Add (scores[i].Find ("name"), new Scoreboard(scores[i]));
						}
						
						m_saveFiles = new Dictionary<string, SaveFile>();
						List<JSONCollection> saveFiles = data.GetArray ("save_groups");
						
						for (i = 0; i < saveFiles.Count; i++)
						{
							m_saveFiles[saveFiles[i].Find ("group_name")] = new Newgrounds.SaveFile(saveFiles[i]);
						}   
						APIEvent.Activate(APIEvent.EventNames.METADATA_LOADED, null);
					break;
					case "loadScores":
						JSONCollection scoreHolder = new JSONCollection(contents);
						List<JSONCollection> scoreHolders = scoreHolder.GetArray("scores");
						
						m_output += "High Scores:\n";
						for (i = 0; i < scoreHolders.Count; i++)
						{
							m_output += "Name: " + scoreHolders[i].Find ("username") + ", Score: " + scoreHolders[i].Find ("value") + '\n';
						}
						
						APIEvent.Activate(APIEvent.EventNames.SCORES_LOADED, scoreHolder);
					break;
					case "saveFile":
						m_output += contents;	
					break;
					case "postScore":
						m_output += m_ngUsername + " posted a score of " + data.FindInt("value").ToString() + "!\n";
						List<object> postScoreInfo = new List<object>();
						
						foreach (Scoreboard sb in m_scoreboards.Values)
						{
							if (sb.m_id == data.FindInt("board"))
							{
								postScoreInfo.Add (sb);
								break;
							} 
						}
						postScoreInfo.Add (m_userName);
						postScoreInfo.Add (data.FindInt("value"));
						APIEvent.Activate(APIEvent.EventNames.SCORE_POSTED, postScoreInfo);
					break;
					default:
						m_output += contents;
					break;
				}
			}
		}

		public SaveFile GetSaveGroup(string groupName)
		{
			if (m_saveFiles.ContainsKey(groupName))
			{
				return m_saveFiles[groupName];
			}
			Debug.Log ("No Save group called " + groupName + " exists!");
			return null;
		}

		public IEnumerator LoadScores(string boardName, string period = "all-time", string tag = "", int page = 1, int numResults = 10, int firstResult = 1, int friendsID = -1)
		{
			Scoreboard scoreboard = m_scoreboards[boardName];
			SendString sendData = new SendString("loadScores");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("publisher_id", m_publisherID);
			sendData.AddCommand ("board", scoreboard.m_id);
			sendData.AddCommand ("period", period);
			sendData.AddCommand ("num_results", numResults);
			sendData.AddCommand ("page", page);
			sendData.AddCommand ("first_result", firstResult);
			sendData.AddCommand ("tag", tag);
			if (friendsID > -1)
			{
				sendData.AddCommand ("friends_of", friendsID);
			}
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator SaveFile(string groupName, string data, string fileName, string description = "", string thumbnail = "")
		{
			SaveFile saveFile = m_saveFiles[groupName];
			
			string seed = RandomString(20);
			JSONCollection collection = new JSONCollection();
			collection.Add("command_id", "saveFile");
			collection.Add("publisher_id", m_publisherID);
			collection.Add("session_id", m_sessionID);
			collection.Add("group", saveFile.m_groupID);
			collection.Add("filename", fileName);
			
			collection.Add("description", description);
			collection.Add("status", 1);
			
			collection.Add("file", data);
			collection.Add("thumbnail", "data:image/png;base64,{BASE64_ENCODED_PNG_IMAGE}");
			collection.Add("seed", seed);
			
			yield return StartCoroutine(PostData(Encrypt(collection.JSONString(), seed), seed));
		}
		
		public IEnumerator CheckFilePrivs(string group, string fileName)
		{ 
			SendString sendData = new SendString("checkFilePrivs");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("publisher_id",  m_publisherID.ToString());
			sendData.AddCommand ("user_id",  m_userID.ToString ());
			sendData.AddCommand ("group", group);
			sendData.AddCommand ("filename", fileName);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator DeleteSaveFile(string group)
		{
			string seed = RandomString(20);
			
			JSONCollection deleteFile = new JSONCollection();
			deleteFile.Add ("command_id", "deleteSaveFile");
			deleteFile.Add ("tracker_id", m_publisherID);
			deleteFile.Add ("session_id", m_sessionID);
			
			
			if (m_saveFiles.ContainsKey(group))
			{
				deleteFile.Add ("save_id", m_saveFiles[group].m_groupID);
			}
			else
			{
				Debug.Log ("Save file " + group + " doesn't exist.");
				yield break;
			}
			
			deleteFile.Add ("seed", seed);
			yield return StartCoroutine(PostData(Encrypt (deleteFile, seed), seed));
		}
		
		public IEnumerator LoadCustomLink(string eName)
		{
			SendString sendData = new SendString("loadCustomLink");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("host", m_currentHost);
			sendData.AddCommand ("event", eName);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator LoadFriendList()
		{
			SendString sendData = new SendString("loadFriendList");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("publisher_id", m_publisherID);
			sendData.AddCommand ("user_id", m_userID);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator LoadMySite()
		{
			SendString sendData = new SendString("loadMySite");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("host", m_currentHost);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator LoadNewgrounds()
		{
			SendString sendData = new SendString("loadNewgrounds");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("host", m_currentHost);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator LoadOfficialVersion()
		{
			SendString sendData = new SendString("loadOfficialVersion");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("host", m_currentHost);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator LogCustomEvent(string eventName)
		{
			SendString sendData = new SendString("logCustomEvent");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("host", m_currentHost);
			sendData.AddCommand ("event", eventName);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator LookUpSaveFiles(string groupName)
		{
			SendString sendData = new SendString("lookupSaveFiles");
			sendData.AddCommand ("tracker_id", m_apiID);
			sendData.AddCommand ("publisher_id", m_publisherID);
			sendData.AddCommand ("group_id", m_saveFiles[groupName].m_groupID);
			sendData.AddCommand ("query", null);
			yield return StartCoroutine(RequestTest(sendData));
		}
		
		public IEnumerator RateSaveFile(string groupName, float rating = 5)
		{
			string seed = RandomString(20);
			
			JSONCollection rateFile = new JSONCollection();
			rateFile.Add ("command_id", "rateSaveFile");
			rateFile.Add ("publisher_id", m_publisherID);
			rateFile.Add ("session_id", m_sessionID);
			rateFile.Add ("group", m_saveFiles[groupName].m_groupID);
			rateFile.Add ("save_id", m_saveFiles[groupName].m_keys[0].m_id);
			rateFile.Add ("rating_id", m_saveFiles[groupName].m_ratings[0].m_id);
			rateFile.Add ("seed", seed);
			yield return StartCoroutine(PostData(Encrypt (rateFile, seed), seed));
			
		}

		[Obsolete("For the same functionality as before, use 'PreloadSettings'. To load a specific scoreboard, use 'GetScoreboard'.")]
		public IEnumerator GetScoreboards()
		{
			return PreloadSettings();
		}

		public IEnumerator PreloadSettings()
		{
			SendString sendData = new SendString("preloadSettings");
			sendData.AddCommand("tracker_id", m_apiID);
			sendData.AddCommand("publisher_id", m_publisherID.ToString());
			sendData.AddCommand("user_id", m_userID.ToString());
			yield return StartCoroutine(RequestTest(sendData));
		}

		public Medal GetMedal(string medalName) //Get single medal from game group
		{
			if (m_medals.ContainsKey(medalName))
			{
				return m_medals[medalName];
			}
			Debug.LogError ("No medal called " + medalName + " is registered in the database!");
			return null;
		}

		public IEnumerator PostScore(string scoreboardName, int amount)
		{
			string seed = RandomString(20);
			
			if (m_scoreboards == null)
			{
				m_output = "Connect to NG first!";
				yield break;
			}
			
			Scoreboard scoreBoard = null;
			try
			{
				scoreBoard = m_scoreboards[scoreboardName];
			}
			catch
			{
				m_output += "Scoreboard " + scoreboardName + " doesn't exist.";
				yield break;
			}
			
			if (m_userName == "Logged-out")
			{
				m_output += "You need to log in before you can post a score!";
				yield break;
			}
			
			JSONCollection postScore = new JSONCollection();
			postScore.Add ("command_id", "postScore");
			postScore.Add ("publisher_id", 1);
			postScore.Add ("session_id", m_sessionID);
			postScore.Add ("board", scoreBoard.m_id);
			postScore.Add ("value", amount);
			postScore.Add ("tag", "");
			postScore.Add ("seed", seed);
			
			yield return StartCoroutine(PostData(Encrypt(postScore.JSONString(), seed), seed));
		}

		public IEnumerator GetMedals() //Get all medals from game group
		{
			SendString sendData = new SendString("getMedals");
			sendData.AddCommand("tracker_id", m_apiID);
			sendData.AddCommand("publisher_id", m_publisherID.ToString());
			sendData.AddCommand("user_id", m_userID.ToString());
			yield return StartCoroutine(RequestTest(sendData));
		}
	
		public IEnumerator Connect()
		{
			SendString sendData = new SendString("connectMovie");
			sendData.AddCommand("tracker_id", m_apiID);
			sendData.AddCommand("publisher_id", m_publisherID.ToString());
			sendData.AddCommand("user_id", m_userID.ToString());
			sendData.AddCommand("host", m_currentHost);
			sendData.AddCommand("movie_version", "v1.0");
			sendData.AddCommand("skip_ads", 1);
			yield return StartCoroutine(RequestTest(sendData));
		}

		public IEnumerator RequestTest(SendString data)
		{
			byte[] rawData = data.ByteArray();
			WWW www = new WWW(m_newgroundsGateway, rawData, m_headers );
			m_connecting = true;
			yield return www;
			m_commands.Enqueue(new JSONCollection(www.text));
			m_connecting = false;
		}

		public void GetScoreBoard(string boardName)
		{
			StartCoroutine(LoadScores (boardName));
		}

		public IEnumerator UnlockMedal(string medalName)
		{
			Medal medal;

			m_output = "";
			try
			{
				medal = m_medals[medalName];
			}
			catch
			{
				m_output += "No medal named " + medalName + " exists.";
				yield break;
			}

			string seed = RandomString(m_seedSize);

			JSONCollection medalUnlock = new JSONCollection();
			medalUnlock.Add ("command_id", "unlockMedal");
			medalUnlock.Add ("publisher_id", 1);
			medalUnlock.Add ("session_id", m_sessionID);
			medalUnlock.Add ("medal_id", medal.m_id);
			medalUnlock.Add ("seed", seed);

			m_output += "Attempting to unlock medal " + medalName + '\n';

			yield return StartCoroutine(PostData(Encrypt (medalUnlock, seed), seed));
		}
		#endregion

	}
//}