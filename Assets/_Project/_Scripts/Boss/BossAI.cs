using KBCore.Refs;
using Project.Assets._Project._Scripts.Player;
using Project.Assets._Project._Scripts.StateMachine;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Assets._Project._Scripts.Boss
{
    [RequireComponent(typeof(AudioSource))]
    public class BossAI : MonoBehaviour
    {
        [Inject] public readonly BossStats Stats;
        [SerializeField, Self] private BossHealth _health;
        [SerializeField, Self] private Animator _animator;
        [SerializeField, Self] private NavMeshAgent _agent;
        [SerializeField, Self] private AudioSource _audioSource;
        [SerializeField] private AttackCollider _rightHandCollider, _leftHandCollider;
        [SerializeField] private Transform _jumpAttackOrigin;

        [Inject] private readonly StateMachine.StateMachine _stateMachine;
        [Inject] private readonly PlayerController _playerController;
        public IBossMovement Movement { get; private set; }
        public IBossCombat Combat { get; private set; }
        public IBossAnimation Animation { get; private set; }
        public Transform PlayerTransform => _playerController.transform;
        public float Speed => EUtils.Math.Remap(0f, Stats.MaxSpeed, 0f, 1f, _agent.velocity.magnitude);
        
        private IState _fromChaseToNextState;
        private IState _fromEvadeToNextState;
        private IState _fromRoarToNextState;
        private IState _fromAttackToNextState;
        private IState _fromJumpAttackToNextState;

        private IState _chaseState;
        private IState _leftAttackState;
        private IState _rightAttackState;
        private IState _evadeState;
        private IState _roarState;
        private IState _jumpAttackState;
        private IState _deathState;

        private float _currentStateActiveDuration = 0f;
        private IState _currentState;


        private void Awake()
        {
            Movement = new BossMovement(_agent, Stats);
            Combat = new BossCombat(_rightHandCollider, _leftHandCollider, _jumpAttackOrigin, Stats);
            Animation = new BossAnimation(_animator);

            _chaseState = new BossChaseState(this);
            _leftAttackState = new BossLeftHandAttackState(this);
            _rightAttackState = new BossRightHandAttackState(this);
            _evadeState = new BossEvadeState(this);
            _roarState = new BossRoarState(this);
            _jumpAttackState = new BossJumpAttackState(this);
            _deathState = new BossDeathState(this);

            InitializeTransitions();
            _stateMachine.SetState(_chaseState);
            _health.OnDamageTaken += () => PlayAudio(Stats.HitmarkerSFX, true, Stats.HitmarkerSFXVolume);
           
        }

        public void PlayAudio(AudioClip clip, bool randomizedPitch = false,float volume = 1f)
        {
            if (randomizedPitch) _audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            _audioSource.PlayOneShot(clip, volume);
        }

        public void UpdateAllRandomOccurences()
        {
            float fromChaseRoll = Random.value;
            if(fromChaseRoll <= Stats.FromChaseToRightHandAttackChance)
            {
                _fromChaseToNextState = _rightAttackState;
            }
            else
            {
                _fromChaseToNextState = _leftAttackState;
            }


            float fromEvadeRoll = Random.value;
            if(fromEvadeRoll <= Stats.FromEvadeToChaseChance)
            {
                _fromEvadeToNextState = _chaseState;
            }
            else if(fromEvadeRoll <= Stats.FromEvadeToChaseChance + Stats.FromEvadeToJumpAttackChance)
            {
                _fromEvadeToNextState = _jumpAttackState;
            }
            else if(fromEvadeRoll <= Stats.FromEvadeToChaseChance + Stats.FromEvadeToJumpAttackChance + Stats.FromEvadeToRoarChance)
            {
                _fromEvadeToNextState = _roarState;
            }
            else
            {
                _fromEvadeToNextState = _evadeState;
            }

            float fromRoarRoll = Random.value;
            if(fromRoarRoll <= Stats.FromRoarToChaseChance)
            {
                _fromRoarToNextState = _chaseState;
            }
            else
            {
                _fromRoarToNextState = _jumpAttackState;
            }

            float fromAttackRoll = Random.value;
            if(fromAttackRoll <= Stats.FromAttackToChaseChance)
            {
                _fromAttackToNextState = _chaseState;
            }
            else if(fromAttackRoll <= Stats.FromAttackToChaseChance + Stats.FromAttackToEvadeChance)
            {
                _fromAttackToNextState = _evadeState;
            }
            else if(fromAttackRoll <= Stats.FromAttackToChaseChance + Stats.FromAttackToEvadeChance + Stats.FromAttackToRoarChance)
            {
                _fromAttackToNextState = _roarState;
            }
            else if (fromAttackRoll <= Stats.FromAttackToChaseChance + Stats.FromAttackToEvadeChance + Stats.FromAttackToRoarChance + Stats.FromAttackToLeftHandAttackChance)
            {
                _fromAttackToNextState = _leftAttackState;
            }
            else
            {
                _fromAttackToNextState = _rightAttackState;
            }

            float fromJumpAttackRoll = Random.value;
            if(fromJumpAttackRoll <= Stats.FromJumpAttackToChaseChance)
            {
                _fromJumpAttackToNextState = _chaseState;
            }
            else if(fromJumpAttackRoll <= Stats.FromJumpAttackToChaseChance + Stats.FromJumpAttackToLeftHandAttackChance)
            {
                _fromJumpAttackToNextState = _leftAttackState;
            }
            else if (fromJumpAttackRoll <= Stats.FromJumpAttackToChaseChance + Stats.FromJumpAttackToLeftHandAttackChance + Stats.FromJumpAttackToRightHandAttackChance)
            {
                _fromJumpAttackToNextState = _rightAttackState;
            }
            else
            {
                _fromJumpAttackToNextState = _jumpAttackState;
            }
        }

        private void InitializeTransitions()
        {
            // Death can happen from anywhere
            _stateMachine.AddAnyTransition(
                _deathState,
                new FuncPredicate(() => _health.IsDead && _currentState != _deathState));

            //From Chase

            _stateMachine.AddTransition(
                _chaseState,
                _leftAttackState,
                new FuncPredicate(() =>
                    (_agent.remainingDistance <= Stats.StoppingDistance && _fromChaseToNextState == _leftAttackState)));

            _stateMachine.AddTransition(
                _chaseState,
                _rightAttackState,
                new FuncPredicate(() =>
                    (_agent.remainingDistance <= Stats.StoppingDistance && _fromChaseToNextState == _rightAttackState)));

            _stateMachine.AddTransition(
                _chaseState,
                _jumpAttackState,
                new FuncPredicate(() =>
                    _agent.remainingDistance >= Stats.StoppingDistance && _currentStateActiveDuration >= Stats.ChaseMaxDuration));
            //From Evade

            _stateMachine.AddTransition(
                _evadeState,
                _chaseState,
                new FuncPredicate(() =>
                    (_agent.remainingDistance <= Stats.StoppingDistance && _fromEvadeToNextState == _chaseState) || _currentStateActiveDuration > Stats.EvadeMaxDuration));

            _stateMachine.AddTransition(
                _evadeState,
                _jumpAttackState,
                new FuncPredicate(() =>
                    (_agent.remainingDistance <= Stats.StoppingDistance && _fromEvadeToNextState == _jumpAttackState) || _currentStateActiveDuration > Stats.EvadeMaxDuration));

            _stateMachine.AddTransition(
                _evadeState,
                _roarState,
                new FuncPredicate(() =>
                    (_agent.remainingDistance <= Stats.StoppingDistance && _fromEvadeToNextState == _roarState) || _currentStateActiveDuration > Stats.EvadeMaxDuration));
            _stateMachine.AddTransition(
                _evadeState,
                _evadeState,
                new FuncPredicate(() =>
                    (_agent.remainingDistance <= Stats.StoppingDistance && _fromEvadeToNextState == _roarState) || _currentStateActiveDuration > Stats.EvadeMaxDuration));

            //From Roar

            _stateMachine.AddTransition(
                _roarState,
                _chaseState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromRoarToNextState == _chaseState));

            _stateMachine.AddTransition(
                _roarState,
                _jumpAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromRoarToNextState == _jumpAttackState));


            //From Left Hand Attack
            _stateMachine.AddTransition(
                _leftAttackState,
                _chaseState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _chaseState));

            _stateMachine.AddTransition(
                _leftAttackState,
                _evadeState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _evadeState));

            _stateMachine.AddTransition(
                _leftAttackState,
                _roarState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _roarState));

            _stateMachine.AddTransition(
                _leftAttackState,
                _rightAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _rightAttackState));

            _stateMachine.AddTransition(
                _leftAttackState,
                _leftAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _leftAttackState));

            //From Right Hand Attack
            _stateMachine.AddTransition(
                _rightAttackState,
                _chaseState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _chaseState));

            _stateMachine.AddTransition(
                _rightAttackState,
                _evadeState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _evadeState));

            _stateMachine.AddTransition(
                _rightAttackState,
                _roarState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _roarState));

            _stateMachine.AddTransition(
                _rightAttackState,
                _rightAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _rightAttackState));

            _stateMachine.AddTransition(
                _rightAttackState,
                _leftAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromAttackToNextState == _leftAttackState));

            //From Jump Attack
            _stateMachine.AddTransition(
                _jumpAttackState,
                _chaseState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromJumpAttackToNextState == _chaseState));

            _stateMachine.AddTransition(
                _jumpAttackState,
                _leftAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromJumpAttackToNextState == _leftAttackState));

            _stateMachine.AddTransition(
                _jumpAttackState,
                _rightAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromJumpAttackToNextState == _rightAttackState));

            _stateMachine.AddTransition(
                _jumpAttackState,
                _jumpAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromJumpAttackToNextState == _jumpAttackState));

            _stateMachine.AddTransition(
                _jumpAttackState,
                _jumpAttackState,
                new FuncPredicate(() =>
                    Animation.CurrentStateHash == -1 &&
                    _fromJumpAttackToNextState == _jumpAttackState));


        }

        private void Update()
        {
            _stateMachine.Update();
            Animation.UpdateStates();

            Debug.Log(
       $"State: {_stateMachine.GetState()?.GetType().Name} | " +
       $"AnimHash: {Animation.CurrentStateHash} | " +
       $"AttackNext: {_fromAttackToNextState?.GetType().Name}"
   );

            if (_currentState == _stateMachine.GetState())
            {
                _currentStateActiveDuration += Time.deltaTime;
            }
            else
            {
                _currentStateActiveDuration = 0;
                _currentState = _stateMachine.GetState();
            }
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

    }
}
