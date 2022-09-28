using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Borb : MonoBehaviour {
    // ==============   variables   ==============
    [SerializeField] int myPoints;
    public int points {get{return myPoints;}}
    
    [SerializeField] float minDistance;
    Vector3 initialPos;
    public Vector3 initialPosition {get{return initialPos;}}
    int mySpawnerNumber;

    [SerializeField] PlaneScriptableObject planeObj;
    Plane myPlane;
    public Plane plane {get{return myPlane;}}
    
    float time;
    [SerializeField] float activeTime;
    IEnumerator myCoroutine;
    UnityAction action;
    bool paused;

    [SerializeField] float speed = 2;

    GameManager gm;
    bool firstLoad = true;

    Collider myCollider;

    bool grown;
    public bool HasGrown{get{return grown;}}

    // ==============   functions   ==============
    void Awake(){
        gm = FindObjectOfType<GameManager>();
        myCollider = GetComponent<Collider>();

        myPlane = planeObj.plane;
        initialPos = transform.position;
        time = activeTime;
        action = HandleTimeout;
    }

    void Update(){
        if (firstLoad && transform.childCount > 0){
            if (transform.GetChild(0).childCount > 0)
            {
                firstLoad = false;
                Handle3DInitialized();
            }
            
        }
        
        if (transform.position != initialPos){
            var step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, initialPos, step);
        }
    }

    public void HandleSpawned(Vector3 pos, int spawner, Plane plane){
        mySpawnerNumber = spawner;
        myPlane = plane;

        ResetVars(pos);
        gameObject.SetActive(true);
        StartTimer();
    }

    public void Handle3DInitialized(){
        transform.GetChild(0).localScale = new Vector3(1.25f, 1.25f, 1.25f);
        gm.InitializeBorb(this);
    }

    public bool ValidateDistance(){
        Vector3 offset = transform.position - initialPos;
        float distance = offset.sqrMagnitude;
        

        if (distance >= minDistance * minDistance){
            HandleDragged();
            return true;
        }
            
        return false;
    }

    private void ResetVars(Vector3 pos){
        paused = false;
        grown = false;
        time = activeTime;
        initialPos = pos;
        transform.position = initialPos;
        myCollider.enabled = true;
    }

    private void HandleDragged(){
        //disable myCollider
        myCollider.enabled = false;
        gameObject.SetActive(false);
        HandleFreed();
    }
    
    private void HandleTimeout(){
        gameObject.SetActive(false);
        HandleFreed();
    }

    private void HandleFreed(){
        //free spawner
        gm.Free(this, mySpawnerNumber);
    }

    //timer functions
    public void StartTimer(){
        grown = true;
        myCoroutine = DecrementTimer();
        StartCoroutine(myCoroutine);
    }

    public void PausePlayTimer(){
        paused = !paused;
    }

    public void StopTimer(){
        if (myCoroutine != null) StopCoroutine(myCoroutine);
    }

    private IEnumerator DecrementTimer(){ //coroutine for timer
        while (time > 0){
            while (paused) yield return null;
            time -= Time.deltaTime;
            yield return null;
        }
        time = 0;
        action();
    }
}

