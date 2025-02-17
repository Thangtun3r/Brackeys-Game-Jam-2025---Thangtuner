public interface IBulletDamage
{
    float Damage { get; }
    void ApplyDamage(IDamageable target);
}