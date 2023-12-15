cmake_minimum_required(VERSION 3.26)
project(advent_of_code_2023)

set(CMAKE_CXX_STANDARD 23)

add_executable(advent_of_code_2023 main.cpp
        helpers/helpers.h
        helpers/helpers.cpp
        day01/day01.h
        day01/day01Part1.cpp
        day01/day01Part2.cpp
        day02/day02.h
        day02/day02Part1.cpp
        day02/day02Part2.cpp
        day03/day03.h
        day03/day03Part1.cpp
        day05/day05.h
        day05/day05Part1.cpp
        day06/day06.h
        day06/day06Part1.cpp
        day06/day06Part2.cpp)
