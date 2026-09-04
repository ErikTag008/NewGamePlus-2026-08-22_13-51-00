using UnityEngine;

namespace Project.Assets._Project._Scripts
{
    public class Persistent : MonoBehaviour
    {
        private static Persistent _instance;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
