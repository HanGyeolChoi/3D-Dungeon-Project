using System;
using System.Collections;
using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float damage);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;
    private Condition health { get { return uiCondition.health; } }
    private Condition stamina { get { return uiCondition.stamina; } }

    public event Action OnDeath;
    private void Update()
    {
        stamina.Add(stamina.passiveValue * Time.deltaTime);
        if(health.curValue <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void StaminaHeal(float amount)
    {
        stamina.Add(amount);
    }

    public void Rest()
    {
        health.Add(health.maxValue);
        stamina.Add(stamina.maxValue);
    }

    public bool UseStamina(float amount)
    {
        if(stamina.curValue < amount)
        {
            return false;
        }

        stamina.curValue -= amount;
        return true;
    }

    public void TakeDamage(float damage)
    {
        health.Add(-damage);
    }

    public void SpeedUp(float amount)
    {
        StartCoroutine(SpeedChange(amount));
    }
    private IEnumerator SpeedChange(float amount)
    {
        CharacterManager.Instance.Player.controller.SetSpeed(amount);
        yield return new WaitForSecondsRealtime(5f);
        CharacterManager.Instance.Player.controller.SetOriginalSpeed();
    }
}
