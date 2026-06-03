using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SavePath = Path.Combine(
        Application.persistentDataPath,
        "save.dat"
    );

    // Chave simples
    private const string EncryptionKey = "iAyQ5yE4d8cvqt4Q";

    // PUBLIC API
    public static void Save(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[SaveSystem] Save called with null data, skipping.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);
            WriteSaveFile(json);

#if UNITY_EDITOR
            Debug.Log("SAVE JSON:\n" + json);
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save: {e}");
        }
    }

    public static Task SaveAsync(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[SaveSystem] SaveAsync called with null data, skipping.");
            return Task.CompletedTask;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);
            return Task.Run(() => WriteSaveFile(json));
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to start async save: {e}");
            return Task.CompletedTask;
        }
    }

    public static SaveData Load()
    {
        if (!HasSave())
            return null;

        try
        {
            byte[] encryptedData = File.ReadAllBytes(SavePath);
            string json = Decrypt(encryptedData);

#if UNITY_EDITOR
            Debug.Log("LOAD JSON:\n" + json);
#endif

            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load save: {e}");
            return null;
        }
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    // ENCRYPTION
    private static byte[] Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        aes.IV = new byte[16];

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(plainBytes, 0, plainBytes.Length);
            cs.FlushFinalBlock();
        }
        return ms.ToArray();
    }

    private static string Decrypt(byte[] cipherBytes)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        aes.IV = new byte[16];

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.FlushFinalBlock();
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private static void WriteSaveFile(string json)
    {
        byte[] encryptedData = Encrypt(json);
        File.WriteAllBytes(SavePath, encryptedData);
    }
}
