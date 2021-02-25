#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BUtility;

#endregion

namespace Sobees.Tools.Crypto
{
  public class EncryptionHelper
  {
    public static byte[] GetHashKey(string hashKey)
    {
      // Initialise
      var encoder = new UTF8Encoding();

      // Get the salt
      const string salt = "I am a nice little salt";
      var saltBytes = encoder.GetBytes(salt);

      // Setup the hasher
      var rfc = new Rfc2898DeriveBytes(hashKey, saltBytes);

      // Return the key
      return rfc.GetBytes(16);
    }

    public static string Encrypt(byte[] key, string dataToEncrypt)
    {
      if (dataToEncrypt == null)
      {
        return "";
      }
      // Initialise
      var encryptor = new AesManaged
                        {
// Set the key
                          Key = key,
                          IV = key
                        };

      // create a memory stream
      using (var encryptionStream = new MemoryStream())
      {
        // Create the crypto stream
        using (var encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        {
          // Encrypt
          var utfD1 = Encoding.UTF8.GetBytes(dataToEncrypt);
          encrypt.Write(utfD1, 0, utfD1.Length);
          encrypt.FlushFinalBlock();
          var result = Convert.ToBase64String(encryptionStream.ToArray());
          encrypt.Close();
          encryptionStream.Close();
          // Return the encrypted data
          return result;
        }
      }
    }

#if !SILVERLIGHT
    public static string Decrypt(string CipherText)
    {
      var passPhrase = "Pas5pr@se"; // can be any string
      var saltValue = "s@1tValue"; // can be any string
      var hashAlgorithm = "SHA1"; // can be "MD5"
      var passwordIterations = 2; // can be any number
      var initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
      var keySize = 256; // can be 192 or 128

      var plainText = Decrypt(CipherText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);

      return plainText;
    }

    /// <summary>
    ///   Decrypts specified ciphertext using Rijndael symmetric key algorithm.
    /// </summary>
    /// <param name = "cipherText">
    ///   Base64-formatted ciphertext value.
    /// </param>
    /// <param name = "passPhrase">
    ///   Passphrase from which a pseudo-random password will be derived. The
    ///   derived password will be used to generate the encryption key.
    ///   Passphrase can be any string. In this example we assume that this
    ///   passphrase is an ASCII string.
    /// </param>
    /// <param name = "saltValue">
    ///   Salt value used along with passphrase to generate password. Salt can
    ///   be any string. In this example we assume that salt is an ASCII string.
    /// </param>
    /// <param name = "hashAlgorithm">
    ///   Hash algorithm used to generate password. Allowed values are: "MD5" and
    ///   "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
    /// </param>
    /// <param name = "passwordIterations">
    ///   Number of iterations used to generate password. One or two iterations
    ///   should be enough.
    /// </param>
    /// <param name = "initVector">
    ///   Initialization vector (or IV). This value is required to encrypt the
    ///   first block of plaintext data. For RijndaelManaged class IV must be
    ///   exactly 16 ASCII characters long.
    /// </param>
    /// <param name = "keySize">
    ///   Size of encryption key in bits. Allowed values are: 128, 192, and 256.
    ///   Longer keys are more secure than shorter keys.
    /// </param>
    /// <returns>
    ///   Decrypted string value.
    /// </returns>
    /// <remarks>
    ///   Most of the logic in this function is similar to the Encrypt
    ///   logic. In order for decryption to work, all parameters of this function
    ///   - except cipherText value - must match the corresponding parameters of
    ///   the Encrypt function which was called to generate the
    ///   ciphertext.
    /// </remarks>
    public static string Decrypt(string cipherText,
                                 string passPhrase,
                                 string saltValue,
                                 string hashAlgorithm,
                                 int passwordIterations,
                                 string initVector,
                                 int keySize)
    {
      var plainText = string.Empty;
      //
      try
      {
        // Convert strings defining encryption key characteristics into byte
        // arrays. Let us assume that strings only contain ASCII codes.
        // If strings include Unicode characters, use Unicode, UTF7, or UTF8
        // encoding.
        var initVectorBytes = Encoding.ASCII.GetBytes(initVector);
        var saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

        // Convert our ciphertext into a byte array.
        var cipherTextBytes = Convert.FromBase64String(cipherText);

        // First, we must create a password, from which the key will be 
        // derived. This password will be generated from the specified 
        // passphrase and salt value. The password will be created using
        // the specified hash algorithm. Password creation can be done in
        // several iterations.
        var password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

        // Use the password to generate pseudo-random bytes for the encryption
        // key. Specify the size of the key in bytes (instead of bits).
        var keyBytes = password.GetBytes(keySize/8);

        // Create uninitialized Rijndael encryption object.
        var symmetricKey = new RijndaelManaged();

        // It is reasonable to set encryption mode to Cipher Block Chaining
        // (CBC). Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC;

        // Generate decryptor from the existing key bytes and initialization 
        // vector. Key size will be defined based on the number of the key 
        // bytes.
        var decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

        // Define memory stream which will be used to hold encrypted data.
        var memoryStream = new MemoryStream(cipherTextBytes);

        // Define cryptographic stream (always use Read mode for encryption).
        var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        // Since at this point we don't know what the size of decrypted data
        // will be, allocate the buffer long enough to hold ciphertext;
        // plaintext is never longer than ciphertext.
        var plainTextBytes = new byte[cipherTextBytes.Length];

        // Start decrypting.
        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

        // Close both streams.
        memoryStream.Close();
        cryptoStream.Close();

        // Convert decrypted data into a string. 
        // Let us assume that the original plaintext string was UTF8-encoded.
        plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry("CryptoHelper::Decrypt:", ex);
      }
      // Return decrypted string.   
      return plainText;
    }
#endif


    /// <summary>
    /// Decrypt
    /// </summary>
    /// <param name="key"></param>
    /// <param name="encryptedString"></param>
    /// <param name="bMustReturnIfNotEncrypted"></param>
    /// <returns></returns>
    public static string Decrypt(byte[] key, string encryptedString, bool bMustReturnIfNotEncrypted)
    {
      try
      {
        if (string.IsNullOrEmpty(encryptedString))
        {
          return string.Empty;
        }
        // Initialise
        var decryptor = new AesManaged();
        var encryptedData = Convert.FromBase64String(encryptedString);

        // Set the key
        decryptor.Key = key;
        decryptor.IV = key;

        // create a memory stream
        using (var decryptionStream = new MemoryStream())
        {
          // Create the crypto stream
          using (var decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
          {
            // Encrypt
            decrypt.Write(encryptedData, 0, encryptedData.Length);
            decrypt.Flush();
            decrypt.Close();

            // Return the unencrypted data
            var decryptedData = decryptionStream.ToArray();
            var result =  Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
            return result;
          }
        }
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(null, ex);
        //For unencrypred password
        if ( bMustReturnIfNotEncrypted)
            return encryptedString;
      }
      return string.Empty;
    }

    public static string Decrypt(byte[] key, string encryptedString)
    {
      return Decrypt(key, encryptedString, false);
    }

    #region Encryption for cross usage between .NET & IOS
#if !SILVERLIGHT
    /// <summary>
    ///   Encrpyts the sourceString, returns this result as an Aes encrpyted, BASE64 encoded string
    /// </summary>
    /// <param name = "plainSourceStringToEncrypt">a plain, Framework string (ASCII, null terminated)</param>
    /// <param name = "passPhrase">The pass phrase.</param>
    /// <returns>
    ///   returns an Aes encrypted, BASE64 encoded string
    /// </returns>
    public static string EncryptString(string plainSourceStringToEncrypt,
                                       string passPhrase)
    {
      //Set up the encryption objects
      using (var acsp = GetProvider(Encoding.Default.GetBytes(passPhrase)))
      {
        var sourceBytes = Encoding.ASCII.GetBytes(plainSourceStringToEncrypt);
        var ictE = acsp.CreateEncryptor();

        //Set up stream to contain the encryption
        var msS = new MemoryStream();

        //Perform the encrpytion, storing output into the stream
        var csS = new CryptoStream(msS, ictE, CryptoStreamMode.Write);
        csS.Write(sourceBytes, 0, sourceBytes.Length);
        csS.FlushFinalBlock();

        //sourceBytes are now encrypted as an array of secure bytes
        var encryptedBytes = msS.ToArray(); //.ToArray() is important, don't mess with the buffer

        //return the encrypted bytes as a BASE64 encoded string
        return Convert.ToBase64String(encryptedBytes);
      }
    }


    /// <summary>
    ///   Decrypts a BASE64 encoded string of encrypted data, returns a plain string
    /// </summary>
    /// <param name = "base64StringToDecrypt">an Aes encrypted AND base64 encoded string</param>
    /// <param name = "passphrase">The passphrase.</param>
    /// <returns>returns a plain string</returns>
    public static string DecryptString(string base64StringToDecrypt,
                                       string passphrase)
    {
      //Set up the encryption objects
      using (var acsp = GetProvider(Encoding.Default.GetBytes(passphrase)))
      {
        var RawBytes = Convert.FromBase64String(base64StringToDecrypt);
        var ictD = acsp.CreateDecryptor();

        //RawBytes now contains original byte array, still in Encrypted state

        //Decrypt into stream
        var msD = new MemoryStream(RawBytes, 0, RawBytes.Length);
        var csD = new CryptoStream(msD, ictD, CryptoStreamMode.Read);
        //csD now contains original byte array, fully decrypted

        //return the content of msD as a regular string
        return (new StreamReader(csD)).ReadToEnd();
      }
    }

    private static AesCryptoServiceProvider GetProvider(byte[] key)
    {
      var result = new AesCryptoServiceProvider();
      result.BlockSize = 128;
      result.KeySize = 128;
      result.Mode = CipherMode.CBC;
      result.Padding = PaddingMode.PKCS7;

      result.GenerateIV();
      result.IV = new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

      var RealKey = GetKey(key, result);
      result.Key = RealKey;
      // result.IV = RealKey;
      return result;
    }

    private static byte[] GetKey(byte[] suggestedKey,
                                 SymmetricAlgorithm p)
    {
      var kRaw = suggestedKey;
      var kList = new List<byte>();

      for (var i = 0; i < p.LegalKeySizes[0].MinSize; i += 8)
      {
        kList.Add(kRaw[(i/8)%kRaw.Length]);
      }
      var k = kList.ToArray();
      return k;
    }
#endif
#endregion


  }
}