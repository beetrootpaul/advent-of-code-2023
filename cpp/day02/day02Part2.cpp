#include <iostream>
#include <sstream>
#include <string>

#include "../helpers/helpers.h"
#include "day02.h"

using std::cin;
using std::cout;
using std::getline;
using std::istringstream;
using std::min;
using std::max;
using str_t = std::string;

//
// https://adventofcode.com/2023/day/2
//
void day02Part2() {
    int sumOfPower = 0;

    for (str_t nextLine; getline(cin, nextLine);) {
        cout << '\n' << nextLine << '\n';

        int gameNumber = -1;

        int rAmountMax = 0;
        int gAmountMax = 0;
        int bAmountMax = 0;

        istringstream ss(nextLine);
        str_t prevToken;
        str_t nextToken;
        while (ss >> nextToken) {
            // TODO: can I make transform a part of a stream?
            helpers::transformToLowercase(&nextToken);
            helpers::removeLastCharIfComaOrColonOrSemicolon(&nextToken);

            if (prevToken == "game") {
                gameNumber = stoi(nextToken);
            }

            if (nextToken == "red") {
                rAmountMax = max(rAmountMax, stoi(prevToken));
            }
            if (nextToken == "green") {
                gAmountMax = max(gAmountMax, stoi(prevToken));
            }
            if (nextToken == "blue") {
                bAmountMax = max(bAmountMax, stoi(prevToken));
            }

            prevToken = nextToken;
        }

        // TODO: use some printf or something which looks easier to read
        cout << "parsed (" << gameNumber << ")"
             << " r" << rAmountMax
             << " g" << gAmountMax
             << " b" << bAmountMax
             << '\n';

        int power = rAmountMax * gAmountMax * bAmountMax;
        sumOfPower += power;
    }

    cout << '\n' << "SUM OF POWER OF SETS: " << sumOfPower << '\n';
}
