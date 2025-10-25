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
}

public class DeckManager : MonoBehaviour
{
    public List<string> situationCards = new List<string> {
        "Assemble IKEA furniture without directions",
        "Teach a classroom Physics",
        "Fix a lightbulb"
    };
    
    public List<Tuple<string, string>> itemCards = new List<Tuple<string, string>>
    {
        new Tuple<string, string>("Sword", "To cut stuff"),
        new Tuple<string, string>("Gun", "To shoot stuff"),
        new Tuple<string, string>("Shield", "To block stuff")
    };

    public List<Tuple<string, string>> talentCards = new List<Tuple<string, string>>
    {
        new Tuple<string, string>("Smart", "To think quickly"),
        new Tuple<string, string>("Strong", "To lift heavy stuff"),
        new Tuple<string, string>("Fast", "To run fast")
    };

    public List<Tuple<string, string>> abilityCards = new List<Tuple<string, string>>
    {
        new Tuple<string, string>("Teleport", "To teleport to a random location"),
        new Tuple<string, string>("Invisibility", "To become invisible"),
        new Tuple<string, string>("Healing", "To heal yourself")
    };

    public List<Tuple<string, string>> clothesCards = new List<Tuple<string, string>>   
    {
        new Tuple<string, string>("Shirt", "To wear"),
        new Tuple<string, string>("Pants", "To wear"),
        new Tuple<string, string>("Hat", "To wear")
    };


    public static DeckManager inst;

    public List<Tuple<string, string>> poolCards;

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
        poolCards = new List<Tuple<string, string>>();
    }

    public void SelectPoolCards()
    {
        poolCards = new List<Tuple<string, string>> {
            drawCharacterCard(itemCards), drawCharacterCard(itemCards),
            drawCharacterCard(talentCards), drawCharacterCard(talentCards),
            drawCharacterCard(abilityCards), drawCharacterCard(abilityCards),
            drawCharacterCard(clothesCards), drawCharacterCard(clothesCards)
        };
    }

    private Tuple<string, string> drawCharacterCard(List<Tuple<string, string>> deck) {
        if (deck.Count() == 0) {
            return null;
        }
        
        // Pick a random key
        int randomIndex = UnityEngine.Random.Range(0, deck.Count());
        
        // Create result dictionary with one card
        Tuple<string, string> selectedCard = deck[randomIndex];
        
        // Remove from the deck
        deck.RemoveAt(randomIndex);
        
        return selectedCard;
    }
}
