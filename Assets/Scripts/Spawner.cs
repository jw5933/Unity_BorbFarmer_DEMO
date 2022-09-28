using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    Plane spawnerPlane;
    int myValue;
    public int value {set{myValue = value;}}
    [SerializeField] PlaneScriptableObject planeObj;

    void Awake()
    {
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, position.y, planeObj.planeZ.z);
        spawnerPlane = planeObj.plane;
    }

    public void Spawn(Borb borb){
        borb.HandleSpawned(transform.position, myValue, spawnerPlane);
    }
}
