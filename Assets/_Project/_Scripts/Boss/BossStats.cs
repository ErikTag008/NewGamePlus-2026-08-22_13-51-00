using DG.Tweening;
using Project.Assets._Project._Scripts.Boss;
using UnityEngine;

namespace Project.Assets._Project._Scripts.Boss
{
    [CreateAssetMenu(fileName = "BossStats", menuName = "Project/Boss Stats", order = 1)]
    public class BossStats : ScriptableObject
    {
        [field: Header("Movement")]
        [field: SerializeField] public float MaxSpeed { get; private set; } = 8f;
        [field: SerializeField] public float AccelerationSpeed { get; private set; } = 20f;
        [field: SerializeField] public float JumpHeight { get; private set; } = 3f;
        [field: SerializeField] public float JumpDuration { get; private set; } = 1f;
        [field: SerializeField] public Ease JumpEase { get; private set; } = Ease.InOutCubic;
        [field: SerializeField] public float AngularSpeed { get; private set; } = 540f;
        [field: SerializeField] public float StoppingDistance { get; private set; } = 0.5f;
        [field: SerializeField] public float DestinationUpdateDelay { get; private set; } = 0.2f;

        [field: Header("Combat")]
        [field: SerializeField] public float LeftHandDamage { get; private set; } = 15f;
        [field: SerializeField] public float RightHandDamage { get; private set; } = 10f;
        [field: SerializeField] public float JumpAttackDamage { get; private set; } = 30f;
        [field: SerializeField] public float JumpAttackRadius { get; private set; } = 30f;
        [field: SerializeField] public LayerMask PlayerLayer { get; private set; }


        [field: Header("Health")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 500f;

        [field: Header("Audio")]
        [field: SerializeField] public AudioClip RoarSFX { get; private set; }
        [field: SerializeField] public float RoarSFXVolume { get; private set; } = 1f;
        [field: SerializeField] public AudioClip DeathSFX { get; private set; }
        [field: SerializeField] public float DeathSFXVolume { get; private set; } = 1f;
        [field: SerializeField] public AudioClip GroundSlamSFX { get; private set; }
        [field: SerializeField] public float GroundSlamSFXVolume { get; private set; } = 1f;
        [field: SerializeField] public AudioClip HitmarkerSFX { get; private set; }
        [field: SerializeField] public float HitmarkerSFXVolume { get; private set; } = 1f;

        [field: Header("Animation")]
        [field: SerializeField] public float CrossFadeDuration { get; private set; } = 0.2f;
        [field: SerializeField] public string LocomotionStateName { get; private set; } = "Locomotion";
        [field: SerializeField] public string DeathStateName { get; private set; } = "Death";
        [field: SerializeField] public string RoarStateName { get; private set; } = "Roar";
        [field: SerializeField] public string RightHandStateName { get; private set; } = "RightHand";
        [field: SerializeField] public string LeftHandStateName { get; private set; } = "LeftHand";
        [field: SerializeField] public string JumpAttackStateName { get; private set; } = "JumpAttack";
        [field: SerializeField] public string JumpStateName { get; private set; } = "Jump";



        [field: Header("State Change Probabilities")]
        [field: SerializeField, Range(0f, 1f)] public float FromChaseToRightHandAttackChance { get; private set; } = 0.7f;
        [field: SerializeField, Range(0f, 1f)] public float FromChaseToLeftHandAttackChance { get; private set; } = 0.3f;

        [field: SerializeField, Range(0f, 1f)] public float FromEvadeToChaseChance { get; private set; } = 0.2f;
        [field: SerializeField, Range(0f, 1f)] public float FromEvadeToRoarChance { get; private set; } = 0.4f;
        [field: SerializeField, Range(0f, 1f)] public float FromEvadeToJumpAttackChance { get; private set; } = 0.3f;
        [field: SerializeField, Range(0f, 1f)] public float FromEvadeToEvadeChance { get; private set; } = 0.1f;
        [field: SerializeField, Range(0f, 1f)] public float FromRoarToJumpAttackChance { get; private set; } = 0.7f;
        [field: SerializeField, Range(0f, 1f)] public float FromRoarToChaseChance { get; private set; } = 0.3f;
        [field: SerializeField, Range(0f, 1f)] public float FromAttackToChaseChance { get; private set; } = 0.35f;
        [field: SerializeField, Range(0f, 1f)] public float FromAttackToRoarChance { get; private set; } = 0.25f;
        [field: SerializeField, Range(0f, 1f)] public float FromAttackToEvadeChance { get; private set; } = 0.2f;
        [field: SerializeField, Range(0f, 1f)] public float FromAttackToLeftHandAttackChance { get; private set; } = 0.1f;
        [field: SerializeField, Range(0f, 1f)] public float FromAttackToRightHandAttackChance { get; private set; } = 0.1f;
        [field: SerializeField, Range(0f, 1f)] public float FromJumpAttackToRightHandAttackChance { get; private set; } = 0.3f;
        [field: SerializeField, Range(0f, 1f)] public float FromJumpAttackToLeftHandAttackChance { get; private set; } = 0.4f;
        [field: SerializeField, Range(0f, 1f)] public float FromJumpAttackToChaseChance { get; private set; } = 0.2f;
        [field: SerializeField, Range(0f, 1f)] public float FromJumpAttackToJumpAttackChance { get; private set; } = 0.1f;

        [field: Header("State Max Active Durations")]
        [field: SerializeField] public float ChaseMaxDuration { get; private set; } = 3f;
        [field: SerializeField] public float EvadeMaxDuration { get; private set; } = 3f;










    }


}




