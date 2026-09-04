using Assets._Project._Scripts.SceneReference;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Managers
{
    [CreateAssetMenu(fileName = "GameScenes", menuName = "ScriptableObjects/GameScenes", order = 1)]
    public class GameScenes : ScriptableObject
    {
        [field: SerializeField, SceneReference] public string NewGameScene { get; private set; }
        [field: SerializeField, SceneReference] public string NewGamePlus1Scene { get; private set; }
        [field: SerializeField, SceneReference] public string NewGamePlus2Scene { get; private set; }
        [field: SerializeField, SceneReference] public string NewGamePlus3Scene { get; private set; }
        [field: SerializeField, SceneReference] public string NewGamePlus4Scene { get; private set; }
        [field: SerializeField, SceneReference] public string MainMenuScene { get; private set; }

        public string NewGameSceneName => System.IO.Path.GetFileNameWithoutExtension(NewGameScene);
        public string NewGamePlus1SceneName => System.IO.Path.GetFileNameWithoutExtension(NewGamePlus1Scene);
        public string NewGamePlus2SceneName => System.IO.Path.GetFileNameWithoutExtension(NewGamePlus2Scene);
        public string NewGamePlus3SceneName => System.IO.Path.GetFileNameWithoutExtension(NewGamePlus3Scene);
        public string NewGamePlus4SceneName => System.IO.Path.GetFileNameWithoutExtension(NewGamePlus4Scene);
        public string MainMenuSceneName => System.IO.Path.GetFileNameWithoutExtension(MainMenuScene);
    }
}
