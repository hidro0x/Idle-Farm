
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace YigitDurmus {

  public class PinchUpdateData {

    public Vector3 pinchCenter;
    public float pinchDistance;
    public float pinchStartDistance;
    public float pinchAngleDelta;
    public float pinchAngleDeltaNormalized;
    public float pinchTiltDelta;
    public float pinchTotalFingerMovement;
  }
}
