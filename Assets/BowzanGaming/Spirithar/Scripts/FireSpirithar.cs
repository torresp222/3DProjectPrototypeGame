using UnityEngine;

public class FireSpirithar : Spirithar {
    void Awake() {
        elementType = ElementType.Fire;
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
                StartCoroutine(Defend(spiritharMove));
                break;
            case MoveType.Boost:
                StartCoroutine(BoostAttack(spiritharMove));
                break;
        }
    }

    public override bool IsWeak(SpiritharMove spiritharMove) {
        return spiritharMove.moveElementType == ElementType.Water;
        /*if(spiritharMove.moveElementType != ElementType.Water)
            return false;
        return true;*/
    }
}
