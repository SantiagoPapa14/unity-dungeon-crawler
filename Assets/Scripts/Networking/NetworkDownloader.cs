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
                    manager.setPlayerPos(recv.username, pos, recv.animDirection);
                }
            }


        }catch(Exception ex){
            Debug.Log(packet);
            Debug.Log(ex.ToString());
        }
    }
    void recvMsg(){
        string packetTemp = "";
        int count = 0;
        while(true){
            if(manager.onlineMode){
                try{           
                    char c = manager.reader.ReadChar();
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
                }catch{
                    manager.checkConn("downloader");
                    break;
                }
            }
        }
    }       
} 
