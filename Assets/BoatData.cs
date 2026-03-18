using UnityEngine;

public class BoatData : MonoBehaviour
{
    [HideInInspector] public string vesselName;
    [HideInInspector] public string vesselType;
    [HideInInspector] public string cargo;
    [HideInInspector] public int pob;
    [HideInInspector] public string visaNumber;

    string[] names = { "MV Northern Star", "SS Pacific Trader", "MV Iron Duke", "SS Coral Queen", "MV Blue Horizon", "SS Atlantic Runner", "MV Sea Hawk", "SS Grey Wolf" };
    string[] types = { "Cargo Ship", "Oil Tanker", "Container Vessel", "Bulk Carrier", "Fishing Trawler", "Passenger Ferry", "Research Vessel" };
    string[] cargos = { "General Freight", "Crude Oil", "Timber", "Iron Ore", "Frozen Fish", "Grain", "Coal", "Vehicles", "Chemicals", "Passengers" };

    void Awake()
    {
        vesselName = names[Random.Range(0, names.Length)];
        vesselType = types[Random.Range(0, types.Length)];
        cargo = cargos[Random.Range(0, cargos.Length)];
        pob = Random.Range(4, 32);
        int d1 = Random.Range(1, 9);
        int d2 = Random.Range(1, 9);
        int sum = d1 + d2;
        string correct = d1.ToString() + d2.ToString() + sum.ToString("D" + (sum >= 10 ? 2 : 1));
        // 1 in 10 chance the last digit(s) are wrong
        if (Random.Range(0, 10) == 0)
        {
            int fakeSum = sum + Random.Range(1, 4); // deliberately wrong
            visaNumber = "VIS-" + d1.ToString() + d2.ToString() + fakeSum.ToString("D" + (sum >= 10 ? 2 : 1));
        }
        else
        {
            visaNumber = "VIS-" + correct;
        }
    }
}