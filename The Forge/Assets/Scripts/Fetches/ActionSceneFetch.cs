using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


[System.Serializable]
public class ResponseWrapper
{
    public string response;
}

[System.Serializable]
public class Moves
{
    public string move_1;
    public string move_2;
    public string move_3;
    public string move_4;

    public string[] ToArray() => new[] { move_1, move_2, move_3, move_4 };
}


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

        GenerateMoveDescriptions(1);
        GenerateMoveDescriptions(2);

        yield return

        loading = false;
    }

    private void GenerateMoveDescriptions(int playerNum)
    {
        string cardsToString = playerNum == 1 ? DeckManager.inst.player1Deck.cardsToString() : DeckManager.inst.player2Deck.cardsToString();

        var container = new MessageContainer
        {
            messages = new[] {
            new Message {
                role = "system",
                content =  "You are the narrator for an epic battle game called 'The Forge'. " +
                            "Players create characters by combining attribute cards, and you determine " +
                            "the outcome of a certain situation through storytelling. Your responses should be " +
                            "creative, engaging, and consider how different attributes would interact in " +
                            "the situation. Focus on creating dramatic moments and unexpected twists based on " +
                            "the characters' unique combinations of attributes. Keep responses concise " +
                            "but vivid, around 7 words per move description."
            },
            new Message {
                role = "user",
                content =
                        $"The player has has: {cardsToString}\n" +
                        "------\n" +
                        $"Situation: {DeckManager.inst.situation}\n" +
                        $"Phase: {GameManager.inst.currentActionPhase.ToString()}"
            },
        }
        };

        StartCoroutine(SendPost<ResponseWrapper>("generate-move-descriptions", JsonUtility.ToJson(container), (ResponseWrapper response) =>
        {
            if (response == null)
            {
                Debug.LogError("Failed to parse moves");
                return;
            }

            string[] moves = JsonUtility.FromJson<Moves>(response.response).ToArray();

            int index = 0;
            foreach (Transform child in (playerNum == 1 ? player1ActionMenuObject : player2ActionMenuObject).transform)
            {
                child.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = moves[index];

                index++;
            }
        }));
    }


    private IEnumerator SendPost<T>(string endpoint, string payload, Action<T> onComplete)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        using (UnityWebRequest req = new UnityWebRequest(baseUri + "/" + endpoint, "POST"))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            req.uploadHandler = new UploadHandlerRaw(bodyRaw) { contentType = "application/json" };
            req.downloadHandler = new DownloadHandlerBuffer();

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("POST error: " + req.error);
            }
            else
            {
                Debug.Log(req.downloadHandler.text);

                onComplete?.Invoke(JsonUtility.FromJson<T>(req.downloadHandler.text));
            }
        }
    }
}