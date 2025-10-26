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
    }


    private IEnumerator ShowWinnerThenNextTurn(string winnerText, string winnerExplanation, Action callback)
    {
        Debug.Log(GameObject.FindGameObjectWithTag("WinnerText"));
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TextMeshProUGUI>().text = winnerText;
        DialogueObject.GetComponent<TextMeshProUGUI>().text = winnerExplanation;

        yield return new WaitForSeconds(5f);

        if (callback != null)
        {
            callback();
        }
    }

    public IEnumerator CheckWinner(Action callback)
    {
        string query = " . Then, decisvely determine if player_1 or player_2 won this situation based on actions selected and how it progress their story. In 20 words or less, declare the winner and give a final summary that wraps up each player's story.";

        yield return StartCoroutine(MyUtils.SendGet("decide-winner", query, (ResponseWrapper response) =>
        {
            if (response == null)
            {
                Debug.LogError("Failed to parse moves");
                return;
            }

            WinnerWrapper winnerWrapper = JsonUtility.FromJson<WinnerWrapper>(response.response);

            string winner = winnerWrapper.winner;
            string winnerExplanation = winnerWrapper.explanation;

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

        AudioManager.PlayClick();

        string move = GetComponentInChildren<TextMeshProUGUI>().text;

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
            StartCoroutine(MyUtils.SendGet("prompt-response", $"The player has chosen to do the following move: {move}. In a short sentence no more than 12 words explain its actual effectiveness having just been performed. Tell the story of how it progressed the scene and always include build-off of previous actions taken by them. Determine the action's effectiveness, complete successes are possible, but failure or partial failures are also allowed and often realistic.",
            (ResponseWrapper response) =>
            {
                if (response == null)
                {
                    Debug.LogError("Failed to parse dialogue");
                    return;
                }

                string actionDialogue = response.response;

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
                }
                else
                {
                    Debug.LogError("DialogueObject is null!");
                }
            }, true));
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
