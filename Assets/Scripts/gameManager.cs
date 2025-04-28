using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class gameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject vMenu;
    [SerializeField] private GameObject gOMenu;
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private GameObject otherText;

    private bool isPaused = false;
    public int _enemiesDone = 0;
    
    public bool _isDead;

    private void Start()
    {
        otherText.SetActive(true);
        counter.gameObject.SetActive(true);
        Time.timeScale = 1.0f;
        _enemiesDone = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (_enemiesDone >= 10)
        {
            Victory();
        }

        if (_isDead == true)
        {
            GameOver();
        }

        counter.text = _enemiesDone.ToString();
    }


    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void GameOver ()
    {
        gOMenu.SetActive(true);
        Time.timeScale = 0f;
        otherText.SetActive(false);
        counter.gameObject.SetActive(false);

    }

    public void Victory()
    {
        vMenu.SetActive(true);
        Time.timeScale = 0f;
        otherText.SetActive(false);
        counter.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f; // Por si acaso seguimos en pausa
        SceneManager.LoadScene("MainMenu"); // Asegúrate que el nombre es el correcto
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}