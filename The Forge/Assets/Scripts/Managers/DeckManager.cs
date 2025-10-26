using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public List<string> situationCards = new List<string> {
        "Assemble IKEA furniture without directions",
        "Teach a classroom Physics",
        "Fix a lightbulb"
    };
    
    public List<string> itemCards = new List<string>
    {
        "Sword", "Gun", "Shield"
    };

    public List<string> talentCards = new List<string>
    {
        "Smart", "Strong", "Fast"
    };

    public List<string> abilityCards = new List<string>
    {
        "Teleportation", "Invisibility", "Healing"
    };

    public List<string> clothesCards = new List<string>   
    {
        "Shirt", "Pants", "Hat"
    };


    public static DeckManager inst;

    public string situation;

    public List<string> poolCards;

    public PlayerDeck player1Deck;
    public PlayerDeck player2Deck;

    /// <summary>
    ///  for checking if its the last turn on rounds of drafts >=2
    /// </summary>
    public bool lastTurn = false;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        poolCards = new List<string>();
    }

    public void SelectSituationCard()
    {
        if (situationCards.Count() == 0) {
            return;
        }
        int randomIndex = UnityEngine.Random.Range(0, situationCards.Count());
        situation = situationCards[randomIndex];
        situationCards.RemoveAt(randomIndex);
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
