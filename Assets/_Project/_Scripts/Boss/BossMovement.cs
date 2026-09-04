using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Assets._Project._Scripts.Boss
{
    public class BossMovement : IBossMovement
    {
        private readonly NavMeshAgent _agent;
        private readonly BossStats _stats;
        private CancellationTokenSource _cts;
        private Tween _jumpTween;
        public event Action OnJumpAttackFinished;
        public BossMovement(NavMeshAgent agent, BossStats stats)
        {
            _agent = agent;
            _stats = stats;
            _agent.stoppingDistance = _stats.StoppingDistance;
            _agent.speed = _stats.MaxSpeed;
            _agent.acceleration = _stats.AccelerationSpeed;
            _agent.angularSpeed = _stats.AngularSpeed;
        }
        public void JumpTo(Vector3 position)
        {

            var direction = (position - _agent.transform.position).normalized;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            if (NavMesh.SamplePosition(position,out NavMeshHit hit,2f,NavMesh.AllAreas))
            {
                _agent.isStopped = true;
                _agent.updateRotation = false;
                _agent.transform.DORotate(targetRotation.eulerAngles, 0.25f);

                Vector3 landingPosition = hit.position;
                _jumpTween?.Kill();
                _jumpTween = _agent.transform.DOJump(landingPosition, _stats.JumpHeight, 1, _stats.JumpDuration)
                    .SetEase(_stats.JumpEase)
                    .OnComplete(() => OnJumpAttackFinished?.Invoke());
            }

        }

        public void MoveTo(Vector3 position)
        {
            if (!TryGetClosestNavMeshPosition(position, out Vector3 closestPosition)) return;
            _agent.isStopped = false;
            _agent.updateRotation = true;
            _agent.SetDestination(closestPosition);
        }

        public void MoveToObject(Transform target, float updateDelay = 0.2f)
        {
            try { _cts?.Cancel(); } catch { }
            
            _cts = new();
            _agent.isStopped = false;
            _agent.updateRotation = true;
            SetDestinationRepeating(_cts.Token, target, updateDelay).Forget();
        }

        private async UniTaskVoid SetDestinationRepeating(CancellationToken token, Transform target, float updateDelay)
        {
            while (!token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                if(TryGetClosestNavMeshPosition(target.position, out Vector3 closestPosition))
                {
                    _agent.SetDestination(closestPosition);
                }
                await UniTask.WaitForSeconds(updateDelay, cancellationToken: token);
            }
        }

        private bool TryGetClosestNavMeshPosition(Vector3 position, out Vector3 navMeshPosition, float searchRadius = 5f)
        {
            if (NavMesh.SamplePosition(position,out NavMeshHit hit,searchRadius,NavMesh.AllAreas))
            {
                navMeshPosition = hit.position;
                return true;
            }

            navMeshPosition = default;
            return false;
        }

        public void StopMovement()
        {
            try { _cts?.Cancel(); } catch { }
            _jumpTween?.Kill();
            _agent.isStopped = true;
            _agent.updateRotation = false;
        
        }


        private bool TryGetGroundedPosition(Vector3 position, out Vector3 groundedPosition)
        {
            const float rayStartHeight = 2f;
            const float rayDistance = 10f;

            Vector3 rayOrigin = position + Vector3.up * rayStartHeight;

            if (Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit hit,
                rayDistance
                ))
            {
                groundedPosition = hit.point;
                return true;
            }

            groundedPosition = position;
            return false;
        }
    }
}