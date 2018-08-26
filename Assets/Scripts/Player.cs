using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BasicPerson {
	
	// Update is called once per frame
	void Update () {
        Keyboard();
	}

    void Keyboard()
    {
        if (Input.GetKey(KeyCode.W)) { SetMove(Direction.Forward); }
        else if (Input.GetKey(KeyCode.S)) { SetMove(Direction.Back); }
        else if (Input.GetKey(KeyCode.A)) { SetMove(Direction.Left); }
        else if (Input.GetKey(KeyCode.D)) { SetMove(Direction.Right); }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.transform.tag)
        {
            case "Point":
                other.transform.GetComponent<Point>().Take();
                break;
            case "Enemy":
                Enemy enemy = other.transform.GetComponent<Enemy>();
                switch (enemy.state)
                {
                    case StateEnemy.Normal: GameLogic.KillPlayer(); break;
                    case StateEnemy.Chase: GameLogic.KillPlayer(); break;
                    case StateEnemy.Afraid: GameLogic.AddScore(300); enemy.ChangeState(StateEnemy.Death); AudioManager.PlayAudio(AudioManager._singleton.clipEatGhost); break;
                }
                break;
        }
    }

}
