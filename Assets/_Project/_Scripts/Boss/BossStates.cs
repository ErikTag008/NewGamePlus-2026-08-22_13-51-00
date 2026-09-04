using Cysharp.Threading.Tasks;
using Project.Assets._Project._Scripts.StateMachine;
using UnityEngine;
using UnityEngine.UIElements;



namespace Project.Assets._Project._Scripts.Boss
{
    public abstract class BossBaseState : IState
    {
        protected readonly BossAI Boss;
        protected readonly IBossMovement Movement;
        protected readonly IBossCombat Combat;
        protected readonly IBossAnimation Animation;
        protected virtual int StateHash { get; set; }
        protected readonly BossStats Stats;

        protected float CrossFadeDuration => Stats.CrossFadeDuration;

        public BossBaseState(BossAI boss)
        {
            Boss = boss;
            Movement = Boss.Movement;
            Combat = Boss.Combat;
            Animation = Boss.Animation;
            Stats = Boss.Stats;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void Update()
        {
        }
        public virtual void FixedUpdate()
        {
        }
    }

    public class BossChaseState : BossBaseState
    {
        public BossChaseState(BossAI bossAI) : base(bossAI)
        {
        }
        public override void OnEnter()
        {
            Boss.UpdateAllRandomOccurences();

            Animation.PlayAnimation(Animator.StringToHash(Stats.LocomotionStateName), CrossFadeDuration);
            Movement.MoveToObject(Boss.PlayerTransform, Stats.DestinationUpdateDelay);
        }

        public override void Update()
        {
            Animation.SetLocomotionSpeed(Boss.Speed);

        }

        public override void OnExit()
        {
            Movement.StopMovement();
        }
    }

    public class BossEvadeState : BossBaseState
    {
        public BossEvadeState(BossAI bossAI) : base(bossAI)
        {
        }
        public override void OnEnter()
        {
            Boss.UpdateAllRandomOccurences();
            Animation.PlayAnimation(Animator.StringToHash(Stats.LocomotionStateName), CrossFadeDuration);
            Movement.MoveTo(GetRandomPerpendicularPosition(Boss.PlayerTransform));
        }
        private Vector3 GetRandomPerpendicularPosition(Transform player)
        {
            Vector3 direction = (Boss.transform.position - player.position).normalized;
            Vector3 perpendicular = Vector3.Cross(Vector3.up, direction).normalized;

            if (Random.value > 0.5f)
                perpendicular = -perpendicular;

            float distance = Random.Range(2f, 5f);

            return player.position + perpendicular * distance;
        }

        public override void Update()
        {
            Animation.SetLocomotionSpeed(Boss.Speed);
        }

        public override void OnExit()
        {
            Movement.StopMovement();
        }
    }

    public class BossRoarState : BossBaseState
    {
        public BossRoarState(BossAI boss) : base(boss) 
        {
        }
        public override void OnEnter()
        {
            Boss.UpdateAllRandomOccurences();

            Animation.PlayAnimation(Animator.StringToHash(Stats.RoarStateName), CrossFadeDuration);
            Boss.PlayAudio(Stats.RoarSFX, true, Stats.RoarSFXVolume);

        }

    }

    public class BossDeathState : BossBaseState
    {

        public BossDeathState(BossAI bossAI) : base(bossAI)
        {
        }
        public override void OnEnter()
        {
            Animation.PlayAnimation(Animator.StringToHash(Stats.DeathStateName), CrossFadeDuration);
            Boss.PlayAudio(Stats.DeathSFX, false, Stats.DeathSFXVolume);
        }
    }

    public class BossLeftHandAttackState : BossBaseState
    {
        public BossLeftHandAttackState(BossAI boss) : base(boss)
        {
        }
        public override void OnEnter()
        {
            Boss.UpdateAllRandomOccurences();

            Animation.PlayAnimation(Animator.StringToHash(Stats.LeftHandStateName), CrossFadeDuration);
            Boss.UpdateAllRandomOccurences();
            EUtils.DelayedExecute(0.5f, () => Combat.ToggleLeftHandCollider(true)).Forget();
        }

        public override void OnExit()
        {
            Combat.ToggleLeftHandCollider(false);
        }
    }

    public class BossRightHandAttackState : BossBaseState
    {
        public BossRightHandAttackState(BossAI boss) : base(boss)
        {
        }
        public override void OnEnter()
        {
            Boss.UpdateAllRandomOccurences();

            Animation.PlayAnimation(Animator.StringToHash(Stats.RightHandStateName), CrossFadeDuration);
            Boss.UpdateAllRandomOccurences();
            EUtils.DelayedExecute(0.5f, () => Combat.ToggleLeftHandCollider(true)).Forget();


        }

        public override void OnExit()
        {
            Combat.ToggleRightHandCollider(false);
        }
    }

    public class BossJumpAttackState : BossBaseState
    {
        public BossJumpAttackState(BossAI boss) : base(boss)
        {
        }
        public override void OnEnter()
        {
            Boss.UpdateAllRandomOccurences();
            Movement.JumpTo(Boss.PlayerTransform.position);
            Animation.PlayAnimation(Animator.StringToHash(Stats.JumpAttackStateName), CrossFadeDuration);
            Movement.OnJumpAttackFinished += CheckJumpAttackDamage;
        }

        private void CheckJumpAttackDamage()
        {
            Combat.CheckJumpAttackDamage();
            Boss.PlayAudio(Stats.GroundSlamSFX, true, Stats.GroundSlamSFXVolume);
        }

        public override void OnExit()
        {
            Movement.OnJumpAttackFinished -= CheckJumpAttackDamage;
            Movement.StopMovement();
        }
    }

}
