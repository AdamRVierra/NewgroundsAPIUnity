#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Newgrounds
{
	[System.Serializable]
	public class Medal
	{
		#region Public Fields
		public enum Difficulty
		{
			Easy = 1,
			Moderate = 2,
			Challenging = 3,
			Difficult = 4,
			Brutal = 5
		}
		
		public int m_id;
		public string m_name;
		public int m_value;
		public Difficulty m_difficulty;
		public bool m_unlocked;
		public Texture2D m_icon;
		public string m_iconURL;
		public bool m_secret;
		public string m_description;
		public Vector2 m_pos;

		public static Sprite DEFAULT_ICON;

		public static string DIFFICULTY_BRUTAL = "Brutal";
		public static string DIFFICULTY_CHALLENGING = "Challenging";
		public static string DIFFICULTY_DIFFICULT = "Difficult";
		public static string DIFFICULTY_EASY = "Easy";
		public static string DIFFICULTY_MODERATE = "Moderate";

		public static uint ICON_HEIGHT = 50;
		#endregion

		#region Constructor
		public Medal(JSONCollection medalData)
		{
			m_id = medalData.FindInt("medal_id");
			m_name = medalData.Find("medal_name");
			m_value = medalData.FindInt("medal_value");
			m_difficulty = (Difficulty)(medalData.FindInt("medal_difficulty"));
			m_unlocked = System.Convert.ToBoolean(medalData.Find("medal_unlocked"));
			m_iconURL = medalData.Find("medal_icon");

			m_iconURL = string.Join ("", m_iconURL.Split ('\\')); //Get rid of certain chars
			m_iconURL = m_iconURL.Insert (4, ":");

			m_secret = ((medalData.FindInt("secret")) == 1);
			m_description = medalData.Find("medal_description");
		}
		#endregion

		#region Public Functions
		public void Unlock()
		{
			if (!m_unlocked)
			{
				m_unlocked = true;
				API.m_output += "Unlocked medal " + m_name;
				return;
			}
			API.m_output += "Medal " + m_name + " is already unlocked! ";
		}
		#endregion
	}
}