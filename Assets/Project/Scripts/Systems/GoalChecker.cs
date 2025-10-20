using UnityEngine;

namespace Memoria.Systems
{
    public class GoalChecker : MonoBehaviour
    {
        private bool isGaoled = false;
        private async void OnTriggerEnter(Collider other)
        {
            if (isGaoled) return;
            if (other.CompareTag("Goal"))
            {
                Debug.Log("Goal Reached!");
                isGaoled = true;
                await GameManager.Instance.SceneDirector.SwitchSceneAsync(Memoria.Constants.Scenes.ContentScene.Ending, false);
            }
        }
    }
}