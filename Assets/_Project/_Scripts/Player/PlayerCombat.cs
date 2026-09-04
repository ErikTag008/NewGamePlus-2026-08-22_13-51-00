using Project.Assets._Project._Scripts.Weapons;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Player
{
    public class PlayerCombat : IPlayerCombat
    {
        private readonly Transform _cameraTransform;
        private readonly ICombatStats _stats;
        private IWeapon _currentWeapon;

        public PlayerCombat(Transform camera, ICombatStats stats)
        {
            _cameraTransform = camera;
            _stats = stats;
        }

        public void UpdateTarget()
        {
            Ray ray = new(_cameraTransform.position, _cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, _stats.MaxHitDistance, _stats.AttackRaycastLayer))
            {
                _currentWeapon?.UpdateTarget(hitInfo.point);
            }
            else
            {
                _currentWeapon?.UpdateTarget(ray.GetPoint(_stats.MaxHitDistance));
            }
        }

        public void HandleAttack(bool isAttackDown)
        {

            Debug.Log("CurrentWeapon: " + _currentWeapon);
            if (isAttackDown)
            {
                _currentWeapon?.Attack();
            }
            else
            {
                _currentWeapon?.StopAttack();
            }
        }

        public void ChangeWeapon(IWeapon weapon)
        {
            _currentWeapon?.StopAttack();
            _currentWeapon = weapon;
        }

        public void HandleReload()
        {
            _currentWeapon?.Reload();
        }
    }
}
