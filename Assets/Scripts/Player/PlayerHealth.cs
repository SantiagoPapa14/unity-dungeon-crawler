using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public float health;
    public Image healthBar;
    public float stamina;
    public Image staminaBar;
    public bool drainingStamina;
    public float mana;

    // Start is called before the first frame update
    void Start()
    {
        health = 10;
        healthBar.fillAmount = health/10;
        drainingStamina=false;   
    }

    // Update is called once per frame
    void Update()
    {
        if(!drainingStamina && stamina<=10){
            stamina+=.5f*Time.deltaTime;
        }
        staminaBar.fillAmount = stamina/10;
    }

    void takeDamage(float dmg){
        health-=dmg;
        healthBar.fillAmount = health/10;
    }
}
