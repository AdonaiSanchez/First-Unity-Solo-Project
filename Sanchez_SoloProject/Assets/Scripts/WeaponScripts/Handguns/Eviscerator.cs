public class Eviscerator : WeaponScript
{
    // Update is called once per frame
    void Update()
    {
        if (equipped)
        {
            weaponName = "Eviscerator";
        }
        else
        {
            weaponName = "M1911?";
        }
    }
}
