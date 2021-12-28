#include <iostream>
#include <cstdlib>
#include <cstdint>
#include <chrono>
#include "exevalator.hpp"
#include "exevalator.cpp"

/**
 * A benchmark to measure the speed of repeated calculations.
 *
 * !!! DON'T FORGET TO SPECIFY OPTIMIZATION OPTIONS WHEN YOU COMPILE THIS CODE !!!
 *     e.g.:
 *           clang++ -std=c++17 -Wall -O2 -o benchmark benchmark.cpp
 */
int main() {

    std::cout << "Please wait..." << std::endl;

    uint64_t loops = 100UL * 1000UL * 1000UL; // 100M LOOPS
    uint64_t flop_per_loop = 10UL;

    exevalator::Exevalator exevalator;
    double sum = 0.0;
    double elapsed_sec = 0.0;

    // Measure required time for evaluating a expression repeatedly for 100M times,
    // where each 10 numerical operations are required for each evaluation.
    try {
        size_t address = exevalator.declare_variable("x");

        std::chrono::system_clock::time_point begin_time = std::chrono::system_clock::now();
        for (uint64_t i=1UL; i<=loops; ++i) {
            sum += exevalator.eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1");
        }
        std::chrono::system_clock::time_point end_time = std::chrono::system_clock::now();

        auto elapsed_nanosec = std::chrono::duration_cast<std::chrono::nanoseconds>(end_time - begin_time);
        elapsed_sec = static_cast<double>(elapsed_nanosec.count()) * 1.0E-9;

    } catch (exevalator::ExevalatorException &e) {
        std::cerr << e.what() << std::endl;
        return EXIT_FAILURE;
    }

    // Display results:
    double eval_speed = loops / elapsed_sec;
    double mega_flops = (flop_per_loop * loops) / elapsed_sec / (1000.0 * 1000.0);
    double correct_sum = (loops * (loops + 1)) / 2.0;
    std::cout << "-----" << std::endl;
    std::cout << "EVAL-LOOP SPEED: " << eval_speed << " [EVALS/SEC]" << std::endl;
    std::cout << "OPERATION SPEED: " << mega_flops << " [M FLOPS]" << std::endl;
    std::cout << "VALUE OF \"sum\" : " << sum << " (EXPECTED: " << correct_sum << ")" << std::endl;

    return EXIT_SUCCESS;
}
