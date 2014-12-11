#region References
using System.Text;
using System;
#endregion

public class RC4
{
	//Original AS Code written by BCADREN http://pastebin.com/N7KTi23H
	#region Private Fields
	private string m_key;
	private int[] m_sbox;
	private int[] m_keyNums;
	private int m_keyLength;
	private const short m_max = 256;
	#endregion

	#region Private Functions
	private void SetUp()
	{
		m_sbox = new int[m_max];
		m_keyNums = new int[m_max];
		
		m_keyLength = m_key.Length;
		
		for (int a = 0; a < m_max; a++)
		{
			m_keyNums[a] = m_key[a % m_keyLength];	
			m_sbox[a] = a;
		}
		
		int b = 0;
		
		for (int c = 0; c < m_max; c++)
		{
			b = (b + m_sbox[c] + m_keyNums[c]) % m_max;
			int tempSwap = m_sbox[c];
			m_sbox[c] = m_sbox[b];
			m_sbox[b] = tempSwap;
		}
	}
	#endregion

	#region Constructor
	public RC4(string k)
	{
		m_key = k;	
		SetUp();
	}
	#endregion

	#region Public Functions
    public string Convert(string input)
	{	
		SetUp();
		
		int i = 0;
		int j = 0;
		int k = 0;
		
		string chiper = "";
		
		for (int a = 0; a < input.Length; a++)
		{
			i = ((i + 1) % m_max);
            j = ((j + m_sbox[i]) % m_max);
			int tempSwap = m_sbox[i];
			m_sbox[i] = m_sbox[j];
            m_sbox[j] = tempSwap;
            k = m_sbox[(m_sbox[i] + m_sbox[j]) % m_max];
			int cipherBy = input[a];
			cipherBy = cipherBy ^ k;
			chiper += cipherBy.ToString("x2");
		}
		
		return chiper;
	}
	#endregion
}