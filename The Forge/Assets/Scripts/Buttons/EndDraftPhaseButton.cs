using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndDraftPhaseButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool skip = false;

    public GameObject DraftButtonObject;
    void Start()
    {
        DraftButtonObject = GameObject.FindGameObjectWithTag("DraftButton");
    }

    // Update is called once per frame
    void Update()
    {
        bool hideOrShow = GameManager.inst.currentPhase == GameManager.CurrentPhase.Draft && GameManager.inst.currentRound >= 3 && GameManager.inst.lastTurn != 1;

        GetComponent<Image>().enabled = hideOrShow;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(hideOrShow);
        }
    }
    
    public void OnPressed()
    {
        AudioManager.inst.PlayClick();

        if (skip == false) {
            skip = true;
            GameManager.inst.lastTurn = 2;
            TextMeshProUGUI textComponent = DraftButtonObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = "Skip Select";
            }
        } else {
            GameManager.inst.NextTurn();
            skip = false;
        }
    }
}
