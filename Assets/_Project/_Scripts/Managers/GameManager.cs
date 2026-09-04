using DG.Tweening;
using Project.Assets._Project._Scripts.Boss;
using Project.Assets._Project._Scripts.Input;
using Project.Assets._Project._Scripts.Player;
using Project.Assets._Project._Scripts.UI;
using Reflex.Attributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Assets._Project._Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private readonly IUIManager _uiManager;
        [Inject] private readonly GameScenes _gameScenes;
        [Inject] private readonly PlayerHealth _playerHealth;
        [Inject] private readonly BossHealth _bossHealth;
        [Inject] private readonly GameUI _gameUI;
        [Inject] private readonly InputReader _inputReader;

        private void Awake()
        {
            _playerHealth.OnKilled += HandlePlayerDeath;
            _bossHealth.OnKilled += HandleBossDeath;
        }
        private void OnDestroy()
        {
            _playerHealth.OnKilled -= HandlePlayerDeath;
            _bossHealth.OnKilled -= HandleBossDeath;
            DOTween.KillAll();
        }


        private void HandlePlayerDeath()
        {
            _gameUI.ShowLoseScreen();
            _inputReader.UnlockMouse();
        }

        private void HandleBossDeath()
        {
            _gameUI.ShowWinScreen();
            _inputReader.UnlockMouse();
            int level = SceneManager.GetActiveScene().buildIndex;
            int lastUnlockedLevel = PlayerPrefs.GetInt("LastUnlockedGameLevel", 0);
            if (level > lastUnlockedLevel)
            {
                PlayerPrefs.SetInt("LastUnlockedGameLevel", level);
            }

        }


    }
}
