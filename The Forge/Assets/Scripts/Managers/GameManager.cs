using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

    public enum CurrentTurn { Player1, Player2 }
    [SerializeField] public CurrentTurn currentTurn;

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
        currentTurn = CurrentTurn.Player1;
    }

    // Update is called once per frame
    void Update()
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
