using UnityEngine;

public class PointTopologyFixer : MonoBehaviour
{
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            // Kalau kucing guna SkinnedMeshRenderer (ada animasi)
            SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                Mesh mesh = smr.sharedMesh;
                int[] indices = new int[mesh.vertexCount];
                for (int i = 0; i < mesh.vertexCount; i++) indices[i] = i;
                mesh.SetIndices(indices, MeshTopology.Points, 0);
            }
        }
        else
        {
            // Kalau kucing biasa (MeshFilter)
            Mesh mesh = mf.sharedMesh;
            int[] indices = new int[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; i++) indices[i] = i;
            mesh.SetIndices(indices, MeshTopology.Points, 0);
        }
    }
}