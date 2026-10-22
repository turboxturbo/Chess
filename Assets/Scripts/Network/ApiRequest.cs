using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) => true;
}

public static class ApiRequest
{
    private static string _baseUrl = "http://localhost:5171/api/";
    public static void Login(Action<LoginAnswer> callback, LoginRequest login)
    {
        GameManager.Instance.StartCoroutine(LoginCoroutine(callback, login));
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
}

