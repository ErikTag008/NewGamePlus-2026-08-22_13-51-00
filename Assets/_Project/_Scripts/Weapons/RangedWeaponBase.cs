using Cysharp.Threading.Tasks;
using KBCore.Refs;
using Reflex.Attributes;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Assets._Project._Scripts.Weapons
{
    public class RangedWeaponBase : WeaponBase
    {
        [SerializeField] private Transform _projectileStart;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _shootSFX;
        [SerializeField] private float _volume = 1f;
        [SerializeField] private bool _fullAuto = false;
        [SerializeField] private bool _infiniteAmmo = true;
        [SerializeField] private bool _infiniteReserve = true;
        [SerializeField] private float _projectileVelocity = 50f;
        [SerializeField] private float _attackCooldown = 0.2f;
        [SerializeField] private int _projectileAmount = 1;
        [SerializeField] private int _projectileDamage = 1;
        [SerializeField] private ProjectileBase _projectilePrefab;

        [SerializeField] private ProjectileSpreadType _spreadType;
        [SerializeField] private float _projectileSpreadReferenceDistance = 10f;
        [SerializeField, Range(0f, 30f)] private float _projectileAccuracyDisplacementMagnitude = 1f;
        [SerializeField, Range(0f, 30f)] private float _projectileSpreadMagnitude = 10f;
        [SerializeField] private float _projectileReleaseDelay = 5f;
        private ObjectPool<ProjectileBase> _projectilesPool;
        [SerializeField] private int _projectilesPoolDefaultCapacity = 100;
        [SerializeField] private int _projectilesPoolMaxSize = 200;
        [SerializeField] private int _projectilesOneFrameMaxSpawnAmount = 5;

        [SerializeField] private int _projectilesPerMagazine = 30;
        [SerializeField] private int _currentProjectilesInMagazine = 30;
        [SerializeField] private int _currentProjectilesInStorage = 210;
        [SerializeField] private float _reloadDuration = 2f;

        [Inject] private readonly ProjectileParent _projectileParent;

        private bool _isAttackDown;
        private Vector3 _currentTarget = Vector3.zero;
        private bool _canShoot = true;

        private void Awake()
        {
            _currentProjectilesInMagazine = _projectilesPerMagazine;
        }

        private void Start()
        {
            InitializeProjectilePool();
            //InitializeProjectiles().Forget();
        }

        private async UniTaskVoid InitializeProjectiles()
        {
            for (int i = 0; i < _projectilesPoolDefaultCapacity; i++)
            {
                var proj = _projectilesPool.Get();
                await UniTask.Yield(cancellationToken: this.destroyCancellationToken);
                _projectilesPool.Release(proj);
            }   
        }

        private void OnDestroy()
        {
            _projectilesPool?.Clear();
            _projectilesPool?.Dispose();
        }


        public override void Attack()
        {
            _isAttackDown = true;
            if (_currentProjectilesInMagazine <= 0) Reload();
            if (!_canShoot) return;
            if (_fullAuto)
            {
                StartFullAutoShooting(this.destroyCancellationToken).Forget();
            }
            else
            {
                SemiAutoShootProjectilesInSpread(this.destroyCancellationToken).Forget();
            }
        }
        public override void Reload()
        {
            if(_currentProjectilesInMagazine >= _projectilesPerMagazine || _currentProjectilesInStorage <= 0 && !_infiniteReserve) return;
            StartReload(this.destroyCancellationToken).Forget();
        }

        private async UniTaskVoid StartReload(CancellationToken token)
        {
            _canShoot = false;
            await UniTask.WaitForSeconds(_reloadDuration, cancellationToken: token);
            int needed = _projectilesPerMagazine - _currentProjectilesInMagazine;
            
            
            int toLoad = _infiniteReserve ? needed : Math.Min(needed, _currentProjectilesInStorage);
            _currentProjectilesInMagazine += toLoad;
            _currentProjectilesInStorage -= _infiniteReserve ? 0 : toLoad;
            _canShoot = true;
        }

        private async UniTaskVoid SemiAutoShootProjectilesInSpread(CancellationToken token)
        {
            _canShoot = false;
            await GetAndShootProjectilesInSpread(_currentTarget);
            await UniTask.WaitForSeconds(_attackCooldown, cancellationToken: token);
            _canShoot = true;
        }

        private async UniTaskVoid StartFullAutoShooting(CancellationToken token)
        {
            while (_isAttackDown && _canShoot && _currentProjectilesInMagazine > 0)
            {
                token.ThrowIfCancellationRequested();
                _canShoot = false;
                await GetAndShootProjectilesInSpread(_currentTarget);
                await UniTask.WaitForSeconds(_attackCooldown, cancellationToken: token);
                _canShoot = true;
            }
        }
        public override void StopAttack()
        {
            _isAttackDown = false;
        }

        public override void UpdateTarget(Vector3 target)
        {
            _currentTarget = target;
        }

        private async UniTask GetAndShootProjectilesInSpread(Vector3 target)
        {
            if(!_infiniteAmmo) _currentProjectilesInMagazine -= 1;
            _audioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            _audioSource.PlayOneShot(_shootSFX, _volume);
            switch (_spreadType)
            {
                case ProjectileSpreadType.Single:
                    {
                        await ShootSingle(target);
                    }
                    break;

                case ProjectileSpreadType.Circular:
                    {
                        await ShootCircular(target);
                    }
                    break;

                case ProjectileSpreadType.Horizontal:
                    {
                        await ShootHorizontal(target);
                    }
                    break;

                case ProjectileSpreadType.Vertical:
                    {
                        await ShootVertical(target);
                    }
                    break;

                case ProjectileSpreadType.XShaped:
                    {
                        await ShootXShaped(target);
                    }
                    break;

                default:
                    await ShootSingle(target);
                    break;

            }
        }

        private async UniTask ShootSingle(Vector3 target)
        {
            int count = Math.Max(1, _projectileAmount);

            Vector3 baseDir = (target - _projectileStart.position);
            if (baseDir.sqrMagnitude < 1e-6f) baseDir = _projectileStart.forward;
            baseDir.Normalize();
            BuildOrthonormalBasis(baseDir, out var tangent, out var bitangent);
            float radius = _projectileSpreadMagnitude;
            for (int i = 0; i < count; i++)
            {
                Vector2 sample = UnityEngine.Random.insideUnitCircle * radius;

                Vector3 offset = tangent * sample.x + bitangent * sample.y;

                Vector3 shotPoint = _projectileStart.position + baseDir * _projectileSpreadReferenceDistance + offset;
                Vector3 direction = shotPoint - _projectileStart.position;

                ShootSingleProjectile(direction);
                if(i % _projectilesOneFrameMaxSpawnAmount == 0)
                    await UniTask.Yield(PlayerLoopTiming.Update,cancellationToken: this.destroyCancellationToken);
            }
        }

        private void BuildOrthonormalBasis(Vector3 forward, out Vector3 tangent, out Vector3 bitangent)
        {
            forward.Normalize();
            // pick an arbitrary vector not parallel to forward (prefer projectile start up)
            Vector3 arbitrary = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(forward, arbitrary)) > 0.999f)
                arbitrary = _projectileStart != null ? _projectileStart.right : Vector3.right;

            tangent = Vector3.Cross(arbitrary, forward).normalized;
            if (tangent.sqrMagnitude < 1e-6f)
                tangent = Vector3.Cross(Vector3.forward, forward).normalized;

            bitangent = Vector3.Cross(forward, tangent).normalized;
        }

        private async UniTask ShootCircular(Vector3 target)
        {
            int count = Math.Max(1, _projectileAmount);
            float radius = _projectileSpreadMagnitude;

            // base forward direction to aim at
            Vector3 baseDir = (target - _projectileStart.position);
            if (baseDir.sqrMagnitude < 1e-6f) baseDir = _projectileStart.forward;
            baseDir.Normalize();

            BuildOrthonormalBasis(baseDir, out var tangent, out var bitangent);

            for (int i = 0; i < count; i++)
            {
                float angle = (float)i / count * 2f * Mathf.PI;
                Vector3 offset = 2f * radius * (tangent * Mathf.Cos(angle) + bitangent * Mathf.Sin(angle));
                Vector3 circlePoint = _projectileStart.position + baseDir * _projectileSpreadReferenceDistance + offset; // 10 units forward center (distance irrelevant — direction matters)
                var direction = (circlePoint - _projectileStart.position);
                ShootSingleProjectile(direction);
                if (i % _projectilesOneFrameMaxSpawnAmount == 0)
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: this.destroyCancellationToken);
            }
        }

        private async UniTask ShootHorizontal(Vector3 target)
        {
            int count = Math.Max(1, _projectileAmount);
            float length = _projectileSpreadMagnitude;

            Vector3 baseDir = (target - _projectileStart.position);
            if (baseDir.sqrMagnitude < 1e-6f) baseDir = _projectileStart.forward;
            baseDir.Normalize();

            BuildOrthonormalBasis(baseDir, out var tangent, out var bitangent);

            if (count == 1)
            {
                var direction = target - _projectileStart.position;
                ShootSingleProjectile(direction);
            }
            else
            {
                float step = length / (count - 1);
                float startOffset = -length * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    float t = startOffset + i * step;
                    Vector3 offset = tangent * t; // horizontal along tangent
                    Vector3 linePoint = _projectileStart.position + baseDir * _projectileSpreadReferenceDistance + offset;
                    var direction = (linePoint - _projectileStart.position);
                    ShootSingleProjectile(direction);
                    if (i % _projectilesOneFrameMaxSpawnAmount == 0)
                        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: this.destroyCancellationToken);
                }
            }
        }

        private async UniTask ShootVertical(Vector3 target)
        {
            int count = Math.Max(1, _projectileAmount);
            float length = _projectileSpreadMagnitude;

            Vector3 baseDir = (target - _projectileStart.position);
            if (baseDir.sqrMagnitude < 1e-6f) baseDir = _projectileStart.forward;
            baseDir.Normalize();

            BuildOrthonormalBasis(baseDir, out var tangent, out var bitangent);

            if (count == 1)
            {
                var direction = target - _projectileStart.position;
                ShootSingleProjectile(direction);
            }
            else
            {
                float step = length / (count - 1);
                float startOffset = -length * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    float t = startOffset + i * step;
                    Vector3 offset = bitangent * t; // vertical along bitangent
                    Vector3 linePoint = _projectileStart.position + baseDir * _projectileSpreadReferenceDistance + offset;
                    var direction = (linePoint - _projectileStart.position);
                    ShootSingleProjectile(direction);
                    if (i % _projectilesOneFrameMaxSpawnAmount == 0)
                        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: this.destroyCancellationToken);
                }
            }
        }

        private async UniTask ShootXShaped(Vector3 target)
        {
            int total = Math.Max(1, _projectileAmount);
            int countLine1 = (total + 1) / 2; // first diagonal count
            int countLine2 = total - countLine1; // second diagonal count
            float length = _projectileSpreadMagnitude;

            Vector3 baseDir = (target - _projectileStart.position);
            if (baseDir.sqrMagnitude < 1e-6f) baseDir = _projectileStart.forward;
            baseDir.Normalize();

            BuildOrthonormalBasis(baseDir, out var tangent, out var bitangent);

            Vector3 diag1Dir = (tangent + bitangent).normalized;
            Vector3 diag2Dir = (tangent - bitangent).normalized;

            async UniTask ShootAlongDiagonal(Vector3 diagDir, int count)
            {
                if (count <= 0) return;
                if (count == 1)
                {
                    Vector3 offset = Vector3.zero;
                    var direction = (target + offset) - _projectileStart.position;
                    ShootSingleProjectile(direction);
                    return;
                }

                float step = length / (count - 1);
                float startOffset = -length * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    float t = startOffset + i * step;
                    Vector3 offset = diagDir * t;
                    Vector3 linePoint = _projectileStart.position + baseDir * _projectileSpreadReferenceDistance + offset;
                    var direction = (linePoint - _projectileStart.position);
                    ShootSingleProjectile(direction);
                    if (i % _projectilesOneFrameMaxSpawnAmount == 0)
                        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: this.destroyCancellationToken);
                }
            }

            await ShootAlongDiagonal(diag1Dir, countLine1);
            await ShootAlongDiagonal(diag2Dir, countLine2);
        }

        private void ShootSingleProjectile(Vector3 direction)
        {
            ProjectileBase proj = _projectilesPool?.Get();
            var displacedDirection = (direction + UnityEngine.Random.insideUnitSphere * _projectileAccuracyDisplacementMagnitude.ToPercent()).normalized;
            proj.transform.rotation = Quaternion.LookRotation(displacedDirection);
            proj.Shoot(_projectileVelocity, displacedDirection, _projectileDamage);
            ReleaseProjectileAfterDelay(proj, this.destroyCancellationToken).Forget();
        }

        private async UniTaskVoid ReleaseProjectileAfterDelay(ProjectileBase proj, CancellationToken token)
        {
            await UniTask.WaitForSeconds(_projectileReleaseDelay, cancellationToken: token);
            if(proj != null)
            _projectilesPool.Release(proj);
        }

        private void InitializeProjectilePool()
        {
            var projectileParent = _projectileParent.transform;
            _projectilesPool = new
                (
                    () => Instantiate(_projectilePrefab, _projectileStart.position, _projectileStart.rotation, projectileParent),
                    p => 
                    {
                        p.ResetProjectile(); 
                        p.transform.SetPositionAndRotation(_projectileStart.position, _projectileStart.rotation); 
                        p.gameObject.SetActive(true);  
                    },
                    p => p.gameObject.SetActive(false),
                    p => Destroy(p.gameObject),
                    true,
                    _projectilesPoolDefaultCapacity,
                    _projectilesPoolMaxSize
                );
        }

        

        public enum ProjectileSpreadType
        {
            Single,
            Horizontal,
            Vertical,
            Circular,
            XShaped,
        }
    }
}
