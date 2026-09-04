using UnityEngine;

namespace Project.Assets._Project._Scripts.Weapons
{
    public interface IWeapon
    {
        void Attack();
        void StopAttack();
        void UpdateTarget(Vector3 target);
        void Reload();
    }

    public class WeaponBase : MonoBehaviour, IWeapon
    {
        public virtual void Attack()
        {
            Debug.Log("Attack");
        }
        public virtual void StopAttack()
        {
            Debug.Log("StopAttack");
        }
        public virtual void UpdateTarget(Vector3 target)
        {
            Debug.Log("UpdateTarget: " + target);
        }
        public virtual void Reload()
        {
            Debug.Log("Reload");
        }
    }

}