using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;

    public int inkCartridges;
    public int paperSheets;

    public float speed = 5.0f;
    public float jumpHeight = 10.0f;
    public float jumpDetectDistance = 1f;
    public float interactDistance = 5f;
    public float fireDmgTickrate = 0.1f;

    public bool isAttacking;
    public bool akimboAttacking;
    public bool fireDmg = false;
    public bool akimbo = false;
    public bool canReloadAkimbo = false;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        weaponSlot = playerCam.transform.GetChild(0);
        akimboSlot = playerCam.transform.GetChild(1);

        interactRay = new Ray();
        jumpRay = new Ray();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if(health <= 0)
        {

        }

        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        if (Physics.Raycast(interactRay, out interactHit, interactDistance))
        {
            if (interactHit.collider.tag == "Weapon" || interactHit.collider.tag == "Ammo")
            {
                pickupObj = interactHit.collider.gameObject;
            }
            else
                pickupObj = null;
        }
        else
            pickupObj = null;

        if (currentWeapon)
            if ((currentWeapon.holdToAttack || currentWeapon.burst) && isAttacking)
                currentWeapon.fire();

        if (akimboWeapon)
        {
            if((akimboWeapon.holdToAttack || akimboWeapon.burst) && akimboAttacking)
                akimboWeapon.fire();
        }


                Vector3 tempMove = rb.linearVelocity;

        tempMove.x = moveInput.x * speed;
        tempMove.z = moveInput.y * speed;

        rb.linearVelocity = (tempMove.x * transform.right) + (tempMove.y * transform.up) + (tempMove.z * transform.forward);
    }

    private void OnCollsionEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Hazard")
        {
            health--;
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

    IEnumerator fireDmgCooldown()
    {
        fireDmg = true;

        yield return new WaitForSeconds(fireDmgTickrate);

        health--;
        fireDmg = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump()
    {
        if (Physics.Raycast(jumpRay, jumpDetectDistance))
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

                pickupObj = null;
            }
            else if (currentWeapon)
                Reload();
        }
    }

    public void Reload()
    {
        if (currentWeapon)
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
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(currentWeapon)
        {
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    isAttacking = true;
                else
                    isAttacking = false;
            }
            else if (context.ReadValueAsButton() && !currentWeapon.bursting)
                currentWeapon.fire();
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
        if(!akimboWeapon && currentWeapon)
        {
            currentWeapon.GetComponent<WeaponScript>().unequip();
        }
        else if (akimboWeapon)
            akimboWeapon.GetComponent<WeaponScript>().unequip();
    }
}
