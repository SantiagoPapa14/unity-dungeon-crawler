using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerState{
public Vector3 position;
public PlayerState(Vector3 position){
    this.position = position;
}

public void setPos(Vector2 position){

    this.position.x = float.Parse(position.x.ToString("F1"));
    this.position.y = float.Parse(position.y.ToString("F1"));
}
public string jsonString(){
    string data = JsonUtility.ToJson(this);
    return data;
}

}
public class NetworkManager : MonoBehaviour
{

    #region Variables
    TcpClient client;
    public string hostname = "127.0.0.1";
    public int port = 1404;
    NetworkStream networkStream;
    BinaryWriter writer;
    byte[] myWriteBuffer;
    bool onlineMode;

    PlayerState pState;
    Transform player;
    
    #endregion

    void Start()
    {
        player = GameObject.Find("Player").transform;
        pState = new PlayerState(new Vector3(0,0,0));
        try{
            client = new TcpClient(hostname, port);
            networkStream = client.GetStream(); 
            writer = new BinaryWriter(networkStream); 
            onlineMode = true;
            Debug.Log("Connected to the server.");
        }
        catch{
            onlineMode = false;
            Debug.Log("Could not establish a connection to the server.");
        }  
    }
    void Update()
    {
        
        if(onlineMode){
            #region sendData
            try{
                if(player == null){
                    player = GameObject.Find("Player").transform;
                }else{
                    if(Mathf.Abs(player.position.x - pState.position.x) > 0.1f || Mathf.Abs(player.position.y - pState.position.y) > 0.1f){
                        pState.setPos(player.position);
                        writer.Write(pState.jsonString());
                    }
                }
            }catch{
                if(writer != null){
                    writer.Close();
                }
                if(networkStream != null){
                    networkStream.Close();
                }
                if(client != null){
                    client.Close();
                }
                if(onlineMode){
                    onlineMode = false;
                }
            }
            #endregion
            #region receiveData
            #endregion
        }
        
    }

}
