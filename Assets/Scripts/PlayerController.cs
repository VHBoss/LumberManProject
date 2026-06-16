using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] Animator CharacterAnimator;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] MeshRenderer bagRenderer;
    [SerializeField] Material burnMaterial;

    private bool m_CanChop = false;
    private bool m_IsChopping = false;

    private readonly int RunAnimation = Animator.StringToHash("Run");
    private readonly int AxeAnimation = Animator.StringToHash("Axe");

    private CharacterController controller;

    private PlayerInput playerInput;
    private InputAction moveAction;

    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    private Vector3 moveInput;
    private bool isDead;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
    }

    void Update()
    {
        if(isDead) return;

        Move();
        Chop();
    }

    void Move()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        moveInput = new Vector3(-input.x, 0, -input.y);
        moveInput = Vector3.ClampMagnitude(moveInput, 1f);

        if (moveInput != Vector3.zero)
        {
            transform.forward = moveInput;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        Vector3 finalMove = moveInput * moveSpeed + Vector3.up * playerVelocity.y;
        controller.Move(finalMove * Time.deltaTime);

        CharacterAnimator.SetBool(RunAnimation, input != Vector2.zero);
    }

    void Chop()
    {
        StartChop(m_CanChop);
        m_CanChop = false;
    }

    public void TriggerStay(Collider collider)
    {
        //Debug.Log("TriggerStay: " + collider.tag, collider);
        if (collider.CompareTag("Tree"))
        {
            m_CanChop = true;
            //StartChop(true);
        }
    }

    void StartChop(bool state)
    {
        if(m_IsChopping == state) return;

        m_IsChopping = state;
        CharacterAnimator.SetBool(AxeAnimation, state);
    }

    public void SetDead()
    {
        isDead = true;

        CharacterAnimator.SetBool(RunAnimation, false);
        CharacterAnimator.SetBool(AxeAnimation, false);

        meshRenderer.material = burnMaterial;
        bagRenderer.material = burnMaterial;
    }
}
