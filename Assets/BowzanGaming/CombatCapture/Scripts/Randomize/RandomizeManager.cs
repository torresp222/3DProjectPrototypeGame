using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RandomizeManager : MonoBehaviour
{
    public static RandomizeManager Instance;

    private void Awake() {
        // Singleton pattern for global access.
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
            Destroy(gameObject);
    }

    public SpiritharMove GetRandomMoveFrom(Spirithar spirithar) {

        // Create list of moves by categories
        List<SpiritharMove> attackMoves = new List<SpiritharMove>();
        List<SpiritharMove> defenseMoves = new List<SpiritharMove>();
        List<SpiritharMove> boostMoves = new List<SpiritharMove>();

        foreach (SpiritharMove move in spirithar.moves) {
            switch (move.moveType) {
                case MoveType.Attack:
                    attackMoves.Add(move);
                    break;
                case MoveType.Defense:
                    defenseMoves.Add(move);
                    break;
                case MoveType.Boost:
                    boostMoves.Add(move);
                    break;
            }
        }

        // Calculate dynamic weigths
        float attackWeight = attackMoves.Count > 0 ? 0.6f : 0f;
        float defenseWeight = defenseMoves.Count > 0 ? 0.2f : 0f;
        float boostWeight = boostMoves.Count > 0 ? 0.2f : 0f;

        float totalWeight = attackWeight + defenseWeight + boostWeight;

        // Control if there is no abilities
        if (totalWeight == 0) {
            Debug.LogError("El enemigo no tiene movimientos disponibles");
            return null;
        }

        float randomValue = Random.Range(0f, totalWeight);
        SpiritharMove selectedMove = null;

        // Selection by weighted probability
        if (randomValue <= attackWeight && attackMoves.Count > 0) {
            selectedMove = attackMoves[Random.Range(0, attackMoves.Count)];
        } else if (randomValue <= attackWeight + defenseWeight && defenseMoves.Count > 0) {
            selectedMove = defenseMoves[Random.Range(0, defenseMoves.Count)];
        } else if (boostMoves.Count > 0) {
            selectedMove = boostMoves[Random.Range(0, boostMoves.Count)];
        } else {
            // Fallback: Take any move available
            selectedMove = spirithar.moves[Random.Range(0, spirithar.moves.Length)];
        }

        return selectedMove;

    }
}
