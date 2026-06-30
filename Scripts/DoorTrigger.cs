using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private doorcontroller door;
    [SerializeField] private Animator animator;
    private int playersInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerBase>())
        {
            if (!other.GetComponent<PlayerBase>())
                return;

            playersInside++;

            animator.SetBool("isTrigger", true);
            door.OpenDoor();
          
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.GetComponent<PlayerBase>())
            return;

        playersInside--;

        if (playersInside <= 0) {
            playersInside = 0;

            animator.SetBool("isTrigger", false);

            door.CloseDoor();
        }
    }
}