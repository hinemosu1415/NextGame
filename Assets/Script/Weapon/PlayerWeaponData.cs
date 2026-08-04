using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeaponData", menuName = "Weapon/PlayerWeaponData")]
public class PlayerWeaponData : ScriptableObject
{
    public WeaponBase weaponBase;
    public Vector2 EquippedOffset;
    public Sprite Image;
}
