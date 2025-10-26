using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class PlayerDeck
{
    public List<string> cards;

    public PlayerDeck()
    {
        cards = Enumerable.Repeat("", 4).ToList();
    }

    public bool IsDeckFull()
    {
        return !cards.Contains("");
    }

    public string cardsToString()
    {
        return $"Move 1: {cards[0]}, Move 2: {cards[1]}, Move 3: {cards[2]}, Move 4: {cards[3]}";
    }
}

public class DeckManager : MonoBehaviour
{
    public List<string> situationCards;
    public List<string> itemCards;
    public List<string> talentCards;
    public List<string> abilityCards;
    public List<string> clothesCards;

    public void Start()
    {
        situationCards = new List<string> {
            "Assemble IKEA furniture without directions",
            "Teach a classroom Physics",
            "Fix a lightbulb",
            "Parallel park a car on a hill",
            "Win a hot dog eating contest"
        };
        itemCards = new List<string> {
            "Nunchucks", "Boomerang", "Balloon", "Frying Pan", "Water Gun",
            "Money Bag", "Deoderant", "Laser Pointer", "Spork", "Torch"
        };
        talentCards = new List<string> {
            "Juggling", "Climbing", "Dancing", "Martial Arts", "Cooking",
            "Programming", "Gardening", "Chess", "Gymnastics", "Singing"
        };
        abilityCards = new List<string> {
            "Teleportation", "Invisibility", "Healing", "Telekinesis", "Super Speed",
            "Fire Breath", "Flight", "X-Ray Vision", "Lava Arms", "Animal Control"
        };
        clothesCards = new List<string> {
            "Tuxedo", "Pajamas", "Cat Ears", "Crown", "Jetpack",
            "Cape", "Magnet Boots", "Halo", "Suit of Armor", "Helmet"
        };
        poolCards = new List<string>();
    }

    public static DeckManager inst;

    public string situation;

    public List<string> poolCards;

    public PlayerDeck player1Deck;
    public PlayerDeck player2Deck;

    /// <summary>
    ///  for checking if its the last turn on rounds of drafts >=2
    /// </summary>
    public bool lastTurn = false;
    private string baseUri = "http://localhost:3000";

    void Awake()
    {
        // live laugh love singleton pattern

        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator SelectSituationCard()
    {
        if (situationCards.Count() == 0) {
            yield break;
        }
        int randomIndex = UnityEngine.Random.Range(0, situationCards.Count());
        situation = situationCards[randomIndex];
        situationCards.RemoveAt(randomIndex);
        yield return StartCoroutine(SendGet<string>("prompt-response", $"The situation is: {DeckManager.inst.situation}"));
    }

    private IEnumerator SendGet<T>(string endpoint, string query = null, Action<T> onComplete = null, bool passAsJson = false)
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
                        else
                        {
                            Debug.LogError("Cannot pass raw data to type " + typeof(T));
                        }
                    }
                }
            }
        }
    }

    public void SelectPoolCards()
    {
        poolCards = new List<string> {
            drawCharacterCard(itemCards), drawCharacterCard(itemCards),
            drawCharacterCard(talentCards), drawCharacterCard(talentCards),
            drawCharacterCard(abilityCards), drawCharacterCard(abilityCards),
            drawCharacterCard(clothesCards), drawCharacterCard(clothesCards)
        };
    }

    private string drawCharacterCard(List<string> deck) {
        if (deck.Count() == 0) {
            return null;
        }
        
        // Pick a random key
        int randomIndex = UnityEngine.Random.Range(0, deck.Count());
        
        // Create result dictionary with one card
        string selectedCard = deck[randomIndex];
        
        // Remove from the deck
        deck.RemoveAt(randomIndex);
        
        return selectedCard;
    }
}
