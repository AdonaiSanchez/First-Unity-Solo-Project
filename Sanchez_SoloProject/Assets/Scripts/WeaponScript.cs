using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    PlayerController player;

    [Header("Object References")]
    public GameObject projectile;
    public Transform firepoint;
    public Camera firingDirection;

    [Header("Meta Attributes")]
    public bool canFire = true;
    public bool holdToAttack = true;
    public bool reloading = false;
    public int weaponID;
    public string weaponName;

    [Header("Weapon Stats")]
    public float projLifespan;
    public float projVelocity;
    public float reloadCooldown;
    public float rof;
    public int fireModes;
    public int currentFireMode;
    public int mag;
    public int magSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void equip()
    {

    }

    public void unequip()
    {

    }

    public void reload()
    {

    }

    public void fire()
    {

    }

    IEnumerator burstDuration()
    {

    }

    IEnumerator cooldownFire()
    {

    }

    IEnumerator reloadingCooldown()
    {

    }
}
