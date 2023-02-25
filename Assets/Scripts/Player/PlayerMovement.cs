using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    PlayerHealth pHealth;


    public Transform moveTo;
    public float moveSpeed;
    public float walkSpeed = 5;
    public float sprintSpeed = 7.5f;
    public Vector3 movDirection;
    public int[] lastPress;
    public int[] keyStates;
    // Start is called before the first frame update
    void Start()
    {
        moveSpeed=sprintSpeed;
        pHealth = transform.GetComponent<PlayerHealth>();
         keyStates = new int[4];
         lastPress = new int[4];
        for (int i = 0; i < 4; i++)
        {
            keyStates[i] = 0;
            lastPress[i] = 0;
        }
        moveTo.parent = null;    
        moveTo.transform.position = transform.position;   
        movDirection = new Vector3(0,0,0); 
    }

    // Update is called once per frame
    void Update()
    {
        movementFunction();
    }

    void movementFunction(){

        if(Input.GetKey(KeyCode.LeftShift) && pHealth.stamina > 0){
            moveSpeed = sprintSpeed;
            pHealth.stamina-=Time.deltaTime;
            if(!pHealth.drainingStamina){pHealth.drainingStamina=true;}
        }else{
            moveSpeed=walkSpeed;
            pHealth.drainingStamina=false;
        }

        transform.position = Vector2.MoveTowards(transform.position, moveTo.position, moveSpeed*Time.deltaTime);
        parseToMovDirection();
        determineLastPress();
        if(Vector2.Distance(transform.position, moveTo.position)==0){
            moveTo.position += movDirection;
        }



    }
    void determineLastPress(){
        //Key States
        if(Input.GetKeyUp("w")){
            keyStates[0] = 0;
            setLastPress(-1);
        }
        if(Input.GetKeyUp("s")){
            keyStates[2] = 0;
            setLastPress(-1);
        }
        if(Input.GetKeyUp("d")){
            keyStates[3] = 0;
            setLastPress(-1);
        }
        if(Input.GetKeyUp("a")){
            keyStates[1] = 0;
            setLastPress(-1);
        }

        if(Input.GetKeyDown("w")){
            keyStates[0] = 1;
            setLastPress(0);
        }
        if(Input.GetKeyDown("s")){
            keyStates[2] = 1;
            setLastPress(2);
        }
        if(Input.GetKeyDown("d")){
            keyStates[3] = 1;
            setLastPress(3);
        }
        if(Input.GetKeyDown("a")){
            keyStates[1] = 1;
            setLastPress(1);
        }
        if(lastPress[0]==0&&lastPress[1]==0&&lastPress[2]==0&&lastPress[3]==0){
            for (int i = 0; i < 4; i++)
        {
            lastPress[i] = keyStates[i];
        }
        }
    }
    void setLastPress(int id){
        for (int i = 0; i < 4; i++)
        {
            lastPress[i] = 0;
        }
        if(id != -1){
            lastPress[id] = 1;
        }
    }
    void parseToMovDirection(){
        if(!(keyStates[2] == 1 && keyStates[0] == 1)){
            if(lastPress[0] == 1){
            movDirection.y=1;
        }else if( lastPress[2] == 1){
            movDirection.y=-1;
        }else{
            movDirection.y=0;
        }
        }else{
            movDirection.y=0;
        }

        if(!(keyStates[1] == 1 && keyStates[3] == 1)){
            if(lastPress[1] == 1){
            movDirection.x=-1;
        }else if( lastPress[3] == 1){
            movDirection.x=1;
        }else{
            movDirection.x=0;
        }
        }else{
            movDirection.x=0;
        }

        if(movDirection.x!=0){movDirection.y=0;}

    }
}
