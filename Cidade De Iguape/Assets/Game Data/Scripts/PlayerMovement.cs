using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float direcao = Input.GetAxisRaw("Horizontal");

        transform.Translate(
            Vector3.right * direcao * velocidade * Time.deltaTime
        );

        if (direcao > 0)
        {
            animator.SetInteger("Direcao", 1);
            spriteRenderer.flipX = false;
        }
        else if (direcao < 0)
        {
            animator.SetInteger("Direcao", -1);
            spriteRenderer.flipX = true;
        }
        else
        {
            animator.SetInteger("Direcao", 0);

            // Mantém a última orientação quando estiver parado.
        }
    }
}
