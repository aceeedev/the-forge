using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;



[System.Serializable]
public class WinnerWrapper
{
    public string winner;
    public string explanation;
}

public class MoveSelect : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum CardType { Player1, Player2 }

    [SerializeField] private CardType cardType;

    public GameObject opponentMatchingMove;

    private string baseUri = "http://localhost:3000";

    private IEnumerator ShowWinnerThenNextTurn(string winnerText, Action callback)
    {
        Debug.Log(GameObject.FindGameObjectWithTag("WinnerText"));
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().text = winnerText;

        yield return new WaitForSeconds(1f);

        if (callback != null)
        {
            callback();
        }
    }

    public IEnumerator CheckWinner(Action callback)
    {
        string query = "Decide if player_1 or player_2 won this situation based off of the actions and moves selected! Make sure to provide a detailed explanation that says exactly why the player won and how they won from the actions selected.";

        yield return StartCoroutine(MyUtils.SendGet("decide-winner", query, (ResponseWrapper response) =>
        {
            if (response == null)
            {
                Debug.LogError("Failed to parse moves");
                return;
            }

            WinnerWrapper winnerWrapper = JsonUtility.FromJson<WinnerWrapper>(response.response);

            string winner = winnerWrapper.winner;

            Debug.Log("Winning Explanation: " + winnerWrapper.explanation);

            if (callback != null)
            {
                if (winner == "player_1")
                {
                    winner = "Player 1 won!";
                }
                else
                {
                    winner = "Player 2 won!";
                }

                StartCoroutine(ShowWinnerThenNextTurn(winner, callback));
            }

        }, true));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if ((GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 && cardType == CardType.Player2) || (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player2 && cardType == CardType.Player1))
        {
            return;
        }

        // "remove" this move
        MyUtils.SetActiveChildren(gameObject, false);
        GetComponent<Image>().enabled = false;

        // "remove" the opponents matching move
        MyUtils.SetActiveChildren(opponentMatchingMove.GetComponent<MoveSelect>().gameObject, false);
        opponentMatchingMove.GetComponent<Image>().enabled = false;

        // if the game is about to end
        if ((GameManager.inst.currentActionPhase[0] == GameManager.ActionPhase.Middle && GameManager.inst.currentActionPhase[1] == GameManager.ActionPhase.End) ||
        (GameManager.inst.currentActionPhase[0] == GameManager.ActionPhase.End && GameManager.inst.currentActionPhase[1] == GameManager.ActionPhase.Middle))
        {
            // see who won
            StartCoroutine(CheckWinner(() => GameManager.inst.NextTurn()));
        }
        else
        {
            GameManager.inst.NextTurn();
        }
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
