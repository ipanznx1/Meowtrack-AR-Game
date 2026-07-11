using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    public class ObjectSpawner : MonoBehaviour
    {
        [Header("Rekod Objek")]
        private GameObject kucingYangDihasilkan;

        [Header("Settings")]
        [SerializeField] Camera m_CameraToFace;
        [SerializeField] List<GameObject> m_ObjectPrefabs = new List<GameObject>();

        // Slot untuk template manager (biarkan)
        public List<GameObject> objectPrefabs { get => m_ObjectPrefabs; set => m_ObjectPrefabs = value; }
        public int spawnOptionIndex { get; set; } = 0;
        public event Action<GameObject> objectSpawned;

        void Awake() { if (m_CameraToFace == null) m_CameraToFace = Camera.main; }

        public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
        {
            // KOD DIUBAH: Jika kucing sudah ada, terus sekat (lock) kedudukan dia. Jangan bagi pindah posisi lagi!
            if (kucingYangDihasilkan != null)
            {
                Debug.LogWarning("Kucing sudah ada di skrin! Posisi telah DI-LOCK dan tidak akan dipindahkan.");
                return true; // Return true supaya fungsi berhenti di sini tanpa mengubah transform.position
            }

            if (m_ObjectPrefabs.Count == 0) return false;

            // 2. Spawn terus tanpa fikir panjang (Hanya berjalan sekali sahaja untuk kucing pertama)
            var index = Mathf.Clamp(spawnOptionIndex, 0, m_ObjectPrefabs.Count - 1);
            var newObject = Instantiate(m_ObjectPrefabs[index]);
            kucingYangDihasilkan = newObject;
            newObject.transform.position = spawnPoint;

            // 3. Hadap kamera
            var forward = m_CameraToFace.transform.position - spawnPoint;
            BurstMathUtility.ProjectOnPlane(forward, spawnNormal, out var projectedForward);
            newObject.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);

            objectSpawned?.Invoke(newObject);
            return true;
        }

        public void SpawnObject(Vector3 spawnPoint, Vector3 spawnNormal) => TrySpawnObject(spawnPoint, spawnNormal);
    }
}