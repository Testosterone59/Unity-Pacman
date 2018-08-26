using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BasicPerson {

    private Vector3[] directions = new Vector3[4] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

    private const float timeToNewDirection = 1;
    const float defaultSpeed = 4;
    float time, timeOfFright, chaseTime = 0;

    public MeshRenderer mr;

    [HideInInspector]
    public StateEnemy state = StateEnemy.Normal; 

    public static List<Enemy> instances = new List<Enemy>();

    new void Awake()
    {
        base.Awake();
        instances.Add(this);
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }

    public static void DestroyInstances()
    {
        foreach (var inst in instances)
        {
            Destroy(inst.gameObject);
        }
        instances.Clear();
    }

    new void Start()
    {
        base.Start();
        StartCoroutine(IAI());
    }

    public new void ResetPerson()
    {
        base.ResetPerson();
        ChangeState(StateEnemy.Normal);
    }

    IEnumerator IAI()
    {
        while (true)
        {
            yield return null;
            if (!GameLogic.gameStarted) { continue; }
            switch (state)
            {
                default:
                    if (time <= 0 || !moved) { SelectNewDirection(); time = timeToNewDirection; }
                    if (CheckPlayer(direction)) { ChangeState(StateEnemy.Chase); }
                    time -= Time.deltaTime;
                    break;
                case StateEnemy.Afraid:
                    timeOfFright -= Time.deltaTime;
                    if (timeOfFright <= 0) { ChangeState(StateEnemy.Normal); }
                    if (time <= 0 || !moved || CheckPlayer(direction)) { SelectNewDirection(); time = timeToNewDirection; }
                    time -= Time.deltaTime;
                    break;
                case StateEnemy.Chase:
                    if (!CheckPlayer(direction)) { SelectDirectionToPlayer(); yield return null; }
                    if (chaseTime <= 0) { yield return new WaitForSeconds(2); ChangeState(StateEnemy.Normal); }
                    chaseTime -= Time.deltaTime;
                    break;
                case StateEnemy.Death:
                    float distance = (startPosition - myTransform.position).magnitude;
                    if (distance < .1f) { moved = false; yield return new WaitForSeconds(2); ChangeState(StateEnemy.Normal); }
                    break;
            }
        }
    }
    /// <summary>
    /// Выбрать случайное направление
    /// </summary>
    private void SelectNewDirection()
    {
        int rand = Random.Range(0, 4);
        if (direction == directions[rand] ||
           (!moved && CheckWall(directions[rand]))) { SelectNewDirection(); }
        else { nextDirection = directions[rand]; }
    }
    /// <summary>
    /// Выбрать более подходящее направление до игрока
    /// </summary>
    private void SelectDirectionToPlayer()
    {
        Vector3 dir = (GameLogic.player.transform.position - myTransform.position).normalized;
        Vector3 newDir = Vector3.zero;
        float oldDot = -1;
        foreach (var d in directions)
        {
            float newDot = Vector3.Dot(d, dir);
            if (newDot > oldDot) { newDir = d; oldDot = newDot; }
        }
        nextDirection = newDir;
    }
    /// <summary>
    /// Изменение состояния призрака
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(StateEnemy newState)
    {
        if (state == newState) { return; }
        if (state == StateEnemy.Death)
        {
            mr.enabled = true;
            myTransform.position = startPosition;
            ignoreWalls = false;
        }
        switch (newState)
        {
            case StateEnemy.Normal:
                mr.sharedMaterial = Resources.Load("Materials/MatGhost") as Material;
                speed = defaultSpeed;
                break;
            case StateEnemy.Afraid:
                mr.sharedMaterial = Resources.Load("Materials/MatAfraidGhost") as Material;
                speed = defaultSpeed * 1.5f;
                timeOfFright = 15;
                break;
            case StateEnemy.Death:
                mr.enabled = false;
                direction = (startPosition - myTransform.position).normalized;
                nextDirection = direction;
                SetRotation();
                ignoreWalls = true;
                break;
            case StateEnemy.Chase:
                speed = GameLogic.player._getSpeed;
                chaseTime = 5; 
                break;
        }
        state = newState;
    }

    private bool CheckPlayer(Vector3 dir)
    {
        Ray r = new Ray(myTransform.position + Vector3.up / 2, dir);
        RaycastHit hit;
        return Physics.Raycast(r, out hit, Generator.sizeCell * 5) && hit.transform.tag == "Player";
    }

}
