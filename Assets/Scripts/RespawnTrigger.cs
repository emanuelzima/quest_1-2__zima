using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController controller = other.gameObject.GetComponent<CharacterController>();

        if (controller != null)
        {
            Respawn(controller);
        }
    }

    private void Respawn(CharacterController controller)
    {
        controller.enabled = false;
        controller.transform.position = this.respawnPoint.position;
        controller.enabled = true;
    }
}