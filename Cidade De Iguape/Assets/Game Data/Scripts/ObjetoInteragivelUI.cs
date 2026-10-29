using UnityEngine;

public class ObjetoInteragivelUI : MonoBehaviour
{
    [Header("Conteúdo da interação")]
    [SerializeField] private string nomeLocal = "Fonte";

    [Tooltip("Mensagem exibida quando o jogador estiver perto. Se estiver vazia, será criado um texto automaticamente.")]
    [TextArea(2, 4)]
    [SerializeField] private string textoInteracao;

    [Tooltip("Se estiver vazio, será usado o sprite do SpriteRenderer deste objeto.")]
    [SerializeField] private Sprite imagemLocal;

    public string NomeLocal => string.IsNullOrWhiteSpace(nomeLocal)
        ? gameObject.name
        : nomeLocal;

    public string TextoInteracao => string.IsNullOrWhiteSpace(textoInteracao)
        ? $"{NomeLocal} — pressione E"
        : textoInteracao;

    public Sprite ImagemLocal => imagemLocal;

    private void Awake()
    {
        if (imagemLocal != null)
            return;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            imagemLocal = spriteRenderer.sprite;
    }

    public void ExibirNoPainel(PainelUI painel)
    {
        if (painel == null)
            return;

        painel.Mostrar(imagemLocal, TextoInteracao);
    }
}
