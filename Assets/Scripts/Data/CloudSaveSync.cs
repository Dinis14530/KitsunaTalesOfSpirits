using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public static class CloudSaveSync
{
    private const string SaveKey = "save_json";

    private static Task<bool> initializationTask;
    private static bool isReady;

    public static Task<bool> InitializeAsync()
    {
        if (isReady)
            return Task.FromResult(true);

        if (initializationTask != null)
            return initializationTask;

        initializationTask = InitializeInternalAsync();
        return initializationTask;
    }

    public static async Task SaveAsync(SaveData data)
    {
        if (data == null)
            return;

        if (!await InitializeAsync())
            return;

        try
        {
            string json = JsonUtility.ToJson(data, true);

            await CloudSaveService.Instance.Data.Player.SaveAsync(
                new Dictionary<string, object> { { SaveKey, json } }
            );
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Cloud save falhou: " + exception.Message);
        }
    }

    public static async Task<SaveData> LoadAsync()
    {
        if (!await InitializeAsync())
            return null;

        try
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(
                new HashSet<string> { SaveKey }
            );

            if (playerData != null && playerData.TryGetValue(SaveKey, out var saveItem))
            {
                string json = saveItem.Value.GetAs<string>();

                if (!string.IsNullOrWhiteSpace(json))
                    return JsonUtility.FromJson<SaveData>(json);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Cloud load falhou: " + exception.Message);
        }

        return null;
    }

    public static async Task DeleteAsync()
    {
        if (!await InitializeAsync())
            return;

        try
        {
            await CloudSaveService.Instance.Data.Player.DeleteAsync(SaveKey);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Cloud delete falhou: " + exception.Message);
        }
    }

    private static async Task<bool> InitializeInternalAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            isReady = true;
        }
        catch (Exception exception)
        {
            isReady = false;
            Debug.LogWarning("Unity Services indisponível: " + exception.Message);
        }
        finally
        {
            initializationTask = null;
        }

        return isReady;
    }
}
