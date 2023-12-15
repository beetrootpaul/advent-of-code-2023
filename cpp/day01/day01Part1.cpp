#include <iostream>
#include <string>

#include "day01.h"

//
// https://adventofcode.com/2023/day/1
//
void day01Part1() {
    int sumOfCalibrationValues = 0;

    for (std::string nextLine; std::getline(std::cin, nextLine);) {
        std::cout << '\n' << "Line is: " << nextLine << '\n';

        size_t firstDigitPos = std::string::npos;
        size_t lastDigitPos = 0;
        int firstDigit = -1;
        int lastDigit = -1;

        for (int digit = 0; digit < 10; ++digit) {
            const size_t digitFirstPosNumeric = nextLine.find(std::to_string(digit));
            const size_t digitLastPosNumeric = nextLine.rfind(std::to_string(digit));
            if (digitFirstPosNumeric <= firstDigitPos) {
                firstDigitPos = digitFirstPosNumeric;
                firstDigit = digit;
            }
            if (digitLastPosNumeric != std::string::npos && digitLastPosNumeric >= lastDigitPos) {
                lastDigitPos = digitLastPosNumeric;
                lastDigit = digit;
            }
        }

        if (firstDigit >= 0 && lastDigit >= 0) {
            const int calibrationValue = firstDigit * 10 + lastDigit;
            std::cout << "Next calibration value: " << calibrationValue << '\n';
            sumOfCalibrationValues += calibrationValue;
        } else {
            std::cout << "There is no calibration value for this line" << '\n';
        }
    }

    std::cout << '\n' << "Sum of all calibration values: " << sumOfCalibrationValues << '\n';
}
