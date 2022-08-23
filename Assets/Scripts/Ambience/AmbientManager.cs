using Muzak;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Voxul;

public class AmbientManager : ExtendedMonoBehaviour
{
    public AmbientLightingState CurrentLightingState { get; private set; }
    public AmbientZone CurrentZone { get; private set; }
    public MuzakPlayer Player => GetComponent<MuzakPlayer>();
    public List<MuzakTrack> AmbientTracks;
    public float LightingTransitionSpeed = 1;
    public AmbientLightingState DefaultLighting;
    private AmbientLightingState m_currentTarget;

    private void Start()
    {
        Player.Track = AmbientTracks.Random();
        StartCoroutine(UpdateZone());
        StartCoroutine(PlayAmbientMusic());
        StartCoroutine(PlayAmbientLighting());
        Player?.EventListener.AddListener(OnEvent);
    }

    private void OnEvent(MuzakPlayerEvent.MuzakEventInfo ev)
    {
        if (ev.EventType == MuzakPlayerEvent.eEventType.TrackLoopEnded)
        {
            Player.Track = AmbientTracks.Random();
        }
    }

    [ContextMenu("Apply Default Lighting")]
    public void ApplyDefaultLighting()
    {
        DefaultLighting.Apply();
    }

    IEnumerator PlayAmbientLighting()
    {
        CurrentLightingState = DefaultLighting;
        while (true)
        {
            yield return null;
            if (!CurrentZone)
            {
                m_currentTarget = DefaultLighting;
            }
            else
            {
                m_currentTarget = CurrentZone.AmbientLighting;
            }
            CurrentLightingState = AmbientLightingState.Lerp(CurrentLightingState, m_currentTarget, Time.deltaTime * LightingTransitionSpeed);
            CurrentLightingState.Apply();
        }
    }

    IEnumerator UpdateZone()
    {
        while (true)
        {
            AmbientZone bestZone = null;
            foreach (var zone in AmbientZone.Instances)
            {
                var wBounds = new Bounds(zone.transform.localToWorldMatrix.MultiplyPoint3x4(zone.Bounds.center), zone.Bounds.size);
                if (wBounds.Contains(transform.position) && (!bestZone || bestZone.Priority < zone.Priority))
                {
                    bestZone = zone;
                }
            }

            CurrentZone = bestZone;
            if (CurrentZone)
            {
                CurrentZone.Activate();
            }
            yield return null;
        }
    }

    IEnumerator PlayAmbientMusic()
    {
        while (true)
        {
            if (CurrentZone)
            {
                Player?.Stop();
            }
            if (!CurrentZone && Player.PlayState != MuzakPlayer.ePlayState.Playing)
            {
                Player?.Play();
            }
            yield return null;
        }
    }
}
