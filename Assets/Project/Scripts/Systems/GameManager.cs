using UnityEngine;
using UnityEngine.SceneManagement;
using Memoria.Constants;

namespace Memoria.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public SceneDirector SceneDirector { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            SceneDirector = GetComponentInChildren<SceneDirector>();
        }
    }
}