using UnityEngine;

public class WeaponStatHandler
{
    private int weaponParts;

    public float Damage { get; private set; }

    public int MaxAmmo { get; private set; }

    public float Recoil { get; private set; }

    public float ReloadTime { get; private set; }

    public WeaponStatHandler(WeaponSO data)
    {
        Damage = data.DMG;
        MaxAmmo = data.MaxAmmo;
        Recoil = data.ShootRecoil;
        ReloadTime = data.ReloadTime;

        weaponParts = 0;
    }

    public void ChangeStat(float dmg, float recoil, int ammo, int partsValue)
    {
        Damage += dmg;
        Recoil += recoil;
        MaxAmmo += ammo;
        Debug.Log($"[장착 후] 반동: {Recoil}");
        weaponParts |= (1 << partsValue);
    }

    public bool IsPartActive(int partsValue)
    {
        return (weaponParts & (1 << partsValue)) != 0;
    }

    public void RemoveStat(float dmg, float recoil, int ammo, int partsValue)
    {
        Damage -= dmg;
        Recoil -= recoil;
        MaxAmmo -= ammo;
        weaponParts &= ~(1 << partsValue);
    }

    public int GetActivePartsCount()
    {
        int count = 0;
        for (int i = 0; i < 32; i++)
        {
            if ((weaponParts & (1 << i)) != 0)
            {
                count++;
            }
        }
        return count;
    }
}