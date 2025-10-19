using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Memoria.Constants;

namespace Memoria.Systems
{
    public class SceneDirector : MonoBehaviour
    {
        public static SceneDirector Instance { get; private set; }
        [SerializeField] private CanvasGroup loadingOverlay;
        private float loadingFadeDuration = 0.25f;

        public bool isBusy { get; private set; }

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async Task SwitchSceneAsync(Scenes.ContentScene nextScene, bool force = true)
        {
            if (isBusy && !force) return;
            isBusy = true;

            try
            {
                await ShowLoadingAsync();
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}
