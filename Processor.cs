using System.Security.Cryptography;
using System.Text;

namespace Cryo {

    public class Processor
    {
        // Method for generating bytes based off of a passkey
        // Used to create Keys and IVs.
        // Keys: 16, 24 or 32
        // IV: 16 (ONLY)
        public byte[] GenerateBytes(string passkey, int size)
        {
            byte[] result = new byte[size];
            byte[] bytes = Encoding.ASCII.GetBytes(passkey);
            int length = bytes.Length;

            for (int i = 0; i < result.Length; i++)
            {
                if (i < length) 
                {
                    result[i] = bytes[i];
                } else {
                    if (i - length < length)
                    {
                        result[i] = bytes[i - length];
                    }
                }
            }

            return result;
        }
        
        // Will create an encrypted copy of the file path.
        // AES encryption based off of the Key and IV.
        public void Encrypt(string filePath, byte[] Key, byte[] IV)
        { 
            string outputFilePath = $"{filePath}.enc";
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (MemoryStream memoryStream = new())
                    {
                        using (CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {   
                            using (FileStream fileStream = File.OpenRead(filePath))
                            {
                                byte[] buffer = new byte[4096];
                                int bytesRead;

                                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0) {
                                    cryptoStream.Write(buffer, 0, bytesRead);
                                }
                            }
                        }
                        File.WriteAllBytes(outputFilePath, memoryStream.ToArray());
                        Console.WriteLine($"File Sucessfully Encrypted");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while encrypting the file: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        // Will output the original file assuming the correct Key and IV is given.
        // If not, no file will output.
        public void Decrypt(string filePath, byte[] Key, byte[] IV)
        {
            string file;
            if (filePath.Contains(".enc")) {
                file = filePath;
            } else {
                file = $"{filePath}.enc";
            }

            string outFile = file.Replace(".enc", "");

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (MemoryStream memoryStream = new(File.ReadAllBytes(file)))
                    {
                        using (CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (FileStream fileStream = File.OpenWrite(outFile))
                            {
                                byte[] buffer = new byte[4096];
                                int bytesRead;

                                while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0) {
                                    fileStream.Write(buffer, 0, bytesRead);
                                }
                                Console.WriteLine($"File Sucessfully Decrypted");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while decrypting the file: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}