using UnityEngine;
using System.Collections.Generic;

public class ZoneSpawner : MonoBehaviour {
    [System.Serializable]
    public class ZoneSettings {
        public Transform zoneCenter;
        public float spawnRadius = 5f;
        public int maxSpirithars = 3;
        public List<GameObject> spiritharPrefabs;
        [Tooltip("Altura desde la que disparar el raycast")]
        public float raycastHeight = 100f;
        [Tooltip("Offset para evitar clipping con el terreno")]
        public float yOffset = 0.2f;
    }

    public List<ZoneSettings> zones;
    [Tooltip("Layer del terreno para filtrar el raycast")]
    public LayerMask terrainLayer;

    void Start() {
        SpawnInAllZones();
    }

    void SpawnInAllZones() {
        foreach (ZoneSettings zone in zones) {
            if (zone.spiritharPrefabs.Count == 0 || zone.zoneCenter == null) {
                Debug.LogWarning("Zona mal configurada!");
                continue;
            }

            for (int i = 0; i < zone.maxSpirithars; i++) {
                Vector2 randomPoint = Random.insideUnitCircle * zone.spawnRadius;
                Vector3 spawnPosition = zone.zoneCenter.position + new Vector3(randomPoint.x, 0, randomPoint.y);

                // Ajuste de altura usando raycast
                Vector3 rayStart = new Vector3(
                    spawnPosition.x,
                    zone.raycastHeight,
                    spawnPosition.z
                );

                // Dentro del bucle de spawn, antes del Instantiate:
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayer)) {
                    spawnPosition.y = hit.point.y + zone.yOffset;
                } else {
                    Debug.LogWarning("No se detectó terreno, usando altura por defecto");
                    spawnPosition.y = Terrain.activeTerrain.SampleHeight(spawnPosition) + zone.yOffset;
                }

                GameObject selectedPrefab = zone.spiritharPrefabs[Random.Range(0, zone.spiritharPrefabs.Count)];
                Instantiate(selectedPrefab, spawnPosition, randomRotation);
            }
        }
    }
}