using UnityEngine;

public class ItemHover : MonoBehaviour
{
    public Ray hoverRay;

    public bool grounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hoverRay = new Ray();
    }

    // Update is called once per frame
    void Update()
    {
        hoverRay.origin = transform.position;
        hoverRay.direction = -transform.up;

        grounded = Physics.Raycast(hoverRay, 1);

        if (grounded)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
