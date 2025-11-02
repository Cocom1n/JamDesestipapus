using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CambioEscenaLobizon : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string sceneName = "CuevaTileSet";
    [SerializeField] private string promptMessage = "Presiona E para entrar a la cueva";

    [Header("Referencia UI")]
    [SerializeField] private TextMeshProUGUI promptText; // arrastra aquí el TextTMP de la UI

    private bool playerInside = false;

    void Start()
    {
        // Asegurarnos de que el prompt está oculto al inicio
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Comprobación informativa: avisar si la escena no está en Build Settings
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid())
                Debug.LogWarning($"La escena '{sceneName}' no parece estar en Build Settings. Aun así se intentará cargar.");

            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = true;
        if (promptText != null)
        {
            promptText.text = promptMessage;
            promptText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = false;
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // asegurar que el prompt no queda visible si el objeto se desactiva
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
}