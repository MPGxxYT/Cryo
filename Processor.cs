using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Aes = System.Security.Cryptography.Aes;

namespace Cryo {

    public enum ByteType
    {
        Key,
        IV
    }

    public class Processor
    {

        // Method for generating bytes based off of a passkey
        // Used to create Keys and IVs.
        // Use ByteType.Key for Keys
        // Use ByteType.IV for IVs
        public byte[] GenerateBytes(string password, ByteType type) {

                byte[] bytes = Encoding.ASCII.GetBytes(password);
                SHA256 sha256 = SHA256.Create();
                byte[] hash = sha256.ComputeHash(bytes);

            if (type == ByteType.Key) {
                return hash;
            } 

            if (type == ByteType.IV) {
                byte[] iv = new byte[16];
                Array.Copy(hash, iv, 16);
                return iv;
            }

            return new byte[1];
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