using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rayuela
{

    public class SplashScreen : MonoBehaviour
    {

        private Image _myLogo;
        private bool _loadFinish;
        private bool _endLogo;

        private void Awake()
        {
            //Cursor.lockState = CursorLockMode.Locked;
            _myLogo = GetComponent<Image>();
            _loadFinish = false;
            _endLogo = false;
            _myLogo.color = new Color(_myLogo.color.r, _myLogo.color.g, _myLogo.color.b, 0f);
        }

        void Start()
        {
          #if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
          #endif

            _loadFinish = true;
        }

        
        void Update()
        {
            if (_loadFinish && _endLogo)
            {
                SceneManager.LoadSceneAsync("Title");
            }
        }

        public void EndAnimationLogo()
        {
            _endLogo = true;
        }

    }

}
