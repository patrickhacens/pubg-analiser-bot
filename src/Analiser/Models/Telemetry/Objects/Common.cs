namespace PUBG.Models.Telemetry
{
    /// <summary>
    /// IsGame = 0 -> Before lift off
    /// IsGame = 0.1 -> On airplane
    /// IsGame = 0.5 -> When there’s no ‘zone’ on map(before game starts)
    /// IsGame = 1.0 -> First safezone and bluezone appear
    /// IsGame = 1.5->First bluezone shrinks
    /// IsGame = 2.0 -> Second bluezone appears
    /// IsGame = 2.5->Second bluezone shrinks
    /// ...
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Represents the phase of the game defined by the status of bluezone and safezone:
        /// </summary>
        public float IsGame { get; set; }
    }
}
