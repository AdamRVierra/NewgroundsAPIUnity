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
			m_API = GameObject.FindGameObjectWithTag("NewgroundsAPI").GetComponent<API>(); //This GameObject along with the correct tag must be in the scene.
		}

		void OnEnable() //Creating events
		{
			APIEvent.AddEvent(APIEvent.EventNames.MEDAL_UNLOCKED, UnlockMedal);
			APIEvent.AddEvent(APIEvent.EventNames.MEDALS_LOADED, LoadMedals);
			APIEvent.AddEvent(APIEvent.EventNames.UNLOCK_MEDAL, UnlockMedal);
			APIEvent.AddEvent(APIEvent.EventNames.METADATA_LOADED, MetadataLoaded);
		}

		public void MetadataLoaded(string type, bool success, object data)
		{
			Debug.Log ("Metadata Loaded");
		}
		
		public void UnlockMedal(string type, bool success, object data) //Function tied to an event
		{
			#pragma warning disable 219
			Medal medal = (Medal)data;
			#pragma warning restore 219
			Debug.Log ("Unlock Medal");
		}

		public void LoadMedals(string type, bool success, object data) //Function tied to an event
		{
			Dictionary<string, Medal> medals = (Dictionary<string, Medal>)data;
			#pragma warning disable 219
			foreach (string k in medals.Keys)
			{
				//Do stuff
			}
			#pragma warning restore 219

			Debug.Log ("Loaded medals locally for future use.");
		}

		void OnGUI() //GUI To test with
		{
			if (GUI.Button(new Rect(10, 10, 100, 50), "Connect to NG"))
			{
				API.m_output = "";
				StartCoroutine(m_API.Connect());
				StartCoroutine(m_API.GetMedals());
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