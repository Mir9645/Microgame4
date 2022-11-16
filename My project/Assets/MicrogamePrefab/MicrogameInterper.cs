using UnityEngine;

namespace Egam202
{
    public class MicrogameInterper : MonoBehaviour
    {
        [SerializeField] private float _frequency = 10f;
        [SerializeField] private float _damping = 1f;

        private float _value = 0f;
        public float value
        {
            get { return _value; }
        }

        private float _velocity = 0f;
        private float _goal = 0f;

        public void SetGoal(float goal, bool instant = false)
        {
            _goal = goal;
            
            if (instant)
            {
                _velocity = 0;
                _value = _goal;
            }
        }

        void Update()
        {
            CalcDampedSimpleHarmonicMotion(ref _value, ref _velocity, _goal, 
                Time.deltaTime, _frequency, _damping);
        }

        public void CalcDampedSimpleHarmonicMotion
                            (
                                ref float pPos,         // position value to update
                                ref float pVel,         // velocity value to update
                                float equilibriumPos,   // position to approach
                                float deltaTime,        // time to update over
                                float angularFrequency, // angular frequency of motion
                                float dampingRatio      // damping ratio of motion
                            )
        {
            const float epsilon = 0.0001f;

            // if there is no angular frequency, the spring will not move
            if (angularFrequency < epsilon)
                return;

            // clamp the damping ratio in legal range
            if (dampingRatio < 0.0f)
                dampingRatio = 0.0f;

            // calculate initial state in equilibrium relative space
            float initialPos = pPos - equilibriumPos;
            float initialVel = pVel;

            // if over-damped
            if (dampingRatio > 1.0f + epsilon)
            {
                // calculate constants based on motion parameters
                // Note: These could be cached off between multiple calls using the same
                //       parameters for deltaTime, angularFrequency and dampingRatio.
                float za = -angularFrequency * dampingRatio;
                float zb = angularFrequency * Mathf.Sqrt(dampingRatio * dampingRatio - 1.0f);
                float z1 = za - zb;
                float z2 = za + zb;
                float expTerm1 = Mathf.Exp(z1 * deltaTime);
                float expTerm2 = Mathf.Exp(z2 * deltaTime);

                // update motion
                float c1 = (initialVel - initialPos * z2) / (-2.0f * zb); // z1 - z2 = -2*zb
                float c2 = initialPos - c1;
                pPos = equilibriumPos + c1 * expTerm1 + c2 * expTerm2;
                pVel = c1 * z1 * expTerm1 + c2 * z2 * expTerm2;
            }
            // else if critically damped
            else if (dampingRatio > 1.0f - epsilon)
            {
                // calculate constants based on motion parameters
                // Note: These could be cached off between multiple calls using the same
                //       parameters for deltaTime, angularFrequency and dampingRatio.
                float expTerm = Mathf.Exp(-angularFrequency * deltaTime);

                // update motion
                float c1 = initialVel + angularFrequency * initialPos;
                float c2 = initialPos;
                float c3 = (c1 * deltaTime + c2) * expTerm;
                pPos = equilibriumPos + c3;
                pVel = (c1 * expTerm) - (c3 * angularFrequency);
            }
            // else under-damped
            else
            {
                // calculate constants based on motion parameters
                // Note: These could be cached off between multiple calls using the same
                //       parameters for deltaTime, angularFrequency and dampingRatio.
                float omegaZeta = angularFrequency * dampingRatio;
                float alpha = angularFrequency * Mathf.Sqrt(1.0f - dampingRatio * dampingRatio);
                float expTerm = Mathf.Exp(-omegaZeta * deltaTime);
                float cosTerm = Mathf.Cos(alpha * deltaTime);
                float sinTerm = Mathf.Sin(alpha * deltaTime);

                // update motion
                float c1 = initialPos;
                float c2 = (initialVel + omegaZeta * initialPos) / alpha;
                pPos = equilibriumPos + expTerm * (c1 * cosTerm + c2 * sinTerm);
                pVel = -expTerm * ((c1 * omegaZeta - c2 * alpha) * cosTerm +
                                   (c1 * alpha + c2 * omegaZeta) * sinTerm);
            }
        }
    }
}
