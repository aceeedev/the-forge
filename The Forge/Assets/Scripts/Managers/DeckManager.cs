using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerDeck
{
    public List<string> cards;

    public PlayerDeck()
    {
        cards = Enumerable.Repeat("", 4).ToList();
    }
}

public class DeckManager : MonoBehaviour
{
    public static DeckManager inst;

    public List<string> poolCards;

    public PlayerDeck player1Deck;
    public PlayerDeck player2Deck;

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
        player1Deck = new PlayerDeck();
        player2Deck = new PlayerDeck();

        poolCards = Enumerable.Repeat("", 8).ToList();

        // dummy data
        poolCards = new List<string> {
            "Card A", "Card B",
            "Card C", "Card D",
            "Card E", "Card F",
            "Card G", "Card H"
        };
    }

    public void SelectPoolCards()
    {
        
    }
}
