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
        [SerializeField] private SceneCatalog catalog;
        [SerializeField] private CanvasGroup loadingOverlay;
        private float loadingFadeDuration = 0.25f;

        [SerializeField] private bool verboseLog = true;

        public bool isBusy { get; private set; }

        readonly Stack<string> overlayStack = new Stack<string>();

        public string MasterSceneName { get; private set; }
        public string CurrentContentName { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            MasterSceneName = gameObject.scene.name;
            if (loadingOverlay)
            {
                loadingOverlay.gameObject.SetActive(false);
                loadingOverlay.alpha = 0f;
            }
        }

        /// <summary>
        /// シーンを切替える
        /// </summary>
        public async Task SwitchSceneAsync(Scenes.ContentScene nextScene, bool force = true)
        {
            if (isBusy && !force) return;
            isBusy = true;

            try
            {
                var nextName = catalog.Resolve(nextScene);
                if (string.IsNullOrEmpty(nextName))
                {
                    Debug.LogError($"[SceneDirector] Scene not found for content key: {nextScene}");
                    return;
                }
                if (!force && nextName == CurrentContentName)
                {
                    if (verboseLog)
                        Debug.Log($"[SceneDirector] Already in content scene: {nextName}");
                    return;
                }

                await ShowLoadingAsync();
                await PopOverlaysAsync();

                if (!string.IsNullOrEmpty(CurrentContentName))
                {
                    var u = SceneManager.UnloadSceneAsync(CurrentContentName);
                    while (!u.isDone)
                        await Task.Yield();
                }

                var l = SceneManager.LoadSceneAsync(nextName, LoadSceneMode.Additive);
                while (!l.isDone)
                    await Task.Yield();

                var sc = SceneManager.GetSceneByName(nextName);
                SceneManager.SetActiveScene(sc);
                CurrentContentName = nextName;
                if (verboseLog)
                    Debug.Log($"[SceneDirector] Switched to content scene: {nextName}");
            }
            finally
            {
                await HideLoadingAsync();
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

        /// <summary>
        /// MasterSceneが持つ 読み込み中画面 を非表示にする
        /// </summary>
        async Task HideLoadingAsync()
        {
            if (!loadingOverlay) return;
            float t = 0f;
            float d = Mathf.Max(0f, loadingFadeDuration);
            float from = loadingOverlay.alpha;
            while (t < d)
            {
                t += Time.unscaledDeltaTime;
                loadingOverlay.alpha = Mathf.Lerp(from, 0f, t/d);
                await Task.Yield();
            }
            loadingOverlay.alpha = 0f;
            loadingOverlay.gameObject.SetActive(false);
        }
    }
}
