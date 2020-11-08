public enum HitType
{
    Melee,
    Fire
}

public interface IDamageable
{
    void Hit(float damage, HitType type);
}
