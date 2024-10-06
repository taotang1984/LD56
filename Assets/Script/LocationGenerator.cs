using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Rendering;
using UnityEngine;

public class LocationGenerator : SingletonMonobehaviour<LocationGenerator>
{
    public Location locationPrefab;
    public Location baseLocation;
    public LineRenderer lineRenderer;
    public Vector2 areaSize; // Define the size of the area
    public int numberOfPoints; // Number of random points to generate
    public float minDistanceToOtherPoint = 2f; // Minimum distance between points
    public float minDistanceToBase = 2f;
    public float maxDistanceConnectToBase = 2.5f;
    public List<Location> locations = new();
    [SerializeField] private List<Location> canReachBaseLocations = new();
    public List<ResourceSO> resourceLibrary;
    public List<ResourceSO> itemLibrary;

    protected override void Awake() {
        base.Awake();
        locations = new();
        canReachBaseLocations = new();
        GenerateRandomPoints();
        Debug.Log("done generate points");
        GenerateLines();
        Debug.Log("done generate lines");

    }
    void GenerateRandomPoints()
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector2 randomPoint;
            bool validPoint;            
            do
            {
                randomPoint = new Vector2(
                    Random.Range(-areaSize.x / 2, areaSize.x / 2),
                    Random.Range(-areaSize.y / 2, areaSize.y / 2)
                );

                validPoint = true;

                foreach (var point in points)
                {
                    if (Vector2.Distance(randomPoint, point) < minDistanceToOtherPoint)
                    {
                        validPoint = false;
                        break;
                    }
                }
                if (Vector2.Distance(randomPoint, Vector2.zero) < minDistanceToBase)
                    {
                        validPoint = false;
                    }
                {
                    
                }
            } while (!validPoint);

            points.Add(randomPoint);
            var location = Instantiate(locationPrefab, transform);
            location.transform.position = randomPoint;
            location.number.text = i.ToString();
            location.name = "location_ "+ i.ToString();
            locations.Add(location);
        }
    }

    private void GenerateLines()
    {
        // line to base
        var maxDistance = maxDistanceConnectToBase;
        do
        {
            foreach (var location in locations)
            {
                if (Vector2.Distance(location.transform.position, Vector2.zero) < maxDistanceConnectToBase)
                {
                    canReachBaseLocations.Add(location);
                    var line = Instantiate(lineRenderer, transform);
                    line.SetPosition(0, location.transform.position);
                    line.SetPosition(1, baseLocation.transform.position);
                    location.connectedLocations.Add(baseLocation);
                    baseLocation.connectedLocations.Add(location);
                    location.connectedLines.Add(line);
                    baseLocation.connectedLines.Add(line);
                }
            }
            maxDistance += 0.5f;
            
        } while (canReachBaseLocations.Count == 0);


        // line to other locations
        foreach (var location in locations)
        {
            // get the closest location to the current location
            var closestLocation = FindClosestLocation(location, locations);

            // connect line between locations
            Connect(location, closestLocation);

        }

        //get reachable locations for each location
        UpdateReachableLocations();


        // draw extra lines if not connected to base
        foreach (var location in locations)
        {
            
            if(!location.reachableLocations.Contains(baseLocation))
            {
                // find the closet location not in its reachble locations

                var minDistance = 999f;
                var closestLocation = locations[0];
                foreach(var loc in locations)
                {
                    if(!location.reachableLocations.Contains(loc))
                    {
                        if(Vector2.Distance(loc.transform.position, location.transform.position) < minDistance)
                        {
                            minDistance = Vector2.Distance(loc.transform.position, location.transform.position);
                            closestLocation = loc;
                        }
                    }
                }
                if(minDistance > 3.5f) continue;
                Connect(closestLocation, location);
                UpdateReachableLocations();
            }
        }

        // init resource for each location
        foreach (var location in locations)
        {
            ResourceSO resource = null;
            if(location.connectedLines.Count == 1)
            {
                resource = resourceLibrary[Random.Range(0, resourceLibrary.Count)];
            }
            else
            {
                var randomValue = Random.value;
                if(randomValue < 0.3f)
                {
                    resource = resourceLibrary[Random.Range(0, resourceLibrary.Count)];
                }
                else if(randomValue > 0.3f && randomValue < 0.45f)
                {
                    resource = itemLibrary[0];
                }
                else if(randomValue > 0.45f && randomValue < 0.55f)
                {
                    resource = itemLibrary[1];
                }
            }
            location.Init(resource);
        }

    }

    public Location FindClosestLocation(Location targetLocation, List<Location> targetGroup)
    {
        var locationWithoutTarget = new List<Location>(targetGroup);
        if (targetGroup.Contains(targetLocation))
        {
            locationWithoutTarget.Remove(targetLocation);
        }
        Location closestLocation = locationWithoutTarget[0];
        float closestDistance = Vector2.Distance(targetLocation.transform.position, closestLocation.transform.position);

        foreach (var location in locationWithoutTarget)
        {
            float distance = Vector2.Distance(targetLocation.transform.position, location.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestLocation = location;
            }
        }

        return closestLocation;
    }

    public void Connect(Location location1, Location location2)
    {
        if(!location1.connectedLocations.Contains(location2))
        {
            location1.connectedLocations.Add(location2);
        }
        if (!location2.connectedLocations.Contains(location1))
        {
            location2.connectedLocations.Add(location1);
            // draw line between locations
            var line = Instantiate(lineRenderer, transform);
            line.SetPosition(0, location1.transform.position);
            line.SetPosition(1, location2.transform.position);
            location1.connectedLines.Add(line);
            location2.connectedLines.Add(line);
        }
    }
    
    public void UpdateReachableLocations()
    {
        foreach(var location in locations)
        {
            HashSet<Location> visited = new HashSet<Location>();
            GetReachableLocations(location, visited);
            location.reachableLocations = visited.ToList();
        }
    }


    void GetReachableLocations(Location location, HashSet<Location> visited)
    {
        if (visited.Contains(location))
        {
            return;
        }

        visited.Add(location);

        foreach (var connectedLocation in location.connectedLocations)
        {
            GetReachableLocations(connectedLocation, visited);
        }
    }

}