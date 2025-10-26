using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor.Animations;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Moves
{
    public string move_1;
    public string move_2;
    public string move_3;
    public string move_4;
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

        StartCoroutine(SendPost("generate-move-descriptions", JsonUtility.ToJson(container), (response) =>
        {
            Moves moves = response as Moves;
            
            foreach (Transform child in player1ActionMenuObject)
            {
                
            }
        }));
    }


    private IEnumerator SendPost(string endpoint, string payload, Action<dynamic> onComplete)
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
                onComplete?.Invoke(JsonUtility.FromJson<Moves>(req.downloadHandler.text));
            }
        }
    }
}