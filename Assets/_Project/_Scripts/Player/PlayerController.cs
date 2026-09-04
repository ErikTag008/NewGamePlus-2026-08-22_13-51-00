using KBCore.Refs;
using Project.Assets._Project._Scripts.CameraUtils;
using Project.Assets._Project._Scripts.Input;
using Project.Assets._Project._Scripts.Managers;
using Project.Assets._Project._Scripts.Weapons;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Assets._Project._Scripts.Player
{
    public enum Team
    {
        None,
        Catcher,
        Runner
    }

    [RequireComponent(typeof(PlayerInput), typeof(InputReader), typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Self] private PlayerInput _playerInput;
        [SerializeField, Self] private InputReader _inputReader;
        [SerializeField, Self] private Rigidbody _rb;
        [SerializeField, Self] private PlayerHealth _health;
        [SerializeField, Self] private AudioSource _audioSource;

        [SerializeField] private Transform _model;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Transform _weaponPivot;
        [SerializeField] private Transform _cameraFollower;
        [SerializeField] private List<WeaponBase> _weapons;
        private WeaponBase _currentWeapon;

        public Transform CameraTarget => _cameraTarget;
        public InputReader InputReader => _inputReader;

        [Inject] private readonly PlayerStats _playerStats;

        [Inject] private readonly Camera _gameplayCamera;
        [Inject] private readonly FPCameraInstaller _fpCameraInstaller;

        private IPlayerMovement _movement;
        private IPlayerCombat _combat;

        public delegate void WeaponSwitchEvent(int slot);
        public event WeaponSwitchEvent OnWeaponSwitched; 


        private void Awake()
        {
            if (_playerStats && _gameplayCamera)
            {
                _movement = new PlayerMovement(_rb, _playerStats, _groundCheck, _gameplayCamera, _model, _cameraFollower);
                _combat = new PlayerCombat(_gameplayCamera.transform, _playerStats);
                
            }
            if (_fpCameraInstaller != null)
            {
                _fpCameraInstaller.BindCameraToPlayer(this);
                _fpCameraInstaller.OnCameraRotationChanged += _movement.HandleRotation;
            }
            
            _inputReader.OnJump += _movement.HandleJump;
            _inputReader.OnShoot += Shoot;
            _inputReader.WeaponSlotSwitch += OnWeaponSlotSwitched;
          
        }

        private void Start()
        {
            foreach (var weapon in _weapons)
            {
                weapon.gameObject.SetActive(false);
            }
            OnWeaponSlotSwitched(1);
            _health.OnKilled += HandleDeath;
        }

        private void OnDestroy()
        {
            _fpCameraInstaller.OnCameraRotationChanged -= _movement.HandleRotation;
            _health.OnKilled -= HandleDeath;
            _inputReader.OnJump -= _movement.HandleJump;
            _inputReader.OnShoot -= Shoot;
            _inputReader.WeaponSlotSwitch -= OnWeaponSlotSwitched;
        }

        private void OnWeaponSlotSwitched(int slot)
        {
            if (slot < 1 || slot > 3)
            {
                slot = 1;
            }
            int weaponIndex = slot - 1;
            _currentWeapon?.gameObject.SetActive(false);
            _currentWeapon = _weapons[weaponIndex];
            _currentWeapon.gameObject.SetActive(true);
            _combat?.ChangeWeapon(_currentWeapon);
            OnWeaponSwitched?.Invoke(weaponIndex);
        }

        private void HandleDeath()
        {
            _audioSource.PlayOneShot(_playerStats.DeathSFX, _playerStats.DeathSFXVolume);
            _movement.ToggleMovement(false);
        }

        private void FixedUpdate()
        {
            if (_movement == null)
            {
                Debug.LogWarning("Movement Is NULL!!!");
                return;
            }
            _movement.HandleFixedMovement(_inputReader.MoveDirection);
        }
        private void LateUpdate()
        {
            _combat?.UpdateTarget();
        }


        private void Shoot(bool isDown)
        {
            _combat?.HandleAttack(isDown);
        }

        private void OnDrawGizmos()
        {
            _movement?.DrawGizmos();
        }
    }
}
