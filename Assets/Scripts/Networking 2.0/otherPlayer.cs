using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class otherPlayer : MonoBehaviour
{

    public string myName;
    public int myId;
    public float mySpeed = -1;
    public Transform myMoveTo;

    // Start is called before the first frame update
    void Start()
    {
        myName = gameObject.name;
        if(myMoveTo==null){
            myMoveTo = GameObject.Find(myName+"MoveTo").transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position != myMoveTo.position){
         transform.position = Vector2.MoveTowards(transform.position, myMoveTo.position, mySpeed*Time.fixedDeltaTime);   
        }
    }
}
