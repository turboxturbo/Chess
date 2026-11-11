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
    private static string _baseUrl = "http://localhost:5095/api/";
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
        Debug.Log(jsonData);
        byte[] bodyRow = System.Text.Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest request = new UnityWebRequest(_baseUrl + "MotoristCotroller/auth", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRow);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.certificateHandler = new AcceptAllCertificates();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success) //толькой 200, поэтому в Api используем только OkObjectResult. Остальные выдают ошибку
            {
                string jsonResponse = request.downloadHandler.text;
                LoginAnswer answer = JsonUtility.FromJson<LoginAnswer>(jsonResponse);
                Debug.Log(answer);
                callback?.Invoke(answer);
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    public static IEnumerator RegCoroutine(Action<RegAnswer> callback, RegRequest login)
    {
        string jsonData = JsonUtility.ToJson(login);
        Debug.Log(jsonData);
        byte[] bodyRow = System.Text.Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest request = new UnityWebRequest(_baseUrl + "MotoristCotroller/auth", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRow);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.certificateHandler = new AcceptAllCertificates();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success) //толькой 200, поэтому в Api используем только OkObjectResult. Остальные выдают ошибку
            {
                string jsonResponse = request.downloadHandler.text;
                RegAnswer answer = JsonUtility.FromJson<RegAnswer>(jsonResponse);
                Debug.Log(answer);
                callback?.Invoke(answer);
            }
            else
            {
                Debug.Log(request.error);
            }
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

