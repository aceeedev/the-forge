using UnityEngine;
using UnityEngine.UI;

public class EndDraftPhaseButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool hideOrShow = GameManager.inst.currentPhase == GameManager.CurrentPhase.Draft && GameManager.inst.currentRound >= 2;

        GetComponent<Image>().enabled = hideOrShow;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(hideOrShow);
        }
    }
    
    public void OnPressed()
    {
        DeckManager.inst.lastTurn = true;
    }
}
