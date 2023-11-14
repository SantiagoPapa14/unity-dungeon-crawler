using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Transform moveTo;
    public float moveSpeed;
    public Animator anim;
    public PlayerHealth pHealth;

    public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {   
        moveTo.parent = null;
        pHealth = transform.GetComponent<PlayerHealth>();
        isMoving = false;
    }

    // Update is called once per frame

    void Update()
    {
        checkSprint();
        movement();
        anim.SetBool("Moving", isMoving);
    }

    void movement(){
        transform.position = Vector3.MoveTowards(transform.position, moveTo.position, moveSpeed*Time.deltaTime);
        if(Vector3.Distance(transform.position, moveTo.position) <= .5f){

            if(Mathf.Abs(Input.GetAxisRaw("Horizontal"))==1f){
                moveTo.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            }
            else if(Mathf.Abs(Input.GetAxisRaw("Vertical"))==1f){
                moveTo.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            }
            isMoving = false;
        }
        else{
            isMoving = true;
        }
    }

    void checkSprint(){
        if(Input.GetKey(KeyCode.LeftShift) && pHealth.stamina > 0.1f){
            moveSpeed = 7.5f;
            pHealth.stamina-=Time.deltaTime;
            if(!pHealth.drainingStamina){
                pHealth.drainingStamina=true;
            }
        }else{
            moveSpeed=5f;
            pHealth.drainingStamina=false;
        }
    }
    
}
