#ifndef ADVENT_OF_CODE_2023_HELPERS_H
#define ADVENT_OF_CODE_2023_HELPERS_H

#include <string>

namespace helpers {
    void transformToLowercase(std::string* text);

    void removeLastCharIfComaOrColonOrSemicolon(std::string* text);
}

#endif //ADVENT_OF_CODE_2023_HELPERS_H
