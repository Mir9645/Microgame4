using UnityEngine;
using System.Collections;

namespace Egam202
{
    public class MicrogameShaker : MonoBehaviour 
    {
        // Transform
        [SerializeField] private Transform _animHandle = null;
        private Vector3 _originalLocalPosition = Vector3.zero;
        private Quaternion _originalLocalRotation = Quaternion.identity;
                
        // Animation information
        [SerializeField] private bool _isLoopShake = false;
        
        private float _shakeT = -1f;
        [SerializeField] private float _shakeDuration = 1f;
        [SerializeField] private int _shakesPerDuration = 5;

        private float _shakeFactor = 1f;
        [SerializeField] private float _shakeStrength = 1f;
        [SerializeField] private Vector3 _shakeDirection = Vector2.one;
        
        [SerializeField] private AnimationCurve _strengthMaskCurve = null;
        [SerializeField] private Vector2 _strengthMaskRange = new Vector2(1f, 0f);
            
        [SerializeField] private float _rotationStrength = 10f;
        [SerializeField] private Vector3 _rotationAxis = Vector3.forward;
            
        private Coroutine _activeRoutine = null;
        public bool isAnimating
        {
            get { return _activeRoutine != null; }
        }
            
        public void Shake(float factor = 1f)
        {
            _shakeFactor = factor;

            // Kickoff routine?
            if (_activeRoutine == null &&
                gameObject.activeInHierarchy)
            {
                _activeRoutine = StartCoroutine(ShakeAnimated());
            }
            else
            {
                _shakeT = 0;
            }
        }
        
        public void StopShake()
        {
            if (_activeRoutine != null)
            {
                StopCoroutine(_activeRoutine);
                _activeRoutine = null;
                
                // Restore positioning
                RestoreTransform();
            }
        }
        
        private IEnumerator ShakeAnimated()
        {
            // Setup
            _originalLocalPosition = _animHandle.localPosition;
            _originalLocalRotation = _animHandle.localRotation;
            
            // Animate
            float shakeFrequency = 1f / _shakesPerDuration;
            
            _shakeT = 0;
            float lastInterp = 0;
            
            while (_shakeT < _shakeDuration)
            {
                float interp = Mathf.Clamp01(_shakeT / _shakeDuration);
                
                // Are we allowed to shake?                        
                int shakeIndex = Mathf.CeilToInt(lastInterp / shakeFrequency);
                float shakeThreshold = shakeIndex * shakeFrequency;
                if (lastInterp < shakeThreshold &&
                    interp >= shakeThreshold)
                {
                    // Pick a random offset
                    Vector3 randomPlusMinus = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f)
                    );

                    Vector3 randomOffset = Vector3.Scale(randomPlusMinus.normalized, _shakeDirection);   
                    randomOffset *= Random.Range(-_shakeStrength, _shakeStrength) * _shakeFactor;
                    
                    float mask = 1;
                    if (_strengthMaskCurve != null &&
                        _strengthMaskCurve.length > 0)
                    {
                        float maskInterp = _strengthMaskCurve.Evaluate(interp);
                        mask = Mathf.Lerp(_strengthMaskRange.x, _strengthMaskRange.y, maskInterp);
                    }
                    
                    randomOffset *= mask;
                    
                    _animHandle.localPosition = _originalLocalPosition + randomOffset;                
                    
                    // Randomly rotate too
                    if (_rotationStrength != 0)
                    {
                        float degrees = Random.Range(-_rotationStrength, _rotationStrength) * _shakeFactor;
                        degrees *= mask;
                        
                        Quaternion rotateOffset = Quaternion.Euler(_rotationAxis * degrees);
                        _animHandle.localRotation = _originalLocalRotation * rotateOffset;
                    }
                }
                
                lastInterp = interp;
                
                yield return null;
                float adjDeltaTime = Time.deltaTime;
                _shakeT += adjDeltaTime;
                
                while (_isLoopShake &&
                    _shakeT >= _shakeDuration)
                {
                    _shakeT -= _shakeDuration;
                }
            }
            
            RestoreTransform();
            
            // Complete
            _activeRoutine = null;
        }
        
        private void RestoreTransform()
        {        
            _animHandle.localPosition = _originalLocalPosition;
            _animHandle.localRotation = _originalLocalRotation;
        }

        void OnDisable()
        {
            StopShake();
        }
    }
}