using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using System.Collections;

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
                cardText.text = DeckManager.inst.poolCards[cardIndex];

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

            MyUtils.SetActiveChildren(gameObject, onOrOff);
            image.enabled = onOrOff;
        }
        else if (player == CardType.Player2)
        {
            bool onOrOff = DeckManager.inst.player2Deck.cards[cardIndex] != "";

            MyUtils.SetActiveChildren(gameObject, onOrOff);
            image.enabled = onOrOff;
        }
    }

    private System.Collections.IEnumerator TweenCardToTarget(GameObject targetObject)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform targetRectTransform = targetObject.GetComponent<RectTransform>();
        
        Vector3 startPosition = rectTransform.position;
        Vector3 targetWorldPosition = targetRectTransform.position;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Use ease-in-out for smooth motion
            float easedT = t * t * (3f - 2f * t);
            rectTransform.position = Vector3.Lerp(startPosition, targetWorldPosition, easedT);
            yield return null;
        }
        
        // Ensure final position is exact
        rectTransform.position = targetWorldPosition;
        
        // After animation, disable the card
        MyUtils.SetActiveChildren(gameObject, false);
        image.enabled = false;
    }

    private System.Collections.IEnumerator HandleCardSelection(int cardIndex, int playerDeckIndex, int otherPoolCardIndex, 
        CardFetch targetPlayer1Card, CardFetch targetPlayer2Card, CardFetch otherPoolCard)
    {
        // Start both tween animations
        Coroutine tween1 = null;
        Coroutine tween2 = null;

        if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 && targetPlayer1Card != null)
        {
            tween1 = StartCoroutine(TweenCardToTarget(targetPlayer1Card.gameObject));
        }
        else if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player2 && targetPlayer2Card != null)
        {
            tween1 = StartCoroutine(TweenCardToTarget(targetPlayer2Card.gameObject));
        }

        if (otherPoolCard != null)
        {
            if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 && targetPlayer2Card != null)
            {
                tween2 = otherPoolCard.StartCoroutine(otherPoolCard.TweenCardToTarget(targetPlayer2Card.gameObject));
            }
            else if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player2 && targetPlayer1Card != null)
            {
                tween2 = otherPoolCard.StartCoroutine(otherPoolCard.TweenCardToTarget(targetPlayer1Card.gameObject));
            }
        }

        // Wait for animations to complete (0.5 seconds default duration)
        GameManager.inst.cardsMoving = true;
        yield return new WaitForSeconds(0.3f);

        // After animations complete, update the deck data
        if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1)
        {
            DeckManager.inst.player1Deck.cards[playerDeckIndex] = DeckManager.inst.poolCards[cardIndex];
            DeckManager.inst.player2Deck.cards[playerDeckIndex] = DeckManager.inst.poolCards[otherPoolCardIndex];
        }
        else
        {
            DeckManager.inst.player2Deck.cards[playerDeckIndex] = DeckManager.inst.poolCards[cardIndex];
            DeckManager.inst.player1Deck.cards[playerDeckIndex] = DeckManager.inst.poolCards[otherPoolCardIndex];
        }
        DeckManager.inst.poolCards[cardIndex] = "";
        DeckManager.inst.poolCards[otherPoolCardIndex] = "";

        yield return new WaitForSeconds(0.05f);
        GameManager.inst.cardsMoving = false;
        GameManager.inst.NextTurn();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardType == CardType.Pool)
        {
            int playerDeckIndex = (int)Math.Floor((double)cardIndex / 2);
            int otherPoolCardIndex = cardIndex % 2 == 0 ? cardIndex + 1 : cardIndex - 1;

            // Find the specific target cards in the player decks at the calculated index
            CardFetch[] allCards = FindObjectsByType<CardFetch>(FindObjectsSortMode.None);
            
            CardFetch targetPlayer1Card = null;
            CardFetch targetPlayer2Card = null;
            CardFetch otherPoolCard = null;

            foreach (CardFetch card in allCards)
            {
                if (card.cardType == CardType.Player1 && card.cardIndex == playerDeckIndex)
                {
                    targetPlayer1Card = card;
                }
                else if (card.cardType == CardType.Player2 && card.cardIndex == playerDeckIndex)
                {
                    targetPlayer2Card = card;
                }
                else if (card.cardType == CardType.Pool && card.cardIndex == otherPoolCardIndex)
                {
                    otherPoolCard = card;
                }
            }

            // Start the coordinated animation and deck update sequence
            StartCoroutine(HandleCardSelection(cardIndex, playerDeckIndex, otherPoolCardIndex, 
                targetPlayer1Card, targetPlayer2Card, otherPoolCard));
        }
    }
}
