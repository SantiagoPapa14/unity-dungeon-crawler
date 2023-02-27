using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerState{
private Vector3 position;

public string username;
public string posX;
public string posY;

public Vector2 animDirection;

public PlayerState(string username){
    this.username = username;
}

public void setPos(Vector2 position){

    this.position.x = position.x;
    this.position.y = position.y;
}

public void setAnimation(Vector2 anim){
    this.animDirection = anim;
}

public Vector3 getPos(){
    return position;
}
public string jsonString(){

    this.posX = this.position.x.ToString("F1");
    this.posY = this.position.y.ToString("F1");
    string data = JsonUtility.ToJson(this);
    return data;
}

}
public class NetworkUploader : MonoBehaviour
{

    #region Variables


    PlayerState pState;
    Transform player;
    NetworkManager manager;
    #endregion

    void Start()
    {
        manager = transform.parent.transform.GetComponent<NetworkManager>();
        player = GameObject.Find("Player").transform;
        pState = new PlayerState(manager.username); 
    }
    void FixedUpdate()
    {
        if(manager.onlineMode){
            try{               
                if(pState.username != manager.username){pState.username=manager.username;}   
                if(Mathf.Abs(player.position.x - pState.getPos().x) > 0.1f || Mathf.Abs(player.position.y -pState.getPos().y) > 0.1f){
                    pState.setPos(player.position);
                    pState.setAnimation(player.GetComponent<PlayerMovement>().animDirection);
                    string sData = pState.jsonString();
                    char[] cData = new char[sData.Length];
                    for (int i = 0; i < sData.Length; i++)
                    {
                        cData[i] = sData[i];
                    }
                    manager.writer.Write(cData);
                    manager.writer.Flush();
                }
            }catch{
                manager.checkConn("uploader");
                if(player == null){
                    player = GameObject.Find("Player").transform;
                }
            }
        }
    }
}
