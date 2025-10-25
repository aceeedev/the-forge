using UnityEngine;
using Colyseus;

public class TestClient : MonoBehaviour
{
    ColyseusClient client;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        client = new ColyseusClient("ws://localhost:2567");

        // NOTE: create C# schema (MyRoomState) from ts schema on the server using: npx @colyseus/schema-codegen
        ColyseusRoom<MyRoomState> room = await client.Create<MyRoomState>(roomName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
