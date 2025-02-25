using UnityEngine;

public class FireSpirithar : Spirithar {
    void Awake() {
        elementType = ElementType.Fire;
        Initialize();
    }

    public override void PerformMove(int moveIndex, Spirithar target) {
        if (moves == null || moveIndex >= moves.Length) {
            Debug.LogWarning("Movimiento no asignado en " + spiritharName);
            return;
        }

        SpiritharMove move = moves[moveIndex];

        switch (move.moveType) {
            case MoveType.Attack:
                // Lógica de ataque: podrías aplicar daño y efectos de fuego
                int damage = move.power;
                target.ReceiveDamage(damage);
                Debug.Log(spiritharName + " ataca a " + target.spiritharName + " con " + move.moveName);
                break;
            case MoveType.Defense:
                Defend();
                break;
            case MoveType.Boost:
                BoostAttack();
                break;
        }
    }
}
