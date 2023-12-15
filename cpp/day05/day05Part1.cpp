#include <iostream>
#include <map>
#include <string>

#include "day05.h"

enum ParsingPhase {
    PPNone,
    PPSeeds,
    PPSeedToSoilMap,
    PPSoilToFertilizerMap,
    PPFertilizerToWaterMap,
    PPWaterToLightMap,
    PPLightToTemperatureMap,
    PPTemperatureToHumidityMap,
    PPHumidityToLocationMap,
};

struct Mapping1 {
    long srcMin;
    long srcMax;
    long offset;
};

long map1(long src, const std::vector<Mapping1> &mapping) {
    for (Mapping1 m: mapping) {
        if (src < m.srcMin) continue;
        if (src > m.srcMax) continue;
        return src + m.offset;
    }
    return src;
}

//
// https://adventofcode.com/2023/day/5
//
void day05Part1() {
    std::vector<long> seeds;
    std::vector<Mapping1> seedToSoil;
    std::vector<Mapping1> soilToFertilizer;
    std::vector<Mapping1> fertilizerToWater;
    std::vector<Mapping1> waterToLight;
    std::vector<Mapping1> lightToTemperature;
    std::vector<Mapping1> temperatureToHumidity;
    std::vector<Mapping1> humidityToLocation;

    ParsingPhase parsingPhase = PPNone;

    for (std::string nextToken; std::cin >> nextToken;) {
        std::cout << "[" << nextToken << "]" << '\n';
        if (nextToken == "seeds:") {
            parsingPhase = PPSeeds;
        } else if (nextToken == "seed-to-soil") {
            std::cin >> nextToken; // `map:`
            parsingPhase = PPSeedToSoilMap;
        } else if (nextToken == "soil-to-fertilizer") {
            std::cin >> nextToken; // `map:`
            parsingPhase = PPSoilToFertilizerMap;
        } else if (nextToken == "fertilizer-to-water") {
            std::cin >> nextToken; // `map:`
            parsingPhase = PPFertilizerToWaterMap;
        } else if (nextToken == "water-to-light") {
            std::cin >> nextToken; // `map:`
            parsingPhase = PPWaterToLightMap;
        } else if (nextToken == "light-to-temperature") {
            std::cin >> nextToken; // `map:`
            parsingPhase = PPLightToTemperatureMap;
        } else if (nextToken == "temperature-to-humidity") {
            std::cin >> nextToken; // `map:`
            parsingPhase = PPTemperatureToHumidityMap;
        } else if (nextToken == "humidity-to-location") {
            std::cin >> nextToken; // `map:`
            parsingPhase = PPHumidityToLocationMap;
        } else if (parsingPhase == PPSeeds) {
            seeds.push_back(std::stol(nextToken));
        } else if (parsingPhase == PPNone) {
            // do nothing
        } else {
            long dst = std::stol(nextToken);
            std::cin >> nextToken;
            long src = std::stol(nextToken);
            std::cin >> nextToken;
            long range = std::stol(nextToken);
            switch (parsingPhase) {
                case PPSeedToSoilMap: {
                    seedToSoil.push_back({.srcMin = src, .srcMax = src + range - 1, .offset = dst - src});
                    break;
                }
                case PPSoilToFertilizerMap: {
                    soilToFertilizer.push_back({.srcMin = src, .srcMax = src + range - 1, .offset = dst - src});
                    break;
                }
                case PPFertilizerToWaterMap: {
                    fertilizerToWater.push_back({.srcMin = src, .srcMax = src + range - 1, .offset = dst - src});
                    break;
                }
                case PPWaterToLightMap: {
                    waterToLight.push_back({.srcMin = src, .srcMax = src + range - 1, .offset = dst - src});
                    break;
                }
                case PPLightToTemperatureMap: {
                    lightToTemperature.push_back({.srcMin = src, .srcMax = src + range - 1, .offset = dst - src});
                    break;
                }
                case PPTemperatureToHumidityMap: {
                    temperatureToHumidity.push_back({.srcMin = src, .srcMax = src + range - 1, .offset = dst - src});
                    break;
                }
                case PPHumidityToLocationMap: {
                    humidityToLocation.push_back({.srcMin = src, .srcMax = src + range - 1, .offset = dst - src});
                    break;
                }
            }
            std::cout << '\n';
        }
    }

    long lowerLocationNumber = INT_MAX;

    for (long seed: seeds) {
        std::cout << "Processing seed " << seed << "..." << '\n';
        long soil = map1(seed, seedToSoil);
        std::cout << "soil        : " << soil << '\n';
        long fertilizer = map1(soil, soilToFertilizer);
        std::cout << "fertilizer  : " << fertilizer << '\n';
        long water = map1(fertilizer, fertilizerToWater);
        std::cout << "water       : " << water << '\n';
        long light = map1(water, waterToLight);
        std::cout << "light       : " << light << '\n';
        long temperature = map1(light, lightToTemperature);
        std::cout << "temperature : " << temperature << '\n';
        long humidity = map1(temperature, temperatureToHumidity);
        std::cout << "humidity    : " << humidity << '\n';
        long location = map1(humidity, humidityToLocation);
        std::cout << "location    : " << location << '\n';
        std::cout << '\n';

        lowerLocationNumber = std::min(lowerLocationNumber, location);
    }

    std::cout << "LOWEST LOCATION NUMBER: " << lowerLocationNumber << '\n';
}
