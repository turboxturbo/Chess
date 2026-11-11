using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) => true;
}

public static class ApiRequest
{
    private static string _baseUrl = "https://localhost:7206/api/";
    public static void Login(Action<LoginAnswer> callback, LoginRequest login)
    {
        GameManager.Instance.StartCoroutine(LoginCoroutine(callback, login));
    }

    public static void Reg(Action<RegAnswer> callback, RegRequest login)
    {
        GameManager.Instance.StartCoroutine(RegCoroutine(callback, login));
    }

    public static void StartGame(Action<GameStartAnswer> callback, GameStartRequest game)
    {
        GameManager.Instance.StartCoroutine(StartGameCoroutine(callback, game));
    }

    public static void EndGame(Action<GameEndAnswer> callback, int gameId)
    {
        GameManager.Instance.StartCoroutine(EndGameCoroutine(callback, gameId));
    }

    public static IEnumerator LoginCoroutine(Action<LoginAnswer> callback, LoginRequest login)
    {
        string jsonData = JsonUtility.ToJson(login);
        Debug.Log($"Login request: {jsonData}");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        using UnityWebRequest request = new UnityWebRequest(_baseUrl + "User/Auth", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Login response: {request.downloadHandler.text}");
            callback?.Invoke(JsonUtility.FromJson<LoginAnswer>(request.downloadHandler.text));
        }
        else
        {
            Debug.LogError($"Login error: {request.error} | {request.downloadHandler.text}");
            callback?.Invoke(null);
        }
    }

    public static IEnumerator RegCoroutine(Action<RegAnswer> callback, RegRequest reg)
    {
        string jsonData = JsonUtility.ToJson(reg);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        using UnityWebRequest request = new UnityWebRequest(_baseUrl + "User/Reg", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(JsonUtility.FromJson<RegAnswer>(request.downloadHandler.text));
        }
        else
        {
            Debug.LogError($"Reg error: {request.error}");
            callback?.Invoke(null);
        }
    }

    private static IEnumerator StartGameCoroutine(
        Action<GameStartAnswer> callback,
        GameStartRequest game)
    {
        string jsonData = JsonUtility.ToJson(game);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using UnityWebRequest request = new UnityWebRequest(_baseUrl + "Game/StartGame", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Unable to create game: {request.error}");
            yield break;
        }

        callback?.Invoke(JsonUtility.FromJson<GameStartAnswer>(request.downloadHandler.text));
    }

    private static IEnumerator EndGameCoroutine(Action<GameEndAnswer> callback, int gameId)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(gameId.ToString());

        using UnityWebRequest request = new UnityWebRequest(_baseUrl + "Game/Endgame", "PUT");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new AcceptAllCertificates();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Unable to end game: {request.error}");
            yield break;
        }

        callback?.Invoke(JsonUtility.FromJson<GameEndAnswer>(request.downloadHandler.text));
    }
}

