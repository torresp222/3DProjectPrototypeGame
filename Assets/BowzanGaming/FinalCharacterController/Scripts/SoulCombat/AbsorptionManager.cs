// AbsorptionManager.cs
using System.Collections.Generic;
using UnityEngine;

public class AbsorptionManager : MonoBehaviour {
    [Header("Dependencies")]
    [SerializeField] private PlayerTeam _playerTeam;

    private Spirithar _currentAbsorbedSpirithar;

    public int GetCurrenAbsorbSpiritharIndex { get; private set; }

    public ElementType CurrentElement => _currentAbsorbedSpirithar?.elementType ?? ElementType.None;

    private void Awake() {
        if (_playerTeam == null) _playerTeam = GetComponent<PlayerTeam>();
    }

    public void AbsorbSpirithar(Spirithar spirithar) {
        if (spirithar == null) return;

        _currentAbsorbedSpirithar = spirithar;
        ApplyElementalEffects();
        Debug.Log($"Absorbed {spirithar.spiritharName}");
    }

    public void AbsorbTeamSpirithar(int teamIndex) {
        if (teamIndex < 0 || teamIndex >= _playerTeam.TeamCount()) return;

        GetCurrenAbsorbSpiritharIndex = teamIndex;
        _playerTeam.SwitchActiveSpirithar(teamIndex);
        AbsorbSpirithar(_playerTeam.GetActiveSpirithar());
    }

    private void ApplyElementalEffects() {
        // Implementation for visual/particle effects
        Debug.Log("Aparecen efectos especialees");
    }

    public SpiritharMove GetCurrentMove(int moveIndex) {
        if (_currentAbsorbedSpirithar == null ||
           moveIndex < 0 ||
           moveIndex >= _currentAbsorbedSpirithar.moves.Length)
            return null;

        return _currentAbsorbedSpirithar.moves[moveIndex];
    }

    public void SetAbsorbedSpiritharToNone() => _currentAbsorbedSpirithar = null;
}