using UnityEngine;

public class Traveler : MonoBehaviour
{
    public Node<object> CurrentNode { get; set; }  // Current node of the traveler
    public Node<object> TargetNode { get; set; }   // Target node for the traveler

    // Method to move the traveler to a new node
    public void MoveToNode(Node<object> newNode)
    {
        // Update current node
        CurrentNode = newNode;

        // Move the traveler in Unity (you would need to implement movement logic)
        transform.position = new Vector3(newNode.Position.x, 0, newNode.Position.y);
    }
}