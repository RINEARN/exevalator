// import anypackage.in.which.you.put.Exevalator;

/**
 * A benchmark to measure the speed of repeated calculations.
 */
public class Benchmark {

    public static void main(String[] args) {

        System.out.println("Please wait...");

        long loops = 100L * 1000L * 1000L; // 100M LOOPS
        long flopPerLoop = 10L;

        Exevalator exevalator = new Exevalator();
        int address = exevalator.declareVariable("x");
        double sum = 0.0;

        // Measure required time for evaluating a expression repeatedly for 100M times,
        // where each 10 numerical operations are required for each evaluation.
        long beginTime = System.nanoTime();
        for (long i=1L; i<=loops; ++i)
        {
            exevalator.writeVariableAt(address, (double)i);
            sum += exevalator.eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1");
        }
        long endTime = System.nanoTime();
        double elapsedSec = (endTime - beginTime) * 1.0E-9;

        // Display results:
        double evalSpeed = loops / elapsedSec;
        double megaFlops = flopPerLoop * loops / elapsedSec / (1000.0 * 1000.0);
        double correctSum = (loops * (loops + 1)) / 2.0;
        System.out.println("-----");
        System.out.println("EVAL-LOOP SPEED: " + evalSpeed + " [EVALS/SEC]");
        System.out.println("OPERATION SPEED: " + megaFlops + " [M FLOPS]");
        System.out.println("VALUE OF \"sum\" : " + sum + " (EXPECTED: "+ correctSum + ")");
    }
}
