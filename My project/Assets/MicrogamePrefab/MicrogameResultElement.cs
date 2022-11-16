using System.Collections;
using UnityEngine;

namespace Egam202
{
    public class MicrogameResultElement : MonoBehaviour
    {
        // Visual information
        [SerializeField] private GameObject _enableHandle = null;

        // Animation information
        [SerializeField] private float _duration = 1f;
        
        private Coroutine _playRoutine = null;
        public bool isPlaying 
        {
            get { return _playRoutine != null; }
        }

        [SerializeField] private MicrogameShaker[] _shakers = null;

        [SerializeField] private MicrogameInterper _flashInterper = null;

        [System.Serializable]
        private class FlashData
        {
            public Transform animHandle;
            public CanvasGroup fadeGroup;

            private Vector3 _originalScale = Vector3.one;

            public void Init()
            {
                if (animHandle != null)
                {
                    _originalScale = animHandle.localScale;
                }            

                SetInterp(0f, 0f);
            }

            public void SetInterp(float scaleFactor, float alpha)
            {
                if (animHandle != null)
                {                
                    animHandle.localScale = _originalScale * scaleFactor;
                }
                
                if (fadeGroup != null)
                {
                    fadeGroup.alpha = alpha;
                }                
            }
        }

        [SerializeField] private FlashData[] _flashElements = null;

        [SerializeField] private Vector2 _flashScaleRange = new Vector2(1, 2f);
        [SerializeField] private Vector2 _flashAlphaRange = new Vector2(1, 0f);

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < _flashElements.Length; i++)
            {
                _flashElements[i].Init();
            }

            if (_flashInterper != null)
            {
                _flashInterper.SetGoal(1, true);
            }

            _enableHandle.SetActive(false);
        }

        void Update()
        {
            if (_flashInterper != null)
            {
                float scale = Mathf.Lerp(_flashScaleRange.x, _flashScaleRange.y, _flashInterper.value);
                float alpha = Mathf.Lerp(_flashAlphaRange.x, _flashAlphaRange.y, _flashInterper.value);

                for (int i = 0; i < _flashElements.Length; i++)
                {
                    _flashElements[i].SetInterp(scale, alpha);
                }
            }
        }

        public void Play()
        {
            _enableHandle.SetActive(true);

            for (int i = 0; i < _shakers.Length; i++)
            {
                _shakers[i].Shake();
            }

            if (_flashInterper != null)
            {
                _flashInterper.SetGoal(0, true);
                _flashInterper.SetGoal(1, false);
            }

            // Kickoff the timer
            StopEffect();
            _playRoutine = StartCoroutine(ExecuteTimer(_duration));
        }

        private IEnumerator ExecuteTimer(float timerDuration)
        {
            yield return new WaitForSeconds(timerDuration);
            _playRoutine = null;
        }

        public void StopEffect()
        {
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }
        }

        void OnDisable()
        {
            StopEffect();
        }
    }
}
