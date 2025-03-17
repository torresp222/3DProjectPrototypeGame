using UnityEngine;

public enum MoveType {
    Attack,
    Defense,
    Boost
}

[CreateAssetMenu(fileName = "NewSpiritharMove", menuName = "Spirithar/Move")]
public class SpiritharMove : ScriptableObject {
    public string moveName;
    public MoveType moveType;
    public ElementType moveElementType;
    public float power;      // Daño para ataques o valor de efecto para otros movimientos
    public float cooldown; // Tiempo de recarga del movimiento

    [Header("Visuals")]
    public GameObject projectilePrefab;  // Projectile prefab to spawn
    /*public GameObject vfx;              // Ability cast VFX
    public GameObject impactVFX;        // Projectile impact VFX*/
    public float projectileSpeed = 10f; // Movement speed of projectile
}
