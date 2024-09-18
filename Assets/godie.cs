using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class godie : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    private Queue<Transform> way = new Queue<Transform>();
    public Transform currentTarget;
    public float interactDistance;
    public float speed;


    private void Start()
    {
        for (int i = 0; i < waypoints.Count; i++) { 
            way.Enqueue(waypoints[i]);
        }
        currentTarget = way.Peek();

        
    }
    private void Update()
    {
        if (Vector3.Distance(this.transform.position, currentTarget.position) > interactDistance)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentTarget.position, speed * Time.deltaTime);
        }
        else {
            way.Dequeue();
            currentTarget = way.Peek();
        }
    }
}
