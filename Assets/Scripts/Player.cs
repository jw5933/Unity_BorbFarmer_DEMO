using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player: MonoBehaviour
{
    public Borb heldBorb = null;
    Plane currentPlane;
    public int totalPoints;
    Vector3 mousePos;
    GameManager gm;

    void Awake(){
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale < 1) return;
        if (Input.GetKeyDown(KeyCode.R)){
            RestartGame();
        }
        else if (Input.GetMouseButtonDown(0)){
            if (heldBorb == null){
                RaycastHit hit = GetObjectInSpace();
                if (hit.collider != null && hit.collider.CompareTag("borb")){
                    heldBorb = hit.transform.gameObject.GetComponent<Borb>();
                    currentPlane = heldBorb.plane;
                    heldBorb.PausePlayTimer();
                }
                
            }
        }
        else if(Input.GetMouseButton(0)){
            if (heldBorb != null){
                UpdateMouseItem();
                if (heldBorb.ValidateDistance()){
                    CalculatePoints(heldBorb.points);
                    heldBorb = null;
                }
                
            }
        }
        else if (Input.GetMouseButtonUp(0)){
            if (heldBorb != null){
                heldBorb.PausePlayTimer();
                heldBorb = null;
            }
        }
    }

    void CalculatePoints(int points){ //if have combos w/ multiplier
        AddPoints(points);
    }

    void AddPoints(int points){
        totalPoints += points;
        gm.ChangePoints(totalPoints);
    }

    void UpdateMouseItem(){
        if (!heldBorb.HasGrown) return;
        //Create a ray from the Mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //move the object to where the mouse is
        float enter = 0.0f;
        // debugging
        if (currentPlane.Raycast(ray, out enter)){
            mousePos = ray.GetPoint(enter);
            Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), mousePos, Color.green);
            if(mousePos.y < heldBorb.initialPosition.y) return;
            heldBorb.transform.position = new Vector3(heldBorb.transform.position.x, mousePos.y, heldBorb.transform.position.z);
        }
    }

    RaycastHit GetObjectInSpace(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        //find point on plane
        if(Physics.Raycast(ray, out hitData)){
            return hitData;
        }
        return hitData;
    }

    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}
