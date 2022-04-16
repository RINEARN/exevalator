mod exevalator;
use std::time::Instant;
use exevalator::Exevalator;

/*
 * !!! DON'T FORGET TO SPECIFY OPTIMIZATION OPTIONS WHEN YOU COMPILE THIS CODE !!!
 *     e.g.:
 *           rustc -C opt-level=3 benchmark.rs
 */

/// A benchmark to measure the speed of repeated calculations.
fn main() {
    println!("Please wait...");

    let loops: u64 = 100u64 * 1000u64 * 1000u64; // 100M LOOPS
    let flop_per_loop: u64 = 10u64;

    let mut exevalator: Exevalator = Exevalator::new();
    let address: usize = exevalator.declare_variable("x").unwrap();
    let mut sum = 0.0;

    // Measure required time for evaluating a expression repeatedly for 100M times,
    // where each 10 numerical operations are required for each evaluation.
    let begin_time: Instant = Instant::now();
    for i in 1u64..(loops + 1u64) {

        if let Err(variable_access_err) = exevalator.write_variable_at(address, i as f64) {
            panic!("{}", variable_access_err);
        }

        sum += match exevalator.eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1") {
            Ok(eval_value) => eval_value,
            Err(eval_error) => panic!("{}", eval_error),
        };

        // If you need more speed, use "reeval()" instead of "eval(expression)"
        // after the first evaluation (then it becomes 10 ~ 20% faster):
        // 
        // if i == 1u64 {
        //     sum += match exevalator.eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1") {
        //     ...
        // } else {
        //     sum += match exevalator.reeval() {
        //     ...
        // }
    }
    let elapsed_sec: f64 = begin_time.elapsed().as_secs_f64();
    
    // Display results:
    let eval_speed: f64 = loops as f64 / elapsed_sec;
    let mega_flops: f64 = (flop_per_loop * loops) as f64 / elapsed_sec / (1000.0 * 1000.0);
    let correct_sum: f64 = (loops * (loops + 1)) as f64 / 2.0;
    println!("-----");
    println!("EVAL-LOOP SPEED: {} [EVALS/SEC]", eval_speed);
    println!("OPERATION SPEED: {} [M FLOPS]", mega_flops);
    println!("VALUE OF \"sum\" : {} (EXPECTED: {})", sum, correct_sum);
}
