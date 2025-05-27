using UnityEngine;

public class PlayerInteractionModel : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private LayerMask interactionLayer;

    private Collider[] interactables = new Collider[4];

    private void Awake()
    {
        ServiceLocator.Instance.Register<PlayerInteractionModel>(this);
    }

    public bool TryInteract()
    {
        Debug.Log("Tried interaction");

        int elements = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionRadius,
                                                     interactables, interactionLayer);

        if (elements == 0)
        {
            Debug.Log("No interactables found");
            return false;
        }

        bool interactionSuccessful = false;

        for (int i = 0; i < elements; i++)
        {
            var interactable = interactables[i];
            var interactableComponent = interactable.GetComponent<Iinteractable>();

            if (interactableComponent != null)
            {
                interactableComponent.interaction();
                interactionSuccessful = true;
            }
        }

        return interactionSuccessful;
    }
    private void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
        }
    }
}
