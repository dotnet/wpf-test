// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using SWF = System.Windows.Forms;
using SWC = System.Windows.Controls;
using SWM = System.Windows.Media;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using SD = System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Test.Display;

//
// Testcase:    ScalingAndAutoScaling
// Description: EH controls should respond to any scaling applied to EH.
//
public class ScalingAndAutoScaling : ReflectBase
{
    #region Test case setup

    ElementHost _elementHost1;
    SWC.Button _avButton1;
    Bitmap _bmp;

    private int _origX;
    private int _origY;
    private int _origWidth;
    private int _origHeight;

    public ScalingAndAutoScaling(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "ScalingAndAutoScaling Test";

        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        Controls.Clear();
        this.Size = new System.Drawing.Size(400, 400);
        this.Font = null;

        //Create Element Host 1
        _elementHost1 = new ElementHost();
        _elementHost1.Location = new System.Drawing.Point(100, 100);
        Controls.Add(_elementHost1);

        _avButton1 = new SWC.Button();
        _avButton1.Content = "Avalon Button";
        _avButton1.Background = SWM.Brushes.White;
        _elementHost1.Child = _avButton1;

        return base.BeforeScenario(p, scenario);
    }

    #endregion

    #region Scenarios

    [Scenario("Check position and size.  AutoSize = true")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = true;
        Utilities.SleepDoEvents(2);

        GetOriginalLocationAndSize(_elementHost1);

        //Verify initial state
        Verify(sr, p, _origX, _origY, _origWidth, _origHeight, 40);
        p.log.WriteLine("Initial state: " + _elementHost1.Size.ToString() + ", " + 
            _elementHost1.Location.ToString());

        //-- Check AutoScaleMode=Font --//
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Font");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, (int)(_origX * 1.5), (int)(_origY * 1.9), (int)(_origWidth * 1.5), 
            (int)(_origHeight * 1), 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, (int)(_origX * 0.8), (int)(_origY * 0.8), (int)(_origWidth * 0.8),
            (int)(_origHeight * 1), 33);

        //-- Check AutoScaleMode=None --//
        this.AutoScaleMode = SWF.AutoScaleMode.None;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=None");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, (int)(_origX * 0.8), (int)(_origY * 0.8), (int)(_origWidth * 1.5),
            (int)(_origHeight * 1), 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, (int)(_origX * 0.8), (int)(_origY * 0.8), (int)(_origWidth * 0.8),
            (int)(_origHeight * 1), 33);

        //-- Check AutoScaleMode=Dpi --//
        this.AutoScaleMode = SWF.AutoScaleMode.Dpi;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Dpi");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, (int)(_origX * 0.8), (int)(_origY * 0.8), (int)(_origWidth * 1.5),
            (int)(_origHeight * 1), 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, (int)(_origX * 0.8), (int)(_origY * 0.8), (int)(_origWidth * 0.8),
            (int)(_origHeight * 1), 33);

        return sr;
    }

    [Scenario("Check position and size. AutoSize = false")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = false;
        Utilities.SleepDoEvents(2);

        //Verify initial state
        Verify(sr, p, 100, 100, 200, 100, 40);

        //-- Check AutoScaleMode=Font --//
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Font");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());

        Verify(sr, p, 167, 192, 333, 192, 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 40);

        //-- Check AutoScaleMode=None --//
        this.AutoScaleMode = SWF.AutoScaleMode.None;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=None");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 40);

        //-- Check AutoScaleMode=Dpi --//
        this.AutoScaleMode = SWF.AutoScaleMode.Dpi;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Dpi");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 40);

        return sr;
    }

    [Scenario("Docking with AutoSize = true")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = true;
        _elementHost1.Dock = SWF.DockStyle.Top;
        Utilities.SleepDoEvents(2);

        GetOriginalLocationAndSize(_elementHost1);

        //Verify initial state
        Verify(sr, p, _origX, _origY, (int)Monitor.ConvertLogicalToScreen(Dimension.Width, _origWidth), _origHeight, 40);

        //-- Check AutoScaleMode=Font --//
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Font");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 1.7),
            (int)(_origHeight * 1), 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 40);

        //-- Check AutoScaleMode=None --//
        this.AutoScaleMode = SWF.AutoScaleMode.None;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=None");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 40);

        //-- Check AutoScaleMode=Dpi --//
        this.AutoScaleMode = SWF.AutoScaleMode.Dpi;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Dpi");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 40);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 40);

        return sr;
    }

    [Scenario("Docking with AutoSize = false")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        _elementHost1.AutoSize = false;
        _elementHost1.Dock = SWF.DockStyle.Top;
        Utilities.SleepDoEvents(2);

        GetOriginalLocationAndSize(_elementHost1);

        //Verify initial state
        Verify(sr, p, _origX, _origY, (int)Monitor.ConvertLogicalToScreen(Dimension.Width, _origWidth), _origHeight, 80);

        //-- Check AutoScaleMode=Font --//
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Font");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 1.7),
            (int)(_origHeight * 1.9), 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 80);

        //-- Check AutoScaleMode=None --//
        this.AutoScaleMode = SWF.AutoScaleMode.None;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=None");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 80);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 80);

        //-- Check AutoScaleMode=Dpi --//
        this.AutoScaleMode = SWF.AutoScaleMode.Dpi;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Dpi");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 80);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, _origX, _origY, (int)(_origWidth * 0.7),
            (int)(_origHeight * 1), 80);

        return sr;
    }

    [Scenario("Scale Method")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        _elementHost1.Scale(new SizeF(1.1f, 1.1f));
        Utilities.SleepDoEvents(2);

        //Verify 1.1 scale state
        Verify(sr, p, 110, 110, 220, 110, 80);

        //-- Check AutoScaleMode=Font --//
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Font");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 183, 212, 367, 212, 80);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 73, 93, 147, 93, 75);

        //-- Check AutoScaleMode=None --//
        this.AutoScaleMode = SWF.AutoScaleMode.None;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=None");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 73, 93, 147, 93, 75);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 73, 93, 147, 93, 75);

        //-- Check AutoScaleMode=Dpi --//
        this.AutoScaleMode = SWF.AutoScaleMode.Dpi;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Dpi");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 73, 93, 147, 93, 75);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 73, 93, 147, 93, 75);

        return sr;
    }

    [Scenario("ScaleChildren property")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        sr.IncCounters(true, this.ScaleChildren, "ScaleChildren property should be true.", p.log);

        return sr;
    }

    [Scenario("ScaleControl on an inherited EH")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        ScaleControl(new SizeF(1.5f, 1.5f), System.Windows.Forms.BoundsSpecified.All);
        Utilities.SleepDoEvents(2);

        //Verify initial state
        Verify(sr, p, 100, 100, 200, 100, 40);

        //-- Check AutoScaleMode=Font --//
        this.AutoScaleMode = SWF.AutoScaleMode.Font;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Font");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 167, 192, 333, 192, 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 40);

        //-- Check AutoScaleMode=None --//
        this.AutoScaleMode = SWF.AutoScaleMode.None;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=None");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 40);

        //-- Check AutoScaleMode=Dpi --//
        this.AutoScaleMode = SWF.AutoScaleMode.Dpi;
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("Check AutoScaleMode=Dpi");

        //Increase the font
        this.Font = new Font("Impact", 20, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font increase to 20: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 33);

        //Decrease the font
        this.Font = new Font("Impact", 8, SD.FontStyle.Regular, GraphicsUnit.Pixel);
        Utilities.SleepDoEvents(2);
        p.log.WriteLine("After font decrease to 8: " + _elementHost1.Size.ToString() + ", " +
            _elementHost1.Location.ToString());
        Verify(sr, p, 67, 84, 133, 84, 40);

        return sr;
    }

    #endregion

    #region Utilities

    void GetOriginalLocationAndSize(ElementHost elementHost1)
    {
        _origX = elementHost1.Location.X;
        _origY = elementHost1.Location.Y;
        _origWidth = elementHost1.Width;
        _origHeight = elementHost1.Height;
    }


    private void Verify(ScenarioResult sr, TParams p, int x, int y, int width, int height, double percent)
    {
        x = (int)Monitor.ConvertScreenToLogical(Dimension.Width, x);
        y = (int)Monitor.ConvertScreenToLogical(Dimension.Height, y);
        width = (int)Monitor.ConvertScreenToLogical(Dimension.Width, width);
        height = (int)Monitor.ConvertScreenToLogical(Dimension.Height, height);
	int variance = (int)Monitor.ConvertLogicalToScreen(Dimension.Height, 40);

        sr.IncCounters((_elementHost1.Location.X >= x - variance && _elementHost1.Location.X <= x + variance), 
            "Failed at elementHost1.Location.X.  Expected (estimate): " +
            x + ", Actual: " + _elementHost1.Location.X, p.log);
        sr.IncCounters((_elementHost1.Location.Y >= y - variance && _elementHost1.Location.Y <= y + variance), 
            "Failed at elementHost1.Location.Y.  Expected (estimate): " +
            y + ", Actual: " + _elementHost1.Location.Y, p.log);
        sr.IncCounters((_elementHost1.Width >= width - variance && _elementHost1.Width <= width + variance), 
            "Failed at elementHost1.Width.  Expected: " +
            width + ", Actual: " + _elementHost1.Width, p.log);
        sr.IncCounters((_elementHost1.Height >= height - variance && _elementHost1.Height <= height + variance), 
            "Failed at elementHost1.Height.  Expected: " +
            height + ", Actual: " + _elementHost1.Height, p.log);
        BitmapTest(sr, p, _elementHost1.Location.X, _elementHost1.Location.Y, _elementHost1.Width,
            _elementHost1.Height, Color.White, 0.5);
    }

    //Takes a bitmap of the form and checks the color of pixels of the bitmap in the given area by 
    //calling BitmapsColorPercent.
    private void BitmapTest(ScenarioResult sr, TParams p, int x, int y,
        int width, int height, Color color, double percent)
    {
        _bmp = Utilities.GetBitmapOfControl(this);
        Utilities.SleepDoEvents(2);
        sr.IncCounters(BitmapsColorPercent(_bmp, x, y, width, height, color) >= percent,
            "Bitmap Failed at Color=" + color + ". Percent match: " +
            BitmapsColorPercent(_bmp, x, y, width, height, color) + "%. Expected: more than " + 
            percent + "%", p.log);
    }

    /// <summary>
    /// Returns the percentage of matching pixels to total pixels with the same ARGB value as the given color,
    /// in the area specified by width and height starting at location (x, y).  The algorithm checks each
    /// pixel in the bitmap and increments the match counter each time it finds a color match.
    /// </summary>
    /// <param name="bmp">Bitmap to search.</param>
    /// <param name="x">X-axis starting location.</param>
    /// <param name="y">Y-axis starting location.</param>
    /// <param name="width">Width of area to search.</param>
    /// <param name="height">Height of area to search.</param>
    /// <param name="color">Color to search for.</param>
    /// <returns>Percentage of matching pixels to total pixels.</returns>
    private static double BitmapsColorPercent(Bitmap bmp, int x, int y, int width, int height, Color color)
    {
        double total = 0;
        double match = 0;
        for (int col = x; col < x + width; col++)
        {
            for (int row = y; row < y + height; row++)
            {
                if (Utilities.ColorsMatch(bmp.GetPixel(col, row), color))
                {
                    match++;
                }
                total++;
            }
        }
        return match * 100 / total;
    }

    #endregion
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Check position and size.  AutoSize = true
//@ Check position and size. AutoSize = false
//@ Docking with AutoSize = true
//@ Docking with AutoSize = false
//@ Scale method
//@ ScaleChildren property
//@ ScaleControl on an inherited EH