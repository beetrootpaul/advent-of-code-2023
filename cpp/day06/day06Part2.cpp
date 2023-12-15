#include <iostream>
#include <sstream>
#include <string>

#include "day06.h"

//
// https://adventofcode.com/2023/day/6
//
void day06Part2() {
    std::vector<Race> races;

    std::string line1;
    getline(std::cin, line1);
    std::istringstream ss1(line1);
    std::string header1;
    ss1 >> header1;
    double nextTime;
    while (ss1 >> nextTime) {
        races.push_back({.time = nextTime, .distance = 0});
    }

    std::string line2;
    getline(std::cin, line2);
    std::istringstream ss2(line2);
    std::string header2;
    ss2 >> header2;
    double nextDistance;
    int i = 0;
    while (ss2 >> nextDistance) {
        races.at(i).distance = nextDistance;
        ++i;
    }

    long result = -1;

    for (int j = 0; j < races.size(); ++j) {
        std::cout << "ANALYZING RACE " << j << '\n';
        Race race = races.at(j);
        std::cout << "time: " << race.time << '\n';
        std::cout << "dist: " << race.distance << '\n';

        double a = -1;
        double b = race.time;
        double c = -race.distance;

        double delta = b * b - 4 * a * c;
        std::cout << "delta = " << delta << '\n';

        long numberOfWaysToBeatThisRace = 0;
        if (delta >= 0) {
            double xMin = std::ceil((-b + std::sqrt(delta)) / 2 * a + 0.0000001);
            double xMax = std::floor((-b - std::sqrt(delta)) / 2 * a - 0.0000001);
            std::cout << "xMin / xMax = " << xMin << " / " << xMax << '\n';
            if (xMax >= xMin) {
                numberOfWaysToBeatThisRace = xMax - xMin + 1;
                if (result < 0) {
                    result = 1;
                }
                result *= numberOfWaysToBeatThisRace;
            }
        }

        std::cout << "# = " << numberOfWaysToBeatThisRace << '\n';

        std::cout << '\n';
    }

    std::cout << "RESULT: " << result << '\n';
}
