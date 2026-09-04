using Project.Assets._Project._Scripts.Weapons;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Player
{
    public interface IPlayerCombat 
    {
        void HandleAttack(bool isAttackDown);
        void HandleReload();
        void ChangeWeapon(IWeapon weapon);
        void UpdateTarget();
    }
}
