using UnityEngine;
using Memoria.Constants;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FetchJson());
    }

    private IEnumerator FetchJson()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(SpreadSheets.ApiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response:\n" + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
