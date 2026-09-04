using UnityEngine;

namespace Project.Assets._Project._Scripts.Player
{
    public interface IPlayerMovement 
    {
        void ToggleMovement(bool isEnabled);
        void HandleJump(bool isButtonDown = true);
        void HandleFixedMovement(Vector2 moveDirection);
        void HandleRotation();
        bool IsGrounded();
        void DrawGizmos();
        void ChangeCamera(Camera camera);
        void ChangeStats(IMovementStats stats);
        void ChangeModel(Transform model);
        void ChangeCameraFollower(Transform cameraFollower);
    }
}
