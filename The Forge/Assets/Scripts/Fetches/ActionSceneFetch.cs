using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ActionSceneFetch : MonoBehaviour
{
    public GameObject DialogueObject;

    public GameObject player1ActionMenuObject;
    public GameObject player2ActionMenuObject;

    public GameObject player1CharacterObject;
    public GameObject player2CharacterObject;

    public bool loading = false;

    private string baseUri = "http://localhost:3000";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadInData());
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class MessageContainer
    {
        public Message[] messages;
    }

    public IEnumerator LoadInData()
    {
        loading = true;


        // Build object graph and convert to JSON
        var msg = new Message { role = "user", content = "say hi!" };
        var container = new MessageContainer { messages = new[] { msg } };

        yield return StartCoroutine(SendPost("prompt-response", JsonUtility.ToJson(container), (response) =>
        {
            Debug.Log("POST response (callback): " + response);
            // handle response here
        }));

        loading = false;
    }


    public IEnumerator SendPost(string endpoint, string payload, Action<string> onComplete)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        using (UnityWebRequest req = new UnityWebRequest(baseUri + "/" + endpoint, "POST"))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            req.uploadHandler = new UploadHandlerRaw(bodyRaw) { contentType = "application/json" };
            req.downloadHandler = new DownloadHandlerBuffer();

            yield return req.SendWebRequest();

            string result = null;
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("POST error: " + req.error);
            }
            else
            {
                result = req.downloadHandler.text;
            }

            onComplete?.Invoke(result);
        }
    }
}