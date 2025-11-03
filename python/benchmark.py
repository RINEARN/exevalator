from __future__ import annotations
from exevalator import Exevalator
import sys
import time

def main() -> None:
    """
    A benchmark to measure the speed of repeated calculations.
    Evaluates the same expression in a tight loop while updating a variable.
    Expression:  x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1  (10 ops, net = x)
    """

    print("Please wait...")

    # Loop count (default: 10M). You can override by passing an integer as the 1st CLI arg.
    loops: int = 10_000_000
    if len(sys.argv) >= 2:
        loops = int(sys.argv[1])

    flop_per_loop: int = 10

    ex = Exevalator()
    addr = ex.declare_variable("x")
    sum_val = 0.0

    expr = "x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1"  # 10 ops

    # Measure
    begin = time.perf_counter()
    for i in range(1, loops + 1):
        ex.write_variable_at(addr, float(i))
        sum_val += ex.eval(expr)
    end = time.perf_counter()

    elapsed_sec = end - begin

    # Print results
    eval_speed = loops / elapsed_sec  # [evals/sec]
    mega_flops = flop_per_loop * loops / elapsed_sec / 1_000_000.0  # [M FLOPS]
    correct_sum = (loops * (loops + 1)) / 2.0
    print("-----")
    print(f"EVAL-LOOP SPEED: {eval_speed} [EVALS/SEC]")
    print(f"OPERATION SPEED: {mega_flops} [M FLOPS]")
    print(f'VALUE OF "sum" : {sum_val} (EXPECTED: {correct_sum})')

if __name__ == "__main__":
    main()
