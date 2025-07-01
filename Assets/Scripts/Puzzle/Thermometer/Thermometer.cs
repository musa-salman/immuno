using UnityEngine;
using System.Collections.Generic;

public class Thermometer : MonoBehaviour
{
    public List<ThermometerSegment> segments = new();

    public void AddSegment(ThermometerSegment segment)
    {
        segments.Add(segment);
        segment.parentThermometer = this;
    }
}
