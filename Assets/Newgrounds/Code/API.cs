#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
#endregion

//Code written by Adam R. Vierra
//adam@adamvierra.com
//http://coolboyman.newgrounds.com/

namespace Newgrounds
{
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
		public Dictionary<string, Medal> m_medals;
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
		private string GetNGVar(string input, string search)
		{
			int strLeng = search.Length;
			int start = input.IndexOf (search) + strLeng + 1;
			
			StringBuilder output = new StringBuilder();
			
			while (start < input.Length&&input[start] != '&')
			{
				output.Append(input[start]);
				start++;
			}

			return output.ToString();
		}

		private void NGVars(string input)
		{
			m_output += input + '\n';
			m_publisherID = System.Convert.ToInt32 (GetNGVar (input, "NewgroundsAPI_PublisherID"));
			m_output += "Publisher ID is " + m_publisherID.ToString() + '\n';
			m_sandboxID = GetNGVar (input, "NewgroundsAPI_SandboxID");
			m_output += "Sandbox ID is " + m_sandboxID + '\n';
			m_userName = GetNGVar (input, "NewgroundsAPI_UserName");

			if (m_userName == "%26lt%3Bdeleted%26gt%3B")
			{
				m_userName = "Logged-out";
			}
			m_output += "Username is " + m_userName + '\n';

			if (m_userName != "Logged-out")
			{
				if (input == m_testVars)
				{
					m_sessionID = m_backupSessionID;
				}
				else
				{
					m_sessionID = GetNGVar (input, "NewgroundsAPI_SessionID");
				}
				m_output += "Session ID is " + m_sessionID + '\n';
			}
			m_userID = System.Convert.ToInt32(GetNGVar (input, "NewgroundsAPI_UserID"));
			m_output += "User ID is " + m_userID.ToString() + '\n';
			m_ngUsername = GetNGVar(input, "ng_username");
			if (m_userName == "Logged-out")
			{	
				m_ngUsername = m_userName;
			}
			m_output += "NG Username is " + m_ngUsername + '\n';
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
			sendData.AddCommand("tracker_id", WWW.EscapeURL(m_apiID));
			sendData.AddCommand("seed", WWW.EscapeURL(seed));
			sendData.AddCommand("secure", WWW.EscapeURL(final));
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
						
						APIEvent.Activate(APIEvent.EventNames.API_CONNECTED, m_medals);
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
				}
			}
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

		public IEnumerator GetMedals() //Get all medals from game group
		{
			SendString sendData = new SendString("getMedals");
			sendData.AddCommand("tracker_id", WWW.EscapeURL(m_apiID));
			sendData.AddCommand("publisher_id", m_publisherID.ToString());
			sendData.AddCommand("user_id", m_userID.ToString());
			yield return StartCoroutine(RequestTest(sendData));
		}
	
		public IEnumerator Connect()
		{
			SendString sendData = new SendString("connectMovie");
			sendData.AddCommand("tracker_id", WWW.EscapeURL(m_apiID));
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
}