using UnityEngine;
using UnityEngine.UI;

namespace Egam202
{
    public class MicrogameCanvas : MonoBehaviour
    {
        // Timer information
        [SerializeField] private Slider _slider = null;

        // Transition out information
        [SerializeField] private Image _transitionImageUi = null;
        [SerializeField] private MicrogameInterper _transitionInterper = null;
        
        // Menu information
        [SerializeField] private MicrogamePauseMenu _pauseMenu = null;

        // Results
        [SerializeField] private MicrogameResultElement _winElement = null;
        [SerializeField] private MicrogameResultElement _loseElement = null;

        // Instance iinformation
        private MicrogameInstance _instance = null;

        // Start is called before the first frame update
        void Start()
        {
            // Find the manager
            _instance = GameObject.FindObjectOfType<MicrogameInstance>();
            _instance.OnMicrogameEnded += OnGameEnded;

            // Hide menus / transitions
            _pauseMenu.SetVisible(false);
            _transitionInterper.SetGoal(0, true);
        }
        
        // Update is called once per frame
        void Update()
        {
            float timeLeft = _instance.timeLeft;
            float timeTotal = _instance.timeTotal;
            float timeElapsed = timeTotal - timeLeft;

            // Update the timer
            float timerInterp = Mathf.Clamp01(_instance.timeLeft / _instance.timeTotal);
            _slider.SetValueWithoutNotify(timerInterp);

            // Udpate the transition
            float fillAmount = _transitionInterper.value;
            _transitionImageUi.enabled = fillAmount > 0;
            _transitionImageUi.fillAmount = fillAmount;        

            // Input information
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            _pauseMenu.TogglePause();
        }

        public void ShowResult(bool isWin)
        {
            if (isWin)
            {
                _winElement.Play();
            }
            else
            {
                _loseElement.Play();
            }
        }        
        
        public bool IsResultPlaying()
        {
            bool isPlaying = _winElement.isPlaying || _loseElement.isPlaying;
            return isPlaying;
        }

        private void OnGameEnded(bool isWin)
        {
            // Transition out
            _transitionInterper.SetGoal(1);
        }
    }
}