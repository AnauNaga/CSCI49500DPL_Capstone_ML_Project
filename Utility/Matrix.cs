using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
  public class Matrix
  {
    public double[][] matrix;

    public readonly int length = 0;
    public readonly int height = 0;

    public Matrix(int height, int length, int defaultValue = 0)
    {

      Random rand = new Random();
      this.length = length;
      this.height = height;
      matrix = new double[height][];

      for (int i = 0; i < height; i++)
        matrix[i] = new double[length];

      for (int i = 0; i < height; i++)
      {
        for (int j = 0; j < length; j++)
        {
          matrix[i][j] = defaultValue;
        }
      }
    }

    public Matrix(Matrix matrix)
    {
      this.height = matrix.height;
      this.length = matrix.length;
      this.matrix = new double[height][];
      for (int i = 0; i < height; i++)
        this.matrix[i] = new double[length];

      for (int i = 0; i < matrix.height; i++)
      {
        for (int j = 0; j < length; j++)
        {
          this.matrix[i][j] = matrix.matrix[i][j];
        }
      }
    }

    public void populateWithRandomValues()
    {
      Random rand = new Random();
      for (int i = 0; i < height; i++)
      {
        for (int j = 0; j < length; j++)
        {
          matrix[i][j] = (rand.NextDouble() * 2 - 1);
        }
      }
    }

    public Matrix transpose()
    {
      Matrix a = new Matrix(length, height);
      for(int i = 0;i < height; i++)
      {
        for(int j = 0; j < length; j++)
        {
          a[j, i] = matrix[i][j];
        }
      }
      return a;
    }

    public Matrix CrossWithTransposeOf(Matrix b)
    {
      Matrix a = this;
      if (a.length != b.length) throw new ArgumentException("Length of Matrix a does not equal the height of Matrix b");

      Matrix c = new Matrix(a.height, b.height);


      //for each cell in matrix c
      for (int i = 0; i < c.height; i++)
      {
        for (int j = 0; j < c.length; j++)
        {

          //find the value of each cell by summing up the products from matrix a and b
          double sum = 0;
          for (int k = 0; k < a.length; k++)
          {
            sum += a.matrix[i][k] * b.matrix[j][k];
          }

          c.matrix[i][j] = sum;
        }
      }

      return c;

    }

    public Matrix transposeAndCrossWith(Matrix b)
    {
      Matrix a = this;
      if (a.height != b.height) throw new ArgumentException("Length of Matrix a does not equal the height of Matrix b");

      Matrix c = new Matrix(a.length, b.length);


      //for each cell in matrix c
      for (int i = 0; i < c.height; i++)
      {
        for (int j = 0; j < c.length; j++)
        {

          //find the value of each cell by summing up the products from matrix a and b
          double sum = 0;
          for (int k = 0; k < a.height; k++)
          {
            sum += a.matrix[k][i] * b.matrix[k][j];
          }

          c.matrix[i][j] = sum;
        }
      }

      return c;

    }

    public Matrix sigmoid()
    {
      Matrix a = new Matrix(height,length);
      for (int i = 0; i < height; i++) {
        for (int j = 0; j < length; j++) {
          //a[i,j] = 1 / (1 + Math.Exp(1 - matrix[i][j]));
          double myNum = matrix[i][j];
          a[i, j] = myNum/ (1+ (myNum > 0 ? myNum : -myNum));
        }
      }
      return a;
    }

    public Matrix sigmoidDerivative()
    {
      Matrix a = new Matrix(height, length);
      for (int i = 0; i < height; i++)
      {
        for (int j = 0; j < length; j++)
        {
          //double eNegx = Math.Exp(matrix[i][j]);
          //a[i, j] = (eNegx / ((1 + eNegx) * (1 + eNegx)));
          double myNum = matrix[i][j];
          double onePlusAbs = (1 + (myNum > 0 ? myNum : -myNum));
          a[i, j] = 1 / (onePlusAbs * onePlusAbs);
        }
      }
      return a;
    }

    public void zero()
    {
      for (int i = 0; i < height; i++)
      {
        for (int j = 0; j < length; j++)
        {
          matrix[i][j] = 0;
        }
      }
    }

    public Matrix hammondProduct(Matrix multiplier)
    {
      if ((height != multiplier.height) || (length != multiplier.length)) throw new ArgumentException("Matricies are not the same size");
      Matrix c = new Matrix(this);

      for (int i = 0; i < height; i++)
      {
        for(int j = 0;j < length; j++)
        {
          c[i, j] *= multiplier[i, j];
        }
      }
      return c; 
    }

    public double dotProduct()
    {
      if (length != 1) throw new ArgumentException("Cannot perform the dot product on a matrix greater than length 1");

      double sum = 0;
      for(int i = 0; i < height; i++)
      {
        sum += matrix[i][0] * matrix[i][0];
      }
      return sum;
    }
    public double dotProduct(Matrix matrixB)
    {
      if (length != 1) throw new ArgumentException("Cannot perform the dot product on a matrix greater than length 1");
      if ((length != matrixB.length) && (height != matrixB.height)) throw new ArgumentException("Matricies are not the same size");

      double sum = 0;
      for (int i = 0; i < height; i++)
      {
        sum += matrix[i][0] * matrixB[i,0];
      }
      return sum;
    }

    public double this[int x,int y]
    {
      get { return matrix[x][y]; }
      set { matrix[x][y] = value;}
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
      if (a.length != b.height) throw new ArgumentException("Length of Matrix a does not equal the height of Matrix b");

      Matrix c = new Matrix(a.height, b.length);

      for (int i = 0; i < a.height; i++)
      {
        for (int j = 0; j < b.length; j++)
        {
          double sum = 0;
          for (int k = 0; k < a.length; k++)
          {
            sum += a.matrix[i][k] * b.matrix[k][j];
          }

          c.matrix[i][j] = sum;
        }
      }

      return c;
    }

    public static Matrix operator /(Matrix a, double b)
    {
      Matrix c = new Matrix(a);
      for (int i = 0; i < a.height; i++)
      {
        for (int j = 0; j < a.length; j++)
        {
          c[i, j] /= b;
        }
      }

      return c;
    }

    public static Matrix operator *(Matrix a, double b)
    {
      Matrix c = new Matrix(a);
      for(int i = 0; i < a.height; i++)
      {
        for(int j = 0; j < a.length; j++)
        {
          c[i,j] *= b;
        }
      }

      return c;
    }

    public static Matrix operator *(double a, Matrix b)
    {
      Matrix c = new Matrix(b);
      for (int i = 0; i < b.height; i++)
      {
        for (int j = 0; j < b.length; j++)
        {
          c[i, j] *= a;
        }
      }

      return c;
    }

    public static Matrix operator +(Matrix a, Matrix b)
    {
      if (a.length != b.length && a.height != b.height) throw new ArgumentException("Matrix a and Matrix B are not the same size");

      Matrix c = new Matrix(a);

      for (int i = 0; i < c.height; i++)
      {
        for (int j = 0; j < c.length; j++)
        {
          c.matrix[i][j] += b.matrix[i][j];
        }
      }
      return c;
    }

    public static Matrix operator -(Matrix a, Matrix b)
    {
      if (a.length != b.length && a.height != b.height) throw new ArgumentException("Matrix a and Matrix B are not the same size");

      Matrix c = new Matrix(a);

      for (int i = 0; i < c.height; i++)
      {
        for (int j = 0; j < c.length; j++)
        {
          c.matrix[i][j] -= b.matrix[i][j];
        }
      }
      return c;
    }
  }


}
