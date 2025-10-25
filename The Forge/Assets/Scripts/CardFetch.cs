using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CardFetch : MonoBehaviour, IPointerDownHandler
{
    public enum CardType { Pool, Player1, Player2 }

    [SerializeField] private CardType cardType;
    [SerializeField] private int cardIndex; // only used if typeOfCard is Attribute

    private TextMeshProUGUI cardText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        switch (cardType)
        {
            case CardType.Pool:
                cardText.text = NetworkManager.inst.poolCards[cardIndex];

                break;
            case CardType.Player1:
                disableIfEmpty(CardType.Player1);

                cardText.text = NetworkManager.inst.player1Cards[cardIndex];

                break;
            case CardType.Player2:
                disableIfEmpty(CardType.Player2);

                cardText.text = NetworkManager.inst.player2Cards[cardIndex];

                break;
        }
    }

    void disableIfEmpty(CardType player)
    {
        if (player == CardType.Player1)
        {
            gameObject.SetActive(NetworkManager.inst.player1Cards[cardIndex] != "");
        }
        else if (player == CardType.Player2)
        {
            gameObject.SetActive(NetworkManager.inst.player2Cards[cardIndex] != "");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardType == CardType.Pool)
        {
            // send message to add card to the proper players deck and to remove this card from the pool
            Debug.Log("card pressed!");
        }
    }
}
