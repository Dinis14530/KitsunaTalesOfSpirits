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
        string json = JsonUtility.ToJson(data, true);
        WriteSaveFile(json);

        // DEBUG apenas no Editor
#if UNITY_EDITOR
        Debug.Log("SAVE JSON:\n" + json);
#endif

        Debug.Log("Jogo guardado em: " + SavePath);
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

        byte[] encryptedData = File.ReadAllBytes(SavePath);
        string json = Decrypt(encryptedData);

        // DEBUG
#if UNITY_EDITOR
        Debug.Log("LOAD JSON:\n" + json);
#endif

        Debug.Log("Jogo carregado");
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
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
