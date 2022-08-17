using Common;
using Muzak;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmbientZone : TrackedObject<AmbientZone>
{
    public MuzakPlayer Player => GetComponent<MuzakPlayer>();
    public float ActivationAmount { get; private set; }
    public List<AudioSource> AmbientDirectSources;
    public List<MuzakTrack> AmbientTracks;
    public Bounds Bounds;

    private void Start()
    {
        if (AmbientTracks.Any())
        {
            Player.Track = AmbientTracks.Random();
        }
        Player.EventListener.AddListener(OnEvent);
    }

    private void OnEvent(MuzakPlayerEvent.MuzakEventInfo ev)
    {
        if (ev.EventType == MuzakPlayerEvent.eEventType.TrackLoopEnded)
        {
            Player.Track = AmbientTracks.Random();
        }
    }

    public void Activate()
    {
        ActivationAmount = Mathf.Clamp01(ActivationAmount + 1);
    }

    private void Update()
    {
        ActivationAmount = Mathf.Clamp01(ActivationAmount - Time.deltaTime);
        if (ActivationAmount <= 0)
        {
            if (Player.PlayState == MuzakPlayer.ePlayState.Playing)
            {
                Player.Stop();
            }
            foreach(var directSource in AmbientDirectSources)
            {
                directSource.volume = Mathf.MoveTowards(directSource.volume, 0, Time.deltaTime);
                if(directSource.volume <= 0)
                {
                    directSource.Stop();
                }
            }
        }
        else
        {
            if(Player.PlayState != MuzakPlayer.ePlayState.Playing)
            {
                Player.Play();
            }
            foreach (var directSource in AmbientDirectSources)
            {
                if (!directSource.isPlaying)
                {
                    directSource.Play();
                }
                directSource.volume = Mathf.MoveTowards(directSource.volume, 1, Time.deltaTime);
                
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
}