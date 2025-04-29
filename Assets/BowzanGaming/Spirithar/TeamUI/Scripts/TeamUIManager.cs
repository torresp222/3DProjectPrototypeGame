using UnityEngine;

public class TeamUIManager : MonoBehaviour {
    [SerializeField] private UISpiritharSlot[] slots;
    [SerializeField] private PlayerTeamTracker teamTracker;

    private void OnEnable() {
        PlayerTeamTracker.OnSpiritharAdded += UpdateSlot;
        PlayerTeamTracker.OnSpiritharHealthUpdated += UpdateHealth;
    }

    private void OnDisable() {
        PlayerTeamTracker.OnSpiritharAdded -= UpdateSlot;
        PlayerTeamTracker.OnSpiritharHealthUpdated -= UpdateHealth;
    }

    private void Start() {
        InitializeUI();
    }

    void InitializeUI() {
        // Obtener slots automáticamente si son hijos directos
        if (slots.Length == 0) {
            slots = GetComponentsInChildren<UISpiritharSlot>();
        }

        // Sincronizar con datos iniciales
        for (int i = 0; i < slots.Length; i++) {
            if (teamTracker.SpiritharStatsTracker.TryGetValue(teamTracker.TrackedSlots[i],
                out PlayerTeamTracker.SpiritharData data)) {
                if (data.IsTracked) {
                    slots[i].UpdateSlot(data.TrackSpiritharName,
                                      data.TrackCurrentHealth,
                                      data.TrackMaxHealth);
                } else {
                    slots[i].ClearSlot();
                }
            }
        }
    }

    void UpdateSlot(int slotIndex) {
        var data = teamTracker.SpiritharStatsTracker[teamTracker.TrackedSlots[slotIndex]];
        slots[slotIndex].UpdateSlot(data.TrackSpiritharName,
                                  data.TrackCurrentHealth,
                                  data.TrackMaxHealth);
    }

    void UpdateHealth(int slotIndex, float newHealth) {
        var data = teamTracker.SpiritharStatsTracker[teamTracker.TrackedSlots[slotIndex]];
        slots[slotIndex].UpdateSlot(data.TrackSpiritharName,
                                  newHealth,
                                  data.TrackMaxHealth);
    }
}