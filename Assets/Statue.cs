using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Statue : MonoBehaviour
{
    public AudioSource statueAudioSource;
    public AudioClip groundImpactSound;
    private bool hasPlayedSound = false;
    public string nextSceneName;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !hasPlayedSound)
        {
            if (statueAudioSource != null && groundImpactSound != null)
            {
                statueAudioSource.PlayOneShot(groundImpactSound);
                hasPlayedSound = true;  // Prevent sound from playing multiple times
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        }
    }
}
