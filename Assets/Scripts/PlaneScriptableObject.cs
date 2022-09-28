using UnityEngine;

[CreateAssetMenu(fileName = "PlaneScriptableObject", menuName = "ScriptableObjects/PlaneScriptableObject.cs/PlaneScriptableObject", order = 0)]
public class PlaneScriptableObject : ScriptableObject
{
    public Vector3 planeZ;
    public Plane plane;

    private void OnValidate(){
        plane = new Plane(Vector3.forward, planeZ);
        // debugging
        Debug.DrawLine(Vector3.right + planeZ, planeZ, Color.red, 100f);
        Debug.DrawLine(Vector3.up + planeZ, planeZ, Color.green, 100f);
        Debug.DrawLine(Vector3.forward + planeZ, planeZ, Color.blue, 100f);
    }
}
