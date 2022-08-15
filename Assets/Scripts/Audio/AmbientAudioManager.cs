using Muzak;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Voxul;

public class AmbientAudioManager : ExtendedMonoBehaviour
{
    public MuzakPlayer Player => GetComponent<MuzakPlayer>();
    public List<MuzakTrack> AmbientTracks;

    private void Start()
    {
        Player.Track = AmbientTracks.Random();
        StartCoroutine(PlayAmbient());
        Player?.EventListener.AddListener(OnEvent);
    }

    private void OnEvent(MuzakPlayerEvent.MuzakEventInfo ev)
    {
        if (ev.EventType == MuzakPlayerEvent.eEventType.TrackLoopEnded)
        {
            Player.Track = AmbientTracks.Random();
        }
    }

    IEnumerator PlayAmbient()
    {
        while (true)
        {
            var anyZone = false;
            foreach (var zone in AmbientZone.Instances)
            {
                var wBounds = new Bounds(zone.transform.localToWorldMatrix.MultiplyPoint3x4(zone.Bounds.center), zone.Bounds.size);
                if (wBounds.Contains(transform.position))
                {
                    anyZone = true;
                    zone.Activate();
                    break;
                }
            }
            if (anyZone)
            {
                Player?.Stop();
            }
            if (!anyZone && Player.PlayState != MuzakPlayer.ePlayState.Playing)
            {
                Player?.Play();
            }
            yield return null;
        }
    }
}
