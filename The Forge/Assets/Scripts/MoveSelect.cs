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
    public GameObject DialogueObject;

    public void Start()
    {
        DialogueObject = GameObject.FindGameObjectWithTag("Dialogue");
        SetDialogueDisplay(false);
    }

    private void SetDialogueDisplay(bool display)
    {
        DialogueObject.transform.parent.GetComponent<Image>().enabled = display;
        DialogueObject.GetComponent<TextMeshProUGUI>().enabled = display;
    }

    private IEnumerator ShowWinnerThenNextTurn(string winnerText, string winnerExplanation, Action callback)
    {
        yield return new WaitForSeconds(5f);
        SetDialogueDisplay(false);
        yield return new WaitForSeconds(0.5f);
        SetDialogueDisplay(true);

        Debug.Log(GameObject.FindGameObjectWithTag("WinnerText"));
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().text = winnerText;
        DialogueObject.GetComponent<TextMeshProUGUI>().text = winnerExplanation;

        yield return new WaitForSeconds(7f);

        if (callback != null)
        {
            callback();
        }
    }

    public IEnumerator CheckWinner(Action callback)
    {
        string query = "Decisvely determine if Player 1 or Player 2 won this situation based on actions selected and how it progress their story. In 15 words or less, declare the winner and give a final action that wraps up each player's story, be concise and to the point.";

        yield return StartCoroutine(MyUtils.SendGet("decide-winner", query, (ResponseWrapper response) =>
        {
            if (response == null)
            {
                Debug.LogError("Failed to parse moves");
                return;
            }

            WinnerWrapper winnerWrapper = JsonUtility.FromJson<WinnerWrapper>(response.response);

            string winner = winnerWrapper.winner;
            string winnerExplanation = winnerWrapper.explanation.Replace("_", " ");

            Debug.Log("Winning Explanation: " + winnerExplanation);

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
                StartCoroutine(ShowWinnerThenNextTurn(winner, winnerExplanation, callback));
            }

        }, true));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if ((GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 && cardType == CardType.Player2) || (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player2 && cardType == CardType.Player1))
        {
            return;
        }

        AudioManager.inst.PlayClick();

        string move = GetComponentInChildren<TextMeshProUGUI>().text;

        // "remove" this move
        MyUtils.SetActiveChildren(gameObject, false);
        GetComponent<Image>().enabled = false;

        // "remove" the opponents matching move
        MyUtils.SetActiveChildren(opponentMatchingMove.GetComponent<MoveSelect>().gameObject, false);
        opponentMatchingMove.GetComponent<Image>().enabled = false;

        SetDialogueDisplay(false);
        string playerName = GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 ? "Player 1" : "Player 2";
        StartCoroutine(MyUtils.SendGet("prompt-response", $"{playerName} has chosen to do the following move: {move}. Use proper english but being concise, in a sentence no more than 12 words always explain how the action progressed the scene and relate it to previous actions taken by them, determining the action's direct result without hypotheticals which can range from complete successes to failure or partial failures as realistic options.",
        (ResponseWrapper response) =>
        {
            if (response == null)
            {
                Debug.LogError("Failed to parse dialogue");
                return;
            }

            string actionDialogue = response.response.Replace("_", " ");

            Debug.Log("Action Dialogue received: " + actionDialogue);

            // Update the DialogueObject text immediately when response arrives
            if (DialogueObject != null)
            {
                Debug.Log("Updating DialogueObject text to: " + actionDialogue);
                TextMeshProUGUI textComponent = DialogueObject.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = actionDialogue;
                    Debug.Log("Text updated successfully");
                }
                else
                {
                    Debug.LogError("DialogueObject does not have a TextMeshProUGUI component!");
                }
                SetDialogueDisplay(true);
            }
            else
            {
                Debug.LogError("DialogueObject is null!");
            }

             if ((GameManager.inst.currentActionPhase[0] == GameManager.ActionPhase.Middle && GameManager.inst.currentActionPhase[1] == GameManager.ActionPhase.End) ||
            (GameManager.inst.currentActionPhase[0] == GameManager.ActionPhase.End && GameManager.inst.currentActionPhase[1] == GameManager.ActionPhase.Middle))
            {
                // if the game is about to end
                StartCoroutine(CheckWinner(() => GameManager.inst.NextTurn()));
            }
            else
            {
                GameManager.inst.NextTurn();
            }
        }, true));
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
        if (GameManager.inst == null)
        {
            return;
        }

        HoverEffect hoverEffect = GetComponent<HoverEffect>();
        if (hoverEffect == null)
        {
            return;
        }

        if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 && cardType == CardType.Player2)
        {
            hoverEffect.enabled = false;
            return;
        }

        if (GameManager.inst.currentTurn == GameManager.CurrentTurn.Player2 && cardType == CardType.Player1)
        {
            hoverEffect.enabled = false;
            return;
        }

        hoverEffect.enabled = true;
    }
}
