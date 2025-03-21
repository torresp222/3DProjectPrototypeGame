// PlayerTeam.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para Enumerable.Repeat

public class PlayerTeam : MonoBehaviour {
    [Header("Team Configuration")]
    public int maxTeamSize = 3;
    public List<Spirithar> team = new List<Spirithar>();
    private int _activeSpiritharIndex = 0;

    private void Awake() {
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
                return true;
            }
        }

        if (team.Count >= maxTeamSize) {
            Debug.Log("Team is full");
            return false;
        }

        team.Add(newSpirithar);
        Debug.Log($"Added {newSpirithar.spiritharName} to team");
        return true;
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