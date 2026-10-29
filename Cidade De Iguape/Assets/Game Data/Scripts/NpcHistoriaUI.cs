using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcHistoriaUI : MonoBehaviour
{
    [Header("Controles")]
    [SerializeField] private KeyCode teclaContinuar = KeyCode.Space;

    [Header("Interface")]
    [SerializeField] private CanvasGroup grupoHistoria;
    [SerializeField] private Image imagemNpc;
    [SerializeField] private Animator animatorNpc;
    [SerializeField] private TMP_Text textoHistoria;
    [SerializeField] private TMP_Text textoContinuar;

    [Header("Animação")]
    [SerializeField] private string estadoAnimacaoNpc = "NpcFalando";

    [Header("Mensagens")]
    [SerializeField] private string mensagemContinuar =
        "Pressione ESPAÇO para continuar";

    [SerializeField] private string mensagemFinalizar =
        "Pressione ESPAÇO para fechar a história";

    public bool EmExecucao { get; private set; }
    public event Action AoConcluir;

    private string[] falas;
    private int indiceAtual = -1;

    private void Awake()
    {
        Ocultar();
    }

    private void Update()
    {
        if (EmExecucao && Input.GetKeyDown(teclaContinuar))
            ExibirProximaFala();
    }

    public bool Iniciar(string[] novasFalas)
    {
        falas = novasFalas;
        indiceAtual = -1;

        if (!PossuiFalaDepoisDe(-1))
        {
            Ocultar();
            return false;
        }

        EmExecucao = true;
        DefinirGrupoVisivel(true);
        DefinirImagemVisivel(true);
        ReiniciarAnimacao();
        ExibirProximaFala();
        return true;
    }

    public void Ocultar()
    {
        EmExecucao = false;
        falas = null;
        indiceAtual = -1;
        DefinirGrupoVisivel(false);
        DefinirImagemVisivel(false);
        DefinirTextoVisivel(textoHistoria, false);
        DefinirTextoVisivel(textoContinuar, false);
    }

    private void ExibirProximaFala()
    {
        int proximoIndice = indiceAtual + 1;

        while (
            proximoIndice < falas.Length
            && string.IsNullOrWhiteSpace(falas[proximoIndice])
        )
        {
            proximoIndice++;
        }

        if (proximoIndice >= falas.Length)
        {
            Concluir();
            return;
        }

        indiceAtual = proximoIndice;

        if (textoHistoria != null)
            textoHistoria.text = falas[indiceAtual];

        DefinirTextoVisivel(textoHistoria, true);

        bool possuiProxima = PossuiFalaDepoisDe(indiceAtual);

        if (textoContinuar != null)
        {
            textoContinuar.text = possuiProxima
                ? mensagemContinuar
                : mensagemFinalizar;
        }

        DefinirTextoVisivel(textoContinuar, true);
    }

    private void Concluir()
    {
        if (!EmExecucao)
            return;

        EmExecucao = false;
        DefinirGrupoVisivel(false);
        DefinirImagemVisivel(false);
        DefinirTextoVisivel(textoHistoria, false);
        DefinirTextoVisivel(textoContinuar, false);
        AoConcluir?.Invoke();
    }

    private bool PossuiFalaDepoisDe(int indice)
    {
        if (falas == null)
            return false;

        for (int i = indice + 1; i < falas.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(falas[i]))
                return true;
        }

        return false;
    }

    private void ReiniciarAnimacao()
    {
        if (animatorNpc == null)
            return;

        animatorNpc.gameObject.SetActive(true);
        animatorNpc.enabled = true;
        animatorNpc.Rebind();
        animatorNpc.Update(0f);

        if (!string.IsNullOrWhiteSpace(estadoAnimacaoNpc))
            animatorNpc.Play(estadoAnimacaoNpc, 0, 0f);
    }

    private void DefinirGrupoVisivel(bool visivel)
    {
        if (grupoHistoria == null)
            return;

        grupoHistoria.gameObject.SetActive(true);
        grupoHistoria.alpha = visivel ? 1f : 0f;
        grupoHistoria.interactable = false;
        grupoHistoria.blocksRaycasts = false;
    }

    private void DefinirImagemVisivel(bool visivel)
    {
        if (imagemNpc == null)
            return;

        if (visivel)
            imagemNpc.gameObject.SetActive(true);

        imagemNpc.enabled = visivel;

        if (!visivel)
            imagemNpc.gameObject.SetActive(false);
    }

    private static void DefinirTextoVisivel(
        TMP_Text texto,
        bool visivel
    )
    {
        if (texto == null)
            return;

        if (visivel)
        {
            texto.gameObject.SetActive(true);
            texto.enabled = true;

            Color cor = texto.color;
            cor.a = 1f;
            texto.color = cor;
        }
        else
        {
            texto.enabled = false;
            texto.gameObject.SetActive(false);
        }
    }
}
