using UnityEngine;

public class WaterSpirithar : Spirithar {
    void Awake() {
        elementType = ElementType.Water;
        Initialize();
    }

    public override void PerformMove(SpiritharMove spiritharMove, int moveIndex, Spirithar target) {
        if (moves == null || moveIndex >= moves.Length) {
            Debug.LogWarning("Movimiento no asignado en " + spiritharName);
            return;
        }

        //SpiritharMove move = moves[moveIndex];

        switch (spiritharMove.moveType) {
            case MoveType.Attack:
                /*int damage = spiritharMove.power;
                target.ReceiveDamage(damage);*/
                Debug.Log(spiritharName + " ataca a " + target.spiritharName + " con " + spiritharMove.moveName);
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
