#region References
using UnityEngine;
using System.Collections;
#endregion

namespace Newgrounds
{
	public class JSONEntry
	{
		#region Public Methods
		public string m_key;
		public object m_value;
		public string m_valueType;
		#endregion


		#region Constructor
		public JSONEntry(string key, object v)	
		{
			m_key = key;
			m_value = v;
			m_valueType = m_value.GetType().ToString();
			m_valueType = m_valueType.Substring(m_valueType.IndexOf(".") + 1);
		}
		#endregion
	}
}
