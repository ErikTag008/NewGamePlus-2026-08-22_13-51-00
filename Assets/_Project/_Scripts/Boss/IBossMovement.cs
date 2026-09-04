using System;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Boss
{
    public interface IBossMovement
    {
        void MoveTo(Vector3 position);
        void MoveToObject(Transform transform, float updateDelay = 0.2f);
        void JumpTo(Vector3 position);
        void StopMovement();
        event Action OnJumpAttackFinished;
    }
}