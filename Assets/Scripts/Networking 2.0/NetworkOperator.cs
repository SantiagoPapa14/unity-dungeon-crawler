using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using UnityEngine;
using TMPro;

public class Packet{
    public string username;
    public int id;
    public int type = 0;

    public string returnJson(){
        return JsonUtility.ToJson(this);
    }
}
public class RenderPacket : Packet{
    public int[] startPosition;
}
public class ServerPacket : Packet{

}
public class PlayerUpdatePacket : RenderPacket{
    public int playerClass;
    public int[] moveTo;
    public int currentSpeed;

    public PlayerUpdatePacket(string user, int index, int[] startPos, int[] movePos, int speed){
        this.username = user;
        this.type = 1;
        this.id = index;
        this.startPosition = startPos;
        this.moveTo = movePos;
        this.currentSpeed = speed;
    }

    new public string returnJson(){
        return JsonUtility.ToJson(this);
    }

}
public class Client{

}

public class PlayerInformation{
    public string username;
    public int id;
    public int playerClass;
    public int[] position;
    public int[] moveTo;
    public int speed;

    public PlayerInformation(string username, int playerClass, int id, int[] position, int[] moveTo, int speed){
        this.username = username;
        this.playerClass = playerClass;
        this.id = id;
        this.position = position;
        this.moveTo = moveTo;
        this.speed = speed;
    }

}

public class NetworkOperator : MonoBehaviour
{
#region Variables
    #region Client-Server Communication Variables
    TcpClient client;
    NetworkStream networkStream;
    BinaryWriter writer;
    BinaryReader reader;
    Thread _receiverThread;
    #endregion Client-Server Communication Variables

    #region General Online Variables
    string serverAddress = "127.0.0.1";
    int serverPort = 1404;
    public bool onlineMode = false;

    string username = "Borat";
    int playerIndex = 0;
    public GameObject spawnablePlayer;

    List<PlayerInformation> playersInformation = new List<PlayerInformation>();
    List<GameObject>        playersObjects     = new List<GameObject>();
    List<GameObject>        moveToObjects      = new List<GameObject>();
    #endregion General Online Variables

    #region Movement Transmission Variables
    public Transform player;
    public Transform lastMoveto;
    public PlayerController pMovement;
    public float lastRegisteredSpeed;
    #endregion Movement Transmission Variables
#endregion Variables
#region Methods

    #region Unity Main Thread Functions
    void Start(){
        //Hook objects before they get a new name
        player = GameObject.Find("Player").transform;
        pMovement = player.GetComponent<PlayerController>();
        lastMoveto = new GameObject("lastMovetoRegistered").transform;
        lastMoveto.parent=null;

        //Set online variables
        _receiverThread = new Thread(receivePackets);
        onlineMode = false;
    }
    void Update(){
        uploadMovement();
        syncronizeObjects();
    }
    #endregion Unity Main Thread Functions

    #region Generic Client Functions
    public void sendPacket(Packet packet){

        #region Serialize packet into char array to be sent by writer
        string sData = packet.returnJson();
        char[] cData = new char[sData.Length];
        for (int i = 0; i < sData.Length; i++)
        {
            cData[i] = sData[i];
        }
        #endregion 

        #region Try To Send Packet Information
        try{
            writer.Write(cData);
            writer.Flush();                      
        }catch(Exception e){
            Debug.Log("Couldn't send package:\n" + sData + " Disconnecting, error:\n" + e.ToString());
            client.Close();
            onlineMode = false;
        }
        #endregion                                    
    }
    public void connectToServer(){
        try{
            //Get server address and username 
            string newHost = GameObject.Find("serverAddress").transform.GetComponent<TMP_InputField>().text;
            string newUsername = GameObject.Find("playerUsername").transform.GetComponent<TMP_InputField>().text;
            serverAddress = newHost;
            username = newUsername;

            //Set Client and information streams
            client = new TcpClient(serverAddress, serverPort);
            networkStream = client.GetStream(); 
            writer = new BinaryWriter(networkStream);
            reader = new BinaryReader(networkStream); 
            if(!_receiverThread.IsAlive){
                _receiverThread.Start();
            }
            //Allow network methods to start
            onlineMode = true;
            Debug.Log("Connected to the server.");
        }
        catch(Exception e){
            onlineMode = false;
            Debug.Log("Could NOT connect to the server:\n" + e.ToString());
        } 
    }
    #endregion Generic Client Functions

    #region Information and Object Sync Methods
    public int getInformationIndexByUsername(string username){
        foreach(PlayerInformation pInfo in playersInformation){
            if(pInfo.username==username){ return playersInformation.IndexOf(pInfo); }
        }
        //returns -1 if player is not known
        return -1;
    }
    public bool playerExists(string username){
        foreach(GameObject playerObj in playersObjects){
            if(playerObj.name==username){ return true; }
        }
        return false;
    }
    bool hasUnrenderedPlayers(){
        return !(playersInformation.Count==playersObjects.Count);
    }
    Vector3[] pInfoVector3Parse(PlayerInformation pInfo){
        Vector3 playerPos = new Vector3(
            ((float)pInfo.position[0])/10,
            ((float)pInfo.position[1])/10
        );
        Vector3 moveToPos = new Vector3(
            ((float)pInfo.moveTo[0])/10,
            ((float)pInfo.moveTo[1])/10
        );
        return new [] { playerPos, moveToPos };
    }
    void spawnPlayer(PlayerInformation pInfo){
        GameObject newPlayer = Instantiate(spawnablePlayer);
        newPlayer.transform.parent = null;
        newPlayer.transform.name = pInfo.username;

        GameObject newPlayerMoveTo = new GameObject(pInfo.username+"MoveTo");
        newPlayerMoveTo.transform.parent = null;

        Vector3 playerPos = pInfoVector3Parse(pInfo)[0];
        Vector3 moveToPos = pInfoVector3Parse(pInfo)[1];

        newPlayer.transform.position = playerPos;
        newPlayerMoveTo.transform.position = moveToPos;

        newPlayer.transform.GetComponent<otherPlayer>().myMoveTo = newPlayerMoveTo.transform;
        newPlayer.transform.GetComponent<otherPlayer>().mySpeed = ((float)pInfo.speed)/10;
        newPlayer.transform.GetComponent<otherPlayer>().myId = pInfo.id;

        playersObjects.Add(newPlayer);
        moveToObjects.Add(newPlayerMoveTo);        
    }
    void updatePlayersPos(){
        foreach(PlayerInformation pInfo in playersInformation){
            int index = playersInformation.IndexOf(pInfo);
            Vector3 playerPos = pInfoVector3Parse(playersInformation[index])[0];
            Vector3 moveToPos = pInfoVector3Parse(playersInformation[index])[1];
            Vector3 diffPos = playersObjects[index].transform.position - moveToPos;
            if(diffPos.magnitude>=1.5f){
                playersObjects[index].transform.position = playerPos;
            }
            playersObjects[index].transform.GetComponent<otherPlayer>().mySpeed = ((float)playersInformation[index].speed)/10;
            moveToObjects[index].transform.position = moveToPos;
        }
    }
    void syncronizeObjects(){
        if(hasUnrenderedPlayers()){
            foreach(PlayerInformation pInfo in playersInformation){
                if(!playerExists(pInfo.username)){
                    spawnPlayer(pInfo);
                }
            }
        }
        updatePlayersPos();
    }
    #endregion

    #region Movement Transmission Methods
    PlayerUpdatePacket GetPlayerUpdatePacket(){
        PlayerUpdatePacket posPack = new PlayerUpdatePacket(
            username,
            playerIndex,
            new int[2]{(int)MathF.Round(player.position.x*10), (int)MathF.Round(player.position.y*10)},
            new int[2]{(int)MathF.Round(pMovement.moveTo.position.x*10), (int)MathF.Round(pMovement.moveTo.position.y*10)},
            (int)MathF.Round(pMovement.moveSpeed*10)
            );
            return posPack;
    }
    void uploadMovement(){
        if(onlineMode){

            #region Send package due to detected movement that hasn't been notified
            if(pMovement.isMoving){
                if(pMovement.moveTo.position != lastMoveto.position){
                    Packet packet = GetPlayerUpdatePacket();
                    sendPacket(packet);
                    lastMoveto.position = pMovement.moveTo.position;
                }
            }
            #endregion
            
            #region Send package due to new Speed
            if(pMovement.moveSpeed != lastRegisteredSpeed){
                Packet packet = GetPlayerUpdatePacket();
                sendPacket(packet);
                lastRegisteredSpeed = pMovement.moveSpeed;
            }
            #endregion

        }
    }
    #endregion Movement Transmission Methods

    #region Download Information Methods
    void receivePackets(){
        string packetTemp = "";
        int count = 0;
        while(true){
            if(onlineMode){
                try{     
                    #region Build packet and send to analyze      
                    char c = reader.ReadChar();
                    if(c == '{'){
                        count+=1;
                    }
                    else if(c=='}'){
                        count-=1;
                    }
                    packetTemp+=c;
                    if (count <= 0){
                        analyzePacket(packetTemp);
                        packetTemp = "";
                    }
                    #endregion Build packet and send to analyze  
                }catch(Exception e){
                    Debug.Log("Issue in packet receiver, disconnecting:\n"+e.ToString());
                    break;
                }
            }
        }
    } 
    void analyzePacket(string strPacket){
        try{

            Packet objPacket = JsonUtility.FromJson<Packet>(strPacket);
            if(objPacket.username != username){
                switch(objPacket.type){
                    case 0:
                        break;
                    case 1:
                        PlayerUpdatePacket posPack = JsonUtility.FromJson<PlayerUpdatePacket>(strPacket);
                        analyzeUpdatePacket(posPack);
                        break;
                }
            }                
        }catch(Exception e){
            Debug.Log("Failed to define packet:\n" + strPacket + "\nThrows:\n" + e.ToString());
        }
    }
    void analyzeUpdatePacket(PlayerUpdatePacket packet){
        //If player is known, update his position
        int playerIndex = getInformationIndexByUsername(packet.username);
        if(playerIndex!=-1){
            playersInformation[playerIndex].playerClass = packet.playerClass;
            playersInformation[playerIndex].position = packet.startPosition;
            playersInformation[playerIndex].moveTo = packet.moveTo;
        }
        //If player is NOT known add him to the queue for spawning
        else{
            PlayerInformation pInformation = new PlayerInformation(
                packet.username,
                packet.playerClass,
                playersInformation.Count,
                packet.startPosition,
                packet.moveTo,
                packet.currentSpeed
            );
            playersInformation.Add(pInformation);
        }
    }
    #endregion Download Information Methods

#endregion Methods
}
