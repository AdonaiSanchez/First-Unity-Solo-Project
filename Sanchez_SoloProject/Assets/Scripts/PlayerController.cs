using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    [Header("Player Stats")]
    public int maxHealth = 100;
    public int health = 100;
    public int maxArmor = 150;
    public int armor;
    
    public int inkCartridges;
    public int paperScraps;
    public int paperSheets;
    
    public float speed = 5.0f;
    public float sprintBoost = 1.5f;
    public float jumpHeight = 10.0f;

    [Header("Meta Stats")]
    public float jumpDetectDistance = 1f;
    public float interactDistance = 5f;
    public float fireDmgTickrate = 0.1f;
    public float regenTickrate = 0.01f;
    
    public bool canReloadAkimbo = false;
    public bool isAttacking;
    public bool akimboAttacking;
    public bool fireDmg = false;
    public bool canRegen = false;
    public bool akimbo = false;
    public bool aiming = false;
    public bool reloading;

    public bool onGround = true;
    
    public bool canSprint = false;
    public bool sprinting = false;
    public bool toggleSprint = false;
    public bool sprintLock = false;

    public bool armored = false;

    Ray jumpRay;
    Ray interactRay;
    RaycastHit interactHit;
    Vector2 moveInput = Vector2.zero;

    public WeaponScript currentWeapon;
    public WeaponScript akimboWeapon;
    Camera playerCam;
    public Transform weaponSlot;
    public Transform akimboSlot;
    PlayerInput input;
    Rigidbody rb;
    public GameObject pickupObj;
    public GameObject gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        weaponSlot = playerCam.transform.GetChild(0);
        akimboSlot = playerCam.transform.GetChild(1);

        interactRay = new Ray();
        jumpRay = new Ray();
    }

    private void FixedUpdate()
    {
        Quaternion playerRotation = Quaternion.identity;
        playerRotation.y = playerCam.transform.rotation.y;
        playerRotation.w = playerCam.transform.rotation.w;
        transform.rotation = playerRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (armor > 0)
        {
            armored = true;
        }
        else
            armored = false;

        if (health <= 0)
        {
            SceneManager.LoadScene(1);
            /*if (currentWeapon && akimboWeapon)
            {
                akimboWeapon.unequip();
                currentWeapon.unequip();
            }
            else if (currentWeapon)
            {
                currentWeapon.unequip();
            }

            transform.SetPositionAndRotation(GameObject.Find("Spawnpoint").transform.position, GameObject.Find("Spawnpoint").transform.rotation);
            health = maxHealth;*/
        }

        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        onGround = Physics.Raycast(jumpRay, jumpDetectDistance);

        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        if (Physics.Raycast(interactRay, out interactHit, interactDistance))
        {
            if (interactHit.collider.tag == "SmallPrinter" || interactHit.collider.tag == "Converter" || interactHit.collider.tag == "Armor" || interactHit.collider.tag == "Weapon" || interactHit.collider.tag == "Ammo" || interactHit.collider.tag == "Teleporter" || interactHit.collider.tag == "StartWave")
            {
                pickupObj = interactHit.collider.gameObject;
            }
            else
                pickupObj = null;
        }
        else
            pickupObj = null;


        if (currentWeapon)
        {
            if ((currentWeapon.holdToAttack || currentWeapon.burst) && isAttacking)
                currentWeapon.fire();

            if (currentWeapon.canLoad && reloading)
            {
                currentWeapon.reload();
                if(currentWeapon.mag >= currentWeapon.magSize)
                {
                    reloading = false;
                }
            }
        }
            
        if (akimboWeapon)
        {
            if((akimboWeapon.holdToAttack || akimboWeapon.burst) && akimboAttacking)
                akimboWeapon.fire();

        }

        Vector3 tempMove = rb.linearVelocity;

        if (!gameManager.GetComponent<GameManager>().menuOpen)
        {
            tempMove.x = moveInput.x * speed;
            tempMove.z = moveInput.y * speed;
        }
        else
        {
            tempMove.x = moveInput.x * 0;
            tempMove.z = moveInput.y * 0;
        }


        if (sprinting)
        {
            tempMove.z *= sprintBoost;
            tempMove.x *= sprintBoost;
        }

        /*if (!sprinting)
        {
            if (!canSprint)
            {

            }
        }*/
        
        rb.linearVelocity = (tempMove.x * transform.right) + (tempMove.y * transform.up) + (tempMove.z * transform.forward);
                    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ammo" && currentWeapon)
        {
            int refillAmt = currentWeapon.maxAmmo - currentWeapon.ammoRefill;

            if (refillAmt >= currentWeapon.maxAmmo)
            {
                currentWeapon.ammo = currentWeapon.maxAmmo;
            }
            else
                currentWeapon.ammo += currentWeapon.ammoRefill;

            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Armor" && armor != maxArmor)
        {
            int armorMissing = maxArmor - armor;

            if (armorMissing <= 25)
                armor = maxArmor;
            else
                armor += 25;

            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Heal")
        {
            if (!canRegen && health < maxHealth)
            {
                StartCoroutine("healCooldown");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Heal")
        {
            if (canRegen)
            {
                StopCoroutine("fireDmgCooldown");
                canRegen = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyProjectile")
        {
            if (armored)
            {
                int armorDmg = (int)((float)collision.gameObject.GetComponent<BulletDmg>().damage * (1f - collision.gameObject.GetComponent<BulletDmg>().armorPen)) + collision.gameObject.GetComponent<BulletDmg>().damage;

                if (armor >= armorDmg)
                    armor -= armorDmg;
                else
                    armor = 0;

                health -= (int)((float)collision.gameObject.GetComponent<BulletDmg>().damage * collision.gameObject.GetComponent<BulletDmg>().armorPen);
            }
            else
            {
                health -= collision.gameObject.GetComponent<BulletDmg>().damage;
            }

                Destroy(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
       if(collision.gameObject.tag == "Hazard")
        {
            if(!fireDmg)
            {
                StartCoroutine("fireDmgCooldown");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Hazard")
        {
            if(fireDmg)
            {
                StopCoroutine("fireDmgCooldown");
                fireDmg = false;
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (!toggleSprint)
        {
            if (context.ReadValueAsButton())
                sprinting = true;
            else
                sprinting = false;
        }
        else
            if (context.performed)
                sprinting = !sprinting;
    }

    public void Jump()
    {
        if (onGround && !gameManager.GetComponent<GameManager>().paused)
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton())
        {
            if (pickupObj)
            {
                if (pickupObj.tag == "Weapon" && !currentWeapon)
                {
                    pickupObj.GetComponent<WeaponScript>().equip(this);
                }
                else if(pickupObj.tag == "Weapon" && currentWeapon && currentWeapon.canAkimbo == true && !akimboWeapon && pickupObj.GetComponent<WeaponScript>().weaponID == currentWeapon.weaponID)
                {
                    pickupObj.GetComponent<WeaponScript>().equipAkimbo(this);
                }

                if (pickupObj.tag == "Ammo" && currentWeapon)
                {
                    int refillAmt = currentWeapon.maxAmmo - currentWeapon.ammoRefill;

                    if (refillAmt >= currentWeapon.maxAmmo)
                    {
                        currentWeapon.ammo = currentWeapon.maxAmmo;
                    }
                    else
                        currentWeapon.ammo += currentWeapon.ammoRefill;

                    Destroy(pickupObj);
                }

                if (pickupObj.tag == "Armor" && armor != maxArmor)
                {
                    int armorMissing = maxArmor - armor;

                    if (armorMissing <= 25)
                        armor = maxArmor;
                    else
                        armor += 25;

                    Destroy(pickupObj);
                }

                if (pickupObj.tag == "Teleporter" && !gameManager.GetComponent<GameManager>().wavesActive)
                {
                    pickupObj.GetComponent<Teleporter>().Teleport();
                }

                if (pickupObj.tag == "StartWave")
                {
                    pickupObj.GetComponent<InkSynthesizer>().startWave();
                }

                if(pickupObj.tag == "Converter")
                {
                    gameManager.GetComponent<GameManager>().OpenMenu(0);
                }

                if (pickupObj.tag == "SmallPrinter")
                {
                    gameManager.GetComponent<GameManager>().OpenMenu(2);
                }

                pickupObj = null;
            }
            else if (gameManager.GetComponent<GameManager>().menuOpen)
                gameManager.GetComponent<GameManager>().OpenMenu(0);
        }
    }

    public void Reload()
    {
        if (currentWeapon && !currentWeapon.singleLoad)
        {
            if (akimboWeapon && canReloadAkimbo)
            {
                if (!akimboWeapon.reloading)
                {
                    akimboWeapon.reload();
                }
            }
            else if(akimboWeapon && !canReloadAkimbo)
                akimboWeapon.GetComponent<WeaponScript>().unequip();

            if (!currentWeapon.reloading)
            {
                currentWeapon.reload();
            }
        }
        else if(currentWeapon && currentWeapon.singleLoad)
        {
            reloading = true;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(currentWeapon && !gameManager.GetComponent<GameManager>().paused && !gameManager.GetComponent<GameManager>().menuOpen)
        {
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    isAttacking = true;
                else
                    isAttacking = false;
            }
            else if (context.ReadValueAsButton() && !currentWeapon.bursting)
            {
                currentWeapon.fire();

                if (currentWeapon.singleLoad)
                    reloading = false;
            }

        }    
    }

    public void Akimbo(InputAction.CallbackContext context)
    {
        if (akimboWeapon)
        {
            if (akimboWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    akimboAttacking = true;
                else
                    akimboAttacking = false;
            }

            else if (context.ReadValueAsButton() && !akimboWeapon.bursting)
                akimboWeapon.fire();
        }
    }

    public void DropWeapon()
    {
        if (akimbo)
        {
            Debug.Log("Akimbo dropped");
            akimboWeapon.GetComponent<WeaponScript>().unequip();
        }
        
        if (!akimbo)
        {
            Debug.Log("Weapon dropped");
            currentWeapon.GetComponent<WeaponScript>().unequip();
        }
    }

    IEnumerator fireDmgCooldown()
    {
        fireDmg = true;

        yield return new WaitForSeconds(fireDmgTickrate);

        health--;
        fireDmg = false;
    }

    IEnumerator healCooldown()
    {
        canRegen = true;

        yield return new WaitForSeconds(regenTickrate);

        int healthMissing = maxHealth - health;

        if (healthMissing <= (int)(maxHealth * 0.05))
            health = maxHealth;
        else
            health += (int)(maxHealth*0.05);

        canRegen = false;
    }
}
