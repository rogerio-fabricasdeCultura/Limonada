using UnityEngine;

public class ObjetoInteragivelUI : MonoBehaviour
{
    [Header("Conteúdo da interação")]
    [Tooltip("Se estiver vazio, será usado o sprite do SpriteRenderer deste objeto.")]
    [SerializeField] private Sprite imagem;

    [TextArea(2, 6)]
    [SerializeField] private string texto;

    private void Awake()
    {
        if (imagem != null)
            return;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            imagem = spriteRenderer.sprite;
    }

    public void ExibirNoPainel(PainelUI painel)
    {
        if (painel == null)
            return;

        painel.Mostrar(imagem, texto);
    }
}
