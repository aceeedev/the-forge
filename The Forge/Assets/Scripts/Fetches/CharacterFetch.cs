using UnityEngine;
using UnityEngine.UI;

public class CharacterFetch : MonoBehaviour
{
    public enum CardType { Player1, Player2 }

    [SerializeField] private CardType cardType;
    private RawImage rawImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rawImage = GetComponentInChildren<RawImage>();
    }

    void Update()
    {
        switch (cardType)
        {
            case CardType.Player1:
                rawImage.texture = NetworkManager.inst.character1Texture;

                break;
            case CardType.Player2:
                rawImage.texture = NetworkManager.inst.character2Texture;

                break;
        }
    }
}
