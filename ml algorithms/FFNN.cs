using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Reflection;
using System.Runtime.CompilerServices;
namespace Main
{

  public class FFNN
  {
    public int numOfLayers = 0;
    public int burstSize = 1;
    string[][] labelTrainBurst;
    float[][][] dataTrainBurst;
    bool data_Chunks_Not_Initialized = true;

    public static Semaphore matrixWriteSem = new Semaphore(1, 1);

    public int threadCount = 0;

    public Matrix[] weights;
    public Matrix[] burst_Sum_Of_Weights;
    public Matrix[] nodes;
    public Matrix[] zValues;
    public Matrix[] Biases;
    public Matrix[] burst_Sum_Of_Biases;
    public Matrix[] z_and_cost;
    public Matrix[] dC_over_dW;
    public double learningRate = 0.001;
    public double cost = 0;
    public void initFFNN(params int[] layerSizes)
    {
      numOfLayers = layerSizes.Length;

      nodes = new Matrix[numOfLayers];
      weights = new Matrix[numOfLayers - 1];
      burst_Sum_Of_Weights = new Matrix[numOfLayers - 1];
      Biases = new Matrix[numOfLayers - 1];
      burst_Sum_Of_Biases = new Matrix[numOfLayers - 1];
      zValues = new Matrix[numOfLayers - 1];

      for (int i = 0; i < numOfLayers - 1; i++)
      {
        nodes[i] = new Matrix(layerSizes[i], 1);
        zValues[i] = new Matrix(layerSizes[i + 1], layerSizes[i]);
        weights[i] = new Matrix(layerSizes[i + 1], layerSizes[i]);
        burst_Sum_Of_Weights[i] = new Matrix(layerSizes[i + 1], layerSizes[i]);
        weights[i].populateWithRandomValues();
        Biases[i] = new Matrix(layerSizes[i + 1], 1);
        burst_Sum_Of_Biases[i] = new Matrix(layerSizes[i + 1], 1);
      }
      // There is one more layer of nodes (neurons) than everything else
      nodes[numOfLayers - 1] = new Matrix(layerSizes.Last(), 1);
    }

    public void initialize_Data(string[] labelTrainingSet, float[][] dataTrainingSet, int chunkSize = 1)
    {
      labelTrainBurst = labelTrainingSet.Chunk(chunkSize).ToArray();
      dataTrainBurst = dataTrainingSet.Chunk(chunkSize).ToArray();
      data_Chunks_Not_Initialized = false;
    }

    public void learnSet(string[] answers, float[][] dataSet, int chunkSize = 1)
    {
      if (data_Chunks_Not_Initialized)
      {
        burstSize = chunkSize;
        initialize_Data(answers, dataSet, chunkSize);
      }
      Console.WriteLine();
      var pos = Console.GetCursorPosition();
      for (int i = 0; i < labelTrainBurst.Count(); i++)
      {
        Console.SetCursorPosition(pos.Left, pos.Top);
        Console.Write("     ");
        Console.SetCursorPosition(pos.Left, pos.Top);
        Console.Write(i);
        learnBurst(labelTrainBurst[i], dataTrainBurst[i]);
        applyGradientAverage();
      }
      Console.WriteLine();
    }

    public void learnBurst(string[] answers, float[][] datas)
    {
      Matrix[] z_and_costTemp = new Matrix[numOfLayers - 1];
      Matrix[] dc_over_dwTemp = new Matrix[numOfLayers - 1];

      //zero burst sums of weights and biases.
      foreach (Matrix matrix in burst_Sum_Of_Weights)
        matrix.zero();
      foreach (Matrix matrix in burst_Sum_Of_Biases)
        matrix.zero();


      var cursorPosY = Console.CursorTop;
      ParallelOptions options = new() { MaxDegreeOfParallelism = 6 };
      //ParallelOptions options = new() { MaxDegreeOfParallelism = 2 };
      Parallel.ForAsync(0, answers.Length, options, async (index, ct) => { await LearnData(answers[index], datas[index]); });
      //Parallel.For(0, answers.Length, index => { LearnData(answers[index], datas[index]); });
      

      applyGradientAverage();

    }

    public async Task LearnData(string answer, float[] data)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      Matrix[] nodesTemp = new Matrix[nodes.Count()];
      nodesTemp[0] = new Matrix(nodes[0]);
      //translate the data into an input
      for (int i = 0; i < data.Count(); i++)
      {
        nodesTemp[0][i, 0] = data[i];
      }
      Matrix[] zValuesTemp = new Matrix[zValues.Count()];
      //perform the calculations
      for (int i = 0; i < nodes.Length - 1; i++)
      {
        zValuesTemp[i] = (weights[i] * nodesTemp[i] + Biases[i]);
        nodesTemp[i + 1] = zValuesTemp[i].sigmoid();
      }

      //find the expected Answer
      Matrix expectedAnswer = new Matrix(nodes.Last().height, 1);
      //needed change of each output (a-y)
      
      expectedAnswer[Master.categories.IndexOf(answer), 0] = 1;
      Matrix neededChange = nodes.Last() - expectedAnswer;

      //(a-y)^2
      double cost = neededChange.dotProduct();

      //2*(a-y)
      Matrix[] partialCostMatrix = new Matrix[numOfLayers];
      partialCostMatrix[numOfLayers - 1] = neededChange * 2;


      Matrix[] dC_over_dW = new Matrix[numOfLayers - 1];
      Matrix[] dC_over_dA = new Matrix[numOfLayers - 1];
      Matrix[] z_and_cost = new Matrix[numOfLayers - 1];

      double[] times = new double[6];
      for (int i = numOfLayers - 2; i >= 0; i--)
      {
        Matrix sigDerivativeOfZ = zValuesTemp[i].sigmoidDerivative();
        //sigmoid(z) * 2(a-y)
        //Hammond product multiplies each element to
        //the corresponding element of the second matrix
        z_and_cost[i] = sigDerivativeOfZ.hammondProduct(partialCostMatrix[i + 1]);
        //dC/da (Vector Matrix)
        //w * sigmoid(z) * 2(a-y)
        partialCostMatrix[i] = weights[i].transposeAndCrossWith(z_and_cost[i]);
        //dC/dw
        //a * sigmoid(z) * 2(a-y)
        dC_over_dW[i] = z_and_cost[i].CrossWithTransposeOf(nodes[i]);
        dC_over_dA[i] = new Matrix(weights[i].height, weights[i].length);
        //dC/db 
        //Code doesn't need to exist for this derivative because:
        //z_and_cost = sigmoid(z) * 2(a-y) is equal to dC/db


      }
      double time = stopwatch.Elapsed.TotalMicroseconds;
      //Console.WriteLine(time + "microS");
      await gradientDescentAverage(dC_over_dW, z_and_cost);
      //Console.Write(cost.ToString("0.0000000000000000")+"  ");
    }

    public async Task gradientDescentAverage(Matrix[] dCdW, Matrix[] dCdB)
    {
      //Apply the gradient descent
      for (int i = numOfLayers - 2; i >= 0; i--)
      {
        burst_Sum_Of_Biases[i] += -0.01 * dCdB[i];
        burst_Sum_Of_Weights[i] += -0.01 * dCdW[i];
      }
    }

    public void applyGradientAverage()
    {
      matrixWriteSem.WaitOne();
      //Apply the gradient descent
      for (int i = numOfLayers - 2; i >= 0; i--)
      {
        Biases[i] += learningRate * burst_Sum_Of_Biases[i] / burstSize;
        weights[i] += learningRate * burst_Sum_Of_Weights[i] / burstSize;
      }
      matrixWriteSem.Release();
    }

    public void gradientDescent()
    {
      //Apply the gradient descent
      for (int i = numOfLayers - 2; i >= 0; i--)
      {
        Biases[i] = Biases[i] - 0.01 * z_and_cost[i];
        weights[i] = weights[i] - 0.01 * dC_over_dW[i];
      }
    }

    public int estimateData(float[] data)
    {

      //translate the data into an input
      for (int i = 0; i < data.Count(); i++)
      {
        nodes[0][i, 0] = data[i] / 256;
      }

      //perform the calculations
      for (int i = 0; i < nodes.Length - 1; i++)
      {
        zValues[i] = (weights[i] * nodes[i] + Biases[i]);
        nodes[i + 1] = zValues[i].sigmoid();
      }

      int answerIndex = 0;
      for (int i = 0; i < nodes[nodes.Length - 1].height; i++)
      {
        if (nodes[nodes.Length - 1][i, 0] > nodes[nodes.Length - 1][answerIndex, 0]) answerIndex = i;
      }
      //Console.WriteLine();

      return answerIndex;
    }
  }
}
