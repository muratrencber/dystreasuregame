using UnityEngine;

public class Interact : MonoBehaviour
{
    public Transform head;
    public float interactRadius;
    public LayerMask validLayers;
    public InteractionUI ui;
    public RectTransform canvasRect;

    IInteractableMono current;

    private void Update()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(head.position, head.forward), out hitInfo, interactRadius, validLayers))
        {
            IInteractableMono interactable = hitInfo.collider.GetComponent<IInteractableMono>();
            if (interactable && !interactable.canBeInteracted)
                interactable = null;
            if (interactable != null)
            {
                if (interactable != current)
                {
                    interactable.OnLookStart();
                    if (current != null)
                    {
                        ui.gameObject.SetActive(false);
                        current.OnLookExit();
                    }
                }
                else
                    current.OnLookStay();
                ui.Set(interactable);
                if(!ui.gameObject.activeSelf)
                    ui.gameObject.SetActive(true);
                ui.rect.anchoredPosition = WorldToCanvasPoint(interactable.transform.position + interactable.offset);
            }
            else if (current != null)
            {
                ui.gameObject.SetActive(false);
                current.OnLookExit();
            }
            current = interactable;
        }
        else
        {
            if (current != null)
            {
                ui.gameObject.SetActive(false);
                current.OnLookExit();
            }
            current = null;
        }
        if (current == null && ui.gameObject.activeSelf)
            ui.gameObject.SetActive(false);
        if (current != null && current.canBeInteracted && Input.GetKeyDown(KeyCode.E))
            current.OnInteract();
    }

    Vector2 WorldToCanvasPoint(Vector3 target)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target);
        return new Vector2(
        ((ViewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
    }

    private void OnDisable()
    {
        if(ui)
            ui.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if(head)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(head.transform.position, head.transform.position + head.forward * interactRadius);
        }
    }
}
