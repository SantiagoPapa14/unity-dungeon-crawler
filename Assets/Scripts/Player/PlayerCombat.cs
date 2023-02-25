using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public Weapon weapon;
    PlayerHealth playerHealth;
    LayerMask playerMask;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = transform.GetComponent<PlayerHealth>();
        weapon = transform.Find("Weapon").transform.GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && weapon.staminaCost <= playerHealth.stamina && weapon.currentCooldown<=0)
        {
            playerHealth.stamina -= weapon.staminaCost;
            Vector3 mousePos = Input.mousePosition;
            {
                if(mousePos.x >= Screen.width/2){
                    weapon.attack(1);
                }else{
                    weapon.attack(-1);
                }
            }
        }
        if(Input.GetButtonUp("Fire1")){
            weapon.attack(0);
        }
        
    }
}