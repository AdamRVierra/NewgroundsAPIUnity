//This is an example class to demonstrate how to communicate with the api

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Newgrounds
{
	public class Main : MonoBehaviour 
	{
		private API m_API;

		public void Start()
		{
			m_API = GameObject.FindGameObjectWithTag("NewgroundsAPI").GetComponent<API>();
		}
		
		void OnEnable() 
		{
			APIEvent.AddEvent(APIEvent.EventNames.SCORES_LOADED, LoadScores);
			APIEvent.AddEvent(APIEvent.EventNames.MEDAL_UNLOCKED, UnlockMedal);
			APIEvent.AddEvent(APIEvent.EventNames.MEDALS_LOADED, LoadMedals);
			APIEvent.AddEvent(APIEvent.EventNames.UNLOCK_MEDAL, UnlockMedal);
			APIEvent.AddEvent(APIEvent.EventNames.METADATA_LOADED, MetadataLoaded);
			APIEvent.AddEvent(APIEvent.EventNames.SCORE_POSTED, ScorePosted);
		}
		
		public void ScorePosted(string type, bool success, object data)
		{
			List<object> info = (List<object>)data;
			Scoreboard board = (Scoreboard)info[0];
			string userName = (string)info[1];
			int value = (int)info[2];
		}
		
		public void MetadataLoaded(string type, bool success, object data)
		{
			Debug.Log ("Metadata Loaded");
		}
		
		public void LoadScores(string type, bool success, object data)
		{
			JSONCollection scores = (JSONCollection)data;
			Debug.Log ("Load Scores");
		}
		
		public void UnlockMedal(string type, bool success, object data)
		{
			Medal medal = (Medal)data;
			Debug.Log ("Unlock Medal");
		}
		
		public void LoadMedals(string type, bool success, object data)
		{
			Dictionary<string, Medal> medals = (Dictionary<string, Medal>)data;
			foreach (string k in medals.Keys)
			{
				//m_medal = medals[k];
			}
		}
		
		private bool m_selectedOption;
		
		void OnGUI() 
		{
			m_selectedOption = false;
			if (GUI.Button(new Rect(10, 10, 100, 50), "Connect to NG"))
			{
				API.m_output = "";
				StartCoroutine(m_API.Connect());
				StartCoroutine(m_API.GetMedals());
				StartCoroutine(m_API.GetScoreboards());
			}
			
			if (GUI.Button(new Rect(120, 10, 100, 50), "Post Score"))
			{
				API.m_output = "";
				StartCoroutine(m_API.PostScore ("Test", Random.Range(0, 100000)));
			}
			
			if (GUI.Button(new Rect(120, 70, 100, 50), "Get Scores"))
			{
				API.m_output = "";
				StartCoroutine(m_API.LoadScores ("Test"));
			}
			
			if (GUI.Button(new Rect(10, 130, 100, 50), "Look Up Save Files"))
			{
				API.m_output = "";
				StartCoroutine(m_API.LookUpSaveFiles("SaveTest"));
			}
			
			if (GUI.Button(new Rect(10, 190, 100, 50), "Rate Save File"))
			{
				API.m_output = "";
				StartCoroutine(m_API.RateSaveFile("SaveTest"));
			}
			
			if (GUI.Button(new Rect(120, 130, 100, 50), "Save File"))
			{
				API.m_output = "";
				StartCoroutine(m_API.SaveFile("SaveTest", "abcdefg", "A save file"));
			}
			
			if (GUI.Button(new Rect(120, 190, 100, 50), "Rate Save File"))
			{
				API.m_output = "";
				StartCoroutine(m_API.RateSaveFile("SaveTest"));
			}
			
			if (GUI.Button(new Rect(230, 10, 100, 50), "deleteSaveFile"))
			{
				API.m_output = "";
				StartCoroutine(m_API.DeleteSaveFile("SaveTest"));
			}
			
			if (GUI.Button(new Rect(230, 70, 100, 50), "CheckFilePrivs"))
			{
				API.m_output = "";
				StartCoroutine(m_API.CheckFilePrivs("SaveTest", "FileName"));
			}
			
			if (GUI.Button(new Rect(340, 10, 100, 50), "LoadCustomLink"))
			{
				API.m_output = "";
				StartCoroutine(m_API.LoadCustomLink("ReferalTest"));
			}
			
			if (GUI.Button(new Rect(340, 70, 100, 50), "LoadFriendList"))
			{
				API.m_output = "";
				StartCoroutine(m_API.LoadFriendList());
			}
			
			if (GUI.Button(new Rect(450, 10, 100, 50), "LoadMySite"))
			{
				API.m_output = "";
				StartCoroutine(m_API.LoadMySite());
			}
			
			if (GUI.Button(new Rect(450, 70, 100, 50), "LoadNewgrounds"))
			{
				API.m_output = "";
				StartCoroutine(m_API.LoadNewgrounds());
			}
			
			if (GUI.Button(new Rect(560, 10, 100, 50), "LoadOfficialVersion"))
			{
				API.m_output = "";
				StartCoroutine(m_API.LoadOfficialVersion());
			}
			
			if (GUI.Button(new Rect(560, 70, 100, 50), "Log Custon Event"))
			{
				API.m_output = "";
				StartCoroutine(m_API.LogCustomEvent("AnEvent"));
			}
			
			if (GUI.Button(new Rect(10, 70, 100, 50), "Unlock Medal"))
			{
				API.m_output = "";
				StartCoroutine(m_API.UnlockMedal("Second Medal"));
			}
			
			GUI.skin.button.wordWrap = true;
			
			GUI.TextField (new Rect (10, 250, 550, 450), API.m_output, 250000);
		}
	}
}