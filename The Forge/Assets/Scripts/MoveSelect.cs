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

    private IEnumerator SendGet<T>(string endpoint, string query = null, Action<T> onComplete = null, bool passAsJson = false)
    {
        string url = baseUri + "/" + endpoint;
        if (!string.IsNullOrEmpty(query))
        {
            url += "?text=" + UnityWebRequest.EscapeURL(query);
        }

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            req.downloadHandler = new DownloadHandlerBuffer();

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("GET error: " + req.error);
            }
            else
            {
                Debug.Log("GET response: " + req.downloadHandler.text);

                if (onComplete != null)
                {
                    if (passAsJson)
                    {
                        T obj = JsonUtility.FromJson<T>(req.downloadHandler.text);
                        onComplete(obj);
                    }
                    else
                    {
                        // Ensure T is byte[] or compatible
                        if (typeof(T) == typeof(byte[]))
                        {
                            onComplete((T)(object)req.downloadHandler.data);
                        }
                        else
                        {
                            Debug.LogError("Cannot pass raw data to type " + typeof(T));
                        }
                    }
                }
            }
        }
    }

    void setActiveChildren(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }

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

        yield return StartCoroutine(SendGet("decide-winner", query, (ResponseWrapper response) =>
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
        setActiveChildren(false);
        GetComponent<Image>().enabled = false;

        // "remove" the opponents matching move
        opponentMatchingMove.GetComponent<MoveSelect>().setActiveChildren(false);
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
