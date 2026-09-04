namespace Project.Assets._Project._Scripts.Player
{
    public interface IDamagable
    {
        void TakeDamage(float damage);
        bool IsDead { get; }
    }
}