namespace Project.Assets._Project._Scripts.Boss
{
    public interface IBossAnimation
    {
       
        void PlayAnimation(int animationHash, float crossFadeDuration = 0f);
        void SetLocomotionSpeed(float speed);
        void UpdateStates();
        int CurrentStateHash { get; }
    }
}