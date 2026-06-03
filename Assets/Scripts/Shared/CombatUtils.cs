using UnityEngine;

public static class CombatUtils
{
    public static bool TryDamagePlayer(GameObject target, int damage, Vector3 sourcePosition)
    {
        PlayerHealth health = target.GetComponent<PlayerHealth>();
        if (health == null || health.isInvincible)
            return false;

        health.TakeDamage(damage);

        PlayerKnockBack knockback = target.GetComponent<PlayerKnockBack>();
        if (knockback != null)
        {
            Vector2 direction = (target.transform.position - sourcePosition).normalized;
            knockback.ApplyKnockback(direction);
        }

        return true;
    }
}
