using System;
public class MoveEvaluationWeights
{
    public float KingThreatWeight = 250f;
    public float LineAlignmentWeight = 25f;
    public float CenterWeight = 4f;
    public float MobilityWeight = 1.5f;
    public float KingDistanceWeight = 3f;
    public float CooperationWeight = 0.75f;
    public float ShotgunDangerWeight = 75f;
    public float KingCaptureRiskWeight = 80f;
    public float NearBestTolerance = 3f;
    public float RecentlyMovedPenalty = 60f;
}
