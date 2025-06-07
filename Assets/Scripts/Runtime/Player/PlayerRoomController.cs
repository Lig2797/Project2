using Unity.Netcode;
using UnityEngine;

public class PlayerRoomController : NetworkBehaviour
{
    private RoomId currentRoom;
    private NetworkVisibilityRoom _networkVisibilityRoom;
    private void Awake()
    {
        _networkVisibilityRoom = GetComponent<NetworkVisibilityRoom>();
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        UpdateRoom(new RoomId { Type = RoomType.None, Id = -1 });
    }

    // call this by player local instance when generate a cave or a house
    public void UpdateRoom(RoomId newRoom)
    {
        currentRoom = newRoom;
        UpdateRoomServerRpc(newRoom.Type, newRoom.Id);
        _networkVisibilityRoom.RoomId = currentRoom;
    }

    [ServerRpc(RequireOwnership = true)]
    private void UpdateRoomServerRpc(RoomType type, int id)
    {
        RoomId newRoom = new RoomId { Type = type, Id = id };
        RoomManager.Instance.SetClientRoom(OwnerClientId, newRoom);
    }

}
