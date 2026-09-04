using DG.Tweening;
using Project.Assets._Project._Scripts.Managers;
using Reflex.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Assets._Project._Scripts.UI
{
    [RequireComponent(typeof(Canvas))]
    public class MainMenuUI : MonoBehaviour
    {
        [Inject] private readonly IUIManager _uiManager;
        [SerializeField] private List<Button> _newGameButtons;
        [SerializeField] private Button _quitButton;
        private Canvas _serverStarterCanvas;
        private void Awake()
        {
            _serverStarterCanvas = GetComponent<Canvas>();
            _quitButton.onClick.AddListener(() => Application.Quit());
            int lastUnlockedGameLevel = PlayerPrefs.GetInt("LastUnlockedGameLevel", 0);
            foreach (var (button, index) in _newGameButtons.Select((button, index) => (button, index)))
            {
                if (index > lastUnlockedGameLevel)
                    button.interactable = false;
                button.onClick.AddListener(() => GoToLevel(index));
            }
        }
        private void Start()
        {
            
        }

        private void GoToLevel(int newGameButtonIndex)
        {
            SceneManager.LoadScene(newGameButtonIndex + 1);
        }


        private void OnDestroy()
        {
            _quitButton.onClick.RemoveAllListeners();
            DOTween.KillAll();
        }

        public void ToggleServerStarterUI(bool enabled)
        {
            _serverStarterCanvas.enabled = enabled;
        }
    }
}
