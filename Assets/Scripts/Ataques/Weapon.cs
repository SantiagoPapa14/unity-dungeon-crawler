using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public float staminaCost;
    public float damage;
    public Transform attackTo;
    public Animator animator;
    public float cooldown;
    public float currentCooldown;


    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
        attackTo = transform.Find("attackTo").transform;
    }

    // Update is called once per frame
    void FixedUpdate(){
         if(currentCooldown >= 0){
            currentCooldown -= Time.deltaTime;
        }
    }

    public void attack(int side){
        animator.SetInteger("attacking",side);
        currentCooldown = cooldown;
    }
}
