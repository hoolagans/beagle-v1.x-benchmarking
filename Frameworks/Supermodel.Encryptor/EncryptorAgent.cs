using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Supermodel.Encryptor;

public static class EncryptorAgent
{
    public static byte[] Lock(byte[] key, string str, out byte[] iv)
    {
        using (var aesAlg = new AesManaged())
        {
            aesAlg.GenerateIV();
            iv = aesAlg.IV;
            using (var encryptor = aesAlg.CreateEncryptor(key, iv))
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var csw = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        var bytes = Encoding.Unicode.GetBytes(str);
                        csw.Write(bytes, 0, bytes.Length); //This breaks old code
                        csw.FlushFinalBlock();
                        var cryptoData = memoryStream.ToArray();
                        return cryptoData;
                    }
                }
            }
        }
    }
    public static string Unlock(byte[] key, byte[] code, byte[] iv)
    {
        using (var aesAlg = new AesManaged())
        {
            using (var decryptor = aesAlg.CreateDecryptor(key, iv))
            {
                using (var memoryStream = new MemoryStream(code))
                {
                    memoryStream.Position = 0;
                    using (var csr = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        var dataFragments = new List<byte[]>();
                        var dataFragmentsLengths = new List<int>();

                        var received = 0;
                        while (true)
                        {
                            var dataFragment = new byte[1024];
                            var receivedThisFragment = csr.Read(dataFragment, 0, dataFragment.Length);
                            if (receivedThisFragment == 0) break;
                                
                            dataFragments.Add(dataFragment);
                            dataFragmentsLengths.Add(receivedThisFragment);

                            received += receivedThisFragment;
                        }

                        var data = new byte[dataFragments.Count * 1024];
                        var idx = 0;

                        //now let's build the entire data
                        for (var i = 0; i < dataFragments.Count; i++)
                        {
                            for (var j = 0; j < dataFragmentsLengths[i]; j++)
                            {
                                data[idx++] = dataFragments[i][j];
                            }
                        }

                        var newPhrase = Encoding.Unicode.GetString(data, 0, received);
                        return newPhrase;
                    }
                }
            }
        }
    }

    //Sample key, 16 bytes
    //private readonly static byte[] _key = { 0xA6, 0x46, 0x10, 0xF1, 0xEA, 0x16, 0x51, 0xA0, 0xB2, 0x41, 0x27, 0x5C, 0x23, 0x9C, 0xF0, 0xDD };
}