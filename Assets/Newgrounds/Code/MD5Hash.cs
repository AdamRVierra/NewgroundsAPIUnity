#region References
using System.Security.Cryptography;
using System.Text;
#endregion

public class MD5Hash
{
	#region Public Functions
	public static string GetHash(string input)
    {
		MD5 hash = MD5.Create();
		byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder sBuilder = new StringBuilder();

		for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }
	#endregion
}
