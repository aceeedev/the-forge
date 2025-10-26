using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class MyUtils
{
    private const string baseUri = "http://localhost:3000";

    /// <summary>
    /// NOTE: MAKE <T> int if you do not care about callback parameter. it will pass a -1.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="endpoint"></param>
    /// <param name="query"></param>
    /// <param name="onComplete"></param>
    /// <param name="passAsJson"></param>
    /// <returns></returns>
    public static IEnumerator SendGet<T>(string endpoint, string query = null, Action<T> onComplete = null, bool passAsJson = false)
    {
        string url = baseUri + "/" + endpoint;
        if (!string.IsNullOrEmpty(query))
        {
            url += "?text=" + UnityWebRequest.EscapeURL(query);
        }

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            req.downloadHandler = new DownloadHandlerBuffer();

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("GET error: " + req.error);
            }
            else
            {
                Debug.Log("GET response: " + req.downloadHandler.text);

                if (onComplete != null)
                {
                    if (passAsJson)
                    {
                        T obj = JsonUtility.FromJson<T>(req.downloadHandler.text);
                        onComplete(obj);
                    }
                    else
                    {
                        // Ensure T is byte[] or compatible
                        if (typeof(T) == typeof(byte[]))
                        {
                            onComplete((T)(object)req.downloadHandler.data);
                        }
                        else if (typeof(T) == typeof(int))
                        {
                            onComplete((T)(object)-1);
                        }
                        else
                        {
                            Debug.LogError("Cannot pass raw data to type " + typeof(T));
                        }
                    }
                }
            }
        }
    }

    public static void SetActiveChildren(GameObject parent, bool value)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(value);
        }
    }
}
