using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
  public static class FFNN
  {
    public static int numOfLayers = 0;
    public static Matrix[] weights;
    public static Matrix[] nodes;
    public static Matrix[] zValues;
    public static Matrix[] Biases;
    public static void initFFNN(params int[] layerSizes)
    {
      numOfLayers = layerSizes.Length;

      nodes = new Matrix[numOfLayers];
      weights = new Matrix[numOfLayers - 1];
      Biases = new Matrix[numOfLayers - 1];
      zValues = new Matrix[numOfLayers - 1];

      for (int i = 0; i < numOfLayers - 1; i++)
      {
        nodes[i] = new Matrix(layerSizes[i], 1);
        zValues[i] = new Matrix(layerSizes[i+1], layerSizes[i]);
        weights[i] = new Matrix(layerSizes[i + 1], layerSizes[i]);
        weights[i].populateWithRandomValues();
        Biases[i] = new Matrix(layerSizes[i + 1], 1);
      }
      nodes[numOfLayers-1] = new Matrix(layerSizes.Last(), 1);
    }


    public static void learnData(string answer, List<string> data)
    {

      //translate the data into an input
      foreach (string word in data)
      {
        int index = Master.vocabulary.IndexOf(word);
        if (index == -1) continue;
        nodes[0][index, 0] += 1;
      }
      nodes[0] = nodes[0].sigmoid();

      //perform the calculations
      for (int i = 0; i < nodes.Length - 1; i++)
      {
        zValues[i] = (weights[i] * nodes[i] + Biases[i]);
        nodes[i + 1] = zValues[i].sigmoid();
      }

      for (int i = 0; i < 5; i++)
        Console.WriteLine(Master.categories[i] + " : " + nodes.Last()[i, 0].ToString("0.0000"));

      //find the expected Answer
      Matrix expectedAnswer = new Matrix(nodes.Last().height, 1);
      expectedAnswer[Master.categories.IndexOf(answer),0] = 1;

      //needed change of each output (a-y)
      Matrix neededChange = nodes.Last() - expectedAnswer;

      //(a-y)^2
      double cost = neededChange.dotProduct();

      //2*(a-y)
      Matrix[] partialCostMatrix = new Matrix[numOfLayers];
      partialCostMatrix[numOfLayers-1] = neededChange * 2;


      Matrix[] dC_over_dW = new Matrix[numOfLayers-1];
      Matrix[] dC_over_dA = new Matrix[numOfLayers-1];
      Matrix[] Z_and_cost = new Matrix[numOfLayers-1];
      for (int i = numOfLayers-2;i >= 0;i--)
      {
        Matrix sigDerivativeOfZ = zValues[i].sigmoidDerivative();

        //sigmoid(z) * 2(a-y)
        //Hammond product multiplies each element to
        //the corresponding element of the second matrix
        Z_and_cost[i] = sigDerivativeOfZ.hammondProduct(partialCostMatrix[i+1]);

        //dC/da (Vector Matrix)
        //w * sigmoid(z) * 2(a-y)
        partialCostMatrix[i] = Z_and_cost[i].transpose() * weights[i];
        partialCostMatrix[i] = partialCostMatrix[i].transpose();

        //dC/dw  (Big matrix)
        //a * sigmoid(z) * 2(a-y)
        dC_over_dW[i] = Z_and_cost[i] * nodes[i].transpose();
        dC_over_dA[i] = new Matrix(weights[i].height, weights[i].length);

        //dC/db derivative with respect to bias
        //This math doesn't exist because the
        //z_and_cost = sigmoid(z) * 2(a-y) is equal to dC/db
      }

      //Apply the gradient descent
      for (int i = numOfLayers - 2;i >= 0; i--)
      {
        Biases[i] = Biases[i] - 0.1 * Z_and_cost[i];
        weights[i] = weights[i] - 0.1 * dC_over_dW[i];
      }
      Console.Write(cost.ToString("0.0000000000000000")+"  ");
    }

    public static string estimateData(List<string> data)
    {

      //translate the data into an input
      foreach (string word in data)
      {
        int index = Master.vocabulary.IndexOf(word);
        if (index == -1) continue;
        nodes[0][index, 0] = 1;
      }

      //perform the calculations
      for (int i = 0; i < nodes.Length - 1; i++)
      {
        zValues[i] = (weights[i] * nodes[i] + Biases[i]);
        nodes[i + 1] = zValues[i].sigmoid();
      }
      int answerIndex = 0;
      for (int i = 0; i < nodes[nodes.Length-1].height; i++) {
        if (nodes[nodes.Length-1][i,0] > answerIndex) answerIndex = i;
      }
      Console.WriteLine();
      for (int i = 0; i < 5; i++)
        Console.WriteLine(Master.categories[i] + " : " + nodes.Last()[i, 0].ToString("0.0000"));
      return Master.categories[answerIndex];
    }
  }
}
