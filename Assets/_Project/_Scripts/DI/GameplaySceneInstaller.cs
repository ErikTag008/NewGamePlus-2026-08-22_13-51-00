using KBCore.Refs;
using Project.Assets._Project._Scripts.Boss;
using Project.Assets._Project._Scripts.CameraUtils;
using Project.Assets._Project._Scripts.Input;
using Project.Assets._Project._Scripts.Managers;
using Project.Assets._Project._Scripts.Player;
using Project.Assets._Project._Scripts.UI;
using Project.Assets._Project._Scripts.Weapons;
using Reflex.Core;
using System;
using UnityEngine;

namespace Project.Assets._Project._Scripts.DI
{
    public class GameplaySceneInstaller : ValidatedMonoBehaviour, IInstaller
    {
        [SerializeField, Scene] private Camera _gameplayCamera;
        [SerializeField, Scene] private FPCameraInstaller _fpcameraInstaller;
        [SerializeField, Scene] private ProjectileParent _projectileParent;
        [SerializeField, Scene] private PlayerHealth _playerHealth;
        [SerializeField, Scene] private PlayerController _playerController;
        [SerializeField, Scene] private BossHealth _bossHealth;
        [SerializeField, Scene] private GameUI _gameUI;
        [SerializeField, Scene] private InputReader _inputReader;
        [SerializeField] private BossStats _bossStats;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_gameplayCamera);
            builder.RegisterValue(_fpcameraInstaller);
            builder.RegisterValue(_projectileParent);
            builder.RegisterValue(_playerHealth);
            builder.RegisterValue(_playerController);
            builder.RegisterValue(_bossStats);
            builder.RegisterValue(_bossHealth);
            builder.RegisterValue(_gameUI);
            builder.RegisterValue(_inputReader);
            builder.RegisterFactory(s => new StateMachine.StateMachine(), new[] { typeof(StateMachine.StateMachine) }, Reflex.Enums.Lifetime.Transient, Reflex.Enums.Resolution.Lazy);
        }
    }

}


