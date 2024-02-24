using UnityEngine;
using System.Collections;
namespace cowsins.BulletHell
{ 
 /// <summary>
 /// Handles sounds. The object that has this component attached is generally included in the player controller prefab.
 /// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private void Awake()
    {
        //Handle singleton
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null; 
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this.gameObject);
        }
    /// <summary>
    /// Plays a custom sound on the SoundManager, given the custom parameters. In most of the cases, Bullet Hell Engine already handles this.
    /// </summary>
    /// <param name="clip">Audio (SFX) that you want to play.</param>
    /// <param name="delay">Units of time to play the sound.</param>
    /// <param name="pitch">Specifies the pitch of the AudioSource.</param>
    /// <param name="spatialBlend">SpatialBlend goes from 0 to 1. 0 meaning completely 2D sound and 1 meaning completely 3D sound</param>
    public void PlaySound(AudioClip clip,float delay, float pitch,float spatialBlend)
    {
        StartCoroutine(Play(clip,delay,pitch,spatialBlend)); 
    }

    // Play the clip with the specified settings
    private IEnumerator Play(AudioClip clip, float delay, float pitch, float spatialBlend)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<AudioSource>().spatialBlend = spatialBlend;
        GetComponent<AudioSource>().pitch = 1 + Random.Range(-pitch, pitch);
        GetComponent<AudioSource>().PlayOneShot(clip); 
        yield return null;
    }
}
}
