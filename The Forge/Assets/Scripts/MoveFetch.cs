using UnityEngine;
using TMPro;

public class MoveFetch : MonoBehaviour
{
    [SerializeField] private int moveIndex; // only used if typeOfCard is Attribute

    private TextMeshProUGUI cardText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        cardText.text = NetworkManager.inst.moves[moveIndex];
    }
}
