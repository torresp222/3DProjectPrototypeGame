using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerTeamTracker : MonoBehaviour {

    public static event Action<int> OnSpiritharAdded; // Índice del slot
    public static event Action<int, float> OnSpiritharHealthUpdated; // Índice y nueva salud

    [SerializeField]
    private List<SerializableSpiritharData> _serializedData = new List<SerializableSpiritharData> {
        new SerializableSpiritharData { key = "spiritharOne" },
        new SerializableSpiritharData { key = "spiritharTwo" },
        new SerializableSpiritharData { key = "spiritharThree" }
    };

    private PlayerTeam _playrTeam;
    [System.Serializable]
    public struct SpiritharData {
        public string TrackSpiritharName;
        public float TrackMaxHealth;
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

    public string[] TrackedSlots = { "spiritharOne", "spiritharTwo", "spiritharThree" };

    private void Awake() {
        _playrTeam = GetComponent<PlayerTeam>();

        var defaultStats = ScriptableObject.CreateInstance<SpiritharBaseStats>();
        SpiritharData data = new() {
            TrackSpiritharName = "",
            TrackMaxHealth = 0f,
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
                trackSpiritharName = kvp.Value.TrackSpiritharName,
                trackMaxHealth = kvp.Value.TrackMaxHealth,
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
                    trackSpiritharName = spiritharDataTracker.TrackSpiritharName,
                    trackMaxHealth = spiritharDataTracker.TrackMaxHealth,
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
                trackSpiritharName = spiritharDataTracker.TrackSpiritharName,
                trackMaxHealth = spiritharDataTracker.TrackMaxHealth,
                trackCurrentHealth = spiritharDataTracker.TrackCurrentHealth,
                trackBaseStat = spiritharDataTracker.TrackBaseStat,
                isTracked = spiritharDataTracker.IsTracked
            });

            Debug.Log($"No Slot of spirithar team track {slotKey} so added new ONE");
        }


    }

    public bool AddNewSpiritharTeamTracked(SpiritharBaseStats newStats, float initialHealth, string spiritharName) {

        foreach (var slotKey in TrackedSlots) {
            if (SpiritharStatsTracker.TryGetValue(slotKey, out SpiritharData currentData)) {
                if (!currentData.IsTracked) {
                    SpiritharData newEntry = new SpiritharData {
                        TrackSpiritharName = spiritharName,
                        TrackMaxHealth = initialHealth,
                        TrackCurrentHealth = initialHealth,
                        TrackBaseStat = newStats,
                        IsTracked = true
                    };

                    SpiritharStatsTracker[slotKey] = newEntry;
                    Debug.Log($"Nuevo Spirithar trackeado en slot: {slotKey} y {SpiritharStatsTracker[slotKey].TrackCurrentHealth}");
                    RefreshWhenAddSerializedData(SpiritharStatsTracker[slotKey], slotKey);
                    int slotIndex = Array.IndexOf(TrackedSlots, slotKey);
                    OnSpiritharAdded?.Invoke(slotIndex);
                    return true;
                }
            }
        }

        Debug.LogError("No hay slots disponibles para tracking");
        return false;
    }

    public bool UpdateSpiritharHealthTeamTracked(float newHealth, int index) {

        if (TrackedSlots[index]== null) {
            Debug.LogError("No hay slots disponibles para tracking");
            return false;
        }

        SpiritharData data = SpiritharStatsTracker[TrackedSlots[index]];
        data.TrackCurrentHealth = newHealth;
        SpiritharStatsTracker[TrackedSlots[index]] = data;
        Debug.Log($"Spirithar trackeado en slot: {TrackedSlots[index]} , vida actual {SpiritharStatsTracker[TrackedSlots[index]].TrackCurrentHealth}");

        // TRACK HERE THE NEW LIST FOR INSPECTOR VIEW
        // HERE -----
        RefreshSerializedData();
        OnSpiritharHealthUpdated?.Invoke(index, newHealth);
        return true;


       
    }

    public int CheckFirstSpiritharWithHealth() {

        int index = 0;

        foreach (var slotKey in TrackedSlots) {
            if (SpiritharStatsTracker.TryGetValue(slotKey, out SpiritharData currentData)) {
                if (currentData.TrackCurrentHealth > 0) {
                    return index;
                }
            }
            index += 1;

        }

        return -1;
    }

    public bool CheckIfSpiritharHasHealth(int indexSpirithar) {
        int index = 0;
        foreach (var slotKey in TrackedSlots) {
            if (SpiritharStatsTracker.TryGetValue(slotKey, out SpiritharData currentData) ) {
                if (currentData.TrackCurrentHealth > 0 && index == indexSpirithar) {
                    return true;
                }
            }
            index += 1;

        }

        return false;

    }

}

[System.Serializable]
public class SerializableSpiritharData {
    public string key;
    public string trackSpiritharName;
    public float trackMaxHealth;
    public float trackCurrentHealth;
    public SpiritharBaseStats trackBaseStat;
    public bool isTracked;
}
