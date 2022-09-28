using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] float timePerGame = 120; //(seconds)
    private float time;

    //there can be fewer borbs than spawners
    [SerializeField] Spawner[] spawners;
    [SerializeField] List<int> freeList = new List<int>();
    Queue<Borb> freeBorbs = new Queue<Borb>();
    private List<Borb> borbs = new List<Borb>();

    [SerializeField] GameObject playButton;
    [SerializeField] GameObject score;
    [SerializeField] GameObject pauseButton;

    int maxBorbs = 2;
    int numBorbs;
    int borbsInScene = 0;

    Player player;
    [SerializeField]TMP_Text pointsText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject loading;

    void Awake(){
        //get all spawners
        spawners = (Spawner[])FindObjectsOfType(typeof(Spawner));
        for (int i = 0; i < spawners.Length; i++){
            spawners[i].value = i;
        }
        player = FindObjectOfType<Player>();
    }

    public void InitializeBorb(Borb borb){
        numBorbs++;
        //freeBorbs.Enqueue(borb);
        borbs.Add(borb);
        borb.gameObject.SetActive(false);

        if (numBorbs >= spawners.Length){
            loading.SetActive(false);
            ShowHidePlayButton();
        }
    }

    public void ChangePoints(int points){
        pointsText.text = points +"";
    }

    void ShowHidePlayButton()
    {
        playButton.SetActive(!playButton.activeSelf);
    }

    void ShowHidePauseButton(){
        pauseButton.SetActive(!pauseButton.activeSelf);
    }

    void ShowHideScore()
    {
        score.SetActive(!score.activeSelf);
    }

    Spawner PickSpawner() {
        //pick a free spawner, return null if none are free
        if (freeList.Count < 1) return null;
        int n = Random.Range(0, freeList.Count);
        int s = freeList[n];
        freeList.RemoveAt(n);
        //Debug.Log(spawners[s].name);
        return spawners[s];
    }

    public void Free(Borb borb, int spawnerIndex){
        freeBorbs.Enqueue(borb);
        borbsInScene--;
        freeList.Add(spawnerIndex);
        StartCoroutine(SetNextSpawn());
    }

    void Generate(Borb borb){
        if (borbsInScene >= maxBorbs){
            freeBorbs.Enqueue(borb);
            return;
        }
        Spawner spawner = PickSpawner();
        //instantiate
        spawner.Spawn(borb);
        borbsInScene++;
        StartCoroutine(SetNextSpawn());
    }

    IEnumerator SetNextSpawn(bool set = false)
    {
        if (freeBorbs.Count < 1 || borbsInScene >= maxBorbs) yield break;
        Borb borb = freeBorbs.Dequeue();
        float time = set ? 0.5f : Random.Range(2, 4);
        yield return new WaitForSeconds(time);
        Generate(borb);
    }

    IEnumerator IncrementBorbs(float time){
        yield return new WaitForSeconds(time);
        
        if (maxBorbs < numBorbs){
            maxBorbs++;
            //Debug.Log("max borbs now " + maxBorbs);
            StartCoroutine(IncrementBorbs(time));
        }
        
    }

    public void PausePlayGame(){
        Time.timeScale = (int)Time.timeScale ^ 1;
    }
    
    public void StartGame()
    {
        ResetGame();
        Time.timeScale = 1;
        StartCoroutine(StartGame(timePerGame));
        ShowHidePlayButton();
        ShowHidePauseButton();
        ShowHideScore();
    }
    
    IEnumerator StartGame(float time) {
        //yield return new WaitForSeconds(1);
        StartCoroutine(SetNextSpawn(true));
        StartCoroutine(IncrementBorbs(time/(numBorbs-maxBorbs)));
        timeText.text = "Time left: " + time;
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time -= 1;
            timeText.text = "Time left: " + time;
        }
        EndGame();
    }

    void EndGame(){
        ShowHidePauseButton();
        Time.timeScale = 0;
        ShowEndScreen();
    }

    void ShowEndScreen(){
        ShowHidePlayButton();
    }

    void ResetGame()
    {
        score.gameObject.SetActive(false);
        StopAllCoroutines();
        freeBorbs.Clear();
        foreach (Borb borb in borbs)
        {
            borb.gameObject.SetActive(false);
            freeBorbs.Enqueue(borb);
        }
        freeList.Clear();
        for (int i = 0; i < spawners.Length; i++)
        {
            freeList.Add(i);
        }
        time = timePerGame;
        player.totalPoints = 0;
        ChangePoints(0);
    }
}
