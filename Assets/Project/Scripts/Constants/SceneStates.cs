namespace Memoria.Constants
{
    public static class SceneStates
    {
        public enum ContentScene
        {
            Title,
            Game,
            MainMenu,
            Stage01,
            Ending
        }

        public enum OverlayScene
        {
            HUD,
            PauseMenu,
            Settings
        }

        public const string LOADING_FADE_DURATION = "loadingFadeDuration";
    }
}