using UnityEngine;

public class EquipWeapon : Equip
{
    public WeaponData weaponData;
    private bool attacking;

    private Animator animator;
    private Camera _camera;

    
    private void Start()
    {
        animator = GetComponent<Animator>();
        _camera = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking && CharacterManager.Instance.Player.condition.UseStamina(weaponData.useStamina))
        {
            attacking = true;
            animator.SetTrigger("Attack");
            Invoke("OnCanAttack", weaponData.attackRate);
        }
    }

    public void OnCanAttack()
    {
        attacking = false;
    }
    public void OnHit()
    {
        if(weaponData.isRange)
        {
        
            Instantiate(weaponData.projectilePrefab,transform.position + new Vector3(0,0.8f,0), transform.rotation * Quaternion.Euler(0,0,90));    
        }

        else { 
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        
            if (Physics.Raycast(ray, out hit, weaponData.attackDistance))
            {
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.TakeDamage(weaponData.damage);
                }
            }
        }

    }
}