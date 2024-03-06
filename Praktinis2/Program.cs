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
                Console.WriteLine("Pasirinkite ka norite daryti:");
                Console.WriteLine("1. Encrypt text.");
                Console.WriteLine("2. Decrypt text.");
                Console.WriteLine("3. Exit.");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                    continue;
                }

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
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Mode = mode;
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    byte[] ivAndEncryptedText = aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray();
                    return Convert.ToBase64String(ivAndEncryptedText);
                }
            }
        }

        static string Decrypt(string encryptedText, string key, CipherMode mode)
        {
            byte[] ivAndEncryptedText = Convert.FromBase64String(encryptedText);
            byte[] iv = ivAndEncryptedText.Take(16).ToArray();
            byte[] encryptedBytes = ivAndEncryptedText.Skip(16).ToArray();

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = iv;
                aesAlg.Mode = mode;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
