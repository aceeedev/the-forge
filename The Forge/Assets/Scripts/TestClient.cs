using UnityEngine;
using Colyseus;

public class TestClient : MonoBehaviour
{
    ColyseusClient client;
    ColyseusRoom<MyRoomState> room;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        client = new ColyseusClient("ws://localhost:2567");

        helper();
    }
    
    async void helper()
    {
        room = await client.JoinOrCreate<MyRoomState>("hello");
    }

    // Update is called once per frame
    void Update()
    {
        if (room != null)
        {
           Debug.Log(room.State.counter);
        }
    }
}
