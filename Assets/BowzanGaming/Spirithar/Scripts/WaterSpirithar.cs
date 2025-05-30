using UnityEngine;

public class WaterSpirithar : Spirithar {
    void Awake() {
        elementType = ElementType.Water;
        Initialize();
    }

    public override void PerformMove(SpiritharMove spiritharMove, Spirithar target, bool isSpiritharFromTeam = false) {
        if (spiritharMove == null) {
            Debug.LogWarning("Movimiento no asignado en " + spiritharName);
            return;
        }

        PerformingMove = true;
        switch (spiritharMove.moveType) {
            case MoveType.Attack:
                Debug.Log(spiritharName + " ataca a " + target.spiritharName + " con " + spiritharMove.moveName);
                StartCoroutine(SpiritharAttack(target, spiritharMove, isSpiritharFromTeam));
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
        if (spiritharMove.moveElementType != ElementType.Earth)
            return false;
        return true;
    }

}
