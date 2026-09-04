using UnityEngine;

namespace Project.Assets._Project._Scripts.Boss
{
    public class BossAnimation : IBossAnimation
    {
        private static readonly int _speedHash = Animator.StringToHash("Speed");
        private readonly Animator _animator;
        private int _currentState;
        public int CurrentStateHash => _currentState;
        
        public BossAnimation(Animator animator)
        {
            _animator = animator;
        }

        public void SetLocomotionSpeed(float speed) 
        {
            _animator.SetFloat(_speedHash, speed);
        }

        public void PlayAnimation(int animationHash, float crossFadeDuration = 0)
        {
            _currentState = animationHash;
            _animator.CrossFade(animationHash, crossFadeDuration);
        }

        public void UpdateStates()
        {
            if (_currentState == -1)
                return;

            var info = _animator.GetCurrentAnimatorStateInfo(0);

            // Make sure we're actually in the state we're tracking
            if (info.shortNameHash != _currentState)
                return;

            // normalizedTime >= 1 means the animation has played through once
            if (info.normalizedTime >= 1f)
            {
                _currentState = -1;
            }
        }
    }
}