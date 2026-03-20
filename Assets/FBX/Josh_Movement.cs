using UnityEngine;

public class Josh_Movement : MonoBehaviour
{
    public float speed = 3.0f;
    public float range = 5.0f;
    public float cooldown = 2.0f;
    public float rotationSpeed = 10.0f;

    public Animator animator;
    public string walking = "isWalking";

    private Vector3 finalPoint;
    private Vector3 inicialPoint;
    private bool waiting = false;

    void Start()
    {
        inicialPoint = transform.position;

        if (animator == null) animator = GetComponent<Animator>();

        UpdateNewDestiny();
    }

    void Update()
    {
        if (!waiting)
        {
            MoveNPC();
        }
    }

    void MoveNPC()
    {
        Vector3 direction = (finalPoint - inicialPoint).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion rotationObjetive = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationObjetive, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, finalPoint, speed * Time.deltaTime);
        if (animator != null) animator.SetBool(walking, true);
        if (Vector3.Distance(transform.position, finalPoint) < 0.1f)
        {
            StartCoroutine(wait());
        }
    }


    void UpdateNewDestiny()
    {
        float randomX = Random.Range(inicialPoint.x - range, inicialPoint.x + range);
        float randomZ = Random.Range(inicialPoint.z - range, inicialPoint.z + range);

        finalPoint = new Vector3(randomX, 0, randomZ);
    }

    System.Collections.IEnumerator wait()
    {
        waiting = true;
        if (animator != null) animator.SetBool(walking, false);

        yield return new WaitForSeconds(cooldown);
        UpdateNewDestiny();
        waiting = false;
    }
}