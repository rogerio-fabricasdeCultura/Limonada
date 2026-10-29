using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool PodeMover { get; private set; } = true;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!PodeMover)
            return;

        float direcao = Input.GetAxisRaw("Horizontal");

        transform.Translate(
            Vector3.right * direcao * velocidade * Time.deltaTime
        );

        AtualizarAnimacao(direcao);
    }

    public void DefinirMovimento(bool podeMover)
    {
        PodeMover = podeMover;

        if (!PodeMover)
            AtualizarAnimacao(0f);
    }

    private void AtualizarAnimacao(float direcao)
    {
        if (animator != null)
            animator.SetInteger("Direcao", direcao > 0f ? 1 : direcao < 0f ? -1 : 0);

        if (direcao > 0)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
        }
        else if (direcao < 0)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = true;
        }
    }
}
