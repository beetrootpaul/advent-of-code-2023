#include <iostream>
#include <sstream>
#include <string>

#include "day06.h"

//
// https://adventofcode.com/2023/day/6
//
void day06Part2() {
    std::string line1;
    getline(std::cin, line1);
    std::istringstream ss1(line1);
    std::string header1;
    ss1 >> header1;
    std::string nextTime;
    std::string raceTimeAsText;
    while (ss1 >> nextTime) {
        raceTimeAsText.append(nextTime);
    }
    double raceTime = std::stod(raceTimeAsText);

    std::string line2;
    getline(std::cin, line2);
    std::istringstream ss2(line2);
    std::string header2;
    ss2 >> header2;
    std::string nextDistance;
    std::string raceDistanceAsText;
    while (ss2 >> nextDistance) {
        raceDistanceAsText.append(nextDistance);
    }
    double raceDistance = std::stod(raceDistanceAsText);

    long result = -1;

    std::cout << "time: " << raceTime << '\n';
    std::cout << "dist: " << raceDistance << '\n';

    double a = -1;
    double b = raceTime;
    double c = -raceDistance;

    double delta = b * b - 4 * a * c;
    std::cout << "delta = " << delta << '\n';

    long numberOfWaysToBeatThisRace = 0;
    if (delta >= 0) {
        double xMin = std::ceil((-b + std::sqrt(delta)) / 2 * a + 0.0000001);
        double xMax = std::floor((-b - std::sqrt(delta)) / 2 * a - 0.0000001);
        std::cout << "xMin / xMax = " << xMin << " / " << xMax << '\n';
        if (xMax >= xMin) {
            numberOfWaysToBeatThisRace = xMax - xMin + 1;
            result = numberOfWaysToBeatThisRace;
        }
    }

    std::cout << "# = " << numberOfWaysToBeatThisRace << '\n';

    std::cout << '\n';

    std::cout << "RESULT: " << result << '\n';
}
