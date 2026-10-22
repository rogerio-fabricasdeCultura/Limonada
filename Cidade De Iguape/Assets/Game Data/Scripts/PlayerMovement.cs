using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
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
        }
        else if (direcao < 0)
        {
            animator.SetInteger("Direcao", -1);
        }
        else
        {
            animator.SetInteger("Direcao", 0);
        }
    }
}