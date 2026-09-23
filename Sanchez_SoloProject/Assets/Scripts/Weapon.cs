using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
    public bool canAkimbo = false;
    public bool burst = false;
    public int burstCounter;
    public int weaponID;
    public string weaponName;

    [Header("Weapon Stats")]
    public int weaponDmg;
    public float projLifespan;
    public float projVelocity;
    public float reloadCooldown;
    public float rof;
    public float burstCooldown;
    public int burstAmt;
    public int fireModes;
    public int currentFireMode;
    public int mag;
    public int magSize;

    [Header("Ammo")]
    public int ammo;
    public int maxAmmo;
    public int ammoRefill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firepoint = transform.GetChild(0);
        firingDirection = Camera.main;
    }

    public void equip(PlayerController p)
    {
        player = p;

        player.currentWeapon = this;

        transform.SetPositionAndRotation(player.weaponSlot.position, player.weaponSlot.rotation);
        transform.SetParent(player.weaponSlot);
    }

    public void equipAkimbo(PlayerController p)
    {
        player = p;

        player.akimboWeapon = this;
        player.akimbo = true;

        transform.SetPositionAndRotation(player.akimboSlot.position, player.akimboSlot.rotation);
        transform.SetParent(player.akimboSlot);
    }

    public void unequip()
    {
        if(!player.akimboWeapon)
        {
            player.currentWeapon = null;
        }
        else
        {
            player.akimboWeapon = null;
            player.akimbo = false;
        }
            
        transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);
        transform.SetParent(null);

         player = null;
    }

    public void reload()
    {
        if (mag >= magSize)
            return;

        int reloadCount = magSize - mag;
        
        if(ammo < reloadCount)
        {
            mag += ammo;
            ammo = 0;
        }
        else
        {
            mag += reloadCount;
            ammo -= reloadCount;
        }

        reloading = true;
        canFire = false;
        StartCoroutine("reloadingCooldown");
    }

    public void fire()
    {
        if(mag > 0 && canFire && !reloading)
        {
            mag--;

            GameObject p = Instantiate(projectile, firepoint.position, firepoint.rotation);
            p.GetComponent<BulletDmg>().damage = weaponDmg;
            p.GetComponent<Rigidbody>().AddForce(firingDirection.transform.forward * projVelocity);
            Destroy(p, projLifespan);

            canFire = false;

            StartCoroutine("cooldownFire");
        }
    }
    IEnumerator burstDuration()
    {
        yield return new WaitForSeconds(rof*burstAmt);
    }

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(rof);

        if (mag > 0)
        {
            canFire = true;
        }
            
    }

    IEnumerator reloadingCooldown()
    {
        yield return new WaitForSeconds(reloadCooldown);

        reloading = false;
        canFire = true;
    }
    
}
