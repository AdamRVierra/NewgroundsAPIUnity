using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Newgrounds
{
	public class SaveFileRating
	{
		public int m_id;
		public string m_name;
		public float m_min;
		public float m_max;
		public bool m_float;

		public SaveFileRating(JSONCollection col)
		{
			m_id = col.FindInt("id");
			m_name = col.Find ("name");
			m_min = float.Parse (col.Find ("min"));
			m_max = float.Parse (col.Find ("max"));
			m_float = (col.Find ("float") == "true");
		}
	}

	public class SaveFileKey
	{
		public enum VarType
		{
			Float = 1,
			Int = 2,
			String = 3,
			Boolean = 4
		}

		public int m_id;
		public string m_name;
		public VarType m_type;

		public SaveFileKey(JSONCollection col)
		{
			m_id = col.FindInt("id");
			m_name = col.Find ("name");
			m_type = (VarType)col.FindInt("type");
		}
	}

	public class SaveFile
	{
		public enum GroupType
		{
			System = 0,
			Private = 1,
			Public = 2,
			Moderated = 3
		}

		public int m_groupID;
		public string m_groupName;
		public GroupType m_groupType;
		public List<SaveFileKey> m_keys;
		public List<SaveFileRating> m_ratings;

		public SaveFile(JSONCollection collection)
		{
			m_groupID = collection.FindInt("group_id");
			m_groupName = collection.Find ("group_name");

			m_groupType = (GroupType)collection.FindInt ("group_type");

			m_keys = new List<SaveFileKey>();
			List<JSONCollection> keyArray = collection.GetArray("keys");

			for (int i = 0; i < keyArray.Count; i++)
			{
				m_keys.Add (new SaveFileKey(keyArray[i]));
			}

			m_ratings = new List<SaveFileRating>();
			List<JSONCollection> ratingArray = collection.GetArray("ratings");

			for (int i = 0; i < ratingArray.Count; i++)
			{
				m_ratings.Add (new SaveFileRating(ratingArray[i]));
			}
		}

		public int GetKeyID(string key)
		{
			for (int i = 0; i < m_keys.Count; i++)
			{
				if (m_keys[i].m_name == key)
				{
					return m_keys[i].m_id;
				}
			}
			return -1;
		}
	}
}