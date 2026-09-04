using Project.Assets._Project._Scripts.Player;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Boss
{
    public class BossCombat : IBossCombat
    {
        private readonly AttackCollider _rightHandCollider, _leftHandCollider;
        private readonly Transform _jumpAttackOrigin;
        private readonly BossStats _stats;
        private Collider[] _jumpAttackColliders = new Collider[1];
        public BossCombat(AttackCollider rightHand, AttackCollider leftHand, Transform jumpAttackOrigin, BossStats stats)
        {
            _rightHandCollider = rightHand;
            _leftHandCollider = leftHand;
            _jumpAttackOrigin = jumpAttackOrigin;
            _stats = stats;
        }
        public void ToggleLeftHandCollider(bool isActive)
        {
            _leftHandCollider.ToggleColider(isActive, _stats.LeftHandDamage);
        }

        public void ToggleRightHandCollider(bool isActive)
        {
            _rightHandCollider.ToggleColider(isActive, _stats.RightHandDamage);
        }

        public void CheckJumpAttackDamage()
        {

            if(Physics.OverlapSphereNonAlloc(_jumpAttackOrigin.position, _stats.JumpAttackRadius, _jumpAttackColliders, _stats.PlayerLayer) > 0
                && _jumpAttackColliders[0].TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(_stats.JumpAttackDamage);
            }
            
        }
    }
}