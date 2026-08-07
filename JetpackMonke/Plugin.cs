using BepInEx;
using BepInEx.Configuration;
using GorillaLocomotion;
using UnityEngine;

namespace JetpackMonke;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    private ConfigEntry<ControllerInput> _jetpackInput;
    private ConfigEntry<float> _jetpackForce;

    private bool _enabled = true;

    private void Awake()
    {
        _jetpackInput = Config.Bind("General", "Input", ControllerInput.RightControllerPrimaryButton, "Jetpack activation button");
        _jetpackForce = Config.Bind("General", "Force", 20f, "Force of the jetpack");
    }

    public void OnEnable() => _enabled = true;
    public void OnDisable() => _enabled = false;

    private void FixedUpdate()
    {
        if(!_enabled)
            return;
        
        if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.GameModeString.Contains("MODDED"))
            return;

        if (!IsInputPressed(_jetpackInput.Value))
            return;
        
        GTPlayer.Instance.AddForce(GTPlayer.Instance.transform.up * _jetpackForce.Value, ForceMode.Force);
    }

    private bool IsInputPressed(ControllerInput input) => input switch
    {
        ControllerInput.RightControllerPrimaryButton   => ControllerInputPoller.instance.rightControllerPrimaryButton,
        ControllerInput.LeftControllerPrimaryButton    => ControllerInputPoller.instance.leftControllerPrimaryButton,
        ControllerInput.RightControllerSecondaryButton => ControllerInputPoller.instance.rightControllerSecondaryButton,
        ControllerInput.LeftControllerSecondaryButton  => ControllerInputPoller.instance.leftControllerSecondaryButton,
        ControllerInput.RightControllerTrigger         => ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f,
        ControllerInput.LeftControllerTrigger          => ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f,
        ControllerInput.RightControllerGrip            => ControllerInputPoller.instance.rightGrab,
        ControllerInput.LeftControllerGrip             => ControllerInputPoller.instance.leftGrab,
        _ => false
    };
}

public static class PluginInfo
{
    public const string Name = "JetpackMonke";
    public const string Guid = "xyz.pl2w.jetpack";
    public const string Version = "1.0.0";
}

public enum ControllerInput
{
    RightControllerPrimaryButton,
    LeftControllerPrimaryButton,
    RightControllerSecondaryButton,
    LeftControllerSecondaryButton,
    RightControllerTrigger,
    LeftControllerTrigger,
    RightControllerGrip,
    LeftControllerGrip
}