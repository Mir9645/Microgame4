using System.Collections;
using TMPro;
using UnityEngine;

namespace Egam202
{
    public class MicrogamePrompt : MonoBehaviour
    {
        // Text information
        [SerializeField] private TextMeshProUGUI[] _promptTextUis = null;

        // Animation information
        private bool _isInited = false;
        private Coroutine _moveRoutine = null;
        
        [SerializeField] private MicrogameInterper _moveInterper = null;
        [SerializeField] private RectTransform _moveHandle = null;
        private Vector3 _originalPosition = Vector3.zero;

        [SerializeField] private RectTransform _topReference = null;
        [SerializeField] private RectTransform _bottomReference = null;
        
        [SerializeField] private MicrogameInterper _fadeInterper = null;
        [SerializeField] private CanvasGroup _fadeGroup = null;
        [SerializeField] private AnimationCurve _fadeCurve = null;

        // Instance info
        private MicrogameInstance _microgameInstance = null;

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (!_isInited)
            {
                _isInited = true;
                
                _originalPosition = _moveHandle.position;
                _microgameInstance = GameObject.FindObjectOfType<MicrogameInstance>();
            }
        }

        public void MicrogameStartPrompt(string promptString)
        {
            for (int i = 0; i < _promptTextUis.Length; i++)
            {
                _promptTextUis[i].text = promptString;
            }        
        }

        public void MicrogameStartPromptType(MicrogameInstance.PromptType promptType)
        {
            Init();

            // Change which animation to use
            switch (promptType)
            {
                case MicrogameInstance.PromptType.Default:
                    SlideTo(null);
                    break;

                case MicrogameInstance.PromptType.StayOnTop:
                    SlideTo(_topReference);
                    break;

                case MicrogameInstance.PromptType.StayOnBottom:
                    SlideTo(_bottomReference);
                    break;
            }
        }

        void Update()
        {
            // Update fade / pos
            float alpha = Mathf.Clamp01(_fadeInterper.value);
            alpha = _fadeCurve.Evaluate(alpha);
            _fadeGroup.alpha = alpha;

            Vector3 pos = _originalPosition;
            pos.y = _moveInterper.value;
            _moveHandle.position = pos;
        }

        private void SlideTo(RectTransform goal)
        {
            if (_moveRoutine == null)
            {
                _moveRoutine = StartCoroutine(ExecuteSlideTo(goal));
            }
        }

        private IEnumerator ExecuteSlideTo(RectTransform goal)
        {
            // Intro animation (fade in)
            _fadeInterper.SetGoal(1, true);
            _moveInterper.SetGoal(_originalPosition.y, true);

            // Stay in place
            while (_microgameInstance.isPromptVisible)
            {
                yield return null;
            }

            // Animate out
            if (null == goal)
            {
                // Fade out
                _fadeInterper.SetGoal(0);
            }
            else
            {
                // Animate to this object
                _moveInterper.SetGoal(goal.position.y);
            }

            _moveRoutine = null;
        }
    }
}
