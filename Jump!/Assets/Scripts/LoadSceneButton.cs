using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public void LoadSceneOnClick(string scene)
    {
        SceneManager.LoadScene(scene);
        AudioOutput.manager.PlayButtonTapSound();
        if (scene == "_Start" && SceneManager.GetActiveScene().name == "Gameplay")
        {
            AudioOutput.manager.PlayClip("menu music", 1, 0);
        }
    }

}
