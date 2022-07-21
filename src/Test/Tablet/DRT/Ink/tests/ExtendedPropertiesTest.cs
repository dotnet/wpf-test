// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DRT
{
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class ExtendedPropertyCollectionTest : DrtInkTestcase
	{
        private string inkFileName = "wispExtProp.isf";

		static System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
		string StringFromByteArray(object obj)
		{
			byte [] bytes = obj as byte[];
			return encoding.GetString(bytes);
		}

		public override void Run()
		{
            StrokeCollection ink = DrtHelpers.LoadInk (inkFileName);

            Guid [] guids = {
                                new Guid("6DC08737-B9D4-4fc0-B9BF-8101320CFF7D"),
                                new Guid("8FF30DAC-DD37-4674-BE19-7D7E5ABC786E"),
                                new Guid("7CC5A020-1B27-4b19-946E-D28491E3A0F8"),
                                new Guid("0CB4F599-8B0F-4cbc-A05B-FA2A066B58E4"),
                                Guid.NewGuid(),
                                Guid.NewGuid(),
                                Guid.NewGuid(),
                                Guid.NewGuid(),
                                Guid.NewGuid(),
                            };


            string PropStr = "InkProp";

            int [] PropIntArray = {1, 2, 3, 4};
            int PropInt = 5;

            float [] PropFloatArray = {6.78f, 7.89f, 8.90f};
            float PropFloat = 11.13f;

            DateTime [] PropDateArray = {new DateTime(2003, 02, 17, 12, 15, 45),
                                                new DateTime(2003, 02, 18, 13, 16, 50),
                                                new DateTime(2003, 02, 19, 14, 20, 55),
            };
            DateTime PropDate = new DateTime(2003, 02, 20, 16, 25, 25);


            // First set the extended Properties in the Ink
            ink.AddPropertyData(guids[2], PropStr);
            ink.AddPropertyData(guids[3], PropIntArray);
            ink.AddPropertyData(guids[4], PropInt);
            ink.AddPropertyData(guids[5], PropFloatArray);
            ink.AddPropertyData(guids[6], PropFloat);
            ink.AddPropertyData(guids[7], PropDateArray);
            ink.AddPropertyData(guids[8], PropDate);

            int i;
//            Object o = ink.GetPropertyData(guids[0]);
//
//            for( i = 2; i < guids.Length; i++ )
//            {
//                Object o = ink.GetPropertyData(guids[i]);
//            }
//
            // Now verify the properties
            string prop2 = (string)ink.GetPropertyData(guids[2]);
            if( false == prop2.Equals(PropStr) )
                throw new InvalidOperationException("Property 2's value does not match with the original value");

            int[] prop3 = (int[])ink.GetPropertyData(guids[3]);
            if( false == prop3.Equals(PropIntArray) )
                throw new InvalidOperationException("Property 3's value does not match with the original value");

            int prop4 = (int)ink.GetPropertyData(guids[4]);
            if( false == prop4.Equals(PropInt) )
                throw new InvalidOperationException("Property 4's value does not match with the original value");

            float[] prop5 = (float[])ink.GetPropertyData(guids[5]);
            if( false == prop5.Equals(PropFloatArray) )
                throw new InvalidOperationException("Property 5's value does not match with the original value");

            float prop6 = (float)ink.GetPropertyData(guids[6]);
            if( false == prop6.Equals(PropFloat) )
                throw new InvalidOperationException("Property 6's value does not match with the original value");

            DateTime[] prop7 = (DateTime[])ink.GetPropertyData(guids[7]);
            if( false == prop7.Equals(PropDateArray) )
                throw new InvalidOperationException("Property 7's value does not match with the original value");

            DateTime prop8 = (DateTime)ink.GetPropertyData(guids[8]);
            if( false == prop8.Equals(PropDate) )
                throw new InvalidOperationException("Property 8's value does not match with the original value");

            //
            // extended properties on StrokeCollection
            //
            Random rand = new Random(0);
            int strkid = rand.Next(60);
            Stroke stroke = ink[strkid];

            // Set the stroke's properties
            stroke.AddPropertyData(guids[2], PropStr);
            stroke.AddPropertyData(guids[3], PropIntArray);
            stroke.AddPropertyData(guids[4], PropInt);
            stroke.AddPropertyData(guids[5], PropFloatArray);
            stroke.AddPropertyData(guids[6], PropFloat);
            stroke.AddPropertyData(guids[7], PropDateArray);
            stroke.AddPropertyData(guids[8], PropDate);

//            Object o = stroke.GetPropertyData(guids[0]);
//            throw new InvalidOperationException("property 0 found on " + strkid.ToString() + "-th Stroke of the Ink object - but it was not supposed to");
//            for( i = 2; i < guids.Length; i++ )
//            {
//                if( false == stroke.ExtendedProperties.Contains(guids[i]) )
//                    throw new InvalidOperationException("Property " + i.ToString() + " was not found on the" + strkid.ToString() + "-th Stroke of Ink Object");
//            }

            // Now verify the properties
            prop2 = (string)stroke.GetPropertyData(guids[2]);
            if( false == prop2.Equals(PropStr) )
                throw new InvalidOperationException("Property 2's value does not match with the original value");

            prop3 = (int[])stroke.GetPropertyData(guids[3]);
            if( false == prop3.Equals(PropIntArray) )
                throw new InvalidOperationException("Property 3's value does not match with the original value");

            prop4 = (int)stroke.GetPropertyData(guids[4]);
            if( false == prop4.Equals(PropInt) )
                throw new InvalidOperationException("Property 4's value does not match with the original value");

            prop5 = (float[])stroke.GetPropertyData(guids[5]);
            if( false == prop5.Equals(PropFloatArray) )
                throw new InvalidOperationException("Property 5's value does not match with the original value");

            prop6 = (float)stroke.GetPropertyData(guids[6]);
            if( false == prop6.Equals(PropFloat) )
                throw new InvalidOperationException("Property 6's value does not match with the original value");

            prop7 = (DateTime[])stroke.GetPropertyData(guids[7]);
            if( false == prop7.Equals(PropDateArray) )
                throw new InvalidOperationException("Property 7's value does not match with the original value");

            prop8 = (DateTime)stroke.GetPropertyData(guids[8]);
            if( false == prop8.Equals(PropDate) )
                throw new InvalidOperationException("Property 8's value does not match with the original value");

            // Now we need to make sure the properties persist across save/load. To verify that
            // first save the ink. Then Load the ink into a new Ink object and then verify the
            // properties on the new Ink object
            StrokeCollection newink = null;
            using (MemoryStream stream = new MemoryStream())
            {
                ink.Save(stream);
                stream.Position = 0;
                newink = new StrokeCollection(stream);
            }

            // Now verify the ink properties

//            if (true == newink.ExtendedProperties.Contains(guids[0]))
//                throw new InvalidOperationException("property 0 found on ink object - but it was not supposed to");
//            for( i = 2; i < guids.Length; i++ )
//            {
//                if( false == newink.ExtendedProperties.Contains(guids[i]) )
//                    throw new InvalidOperationException("Property " + i.ToString() + " was not found on the Ink Object");
//            }

            prop2 = (string)newink.GetPropertyData(guids[2]);
            if( false == prop2.Equals(PropStr) )
                throw new InvalidOperationException("Property 2's value does not match with the original value");

            prop3 = (int[])newink.GetPropertyData(guids[3]);
            if( prop3.Length != PropIntArray.Length )
                throw new InvalidOperationException("Property 3's count does not match with the original value");
            for( i = 0; i < prop3.Length; i++ )
            {
                if( false == prop3[i].Equals(PropIntArray[i]) )
                    throw new InvalidOperationException("Property 3's value does not match with the original value");
            }
            prop4 = (int)newink.GetPropertyData(guids[4]);
            if( false == prop4.Equals(PropInt) )
                throw new InvalidOperationException("Property 4's value does not match with the original value");

            prop5 = (float[])newink.GetPropertyData(guids[5]);
            if( prop5.Length != PropFloatArray.Length )
                throw new InvalidOperationException("Property 5's count does not match with the original value");

            for( i = 0; i < prop5.Length; i++ )
            {
                if( false == prop5[i].Equals(PropFloatArray[i]) )
                    throw new InvalidOperationException("Property 5's value does not match with the original value");
            }
            prop6 = (float)newink.GetPropertyData(guids[6]);
            if( false == prop6.Equals(PropFloat) )
                throw new InvalidOperationException("Property 6's value does not match with the original value");

            prop7 = (DateTime[])newink.GetPropertyData(guids[7]);
            if( prop7.Length != PropDateArray.Length )
                throw new InvalidOperationException("Property 7's value does not match with the original value");
            for( i = 0; i < prop7.Length; i++ )
            {
                if( false == prop7[i].Equals(PropDateArray[i]) )
                    throw new InvalidOperationException("Property 7's value does not match with the original value");
            }
            prop8 = (DateTime)newink.GetPropertyData(guids[8]);
            if( false == prop8.Equals(PropDate) )
                throw new InvalidOperationException("Property 8's value does not match with the original value");


            // Now compare the Stroke properties
            stroke = newink[strkid];
//
//            if (true == stroke.ExtendedProperties.Contains(guids[0]))
//                throw new InvalidOperationException("property 0 found on Stroke (" + strkid.ToString() + ") of Ink object - but it was not supposed to");
//            for( i = 2; i < guids.Length; i++ )
//            {
//                if( false == stroke.ExtendedProperties.Contains(guids[i]) )
//                    throw new InvalidOperationException("Property " + i.ToString() + " was not found on Stroke (" + strkid.ToString() + ") of Ink Object");
//            }

            // Now verify the properties
            prop2 = (string)stroke.GetPropertyData(guids[2]);
            if( false == prop2.Equals(PropStr) )
                throw new InvalidOperationException("Property 2's value does not match with the original value");

            prop3 = (int[])stroke.GetPropertyData(guids[3]);
            if( prop3.Length != PropIntArray.Length )
                throw new InvalidOperationException("Property 3's count does not match with the original value");
            for( i = 0; i < prop3.Length; i++ )
            {
                if( false == prop3[i].Equals(PropIntArray[i]) )
                    throw new InvalidOperationException("Property 3's value does not match with the original value");
            }
            prop4 = (int)stroke.GetPropertyData(guids[4]);
            if( false == prop4.Equals(PropInt) )
                throw new InvalidOperationException("Property 4's value does not match with the original value");

            prop5 = (float[])stroke.GetPropertyData(guids[5]);
            if( prop5.Length != PropFloatArray.Length )
                throw new InvalidOperationException("Property 5's count does not match with the original value");

            for( i = 0; i < prop5.Length; i++ )
            {
                if( false == prop5[i].Equals(PropFloatArray[i]) )
                    throw new InvalidOperationException("Property 5's value does not match with the original value");
            }
            prop6 = (float)stroke.GetPropertyData(guids[6]);
            if( false == prop6.Equals(PropFloat) )
                throw new InvalidOperationException("Property 6's value does not match with the original value");

            prop7 = (DateTime[])stroke.GetPropertyData(guids[7]);
            if( prop7.Length != PropDateArray.Length )
                throw new InvalidOperationException("Property 7's value does not match with the original value");
            for( i = 0; i < prop7.Length; i++ )
            {
                if( false == prop7[i].Equals(PropDateArray[i]) )
                    throw new InvalidOperationException("Property 7's value does not match with the original value");
            }
            prop8 = (DateTime)stroke.GetPropertyData(guids[8]);
            if( false == prop8.Equals(PropDate) )
                throw new InvalidOperationException("Property 8's value does not match with the original value");

            //
            // now test copying EPs on a Stroke
            //
            Stroke copiedStroke = stroke.Clone();

            // Now verify the properties
            prop2 = (string)copiedStroke.GetPropertyData(guids[2]);
            if (false == prop2.Equals(PropStr))
                throw new InvalidOperationException("Property 2's value does not match with the original value on the copied stroke");

            prop3 = (int[])copiedStroke.GetPropertyData(guids[3]);
            if (prop3.Length != PropIntArray.Length)
                throw new InvalidOperationException("Property 3's count does not match with the original value on the copied stroke");
            for (i = 0; i < prop3.Length; i++)
            {
                if (false == prop3[i].Equals(PropIntArray[i]))
                    throw new InvalidOperationException("Property 3's value does not match with the original value on the copied stroke");
            }

            //
            // now test to make sure that the two strokes aren't sharing a ref to the same array
            //
            int[] originalStrokeIntArray = (int[])stroke.GetPropertyData(guids[3]);
            int[] copiedStrokeIntArray = (int[])copiedStroke.GetPropertyData(guids[3]);
            copiedStrokeIntArray[0] = copiedStrokeIntArray[0] + 1;
            if (originalStrokeIntArray[0].Equals(copiedStrokeIntArray[0]))
            {
                throw new InvalidOperationException("Stroke.Clone() did not perform a deep copy of the ExtendedProperty's int[]");
            }

            //
            // try changing an array member on the copiedStroke, it should be different since
            // the array was deep copied
            //
            prop4 = (int)copiedStroke.GetPropertyData(guids[4]);
            if (false == prop4.Equals(PropInt))
                throw new InvalidOperationException("Property 4's value does not match with the original value on the copied stroke");

            prop5 = (float[])copiedStroke.GetPropertyData(guids[5]);
            if (prop5.Length != PropFloatArray.Length)
                throw new InvalidOperationException("Property 5's count does not match with the original value on the copied stroke");

            for (i = 0; i < prop5.Length; i++)
            {
                if (false == prop5[i].Equals(PropFloatArray[i]))
                    throw new InvalidOperationException("Property 5's value does not match with the original value on the copied stroke");
            }
            prop6 = (float)copiedStroke.GetPropertyData(guids[6]);
            if (false == prop6.Equals(PropFloat))
                throw new InvalidOperationException("Property 6's value does not match with the original value on the copied stroke");

            prop7 = (DateTime[])copiedStroke.GetPropertyData(guids[7]);
            if (prop7.Length != PropDateArray.Length)
                throw new InvalidOperationException("Property 7's value does not match with the original value on the copied stroke");
            for (i = 0; i < prop7.Length; i++)
            {
                if (false == prop7[i].Equals(PropDateArray[i]))
                    throw new InvalidOperationException("Property 7's value does not match with the original value on the copied stroke");
            }
            prop8 = (DateTime)copiedStroke.GetPropertyData(guids[8]);
            if (false == prop8.Equals(PropDate))
                throw new InvalidOperationException("Property 8's value does not match with the original value on the copied stroke");


            Success = true;
		}
	}
}

