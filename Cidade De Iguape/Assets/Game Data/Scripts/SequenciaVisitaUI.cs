using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SequenciaVisitaUI : MonoBehaviour
{
    [Header("Controles")]
    [SerializeField] private PlayerMovement movimentoJogador;
    [SerializeField] private KeyCode teclaFecharFoto = KeyCode.Space;

    [Header("Elementos da sequência")]
    [SerializeField] private CanvasGroup raizSequencia;
    [SerializeField] private CanvasGroup fundoPreto;
    [SerializeField] private Image imagemLocal;
    [SerializeField] private Image personagemAnimado;
    [SerializeField] private TMP_Text textoVoltar;
    [SerializeField] private Animator animatorPersonagem;

    [Header("História do local")]
    [SerializeField] private NpcHistoriaUI npcHistoria;

    [Tooltip("Texto exibido durante a visita.")]
    [SerializeField] private string mensagemFecharFoto =
        "Pressione ESPAÇO para fechar a foto";

    [Header("Animação")]
    [Tooltip("Nome do estado criado no Animator do PersonagemAnimado.")]
    [SerializeField] private string estadoAnimacaoPersonagem =
        "PersonagemTirandoFoto";

    [Min(0f)]
    [SerializeField] private float duracaoFadeEntrada = 0.75f;

    [Min(0f)]
    [SerializeField] private float duracaoAnimacaoPersonagem = 1.5f;

    [Min(0f)]
    [SerializeField] private float duracaoFadeSaida = 0.5f;

    public bool EmExecucao { get; private set; }
    public event Action AoFinalizar;

    private bool aguardandoFecharFoto;
    private int frameHabilitouFecharFoto = -1;

    private void Awake()
    {
        PrepararEstadoInicial();
    }

    private void OnEnable()
    {
        if (npcHistoria != null)
            npcHistoria.AoConcluir += HabilitarFecharFoto;
    }

    private void OnDisable()
    {
        if (npcHistoria != null)
            npcHistoria.AoConcluir -= HabilitarFecharFoto;
    }

    private void Update()
    {
        if (
            aguardandoFecharFoto
            && Time.frameCount > frameHabilitouFecharFoto
            && Input.GetKeyDown(teclaFecharFoto)
        )
        {
            StartCoroutine(FinalizarSequencia());
            return;
        }
    }

    public bool Iniciar(ObjetoInteragivelUI local)
    {
        if (EmExecucao || local == null)
            return false;

        StartCoroutine(ExecutarSequencia(local));
        return true;
    }

    private IEnumerator ExecutarSequencia(ObjetoInteragivelUI local)
    {
        EmExecucao = true;
        aguardandoFecharFoto = false;

        if (movimentoJogador != null)
            movimentoJogador.DefinirMovimento(false);

        DefinirCanvasGroup(raizSequencia, 1f, true);
        DefinirCanvasGroup(fundoPreto, 0f, false);
        DefinirImagem(imagemLocal, false);
        DefinirImagem(personagemAnimado, false);
        DefinirTexto(textoVoltar, false);

        if (npcHistoria != null)
            npcHistoria.Ocultar();

        yield return AlterarAlpha(
            fundoPreto,
            0f,
            1f,
            duracaoFadeEntrada
        );

        DefinirImagem(personagemAnimado, true);
        ReiniciarAnimacaoPersonagem();

        if (duracaoAnimacaoPersonagem > 0f)
            yield return new WaitForSecondsRealtime(duracaoAnimacaoPersonagem);

        DefinirImagem(personagemAnimado, false);

        if (imagemLocal != null)
        {
            imagemLocal.sprite = local.ImagemLocal;
            DefinirImagem(imagemLocal, local.ImagemLocal != null);
        }

        if (npcHistoria == null || !npcHistoria.Iniciar(local.Historia))
            HabilitarFecharFoto();
    }

    private IEnumerator FinalizarSequencia()
    {
        aguardandoFecharFoto = false;

        float alphaInicial = raizSequencia != null
            ? raizSequencia.alpha
            : 1f;

        yield return AlterarAlpha(
            raizSequencia,
            alphaInicial,
            0f,
            duracaoFadeSaida
        );

        PrepararEstadoInicial();

        if (movimentoJogador != null)
            movimentoJogador.DefinirMovimento(true);

        EmExecucao = false;
        AoFinalizar?.Invoke();
    }

    private void ReiniciarAnimacaoPersonagem()
    {
        if (animatorPersonagem == null)
            return;

        ReiniciarAnimator(
            animatorPersonagem,
            estadoAnimacaoPersonagem
        );
    }

    private void HabilitarFecharFoto()
    {
        if (!EmExecucao)
            return;

        if (textoVoltar != null)
            textoVoltar.text = mensagemFecharFoto;

        DefinirTexto(textoVoltar, true);
        aguardandoFecharFoto = true;
        frameHabilitouFecharFoto = Time.frameCount;
    }

    private static void ReiniciarAnimator(
        Animator animator,
        string nomeEstado
    )
    {
        if (animator == null)
            return;

        animator.gameObject.SetActive(true);
        animator.enabled = true;
        animator.Rebind();
        animator.Update(0f);

        if (!string.IsNullOrWhiteSpace(nomeEstado))
            animator.Play(nomeEstado, 0, 0f);
    }

    private void PrepararEstadoInicial()
    {
        aguardandoFecharFoto = false;
        frameHabilitouFecharFoto = -1;
        DefinirCanvasGroup(raizSequencia, 0f, false);
        DefinirCanvasGroup(fundoPreto, 0f, false);
        DefinirImagem(imagemLocal, false);
        DefinirImagem(personagemAnimado, false);
        DefinirTexto(textoVoltar, false);

        if (npcHistoria != null)
            npcHistoria.Ocultar();
    }

    private static IEnumerator AlterarAlpha(
        CanvasGroup grupo,
        float origem,
        float destino,
        float duracao
    )
    {
        if (grupo == null)
            yield break;

        if (duracao <= 0f)
        {
            grupo.alpha = destino;
            yield break;
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;
            grupo.alpha = Mathf.Lerp(
                origem,
                destino,
                Mathf.Clamp01(tempo / duracao)
            );

            yield return null;
        }

        grupo.alpha = destino;
    }

    private static void DefinirCanvasGroup(
        CanvasGroup grupo,
        float alpha,
        bool bloquearRaycasts
    )
    {
        if (grupo == null)
            return;

        grupo.alpha = alpha;
        grupo.interactable = false;
        grupo.blocksRaycasts = bloquearRaycasts;
    }

    private static void DefinirImagem(Image imagem, bool visivel)
    {
        if (imagem == null)
            return;

        if (visivel)
            imagem.gameObject.SetActive(true);

        imagem.enabled = visivel;

        if (!visivel)
            imagem.gameObject.SetActive(false);
    }

    private static void DefinirTexto(TMP_Text texto, bool visivel)
    {
        if (texto == null)
            return;

        if (visivel)
        {
            texto.gameObject.SetActive(true);

            Color cor = texto.color;
            cor.a = 1f;
            texto.color = cor;
        }

        texto.enabled = visivel;

        if (!visivel)
            texto.gameObject.SetActive(false);
    }
}
