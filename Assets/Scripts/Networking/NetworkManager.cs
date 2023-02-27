using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;

public class NetworkManager : MonoBehaviour
{

    #region  Variables
    TcpClient client;
    public string hostname = "127.0.0.1";
    public int port = 1404;
    NetworkStream networkStream;
    public BinaryWriter writer;
    public BinaryReader reader;
    public bool onlineMode;
        
    public List<PlayerState> queue = new List<PlayerState>();
    public List<PlayerState> playersOn = new List<PlayerState>();
    public List<GameObject> players = new List<GameObject>();


    public GameObject playerPrefab;
    public string username = "Blaster :D";
    #endregion

    void Start()
    {
        onlineMode = false;
    }

    public void FixedUpdate(){
        acceptPlayers();
        updatePositions();
    }

    public bool isOnline(string username){
        for (int i = 0; i < playersOn.Count; i++)
        {
            if(playersOn[i].username == username){
                return true;
            }
        }
        return false;
    }

    public void setPlayerPos(string username, Vector3 pos, Vector2 anim){
        for (int i = 0; i < playersOn.Count; i++)
        {
            if(playersOn[i].username == username){
                playersOn[i].setPos(pos);
                playersOn[i].animDirection = anim;
            }
        }
    }

    public void updatePositions(){
        if(onlineMode){
            for (int i = 0; i < playersOn.Count; i++)
            {
                players[i].transform.position = playersOn[i].getPos();
            }
        }
    }

    public void acceptPlayers(){
        try{
            if(queue.Count>0){
                PlayerState playerNew = queue[0];
                Vector3 pos = new Vector3(float.Parse(playerNew.posX),float.Parse(playerNew.posY),0);
                GameObject newPlayer = Instantiate(playerPrefab, pos, Quaternion.identity, null);
                newPlayer.name = playerNew.username;
                players.Add(newPlayer);
                playersOn.Add(playerNew);
                queue.RemoveAt(0);
            }
        }catch{}
    }

    public void checkConn(string where){
        Debug.Log("Some sort of connection issue popped up in the "+where);
    }

    public void retryConnection(){
        try{
            string newHost = GameObject.Find("serverAddress").transform.GetComponent<TMP_InputField>().text;
            hostname = newHost;
            string newUsername = GameObject.Find("playerUsername").transform.GetComponent<TMP_InputField>().text;
            username = newUsername;
            client = new TcpClient(newHost, port);
            networkStream = client.GetStream(); 
            writer = new BinaryWriter(networkStream);
            reader = new BinaryReader(networkStream); 
            onlineMode = true;
            Debug.Log("Connected to the server.");
        }
        catch(Exception e){
            onlineMode = false;
            Debug.Log(e.ToString());
        } 
    }
}
