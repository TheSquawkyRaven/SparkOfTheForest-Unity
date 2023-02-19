using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public Transform TR;
    public Player Player;

    public float lerpTime;

    public bool TrackPlayer = true;
    public bool TrackCampfire = false;

    public Vector3 TrackingCampfireOffset;
    private Vector3 TrackingCampfirePos;

    private void LateUpdate()
    {
        if (TrackPlayer)
        {
            Vector3 pos = Player.TR.localPosition;
            pos.z = -10;
            TR.localPosition = Vector3.Lerp(TR.localPosition, pos, lerpTime);
        }
        else if (TrackCampfire)
        {
            TR.localPosition = Vector3.Lerp(TR.localPosition, TrackingCampfirePos + TrackingCampfireOffset, lerpTime);
        }
    }

    public void SetTrackCampfire(Campfire campfire)
    {
        TrackCampfire = true;
        TrackPlayer = false;
        TrackingCampfirePos = campfire.TR.localPosition;
        TrackingCampfirePos.z = -10;
    }

    public void SetTrackPlayer()
    {
        TrackPlayer = true;
        TrackCampfire = false;
    }

}
