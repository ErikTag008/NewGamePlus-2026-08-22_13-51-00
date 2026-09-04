namespace Project.Assets._Project._Scripts.Boss
{
    public interface IBossCombat
    {
        void ToggleRightHandCollider(bool isActive);
        void ToggleLeftHandCollider(bool isActive);
        void CheckJumpAttackDamage();
    }
}