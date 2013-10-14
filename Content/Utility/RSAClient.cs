using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PoVWebsite.Content.Utility
{
    public class RSAClient
    {
        private static string _privateKey = "<RSAKeyValue><Modulus>txRzokDZOvlv3sOJ4Nym2w8Wm1kFc4fMKKdLrbLqY1aXyjyX2LsIxfCjlKp4NfkNL5oqab1TTvv5qDMz5gN0/7l5iBueCTmlnGyVJNh7JURO0jrKpu2EQaL7sxdUY/i+H9cDa+zZn1UNZeas8nT6cxfv0isU0NrBg1WMklwx3DU=</Modulus><Exponent>AQAB</Exponent><P>8CplGsPdUUmqPLByJf2ekCsUX5ws+ePTtvRgn/BpwtmCHuvsr5ffIVY1VxU4fm/pEsSC3bZJmcbFmzxFUHi6ZQ==</P><Q>wyaK65Ng/cuROD6a+Ig5h9JGTSXWSZOUG4eZhp2mO7nfTKyc4hRLAhURx4+E3xg3FzoHJ9RDHdwmLVQygw4VkQ==</Q><DP>NqVu2+g1M4nPcn+zeXF74tHqkNa4ZSOxyK9STRIm/m7/bInCETI9UxiKioYJlLtiDSMpo8kWdicET44nN2hBAQ==</DP><DQ>mc+ZEs/YVHDMBq6hal3EKl77dfbqjJc61u/re4Rp7w0zCLXL+QzRidSdbNKbgVMOqMndtfDA7ZoEpRmFrnG9YQ==</DQ><InverseQ>yLCF1Wcv1KSzTDTpfxZVII7STNvoMFKsDZJSeRXHQtJzsLT8Zb6Glx5baPZh/NsEFaH7r9AY+lIs/WtQjqoMbA==</InverseQ><D>AtYhXQ7spccQuDk8KmAqkC5yaaJY3RRjKqn234Bd0VTnwb5Wauy4AwzYop+KDtvKMWz4c4c3aZFGqeZEEoyGcLDAH/poodhaGzxEx819uiGcc55teR2pNSRd2G9s+6Neq8YxU5kuiPn66NXmQ8Oks/0pI/aYQ3+Yy2Cu4xzojxE=</D></RSAKeyValue>";
        internal static string _publicKey = " <RSAKeyValue><Modulus>txRzokDZOvlv3sOJ4Nym2w8Wm1kFc4fMKKdLrbLqY1aXyjyX2LsIxfCjlKp4NfkNL5oqab1TTvv5qDMz5gN0/7l5iBueCTmlnGyVJNh7JURO0jrKpu2EQaL7sxdUY/i+H9cDa+zZn1UNZeas8nT6cxfv0isU0NrBg1WMklwx3DU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static UnicodeEncoding _encoder = new UnicodeEncoding();
        private static HMAC _hash = new HMACSHA256();
        private static RandomNumberGenerator _rng = RNGCryptoServiceProvider.Create();
        

        internal static string Decrypt(string data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string[] dataArray = data.Split(new char[] { ',' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }
            rsa.FromXmlString(_privateKey);
            byte[] decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);

        }

        internal static string Encrypt(string data, bool usePubKey = true)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            if (usePubKey)
                rsa.FromXmlString(_publicKey);
            else
                rsa.FromXmlString(_privateKey);
            byte[] dataToEncrypt = _encoder.GetBytes(data);
            byte[] encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            int length = encryptedByteArray.Count();
            int item = 0;
            StringBuilder sb = new StringBuilder();
            foreach (byte x in encryptedByteArray)
            {
                item++;
                sb.Append(x);
                if (item < length)
                    sb.Append(",");
            }
            return sb.ToString();
        }

        internal static string Hash(string dataString)
        {
            _hash.Key = System.Text.Encoding.UTF8.GetBytes(_privateKey);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(dataString);
            byte[] hashedData = _hash.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            int length = hashedData.Count();
            int item = 0;
            foreach (byte x in hashedData)
            {
                item++;
                sb.Append(x);
                if (item < length)
                    sb.Append(",");
            }
            return sb.ToString();

        }

        internal static string NewToken()
        {
            byte[] tokenData = new byte[32];
            _rng.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }
    }
}