// VacuumShaders 2014
// https://www.facebook.com/VacuumShaders

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using VacuumShaders.TheAmazingWireframeShader;


[AddComponentMenu("VacuumShaders/The Amazing Wireframe Generator")]
public class TheAmazingWireframeGenerator : MonoBehaviour
{
    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Variables                                                                 //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////

    public WIRE_INSIDE wireDataStoredInside;
    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Unity Functions                                                           //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        Generate(wireDataStoredInside);
    }
    
    //////////////////////////////////////////////////////////////////////////////
    //                                                                          // 
    //Custom Functions                                                          //                
    //                                                                          //               
    //////////////////////////////////////////////////////////////////////////////
    public void Generate(WIRE_INSIDE _wireInside)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.sharedMesh = WireframeManager.GetWire(meshFilter.sharedMesh, _wireInside);
        }
        else
        {
            SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.sharedMesh = WireframeManager.GetWire(skinnedMeshRenderer.sharedMesh, _wireInside);
            }
        }
    }
}

static public class WireframeManager
{
    static Dictionary<int, Mesh> meshes;

    static public Mesh GetWire(Mesh _mesh, WIRE_INSIDE _wireInside)
    {
        if (meshes == null)
            meshes = new Dictionary<int, Mesh>();

        if (meshes.ContainsKey(_mesh.GetInstanceID()) == false)
        {
            Mesh newMesh = WireframeGenerator.Generate(ref _mesh, _wireInside);
            newMesh.name = "WIRE_" + newMesh.name.Replace("(Clone)", string.Empty);
            meshes.Add(_mesh.GetInstanceID(), newMesh);

            return newMesh;
        }
        else
        {
            return meshes[_mesh.GetInstanceID()];
        }
    }

}

