using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEditor.Search;
using UnityEngine.UI;
using UnityEditor.Rendering;


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


    public GameObject player1ActionMenuObject;
    public GameObject player2ActionMenuObject;

    public GameObject player1CharacterObject;
    public GameObject player2CharacterObject;

    public bool loading = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().text = "";

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

        while (DeckManager.inst.situationPromptLoading)
        {
            yield return new WaitForSeconds(0.1f);
        }
        // setup the situation
        // yield return StartCoroutine(SendGet<string>("prompt-response", $"The situation is: {DeckManager.inst.situation}"));

        // Start all tasks in parallel
        Coroutine[] tasks = new Coroutine[]
        {
            StartCoroutine(RunGenerateMoves()),
            StartCoroutine(GenerateCharacterImage(1)),
            StartCoroutine(GenerateCharacterImage(2))
        };

        // Wait for all tasks to complete
        foreach (var task in tasks)
        {
            yield return task;
        }

        loading = false;
    }

    private IEnumerator RunGenerateMoves() {
        yield return StartCoroutine(GenerateMoveDescriptions(1));
        yield return StartCoroutine(GenerateMoveDescriptions(2));
    }

    private IEnumerator GenerateMoveDescriptions(int playerNum)
    {
        string cardsToString = playerNum == 1 ? DeckManager.inst.player1Deck.cardsToString() : DeckManager.inst.player2Deck.cardsToString();

        string query =
                    $"The player has has: {cardsToString}\n" +
                    "------\n" +
                    $"Phase: {GameManager.inst.currentActionPhase.ToString()}";


        bool done = false;

        yield return StartCoroutine(MyUtils.SendGet("generate-move-descriptions", query, (ResponseWrapper response) =>
        {
            if (response == null)
            {
                Debug.LogError("Failed to parse moves - response is null");
                return;
            }

            Debug.Log($"Moves JSON to parse: {response.response}");
            
            // Trim whitespace from the JSON string
            string jsonToParse = response.response.Trim();
            
            string[] moves = JsonUtility.FromJson<Moves>(jsonToParse).ToArray();

            int index = 0;
            foreach (Transform child in (playerNum == 1 ? player1ActionMenuObject : player2ActionMenuObject).transform)
            {
                child.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = moves[index];

                index++;
            }

            done = true;
        }, true));

        yield return new WaitUntil(() => done);
    }

    private Texture2D MakeBackgroundTransparent(Texture2D source, Color bgColor, float tolerance = 0.01f)
    {
        Texture2D tex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        Color[] pixels = source.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            Color c = pixels[i];
            float distance = Vector3.Distance(new Vector3(c.r, c.g, c.b), new Vector3(bgColor.r, bgColor.g, bgColor.b));
            if (distance <= tolerance)
                c.a = 0f; // only background becomes transparent

            pixels[i] = c;
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    private Texture2D ApplyFeatheredEdge(Texture2D source, Rect uvRect, float featherAmount = 0.15f)
    {
        int width = source.width;
        int height = source.height;
        
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = source.GetPixels();
        
        int featherPixels = Mathf.Max(1, (int)(Mathf.Min(width, height) * featherAmount));
        
        // Calculate the visible region in pixel coordinates
        int visibleLeft = (int)(uvRect.x * width);
        int visibleRight = (int)((uvRect.x + uvRect.width) * width);
        int visibleBottom = (int)(uvRect.y * height);
        int visibleTop = (int)((uvRect.y + uvRect.height) * height);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                Color c = pixels[index];
                
                // Calculate distance from the VISIBLE edges (after cropping)
                float distLeft = x - visibleLeft;
                float distRight = visibleRight - 1 - x;
                float distTop = visibleTop - 1 - y;
                float distBottom = y - visibleBottom;
                
                // Get minimum distance to any visible edge
                float minDist = Mathf.Min(distLeft, Mathf.Min(distRight, Mathf.Min(distTop, distBottom)));
                
                // Apply smooth feathering
                float alpha = Mathf.Clamp01(minDist / featherPixels);
                alpha = Mathf.SmoothStep(0, 1, alpha); // Smooth the transition
                
                c.a *= alpha;
                pixels[index] = c;
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
    
    private IEnumerator GenerateCharacterImage(int playerNum)
    {
        string cardsToPrompt = playerNum == 1 ? DeckManager.inst.player1Deck.cardsToPrompt() : DeckManager.inst.player2Deck.cardsToPrompt();
        string directionToFace = playerNum == 1 ? "right" : "left";

        string query = $"A colorful and expressive drawing of a character with bold features that is striking an action pose facing {directionToFace} realistically using their {cardsToPrompt}. The character is captured against a pure white background with even lighting and no other text, props, characters, and scenery.";

        bool done = false;

        yield return StartCoroutine(MyUtils.SendGet("generate-image", query, (byte[] response) =>
        {
            if (response == null)
            {
                Debug.LogError("Failed to parse moves");
                return;
            }

            RawImage rawImage = (playerNum == 1 ? player1CharacterObject : player2CharacterObject).GetComponent<RawImage>();

            Texture2D tex = new Texture2D(2, 2); // temporary size, will resize automatically
            if (tex.LoadImage(response)) // loads PNG/JPG into texture
            {
                // Calculate crop region first
                Vector2 targetSize = rawImage.rectTransform.sizeDelta;
                float targetAspect = targetSize.x / targetSize.y;
                float textureAspect = (float)tex.width / tex.height;

                Rect uvRect;
                if (textureAspect > targetAspect)
                {
                    // Texture is wider - crop horizontally
                    float scale = targetAspect / textureAspect;
                    float offset = (1f - scale) / 2f;
                    uvRect = new Rect(offset, 0, scale, 1);
                }
                else
                {
                    // Texture is taller - crop vertically
                    float scale = textureAspect / targetAspect;
                    float offset = (1f - scale) / 2f;
                    uvRect = new Rect(0, offset, 1, scale);
                }

                // Apply feathered edges based on the visible region
                tex = ApplyFeatheredEdge(tex, uvRect, 0.1f);
                
                rawImage.texture = tex;
                rawImage.uvRect = uvRect;
            }

            done = true;
        }));

        yield return new WaitUntil(() => done);
    }
}