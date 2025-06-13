namespace Supermodel.Persistence.Entities.ValueTypes;

public class Geolocation : ValueObject
{
    #region Constructors
    public Geolocation() { }
    public Geolocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    #endregion

    #region Properties
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    #endregion
}