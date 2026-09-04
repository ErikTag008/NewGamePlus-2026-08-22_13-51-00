using DG.Tweening;
using Project.Assets._Project._Scripts.Boss;
using Project.Assets._Project._Scripts.Managers;
using Project.Assets._Project._Scripts.Player;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Assets._Project._Scripts.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Image _playerHealthbar;
        [SerializeField] private Image _bossHealthbar;

        [SerializeField] private Image _winScreen, _loseScreen;
        [SerializeField] private Button _newGamePlusButton, _winScreenMainMenuButton, _loseScreenMainMenuButton, _retryButton;
         
        [SerializeField] private List<Image> _weaponSlots;
        [SerializeField] private float _activeWeaponCrossFadeDuration = 0.1f;
        [SerializeField] private float _inactiveWeaponCrossFadeAlpha = 0.25f;
        [SerializeField, Range(0f, 1f)] private float _inactiveWeaponCrossFadeDuration = 0.25f;
        [SerializeField] private float _healthbarChangeDuration = 0.4f;
        [SerializeField] private Ease _healthbarTweenEase = Ease.OutCubic;

        [Inject] private readonly IUIManager _uiManager;
        [Inject] private readonly PlayerHealth _playerHealth;
        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly BossHealth _bossHealth;

        private Tween _playerHealthbarTween;
        private Tween _bossHealthbarTween;

        private void Awake()
        {
            _playerHealth.OnDamageTaken += HandlePlayerHealthbar;
            _playerController.OnWeaponSwitched += HandleWeaponSwitch;
            _bossHealth.OnDamageTaken += HandleBossHealthbar;
        }

        private void OnDestroy()
        {
            _playerHealth.OnDamageTaken -= HandlePlayerHealthbar;
            _playerController.OnWeaponSwitched -= HandleWeaponSwitch;
            _bossHealth.OnDamageTaken -= HandleBossHealthbar;

        }

        public void ShowWinScreen()
        {
            _winScreen.gameObject.SetActive(true);
            _newGamePlusButton?.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
            _winScreenMainMenuButton.onClick.AddListener(() => SceneManager.LoadScene(0));
            _winScreen.rectTransform.DOScale(1f, 1f).From(0f).SetEase(Ease.OutSine);
        }

        public void ShowLoseScreen()
        {
            _loseScreen.gameObject.SetActive(true);
            _retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
            _loseScreenMainMenuButton.onClick.AddListener(() => SceneManager.LoadScene(0));
            _loseScreen.rectTransform.DOScale(1f, 1f).From(0f).SetEase(Ease.OutSine);
        }

        private void HandleWeaponSwitch(int slot)
        {
            ToggleActiveSlot(slot);
        }

        private void ToggleActiveSlot(int slot)
        {
            foreach(var (item, index) in _weaponSlots.Select((item, index) => (item, index)))
            {
                if (index == slot)
                {
                    item.CrossFadeAlpha(1f, _activeWeaponCrossFadeDuration, true);
                    continue;
                }
                item.CrossFadeAlpha(_inactiveWeaponCrossFadeAlpha, _inactiveWeaponCrossFadeDuration, true);
            }
        }

        private void HandlePlayerHealthbar()
        {
            _playerHealthbarTween?.Kill();
            _playerHealthbarTween = DOVirtual.Float(_playerHealthbar.fillAmount, EUtils.Math.Remap(0f, _playerHealth.MaxHealth, 0f, 1f, _playerHealth.CurrentHealth), _healthbarChangeDuration,
                v => _playerHealthbar.fillAmount = v)
                .SetEase(_healthbarTweenEase);
        }

        private void HandleBossHealthbar()
        {
            _bossHealthbarTween?.Kill();
            _bossHealthbarTween = DOVirtual.Float(_bossHealthbar.fillAmount, EUtils.Math.Remap(0f, _bossHealth.MaxHealth, 0f, 1f, _bossHealth.CurrentHealth), _healthbarChangeDuration,
                v => _bossHealthbar.fillAmount = v)
                .SetEase(_healthbarTweenEase);
        }
    }
}
