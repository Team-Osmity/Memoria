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

        [SerializeField] private bool verboseLog = true;

        public bool isBusy { get; private set; }

        readonly Stack<string> overlayStack = new Stack<string>();

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
                await PopOverlaysAsync();
            }
            finally
            {
                isBusy = false;
            }
        }

        
        /// <summary>
        /// MasterSceneが持つ 読み込み中画面 を表示する
        /// </summary>
        async Task ShowLoadingAsync()
        {
            if (!loadingOverlay) return;
            loadingOverlay.gameObject.SetActive(true);
            float t = 0f;
            float d = Mathf.Max(0f, loadingFadeDuration);
            float from = loadingOverlay.alpha;
            while (t < d)
            {
                t += Time.unscaledDeltaTime;
                loadingOverlay.alpha = Mathf.Lerp(from, 1f, t/d);
                await Task.Yield();
                loadingOverlay.alpha = 1f;
            }
        }

        /// <summary>
        /// 最上位の overlay を閉じる
        /// </summary>
        async Task PopOverlayAsync()
        {
            if (overlayStack.Count == 0) return;
            var top = overlayStack.Pop();
            var u = SceneManager.UnloadSceneAsync(top);
            while (!u.isDone)
                await Task.Yield();
            if (verboseLog)
                Debug.Log($"[SceneDirector] Unloaded overlay scene: {top}");
        }

        /// <summary>
        /// 全ての overlay を閉じる
        /// </summary>
        async Task PopOverlaysAsync()
        {
            while (overlayStack.Count > 0)
                await PopOverlayAsync();
        }
    }
}
