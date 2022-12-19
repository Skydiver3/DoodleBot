using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    
    //Screen
    [Header("Screen")] 
    [SerializeField] private Animator heartAnimator;
    [SerializeField] private Image heartImage;
    [SerializeField] private Image heartImage2;
    [SerializeField] private Transform leftBottom;
    [SerializeField] private Transform rightTop;
    public static float screenWidth;
    public static float screenHeight;

    //PowerUp
    [Header("PowerUps")]
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private float spawnTimeMin;
    [SerializeField] private float spawnTimeMax;
    private bool powerUpSpawned = true;
    private Coroutine _spawnPowerUp;

    //Asteroids
    [Header("Asteroids")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private int maxNumAsteroids;
    [SerializeField] private float asteroidSpawnTimeMax;
    [SerializeField] private List<GameObject> asteroidSprites = new List<GameObject>();
    private static List<GameObject> asteroidsToAdd = new List<GameObject>();
    private bool asteroidSpawned = true;
    private Coroutine _spawnAsteroids;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1080, 1080, false, 60);
        
        asteroidsToAdd.Clear();
        screenWidth = rightTop.position.x - leftBottom.position.x;
        screenHeight = rightTop.position.y - leftBottom.position.y;
        for(int i = 0; i < maxNumAsteroids; i++){
            int ranSprite = Random.Range(0, asteroidSprites.Count);
            GameObject asteroid = Instantiate(asteroidSprites[ranSprite]);
            asteroidsToAdd.Add(asteroid);
            asteroidsToAdd[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        BigBot.EndLevel += OnEndLevel;
    }
    private void OnDisable()
    {
        BigBot.EndLevel -= OnEndLevel;
    }

    private void FixedUpdate()
    {
        if(powerUpSpawned){
            _spawnAsteroids=StartCoroutine(SpawnPowerUpAfterSeconds(Random.Range(spawnTimeMin, spawnTimeMax)));
        }  
        if(asteroidSpawned && asteroidsToAdd.Count > 0){
            _spawnPowerUp=StartCoroutine(SpawnAsteroidAfterSeconds(Random.Range(0, asteroidSpawnTimeMax)));
        }
    }

    private void OnEndLevel()
    {
        StopCoroutine(_spawnAsteroids);
        StopCoroutine(_spawnPowerUp);
        StartCoroutine(WaitBeforeLeaveScene());
        heartImage.enabled = true;
        heartImage2.enabled = true;
        heartAnimator.SetTrigger("break");
    }
    
    private IEnumerator WaitBeforeLeaveScene()
    {
        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;
        }

        LoadScene.DoLoadScene(Scenes.GameOver);
    }

    private IEnumerator SpawnAsteroidAfterSeconds(float seconds)
    {
        asteroidSpawned = false;
        yield return new WaitForSeconds(seconds);
        createAsteroid();
        asteroidSpawned = true;
    }

    private void createAsteroid(){
        float xStart = 0;
        float yStart = 0;
        float sH = screenHeight / 2;
        float sW = screenWidth / 2;
        switch (Random.Range(0, 4)) {
            case 0://UP
                yStart = sH + 1f;
                xStart = Random.Range(-sW, sW);
                break;
            case 1://DOWN
                yStart = -sH -1f;
                xStart = Random.Range(-sW, sW);
                break;
            case 2://LEFT
                yStart = Random.Range(-sH, sH);
                xStart = -sW - 1f;
                break;
            case 3://RIGHT
                yStart = Random.Range(-sH, sH);
                xStart = sW + 1f;
                break;
            default:
                break;
        }
        float offsetX = Random.Range(-1f, 1f);
        float offsetY = Random.Range(-1f, 1f);
        float direction = Mathf.Atan2(-yStart + offsetY * 6, -xStart + offsetX * 6);
        GameObject asteroid = asteroidsToAdd[0];
        asteroidsToAdd.RemoveAt(0);
        asteroid.SetActive(true);
        asteroid.transform.position = new Vector3(xStart, yStart, -1);
        asteroid.transform.right = new Vector3(Mathf.Cos(direction), Mathf.Sin(direction), 0);
        
    }

    private IEnumerator SpawnPowerUpAfterSeconds(float seconds)
    {
        powerUpSpawned = false;
        yield return new WaitForSeconds(seconds);
        GameObject powerUp = Instantiate(powerUpPrefab);
        float xpos = Random.Range(leftBottom.position.x * 0.9f, rightTop.position.x * 0.9f);
        float ypos = Random.Range(leftBottom.position.y * 0.9f, rightTop.position.y * 0.9f);
        powerUp.transform.position = new Vector3(xpos, ypos, 0);
        powerUpSpawned = true;
    }

    public static void destroyAsteroid(GameObject asteroid){
        asteroid.SetActive(false);
        asteroidsToAdd.Add(asteroid);
    }

    public static float getWidth(){
        return screenWidth;
    }

    public static float getHeight(){
        return screenHeight;
    }
}
