namespace Adobe.Target.Client.OnDevice
{
    using System.Threading.Tasks;
    using Adobe.Target.Delivery.Model;

    /// <summary>
    /// Geo Client
    /// </summary>
    public interface IGeoClient
    {
        /// <summary>
        /// Look up Geo data
        /// </summary>
        /// <param name="geo">Geo with IP address</param>
        /// <returns>Geo with location data</returns>
        Task<Geo> LookupGeoAsync(Geo geo);
    }
}
