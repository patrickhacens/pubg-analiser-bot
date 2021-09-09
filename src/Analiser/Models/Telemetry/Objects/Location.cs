namespace PUBG.Models.Telemetry
{
    /// <summary>
    /// Location values are measured in centimeters.
    /// (0,0) is at the top-left of each map.
    /// The range for the X and Y axes is 0 - 816,000 for Erangel and Miramar
    /// The range for the X and Y axes is 0 - 612,000 for Vikendi.
    /// The range for the X and Y axes is 0 - 408,000 for Sanhok.
    /// The range for the X and Y axes is 0 - 204,000 for Karakin and Range.
    /// </summary>
    public class Location
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override string ToString() => $"{X}, {Y}, {Z}";
    }
}
