using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CommerceBuilder.Data;
using CommerceBuilder.Configuration;
using CommerceBuilder.Common;
using CommerceBuilder.Exceptions;
using CommerceBuilder.Utility;
using CommerceBuilder.Payments;
using CommerceBuilder.Stores;

namespace CommerceBuilder.Utility
{
    /// <summary>
    /// Utility class for help in encryption
    /// </summary>
    public static class EncryptionHelper
    {
        private static byte[] GetKeyFromPassPhrase(string passPhrase)
        {
            //GENERATE 256 BIT KEY FROM PASS PHRASE
            if (string.IsNullOrEmpty(passPhrase)) return null;
            byte[] salt = { 0, 1, 2, 3, 4, 5, 6, 7 };
            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, salt, 1024);
            return password.GetBytes(32);
        }

        /// <summary>
        /// Sets the encryption key for the current application
        /// </summary>
        /// <param name="passPhrase">A random string or password that will help form the key.</param>
        /// <returns></returns>
        public static byte[] SetEncryptionKey(string passPhrase)
        {
            //GET THE ABLECOMMERCE SETTINGS
            //IF CONFIG IS NULL, WE DO NOT HAVE AN "UPDATABLE" SECTION
            System.Configuration.Configuration updateableConfig = null;
            AbleCommerceEncryptionSection encryptionSection = null;
            try
            {
                encryptionSection = AbleCommerceEncryptionSection.GetUpdatableSection(out updateableConfig);
            }
            catch { }
            if (encryptionSection == null) encryptionSection = AbleCommerceEncryptionSection.GetSection();

            //GENERATE THE KEY
            byte[] newKey = GetKeyFromPassPhrase(passPhrase);

            //SAVE THE EXISTING KEY
            byte[] oldKey = encryptionSection.EncryptionKey.GetKey();
            encryptionSection.OldEncryptionKey.SetKey(oldKey);
            //PRESERVE THE OLD CREATE DATE
            encryptionSection.OldEncryptionKey.CreateDate = encryptionSection.EncryptionKey.CreateDate;

            //UPDATE THE KEY
            encryptionSection.EncryptionKey.SetKey(newKey);

            //SET THE RECRYPT FLAG
            RecryptionHelper.SetRecryptionFlag(true);

            //SAVE SETTINGS
            AbleCommerceEncryptionSection.UpdateConfig(updateableConfig, encryptionSection);

            //RECRYPT EXISTING ACCOUNT DATA
            RecryptionHelper.RecryptDatabase(oldKey, newKey);

            //RETURN THE GENERATED KEY
            return newKey;
        }

        private static void InternalRestoreBackupKey(byte[] keyData)
        {
            //GET THE ABLECOMMERCE SETTINGS
            //IF CONFIG IS NULL, WE DO NOT HAVE AN "UPDATABLE" SECTION
            System.Configuration.Configuration updateableConfig = null;
            AbleCommerceEncryptionSection encryptionSection = null;
            try
            {
                encryptionSection = AbleCommerceEncryptionSection.GetUpdatableSection(out updateableConfig);
            }
            catch { }
            if (encryptionSection == null) encryptionSection = AbleCommerceEncryptionSection.GetSection();

            //UPDATE THE KEY
            encryptionSection.EncryptionKey.SetKey(keyData);

            //SAVE SETTINGS
            AbleCommerceEncryptionSection.UpdateConfig(updateableConfig, encryptionSection);
        }

        /// <summary>
        /// Gets the encryption key from AbleCommerce settings
        /// </summary>
        /// <returns>The encryption key</returns>
        public static byte[] GetEncryptionKey()
        {
            //GET THE ABLECOMMERCE SETTINGS
            AbleCommerceEncryptionSection encryptionConfig = AbleCommerceEncryptionSection.GetSection();

            //RETURN THE KEY
            return encryptionConfig.EncryptionKey.GetKey();
        }

        /// <summary>
        /// Gets the backup key for given part number
        /// </summary>
        /// <param name="partNumber">The part number which is eithr '1' or '2'</param>
        /// <returns>The backup key</returns>
        public static byte[] GetBackupKey(int partNumber)
        {
            //ENSURE THE PERSON REQUESTING IS ALLOWED
            if (!Token.Instance.User.IsInRole(CommerceBuilder.Users.Role.SystemAdminRoles))
            {
                Logger.Audit(AuditEventType.BackupEncryptionKey, false, "Unauthorized Access");
                throw new SecurityException("This unauthorized access attempt has been logged.");
            }

            //VALIDATE PART NUMBER
            if ((partNumber != 1) && (partNumber != 2))
            {
                Logger.Audit(AuditEventType.BackupEncryptionKey, false, "Invalid Part Requested");
                throw new InvalidOperationException("You must specify part 1 or part 2.");
            }

            //GET THE KEY
            byte[] keyData = EncryptionHelper.GetEncryptionKey();

            //MAKE SURE KEY FITS WITHIN VALID RANGE
            if (!IsKeyValid(keyData))
            {
                Logger.Audit(AuditEventType.BackupEncryptionKey, false, "Key is Missing or Invalid Length");
                throw new InvalidOperationException("The key must be either 16 or 32 bytes in length!");
            }

            //CREATE THE BACKUP PART
            int keyLength = keyData.Length;
            int halfLength = (keyLength / 2);
            byte[] backupKeyData = new byte[halfLength + 3];
            //FIRST BYTE IDENTIFIES REVISION OF BACKUP (IN CASE OF FUTURE CHANGE TO ALGORITHM)
            backupKeyData[0] = 1;
            //SECOND BYTE TELLS TOTAL LENGTH OF RECONSTRUCTED KEY
            backupKeyData[1] = (byte)keyData.Length;
            //THIRD BYTE TELLS WHICH PART THIS IS
            backupKeyData[2] = (byte)partNumber;
            //NOW PUT IN THE BYTES OF KEY DATA
            if (partNumber == 1)
            {
                //FIRST HALF OF KEY
                System.Buffer.BlockCopy(keyData, 0, backupKeyData, 3, halfLength);
            }
            else
            {
                //SECOND HALF OF KEY
                System.Buffer.BlockCopy(keyData, halfLength, backupKeyData, 3, halfLength);
            }

            //LOG SUCCESS
            Logger.Audit(AuditEventType.BackupEncryptionKey, true, "Part " + partNumber.ToString());
            return backupKeyData;
        }

        /// <summary>
        /// Restores the backup key from the backup
        /// </summary>
        /// <param name="firstBackup">The first backup</param>
        /// <param name="secondBackup">The second backup</param>
        /// <returns></returns>
        public static bool RestoreBackupKey(byte[] firstBackup, byte[] secondBackup)
        {
            //ENSURE THE PERSON REQUESTING IS ALLOWED
            if (!Token.Instance.User.IsInRole("system"))
            {
                Logger.Audit(AuditEventType.RestoreEncryptionKey, false, "Unauthorized Access");
                throw new SecurityException("Key backup is unauthorized, you are not in the system role.");
            }

            //VALIDATE THE BACKUP PARTS
            if ((!IsBackupPartValid(firstBackup)) || (!IsBackupPartValid(secondBackup)) || (firstBackup.Length != secondBackup.Length))
            {
                Logger.Audit(AuditEventType.RestoreEncryptionKey, false, "Invalid Backup Part");
                throw new InvalidOperationException("One of the backup parts is missing or has an invalid length.");
            }

            //VERIFY THE VERSIONS
            int firstVersion = (int)firstBackup[0];
            int secondVersion = (int)secondBackup[0];
            if (firstVersion != secondVersion)
            {
                Logger.Audit(AuditEventType.RestoreEncryptionKey, false, "Backup Part Version Mismatch");
                throw new InvalidOperationException("The backup parts do not have matching versions.");
            }

            //VERIFY THE PARTS
            int firstPartNumber = (int)firstBackup[2];
            int secondPartNumber = (int)secondBackup[2];
            if (firstPartNumber == 1)
            {
                if (secondPartNumber != 2)
                {
                    Logger.Audit(AuditEventType.RestoreEncryptionKey, false, "Backup Part Number Mismatch");
                    throw new InvalidOperationException("Both part 1 and part 2 are required restore a backup.");
                }
            }
            else if (firstPartNumber == 2)
            {
                if (secondPartNumber == 1)
                {
                    //SWAP ORDER OF BACKUP DATA
                    byte[] temp = firstBackup;
                    firstBackup = secondBackup;
                    secondBackup = temp;
                }
                else
                {
                    Logger.Audit(AuditEventType.RestoreEncryptionKey, false, "Backup Part Number Mismatch");
                    throw new InvalidOperationException("You must provide parts one and two in order to restore.");
                }
            }

            //GET THE RECONSTRUCTED KEY LENGTH
            int keyLength = (firstBackup.Length - 3) * 2;
            //CREATE AN ARRAY FOR THE RECONSTRUCTED KEY
            byte[] keyData = new byte[keyLength];
            //DETERMINE THE HALFWAY POINT FOR COPYING BACKUP DATA
            int halfLength = (keyLength / 2);

            //COPY IN THE KEY DATA
            System.Buffer.BlockCopy(firstBackup, 3, keyData, 0, halfLength);
            System.Buffer.BlockCopy(secondBackup, 3, keyData, halfLength, halfLength);

            //UPDATE WITH THE RESTORED KEY
            Logger.Audit(AuditEventType.RestoreEncryptionKey, true, "Backup Restored");
            EncryptionHelper.InternalRestoreBackupKey(keyData);

            return true;
        }

        /// <summary>
        /// Determines whether a backup key part is present and has a valid length.
        /// </summary>
        /// <param name="backupData">bytes of backup part data; can be null</param>
        /// <returns>True if the backup part is not null and has a valid length</returns>
        private static bool IsBackupPartValid(byte[] backupData)
        {
            //KEYS OF 16 OR 32 BYTES
            //HALF OF 16 IS 8, PLUS 3 EQUALS 11
            //HALF OF 32 IS 16, PLUS 3 EQUALS 19
            return ((backupData != null) && ((backupData.Length == 11) || backupData.Length == 19));
        }

        /// <summary>
        /// Determines whether a key is present and has a valid length.
        /// </summary>
        /// <param name="keyData">bytes of key data; can be null</param>
        /// <returns>True if the key is not null and has a valid length</returns>
        public static bool IsKeyValid(byte[] keyData)
        {
            return ((keyData != null) && ((keyData.Length == 16) || keyData.Length == 32));
        }

        /// <summary>
        /// Encrypts the given plain text using AES encryption
        /// </summary>
        /// <param name="plainText">The text to encrypt</param>
        /// <returns>The encrypted text</returns>
        public static string EncryptAES(string plainText)
        {
            return EncryptAES(plainText, GetEncryptionKey());
        }

        /// <summary>
        /// Encrypts the given plain text using AES encryption. Gets the encryption key from given pass-phrase.
        /// </summary>
        /// <param name="plainText">The text to encrypt</param>
        /// <param name="passPhrase">The pass-pharse to use</param>
        /// <returns>The encrypted text</returns>
        public static string EncryptAES(string plainText, string passPhrase)
        {
            //GENERATE PASSWORD FROM PARAMETERS
            return EncryptAES(plainText, GetKeyFromPassPhrase(passPhrase));
        }

        /// <summary>
        /// Encrypts the given plain text using AES encryption. Uses the given encryption key data.
        /// </summary>
        /// <param name="plainText">The text to encrypt</param>
        /// <param name="keyBytes">The key to use for encryption</param>
        /// <returns>The encrypted text</returns>
        public static string EncryptAES(string plainText, byte[] keyBytes)
        {
            //DO NOT ENCRYPT EMPTY STRING
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            //DO NOT ENCRYPT IF THERE IS NO CRYPT KEY
            if ((keyBytes == null) || (keyBytes.Length == 0)) return plainText;

            //CONFIGURE AES
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Key = keyBytes;

            //CREATE RANDOM IV
            symmetricKey.GenerateIV();
            byte[] iv = symmetricKey.IV;

            //CREATE ENCRYPTOR
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor();

            //CONVERT PLAIN TEXT TO BYTES
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            //CREATE MEMORY STREAM TO HOLD ENCRYPTED DATA
            byte[] cipherTextBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                //CREATE THE CRYPTO STREAM
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {

                    //ENCRYPT THE MESSAGE
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    //GET ENCRYPTED BYTES FROM MEMORY STREAM
                    cipherTextBytes = memoryStream.ToArray();

                    //CLOSE CRYPTO STREAM
                    cryptoStream.Close();
                }
                //CLOSE MEMORY STREAM
                memoryStream.Close();
            }

            //CONCAT IV AND CIPHER TEXT
            byte[] ivPlusCipher = new byte[iv.Length + cipherTextBytes.Length];
            System.Buffer.BlockCopy(iv, 0, ivPlusCipher, 0, iv.Length);
            System.Buffer.BlockCopy(cipherTextBytes, 0, ivPlusCipher, iv.Length, cipherTextBytes.Length);

            //CONVERT COMBINED BYTES TO BASE64 TEXT
            string cipherText = Convert.ToBase64String(ivPlusCipher);

            //RETURN ENCRYPTED TEXT
            return cipherText;
        }

        /// <summary>
        /// Decrypts the given encrypted text
        /// </summary>
        /// <param name="cipherText">The encrypted text to decrypt</param>
        /// <returns>The decrypted text</returns>
        public static string DecryptAES(string cipherText)
        {
            return DecryptAES(cipherText, GetEncryptionKey());
        }

        /// <summary>
        /// Decrypts the given encrypted text using the key from given pass-phrase.
        /// </summary>
        /// <param name="cipherText">The encrypted text</param>
        /// <param name="passPhrase">The pass-pharse to use</param>
        /// <returns>The decrypted text</returns>
        public static string DecryptAES(string cipherText, string passPhrase)
        {
            //GENERATE PASSWORD FROM PARAMETERS
            return DecryptAES(cipherText, GetKeyFromPassPhrase(passPhrase));
        }

        /// <summary>
        /// Decrypts the given encrypted text using the given key.
        /// </summary>
        /// <param name="cipherText">The encrypted text</param>
        /// <param name="keyBytes">The key to use</param>
        /// <returns>The decrypted text</returns>
        public static string DecryptAES(string cipherText, byte[] keyBytes)
        {
            //DO NOT DECRYPT EMPTY STRING
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            //DO NOT DECRYPT IF THERE IS NO CRYPT KEY
            if ((keyBytes == null) || (keyBytes.Length == 0)) return cipherText;

            // STORAGE FOR DATA DECODED FROM BASE64
            byte[] ivPlusCipher;
            try
            {
                // DECODE THE BASE64 DATA
                ivPlusCipher = Convert.FromBase64String(cipherText);
            }
            catch (System.FormatException)
            {
                // DATA WAS NOT VALID BASE64, IT CANNOT BE DECRYPTED
                return cipherText;
            }

            // THE DECRYPTED DATA MUST BE AT LEAST 17 BYTES (AND PROBABLY LONGER)
            if (ivPlusCipher.Length <= 16)
            {
                return cipherText;
            }

            //CONVERT CIPHER TEXT TO BYTES
            try
            {
                //SPLIT THE IV (FIRST 128 BITS) AND CIPHER TEXT
                byte[] ivBytes = new byte[16];
                byte[] cipherTextBytes = new byte[ivPlusCipher.Length - 16];
                System.Buffer.BlockCopy(ivPlusCipher, 0, ivBytes, 0, 16);
                System.Buffer.BlockCopy(ivPlusCipher, 16, cipherTextBytes, 0, cipherTextBytes.Length);

                //CONFIGURE AES
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Key = keyBytes;
                symmetricKey.IV = ivBytes;

                //CREATE DECRYPTOR
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor();

                //CREATE BUFFER TO HOLD DECRYPTED TEXT
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount;

                //CREATE MEMORY STREAM OF DECRYPTED DATA
                using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                {
                    //CREATE THE CRYPTO STREAM
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        //DECRYPT THE CIPHER TEXT
                        decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                        //CLOSE CRYPTO STREAM
                        cryptoStream.Close();
                    }
                    //CLOSE MEMORY STREAM
                    memoryStream.Close();
                }

                //CONVERT DECRYPTED BYTES TO STRING
                string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                //RETURN DECRYPTED TEXT
                return plainText;
            }
            catch (Exception ex)
            {
                //SOMETHING WENT WRONG, RETURN ORIGINAL VALUE
                string scriptName = HttpContextHelper.GetCurrentScriptName();
                Logger.Debug("Error decrypting value " + cipherText + " in script " + scriptName, ex);
                return cipherText;
            }
        }
    }
}
