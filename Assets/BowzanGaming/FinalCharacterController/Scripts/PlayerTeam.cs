// PlayerTeam.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para Enumerable.Repeat

public class PlayerTeam : MonoBehaviour {
    [Header("Team Configuration")]
    public int maxTeamSize = 3;
    public List<Spirithar> team = new List<Spirithar>();
    private int _activeSpiritharIndex = 0;
    private PlayerTeamTracker _playerTeamTracker;

    private void Awake() {
        _playerTeamTracker = GetComponent<PlayerTeamTracker>();

        int num = maxTeamSize;
        if (team.Count > 0) { 
            num  -= team.Count;
        }
        // Crea 3 elementos null
        team.AddRange(Enumerable.Repeat<Spirithar>(null, num));
       
    }


    // Main Methods
    public bool AddSpirithar(Spirithar newSpirithar) {
        SpiritharBaseStats baseStats = ScriptableObject.CreateInstance<SpiritharBaseStats>();
        for (int i = 0; i < team.Count; i++) {
            if (team[i] == null) {
                team[i] = newSpirithar;
                baseStats = newSpirithar.SetGetFirstStats();
                Debug.Log($"Replaced slot {i} with {newSpirithar.spiritharName}");
                if (_playerTeamTracker.AddNewSpiritharTeamTracked(baseStats, baseStats.baseHealth))
                    Debug.Log("Tracked");
                else {
                    Debug.LogWarning("No se ha podido trackear el spirithar añadido");
                    return false;
                }
                return true;
            }
        }

        if (team.Count >= maxTeamSize) {
            Debug.Log("Team is full");
            return false;
        }

        return false;
    }

    public void SwitchActiveSpirithar(int newIndex) {
        if (newIndex >= 0 && newIndex < team.Count) {
            _activeSpiritharIndex = newIndex;
            Debug.Log($"Switched to _activeSpiritharIndex to {_activeSpiritharIndex}");
        }
    }

    // Getters
    public Spirithar GetActiveSpirithar() {
        int indexHealth;
        if (team.Count == 0) {
            Debug.LogError("No Spirithars in team");
            return null;
        }
        indexHealth = _playerTeamTracker.CheckFirstSpiritharWithHealth();
        if ( indexHealth != _activeSpiritharIndex && _playerTeamTracker.CheckIfSpiritharHasHealth(_activeSpiritharIndex))
        {
            return team[_activeSpiritharIndex];
        } else {
            _activeSpiritharIndex = indexHealth;
        }
        return team[_activeSpiritharIndex];
    }

    public GameObject GetActiveSpiritharPrefab() {
        Spirithar active = GetActiveSpirithar();
        return active != null ? active.gameObject : null;
    }

    public int GetActiveSpiritharIndex() => _activeSpiritharIndex;
    public bool IsTeamFull() => team.Count >= maxTeamSize;
    public int TeamCount() => team.Count;

}