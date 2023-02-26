using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public Weapon weapon;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
        weapon = transform.Find("Weapon").transform.GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = Input.mousePosition;
            if(mousePos.x >= Screen.width/2){
                //Add damage and hit script here
                animator.SetTrigger("AttackRight");
            }else{
                //Add damage and hit script here
                animator.SetTrigger("AttackLeft");
            }
        }
        
    }
}
