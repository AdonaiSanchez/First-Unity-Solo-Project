using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public GameObject player;
    public GameObject teleporterLinked;

    public Transform teleportPoint;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        teleportPoint = teleporterLinked.transform.GetChild(0);
    }

    public void Teleport()
    {
        player.transform.SetPositionAndRotation(teleportPoint.position, teleportPoint.rotation);
    }
}
