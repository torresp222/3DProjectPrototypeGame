using BowzanGaming.FinalCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static PlayerTeamTracker;

public class HealthBoosterManager : MonoBehaviour
{
    [Header("Player references")]
    public Transform Player;
    public LayerMask WhatIsPlayer;

    private PlayerActionsInput _playerActionsInput;
    private PlayerTeamTracker _playerTeamTracker;
    private bool _isHealthBoosting;
    // States
    public float HealthRange;
    public bool PlayerInHealthRange;

    private void Awake() {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerActionsInput = Player.GetComponent<PlayerActionsInput>();
        _playerTeamTracker = Player.GetComponent<PlayerTeamTracker>();
        _isHealthBoosting = false;
    }

    private void Update() {
        PlayerInHealthRange = Physics.CheckSphere(transform.position, HealthRange, WhatIsPlayer);

        if (PlayerInHealthRange && _playerActionsInput.GatherPressed && !_isHealthBoosting) {
            _isHealthBoosting = true;
            CureSpiritharInTeam();
        }

        if(!PlayerInHealthRange)
            _isHealthBoosting = false;
    }

    public void CureSpiritharInTeam() {
        int index = 0;
        foreach (var slotKey in _playerTeamTracker.TrackedSlots) {
            if (_playerTeamTracker.SpiritharStatsTracker.TryGetValue(slotKey, out SpiritharData currentData)) {
                if (currentData.TrackCurrentHealth < currentData.TrackBaseStat.baseHealth && currentData.IsTracked) {
                    _playerTeamTracker.UpdateSpiritharHealthTeamTracked(currentData.TrackBaseStat.baseHealth, index);
                    /*currentData.TrackCurrentHealth = currentData.TrackBaseStat.baseHealth;
                    _playerTeamTracker.RefreshSerializedData();*/
                    Debug.Log($"{slotKey} CURADO su vida actual es {currentData.TrackCurrentHealth}");
                }
            }
            index ++;
        }

    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, HealthRange);

    }
}
