using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerTeamTracker : MonoBehaviour {
    [SerializeField]
    private List<SerializableSpiritharData> _serializedData = new List<SerializableSpiritharData> {
        new SerializableSpiritharData { key = "spiritharOne" },
        new SerializableSpiritharData { key = "spiritharTwo" },
        new SerializableSpiritharData { key = "spiritharThree" }
    };

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
    }

    public void RefreshSerializedData() {
        _serializedData.Clear();

        foreach (var kvp in SpiritharStatsTracker) {
            _serializedData.Add(new SerializableSpiritharData {
                key = kvp.Key,
                trackCurrentHealth = kvp.Value.TrackCurrentHealth,
                trackBaseStat = kvp.Value.TrackBaseStat,
                isTracked = kvp.Value.IsTracked
            });
        }
    }

    public void RefreshWhenAddSerializedData(SpiritharData spiritharDataTracker, string slotKey) {
        bool found = false;


        for (int i = 0; i < _serializedData.Count; i++) {
            if (_serializedData[i].key == slotKey) {
                _serializedData[i] = new SerializableSpiritharData {
                    key = slotKey,
                    trackCurrentHealth = spiritharDataTracker.TrackCurrentHealth,
                    trackBaseStat = spiritharDataTracker.TrackBaseStat,
                    isTracked = spiritharDataTracker.IsTracked
                };
                Debug.Log("Added Spirithar Gathered in List of track team spirithar to see in Inspector");
                found = true;
                break;
            }
        }

        if (!found) {
            _serializedData.Add(new SerializableSpiritharData {
                key = slotKey,
                trackCurrentHealth = spiritharDataTracker.TrackCurrentHealth,
                trackBaseStat = spiritharDataTracker.TrackBaseStat,
                isTracked = spiritharDataTracker.IsTracked
            });

            Debug.Log($"No Slot of spirithar team track {slotKey} so added new ONE");
        }


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
                    RefreshWhenAddSerializedData(SpiritharStatsTracker[slotKey], slotKey);
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

        // TRACK HERE THE NEW LIST FOR INSPECTOR VIEW
        // HERE -----
        RefreshSerializedData();

        return true;


       
    }

}

[System.Serializable]
public class SerializableSpiritharData {
    public string key;
    public float trackCurrentHealth;
    public SpiritharBaseStats trackBaseStat;
    public bool isTracked;
}
