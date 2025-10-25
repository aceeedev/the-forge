using UnityEngine;
using TMPro;

public class SituationFetch : MonoBehaviour
{
    private TextMeshProUGUI cardText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        cardText.text = GameManager.inst.situation;
    }
}
