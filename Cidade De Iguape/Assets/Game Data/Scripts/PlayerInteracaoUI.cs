using System.Collections.Generic;
using UnityEngine;

public class PlayerInteracaoUI : MonoBehaviour
{
    [Header("Interação")]
    [SerializeField] private KeyCode teclaInteracao = KeyCode.E;

    [Header("Interface")]
    [SerializeField] private PainelUI painel;

    private readonly List<ObjetoInteragivelUI> objetosAoAlcance =
        new List<ObjetoInteragivelUI>();
    private ObjetoInteragivelUI objetoEmDestaque;
    private ObjetoInteragivelUI objetoExibido;

    private void Update()
    {
        AtualizarObjetoEmDestaque();

        if (objetoEmDestaque != null && Input.GetKeyDown(teclaInteracao))
            AlternarPainel();
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

        if (objetoExibido == objeto)
            FecharPainel();

        AtualizarObjetoEmDestaque();
    }

    private void AlternarPainel()
    {
        if (painel == null)
        {
            Debug.LogWarning(
                "O PainelUI não foi atribuído ao PlayerInteracaoUI.",
                this
            );

            return;
        }

        if (painel.EstaVisivel && objetoExibido == objetoEmDestaque)
        {
            FecharPainel();
            return;
        }

        objetoEmDestaque.ExibirNoPainel(painel);
        objetoExibido = objetoEmDestaque;
    }

    private void FecharPainel()
    {
        if (painel != null)
            painel.Ocultar();

        objetoExibido = null;
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

        objetoEmDestaque = maisProximo;
    }
}
