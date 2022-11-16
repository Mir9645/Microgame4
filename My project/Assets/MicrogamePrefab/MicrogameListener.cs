using UnityEngine;

namespace Egam202
{
    public class MicrogameListener : MonoBehaviour
    {    
        public void OnMicrogameAwake(MicrogameInstance.Difficulty difficulty)
        {
            // Basic information
            gameObject.SendMessage("MicrogameAwake", SendMessageOptions.DontRequireReceiver);
            
            // Difficulty information
            gameObject.SendMessage("MicrogameAwakeDifficulty", difficulty, SendMessageOptions.DontRequireReceiver);     
            
            switch (difficulty)
            {
                case MicrogameInstance.Difficulty.Easy:
                    gameObject.SendMessage("MicrogameAwakeEasy", SendMessageOptions.DontRequireReceiver);
                    break;
                case MicrogameInstance.Difficulty.Medium:
                    gameObject.SendMessage("MicrogameAwakeMedium", SendMessageOptions.DontRequireReceiver);
                    break;
                case MicrogameInstance.Difficulty.Hard:
                    gameObject.SendMessage("MicrogameAwakeHard", SendMessageOptions.DontRequireReceiver);
                    break;
            }   
        }

        public void OnMicrogameStart(float duration, MicrogameInstance.Difficulty difficulty,
            string prompt, MicrogameInstance.PromptType promptType)
        {
            // Basic information
            gameObject.SendMessage("MicrogameStart", SendMessageOptions.DontRequireReceiver);
            
            // Timer information
            gameObject.SendMessage("MicrogameStartDuration", duration, SendMessageOptions.DontRequireReceiver);

            // Prompt information
            gameObject.SendMessage("MicrogameStartPrompt", prompt, SendMessageOptions.DontRequireReceiver);
            gameObject.SendMessage("MicrogameStartPromptType", promptType, SendMessageOptions.DontRequireReceiver);
            
            // Difficulty information
            gameObject.SendMessage("MicrogameStartDifficulty", difficulty, SendMessageOptions.DontRequireReceiver);

            switch (difficulty)
            {
                case MicrogameInstance.Difficulty.Easy:
                    gameObject.SendMessage("MicrogameStartEasy", SendMessageOptions.DontRequireReceiver);
                    break;
                case MicrogameInstance.Difficulty.Medium:
                    gameObject.SendMessage("MicrogameStartMedium", SendMessageOptions.DontRequireReceiver);
                    break;
                case MicrogameInstance.Difficulty.Hard:
                    gameObject.SendMessage("MicrogameStartHard", SendMessageOptions.DontRequireReceiver);
                    break;
            }
        }

        public void OnMicrogameTimeOut(MicrogameInstance instance)
        {
            // Basic information
            gameObject.SendMessage("MicrogameTimeOut", instance, SendMessageOptions.DontRequireReceiver);
        }
    }
}