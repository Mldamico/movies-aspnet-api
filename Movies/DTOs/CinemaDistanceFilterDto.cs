using System.ComponentModel.DataAnnotations;

namespace Movies.DTOs;

public class CinemaDistanceFilterDto
{
     
    [Range(-90,90)]
    public double Latitude { get; set; }
    [Range(-180,180)]
    public double Longitude { get; set; }

    private int _distanceKm = 10;
    private int _maxDistance = 50;

    public int DistanceKm
    {
        get
        {
            return _distanceKm;
        }
        set
        {
            _distanceKm = (value > _maxDistance) ? _maxDistance : value;
        }
    }


}