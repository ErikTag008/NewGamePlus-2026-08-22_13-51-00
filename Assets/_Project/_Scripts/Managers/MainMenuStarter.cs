using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using Project.Assets._Project._Scripts.UI;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Assets._Project._Scripts.Managers
{
    public class MainMenuStarter : MonoBehaviour
    {
        [Inject] private readonly MainMenuUI _serverStarterUI;
        [Inject] private readonly IUIManager _uiManager;
        [Inject] private readonly GameScenes _gameScenes;
        private void Start()
        {
        }

        
        

        private void OnDestroy()
        {
        }
    }
}
