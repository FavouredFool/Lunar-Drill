using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillianMetricManager : MonoBehaviour
{
    [System.Serializable]
    public class DrillianMetricList
    {
        public List<DrillianMetricEntry> Entries = new();
    }
    
    [System.Serializable]
    public class DrillianMetricEntry
    {
        public int OutsideActivations;
        public int InsideActivations;

        public int DormantActivations;
        public int ChargingActivations;
        public int AttackActivations;
        
        public DrillianMetricEntry(int outsideActivations, int insideActivations, int dormantActivations, int chargingActivations, int attackActivations)
        {
            OutsideActivations = outsideActivations;
            InsideActivations = insideActivations;
            DormantActivations = dormantActivations;
            ChargingActivations = chargingActivations;
            AttackActivations = attackActivations;
        }
    }
}
