using UnityEngine;
using Colyseus;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager inst;

    ColyseusClient colyseusClient;
    ColyseusRoom<MyRoomState> room;

    public string situation;
    public List<string> poolCards;
    public List<string> player1Cards;
    public List<string> player2Cards;

    public List<string> moves;

    public Texture2D character1Texture;
    public Texture2D character2Texture;

    void Awake()
    {
        // Basic singleton pattern. Make sure there is only ever 1 GameManager in the scene and updates inst accordingly.
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
        colyseusClient = new ColyseusClient("ws://localhost:2567");

        // initialize values
        situation = "";
        poolCards = Enumerable.Repeat("", 8).ToList();

        player1Cards = Enumerable.Repeat("", 6).ToList();
        player2Cards = Enumerable.Repeat("", 6).ToList();

        moves = Enumerable.Repeat("", 4).ToList();

        // try to connect to the room
        initialRoomConnection();
    }
    
    async void initialRoomConnection()
    {
        try
        {
            room = await colyseusClient.JoinOrCreate<MyRoomState>("hello");
        }
        catch (System.Exception e)
        {
            // connection failed 
            Debug.LogWarning("Not able to connect to colyseus room: " + e.Message);

            // TODO: remove. just dummy data
            situation = "Situation 1";

            poolCards = new List<string>
            {
                "Head1", "Head2",
                "Body1", "Body2",
                "Limb1", "Limb2",
                "Ability1", "Ability2"
            };

            moves = new List<string>
            {
                "Move1", "Move2",
                "Move3", "Move4"
            };

            character1Texture = Texture2D.grayTexture;
            character2Texture = Texture2D.grayTexture;
        }
    }
}
