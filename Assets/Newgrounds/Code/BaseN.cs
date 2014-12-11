#region References
using UnityEngine;
using System.Collections;
using System;
#endregion

public class BaseN
{	
	#region Private Fields
	private static string m_rad = "/g8236klvBQ#&|;Zb*7CEA59%s`Oue1wziFp$rDVY@TKxUPWytSaGHJ>dmoMR^<0~4qNLhc(I+fjn)X";
	private static int m_radSize = 6;
	#endregion

	#region Private Functions
	public static string ProduceString(string convert)
	{
		int stringLength = convert.Length;
		string finalString = (stringLength % m_radSize).ToString();
		float length = (float)m_rad.Length;
		
		for (int i = 0; i < stringLength; i += m_radSize)
		{
			int endPart = ((stringLength - i > m_radSize) ? m_radSize : (stringLength - i)); //Getting a substring of the byte we're going to convert into the radix format.
			string part = convert.Substring(i, endPart);
			int num = Convert.ToInt32(part, 0x10);

			for (int j = 3; j >= 0; j--) //Lets get the characters and place them in order from right to left
			{
				int radIndex = Mathf.FloorToInt ((num % Mathf.Pow(length, j + 1)) / (Mathf.Pow(length, j)));

				if (radIndex == 0) //Making sure we don't get any extra slashes for the last number.
				{
					if (i >= (stringLength - m_radSize))
					{
						continue;	
					}
				}
				
				finalString += m_rad[radIndex];
			}
		}
		
		return finalString;
	}
	#endregion
}
