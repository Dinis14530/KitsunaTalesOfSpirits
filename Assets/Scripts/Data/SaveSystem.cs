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

    private static readonly string LegacySavePath = Path.Combine(
        Application.persistentDataPath,
        "save.dat.legacy"
    );

    private const int IvSize = 16;

    // XOR-obfuscated key bytes (deobfuscated at runtime).
    // Original key kept for backward-compatible migration of existing saves.
    private static readonly byte[] KeyMask =
    {
        0xAA,
        0xBB,
        0xCC,
        0xDD,
        0xEE,
        0xFF,
        0x11,
        0x22,
        0x33,
        0x44,
        0x55,
        0x66,
        0x77,
        0x88,
        0x99,
        0x00,
    };
    private static readonly byte[] ObfuscatedKey = GenerateObfuscatedKey();

    private static byte[] GenerateObfuscatedKey()
    {
        // "iAyQ5yE4d8cvqt4Q" encoded with XOR mask
        byte[] raw = Encoding.UTF8.GetBytes("iAyQ5yE4d8cvqt4Q");
        byte[] obfuscated = new byte[raw.Length];
        for (int i = 0; i < raw.Length; i++)
            obfuscated[i] = (byte)(raw[i] ^ KeyMask[i % KeyMask.Length]);
        return obfuscated;
    }

    private static byte[] GetKey()
    {
        byte[] key = new byte[ObfuscatedKey.Length];
        for (int i = 0; i < ObfuscatedKey.Length; i++)
            key[i] = (byte)(ObfuscatedKey[i] ^ KeyMask[i % KeyMask.Length]);
        return key;
    }

    // PUBLIC API
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        WriteSaveFile(json);

#if UNITY_EDITOR
        Debug.Log("SAVE JSON:\n" + json);
        Debug.Log("Jogo guardado em: " + SavePath);
#endif
    }

    public static Task SaveAsync(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        return Task.Run(() => WriteSaveFile(json));
    }

    public static SaveData Load()
    {
        if (!HasSave())
            return null;

        byte[] fileData = File.ReadAllBytes(SavePath);
        string json;

        if (fileData.Length > IvSize && IsNewFormat(fileData))
        {
            json = Decrypt(fileData);
        }
        else
        {
            json = DecryptLegacy(fileData);
            // Re-encrypt with random IV for future loads
            WriteSaveFile(json);
        }

#if UNITY_EDITOR
        Debug.Log("LOAD JSON:\n" + json);
        Debug.Log("Jogo carregado");
#endif

        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    // ENCRYPTION (random IV prepended to ciphertext)
    private static byte[] Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = GetKey();
        aes.GenerateIV();

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(plainBytes, 0, plainBytes.Length);
            cs.FlushFinalBlock();
        }
        return ms.ToArray();
    }

    private static string Decrypt(byte[] data)
    {
        using var aes = Aes.Create();
        aes.Key = GetKey();

        byte[] iv = new byte[IvSize];
        Array.Copy(data, 0, iv, 0, IvSize);
        aes.IV = iv;

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cs.Write(data, IvSize, data.Length - IvSize);
            cs.FlushFinalBlock();
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    // Legacy decryption for saves created before random-IV migration
    private static string DecryptLegacy(byte[] cipherBytes)
    {
        using var aes = Aes.Create();
        aes.Key = GetKey();
        aes.IV = new byte[IvSize];

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.FlushFinalBlock();
        }

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private static bool IsNewFormat(byte[] data)
    {
        // New format: IV (16 bytes) + AES ciphertext (multiple of 16).
        // Legacy format: raw AES ciphertext with zero IV (also multiple of 16).
        // Heuristic: attempt new-format decrypt; fall back to legacy on failure.
        try
        {
            Decrypt(data);
            return true;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }

    private static void WriteSaveFile(string json)
    {
        byte[] encryptedData = Encrypt(json);
        File.WriteAllBytes(SavePath, encryptedData);
    }
}
