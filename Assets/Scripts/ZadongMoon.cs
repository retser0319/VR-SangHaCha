using UnityEngine;
using System.Collections;

public class ZadongMoon : MonoBehaviour
{
    public Transform door;

    public Vector3 openPos;
    public Vector3 closePos;

    public float moveSpeed = 2f;
    public float waitTime = 2f;

    void Start()
    {
    openPos = transform.position;
    closePos = transform.position + Vector3.down * 9f;

    StartCoroutine(DoorRoutine());
    }

    IEnumerator DoorRoutine()
    {
        while (true)
        {
            yield return MoveDoor(closePos);

            yield return new WaitForSeconds(waitTime);

            yield return MoveDoor(openPos);

            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator MoveDoor(Vector3 target)
    {
        while (Vector3.Distance(door.position, target) > 0.01f)
        {
            door.position =
                Vector3.MoveTowards(
                    door.position,
                    target,
                    moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
}