using System;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class GameController : MonoBehaviour
{
    public SocketIOUnity socket;

    public MapCreatorScript MapCreatorScript;

    // Start is called before the first frame update
    void Start()
    {
        MapCreatorScript = GameObject.Find("MapCreator").GetComponent<MapCreatorScript>();
        var uri = new Uri("http://localhost:8080");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", Environment.GetEnvironmentVariable("UNITY_ACCESS_TOKEN") }  // TODO find a way to not expose this secret
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("disconnect: " + e);
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };
        

        Debug.Log("Connecting...");
        socket.Connect();

        socket.OnUnityThread("map_update", (response) =>
        {
            Debug.Log("Running in here");
            Debug.Log(response.GetValue());
            MapCreatorScript.CreateMap(response.GetValue().GetRawText());  // pass the map details to the script
        });

        socket.OnUnityThread("trigger_event", (response) =>
        {
            Debug.Log("Running in here");
            int[] coords = response.GetValue<int[]>();
            MapCreatorScript.TriggerEvent(coords);
        });
        
    }
    
    public static bool IsJSON(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) { return false; }
        str = str.Trim();
        if ((str.StartsWith("{") && str.EndsWith("}")) || //For object
            (str.StartsWith("[") && str.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(str);
                return true;
            }catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void EmitSpin()
    {
        socket.Emit("spin");
    }
}