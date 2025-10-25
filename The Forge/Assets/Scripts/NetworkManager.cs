using UnityEngine;
using Colyseus;
using System.Collections.Generic;
using System.Linq;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager inst;

    ColyseusClient colyseusClient;
    ColyseusRoom<MyRoomState> room;

    public List<string> attributeCards;
    public string situationCard;

    public List<string> moves;

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
        attributeCards = Enumerable.Repeat("", 8).ToList();
        situationCard = "";

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
            attributeCards = new List<string>
            {
                "Attr1", "Attr2", "Attr3", "Attr4",
                "Attr5", "Attr6", "Attr7", "Attr8"
            };
            situationCard = "Situation 1";

            moves = new List<string>
            {
                "Move1", "Move2",
                "Move3", "Move4"
            };
        }
    }
}
