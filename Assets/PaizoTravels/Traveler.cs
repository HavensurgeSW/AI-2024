using UnityEngine;

public class Traveler : MonoBehaviour
{
  
    public void MoveToNode()
    {
        // Update current node
        //CurrentNode = newNode;

        // Move the traveler in Unity (you would need to implement movement logic)
        //transform.position = new Vector3(newNode.Position.x, 0, newNode.Position.y);
    }
}

public class Matoran : Traveler 
{

}

public class UssalCrab : Traveler
{ 

}