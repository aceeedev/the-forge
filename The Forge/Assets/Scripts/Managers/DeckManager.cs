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

    public bool IsDeckFull()
    {
        return !cards.Contains("");
    }
}

public class DeckManager : MonoBehaviour
{
    public static DeckManager inst;

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
        poolCards = Enumerable.Repeat("", 8).ToList();
    }

    public void SelectPoolCards()
    {
        // make sure the cards are visible

        // dummy data
        poolCards = new List<string> {
            "Card A", "Card B",
            "Card C", "Card D",
            "Card E", "Card F",
            "Card G", "Card H"
        };
    }
}
