using UnityEngine;
using TMPro;

public class PlayerTurnText : MonoBehaviour
{
    TextMeshProUGUI textComponent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textComponent != null)
        {
            if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1)
            {
                textComponent.text = "Player 1's Turn";
            }
            else
            {
                textComponent.text = "Player 2's Turn";
            }
        }
    }
}
