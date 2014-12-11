#region References
using UnityEngine;
using System.Collections.Generic;
#endregion

namespace Newgrounds
{
	public class APIEventList
	{
		#region Public Fields
		public delegate void EventAction(string type, bool success, object data);
		public event EventAction E;
		public string m_eventName;
		#endregion

		#region Public Functions
		public APIEventList(string eventName)
		{
			m_eventName = eventName;
		}

		public void Activate(object obj)
		{
			E(m_eventName, true, obj);
		}
		#endregion
	}
	
	public class APIEvent
	{
		#region Public Fields
		public enum EventNames
		{
			NEW_VERSION_AVAILABLE, 
			API_CONNECTED, 
			ADS_APPROVED, 
			HOST_BLOCKED, 
			EVENT_LOGGED, 
			METADATA_LOADED, 
			SCORE_POSTED, 
			SCORES_LOADED, 
			MEDAL_UNLOCKED, 
			MEDALS_LOADED, 
			QUERY_COMPLETE, 
			UNLOCK_MEDAL,
			FILE_LOADED,
			AD_ATTACHED,
			FILE_INITIALIZED,
			FILE_REQUESTED
		};

		public static Dictionary<string, APIEventList> Events = new Dictionary<string, APIEventList>();
		#endregion

		#region Public Functions
		public static void AddEvent(EventNames eventName, APIEventList.EventAction f)
		{
			string eName = eventName.ToString();
			if (!Events.ContainsKey(eName))
			{
				Events[eName] = new APIEventList(eName);
			}
			Events[eName].E += f;
		}

		public static void Activate(EventNames eventName, object obj)
		{
			string eventNameString = eventName.ToString();
			if (Events.ContainsKey(eventNameString))
			{
				Events[eventNameString].Activate(obj);
			}
		}
		#endregion
	}
}	