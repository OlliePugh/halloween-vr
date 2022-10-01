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
                    {"token", "UNITY" }
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
        socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
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
        // socket.OnUnityThread((name, response) => {
        //     Debug.Log("Received On " + name + " : " + response.GetValue().GetRawText() + "\n");
        // });

        // "spin" is an example of an event name.
        socket.OnUnityThread("map_update", (response) =>
        {
            Debug.Log("Running in here");
            Debug.Log(response.GetValue());
            MapCreatorScript.CreateMap(response.GetValue().GetRawText());  // pass the map details to the script
        });
        
    }

    // public void EmitTest()
    // {
    //     string eventName = EventNameTxt.text.Trim().Length < 1 ? "hello" : EventNameTxt.text;
    //     string txt = DataTxt.text;
    //     if (!IsJSON(txt))
    //     {
    //         socket.Emit(eventName, txt);
    //     }
    //     else
    //     {
    //         socket.EmitStringAsJSON(eventName, txt);
    //     }
    // }

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