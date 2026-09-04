using DG.Tweening;
using System;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Player
{

    public class PlayerMovement : IPlayerMovement
    {
        private readonly Rigidbody _rb;
        private IMovementStats _stats;
        private readonly Transform _groundCheck;
        private Camera _camera;
        private Transform _model;
        private Transform _cameraFollower;
        private float _lastGroundedTime = -Mathf.Infinity;
        private float _lastJumpPressedTime = -Mathf.Infinity;
        private Vector3 _lastGroundNormal = Vector3.up;
        private bool _movementEnabled = true;

        public PlayerMovement(Rigidbody rb, IMovementStats stats, Transform groundCheck, Camera camera, Transform model, Transform cameraFollower)
        {
            _rb = rb;
            _stats = stats;
            _groundCheck = groundCheck;
            _camera = camera;
            _model = model;
            _cameraFollower = cameraFollower;
        }

        public void ToggleMovement(bool isEnabled)
        {
            _movementEnabled = isEnabled;
            if (!_movementEnabled)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }


        public void ChangeCamera(Camera camera)
        {
            _camera = camera;
        }
        public void ChangeStats(IMovementStats stats)
        {
            if (stats == null) return;
            _stats = stats;
        }
        public void ChangeModel(Transform model)
        {
            if (model == null) return;
            _model = model;
        }
        public void ChangeCameraFollower(Transform cameraFollower)
        {
            if (cameraFollower == null) return;
            _cameraFollower = cameraFollower;
        }
        public void HandleJump(bool isButtonDown = true)
        {
            if (!_movementEnabled) return;

            if (isButtonDown)
            {

                _lastJumpPressedTime = Time.time;
                if ((IsGrounded() || Time.time - _lastGroundedTime <= _stats.CoyoteTime) && GetVelocityAlongGroundNormal() < _stats.GroundCheckMaxYVelocity)
                {
                    PerformJump();
                    _lastJumpPressedTime = -Mathf.Infinity;
                    return;
                }
                //if (_canDoubleJump && !_doubleJumpUsed)
                //{
                //    PerformJump(isDoubleJump: true);
                //    _doubleJumpUsed = true;
                //    _lastJumpPressedTime = -Mathf.Infinity;
                //}
            }
            else
            {
                if(_rb.linearVelocity.y > 0.1f)
                {
                    SlowDownVerticalVelocity();
                }
                _lastJumpPressedTime = -Mathf.Infinity;
            }

            void SlowDownVerticalVelocity()
            {
                var velocityVector = _rb.linearVelocity;
                velocityVector.y *= _stats.JumpButtonUpVerticalVelocityMultiplier;
                _rb.linearVelocity = velocityVector;
            }
        }

        public bool IsGrounded()
        {
            if (_groundCheck == null || _stats == null) return false;

            if (Physics.Raycast(_groundCheck.position, Vector3.down, out RaycastHit hitInfo, _stats.GroundCheckRadius, _stats.GroundCheckLayers))
            {
                _lastGroundNormal = hitInfo.normal;
                if (_model == null) return false;
                float groundAngle = Vector3.Angle(_lastGroundNormal, _model.up);
                return groundAngle <= _stats.MaxGroundAngle;
            }
            else
            {
                _lastGroundNormal = Vector3.up;
                return false;
            }
        }

        private float GetVelocityAlongGroundNormal()
        {
            return Vector3.Dot(_rb.linearVelocity, _lastGroundNormal);
        }

        public void HandleFixedMovement(Vector2 moveDirection)
        {
            if(_stats == null || !_movementEnabled) return;
            bool grounded = IsGrounded();
            HandleHorizontalMovement(moveDirection, grounded);
            if (grounded)
            {
                _lastGroundedTime = Time.time;
                //_doubleJumpUsed = false;
                _rb.linearDamping = _stats.HorizontalDamping;
            }
            else
            {
                _rb.linearDamping = _stats.VerticalDamping;
                if (_rb.linearVelocity.y < 0f)
                    _rb.AddForce(Physics.gravity, ForceMode.Acceleration);
            }

            TryConsumeBufferedJump(grounded);

        }

        public void HandleRotation()
        {
            HandleModelRotation();
            HandleCameraFollowerRotation();
        }

        private void HandleCameraFollowerRotation()
        {
            if (!_cameraFollower || !_cameraFollower.gameObject.activeInHierarchy || _stats == null) return;
            Quaternion newRotation = _camera.transform.rotation;
            _cameraFollower.rotation = newRotation;
        }

        private void HandleModelRotation()
        {
            if (!_camera || !_camera.gameObject.activeInHierarchy || _stats == null || !_movementEnabled) return;

            Vector3 cameraForward = _camera.transform.forward;
            cameraForward.y = 0f;
            if (cameraForward == Vector3.zero) return;

            Quaternion newRotation = Quaternion.LookRotation(cameraForward);
            _model.rotation = newRotation;
        }

        private void HandleHorizontalMovement(Vector2 moveDirection, bool grounded)
        {
            if (!_camera || !_camera.gameObject.activeInHierarchy || _stats == null || !_movementEnabled) return;
            Vector3 moveVector = _camera.transform.forward * moveDirection.y +
                                 _camera.transform.right * moveDirection.x;
            float magnitude = moveDirection.magnitude;

            moveVector.y = 0f;
            if (moveVector.sqrMagnitude > 0f) moveVector.Normalize();

            // project desired movement onto ground plane when grounded
            Vector3 moveDir = grounded
                            ? Vector3.ProjectOnPlane(moveVector, _lastGroundNormal)
                            : moveVector;


            if (moveDir.sqrMagnitude > 0f) moveDir.Normalize();

            Vector3 velocity = _rb.linearVelocity;
            Vector3 planarVelocity = grounded
                                    ? Vector3.ProjectOnPlane(velocity, _lastGroundNormal)
                                    : new Vector3(velocity.x, 0f, velocity.z);

            float maxSpeed = _stats.MaxMoveSpeed;
            float accel = grounded ? _stats.GroundAcceleration : _stats.AirAcceleration;

            Vector3 targetVelocity = magnitude * maxSpeed * moveVector;
            Vector3 velocityDelta = targetVelocity - planarVelocity;

            velocityDelta = Vector3.ClampMagnitude(velocityDelta, accel * Time.fixedDeltaTime);
            Vector3 applyDelta = grounded
                                ? Vector3.ProjectOnPlane(velocityDelta, _lastGroundNormal)
                                : velocityDelta;
            _rb.AddForce(applyDelta, ForceMode.VelocityChange);
        }

        private void TryConsumeBufferedJump(bool grounded)
        {
            if (!_movementEnabled) return;

            if (Time.time - _lastJumpPressedTime > _stats.JumpBuffer) return;

            if ((grounded || Time.time - _lastGroundedTime <= _stats.CoyoteTime) && GetVelocityAlongGroundNormal() < _stats.GroundCheckMaxYVelocity)
            {
                PerformJump();
                _lastJumpPressedTime = -Mathf.Infinity;
                return;
            }

            //if (!_doubleJumpUsed && _canDoubleJump)
            //{

            //    PerformJump(isDoubleJump: true);
            //    _doubleJumpUsed = true;
            //    _lastJumpPressedTime = -Mathf.Infinity;
            //}
        }

        private void PerformJump(bool isDoubleJump = false)
        {
            if (!_movementEnabled) return;
            // remove any velocity component that points into the ground normal
            float intoNormal = Vector3.Dot(_rb.linearVelocity, _lastGroundNormal);
            if (intoNormal < 0f)
            {
                _rb.linearVelocity -= _lastGroundNormal * intoNormal;
            }

            // choose jump direction: along ground normal when grounded, otherwise world up
            Vector3 jumpDir = IsGrounded() ? _lastGroundNormal : Vector3.up;

            _rb.AddForce(jumpDir * _stats.JumpForce, ForceMode.VelocityChange);
        }

        public void DrawGizmos()
        {
            if (!_movementEnabled) return;

            if (_groundCheck != null && _stats != null)
            {
                Gizmos.color = IsGrounded() ? Color.green : Color.red;
                Gizmos.DrawWireSphere(_groundCheck.position, _stats.GroundCheckRadius);
            }
        }

        
    }
}
