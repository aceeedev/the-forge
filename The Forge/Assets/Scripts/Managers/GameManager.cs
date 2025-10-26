using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public class AwardsResponseWrapper
{
    public string award_1_winner;
    public string award_2_winner;
}

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

    public enum CurrentTurn { Player1, Player2 }
    public CurrentTurn currentTurn;

    public enum CurrentPhase { Draft, Action, Final }
    public CurrentPhase currentPhase = CurrentPhase.Draft;

    public int currentRound = 1;

    public enum ActionPhase { Beginning, Middle, End }
    public ActionPhase[] currentActionPhase;

    public int numActionPhases = 1;

    public int maxNumActionPhases = 2;

    public bool runMovesFlag = false;

    public bool cardsMoving = false;

    public int lastTurn = -1;

    public string award1Winner = "";
    public string award2Winner = "";

    void Awake()
    {
        // live laugh love singleton pattern

        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        RestartGame();
        SceneManager.LoadScene("DraftScene");
    }
    
    public void RestartGame()
    {
        currentTurn = CurrentTurn.Player1;
        currentPhase = CurrentPhase.Draft;
        currentRound = 1;

        currentActionPhase = new ActionPhase[] { ActionPhase.Beginning, ActionPhase.Beginning };

        // select situation and pool cards
        DeckManager.inst.SelectPoolCards();
        StartCoroutine(DeckManager.inst.SelectSituationCard());
        DeckManager.inst.player1Deck = new PlayerDeck();
        DeckManager.inst.player2Deck = new PlayerDeck();
    }

    public void NextTurn(bool actionRepeatFlag = false)
    {
        /// will switch the phase and scene and round++ if True
        bool moveToNextPhase = false;

        // if draft phase
        if (currentPhase == CurrentPhase.Draft)
        {
            if (lastTurn == 1)
            {
                moveToNextPhase = true;

                lastTurn = -1;
            }
            else if (lastTurn > 1)
            {
                lastTurn -= 1;
            }

            // check if pool is empty 
            // TO DO: "end turn" is is clicked
            else if (DeckManager.inst.poolCards.All(card => card == ""))
            {
                moveToNextPhase = true;
            }
        }
        // if action phase
        else
        {
            if (currentTurn == CurrentTurn.Player1)
            {
                switch (currentActionPhase[0])
                {
                    case ActionPhase.Beginning:

                        currentActionPhase[0] = ActionPhase.Middle;

                        break;
                    case ActionPhase.Middle:

                        currentActionPhase[0] = ActionPhase.End;

                        break;
                }
            }
            else
            {
                switch (currentActionPhase[1])
                {
                    case ActionPhase.Beginning:

                        currentActionPhase[1] = ActionPhase.Middle;

                        break;
                    case ActionPhase.Middle:

                        currentActionPhase[1] = ActionPhase.End;

                        break;
                }
            }

            if (currentActionPhase[0] == ActionPhase.End && currentActionPhase[1] == ActionPhase.End)
            {
                moveToNextPhase = true;
            }
        }

        // switch current turn


        if (moveToNextPhase)
        {
            if (currentRound == 6 && currentPhase == CurrentPhase.Action)
            {
                currentPhase = CurrentPhase.Final;
                StartCoroutine(MyUtils.SendGet("final-winner", "Based on the following situations and stories chosen by the players, decide the winner of the game.",
                (ResponseWrapper response) =>
                {
                    Debug.Log("API Response received!");
                    
                    AwardsResponseWrapper awardsResponse = JsonUtility.FromJson<AwardsResponseWrapper>(response.response);
                    award1Winner = awardsResponse.award_1_winner.Replace("_", " ");
                    award2Winner = awardsResponse.award_2_winner.Replace("_", " ");
                    Debug.Log("Awards set: award1Winner=" + award1Winner + ", award2Winner=" + award2Winner);
                    // Load the scene after the data is received
                    SceneManager.LoadScene("FinalScene");
                }, true));
            }
            else if (currentPhase == CurrentPhase.Draft)
            {
                currentPhase = CurrentPhase.Action;
                currentActionPhase[0] = ActionPhase.Beginning;
                currentActionPhase[1] = ActionPhase.Beginning;
                currentRound += 1;

                SceneManager.LoadScene("ActionScene");
            }
            else
            {
                currentPhase = CurrentPhase.Draft;
                DeckManager.inst.SelectPoolCards();
                StartCoroutine(DeckManager.inst.SelectSituationCard());
                currentRound += 1;

                numActionPhases = 1;

                SceneManager.LoadScene("DraftScene");
            }
            currentTurn = currentRound % 2 == 0 ? CurrentTurn.Player1 : CurrentTurn.Player2;
        }
        else {
            if (currentTurn == CurrentTurn.Player1)
            {
                currentTurn = CurrentTurn.Player2;
            }
            else
            {
                currentTurn = CurrentTurn.Player1;
            }

            if (actionRepeatFlag)
            {
                currentActionPhase[0] = ActionPhase.Beginning;
                currentActionPhase[1] = ActionPhase.Beginning;

                runMovesFlag = true;
            }
        }
    }
}
