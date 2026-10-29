using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteracaoUI : MonoBehaviour
{
    [Header("Interação")]
    [SerializeField] private KeyCode teclaInteracao = KeyCode.E;

    [Header("Mensagem de interação")]
    [SerializeField] private CanvasGroup promptInteracao;
    [SerializeField] private TMP_Text textoPrompt;

    [Header("Sequência do local")]
    [SerializeField] private SequenciaVisitaUI sequenciaVisita;

    private readonly List<ObjetoInteragivelUI> objetosAoAlcance =
        new List<ObjetoInteragivelUI>();
    private ObjetoInteragivelUI objetoEmDestaque;

    private void Awake()
    {
        DefinirPromptVisivel(false);
    }

    private void OnEnable()
    {
        if (sequenciaVisita != null)
            sequenciaVisita.AoFinalizar += AtualizarPrompt;
    }

    private void OnDisable()
    {
        if (sequenciaVisita != null)
            sequenciaVisita.AoFinalizar -= AtualizarPrompt;

        DefinirPromptVisivel(false);
    }

    private void Update()
    {
        AtualizarObjetoEmDestaque();

        if (sequenciaVisita != null && sequenciaVisita.EmExecucao)
        {
            DefinirPromptVisivel(false);
            return;
        }

        if (objetoEmDestaque != null && Input.GetKeyDown(teclaInteracao))
            IniciarInteracao();
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        ObjetoInteragivelUI objeto =
            outro.GetComponentInParent<ObjetoInteragivelUI>();

        if (objeto == null || objetosAoAlcance.Contains(objeto))
            return;

        objetosAoAlcance.Add(objeto);
        AtualizarObjetoEmDestaque();
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        ObjetoInteragivelUI objeto =
            outro.GetComponentInParent<ObjetoInteragivelUI>();

        if (objeto == null)
            return;

        objetosAoAlcance.Remove(objeto);

        AtualizarObjetoEmDestaque();
    }

    private void IniciarInteracao()
    {
        if (sequenciaVisita == null)
        {
            Debug.LogWarning(
                "A SequenciaVisitaUI não foi atribuída ao PlayerInteracaoUI.",
                this
            );

            return;
        }

        if (sequenciaVisita.Iniciar(objetoEmDestaque))
            DefinirPromptVisivel(false);
    }

    private void AtualizarObjetoEmDestaque()
    {
        objetosAoAlcance.RemoveAll(objeto => objeto == null);

        ObjetoInteragivelUI maisProximo = null;
        float menorDistancia = float.MaxValue;

        foreach (ObjetoInteragivelUI objeto in objetosAoAlcance)
        {
            float distancia =
                (objeto.transform.position - transform.position).sqrMagnitude;

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProximo = objeto;
            }
        }

        if (objetoEmDestaque == maisProximo)
            return;

        objetoEmDestaque = maisProximo;
        AtualizarPrompt();
    }

    private void AtualizarPrompt()
    {
        bool podeExibir = objetoEmDestaque != null
            && (sequenciaVisita == null || !sequenciaVisita.EmExecucao);

        if (podeExibir && textoPrompt != null)
            textoPrompt.text = objetoEmDestaque.TextoInteracao;

        DefinirPromptVisivel(podeExibir);
    }

    private void DefinirPromptVisivel(bool visivel)
    {
        if (promptInteracao == null)
            return;

        promptInteracao.alpha = visivel ? 1f : 0f;
        promptInteracao.interactable = false;
        promptInteracao.blocksRaycasts = false;
    }
}
