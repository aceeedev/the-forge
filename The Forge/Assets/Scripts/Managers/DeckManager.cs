using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using Unity.VisualScripting;

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
        Debug.Log("DeckManager started");
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

    public bool situationPromptLoading = false;

    void Awake()
    {
        // live laugh love singleton pattern

        if (inst == null)
        {
            inst = this;

            StartCoroutine(MyUtils.SendGet<int>("new-conversation", null, (response) => {
            Debug.Log("New conversation initiated: " + response);
        }));
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

        situationPromptLoading = true;
        yield return StartCoroutine(MyUtils.SendGet("prompt-response", $"The situation is: {DeckManager.inst.situation}",
        (int response) => { situationPromptLoading = false; }));
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
