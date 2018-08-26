using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {

    public int score = 10;

    public bool energizer = false;

    public static List<Point> instances = new List<Point>();

    private void Awake()
    {
        instances.Add(this);
    }

    private void OnDestroy()
    {
        if (instances.Contains(this)) { instances.Remove(this); }
    }

    public void Take()
    {
        instances.Remove(this);
        GameLogic.AddScore(score);
        if (energizer)
        {
            foreach (var e in Enemy.instances)
            {
                if (e.state == StateEnemy.Death) { continue; }
                e.ChangeState(StateEnemy.Afraid);
            }
        }
        Destroy(gameObject);
        AudioManager.PlayAudio(AudioManager._singleton.clipChomp);
    }
}
