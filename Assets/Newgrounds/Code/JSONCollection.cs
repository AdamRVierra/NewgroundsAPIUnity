#region References
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Newgrounds
{
	public class JSONCollection
	{
		#region Private Fields
		private List <JSONEntry> m_entries;
		#endregion

		#region Constructor
		public JSONCollection()
		{
			m_entries = new List<JSONEntry>();
		}
		#endregion

		#region Public Functions
		public List <JSONEntry> Entries
		{
			get
			{
				return m_entries;
			}
		}

		public JSONCollection(string list)
		{
			m_entries = new List<JSONEntry>();
			CreateList (list);
		}
		
		public void Add(string key, object value)
		{
			m_entries.Add (new JSONEntry(key, value));
		}

		public void CreateList(string entries)
		{
			if (entries == "")
			{
				return;
			}
			while (entries[0] == 0x09)
			{
				entries = entries.Substring(1);
			}

			int i = 0;
			string entry = "";
			string key = "";

			int brackets = 0;

			for (i = 0; i < entries.Length; i++)
			{
				if (entries[i] == '[')
				{
					entry += '[';
					brackets++;
					continue;
				}
				else if (entries[i] == ']')
				{
					brackets--;
					entry += ']';
					if (brackets == 0)
					{
						Add(key, entry);
						key = "";
						entry = "";
						i+=2;
						continue;
					}
				}
				else if (brackets > 0)
				{
					entry += entries[i];
					continue;
				}

				if (entries[i] == '"')
				{
					if (entry == "")
					{

					}
					else if (key == "")
					{
						key = entry;
						entry = "";
						i++;
					}
					else
					{
						if (entry[entry.Length - 1] == ',')
						{
							entry = entry.Substring(0, entry.Length - 1);
							i--;
						}
						Add(key, entry);

						key = "";
						entry = "";
						i++;

						if (entries[i] == '}')
						{
							return;
						}
					}

				}
				else if (entries[i] == '{' || entries[i] == '}' || entries[i] == ':')
				{

				}
				else
				{
					entry += entries[i];

				}
			}

			Add (key, entry);
		}
		
		public string Find(string s)
		{
			for (int i = 0; i < m_entries.Count; i++)
			{
				if (m_entries[i].m_key.Equals(s))
				{
					return m_entries[i].m_value.ToString();
				}
			}
			
			return "";
		}

		public int FindInt(string s)
		{
			for (int i = 0; i < m_entries.Count; i++)
			{
				if (m_entries[i].m_key == s)
				{

					string check = m_entries[i].m_value.ToString();
					while (check[check.Length - 1] == ']')
					{
						check = check.Substring(0, check.Length - 2);
					}
					return System.Convert.ToInt32 (check);
				}
			}
			return -1;
		}
		
		public string JSONString()
		{
			string JSON = "{";
			int totalCommands = m_entries.Count;
			
			for (int i = 0; i < totalCommands; i++)
			{
				JSON += '"';
				JSON += m_entries[i].m_key;
				JSON += "\":";
				
				if (m_entries[i].m_valueType == "String")
				{
					JSON += '"';
					JSON += m_entries[i].m_value;
					JSON += '"';
				}
				else
				{
					JSON += m_entries[i].m_value;
				}
				
				if (i + 1 != totalCommands)
				{
					JSON += ',';
				}
			}
			
			JSON += "}";
			
			return JSON;
		}

		public List<JSONCollection> GetArray(string search)
		{
			string entry = Find (search);
			List <JSONCollection> collections = new List<JSONCollection>();

			entry = entry.Substring(1);
			if (entry == "]")
			{
				return collections;
			}

			while (true)
			{
				int i = 0;
				int brackets = 0;
				collections.Add (new JSONCollection(entry));

				do
				{
					if (entry[i] == '{')
					{
						brackets++;
					}
					else if (entry[i] == '}')
					{
						brackets--;
					}
					i++;
				}
				while (i < entry.Length &&brackets > 0);

				if (i + 1 >= entry.Length)
				{
					break;
				}

				entry = entry.Substring (i + 1);
			}

			return collections;
		}
		#endregion
	}
}