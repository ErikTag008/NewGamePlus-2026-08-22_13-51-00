using Alchemy.Inspector;
using Project.Assets._Project._Scripts.Managers;
using Project.Assets._Project._Scripts.Player;
using Reflex.Core;
using System;
using UnityEngine;

namespace Project.Assets._Project._Scripts.DI
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField, AssetsOnly] private PlayerStats _playerStats;
        [SerializeField, AssetsOnly] private GameScenes _gameScenes;
        public void InstallBindings(ContainerBuilder builder)
        {

            // Services that are plain C# classes (POCOs) - RegisterType is correct here
            builder.RegisterType(typeof(UIManager), new Type[] { typeof(IUIManager) }, Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Eager);

            // Services that are MonoBehaviours/NetworkBehaviours - Use Factory to find the instance in the scene
            builder.RegisterFactory(p => _playerStats, new Type[] { typeof(PlayerStats), typeof(IMovementStats), typeof(ICameraStats), typeof(IHealthStats), typeof(ICombatStats) },Reflex.Enums.Lifetime.Singleton, Reflex.Enums.Resolution.Eager);
            builder.RegisterValue(_gameScenes);
        }
    }
}
