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

        for (int i = 0; i < team.Count; i++) {
            if (team[i] == null) {
                team[i] = newSpirithar;
                Debug.Log($"Replaced slot {i} with {newSpirithar.spiritharName}");
                if (_playerTeamTracker.AddNewSpiritharTeamTracked(newSpirithar.SetGetFirstStats(), newSpirithar.maxHealth))
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
            Debug.Log($"Switched to {GetActiveSpirithar().spiritharName}");
        }
    }

    // Getters
    public Spirithar GetActiveSpirithar() {
        if (team.Count == 0) {
            Debug.LogError("No Spirithars in team");
            return null;
        }
        _activeSpiritharIndex = _playerTeamTracker.CheckFirstSpiritharWithHealth();
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