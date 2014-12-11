using UnityEngine;
using System.Collections;

namespace Newgrounds
{
	public class Scoreboard
	{
		public int m_id;
		public string m_name;

		public Scoreboard(JSONCollection collection) 
		{
			m_id = collection.FindInt("id");
			m_name = collection.Find("name");
		}
	}
}
