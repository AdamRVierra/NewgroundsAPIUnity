#region References
using System.Collections;
using System.Text;
using UnityEngine;
#endregion

namespace Newgrounds
{

	public class SendString
	{
		#region Public Fields
		public StringBuilder m_contents;
		#endregion

		#region Constructor
		public SendString()
		{

		}
		#endregion

		#region Public Functions
		public SendString(string commandName)
		{
			m_contents = new StringBuilder();
			AddCommand ("command_id", commandName);
		}
		 
		public void AddCommand(string command, string argument)
		{
			if (m_contents.Length != 0)
			{
				m_contents.Append ('&');
			}
			m_contents.Append (WWW.EscapeURL(command));
			m_contents.Append ('=');
			m_contents.Append (WWW.EscapeURL(argument));
		}

		public void AddCommand(string command, int argument)
		{
			AddCommand (command, argument.ToString());
		}

		public byte[] ByteArray()
		{
			string contentsStr = m_contents.ToString();
			API.m_output += "Sending data to server: " + contentsStr + '\n';
			return Encoding.UTF8.GetBytes(contentsStr.ToCharArray ());
		}
		#endregion
	}
}
