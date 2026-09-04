using KBCore.Refs;
using Project.Assets._Project._Scripts.Managers;
using Project.Assets._Project._Scripts.UI;
using Reflex.Core;
using UnityEngine;

namespace Project.Assets._Project._Scripts.DI
{
    public class MainMenuSceneInstaller : ValidatedMonoBehaviour, IInstaller
    {
        [SerializeField, Scene] private Camera _mainCamera;
        [SerializeField, Scene] private MainMenuUI _serverStarterUI;
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(_mainCamera);
            builder.RegisterValue(_serverStarterUI);
        }
    }

}


