using UnityEngine;

public class ArrowPlatformTrigger : MonoBehaviour
{
    [SerializeField] private doorcontroller door;
    [SerializeField] private bool allowPlayers = true;
[SerializeField] private bool allowBoxes = false;

    private PlayerBase currentPlayer;

    private bool IsValidTrigger(Collider2D other) {
        bool isPlayer =
            allowPlayers &&
            other.GetComponent<PlayerBase>();

        bool isBox =
            allowBoxes &&
            other.CompareTag("PushableBox");

        return isPlayer || isBox;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (!IsValidTrigger(other))
            return;

        door.OpenDoor();

        PlayerBase player =
            other.GetComponent<PlayerBase>();

        if (player == null)
            return;

        // Boxes stop here
        if (currentPlayer != null)
            return;

        currentPlayer = player;

        player.SetMovementLocked(true);

        player.OnArrowLocked += ResumeMovement;
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (!IsValidTrigger(other))
            return;

        if (other.CompareTag("PushableBox")) {
            door.CloseDoor();
        }
    }

    private void ResumeMovement()
    {
        if (currentPlayer == null)
            return;

        currentPlayer.SetMovementLocked(false);

        door.CloseDoor();

        currentPlayer.OnArrowLocked -= ResumeMovement;

        currentPlayer = null;
    }
}