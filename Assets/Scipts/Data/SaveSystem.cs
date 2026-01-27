using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string path =>
        Application.persistentDataPath + "/save.json";
        //C:/Users/Dinis/AppData/LocalLow/DefaultCompany/2DGame

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Jogo guardado em: " + path);
    }

    public static SaveData Load()
    {
        string json = File.ReadAllText(path);
        Debug.Log("Jogo carregado");
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave()
    {
        return File.Exists(path);
    }
}
