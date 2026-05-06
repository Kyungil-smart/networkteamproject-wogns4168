using Unity.Netcode;
using UnityEngine;

public class CharacterScaleSync : NetworkBehaviour
{
    public NetworkVariable<Vector3> TargetScale = new NetworkVariable<Vector3>(
        Vector3.one, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        transform.localScale = TargetScale.Value;
        
        TargetScale.OnValueChanged += OnScaleChanged;
    }

    public override void OnNetworkDespawn()
    {
        TargetScale.OnValueChanged -= OnScaleChanged;
    }

    private void OnScaleChanged(Vector3 prev, Vector3 current)
    {
        transform.localScale = current;
    }
}