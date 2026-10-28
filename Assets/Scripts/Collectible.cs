using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager nao encontrado na cena.", this);
            return;
        }

        GameManager.Instance.CollectItem();
        gameObject.SetActive(false);
    }
}
