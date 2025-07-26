using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    public Transform camara;
    public AudioSource audioSource;
    //public AudioClip sonidoPasos;// activar para sonido
    public float intervaloPasos = 0.4f;
    private float temporizadorPasos = 0f;
    public Animator animator;
    public float velocidad = 5f;
    public float fuerzaSalto = 5f;
    public Transform chequeoSuelo;
    public float radioSuelo = 0.3f;
    public LayerMask capaSuelo;
    public float velocidadRotacion = 3f;
    private Rigidbody rb;
    private bool enElSuelo;
    private bool saltoSolicitado;
    float velocidad1;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        velocidad1 = velocidad;
    }

    void Update()
    {
        
        enElSuelo = Physics.CheckSphere(chequeoSuelo.position, radioSuelo, capaSuelo);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && enElSuelo)
        {
            saltoSolicitado = true;
        }
        if (Mouse.current != null && animator != null)
        {
            bool estaApuntando = Mouse.current.rightButton.isPressed;
            animator.SetBool("Apuntando", estaApuntando);
            if (estaApuntando)
            {
                velocidad = 0;
                animator.SetFloat("VelocidadAnim", 0);

            }
            else if(estaApuntando==false)
            {
                velocidad = velocidad1;
            }
            
        }

        if (animator != null)
        {
            animator.SetBool("Saltando", !enElSuelo);
        }
    }

    void FixedUpdate()
    {
        if (Keyboard.current == null) return;

        Vector3 direccion = Vector3.zero;

        Vector3 forwardCam = camara.forward;
        Vector3 rightCam = camara.right;

        // Eliminar la componente vertical para evitar que suba/baje
        forwardCam.y = 0f;
        rightCam.y = 0f;

        forwardCam.Normalize();
        rightCam.Normalize();

        if (Keyboard.current.wKey.isPressed) direccion += forwardCam;
        if (Keyboard.current.sKey.isPressed) direccion -= forwardCam;
        if (Keyboard.current.aKey.isPressed) direccion -= rightCam;
        if (Keyboard.current.dKey.isPressed) direccion += rightCam;


        direccion = direccion.normalized;

        // Movimiento
        Vector3 velocidadDeseada = direccion * velocidad;
        rb.linearVelocity = new Vector3(velocidadDeseada.x, rb.linearVelocity.y, velocidadDeseada.z);

        // Rotación del personaje al moverse
        if (direccion.magnitude >= 0.1f)
        {
            float anguloObjetivo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg;
            Quaternion rotacionDeseada = Quaternion.Euler(0f, anguloObjetivo, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionDeseada, Time.fixedDeltaTime * velocidadRotacion);
        }


        // Verificar si está retrocediendo
        bool retrocediendo = Vector3.Dot(transform.forward, direccion) < -0.1f;

        // Animaciones
        if (animator != null)
        {
            animator.SetBool("Moviendose", direccion != Vector3.zero);
            animator.SetBool("Retrocediendo", retrocediendo);
            animator.SetFloat("VelocidadAnim", retrocediendo ? -1f : 1f);
        }

        // Ejecutar salto
        if (saltoSolicitado)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaSalto, rb.linearVelocity.z);
            saltoSolicitado = false;
        }

        // Reproducir pasos si está caminando en el suelo
        /*bool estaCaminando = direccion != Vector3.zero && enElSuelo;
        if (estaCaminando)
        {
            temporizadorPasos -= Time.fixedDeltaTime;
            if (temporizadorPasos <= 0f && sonidoPasos != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoPasos);
                temporizadorPasos = intervaloPasos;
            }
        }
        else
        {
            temporizadorPasos = 0f;
        }*/
    }
}
