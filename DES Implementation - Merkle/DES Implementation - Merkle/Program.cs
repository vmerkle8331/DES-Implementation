using System;
using System.IO; 
using System.Security.Cryptography; 
using System.Text; 
using System.Linq; 

class Program
{
 
    static void Main(string[] args)
    {
        // Define the input and output file names
        string[] inputFiles = { "class_input_2A.txt", "class_input_2B.txt", "class_input_2C.txt", "Merkle_input_selftest.txt" };
        string[] outputFiles = { "Merkle_output_2A", "Merkle_output_2B", "Merkle_output_2C", "Merkle_output_selftest" };

        // Loop through the input and output files using the Zip method to combine them into tuples
        foreach (var (inputFile, outputFile) in inputFiles.Zip(outputFiles, (input, output) => (input, output)))
        {
            // Read the input file and extract the number of rounds, key, and plaintext
            var (rounds, key, plaintext) = ReadInputFile(inputFile);

            // Encrypt the plaintext using the DES algorithm with the specified number of rounds and key
            var ciphertext = Encrypt(rounds, key, plaintext);

            // Write the ciphertext to the output file
            WriteOutputFile(outputFile, ciphertext);
        }
    }

    // Define a method to read the input file and extract the number of rounds, key, and plaintext
    static (int rounds, string key, string plaintext) ReadInputFile(string inputFile)
    {
        // Read all lines from the input file
        var lines = File.ReadAllLines(inputFile);

        // Extract the number of rounds, key, and plaintext from the input file
        var rounds = int.Parse(lines[0]);
        var key = lines[1];
        var plaintext = lines[2];

        // Return the number of rounds, key, and plaintext as a tuple
        return (rounds, key, plaintext);
    }

    // Define a method to encrypt the plaintext using the DES algorithm with the specified number of rounds and key
    static byte[] Encrypt(int rounds, string key, string plaintext)
    {
        // Create a new DES encryption provider and set the key and initialization vector
        using var des = new DESCryptoServiceProvider { Key = HexToByteArray(key), IV = HexToByteArray(plaintext) };

        // Encrypt the plaintext using the DES encryption provider
        var ciphertext = des.CreateEncryptor().TransformFinalBlock(HexToByteArray(plaintext), 0, plaintext.Length / 2);

        // Apply the specified number of rounds to the ciphertext
        for (int i = 0; i < rounds; i++)
        {
            var temp = new byte[ciphertext.Length];
            Buffer.BlockCopy(ciphertext, 0, temp, 0, ciphertext.Length);
            ciphertext = des.CreateEncryptor().TransformFinalBlock(temp, 0, temp.Length);
        }

        // Return the ciphertext
        return ciphertext;
    }

    // Define a method to write the ciphertext to the output file
    static void WriteOutputFile(string outputFile, byte[] ciphertext)
    {
        // Write the ciphertext to the output file as a string of hexadecimal digits
        File.WriteAllText(outputFile, BitConverter.ToString(ciphertext).Replace("-", ""));
    }

    // Define a method to convert a hexadecimal string to a byte array
    static byte[] HexToByteArray(string hex)
    {
        // Create a new byte array with half the length of the hexadecimal string
        var bytes = new byte[hex.Length / 2];

        // Loop through the hexadecimal string and convert each pair of digits to a byte
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        // Return the byte array
        return bytes;
    }
}