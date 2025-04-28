using UnityEngine.SceneManagement;
using UnityEngine;
public class ToGameplay : MonoBehaviour
{
    public string gameplaySceneName; 

    public void ChangeToGameplay()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }
}