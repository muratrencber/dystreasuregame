using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public static GameObject PlayerObject;

    public static Vector3 PlayerPosition;
    public static Vector3 PlayerHead;
    public static Vector3 PlayerFeet;

    [SerializeField]
    CharacterController controller;
    [SerializeField]
    Bobbing bobbing;

    [SerializeField]
    float speed;
    [SerializeField]
    float gravityScale = 1;
    [SerializeField]
    float jumpHeight;
    [SerializeField]
    LayerMask groundMask;

    [SerializeField]
    AudioSource walkSource;
    [SerializeField]
    AudioClip[] woodClips;
    [SerializeField]
    AudioClip[] concreteClips;
    [SerializeField]
    float durationBetweenSteps;
    float stepTimer;

    bool sprinting;

    float groundDistance = .4f;
    public bool isGrounded;
    Vector3 velocity;

    private void Start()
    {
        PlayerObject = gameObject;
    }

    public void Teleport(Vector3 target)
    {
        controller.enabled = false;
        transform.position = target;
        controller.enabled = true;
    }

    void Update()
    {
        PlayerPosition = transform.position;
        PlayerHead = transform.position + transform.up * controller.height * .5f;
        PlayerFeet = transform.position - transform.up * controller.height * .5f;

        sprinting = Input.GetKey(KeyCode.LeftShift);
        bobbing.sprinting = sprinting;

        Vector3 groundPosition = PlayerPosition + Vector3.down * controller.height * .5f;
        isGrounded = Physics.CheckSphere(groundPosition, groundDistance, groundMask);

        GameObject groundObject = null;
        RaycastHit hit;
        if (isGrounded && Physics.Raycast(new Ray(PlayerFeet, Vector3.down), out hit, groundDistance, groundMask))
            groundObject = hit.collider.gameObject;

        if(isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 0 && isGrounded)
        {
            if (stepTimer <= 0)
            {
                walkSource.PlayOneShot((groundObject && groundObject.tag == "Concrete") ? concreteClips[Random.Range(0, concreteClips.Length)] : woodClips[Random.Range(0, woodClips.Length)]);
                stepTimer = sprinting ? durationBetweenSteps * .35f : durationBetweenSteps;
            }
            else
                stepTimer -= Time.deltaTime;

        }
        else
            stepTimer = 0;

        bobbing.bobbing = move.magnitude > 0;

        controller.Move(move*speed * (sprinting ? 2.5f : 1) * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y * gravityScale);

        velocity.y += Physics.gravity.y * gravityScale * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}