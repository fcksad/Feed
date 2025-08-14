using UnityEngine;

[CreateAssetMenu(fileName = "EntityBrainConfig", menuName = "Configs/Entity/EntityBrainConfig")]
public class EntityBrainConfig : ScriptableObject
{
    [Header("Combat // Бой")]
    [field: SerializeField] public bool Aggressive { get; private set; } = false; // Агрессивный
    [field: SerializeField] public bool UseRangedWeapon { get; private set; } = false; // Использует дальнобойное оружие
    [field: SerializeField] public bool CanRetreatWhenLowHP { get; private set; } = true; // Отступает при низком здоровье
    [field: SerializeField] public bool HideIsLowHP { get; private set; } = false; // Прячется при низком здоровье

    [Header("Patrol & Movement // Патрулирование и движение")]
    [field: SerializeField] public bool PatrolWhenIdle { get; private set; } = true; // Патрулирует, когда бездействует
    [field: SerializeField] public bool CanOpenDoors { get; private set; } = false; // Может открывать двери
    [field: SerializeField] public bool AvoidObstacles { get; private set; } = true; // Обходит препятствия

    [Header("Awareness // Восприятие")]
    [field: SerializeField] public bool ReactsToNoise { get; private set; } = true; // Реагирует на шум  //mb
    [field: SerializeField] public bool ReactsToLight { get; private set; } = false; // Реагирует на свет //mb
    [field: SerializeField] public float SightRange { get; private set; } = 10f; // Дальность зрения
    [field: SerializeField] public float HearingRange { get; private set; } = 7f; // Дальность слуха // mb

    [Header("Other // Другое")]
    [field: SerializeField] public bool Friendly { get; private set; } = false; // Дружелюбный
    [field: SerializeField] public bool IsBoss { get; private set; } = false; // Является боссом
}
