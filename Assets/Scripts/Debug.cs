using UnityEngine;
using Weapons;

public class Debug : MonoBehaviour
{
    private PlayerScript _player;

    public GameObject pistol;
    public GameObject rifle;
    public GameObject shotgun;

    private void Start()
    {
        _player = GetComponent<PlayerScript>();
    }

    public void AddPistol()
    {
        var pistolObj = Instantiate(pistol);
        _player.Inventory.AddWeapon(pistolObj.GetComponent<CollectableWeapon>());
    }
    
    public void AddRifle()
    {
        var rifleObj = Instantiate(rifle);
        _player.Inventory.AddWeapon(rifleObj.GetComponent<CollectableWeapon>());
    }
    
    public void AddShotgun()
    {
        var shotgunObj = Instantiate(shotgun);
        _player.Inventory.AddWeapon(shotgunObj.GetComponent<CollectableWeapon>());
    }
}