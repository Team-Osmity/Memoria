using UnityEngine;
using System.Threading.Tasks;
using Memoria.Constants;

namespace Memoria.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public SceneDirector SceneDirector { get; private set; }

        [SerializeField] private Scenes.ContentScene first = Scenes.ContentScene.Title;
        [SerializeField] private bool showCreditsOnBoot = false;
        [SerializeField] private GameObject creditPanel;
        [SerializeField] private float creditSeconds = 1.5f;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            SceneDirector = GetComponentInChildren<SceneDirector>();
        }

        private async void Start()
        {
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