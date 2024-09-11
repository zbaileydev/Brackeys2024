using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePlayerDetector : MonoBehaviour
{
    [SerializeField] Interactable interactable;
    [SerializeField] Canvas promptCanvas;

    [Tooltip("The distance in units that the prompt will be above the interactable object.")]
    [SerializeField, Range(0f, 5f)] float promptYOffset;

    Rigidbody2D rb;
    Canvas currentPromptCanvas;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentPromptCanvas = Instantiate(promptCanvas, transform.position, Quaternion.identity);
        currentPromptCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (currentPromptCanvas.gameObject.activeInHierarchy 
        && Input.GetKeyDown(KeyCode.E))
        {
            interactable.PickUp();
            Destroy(currentPromptCanvas.gameObject);
            Destroy(interactable.gameObject);
        }

        //Make sure the prompt canvas always appears a certain distance above the interactable, **regardless of the interactable's rotation**
        if (currentPromptCanvas.gameObject.activeInHierarchy 
        && currentPromptCanvas.transform.position != new Vector3(transform.position.x, transform.position.y + promptYOffset, 0f))
        {
            currentPromptCanvas.transform.position = new Vector3(transform.position.x, transform.position.y + promptYOffset, 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPromptCanvas.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPromptCanvas.gameObject.SetActive(false);
        }        
    }
}
