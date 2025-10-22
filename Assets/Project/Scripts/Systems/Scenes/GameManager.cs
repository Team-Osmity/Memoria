using UnityEngine;
using System.Threading.Tasks;
using Memoria.Constants;

namespace Memoria.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public SceneDirector SceneDirector { get; private set; }

        [SerializeField] private GameObject creditPanel;
        private Scenes.ContentScene first = Scenes.ContentScene.Title;
        private bool showCreditsOnBoot;
        private float creditSeconds;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            SceneDirector = GetComponentInChildren<SceneDirector>();
        }

        private async void Start()
        {
            await ParameterManager.InitializeAsync();

            showCreditsOnBoot = ParameterManager.GetParam<bool>(GoogleSheets.SHOW_CREDITS_ON_BOOT, false);
            creditSeconds = ParameterManager.GetParam<float>(GoogleSheets.CREDIT_SECONDS, 1.5f);

            // クレジット表示をできるようにしておいた
            if (showCreditsOnBoot && creditPanel)
            {
                creditPanel.SetActive(true);
                await Task.Delay(Mathf.RoundToInt(creditSeconds * 1000));
                creditPanel.SetActive(false);
            }

            // TitleSceneを読み込み
            if (SceneDirector)
                await SceneDirector.SwitchSceneAsync(first, false);
            else 
                Debug.LogError("[GameManager] SceneDirector not found!");
        }
    }
}