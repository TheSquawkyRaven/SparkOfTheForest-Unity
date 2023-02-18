using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public Transform TR;
    public Player Player;

    private void LateUpdate()
    {
        Vector3 pos = Player.TR.localPosition;
        pos.z = -10;
        TR.localPosition = pos;
    }

}
