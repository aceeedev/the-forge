using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;

public class CardFetch : MonoBehaviour, IPointerDownHandler
{
    public enum CardType { Pool, Player1, Player2 }

    [SerializeField] private CardType cardType;
    [SerializeField] private int cardIndex; // only used if typeOfCard is Attribute

    [SerializeField] private GameObject player1CardGameObject;
    [SerializeField] private GameObject player2CardGameObject;

    private TextMeshProUGUI cardText;
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardText = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
    }

    void Update()
    {
        switch (cardType)
        {
            case CardType.Pool:
                cardText.text = DeckManager.inst.poolCards[cardIndex].Item1;

                break;
            case CardType.Player1:
                disableIfEmpty(CardType.Player1);
                cardText.text = DeckManager.inst.player1Deck.cards[cardIndex];

                break;
            case CardType.Player2:
                disableIfEmpty(CardType.Player2);

                cardText.text = DeckManager.inst.player2Deck.cards[cardIndex];

                break;
        }
    }

    void disableIfEmpty(CardType player)
    {
        if (player == CardType.Player1)
        {
            bool onOrOff = DeckManager.inst.player1Deck.cards[cardIndex] != "";

            setActiveChildren(onOrOff);
            image.enabled = onOrOff;
        }
        else if (player == CardType.Player2)
        {
            bool onOrOff = DeckManager.inst.player2Deck.cards[cardIndex] != "";

            setActiveChildren(onOrOff);
            image.enabled = onOrOff;
        }
    }

    void setActiveChildren(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardType == CardType.Pool)
        {
            // send message to add card to the proper players deck and to remove this card from the pool

            int playerDeckIndex = (int)Math.Floor((double)cardIndex / 2);
            int otherPoolCardIndex = cardIndex % 2 == 0 ? cardIndex + 1 : cardIndex - 1;

            if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1)
            {
                DeckManager.inst.player1Deck.cards[playerDeckIndex] = DeckManager.inst.poolCards[cardIndex].Item1;

                DeckManager.inst.player2Deck.cards[playerDeckIndex] = DeckManager.inst.poolCards[otherPoolCardIndex].Item1;
            }
            else
            {
                DeckManager.inst.player2Deck.cards[playerDeckIndex] = DeckManager.inst.poolCards[cardIndex].Item1;

                DeckManager.inst.player1Deck.cards[playerDeckIndex] = DeckManager.inst.poolCards[otherPoolCardIndex].Item1;
            }

            // remove the cards visually from the pool
            setActiveChildren(false);
            image.enabled = false;

            // Find and disable the other pool card
            CardFetch[] allCards = FindObjectsOfType<CardFetch>();
            foreach (CardFetch card in allCards)
            {
                if (card.cardType == CardType.Pool && card.cardIndex == otherPoolCardIndex)
                {
                    card.setActiveChildren(false);
                    card.image.enabled = false;
                }
            }

            // TODO: maybe switch who goes next?
            GameManager.inst.NextTurn();
        }
    }
}
