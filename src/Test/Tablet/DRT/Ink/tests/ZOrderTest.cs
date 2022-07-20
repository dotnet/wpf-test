// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Ink;

namespace DRT
{
	/// <summary>
	/// Summary description for ZOrderTest.
	/// </summary>
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class ZOrderTest : DrtInkTestcase
	{
        public override void Run()
        {
            StrokeCollection ink = new StrokeCollection();

            ink.Add(new Stroke(new StylusPointCollection(new StylusPoint[] {
                new StylusPoint(1, 1), new StylusPoint(1, 1)})));
            ink.Add(new Stroke(new StylusPointCollection(new StylusPoint[] {
                new StylusPoint(2, 2), new StylusPoint(1, 1)})));
            ink.Add(new Stroke(new StylusPointCollection(new StylusPoint[] {
                new StylusPoint(4, 4), new StylusPoint(1, 1)})));
            ink.Add(new Stroke(new StylusPointCollection(new StylusPoint[] {
                new StylusPoint(5, 5), new StylusPoint(1, 1)})));

            ////////////////////////////////////////////////////////
            // Test 1:  Verify insert
            ////////////////////////////////////////////////////////
            {
                ink.Insert(2, new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(3, 3) })));
                for ( int i = 1; i <= ink.Count; i++ )
                {
                    if ( ink[i - 1].StylusPoints[0] != new StylusPoint(i, i) )
                    {
                        throw new InvalidOperationException("Stroke != expected");
                    }
                }
                if ( ink.Count != 5 )
                {
                    throw new InvalidOperationException("Stroke Count != expected");
                }
            }

            ////////////////////////////////////////////////////////
            // Test 2:  Verify no-op replace
            ////////////////////////////////////////////////////////
            {
                bool caughtException = false;
                try
                {
                    ink[3] = ink[3];
                }
                catch (ArgumentException)
                {
                    caughtException = true;
                }

                if ( !caughtException )
                {
                    throw new InvalidOperationException("ink[3] = ink[3] should cause ArgumentException being thrown");
                }

                for ( int i = 1; i <= ink.Count; i++ )
                {
                    if ( ink[i - 1].StylusPoints[0] != new StylusPoint(i, i) )
                    {
                        throw new InvalidOperationException("Stroke != expected");
                    }
                }

                if ( ink.Count != 5 )
                {
                    throw new InvalidOperationException("Stroke Count != expected");
                }
            }

            ////////////////////////////////////////////////////////
            // Test 3:  Verify re-ordering (R.R.I.I)
            //          ink[2] <=> ink[3]
            //          1,1         1,1
            //          2,2         2,2
            //          3,3   <=>   4,4
            //          4,4         3,3
            //          5,5         5,5
            ////////////////////////////////////////////////////////
            {
                Stroke ink2 = ink[2];
                Stroke ink3 = ink[3];
                ink.Remove(ink2);
                ink.Remove(ink3);
                ink.Insert(2, ink3);
                ink.Insert(3, ink2);
                for ( int i = 0; i < ink.Count; i++ )
                {
                    if ( i == 2 )
                    {
                        if ( ink[i].StylusPoints[0] != new StylusPoint(4, 4) )
                        {
                            throw new InvalidOperationException("Stroke != expected");
                        }
                    }
                    else if ( i == 3 )
                    {
                        if ( ink[i].StylusPoints[0] != new StylusPoint(3, 3) )
                        {
                            throw new InvalidOperationException("Stroke != expected");
                        }
                    }
                    else
                    {
                        if ( ink[i].StylusPoints[0] != new StylusPoint(i + 1, i + 1) )
                        {
                            throw new InvalidOperationException("Stroke != expected");
                        }
                    }
                }

                if ( ink.Count != 5 )
                {
                    throw new InvalidOperationException("Stroke Count != expected");
                }
            }

            ////////////////////////////////////////////////////////
            // Test 4:  Verify re-ordering (R.Indexer.I)
            //          ink[3] <=> ink[1]
            //          1,1         1,1
            //          2,2         3,3
            //          4,4   <=>   4,4
            //          3,3         2,2
            //          5,5         5,5
            ////////////////////////////////////////////////////////
            {
                Stroke ink3 = ink[3];
                Stroke ink1 = ink[1];

                ink.Remove(ink3);
                ink[1] = ink3;
                ink.Insert(3, ink1);

                for ( int i = 0; i < ink.Count; i++ )
                {
                    if ( i == 1 )
                    {
                        if ( ink[i].StylusPoints[0] != new StylusPoint(3, 3) )
                        {
                            throw new InvalidOperationException("Stroke != expected");
                        }
                    }
                    else if ( i == 2 )
                    {
                        if ( ink[i].StylusPoints[0] != new StylusPoint(4, 4) )
                        {
                            throw new InvalidOperationException("Stroke != expected");
                        }
                    }
                    else if ( i == 3 )
                    {
                        if ( ink[i].StylusPoints[0] != new StylusPoint(2, 2) )
                        {
                            throw new InvalidOperationException("Stroke != expected");
                        }
                    }
                    else
                    {
                        if ( ink[i].StylusPoints[0] != new StylusPoint(i + 1, i + 1) )
                        {
                            throw new InvalidOperationException("Stroke != expected");
                        }
                    }
                }

                if ( ink.Count != 5 )
                {
                    throw new InvalidOperationException("Stroke Count != expected");
                }
            }

            Success = true;
        }
	}
}
