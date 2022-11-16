using UnityEngine;

namespace Egam202
{
    public class MicrogamePauseMenu : MonoBehaviour
    {        
        // Visual information
        [SerializeField] private GameObject _enableHandle = null;

        public void SetVisible(bool isVisible)
        {
            _enableHandle.SetActive(isVisible);

            // Restore timing - pretty hacky way to stop time for everyone's game
            float timeScale = 1f;
            if (isVisible)
            {
                timeScale = 0f;
            }
            Time.timeScale = timeScale;
        }

        public void TogglePause()
        {
            bool isVisible = _enableHandle.activeInHierarchy;
            SetVisible(!isVisible);
        }

        void Update()
        {
            // Listen for key presses
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RestartOnEasy();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                RestartOnMedium();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                RestartOnHard();
            }
        }

        public void RestartOnEasy()
        {
            RestartOnDifficulty(MicrogameInstance.Difficulty.Easy);
        }

        public void RestartOnMedium()
        {
            RestartOnDifficulty(MicrogameInstance.Difficulty.Medium);
        }

        public void RestartOnHard()
        {
            RestartOnDifficulty(MicrogameInstance.Difficulty.Hard);
        }

        public void RestartOnDifficulty(MicrogameInstance.Difficulty difficulty)
        {
            // We need to restart the scene with this information baked in?...
            MicrogameInstance.PrepSceneDifficulty(difficulty);

            // Restart the scene
            MicrogameInstance microgameInstance = GameObject.FindObjectOfType<MicrogameInstance>();
            microgameInstance.Restart();
        }
    }
}
