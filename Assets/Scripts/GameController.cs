using System;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public class GameController : MonoBehaviour
{
    public const int MIN_BPM = 50;
    public const int MAX_BPM = 200;

    public SocketIOUnity socket;
    public MapCreatorScript mapCreatorScript;
    public NonBlockEventManager nonBlockEventManager;
    [Range(MIN_BPM, MAX_BPM)]
    public int bpm;
    public bool fetchBpm = true;
    public int defaultBpm = 100;
    public int bpmPollingRate = 1;  // how many seconds between updates

    // Start is called before the first frame update
    void Start()
    {
        mapCreatorScript = GameObject.Find("MapCreator").GetComponent<MapCreatorScript>();
        nonBlockEventManager = GameObject.Find("NonBlockEventManager").GetComponent<NonBlockEventManager>();
        
        bpm = defaultBpm;

        Uri uri = new Uri("http://localhost:8080");
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

        SocketSetup(ref socket);

        StartCoroutine(BpmCoroutine(uri));

    }
    
    public void SocketSetup(ref SocketIOUnity socket)
    {
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
            mapCreatorScript.CreateMap(response.GetValue().GetRawText());  // pass the map details to the script
            this.GameReady();  // map has been created and the game is ready to play
        });

        socket.OnUnityThread("trigger_event", (response) =>
        {
            int[] coords = response.GetValue<int[]>();
            mapCreatorScript.TriggerEvent(coords);
        });

        socket.OnUnityThread("non_block_event", (response) =>
        {
            Debug.Log("Non block trigger");
            EventData eventData = response.GetValue<EventData>();
            nonBlockEventManager.TriggerEvent(eventData);
        });

        socket.OnUnityThread("end_game", (response) =>
        {
            EndGame();
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

    IEnumerator BpmCoroutine(Uri uri)
    {
        while (true)
        {
            if (fetchBpm)
            {
                StartCoroutine(GetRequest(new Uri(uri, "/bpm").ToString(), (responseText) =>
                {
                    if (responseText != null) // if there is response text
                    {
                        int newBpm;
                        if (int.TryParse(responseText, out newBpm))
                        {
                            bpm = Mathf.Clamp(newBpm, MIN_BPM, MAX_BPM);
                        }
                        else
                        {
                            bpm = defaultBpm; // default the bpm to 100
                        }
                    }
                }));
            }
            yield return new WaitForSeconds(bpmPollingRate);
        }
    }

    IEnumerator GetRequest(string uri, System.Action<string> callback)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            callback(uwr.downloadHandler.text);  // call the callback with the data
        }
    }

    public void GameReady()
    {
        socket.Emit("game_ready");  // emit this when the game is ready for the user to start sending events
    }

    public void KillPlayer()
    {
        Debug.Log("PLAYER KILLED BY MONSTER");
        socket.Emit("end_game", "player_killed");
        EndGame();
    }

    public void CollectKey()
    {
        Debug.Log("KEY HAS BEEN COLLECTED PLAYER WINS");
        socket.Emit("end_game", "key_collected");
        EndGame();
    }

    public void EndGame()
    {
        mapCreatorScript.ClearMap();
        nonBlockEventManager.CleanAll();
    }

    public void EmitObjectLocation(string name, Vector3 location)
    {
        if (socket != null)
        {
            socket.EmitStringAsJSON($"entity_location", $"{{\"key\":\"{name}\",\"location\":{{\"row\": {location.x},\"col\":{location.z}}}}}");
        }
    }
}