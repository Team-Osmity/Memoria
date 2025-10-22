using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Memoria.Constants;

namespace Memoria.Systems
{
    public class EndingManager : MonoBehaviour
    {
        async public void onClickBackTitleButton()
        {
            await GameManager.Instance.SceneDirector.SwitchSceneAsync(Scenes.ContentScene.Title, false);
        }
    }
}
