using UnityEngine;

public class WinTile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.WinGame();   
        }
    }
}