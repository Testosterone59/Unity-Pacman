using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameLogic : MonoBehaviour {

    public CinemachineVirtualCamera camera;
    private CinemachineTransposer virtualTransposer;

    public GameObject centerLevelObj;

    public static Player player;

    public static GameObject enemySpawnObj;

    public static bool gameStarted = false;

    public static int levelSize = 10;

    static int enemyCount = 5;

    public const int maxLives = 5;
    public static int level, score, lives = 0;

    public static GameLogic _singleton
    {
        get
        {
            if (singleton == null) { singleton = GetSingleton(); }
            return singleton;
        }
    }
    private static GameLogic singleton;

    static GameLogic GetSingleton() { return FindObjectOfType<GameLogic>(); }

    private void Awake()
    {
        virtualTransposer = camera.GetCinemachineComponent<CinemachineTransposer>();
    }

    public void NewGame()
    {
        score = 0;
        lives = 3;
        level = 1;
        SetNewLevelParameters();
        StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(IStartGame());
    }
    /// <summary>
    /// Подготовка игры
    /// </summary>
    /// <returns></returns>
    IEnumerator IStartGame()
    {
        Generator.GenerateLevel(levelSize);
        ControlUI.ChangeActiveGameHUD(true);
        ControlUI.UpdateInfo();

        AudioManager.PlayAudio(AudioManager._singleton.clipBeginning);

        SetObjToCenter();
        yield return new WaitForSeconds(3);
        CameraSetTarget(player.transform);
        gameStarted = true;

        for (int i = 0; i < enemyCount; i++)
        {
            yield return new WaitForSeconds(2);
            Instantiate(Resources.Load("Prefabs/Ghost"), enemySpawnObj.transform.position, Quaternion.identity);
        }
    }
    /// <summary>
    /// Запуск следующего уровня
    /// </summary>
    static void StartNextLevel()
    {
        level++;
        lives++;
        if (lives > maxLives) { lives = maxLives; }
        ClearLevel();
        gameStarted = false;
        SetNewLevelParameters();
        _singleton.CameraSetTarget(_singleton.centerLevelObj.transform);
        _singleton.StartGame();
    }
    /// <summary>
    /// Применить параметры следующего уровня
    /// </summary>
    static void SetNewLevelParameters()
    {
        levelSize = level + 10;
        if (level < 3) { enemyCount = 2; }
        else if (level < 5) { enemyCount = 3; }
        else { enemyCount = levelSize / 3; }
    }
    /// <summary>
    /// Вызывается при поражении
    /// </summary>
    static void LoseGame()
    {
        ClearLevel();
        gameStarted = false;
        _singleton.CameraSetTarget(_singleton.centerLevelObj.transform);
        ControlUI.ChangeActiveGameHUD(false);
        ControlUI.ChangeActiveMain(true);
    }
    /// <summary>
    /// Тотальная очистка уровня
    /// </summary>
    static void ClearLevel()
    {
        foreach (var inst in Generator.instances) { if (inst != null) { Destroy(inst); } }
        Generator.instances.Clear();
        Enemy.DestroyInstances();
    }

    void CameraSetTarget(Transform target)
    {
        camera.Follow = target;
        camera.LookAt = target;
    }

    void SetObjToCenter()
    {
        centerLevelObj.transform.position = new Vector3(levelSize / 2, 0, levelSize / 2);
    }

    /// <summary>
    /// Вызывается при столкновении игрока с призраком
    /// </summary>
    public static void KillPlayer()
    {
        lives--;
        ControlUI.UpdateInfo();
        AudioManager.PlayAudio(AudioManager._singleton.clipDeath);
        System.Threading.Thread.Sleep(500); /// Пауза, дабы не создавать ради этого отдельный корунтин
        if (lives > 0) { ResetPersons(); }
        else { LoseGame(); }
    }
    /// <summary>
    /// Возврат игрока и призраков на стартовую позицию
    /// </summary>
    public static void ResetPersons()
    {
        player.ResetPerson();
        foreach (var e in Enemy.instances) { e.ResetPerson(); }
    }

    public static void AddScore(int addScore)
    {
        score += addScore;
        ControlUI.UpdateInfo();
        if (Point.instances.Count == 0) { StartNextLevel(); }
    }
}
