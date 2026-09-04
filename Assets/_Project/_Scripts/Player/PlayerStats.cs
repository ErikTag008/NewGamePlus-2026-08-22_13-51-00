using Project.Assets._Project._Scripts.Weapons;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Project/Player Stats", order = 0)]
    public class PlayerStats : ScriptableObject, IMovementStats, ICameraStats, IInteractionStats, ICombatStats, IHealthStats, IAudioStats
    {
        [field: Header("Movement Stats")]
        [field: SerializeField] public float MaxMoveSpeed { get; private set; } = 25f;
        [field: SerializeField] public float JumpForce { get; private set; } = 30f;
        [field: SerializeField] public float CoyoteTime { get; private set; } = 0.2f;
        [field: SerializeField] public float JumpBuffer { get; private set; } = 0.2f;
        [field: SerializeField] public float JumpButtonUpVerticalVelocityMultiplier { get; private set; } = 0.3f;
        [field: SerializeField] public float GroundCheckRadius { get; private set; } = 0.2f;
        [field: SerializeField] public float GroundCheckMaxYVelocity { get; private set; } = 0.5f;

        [field: SerializeField, Range(0f, 90f)] public float MaxGroundAngle { get; private set; } = 30f;
        [field: SerializeField] public float HorizontalDamping { get; private set; } = 5f;
        [field: SerializeField] public float VerticalDamping { get; private set; } = 0.1f;
        [field: SerializeField] public float GroundAcceleration { get; private set; } = 20f;
        [field: SerializeField] public float AirAcceleration { get; private set; } = 10f;
        [field: SerializeField] public LayerMask GroundCheckLayers { get; private set; }

        [field: Header("Camera Stats")]
        [field: SerializeField, Range(1f, 89f)] public float MaxPitch { get; private set; } = 85f;
        [field: SerializeField] public float MouseLookXSensitivity { get; private set; } = 5f;
        [field: SerializeField] public float MouseLookYSensitivity { get; private set; } = 5f;
        [field: SerializeField] public float GamepadLookXSensitivity { get; private set; } = 5f;
        [field: SerializeField] public float GamepadLookYSensitivity { get; private set; } = 5f;

        [field: Header("Interaction Stats")]
        [field: SerializeField] public float MaxInteractionDistance { get; private set; } = 5f;
        [field: SerializeField] public LayerMask InteractionLayer { get; private set; }

        [field: Header("Combat Stats")]
        [field: SerializeField] public float MaxHitDistance { get; private set; } = 500f;
        [field: SerializeField] public LayerMask AttackRaycastLayer { get; private set; }
        [field: SerializeField] public Vector3 WeaponPivotLocalPos { get; private set; } = new Vector3(0.4f, 1f, 0.6f);

        [field: Header("Health Stats")]
        [field: SerializeField] public int MaxHealth { get; private set; } = 100;
        [field: SerializeField] public int PotionHealAmount { get; private set; } = 50;

        [field: Header("Audio Stats")]
        [field: SerializeField] public AudioClip DeathSFX { get; private set; }
        [field: SerializeField] public float DeathSFXVolume { get; private set; } = 1f;



    }

    public interface ICameraStats
    {
        public float MaxPitch { get; }
        public float MouseLookXSensitivity { get; }
        public float MouseLookYSensitivity { get; }
        public float GamepadLookXSensitivity { get; }
        public float GamepadLookYSensitivity { get; }
    }

    public interface IMovementStats
    {
        public float MaxMoveSpeed { get; }
        public float JumpForce { get; }
        public float CoyoteTime { get; }
        public float JumpBuffer { get; }
        public float JumpButtonUpVerticalVelocityMultiplier { get; }
        public float GroundCheckRadius { get; }
        public float GroundCheckMaxYVelocity { get; }
        public float MaxGroundAngle { get; }
        public float HorizontalDamping { get; }
        public float VerticalDamping { get; }
        public float GroundAcceleration { get; }
        public float AirAcceleration { get; }   
        public LayerMask GroundCheckLayers { get; }
    }    

    public interface IInteractionStats
    {
        public float MaxInteractionDistance { get; }
        public LayerMask InteractionLayer { get; }
    }

    public interface ICombatStats
    {
        public float MaxHitDistance { get; }
        public LayerMask AttackRaycastLayer { get; }
    }

    public interface IHealthStats
    {
        public int MaxHealth { get; }
        public int PotionHealAmount { get; }
    }

    public interface IModelStats
    {
        public Transform Model { get; }
    }

    public interface IAudioStats
    {
        public AudioClip DeathSFX { get; }
        public float DeathSFXVolume { get; }
    }
}
