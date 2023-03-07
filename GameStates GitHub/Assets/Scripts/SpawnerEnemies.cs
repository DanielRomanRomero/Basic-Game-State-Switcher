using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerEnemies : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemiesInThisLevel,spawnDelay; //Set this from inspector to change the number of enemies you'll have to defeat and change the delay between spawn.
    private Transform positionSpawn;
    private int spawnedEnemies;
    public static int enemiesDefeat;
    private Text textEnemiesToWin,textEnemiesDefeated;

    private void Awake()
    {
        enemiesDefeat = 0;
        spawnedEnemies = 0;
    }

    private void Start()
    {
        textEnemiesToWin = GameObject.Find("Text Enemies To Win").GetComponent<Text>();
        textEnemiesDefeated = GameObject.Find("Text Enemies Defeated").GetComponent<Text>();
        positionSpawn = GameObject.Find("PositionSpawn").GetComponent<Transform>();
        StartCoroutine("SpawnEnemies");
    }

    private void Update()
    {  
        //I know is not the best practice to use if statements inside another if, but for this example code is enough, if you want to use it, you can use a switch statement instead.
        if (GameManager.sharedInstance.currentGameState== GameState.inGame)
        {
            if (enemiesDefeat == enemiesInThisLevel)
            {
                GameManager.gameWon = true;
            }
        }

        textEnemiesToWin.text = "Enemies to win:  " + enemiesInThisLevel.ToString();
        textEnemiesDefeated.text = "Enemies defeated: " + enemiesDefeat.ToString();
    }

    IEnumerator SpawnEnemies()
    {
        while (spawnedEnemies <  enemiesInThisLevel)
        {
            Instantiate(enemyPrefab, positionSpawn.position, Quaternion.identity);
            spawnedEnemies++;
            var randomPosition = Random.Range(0, 2);

            if (randomPosition == 0)
            {
                transform.position = new Vector2(0f, 0f);
            }
            else
            {
                transform.position = new Vector2(0f, -4f);
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
