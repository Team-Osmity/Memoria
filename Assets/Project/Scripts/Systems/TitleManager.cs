using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Memoria.Constants;

namespace Memoria.Systems
{
    public class TitleManager : MonoBehaviour
    {
        async public void onClickStartButton()
        {
            await GameManager.Instance.SceneDirector.SwitchSceneAsync(Scenes.ContentScene.Game, false);
        }
    }
}
