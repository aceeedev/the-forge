using UnityEngine;
using TMPro;

public class CardFetch : MonoBehaviour
{
    public enum CardType
    {
        Attribute, Situation
    }

    [SerializeField] private CardType typeOfCard;
    [SerializeField] private int cardIndex; // only used if typeOfCard is Attribute

    private TextMeshProUGUI cardText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (typeOfCard == CardType.Attribute)
        {
            cardText.text = NetworkManager.inst.attributeCards[cardIndex];
        }
        else if (typeOfCard == CardType.Situation)
        {
            cardText.text = NetworkManager.inst.situationCard;
        }
    }
}
