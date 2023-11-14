using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Transform movePoint;
    public float moveSpeed;
    public Animator anim;
    PlayerHealth pHealth;

    // Start is called before the first frame update
    void Start()
    {   
        movePoint.parent = null;
        pHealth = transform.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame

    void Update()
    {
        checkSprint();
        movement();
    }

    void movement(){
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);
        if(Vector3.Distance(transform.position, movePoint.position) <= .5f){

            if(Mathf.Abs(Input.GetAxisRaw("Horizontal"))==1f){
                movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            }
            else if(Mathf.Abs(Input.GetAxisRaw("Vertical"))==1f){
                movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            }
            anim.SetBool("Moving", false);
        }
        else{
            anim.SetBool("Moving", true);
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
