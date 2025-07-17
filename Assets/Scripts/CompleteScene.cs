using UnityEngine;

/* *** SUMMARY *** 

*/

public class CompleteScene : MonoBehaviour
{

    public GameObject sceneLoader;


    public void OnButtonClicked()
    {
        Debug.Log("Changing scenes");
        sceneLoader.GetComponent<SceneLoader>().LoadNextLevel();
    }


}
