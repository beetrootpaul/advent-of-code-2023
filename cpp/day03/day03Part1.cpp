#include <iostream>
#include <sstream>
#include <string>

#include "day03.h"

constexpr int ROWS_LIMIT = 150;
constexpr int COLS_LIMIT = 150;

void printChars2d(char chars2d[ROWS_LIMIT][COLS_LIMIT], int maxRow, int maxCol) {
    std::cout << '\n';
    for (int row = 0; row <= maxRow + 1; ++row) {
        for (int col = 0; col <= maxCol + 1; ++col) {
            std::cout << chars2d[row][col];
        }
        std::cout << '\n';
    }
}

//
// https://adventofcode.com/2023/day/3
//
void day03Part1() {
    char schematic[ROWS_LIMIT][COLS_LIMIT];
    for (auto &row: schematic) {
        for (char &c: row) {
            c = '.';
        }
    }

    char markers[ROWS_LIMIT][COLS_LIMIT];
    for (auto &row: markers) {
        for (char &c: row) {
            c = '-';
        }
    }

    int adjacencyOffsetsR[8] = {-1, -1, -1, 0, 0, 1, 1, 1};
    int adjacencyOffsetsC[8] = {-1, 0, 1, -1, 1, -1, 0, 1};

    //
    // Read the schematic from stdin
    //

    int maxRow = 1;
    int maxCol = 1;

    int r = 1;
    for (std::string nextLine; getline(std::cin, nextLine);) {
        maxRow = std::max(maxRow, r);
        for (int i = 0; i < nextLine.size(); ++i) {
            int c = i + 1;
            schematic[r][c] = nextLine[i];
            maxCol = std::max(maxCol, c);
        }
        r += 1;
    }

    printChars2d(schematic, maxRow, maxCol);
    printChars2d(markers, maxRow, maxCol);

    //
    // Mark at least single digits of engine parts
    //

    for (int row = 1; row <= maxRow; ++row) {
        for (int col = 1; col <= maxCol; ++col) {
            if (const char c = schematic[row][col]; c < '0' || c > '9') {
                continue;
            }

            bool isEnginePart = false;
            for (const int offsetRIndex: adjacencyOffsetsR) {
                for (const int offsetCIndex: adjacencyOffsetsC) {
                    if (
                        const char symbolToCheck = schematic[row + offsetRIndex][col + offsetCIndex];
                            symbolToCheck != '.' && (symbolToCheck < '0' || symbolToCheck > '9')
                            ) {
                        isEnginePart = true;
                    }
                }
            }
            if (isEnginePart) {
                markers[row][col] = 'x';
            }
        }
    }

    printChars2d(schematic, maxRow, maxCol);
    printChars2d(markers, maxRow, maxCol);

    //
    // Mark remaining digits of engine parts
    //

    for (int row = 1; row <= maxRow; ++row) {
        for (int col = 1; col <= maxCol; ++col) {
            if (const char m = markers[row][col]; m != 'x') {
                continue;
            }

            bool isInsideNumber = true;
            int processedCol = col;
            while (isInsideNumber) {
                processedCol--;
                if (schematic[row][processedCol] >= '0' && schematic[row][processedCol] <= '9') {
                    markers[row][processedCol] = 'x';
                } else {
                    isInsideNumber = false;
                }
            }
            isInsideNumber = true;
            processedCol = col;
            while (isInsideNumber) {
                processedCol++;
                if (schematic[row][processedCol] >= '0' && schematic[row][processedCol] <= '9') {
                    markers[row][processedCol] = 'x';
                } else {
                    isInsideNumber = false;
                }
            }
        }
    }

    printChars2d(schematic, maxRow, maxCol);
    printChars2d(markers, maxRow, maxCol);

    //
    // Leave only engine part numbers in the schematic
    //

    for (int row = 1; row <= maxRow; ++row) {
        for (int col = 1; col <= maxCol; ++col) {
            if (markers[row][col] != 'x') {
                schematic[row][col] = ' ';
            }
        }
    }
    for (auto &row: schematic) {
        for (char &col: row) {
            if (col == '.') {
                col = ' ';
            }
        }
    }

    printChars2d(schematic, maxRow, maxCol);

    //
    // Sum all engine part numbers
    //

    // TODO: is it OK that we iterate over all rows here as they probably lie in a continuous chunk of memory?
    std::istringstream ss(schematic[0]);
    int nextEnginePartNumber;
    int sumOfEnginePartNumbers = 0;
    while (ss >> nextEnginePartNumber) {
        sumOfEnginePartNumbers += nextEnginePartNumber;
    }

    std::cout << "SUM OF ENGINE PART NUMBERS: " << sumOfEnginePartNumbers << '\n';
}
