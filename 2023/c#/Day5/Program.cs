using System.Collections.Generic;
using Utilities;

internal class Program
{

    enum MapType { Unknown, Seeds, SeedToSoil, SoilToFertilizer, FertilizerToWater, WaterToLight, LightToTemperature, TemperatureToHumidity, HumidityToLocation }

    class DestinationFinder
    {
        public List<MapFormat> maps { get; set; }

        public DestinationFinder(List<MapFormat> maps)
        {
            this.maps = maps;
        }

        public long GetDestination(long source)
        {
            MapFormat element = this.maps.FirstOrDefault(x => x.SourceInRange(source));

            if (element is null) return -1;
            return element.GetDestination(source);
        }
    }

    class SeedFormat
    {
        public long SeedStart { get; set; }
        public long RangeLength { get; set; }
    }

    class MapFormat
    {
        public MapType Type { get; set; }
        public long DestinationStart { get; set; }
        public long DestinationEnd { get; set; }

        public long SourceStart { get; set; }
        public long SourceEnd { get; set; }

        public long RangeLength { get; set; }

        public MapFormat(MapType type, long dStart, long sStart, long range)
        {
            this.Type = type;
            this.DestinationStart = dStart;
            this.SourceStart = sStart;
            this.RangeLength = range;

            this.DestinationEnd = this.DestinationStart + this.RangeLength;
            this.SourceEnd = this.SourceStart + this.RangeLength;
            
        }

        public long GetDestination(long source)
        {
            if(SourceInRange(source))
                return this.DestinationStart + (source - this.SourceStart);

            return -1;
        }
        public bool SourceInRange(long source)
        {
            return source >= this.SourceStart && source <= this.SourceEnd;
        }

        public (long, long) GetSourceRange()
        {
            return (this.SourceStart, this.SourceEnd);
        }

        public (long, long) GetDestinationRange()
        {
            return (this.DestinationStart, this.DestinationEnd);
        }
    }

    private static void Main(string[] args)
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\input_day5.txt");
        var lines = FileHandling.ReadInputFile(path);

        // Data structures
        List<SeedFormat> seedList = new List<SeedFormat>();

        List<MapFormat> seedToSoilMap = new List<MapFormat>();
        List<MapFormat> soilToFertilizerMap = new List<MapFormat>();
        List<MapFormat> fertilizerToWaterMap = new List<MapFormat>();
        List<MapFormat> waterToLightMap = new List<MapFormat>();
        List<MapFormat> lightToTemperatureMap = new List<MapFormat>();
        List<MapFormat> temperatureToHumidityMap = new List<MapFormat>();
        List<MapFormat> humidityToLocationMap = new List<MapFormat>();

        MapType currentType = MapType.Unknown;

        foreach (string line in lines)
        {
            if (line.Trim() == "") continue;

            var lineMapType = GetRowMapType(line);

            // The "currentType" will only be set when the map type is known
            switch (lineMapType)
            {
                case MapType.Unknown:
                    // This is the data for the different types of maps

                    List<long> values = line.Trim().Split(" ").Select(x => long.Parse(x)).ToList();
                    switch (currentType)
                    {
                        case MapType.SeedToSoil:
                            seedToSoilMap.Add(new MapFormat(currentType, values[0], values[1], values[2]));
                            break;
                        case MapType.SoilToFertilizer:
                            soilToFertilizerMap.Add(new MapFormat(currentType, values[0], values[1], values[2]));
                            break;
                        case MapType.FertilizerToWater:
                            fertilizerToWaterMap.Add(new MapFormat(currentType, values[0], values[1], values[2]));
                            break;
                        case MapType.WaterToLight:
                            waterToLightMap.Add(new MapFormat(currentType, values[0], values[1], values[2]));
                            break;
                        case MapType.LightToTemperature:
                            lightToTemperatureMap.Add(new MapFormat(currentType, values[0], values[1], values[2]));
                            break;
                        case MapType.TemperatureToHumidity:
                            temperatureToHumidityMap.Add(new MapFormat(currentType, values[0], values[1], values[2]));
                            break;
                        case MapType.HumidityToLocation:
                            humidityToLocationMap.Add(new MapFormat(currentType, values[0], values[1], values[2]));
                            break;
                        default:
                            break;
                    }
                    break;

                case MapType.Seeds:
                    currentType = MapType.Seeds; // Seeds row is unique, since it contains the number of seeds
                    //seedList.AddRange(GetSeedNumbers_Part1(line));
                    seedList.AddRange(GetSeedNumbers_Part2(line));
                    break;

                default:
                    currentType = lineMapType;
                    break;
            }

            // Console.WriteLine($"{currentType.ToString()} | {line}");

        }

        // Summary
        Console.WriteLine($"Seeds List: {seedList.Count}");
        Console.WriteLine($"SeedToSoil: {seedToSoilMap.Count}");
        Console.WriteLine($"SoilToFertilizer: {soilToFertilizerMap.Count}");
        Console.WriteLine($"FertilizerToWater: {fertilizerToWaterMap.Count}");
        Console.WriteLine($"WaterToLight: {waterToLightMap.Count}");
        Console.WriteLine($"LightToTemperature: {lightToTemperatureMap.Count}");
        Console.WriteLine($"TemperatureToHumidity: {temperatureToHumidityMap.Count}");
        Console.WriteLine($"HumidityToLocation: {humidityToLocationMap.Count}");

        // Get Locations For Seeds
        DestinationFinder findSoils = new DestinationFinder(seedToSoilMap);
        DestinationFinder findFertilizers = new DestinationFinder(soilToFertilizerMap);
        DestinationFinder findWaters = new DestinationFinder(fertilizerToWaterMap);
        DestinationFinder findLights = new DestinationFinder(waterToLightMap);
        DestinationFinder findTemperatures = new DestinationFinder(lightToTemperatureMap);
        DestinationFinder findHumidities = new DestinationFinder(temperatureToHumidityMap);
        DestinationFinder findLocations = new DestinationFinder(humidityToLocationMap);

        Console.WriteLine("---- Processing ----");

        long processingCount = 0;
        long lowestLocation = long.MaxValue;

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        Parallel.ForEach(seedList, sf =>
        {
            for (long seed = sf.SeedStart; seed <= sf.SeedStart + sf.RangeLength; seed++)
            {
                processingCount++;
                if (processingCount % 10000000 == 0)
                    Console.WriteLine($"Locations Processed: {processingCount}");

                long soil = findSoils.GetDestination(seed);
                if (soil == -1) soil = seed; // Any source numbers that aren't mapped correspond to the same destination number. So, seed number 10 corresponds to soil number 10.

                long fertilizer = findFertilizers.GetDestination(soil);
                if (fertilizer == -1) fertilizer = soil;

                long water = findWaters.GetDestination(fertilizer);
                if (water == -1) water = fertilizer;

                long light = findLights.GetDestination(water);
                if (light == -1) light = water;

                long temperature = findTemperatures.GetDestination(light);
                if (temperature == -1) temperature = light;

                long humidity = findHumidities.GetDestination(temperature);
                if (humidity == -1) humidity = temperature;

                long location = findLocations.GetDestination(humidity);
                if (location == -1) location = humidity;

                // Found one!
                lowestLocation = Math.Min(location, lowestLocation);
            }
        });
        watch.Stop();

        Console.WriteLine($"# Records Processed: {processingCount}");
        Console.WriteLine($"Lowest Location: {lowestLocation}");

        long duration_milliseconds = watch.ElapsedMilliseconds;
        long duration_seconds = watch.ElapsedMilliseconds / 1000;
        long duration_minutes = duration_seconds / 60;

        Console.WriteLine($"Execution Time (m:s:ms) : {duration_minutes}:{duration_seconds % 60}:{duration_milliseconds % 1000}");

        // Part 1: 340994526

        // Part 2: 77864447 - Too high
        // Part 2: 52210644

    }

    private static MapType GetRowMapType(string line)
    {
        if (line.StartsWith("seeds:")) return MapType.Seeds;
        if (line.StartsWith("seed-to-soil map:")) return MapType.SeedToSoil;
        if (line.StartsWith("soil-to-fertilizer map:")) return MapType.SoilToFertilizer;
        if (line.StartsWith("fertilizer-to-water map:")) return MapType.FertilizerToWater;
        if (line.StartsWith("water-to-light map:")) return MapType.WaterToLight;
        if (line.StartsWith("light-to-temperature map:")) return MapType.LightToTemperature;
        if (line.StartsWith("temperature-to-humidity map:")) return MapType.TemperatureToHumidity;
        if (line.StartsWith("humidity-to-location map:")) return MapType.HumidityToLocation;

        return MapType.Unknown;   // Busy Processing
    }


    private static List<SeedFormat> GetSeedNumbers_Part1(string line)
    {
        List<SeedFormat> list = new List<SeedFormat>();

        string seedString = line.Split(":")[1].Trim();
        List<long> seedParts = seedString.Split(" ").Select(x => long.Parse(x)).ToList();


        for (int i = 0; i < seedParts.Count; i++)
        {
            list.Add(new SeedFormat() { SeedStart = seedParts[i], RangeLength = 0 });
        }

        return list;

    }

    private static List<SeedFormat> GetSeedNumbers_Part2(string line)
    {
        List<SeedFormat> list = new List<SeedFormat>();


        string seedString = line.Split(":")[1].Trim();
        List<long> seedParts = seedString.Split(" ").Select(x => long.Parse(x)).ToList();


        for (int i = 0; i < seedParts.Count; i = i + 2)
        {
            list.Add(new SeedFormat() { SeedStart = seedParts[i], RangeLength = seedParts[i + 1] });
        }

        return list;
    }

}

/*
foreach(SeedFormat sf in seedList)
        {
            for (long seed = sf.SeedStart; seed <= sf.SeedStart + sf.RangeLength; seed++)
            {
                processingCount++;
                if (processingCount % 10000000 == 0)
                    Console.WriteLine($"Locations Processed: {processingCount}");

                long soil = findSoils.GetDestination(seed);
                if (soil == -1) soil = seed; // Any source numbers that aren't mapped correspond to the same destination number. So, seed number 10 corresponds to soil number 10.

                long fertilizer = findFertilizers.GetDestination(soil);
                if (fertilizer == -1) fertilizer = soil;

                long water = findWaters.GetDestination(fertilizer);
                if (water == -1) water = fertilizer;

                long light = findLights.GetDestination(water);
                if (light == -1) light = water;

                long temperature = findTemperatures.GetDestination(light);
                if (temperature == -1) temperature = light;

                long humidity = findHumidities.GetDestination(temperature);
                if (humidity == -1) humidity = temperature;

                long location = findLocations.GetDestination(humidity);
                if (location == -1) location = humidity;

                // Found one!
                locationCount++;
                if (location < lowestLocation) lowestLocation = location;
                
            }
        }
*/