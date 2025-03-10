using UnityEngine;

public class WaterSpirithar : Spirithar {
    void Awake() {
        elementType = ElementType.Water;
        Initialize();
    }

    public override void PerformMove(SpiritharMove spiritharMove, Spirithar target) {
        if (spiritharMove == null) {
            Debug.LogWarning("Movimiento no asignado en " + spiritharName);
            return;
        }
        PerformingMove = true;
        switch (spiritharMove.moveType) {
            case MoveType.Attack:
                Debug.Log(spiritharName + " ataca a " + target.spiritharName + " con " + spiritharMove.moveName);
                StartCoroutine(SpiritharAttack(target, spiritharMove));
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
