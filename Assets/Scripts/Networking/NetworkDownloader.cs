using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

public class NetworkDownloader : MonoBehaviour
{
    #region Variables
    NetworkManager manager;
    Thread _thread;
    #endregion

    void Start()
    {
        manager = transform.parent.transform.GetComponent<NetworkManager>();
        _thread = new Thread(recvMsg);
        if(!_thread.IsAlive){
            _thread.Start();
        }
    }

    // Update is called once per frame  

    void analyzePacket(string packet){
        try{
            PlayerState recv = JsonUtility.FromJson<PlayerState>(packet);

            if(recv.username != manager.username){
                
                Debug.Log(packet);
                if(!manager.isOnline(recv.username)){
                    manager.queue.Add(recv);
                }
                else{
                    Vector3 pos = new Vector3(float.Parse(recv.posX),float.Parse(recv.posY),0);
                    manager.setPlayerPos(recv.username, pos);
                }
            }


        }catch(Exception ex){
            Debug.Log(packet);
            Debug.Log(ex.ToString());
        }
    }
    void recvMsg(){
        string packetTemp = "{";
        while(true){
            if(manager.onlineMode){
                try{           
                    char c = manager.reader.ReadChar();
                    if(c!='}'){
                        packetTemp+=c;
                    }
                    else{
                        packetTemp+=c;
                        analyzePacket(packetTemp);
                        packetTemp = "";
                    }
                }catch{
                   manager.checkConn("downloader");
                    break;
                }
            }
        }
    }       
} 
