// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: This file contains a few simple perf tests.
//
//

using System;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;

public class DrtMediaAPI
{
    /// <summary>
    /// Compuate the square of each element of an array of Matrices
    /// </summary>
    /// <returns>
    /// Return milliseconds required.
    /// </returns>
    static private double TimeSquares(Matrix[] matrices)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < matrices.Length; ++i)
        {
            matrices[i] = matrices[i]*matrices[i];
        }
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    /// <returns>
    /// Return array of scale matrices.
    /// </returns>
    static public Matrix[] ScaleMatrices(int count)
    {
        Matrix[] matrices = new Matrix[count];
        Random random = new Random();
        for (int i = 0; i < matrices.Length; ++i)
        {
            matrices[i] = new Matrix(random.NextDouble(),0,
                                     0,random.NextDouble(),
                                     0,0);
        }
        return matrices;
    }

    /// <returns>
    /// Return array of scale matrices.
    /// </returns>
    static public Matrix[] TranslationMatrices(int count)
    {
        Matrix[] matrices = new Matrix[count];
        Random random = new Random();
        for (int i = 0; i < matrices.Length; ++i)
        {
            matrices[i] = new Matrix(1,0,
                                     0,1,
                                     random.NextDouble(),
                                     random.NextDouble());
        }
        return matrices;
    }

    /// <returns>
    /// Return array of random matrices.
    /// </returns>
    static public Matrix[] RandomMatrices(int count)
    {
        Matrix[] matrices = new Matrix[count];
        Random random = new Random();
        for (int i = 0; i < matrices.Length; ++i)
        {
            matrices[i] = new Matrix(random.NextDouble(),
                                     random.NextDouble(),
                                     random.NextDouble(),
                                     random.NextDouble(),
                                     random.NextDouble(),
                                     random.NextDouble());
        }
        return matrices;
    }

    [STAThread]
    public static int Main()
    {
        const int ITERS = 20;
        const int COUNT = 1000000;

        Console.Write( "Count is " + COUNT );
        Console.Write( "\n      Scale" );
        for (int i = 0; i < ITERS; ++i)
        {
            Matrix[] matrices = ScaleMatrices(COUNT);
            double seconds = TimeSquares(matrices);
            Console.Write(" " + seconds);
        }
        Console.Write( "\nTranslation" );
        for (int i = 0; i < ITERS; ++i)
        {
            Matrix[] matrices = TranslationMatrices(COUNT);
            double seconds = TimeSquares(matrices);
            Console.Write(" " + seconds);
        }
        Console.Write( "\n     Random" );
        for (int i = 0; i < ITERS; ++i)
        {
            Matrix[] matrices = RandomMatrices(COUNT);
            double seconds = TimeSquares(matrices);
            Console.Write(" " + seconds);
        }
        
        return 0;
    }
}
