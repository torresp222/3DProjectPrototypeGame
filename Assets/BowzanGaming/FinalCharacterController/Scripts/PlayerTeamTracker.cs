using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerTeamTracker : MonoBehaviour {

    private PlayerTeam _playrTeam;
    [System.Serializable]
    public struct SpiritharData {
        public float TrackCurrentHealth;
        public SpiritharBaseStats TrackBaseStat;
        public bool IsTracked;
    }

    public Dictionary<string, SpiritharData> SpiritharStatsTracker = new Dictionary<string, SpiritharData>()
    {
        {"spiritharOne", new SpiritharData()},
        {"spiritharTwo", new SpiritharData()},
        {"spiritharThree", new SpiritharData()}
    };

    private void Awake() {
        _playrTeam = GetComponent<PlayerTeam>();

        var defaultStats = ScriptableObject.CreateInstance<SpiritharBaseStats>();
        SpiritharData data = new() {
            TrackCurrentHealth = 0f,
            TrackBaseStat = defaultStats,
            IsTracked = false
        };
        SpiritharStatsTracker["spiritharOne"] = data;
        SpiritharStatsTracker["spiritharTwo"] = data;
        SpiritharStatsTracker["spiritharThree"] = data;

        /*foreach (var key in spiritharStats.Keys) {
            //SpiritharData data = spiritharStats[key];
            SpiritharData data = new();
            data.TrackCurrentHealth = 0f;
            data.TrackBaseStat = defaultStats;
            data.IsTracked = false;
            spiritharStats[key] = data;
        }*/
    }

    public bool AddNewSpiritharTeamTracked(SpiritharBaseStats newStats, float initialHealth) {
        // Orden de prioridad para los slots
        string[] prioritySlots = { "spiritharOne", "spiritharTwo", "spiritharThree" };

        foreach (var slotKey in prioritySlots) {
            if (SpiritharStatsTracker.TryGetValue(slotKey, out SpiritharData currentData)) {
                if (!currentData.IsTracked) {
                    SpiritharData newEntry = new SpiritharData {
                        TrackCurrentHealth = initialHealth,
                        TrackBaseStat = newStats,
                        IsTracked = true
                    };

                    SpiritharStatsTracker[slotKey] = newEntry;
                    Debug.Log($"Nuevo Spirithar trackeado en slot: {slotKey} y {SpiritharStatsTracker[slotKey].TrackCurrentHealth}");
                    return true;
                }
            }
        }

        Debug.LogError("No hay slots disponibles para tracking");
        return false;
    }

    public bool UpdateSpiritharHealthTeamTracked(float newHealth, int index) {
        // Orden de prioridad para los slots
        string[] trackedSlots = { "spiritharOne", "spiritharTwo", "spiritharThree" };

        if (trackedSlots[index]== null) {
            Debug.LogError("No hay slots disponibles para tracking");
            return false;
        }

        SpiritharData data = SpiritharStatsTracker[trackedSlots[index]];
        data.TrackCurrentHealth = newHealth;
        SpiritharStatsTracker[trackedSlots[index]] = data;
        Debug.Log($"Spirithar trackeado en slot: {trackedSlots[index]} , vida actual {SpiritharStatsTracker[trackedSlots[index]].TrackCurrentHealth}");
        return true;


       
    }

}
