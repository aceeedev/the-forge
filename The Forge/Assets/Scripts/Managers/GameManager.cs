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
        SceneManager.LoadScene("DraftScene");
    }
    
    public void RestartGame()
    {
        
    }

    public void NextTurn()
    {
        if (currentTurn == CurrentTurn.Player1)
        {
            currentTurn = CurrentTurn.Player2;
        }
        else
        {
            currentTurn = CurrentTurn.Player1;
        }
    }
}
