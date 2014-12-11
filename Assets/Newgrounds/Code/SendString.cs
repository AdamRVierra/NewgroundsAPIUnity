#region References
using System.Collections;
using System.Text;
#endregion

namespace Newgrounds
{

	public class SendString
	{
		#region Public Fields
		public string m_contents = "";
		#endregion

		#region Constructor
		public SendString()
		{

		}
		#endregion

		#region Public Functions
		public SendString(string commandName)
		{
			AddCommand ("command_id", commandName);
		}
		 
		public void AddCommand(string command, string argument)
		{
			if (m_contents != "")
			{
				m_contents += "&";
			}
			m_contents += command + "=" + argument;
		}

		public void AddCommand(string command, int argument)
		{
			AddCommand (command, argument.ToString());
		}

		public byte[] ByteArray()
		{
			API.m_output += "Sending data to server: " + m_contents + '\n';
			return Encoding.UTF8.GetBytes(m_contents.ToCharArray ());
		}
		#endregion
	}
}
