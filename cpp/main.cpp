#include <iostream>
#include <string>

#include "day01/day01.h"
#include "day02/day02.h"
#include "day03/day03.h"
#include "day05/day05.h"
#include "day06/day06.h"

// TODO: how to make CMakeLists include all cpp files found in a tree instead of listing each one individually?
// TODO: how to write unit tests for an example input vs output?

// ReSharper disable once CppDFAConstantFunctionResult
int main(const int argc, char *argv[]) {
    if (argc < 2) {
        std::cout << "Please choose a day and its part to run the code for. For example: ./<this_program> 1 1" << '\n';
        return 0;
    }

    const int chosenDay = std::stoi(argv[1]);
    const int chosenPart = std::stoi(argv[2]);

    if (chosenDay == 1 && chosenPart == 1) {
        day01Part1();
    } else if (chosenDay == 1 && chosenPart == 2) {
        day01Part2();
    } else if (chosenDay == 2 && chosenPart == 1) {
        day02Part1();
    } else if (chosenDay == 2 && chosenPart == 2) {
        day02Part2();
    } else if (chosenDay == 3 && chosenPart == 1) {
        day03Part1();
    } else if (chosenDay == 5 && chosenPart == 1) {
        day05Part1();
    } else if (chosenDay == 6 && chosenPart == 1) {
        day06Part1();
    } else if (chosenDay == 6 && chosenPart == 2) {
        day06Part2();
    } else {
        std::cout << "There is no code for day " << chosenDay << " part " << chosenPart << '\n';
    }

    return 0;
}
