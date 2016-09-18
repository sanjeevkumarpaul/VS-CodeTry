using System;
using System.Collections.Generic;
using System.Linq;

namespace AvenueCodeTest
{
    public class Gps
    {
        public int City {get; set;}
        public int Distance {get; set;}
    }
 
    public class DataStruct
    {    
        public static List<Gps> gps = new List<Gps>();
        
        /// <summary>
        /// Adds new city with new distance or updates existing city with shorter distance.
        /// </summary>
        /// <param name="cityId">City identifier.</param>
        /// <param name="distance">Distance.</param>
        public void AddOrUpdate(int cityId, int distance)
        {
            var currentGps = gps.Find(g => g.City == cityId);

            if (currentGps == null)        
                gps.Add(new Gps{ City = cityId, Distance = distance });
            else
                currentGps.Distance = distance;
                
        }

    /// <summary>
    /// Retrieves and removes city with smallest distance and returns it's id.
    /// </summary>
    /// <returns>The closest city identifier.</returns>
    public int PollClosestCity()
    {
        if (gps.Count <= 0 ) throw new Exception("There are no cities/distance mapped into GPS System. Please add a few and try to experiment");
        
        int minDistance = gps.Min(g => g.City + g.Distance);
        var orderedGps = gps.Where( g => ( g.City + g.Distance) == minDistance).OrderBy(g => g.Distance);
        
        var closestCity = orderedGps.Single( g => g.City > 0  );

        gps.Remove(closestCity);    //Removes the found one.

        return closestCity.City;
    }   

    public static void Try() {
        DataStruct ds = new DataStruct();

        ds.AddOrUpdate(1, 3);
        ds.AddOrUpdate(2, 2);
        ds.AddOrUpdate(1, 1);

        Console.WriteLine(ds.PollClosestCity());
        Console.WriteLine(ds.PollClosestCity());
    }
}


}