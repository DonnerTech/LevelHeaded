using Unity.VisualScripting;
using UnityEngine;

public class Feet : MonoBehaviour
{
    [SerializeField]
    private BasicPlayer player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.FootPos;
    }
}
