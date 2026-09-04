namespace Project.Assets._Project._Scripts.StateMachine
{
    public interface IState
    {

        void OnEnter();
        void OnExit();
        void Update();
        void FixedUpdate();
    }
}
