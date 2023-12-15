#include "helpers.h"

namespace helpers {
    void transformToLowercase(std::string* text) {
        std::ranges::transform(
            *text,
            text->begin(),
            [](unsigned char c) { return std::tolower(c); }
        );
    }

    // TODO: rename, clean up
    void removeLastCharIfComaOrColonOrSemicolon(std::string* text) {
        if (!text->empty() && (text->back() == ',' || text->back() == ':' || text->back() == ';')) {
            text->erase(text->size() - 1);
        }
    }
}
