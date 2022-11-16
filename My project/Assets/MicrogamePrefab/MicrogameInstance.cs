using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Egam202
{
    public class MicrogameInstance : MonoBehaviour
    {
        // Microgame information
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        [System.Serializable] 
        public class MicrogameSetting
        {
            public Difficulty difficulty;
            public float duration = 10f;
        }

        [Header("Difficulty Info")]
        [SerializeField] private MicrogameSetting[] _settings = null;

        // Time over behavior
        public enum TimeStartType
        {
            Immediate,
            WaitForPrompt                       
        }

        public enum TimeOverType
        {
            Lose,
            Win,
            Notify
        }

        [Header("Time Info")]
        [SerializeField] private TimeStartType _timeStartType = TimeStartType.Immediate;
        [SerializeField] private TimeOverType _timeOverType = TimeOverType.Lose;        

        // Prompt information
        public enum PromptType
        {
            Default,
            StayOnTop,
            StayOnBottom
        }

        [Header("Prompt Info")]
        [SerializeField] private string _microgamePrompt = "Command!";
        [SerializeField] private float _promptDuration = 1f;
        [SerializeField] private PromptType _promptType = PromptType.Default;

        // Timer information
        private float _timer = 10f;
        public float timeLeft 
        {
            get { return _timer; }
        }

        private float _originalTimer = 10f;
        public float timeTotal 
        {
            get { return _originalTimer; }
        }

        public bool isTimerActive
        {
            get { return _timer >= 0; }
        }

        public bool isPromptVisible 
        {
            get 
            { 
                bool isVisible = (timeTotal - timeLeft) <= _promptDuration;
                switch (_timeStartType)
                {
                    case TimeStartType.WaitForPrompt:
                        isVisible = timeLeft > timeTotal;
                        break;
                }
                return isVisible;
            }                
        }

        // Debug information
        public enum DebugType
        {
            RestartOnTimeout,
            DoNothing,            
        }

        [Header("Debug Info")]
        public Difficulty _DEBUG_Difficulty = Difficulty.Easy;
        public DebugType _DEBUG_Type = DebugType.DoNothing;        
        private bool _isDebugFlow = false;
        private float _debugEndDelay = 1f;
        
        // Exit flow
        private Coroutine _exitRoutine = null;
        private bool _hasMicrogameStarted = false;

        public delegate void MicrogameEndHandler(bool isWin);
        public event MicrogameEndHandler OnMicrogameEnded = delegate {};

        private MicrogameCanvas _canvas = null;
        private Difficulty _lastUsedDifficulty;

        void Awake()
        {
            _canvas = GameObject.FindObjectOfType<MicrogameCanvas>();
        }

        void Start()
        {
            Difficulty difficulty = _DEBUG_Difficulty;

            // Did we find an in-scene object?  Take the difficulty from them instead
            // Get ALL instances so there's no stragglers
            MicrogameStateSaver[] stateSavers = GameObject.FindObjectsOfType<MicrogameStateSaver>();
            for (int i = 0; i < stateSavers.Length; i++)
            {
                difficulty = stateSavers[i].difficulty;
                GameObject.Destroy(stateSavers[i].gameObject);
            }

            // Broadcast a message the game has started?
            StartMicrogame(difficulty, true);
        }

        public static void PrepSceneDifficulty(Difficulty difficulty)
        {
            GameObject stateGameObject = new GameObject();
            MicrogameStateSaver stateScript = stateGameObject.AddComponent<MicrogameStateSaver>();
            stateScript.difficulty = difficulty;
        }

        public void StartMicrogame(Difficulty difficulty, bool isDebug = false)
        {
            // Refuse to restart
            if (!_hasMicrogameStarted)
            {
                _hasMicrogameStarted = true;            
                _isDebugFlow = isDebug;

                // Do we want to set the debug difficulty to this just to make it clear in editor we've been switched?
                _lastUsedDifficulty = difficulty;
                _DEBUG_Difficulty = _lastUsedDifficulty;

                // Find the matching settings
                MicrogameSetting setting = null;
                for (int i = 0; i < _settings.Length; i++)
                {
                    if (difficulty == _settings[i].difficulty)
                    {
                        setting = _settings[i];
                        break;
                    }
                }

                _originalTimer = 10f;
                if (setting != null)
                {
                    _originalTimer = setting.duration;
                }

                // If the timer is 0 we have a problem
                if (_originalTimer <= 0)
                {
                    Debug.LogError(
                        "MicrogameInstance.cs > Your game duration 0 or less! " + 
                        "Use the difficulty settings on MicrogameInstance to change"
                    );
                }
                else
                {            
                    // Start the timer
                    _timer = _originalTimer;

                    // Buffer time for the prompt?
                    switch (_timeStartType)
                    {
                        case TimeStartType.WaitForPrompt:
                            _timer += _promptDuration;
                            break;
                    }

                    // Broadcast a message that we're ready to go
                    MicrogameListener[] listeners = GameObject.FindObjectsOfType<MicrogameListener>();
                    
                    for (int i = 0; i < listeners.Length; i++)
                    {
                        // This gives game the option to override the prompt information
                        listeners[i].OnMicrogameAwake(difficulty);
                    }
                    
                    for (int i = 0; i < listeners.Length; i++)
                    {
                        listeners[i].OnMicrogameStart(_originalTimer, difficulty, _microgamePrompt, _promptType);
                    }
                }
            }
        }

        void Update()
        {
            // Decrement the timer
            if (isTimerActive)
            {
                _timer -= Time.deltaTime;

                // What should we do if the timer runs out?
                if (!isTimerActive)
                {
                    switch (_timeOverType)
                    {
                        case TimeOverType.Lose:
                            OnGameLose(true);
                            break;

                        case TimeOverType.Win:
                            OnGameWin(true);
                            break;

                        case TimeOverType.Notify:
                            // Ask someone else to report?
                            OnGameTimerOut();
                            break;
                    }
                }
            }
        }

        public void OverridePrompt(string newPromptString)
        {
            _microgamePrompt = newPromptString;
        }

        public void SetTimeOverType(TimeOverType newTimeOverType)
        {
            _timeOverType = newTimeOverType;
        }

        private void OnGameTimerOut()
        {
            MicrogameListener[] listeners = GameObject.FindObjectsOfType<MicrogameListener>();
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].OnMicrogameTimeOut(this);
            }
        }

        public void OnGameLose(bool isInstantExit = false)
        {
            // Dock a life, move back to the main flow
            ExitMicrogame(false, isInstantExit);
        }

        public void OnGameWin(bool isInstantExit = false)
        {
            // Move back to the main flow
            ExitMicrogame(true, isInstantExit);
        }

        public void AddTime(float secondsToAdd)
        {
            // We can't add more than the original time
            _timer = Mathf.Min(_originalTimer, _timer + secondsToAdd);
        }

        public void ResetTime()
        {
            // Go back to the original time
            _timer = _originalTimer;
        }

        private void ExitMicrogame(bool isWin, bool isInstantExit)
        {
            // Only exit once
            if (_exitRoutine == null)
            {
                _exitRoutine = StartCoroutine(ExecuteExitMicrogame(isWin, isInstantExit));
            }
        }

        private IEnumerator ExecuteExitMicrogame(bool isWin, bool isInstantExit)
        {
            // Show the debug result
            string resultString = "LOSE";
            if (isWin)
            {
                resultString = "WIN";
            }
            Debug.Log("MicrogameInstance.cs > Microgame " + resultString);

            // Play the canvas effects
            _canvas.ShowResult(isWin);
            yield return null;

            // Wait for the microgame to finish?
            if (!isInstantExit)
            {
                // Wait for the game to end
                while (isTimerActive)
                {
                    yield return null;
                }
            }

            // Result element needs to be done too
            while (_canvas.IsResultPlaying())
            {
                yield return null;
            }

            // Transition out
            OnMicrogameEnded(isWin);

            // Now we're ready to exit back to the main flow
            _exitRoutine = null;

            // Debug flow?  Restart the scene so we can play again...)
            if (_isDebugFlow)
            {
                yield return new WaitForSeconds(_debugEndDelay);

                switch (_DEBUG_Type)
                {
                    case DebugType.RestartOnTimeout:
                        // Maintain teh current difficulty
                        MicrogameInstance.PrepSceneDifficulty(_lastUsedDifficulty);
                        Restart();
                        break;
                }
            }
        }     

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }   
    }
}