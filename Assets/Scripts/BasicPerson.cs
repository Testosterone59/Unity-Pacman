using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPerson : MonoBehaviour {

    [SerializeField]
    protected float speed = 10;
    public float _getSpeed { get { return speed; } }

    protected bool moved, ignoreWalls = false;

    [SerializeField]
    private Transform childTransform;
    protected Transform myTransform;

    protected Vector3 direction, nextDirection, startPosition = Vector3.zero;
    public Vector3 _getDirection { get { return direction; } }

    protected void Awake()
    {
        myTransform = transform;
        startPosition = myTransform.position;
    }

    protected void Start()
    {
        StartCoroutine(IMoving());
    }

    public void ResetPerson()
    {
        myTransform.position = startPosition;
        direction = Vector3.zero;
        nextDirection = direction;
        moved = false;
    }

    protected void SetMove(Direction dir)
    {
        switch (dir)
        {
            case Direction.Forward: nextDirection = Vector3.forward; break;
            case Direction.Back: nextDirection = Vector3.back; break;
            case Direction.Left: nextDirection = Vector3.left; break;
            case Direction.Right: nextDirection = Vector3.right; break;
        }
    }

    protected void SetRotation()
    {
        childTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    IEnumerator IMoving()
    {
        while (true)
        {
            yield return null;
            if (!GameLogic.gameStarted) { continue; }
            if (nextDirection != direction)
            {
                /// Проверка объекта на возможность повернуть в назначенное направление
                float remainderX = myTransform.position.x % Generator.sizeCell;
                float remainderZ = myTransform.position.z % Generator.sizeCell;
                if ((remainderX < .12f || remainderX > .88f) && (remainderZ < .12f || remainderZ > .88f))
                {
                    if (!CheckWall(nextDirection))
                    {
                        direction = nextDirection;
                        SetRotation();
                        moved = true;
                    }
                }
                /// Исправление ошибки, в случае сильного смещения объекта, на высокой скорости
                else if (!moved) { myTransform.position = new Vector3(myTransform.position.x - remainderX, myTransform.position.y, myTransform.position.z - remainderZ); }
            }
            if (!moved) { continue; }
            myTransform.Translate(direction * speed * Time.deltaTime);
            if (!ignoreWalls && CheckWall(direction)) { moved = false; continue; }
        }
    }

    protected bool CheckWall(Vector3 dir)
    {
        Ray r = new Ray(myTransform.position, dir);
        RaycastHit hit;
        return Physics.Raycast(r, out hit, Generator.sizeCell / 2 + .1f) && hit.transform.tag == "Wall";
    }


}
