using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEditor.Search;


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
    public GameObject LoadingObject;

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
        LoadingObject.SetActive(loading);
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

        // setup the situation
        yield return StartCoroutine(SendGet<string>("prompt-response", $"The situation is: {DeckManager.inst.situation}"));

        // generate moves
        yield return GenerateMoveDescriptions(1);
        yield return GenerateMoveDescriptions(2);

        // generate images
        // yield return GenerateCharacterImage(1);
        // yield return GenerateCharacterImage(2);

        loading = false;
    }

    private IEnumerator GenerateMoveDescriptions(int playerNum)
    {
        string cardsToString = playerNum == 1 ? DeckManager.inst.player1Deck.cardsToString() : DeckManager.inst.player2Deck.cardsToString();

        string query =
                    $"The player has has: {cardsToString}\n" +
                    "------\n" +
                    $"Phase: {GameManager.inst.currentActionPhase.ToString()}";


        bool done = false;

        yield return StartCoroutine(SendGet("generate-move-descriptions", query, (ResponseWrapper response) =>
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

            done = true;
        }));

        yield return new WaitUntil(() => done);
    }
    
    private IEnumerator GenerateCharacterImage(int playerNum)
    {
        string cardsToString = playerNum == 1 ? DeckManager.inst.player1Deck.cardsToString() : DeckManager.inst.player2Deck.cardsToString();
        
        string query =
                    $"The player has has: {cardsToString}\n" +
                    "------\n" +
                    $"Phase: {GameManager.inst.currentActionPhase.ToString()}";
        

        bool done = false;

        yield return StartCoroutine(SendGet("generate-move-descriptions", query, (ResponseWrapper response) =>
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

            done = true;
        }));

        yield return new WaitUntil(() => done);
    }


    private IEnumerator SendGet<T>(string endpoint, string query = null, Action<T> onComplete = null)
    {
    // Build the full URL with optional query parameter
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
                onComplete(JsonUtility.FromJson<T>(req.downloadHandler.text));
            }
        }
    }
}

}