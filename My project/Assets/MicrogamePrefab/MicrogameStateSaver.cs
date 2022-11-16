using UnityEngine;

namespace Egam202
{
    public class MicrogameStateSaver : MonoBehaviour
    {
        // Attached data
        private MicrogameInstance.Difficulty _difficulty;
        public MicrogameInstance.Difficulty difficulty
        {
            set { _difficulty = value; }
            get { return _difficulty; }
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
