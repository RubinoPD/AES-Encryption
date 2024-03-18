using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Praktinis2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {

                // Display menu options
                Console.WriteLine("Pasirinkite ka norite daryti:");
                Console.WriteLine("1. Encrypt text.");
                Console.WriteLine("2. Decrypt text.");
                Console.WriteLine("3. Exit.");


                // Read users choice
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                    continue;
                }


                // Perform actions based on on users choice
                switch (choice)
                {
                    case 1:
                        EncryptText();
                        break;
                    case 2:
                        DecryptText();
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a valid option.");
                        break;
                }

            }
        }
        static void EncryptText()
        {

            Console.WriteLine("Enter the text to encrypt:");
            string text = Console.ReadLine();

            Console.WriteLine("Enter the secret key:");
            string key = Console.ReadLine();

            // Uzpildyti ar sutrumpinti rakta iki 16 baitu (128bitu)
            key = key.PadRight(16, '\0').Substring(0, 16);

            Console.WriteLine("Select encryption mode:");
            Console.WriteLine("1. ECB");
            Console.WriteLine("2. CBC");
            Console.WriteLine("3. CFB");
            Console.WriteLine("4. OFB");

            CipherMode mode;

            switch (Console.ReadLine())
            {
                case "1":
                    mode = CipherMode.ECB;
                    break;
                case "2":
                    mode = CipherMode.CBC;
                    break;
                case "3":
                    mode = CipherMode.CFB;
                    break;
                case "4":
                    mode = CipherMode.OFB;
                    break;
                default:
                    Console.WriteLine("Invalid mode. Using ECB by default.");
                    mode = CipherMode.ECB;
                    break;
            }

            // Teksto kodavimas(encrypt)
            string encryptedText = Encrypt(text, key, mode);
            Console.WriteLine("Encrypted text: " + encryptedText);

            Console.WriteLine("Do you want to save the encrypted text to a file? (yes/no)");
            if (Console.ReadLine().ToLower() == "yes")
            {
                Console.WriteLine("Enter file name to save the encrypted text:");
                string fileName = Console.ReadLine();
                File.WriteAllText(fileName, encryptedText);
                Console.WriteLine("Encrypted text saved to file successfully.");
            }
        }

        static void DecryptText()
        {
            Console.WriteLine("Enter the file name containing the encrypted text:");
            string fileName = Console.ReadLine();
            string encryptedText = File.ReadAllText(fileName);

            Console.WriteLine("Enter the seckret key: ");
            string key = Console.ReadLine();

            // Pad or truncate the key to 16 bytes (128 bits)
            key = key.PadRight(16, '\0').Substring(0, 16);

            Console.WriteLine("Select encryption mode:");
            Console.WriteLine("1. ECB");
            Console.WriteLine("2. CBC");
            Console.WriteLine("3. CFB");
            Console.WriteLine("4. OFB");

            CipherMode mode;
            switch (Console.ReadLine())
            {
                case "1":
                    mode = CipherMode.ECB;
                    break;
                case "2":
                    mode = CipherMode.CBC;
                    break;
                case "3":
                    mode = CipherMode.CFB;
                    break;
                case "4":
                    mode = CipherMode.OFB;
                    break;
                default:
                    Console.WriteLine("Invalid mode. Using ECB by default.");
                    mode = CipherMode.ECB;
                    break;
            }

            string decryptedText = Decrypt(encryptedText, key, mode);
            Console.WriteLine("Decrypted Text: " + decryptedText);
        }

        static string Encrypt(string plainText, string key, CipherMode mode)
        {

            // Create AES encryption algorithm instance
            using (Aes aesAlg = Aes.Create())
            {

                // Set the encryption key
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                // Set the encryption mode
                aesAlg.Mode = mode;
                // Generate a random initialization vector (IV)
                aesAlg.GenerateIV();


                // Create encryptor with the specified key and IV
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


                // Create a memory stream to store the encrypted data
                using (MemoryStream msEncrypt = new MemoryStream())
                {

                    // Create a CryptoStream to perform the encryption
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {

                        // Create a StreamWriter to write the data to the CryptoStream
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write the plain text to the StreamWriter
                            swEncrypt.Write(plainText);
                        }
                    }
                    // Combine the IV and encrypted data
                    byte[] ivAndEncryptedText = aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray();
                    // Convert the combined data to base64 string
                    return Convert.ToBase64String(ivAndEncryptedText);
                }
            }
        }

        static string Decrypt(string encryptedText, string key, CipherMode mode)
        {
            // Convert the base64 string to byte array
            byte[] ivAndEncryptedText = Convert.FromBase64String(encryptedText);
            // Extract IV from the combined data
            byte[] iv = ivAndEncryptedText.Take(16).ToArray();
            // Extract encrypted data from the combined data
            byte[] encryptedBytes = ivAndEncryptedText.Skip(16).ToArray();

            // Create AES encryption algorithm instance
            using (Aes aesAlg = Aes.Create())
            {
                // Set the encryption key
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                // Set the IV
                aesAlg.IV = iv;
                // Set the encryption mode
                aesAlg.Mode = mode;

                // Create decryptor with the specified key and IV
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create a memory stream to store the decrypted data
                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    // Create a CryptoStream to perform the decryption
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // Create a StreamReader to read the decrypted data from the CryptoStream
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted data and return as string
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
