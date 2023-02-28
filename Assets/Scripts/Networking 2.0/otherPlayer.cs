using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class otherPlayer : MonoBehaviour
{

    public string myName;
    public int myId;
    public float mySpeed = -1;
    public Transform myMoveTo;
    Animator animator;
    Vector2 animDirection;
    bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        myName = gameObject.name;
        animator = transform.GetComponent<Animator>();
        isMoving = false;
        if(myMoveTo==null){
            myMoveTo = GameObject.Find(myName+"MoveTo").transform;
        }
    }

    // Update is called once per frame
    void Update(){
        setAnimationVariables();
        animator.SetFloat("Horizontal", animDirection.x);
        animator.SetFloat("Vertical", animDirection.y);
        animator.SetBool("Movement", isMoving);
    }
    void FixedUpdate()
    {
        if(transform.position != myMoveTo.position){
         transform.position = Vector2.MoveTowards(transform.position, myMoveTo.position, mySpeed*Time.fixedDeltaTime); 
         isMoving = true;
        }else{
            isMoving = false;
        }
    }

    void setAnimationVariables(){
        if(myMoveTo.position.x > transform.position.x){
                animDirection.x = 1;
        }else if (myMoveTo.position.x < transform.position.x){
            animDirection.x = -1;
        }else{
            animDirection.x = 0;
        }

        if(myMoveTo.position.y > transform.position.y){
            animDirection.y = 1;
        }else if (myMoveTo.position.y < transform.position.y){
            animDirection.y = -1;
        }else{
            animDirection.y = 0;
        }
    }
}
