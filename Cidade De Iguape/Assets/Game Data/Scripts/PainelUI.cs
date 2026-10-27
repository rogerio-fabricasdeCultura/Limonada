using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PainelUI : MonoBehaviour
{
    [Header("Elementos da interface")]
    [SerializeField] private Image imagem;
    [SerializeField] private TMP_Text texto;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Configuração inicial")]
    [SerializeField] private bool visivelAoIniciar;

    public bool EstaVisivel { get; private set; }

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        DefinirVisibilidade(visivelAoIniciar);
    }

    public void Mostrar(Sprite novaImagem, string novoTexto)
    {
        AlterarImagem(novaImagem);
        AlterarTexto(novoTexto);
        DefinirVisibilidade(true);
    }

    public void Ocultar()
    {
        DefinirVisibilidade(false);
    }

    public void AlterarTexto(string novoTexto)
    {
        if (texto != null)
            texto.text = novoTexto;
    }

    public void AlterarImagem(Sprite novaImagem)
    {
        if (imagem == null)
            return;

        imagem.sprite = novaImagem;
        imagem.enabled = novaImagem != null;
    }

    private void DefinirVisibilidade(bool visivel)
    {
        EstaVisivel = visivel;

        canvasGroup.alpha = visivel ? 1f : 0f;
        canvasGroup.interactable = visivel;
        canvasGroup.blocksRaycasts = visivel;
    }
}
