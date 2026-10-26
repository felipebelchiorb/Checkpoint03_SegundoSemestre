using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personagem : MonoBehaviour
{
    // Velocidade do deslocamento do personagem
    public float velocidade = 5.0f;
    // Forca de impulso para o pulo
    public float impulso = 800.0f;
    // Valor da gravidade
    public float gravidade = 2.0f;

    // Sensor para verificar colisao com chao
    public Transform sensorChao;
    private bool estaNoChao;
    private bool pular;

    // Recebe os componentes
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anima;

    void Start()
    {
        // Atribui para ter referência dos componentes
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anima = GetComponent<Animator>();

        // Altera o valor da gravidade do rigid body 2d
        rb.gravityScale = gravidade;
    }

    void Update()
    {
        // Recebe o input horizontal
        float px = Input.GetAxisRaw("Horizontal") * velocidade * Time.deltaTime;
        transform.Translate(px, 0.0f, 0.0f);

        // Altera a orientação do personagem
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            sr.flipX = false;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            sr.flipX = true;
        }

        // Verifica input para o pulo
        if (Input.GetButtonDown("Jump"))
        {
            pular = true;
        }

        // Verifica colisao entre o chao e o personagem
        estaNoChao = Physics2D.Linecast(transform.position,
        sensorChao.position, 1 << LayerMask.NameToLayer("Piso"));

        // Reproduz as animações de movimento
        anima.SetFloat("mover", Mathf.Abs(Input.GetAxisRaw("Horizontal")));
    }

    private void FixedUpdate()
    {
        if (pular && estaNoChao)
        {
            rb.AddForce(Vector2.up * impulso);
            pular = false;
        }
    }

    // Função para chamar a animação de hit do próprio personagem (caso ele precise futuramente)
    public void TomarDano()
    {
        anima.SetTrigger("hit");
    }
}