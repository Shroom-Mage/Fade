using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    [EventRef]
    private string music;
    private FMOD.Studio.EventInstance musicInstance;
    [SerializeField]
    [EventRef]
    private string footsteps;
    private FMOD.Studio.EventInstance footstepsInstance;
    [SerializeField]
    [EventRef]
    private string pickup;
    private FMOD.Studio.EventInstance pickupInstance;
    [SerializeField]
    [EventRef]
    private string putdown;
    private FMOD.Studio.EventInstance putdownInstance;

    [SerializeField]
    [ParamRef]
    public string variationsParameter;
    [SerializeField]
    [ParamRef]
    public string musicFadeParameter;

    private void Awake()
    {
        musicInstance = RuntimeManager.CreateInstance(music);
        footstepsInstance = RuntimeManager.CreateInstance(footsteps);
        pickupInstance = RuntimeManager.CreateInstance(pickup);
        putdownInstance = RuntimeManager.CreateInstance(putdown);
    }

    public void PlayMusicEventInstance()
    {
        musicInstance.start();
    }

    public void PlayFootstepsEventInstance(GameObject player)
    {
        RuntimeManager.PlayOneShotAttached(footsteps, player);
    }

    public void PlayPickupEventInstance(GameObject player)
    {
        RuntimeManager.PlayOneShotAttached(pickup, player);
    }

    public void PlayPutdownEventInstance(GameObject player)
    {
        RuntimeManager.PlayOneShotAttached(putdown, player);
    }

    public void StopMusicEventInstances()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }

    public void StopSFXEventInstances()
    {
        footstepsInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        footstepsInstance.release();
        pickupInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        pickupInstance.release();
        putdownInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        putdownInstance.release();
    }

    public void SetVariationsParameter(float variationsParameterValue)
    {
        RuntimeManager.StudioSystem.setParameterByName(variationsParameter, variationsParameterValue);
    }

    public void SetMusicFadeParameter(float musicFadeParameterValue)
    {
        RuntimeManager.StudioSystem.setParameterByName(musicFadeParameter, musicFadeParameterValue);
    }
}
