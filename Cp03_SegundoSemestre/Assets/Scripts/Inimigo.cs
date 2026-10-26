using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    public float velocidade = 2.0f;
    public float raioDeteccao = 5.0f;
    public float distanciaAtaque = 1.0f;
    public int vidas = 3;

    // Controle do tempo de ataque para a animação não bugar
    public float tempoEntreAtaques = 1.5f;
    private float proximoAtaque = 0f;

    private Vector3 posicaoInicial;
    private Transform playerTransform;
    private bool estaMorto = false;

    private SpriteRenderer sr;
    private Animator anima;
    private Rigidbody2D rb;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anima = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Registra a posição inicial
        posicaoInicial = transform.position;

        // Procura o player pela Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (estaMorto) return;

        // Se por acaso o player não foi achado no Start, tenta achar agora
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            return;
        }

        float distanciaAtePlayer = Vector2.Distance(transform.position, playerTransform.position);
        float distanciaAteOrigem = Vector2.Distance(transform.position, posicaoInicial);

        // Se o player entrar no raio de detecção
        if (distanciaAtePlayer <= raioDeteccao)
        {
            if (distanciaAtePlayer > distanciaAtaque)
            {
                PerseguirPlayer();
            }
            else
            {
                Atacar();
            }
        }
        else
        {
            // Se o player fugir, retorna para a posição inicial
            if (distanciaAteOrigem > 0.1f)
            {
                RetornarParaOrigem();
            }
            else
            {
                PararIdle();
            }
        }
    }

    void PerseguirPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, velocidade * Time.deltaTime);

        // Ajusta a orientação do sprite
        if (playerTransform.position.x > transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }

        anima.SetBool("enemyRun", true);
    }

    void Atacar()
    {
        // Para de correr
        anima.SetBool("enemyRun", false);

        // Controla o intervalo para disparar a animação de ataque no tempo certo
        if (Time.time >= proximoAtaque)
        {
            anima.SetTrigger("enemyAttack");
            proximoAtaque = Time.time + tempoEntreAtaques;
        }
    }

    void RetornarParaOrigem()
    {
        transform.position = Vector2.MoveTowards(transform.position, posicaoInicial, velocidade * Time.deltaTime);

        if (posicaoInicial.x > transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }

        anima.SetBool("enemyRun", true);
    }

    void PararIdle()
    {
        anima.SetBool("enemyRun", false);
    }

    private void OnCollisionEnter2D(Collision2D outro)
    {
        if (estaMorto) return;

        if (outro.gameObject.CompareTag("Player"))
        {
            bool pulouEmCima = false;
            foreach (ContactPoint2D contato in outro.contacts)
            {
                // Se o contato veio de cima do inimigo
                if (contato.normal.y < -0.5f)
                {
                    pulouEmCima = true;
                    break;
                }
            }

            if (pulouEmCima)
            {
                PerderVida();

                // Dá um impulso para cima no Player
                Rigidbody2D rbPlayer = outro.gameObject.GetComponent<Rigidbody2D>();
                if (rbPlayer != null)
                {
                    rbPlayer.linearVelocity = new Vector2(rbPlayer.linearVelocity.x, 10f);
                }
            }
        }
    }

    void PerderVida()
    {
        vidas--;

        if (vidas > 0)
        {
            anima.SetTrigger("enemyHit");
        }
        else
        {
            estaMorto = true;
            anima.SetTrigger("enemyDie");

            GetComponent<Collider2D>().enabled = false;
            if (rb != null) rb.simulated = false;

            Destroy(gameObject, 1.0f);
        }
    }
}