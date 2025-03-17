// PlayerTeam.cs
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour {
    [Header("Team Configuration")]
    public int maxTeamSize = 3;
    public List<Spirithar> team = new List<Spirithar>();
    private int _activeSpiritharIndex = 0;

    // Main Methods
    public bool AddSpirithar(Spirithar newSpirithar) {
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