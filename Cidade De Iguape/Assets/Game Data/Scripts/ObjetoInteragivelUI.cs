using System.Collections;
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

    [Header("Áudio")]
    [SerializeField] private AudioClip audioInteracao;

    [Range(0f, 1f)]
    [SerializeField] private float volumeAudio = 1f;

    [Min(0f)]
    [SerializeField] private float atrasoAudio = 0.3f;

    [Min(1)]
    [SerializeField] private int repeticoesAudio = 2;

    [Min(0f)]
    [SerializeField] private float intervaloEntreRepeticoes = 0.1f;

    [Header("História do local")]
    [Tooltip("Cada item representa uma fala. A barra de espaço avança para o próximo item.")]
    [TextArea(2, 6)]
    [SerializeField] private string[] historia;

    public string NomeLocal => string.IsNullOrWhiteSpace(nomeLocal)
        ? gameObject.name
        : nomeLocal;

    public string TextoInteracao => string.IsNullOrWhiteSpace(textoInteracao)
        ? $"{NomeLocal} — pressione E"
        : textoInteracao;

    public Sprite ImagemLocal => imagemLocal;
    public string[] Historia => historia;

    private Coroutine rotinaAudio;

    public void TocarAudio(AudioSource fonteAudio)
    {
        if (fonteAudio == null || audioInteracao == null)
            return;

        if (rotinaAudio != null)
            StopCoroutine(rotinaAudio);

        rotinaAudio = StartCoroutine(ReproduzirAudio(fonteAudio));
    }

    private IEnumerator ReproduzirAudio(AudioSource fonteAudio)
    {
        if (atrasoAudio > 0f)
            yield return new WaitForSecondsRealtime(atrasoAudio);

        int quantidade = Mathf.Max(1, repeticoesAudio);

        for (int i = 0; i < quantidade; i++)
        {
            fonteAudio.PlayOneShot(audioInteracao, volumeAudio);

            if (i >= quantidade - 1)
                continue;

            float velocidadeAudio = Mathf.Max(
                0.01f,
                Mathf.Abs(fonteAudio.pitch)
            );

            float espera = audioInteracao.length / velocidadeAudio
                + intervaloEntreRepeticoes;

            yield return new WaitForSecondsRealtime(espera);
        }

        rotinaAudio = null;
    }

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
