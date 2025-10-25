using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

    public enum CurrentTurn { Player1, Player2 }
    public CurrentTurn currentTurn;

    public enum CurrentPhase { Draft, Action }
    public CurrentPhase currentPhase = CurrentPhase.Draft;

    public int currentRound = 1;

    public string situation;

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

        // select situation and pool cards
        DeckManager.inst.SelectPoolCards();
        DeckManager.inst.SelectSituationCard();
        DeckManager.inst.player1Deck = new PlayerDeck();
        DeckManager.inst.player2Deck = new PlayerDeck();
    }

    public void NextTurn()
    {
        /// will switch the phase and scene and round++ if True
        bool moveToNextPhase = false;

        // switch current turn
        if (currentTurn == CurrentTurn.Player1)
        {
            currentTurn = CurrentTurn.Player2;
        }
        else
        {
            currentTurn = CurrentTurn.Player1;
        }

        // if draft phase
        if (currentPhase == CurrentPhase.Draft)
        {
            if (DeckManager.inst.lastTurn)
            {
                moveToNextPhase = true;
            
                DeckManager.inst.lastTurn = false;
            }

            // check if all decks are full and ready to go to the next phase
            else if (DeckManager.inst.player1Deck.IsDeckFull() && DeckManager.inst.player2Deck.IsDeckFull())
            {
                moveToNextPhase = true;
            }
        }
        // if action phase
        else
        {

        }

        if (moveToNextPhase)
        {
            if (currentPhase == CurrentPhase.Draft)
            {
                currentPhase = CurrentPhase.Action;
                currentRound += 1;

                SceneManager.LoadScene("ActionScene");
            }
            else
            {
                currentPhase = CurrentPhase.Draft;
                currentRound += 1;

                SceneManager.LoadScene("DraftScene");
            }
        }
    }
}
