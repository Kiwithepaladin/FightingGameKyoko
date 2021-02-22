using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace BeatEmUp
{
    public class BeatEmUpWebSocketServer : MonoBehaviour
    {
        WebSocketServer wssv;
        [SerializeField] private PlayerControllerScript player;
        private float waitTime;

        private void Start()
        {
            player = GameObject.Find("Kyoko").GetComponent<PlayerControllerScript>();
            wssv = new WebSocketServer("ws://localhost:8080");
            Application.OpenURL(@"C:\Users\nadav\Desktop\WebClient\index.html");
            wssv.AddWebSocketService<BEUServer>("/BEUServer");
            wssv.Start();
        }

        private void Update()
        {
        }

        private void OnApplicationQuit()
        {
            wssv.Stop();
        }
    }
    [System.Serializable]
    public class BEUServer : WebSocketBehavior
    {
        public static KyokoStates playerState;
        protected override void OnMessage(MessageEventArgs e)
        {
            //var msg = e.Data == "BALUS"
            // ? "I've been balused already..."
            // : "I'm not available now.";

            //Send(e.Data);
            if (e.Data == "Jump" && PlayerControllerScript.state != KyokoStates.Death)
            {
                PlayerControllerScript.state = KyokoStates.Jumping;
                
            }
            else if(e.Data == "BackElbow" && PlayerControllerScript.state != KyokoStates.Death)
            {
                PlayerControllerScript.state = KyokoStates.BackElbow;
            }
        }
    }
}