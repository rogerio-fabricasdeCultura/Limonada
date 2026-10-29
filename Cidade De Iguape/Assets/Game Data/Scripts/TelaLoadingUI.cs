using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TelaLoadingUI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text textoContinuar;
    [SerializeField] private PlayerMovement movimentoJogador;

    [Header("Configuração")]
    [SerializeField] private KeyCode teclaContinuar = KeyCode.Space;

    [Min(0f)]
    [SerializeField] private float tempoMinimo = 1f;

    [Min(0f)]
    [SerializeField] private float duracaoFadeSaida = 0.5f;

    [SerializeField] private string mensagemContinuar =
        "Pressione ESPAÇO para começar";

    public bool EstaVisivel { get; private set; }

    private bool podeFechar;
    private bool fechando;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        Mostrar();
    }

    private void Update()
    {
        if (
            EstaVisivel
            && podeFechar
            && !fechando
            && Input.GetKeyDown(teclaContinuar)
        )
        {
            StartCoroutine(Fechar());
        }
    }

    public void Mostrar()
    {
        StopAllCoroutines();

        gameObject.SetActive(true);
        EstaVisivel = true;
        podeFechar = false;
        fechando = false;

        DefinirCanvasGroup(1f, true);
        DefinirTextoContinuar(false);

        if (movimentoJogador != null)
            movimentoJogador.DefinirMovimento(false);

        StartCoroutine(LiberarFechamento());
    }

    private IEnumerator LiberarFechamento()
    {
        if (tempoMinimo > 0f)
            yield return new WaitForSecondsRealtime(tempoMinimo);

        if (textoContinuar != null)
            textoContinuar.text = mensagemContinuar;

        DefinirTextoContinuar(true);
        podeFechar = true;
    }

    private IEnumerator Fechar()
    {
        fechando = true;
        podeFechar = false;
        DefinirTextoContinuar(false);

        float tempo = 0f;
        float alphaInicial = canvasGroup != null
            ? canvasGroup.alpha
            : 1f;

        while (canvasGroup != null && tempo < duracaoFadeSaida)
        {
            tempo += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(
                alphaInicial,
                0f,
                duracaoFadeSaida <= 0f
                    ? 1f
                    : Mathf.Clamp01(tempo / duracaoFadeSaida)
            );

            yield return null;
        }

        DefinirCanvasGroup(0f, false);
        EstaVisivel = false;
        fechando = false;

        if (movimentoJogador != null)
            movimentoJogador.DefinirMovimento(true);
    }

    private void DefinirCanvasGroup(float alpha, bool bloquearRaycasts)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = alpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = bloquearRaycasts;
    }

    private void DefinirTextoContinuar(bool visivel)
    {
        if (textoContinuar == null)
            return;

        if (visivel)
        {
            textoContinuar.gameObject.SetActive(true);
            textoContinuar.enabled = true;

            Color cor = textoContinuar.color;
            cor.a = 1f;
            textoContinuar.color = cor;
        }
        else
        {
            textoContinuar.enabled = false;
            textoContinuar.gameObject.SetActive(false);
        }
    }
}
