using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
/* *** SUMMARY *** 

*/

public class SceneLoader : MonoBehaviour
{
    public Animator transition;

    public GameObject? globalVariables;

    public bool changeScene;

    void Awake() {
        gameObject.SetActive(true);
    }
    void Start()
    {
        changeScene = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (globalVariables != null)
        {
            if (globalVariables.GetComponent<AllMatched>().checkAllMatched)
            {
                changeScene = true;
            }  
        }



        if (changeScene)
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        // Play Animation

        transition.SetTrigger("Start");

        // Wait

        yield return new WaitForSeconds(1);

        // Load Scene
        SceneManager.LoadScene(levelIndex);

    }
}
