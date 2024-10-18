import Exevalator from "./exevalator";

/*
 * A benchmark to measure the speed of repeated calculations.
 */

console.log("Please wait...");

const loops: number = 100 * 1000 * 1000; // 100M LOOPS
let flopPerLoop: number = 10;

let exevalator: Exevalator = new Exevalator();
const address: number = exevalator.declareVariable("x");
let sum: number = 0.0;

// Measure required time for evaluating a expression repeatedly for 100M times,
// where each 10 numerical operations are required for each evaluation.
const beginTime: number = Date.now(); // milliseconds
for (let i: number =  1; i <= loops; ++i) {
    exevalator.writeVariableAt(address, i);
    sum += exevalator.eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1");
}
const endTime: number = Date.now();
const elapsedSec: number = (endTime - beginTime) * 1.0E-3;

// Display results:
const evalSpeed = loops / elapsedSec;
const megaFlops = flopPerLoop * loops / elapsedSec / (1000.0 * 1000.0);
const correctSum = (loops * (loops + 1)) / 2.0;
console.log("-----");
console.log(`EVAL-LOOP SPEED: ${evalSpeed} [EVALS/SEC]`);
console.log(`OPERATION SPEED: ${megaFlops} [M FLOPS]`);
console.log(`VALUE OF \"sum\" ${sum} (EXPECTED: ${correctSum})`);
