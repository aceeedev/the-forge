using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveSelect : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum CardType { Player1, Player2 }

    [SerializeField] private CardType cardType;

    public GameObject opponentMatchingMove;

    void setActiveChildren(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // "remove" this move
        setActiveChildren(false);
        GetComponent<Image>().enabled = false;

        // "remove" the opponents matching move
        opponentMatchingMove.GetComponent<MoveSelect>().setActiveChildren(false);
        opponentMatchingMove.GetComponent<Image>().enabled = false;

        GameManager.inst.NextTurn();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ((GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 && cardType == CardType.Player1) || (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player2 && cardType == CardType.Player2))
        {
            Color c = opponentMatchingMove.GetComponent<Image>().color;
            c.a = 0.5f;

            opponentMatchingMove.GetComponent<Image>().color = c;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Color c = opponentMatchingMove.GetComponent<Image>().color;
        c.a = 1f;

        opponentMatchingMove.GetComponent<Image>().color = c;
    }

    public void Update()
    {
        if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 && cardType == CardType.Player2)
        {
            GetComponent<HoverEffect>().enabled = false;

            return;
        }
        else
        {
            GetComponent<HoverEffect>().enabled = true;
        }

        if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player2 && cardType == CardType.Player1)
        {
            GetComponent<HoverEffect>().enabled = false;

            return;
        }
        else
        {
            GetComponent<HoverEffect>().enabled = true;
        }
    }
}
