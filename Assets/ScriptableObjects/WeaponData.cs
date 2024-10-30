using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class WeaponData : ScriptableObject
{
    public bool isRange;
    public float damage;
    public float attackRate;
    public float useStamina;

    [Header("Melee Weapon")]
    public float attackDistance;

    [Header("Ranged Weapon")]
    public float attackRange;
    public GameObject projectilePrefab;

}