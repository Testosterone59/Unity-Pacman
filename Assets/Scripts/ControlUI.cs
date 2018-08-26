using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour {

    public GameObject objHUD;
    public GameObject objMain;

    public Text textInformation;

    public static ControlUI _singleton
    {
        get
        {
            if (singleton == null) { singleton = GetSingleton(); }
            return singleton;
        }
    }
    private static ControlUI singleton;

    static ControlUI GetSingleton() { return FindObjectOfType<ControlUI>(); }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) { Application.Quit(); }
        if (objMain.activeSelf && Input.GetKeyUp(KeyCode.Return)) { GameLogic._singleton.NewGame(); ChangeActiveMain(false); }
    }

    public static void UpdateInfo()
    {
        string text = $"Score : {GameLogic.score}\nLives : {GameLogic.lives}\n\nPoints : {Point.instances.Count}\nLevel : {GameLogic.level}";
        _singleton.textInformation.text = text;
    }

    public static void ChangeActiveGameHUD(bool enable) { _singleton.objHUD.SetActive(enable); }

    public static void ChangeActiveMain(bool enable) { _singleton.objMain.SetActive(enable); }

}
