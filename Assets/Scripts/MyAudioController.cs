using System.Collections.Generic;
using UnityEngine;

public class MyAudioController : MonoBehaviour
{

    public readonly Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>(); // Dictionnaire permettant de pré-charger tout les sons du personnage

    void Awake()
    {
        // Pré-chargement des sons du personnage
        AudioClips.Add("hit_combo1", Resources.Load<AudioClip>("Sounds/prepunch1"));
        AudioClips.Add("fat_kick", Resources.Load<AudioClip>("Sounds/strongkick"));
        AudioClips.Add("fat_punch", Resources.Load<AudioClip>("Sounds/strongpunch"));
        AudioClips.Add("grounded", Resources.Load<AudioClip>("Sounds/groundhit"));
        AudioClips.Add("jump", Resources.Load<AudioClip>("Sounds/jump"));
        AudioClips.Add("aura", Resources.Load<AudioClip>("Sounds/kamehameha_fire"));
        AudioClips.Add("projection", Resources.Load<AudioClip>("Sounds/throw"));
        AudioClips.Add("weak_punch", Resources.Load<AudioClip>("Sounds/weakpunch"));
        AudioClips.Add("weak_kick", Resources.Load<AudioClip>("Sounds/weakkick"));
        AudioClips.Add("hit_projection", Resources.Load<AudioClip>("Sounds/powerHit2"));
        AudioClips.Add("swoop_hit", Resources.Load<AudioClip>("Sounds/swoop_hit"));
        AudioClips.Add("teleport1", Resources.Load<AudioClip>("Sounds/teleport00"));
        AudioClips.Add("spin", Resources.Load<AudioClip>("Sounds/spin"));
        // AudioClips.Add("teleportation", Resources.Load<AudioClip>("Sounds/"));
    }
}
