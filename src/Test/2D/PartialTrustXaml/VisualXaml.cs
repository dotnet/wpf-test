// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.VisualVerification;
using System;
using System.Windows.Media;
using System.IO;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// This test suite is a port of the old 2d xamlbrowserhost tests.
    /// They render with the same purpose - to verify that we render correctly in partial trust.
    /// </summary>
    // [Test(1, "VisualXaml", "VisualXaml",
    //     Area = "2D",
    //     Description = @"Partial Trust Visual Xaml tests",
    //     SupportFiles = @"FeatureTests\2D\VscanData\TestAPIDefaultProfile.xml,FeatureTests\2D\VscanData\puppies.jpg,FeatureTests\2D\GtoData\Images\macaw.jpg,FeatureTests\2D\VscanData\alphamask.png,FeatureTests\2D\VscanData\peacock.jpg")]
    public class VisualXaml : StepsTest
    {
        //disabling ignored variations
        #region Variations
        // [Variation(@"P1path.xaml", @"P1path.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1path.xaml,FeatureTests\2D\VscanMasters\P1path.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1polyline.xaml", @"P1polyline.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1polyline.xaml,FeatureTests\2D\VscanMasters\P1polyline.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1rectangle.xaml", @"P1rectangle.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1rectangle.xaml,FeatureTests\2D\VscanMasters\P1rectangle.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTpolyline.xaml", @"BVTpolyline.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTpolyline.xaml,FeatureTests\2D\VscanMasters\BVTpolyline.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTDashPen.xaml", @"BVTDashPen.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTDashPen.xaml,FeatureTests\2D\VscanMasters\BVTDashPen.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTLineCap.xaml", @"BVTLineCap.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTLineCap.xaml,FeatureTests\2D\VscanMasters\BVTLineCap.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTEllipseGeo.xaml", @"BVTEllipseGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTEllipseGeo.xaml,FeatureTests\2D\VscanMasters\BVTEllipseGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTBezierPath.xaml", @"BVTBezierPath.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTBezierPath.xaml,FeatureTests\2D\VscanMasters\BVTBezierPath.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTEllipse2.xaml", @"BVTEllipse2.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTEllipse2.xaml,FeatureTests\2D\VscanMasters\BVTEllipse2.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTEllipse.xaml", @"BVTEllipse.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTEllipse.xaml,FeatureTests\2D\VscanMasters\BVTEllipse.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTEllipse1.xaml", @"BVTEllipse1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTEllipse1.xaml,FeatureTests\2D\VscanMasters\BVTEllipse1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTPathArc.xaml", @"BVTPathArc.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPathArc.xaml,FeatureTests\2D\VscanMasters\BVTPathArc.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTPolygon1.xaml", @"BVTPolygon1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPolygon1.xaml,FeatureTests\2D\VscanMasters\BVTPolygon1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTPolygon2.xaml", @"BVTPolygon2.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPolygon2.xaml,FeatureTests\2D\VscanMasters\BVTPolygon2.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTPolygon.xaml", @"BVTPolygon.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPolygon.xaml,FeatureTests\2D\VscanMasters\BVTPolygon.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTQuadraticPath.xaml", @"BVTQuadraticPath.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTQuadraticPath.xaml,FeatureTests\2D\VscanMasters\BVTQuadraticPath.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRectangle1.xaml", @"BVTRectangle1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectangle1.xaml,FeatureTests\2D\VscanMasters\BVTRectangle1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRectangle2.xaml", @"BVTRectangle2.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectangle2.xaml,FeatureTests\2D\VscanMasters\BVTRectangle2.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRectangle3.xaml", @"BVTRectangle3.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectangle3.xaml,FeatureTests\2D\VscanMasters\BVTRectangle3.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRectangle4.xaml", @"BVTRectangle4.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectangle4.xaml,FeatureTests\2D\VscanMasters\BVTRectangle4.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRectangle5.xaml", @"BVTRectangle5.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectangle5.xaml,FeatureTests\2D\VscanMasters\BVTRectangle5.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRectangle.xaml", @"BVTRectangle.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectangle.xaml,FeatureTests\2D\VscanMasters\BVTRectangle.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTSimplePath.xaml", @"BVTSimplePath.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTSimplePath.xaml,FeatureTests\2D\VscanMasters\BVTSimplePath.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTFillrule1.xaml", @"BVTFillrule1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTFillrule1.xaml,FeatureTests\2D\VscanMasters\BVTFillrule1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTFillrule.xaml", @"BVTFillrule.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTFillrule.xaml,FeatureTests\2D\VscanMasters\BVTFillrule.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTMiterLimit.xaml", @"BVTMiterLimit.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTMiterLimit.xaml,FeatureTests\2D\VscanMasters\BVTMiterLimit.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTStrokeLineJoin.xaml", @"BVTStrokeLineJoin.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTStrokeLineJoin.xaml,FeatureTests\2D\VscanMasters\BVTStrokeLineJoin.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTStrokeLineJoin2.xaml", @"BVTStrokeLineJoin2.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTStrokeLineJoin2.xaml,FeatureTests\2D\VscanMasters\BVTStrokeLineJoin2.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTDrawing1.xaml", @"BVTDrawing1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTDrawing1.xaml,FeatureTests\2D\VscanMasters\BVTDrawing1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTGeoComplement.xaml", @"BVTGeoComplement.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTGeoComplement.xaml,FeatureTests\2D\VscanMasters\BVTGeoComplement.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTGeoUnion.xaml", @"BVTGeoUnion.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTGeoUnion.xaml,FeatureTests\2D\VscanMasters\BVTGeoUnion.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTGeoXor.xaml", @"BVTGeoXor.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTGeoXor.xaml,FeatureTests\2D\VscanMasters\BVTGeoXor.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTLineGeo.xaml", @"BVTLineGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTLineGeo.xaml,FeatureTests\2D\VscanMasters\BVTLineGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTPathGeo.xaml", @"BVTPathGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPathGeo.xaml,FeatureTests\2D\VscanMasters\BVTPathGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRectGeo.xaml", @"BVTRectGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectGeo.xaml,FeatureTests\2D\VscanMasters\BVTRectGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTSolidColor.xaml", @"BVTSolidColor.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTSolidColor.xaml,FeatureTests\2D\VscanMasters\BVTSolidColor.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRectGeo1.xaml", @"BVTRectGeo1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectGeo1.xaml,FeatureTests\2D\VscanMasters\BVTRectGeo1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTSolidColor.xaml", @"BVTSolidColor.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTSolidColor.xaml,FeatureTests\2D\VscanMasters\BVTSolidColor.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTSolidColor1.xaml", @"BVTSolidColor1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTSolidColor1.xaml,FeatureTests\2D\VscanMasters\BVTSolidColor1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTLinearGrad1.xaml", @"BVTLinearGrad1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTLinearGrad1.xaml,FeatureTests\2D\VscanMasters\BVTLinearGrad1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTLinearGrad3.xaml", @"BVTLinearGrad3.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTLinearGrad3.xaml,FeatureTests\2D\VscanMasters\BVTLinearGrad3.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRadialGrad.xaml", @"BVTRadialGrad.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRadialGrad.xaml,FeatureTests\2D\VscanMasters\BVTRadialGrad.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRadialGrad3.xaml", @"BVTRadialGrad3.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRadialGrad3.xaml,FeatureTests\2D\VscanMasters\BVTRadialGrad3.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTImagebrush.xaml", @"BVTImagebrush.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTImagebrush.xaml,FeatureTests\2D\VscanMasters\BVTImagebrush.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTImagebrush1.xaml", @"BVTImagebrush1.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTImagebrush1.xaml,FeatureTests\2D\VscanMasters\BVTImagebrush1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTSkew.xaml", @"BVTSkew.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTSkew.xaml,FeatureTests\2D\VscanMasters\BVTSkew.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTScale.xaml", @"BVTScale.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTScale.xaml,FeatureTests\2D\VscanMasters\BVTScale.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTRotate.xaml", @"BVTRotate.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRotate.xaml,FeatureTests\2D\VscanMasters\BVTRotate.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTTranslate.xaml", @"BVTTranslate.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTTranslate.xaml,FeatureTests\2D\VscanMasters\BVTTranslate.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1rectangle.xaml", @"P1rectangle.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1rectangle.xaml,FeatureTests\2D\VscanMasters\P1rectangle.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1ellipse.xaml", @"P1ellipse.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ellipse.xaml,FeatureTests\2D\VscanMasters\P1ellipse.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1polygon.xaml", @"P1polygon.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1polygon.xaml,FeatureTests\2D\VscanMasters\P1polygon.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // //Disabling for .NET Core 3, Fix and re-enable. Only disabling on Windows Server 2008 R2.
        // [Variation(@"P1line.xaml", @"P1line.png", true, SupportFiles = @"FeatureTests\2D\VscanData\P1line.xaml,FeatureTests\2D\VscanMasters\P1line.png", SecurityLevel = TestCaseSecurityLevel.PartialTrust, Configurations = @"Infra\Configurations\No2K8R2.xml")]
        // [Variation(@"P1strokelinejoin.xaml", @"P1strokelinejoin.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1strokelinejoin.xaml,FeatureTests\2D\VscanMasters\P1strokelinejoin.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1fillrule.xaml", @"P1fillrule.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1fillrule.xaml,FeatureTests\2D\VscanMasters\P1fillrule.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1union.xaml", @"P1union.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1union.xaml,FeatureTests\2D\VscanMasters\P1union.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1complement.xaml", @"P1complement.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1complement.xaml,FeatureTests\2D\VscanMasters\P1complement.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1xor.xaml", @"P1xor.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1xor.xaml,FeatureTests\2D\VscanMasters\P1xor.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1rectGeo.xaml", @"P1rectGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1rectGeo.xaml,FeatureTests\2D\VscanMasters\P1rectGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1ellipseGeo.xaml", @"P1ellipseGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ellipseGeo.xaml,FeatureTests\2D\VscanMasters\P1ellipseGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1lineGeo.xaml", @"P1lineGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1lineGeo.xaml,FeatureTests\2D\VscanMasters\P1lineGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1pathGeo.xaml", @"P1pathGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1pathGeo.xaml,FeatureTests\2D\VscanMasters\P1pathGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1ellipseSolid.xaml", @"P1ellipseSolid.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ellipseSolid.xaml,FeatureTests\2D\VscanMasters\P1ellipseSolid.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1ellipseRadial.xaml", @"P1ellipseRadial.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ellipseRadial.xaml,FeatureTests\2D\VscanMasters\P1ellipseRadial.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1ellipseLinear.xaml", @"P1ellipseLinear.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ellipseLinear.xaml,FeatureTests\2D\VscanMasters\P1ellipseLinear.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1ellipseDrawing.xaml", @"P1ellipseDrawing.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ellipseDrawing.xaml,FeatureTests\2D\VscanMasters\P1ellipseDrawing.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1polygonDrawing.xaml", @"P1polygonDrawing.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1polygonDrawing.xaml,FeatureTests\2D\VscanMasters\P1polygonDrawing.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1polygonLinear.xaml", @"P1polygonLinear.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1polygonLinear.xaml,FeatureTests\2D\VscanMasters\P1polygonLinear.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1polygonRadial.xaml", @"P1polygonRadial.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1polygonRadial.xaml,FeatureTests\2D\VscanMasters\P1polygonRadial.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1polygonSolid.xaml", @"P1polygonSolid.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1polygonSolid.xaml,FeatureTests\2D\VscanMasters\P1polygonSolid.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1bezierPath.xaml", @"P1bezierPath.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1bezierPath.xaml,FeatureTests\2D\VscanMasters\P1bezierPath.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1pathArc.xaml", @"P1pathArc.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1pathArc.xaml,FeatureTests\2D\VscanMasters\P1pathArc.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1pathQuadratic.xaml", @"P1pathQuadratic.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1pathQuadratic.xaml,FeatureTests\2D\VscanMasters\P1pathQuadratic.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // //Disabling for .NET Core 3, Fix and re-enable. Only disabling on Windows Server 2008 R2.
        // [Variation(@"P1ellipseImage.xaml", @"P1ellipseImage.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ellipseImage.xaml,FeatureTests\2D\VscanMasters\P1ellipseImage.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust, Configurations = @"Infra\Configurations\No2K8R2.xml")]
        // [Variation(@"P1polygonImage.xaml", @"P1polygonImage.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1polygonImage.xaml,FeatureTests\2D\VscanMasters\P1polygonImage.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1DashEllipse.xaml", @"P1DashEllipse.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1DashEllipse.xaml,FeatureTests\2D\VscanMasters\P1DashEllipse.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1DashLine.xaml", @"P1DashLine.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1DashLine.xaml,FeatureTests\2D\VscanMasters\P1DashLine.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1DashPath.xaml", @"P1DashPath.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1DashPath.xaml,FeatureTests\2D\VscanMasters\P1DashPath.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1DashPolygon.xaml", @"P1DashPolygon.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1DashPolygon.xaml,FeatureTests\2D\VscanMasters\P1DashPolygon.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1MiterLimit.xaml", @"P1MiterLimit.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1MiterLimit.xaml,FeatureTests\2D\VscanMasters\P1MiterLimit.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1OpacityMaskImage.xaml", @"P1OpacityMaskImage.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1OpacityMaskImage.xaml,FeatureTests\2D\VscanMasters\P1OpacityMaskImage.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1OpacityMaskLine.xaml", @"P1OpacityMaskLine.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1OpacityMaskLine.xaml,FeatureTests\2D\VscanMasters\P1OpacityMaskLine.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1OpacityMaskPath.xaml", @"P1OpacityMaskPath.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1OpacityMaskPath.xaml,FeatureTests\2D\VscanMasters\P1OpacityMaskPath.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1OpacityMaskPolygon.xaml", @"P1OpacityMaskPolygon.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1OpacityMaskPolygon.xaml,FeatureTests\2D\VscanMasters\P1OpacityMaskPolygon.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1OpacityMaskPolyline.xaml", @"P1OpacityMaskPolyline.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1OpacityMaskPolyline.xaml,FeatureTests\2D\VscanMasters\P1OpacityMaskPolyline.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1OpacityMaskRadial.xaml", @"P1OpacityMaskRadial.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1OpacityMaskRadial.xaml,FeatureTests\2D\VscanMasters\P1OpacityMaskRadial.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1OpacityMaskRect.xaml", @"P1OpacityMaskRect.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1OpacityMaskRect.xaml,FeatureTests\2D\VscanMasters\P1OpacityMaskRect.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1OpacityMaskSolid.xaml", @"P1OpacityMaskSolid.png", true, SupportFiles = @"FeatureTests\2D\VscanData\P1OpacityMaskSolid.xaml,FeatureTests\2D\VscanMasters\P1OpacityMaskSolid.png", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1LineCap.xaml", @"P1LineCap.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1LineCap.xaml,FeatureTests\2D\VscanMasters\P1LineCap.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1DashPolyline.xaml", @"P1DashPolyline.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1DashPolyline.xaml,FeatureTests\2D\VscanMasters\P1DashPolyline.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1polylineDrawing.xaml", @"P1polylineDrawing.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1polylineDrawing.xaml,FeatureTests\2D\VscanMasters\P1polylineDrawing.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTEllipseGeo.xaml", @"BVTEllipseGeo.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTEllipseGeo.xaml,FeatureTests\2D\VscanMasters\BVTEllipseGeo.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTDashPen.xaml", @"BVTDashPen.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTDashPen.xaml,FeatureTests\2D\VscanMasters\BVTDashPen.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTLineCap.xaml", @"BVTLineCap.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTLineCap.xaml,FeatureTests\2D\VscanMasters\BVTLineCap.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Regression_Bug34.xaml", @"Regression_Bug34.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\Regression_Bug34.xaml,FeatureTests\2D\VscanMasters\Regression_Bug34.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Regression_Bug33.xaml", @"Regression_Bug33.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\Regression_Bug33.xaml,FeatureTests\2D\VscanMasters\Regression_Bug33.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Regression_Bug50.xaml", @"Regression_Bug50.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\Regression_Bug50.xaml,FeatureTests\2D\VscanMasters\Regression_Bug50.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Regression_Bug255.xaml", @"Regression_Bug255.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\Regression_Bug255.xaml,FeatureTests\2D\VscanMasters\Regression_Bug255.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Regression_Bug36.xaml", @"Regression_Bug36.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\Regression_Bug36.xaml,FeatureTests\2D\VscanMasters\Regression_Bug36.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTGeoComplement.xaml", @"BVTGeoComplement.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTGeoComplement.xaml,FeatureTests\2D\VscanMasters\BVTGeoComplement.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1Rotate36000000.xaml", @"P1Rotate36000000.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1Rotate36000000.xaml,FeatureTests\2D\VscanMasters\P1Rotate36000000.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTLineStretch.xaml", @"BVTLineStretch.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTLineStretch.xaml,FeatureTests\2D\VscanMasters\BVTLineStretch.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // //Disabling for .NET Core 3, Fix and re-enable. Only disabling on Windows Server 2008 R2.
        // [Variation(@"BVTPolygonStretch.xaml", @"BVTPolygonStretch.png", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPolygonStretch.xaml,FeatureTests\2D\VscanMasters\BVTPolygonStretch.png", SecurityLevel = TestCaseSecurityLevel.PartialTrust, Configurations = @"Infra\Configurations\No2K8R2.xml")]
        // [Variation(@"BVTPolylineStretch.xaml", @"BVTPolylineStretch.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPolylineStretch.xaml,FeatureTests\2D\VscanMasters\BVTPolylineStretch.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust, Configurations = @"Infra\Configurations\No2K8R2.xml")]
        // [Variation(@"BVTPathStretch.xaml", @"BVTPathStretch.png", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPathStretch.xaml,FeatureTests\2D\VscanMasters\BVTPathStretch.png", SecurityLevel = TestCaseSecurityLevel.PartialTrust, Configurations = @"Infra\Configurations\No2K8R2.xml")]
        // [Variation(@"BVTRectStretch.xaml", @"BVTRectStretch.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTRectStretch.xaml,FeatureTests\2D\VscanMasters\BVTRectStretch.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Regression_Bug51.xaml", @"Regression_Bug51.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\Regression_Bug51.xaml,FeatureTests\2D\VscanMasters\Regression_Bug51.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // //Disabling for .NET Core 3, Fix and re-enable. Only disabling on Windows Server 2008 R2.
        // [Variation(@"BVThvLine.xaml", @"BVThvLine.png", true, SupportFiles = @"FeatureTests\2D\VscanData\BVThvLine.xaml,FeatureTests\2D\VscanMasters\BVThvLine.png", SecurityLevel = TestCaseSecurityLevel.PartialTrust, Configurations = @"Infra\Configurations\No2K8R2.xml")]
        // [Variation(@"P1hvLineLinear.xaml", @"P1hvLineLinear.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1hvLineLinear.xaml,FeatureTests\2D\VscanMasters\P1hvLineLinear.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1hvLineRadial.xaml", @"P1hvLineRadial.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1hvLineRadial.xaml,FeatureTests\2D\VscanMasters\P1hvLineRadial.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1LineCap2.xaml", @"P1LineCap2.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1LineCap2.xaml,FeatureTests\2D\VscanMasters\P1LineCap2.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1ThinLine.xaml", @"P1ThinLine.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ThinLine.xaml,FeatureTests\2D\VscanMasters\P1ThinLine.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1rectThinLine.xaml", @"P1rectThinLine.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1rectThinLine.xaml,FeatureTests\2D\VscanMasters\P1rectThinLine.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1BigEllipse.xaml", @"P1BigEllipse.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1BigEllipse.xaml,FeatureTests\2D\VscanMasters\P1BigEllipse.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1BigLine.xaml", @"P1BigLine.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1BigLine.xaml,FeatureTests\2D\VscanMasters\P1BigLine.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1BigPath.xaml", @"P1BigPath.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1BigPath.xaml,FeatureTests\2D\VscanMasters\P1BigPath.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1BigPolygon.xaml", @"P1BigPolygon.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1BigPolygon.xaml,FeatureTests\2D\VscanMasters\P1BigPolygon.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1BigPolyline.xaml", @"P1BigPolyline.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1BigPolyline.xaml,FeatureTests\2D\VscanMasters\P1BigPolyline.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1BigRect.xaml", @"P1BigRect.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1BigRect.xaml,FeatureTests\2D\VscanMasters\P1BigRect.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1ShapesNoFill.xaml", @"P1ShapesNoFill.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1ShapesNoFill.xaml,FeatureTests\2D\VscanMasters\P1ShapesNoFill.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeRadial.xaml", @"P1StrokeRadial.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeRadial.xaml,FeatureTests\2D\VscanMasters\P1StrokeRadial.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeLinear.xaml", @"P1StrokeLinear.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeLinear.xaml,FeatureTests\2D\VscanMasters\P1StrokeLinear.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeDrawing.xaml", @"P1StrokeDrawing.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeDrawing.xaml,FeatureTests\2D\VscanMasters\P1StrokeDrawing.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BvtEllipseStretch.xaml", @"BvtEllipseStretch.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BvtEllipseStretch.xaml,FeatureTests\2D\VscanMasters\BvtEllipseStretch.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTPolylineFill.xaml", @"BVTPolylineFill.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTPolylineFill.xaml,FeatureTests\2D\VscanMasters\BVTPolylineFill.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"GuidelineCollectioninResources.xaml", @"GuidelineCollectioninResources.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\GuidelineCollectioninResources.xaml,FeatureTests\2D\VscanMasters\GuidelineCollectioninResources.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"GuidelineCollectioninDG.xaml", @"GuidelineCollectioninDG.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\GuidelineCollectioninDG.xaml,FeatureTests\2D\VscanMasters\GuidelineCollectioninDG.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"RadialGradientFocusOutsideEllipse.xaml", @"RadialGradientFocusOutsideEllipse.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\RadialGradientFocusOutsideEllipse.xaml,FeatureTests\2D\VscanMasters\RadialGradientFocusOutsideEllipse.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeDrawingLineCaps.xaml", @"P1StrokeDrawingLineCaps.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeDrawingLineCaps.xaml,FeatureTests\2D\VscanMasters\P1StrokeDrawingLineCaps.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Abuttin_Objects.xaml", @"Abuttin_Objects.png", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\Abutting_Objects\* , FeatureTests\2D\OfficeXaml\masters\Abuttin_Objects.png", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Escher_WordArt.xaml", @"Escher_WordArt.bmp", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\Escher_WordArt\*,FeatureTests\2D\VscanMasters\Escher_WordArt.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Flyer.xaml", @"Flyer.bmp", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\Flyer\*,FeatureTests\2D\VscanMasters\Flyer.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Gradient_emf.xaml", @"Gradient_emf.bmp", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\Gradient_emf\*,FeatureTests\2D\VscanMasters\Gradient_emf.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"MetaFile_Gradients_1.xaml", @"MetaFile_Gradients_1.bmp", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\MetaFile_Gradients_1\*,FeatureTests\2D\VscanMasters\MetaFile_Gradients_1.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"MetaFile_Gradients_2.xaml", @"MetaFile_Gradients_2.bmp", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\MetaFile_Gradients_2\*,FeatureTests\2D\VscanMasters\MetaFile_Gradients_2.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"RotatedRects.xaml", @"RotatedRects.bmp", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\RotatedRects\*,FeatureTests\2D\VscanMasters\RotatedRects.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Table_Gride_Lines.xaml", @"Table_Gride_Lines.bmp", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\Table_Gride_Lines\*,FeatureTests\2D\VscanMasters\Table_Gride_Lines.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Thin_Lines.xaml", @"Thin_Lines.bmp", true, SupportFiles = @"FeatureTests\2D\OfficeXaml\Thin_Lines\*,FeatureTests\2D\VscanMasters\Thin_Lines.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeDrawingLineJoin.xaml", @"P1StrokeDrawingLineJoin.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeDrawingLineJoin.xaml,FeatureTests\2D\VscanMasters\P1StrokeDrawingLineJoin.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeDrawingDashed.xaml", @"P1StrokeDrawingDashed.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeDrawingDashed.xaml,FeatureTests\2D\VscanMasters\P1StrokeDrawingDashed.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeLinearLineCap.xaml", @"P1StrokeLinearLineCap.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeLinearLineCap.xaml,FeatureTests\2D\VscanMasters\P1StrokeLinearLineCap.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeLinearLineJoin.xaml", @"P1StrokeLinearLineJoin.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeLinearLineJoin.xaml,FeatureTests\2D\VscanMasters\P1StrokeLinearLineJoin.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeLinearDashed.xaml", @"P1StrokeLinearDashed.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeLinearDashed.xaml,FeatureTests\2D\VscanMasters\P1StrokeLinearDashed.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeRadialLineCap.xaml", @"P1StrokeRadialLineCap.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeRadialLineCap.xaml,FeatureTests\2D\VscanMasters\P1StrokeRadialLineCap.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeRadialLineJoin.xaml", @"P1StrokeRadialLineJoin.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeRadialLineJoin.xaml,FeatureTests\2D\VscanMasters\P1StrokeRadialLineJoin.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeRadialDashed.xaml", @"P1StrokeRadialDashed.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeRadialDashed.xaml,FeatureTests\2D\VscanMasters\P1StrokeRadialDashed.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeVisualLineCap.xaml", @"P1StrokeVisualLineCap.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeVisualLineCap.xaml,FeatureTests\2D\VscanMasters\P1StrokeVisualLineCap.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeVisualLineJoin.xaml", @"P1StrokeVisualLineJoin.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeVisualLineJoin.xaml,FeatureTests\2D\VscanMasters\P1StrokeVisualLineJoin.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeVisualDashed.xaml", @"P1StrokeVisualDashed.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeVisualDashed.xaml,FeatureTests\2D\VscanMasters\P1StrokeVisualDashed.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeImageLineCap.xaml", @"P1StrokeImageLineCap.png", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeImageLineCap.xaml,FeatureTests\2D\VscanMasters\P1StrokeImageLineCap.png,FeatureTests\2D\VscanData\1BFLY.GIF", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeImageLineJoin.xaml", @"P1StrokeImageLineJoin.png", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeImageLineJoin.xaml,FeatureTests\2D\VscanMasters\P1StrokeImageLineJoin.png,FeatureTests\2D\VscanData\1BFLY.GIF", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeImageDashed.xaml", @"P1StrokeImageDashed.png", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeImageDashed.xaml,FeatureTests\2D\VscanMasters\P1StrokeImageDashed.png,FeatureTests\2D\VscanData\1BFLY.GIF", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeImageBrush.xaml", @"P1StrokeImageBrush.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeImageBrush.xaml,FeatureTests\2D\VscanMasters\P1StrokeImageBrush.bmp,FeatureTests\2D\VscanData\1BFLY.GIF", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeRadialDashedSW.xaml", @"P1StrokeRadialDashedSW.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeRadialDashedSW.xaml,FeatureTests\2D\VscanMasters\P1StrokeRadialDashedSW.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeImageDashedSW.xaml", @"P1StrokeImageDashedSW.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeImageDashedSW.xaml,FeatureTests\2D\VscanMasters\P1StrokeImageDashedSW.bmp,FeatureTests\2D\VscanData\1BFLY.GIF", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeDrawingDashedSW.xaml", @"P1StrokeDrawingDashedSW.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeDrawingDashedSW.xaml,FeatureTests\2D\VscanMasters\P1StrokeDrawingDashedSW.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeVisualDashedSW.xaml", @"P1StrokeVisualDashedSW.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeVisualDashedSW.xaml,FeatureTests\2D\VscanMasters\P1StrokeVisualDashedSW.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeRadialLineCapHW.xaml", @"P1StrokeRadialLineCapHW.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeRadialLineCapHW.xaml,FeatureTests\2D\VscanMasters\P1StrokeRadialLineCapHW.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeImageLineCapHW.xaml", @"P1StrokeImageLineCapHW.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeImageLineCapHW.xaml,FeatureTests\2D\VscanMasters\P1StrokeImageLineCapHW.bmp,FeatureTests\2D\VscanData\1BFLY.GIF", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P1StrokeDrawingLineCapsHW.xaml", @"P1StrokeDrawingLineCapsHW.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P1StrokeDrawingLineCapsHW.xaml,FeatureTests\2D\VscanMasters\P1StrokeDrawingLineCapsHW.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTImageDrawing.xaml", @"BVTImageDrawing.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTImageDrawing.xaml,FeatureTests\2D\VscanMasters\BVTImageDrawing.bmp,FeatureTests\2D\VscanData\maui_turtle.jpg", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"StreamGeometryBVT.xaml", @"StreamGeometryBVT.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\StreamGeometryBVT.xaml,FeatureTests\2D\VscanMasters\StreamGeometryBVT.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"LGBInvertedColor.xaml", @"LGBInvertedColor.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\LGBInvertedColor.xaml,FeatureTests\2D\VscanMasters\LGBInvertedColor.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"P2DashedPen.xaml", @"P2DashedPen.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\P2DashedPen.xaml,FeatureTests\2D\VscanMasters\P2DashedPen.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"VisualBrushCycle.xaml", @"VisualBrushCycle.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\VisualBrushCycle.xaml,FeatureTests\2D\VscanMasters\VisualBrushCycle.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"VisualBrushAsForegroundHW.xaml", @"VisualBrushAsForegroundHW.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\VisualBrushAsForegroundHW.xaml,FeatureTests\2D\VscanMasters\VisualBrushAsForegroundHW.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"GradientStopTests.xaml", @"GradientStopTests.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\GradientStopTests.xaml,FeatureTests\2D\VscanMasters\GradientStopTests.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"SnapsToDevicePixelsOnPath.xaml", @"SnapsToDevicePixelsOnPath.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\SnapsToDevicePixelsOnPath.xaml,FeatureTests\2D\VscanMasters\SnapsToDevicePixelsOnPath.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"Regression_Bug26.xaml", @"Regression_Bug26.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\Regression_Bug26.xaml,FeatureTests\2D\VscanMasters\Regression_Bug26.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"BVTSolidColor.xaml", @"BVTSolidColor.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\BVTSolidColor.xaml,FeatureTests\2D\VscanMasters\BVTSolidColor.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"RadialGradientBasic.xaml", @"RadialGradientBasic.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\RadialGradientBasic.xaml,FeatureTests\2D\VscanMasters\RadialGradientBasic.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"ImageBrushBasic.xaml", @"ImageBrushBasic.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\ImageBrushBasic.xaml,FeatureTests\2D\VscanMasters\ImageBrushBasic.bmp,FeatureTests\2D\VscanData\avalon.png", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"DrawingBrushBasic.xaml", @"DrawingBrushBasic.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\DrawingBrushBasic.xaml,FeatureTests\2D\VscanMasters\DrawingBrushBasic.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"VisualBrushBasic.xaml", @"VisualBrushBasic.png", true, SupportFiles = @"FeatureTests\2D\VscanData\VisualBrushBasic.xaml,FeatureTests\2D\VscanMasters\VisualBrushBasic.png,FeatureTests\2D\VscanData\avalon.png", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        // [Variation(@"VisualBrushIntegration_ElementLayout.xaml", @"VisualBrushIntegration_ElementLayout.bmp", true, SupportFiles = @"FeatureTests\2D\VscanData\VisualBrushIntegration_ElementLayout.xaml,FeatureTests\2D\VscanMasters\VisualBrushIntegration_ElementLayout.bmp", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
        #endregion
        /// <summary>
        /// Tests 2d visuals rendering in partial trust contexts.
        /// All tests are based on xaml files.
        /// </summary>
        /// <param name="xamlFileIn"> The test xaml to render</param>
        /// <param name="expectedFileIn"> Known good render of the input file</param>
        /// <param name="toleranceIn"> Allowable difference of any rendered pixel</param>
        /// <param name="successExpectedIn"> Whether we expect this test to succeed</param>        
        public VisualXaml(
                          string xamlFileIn,
                          string expectedFileIn,
                          bool successExpectedIn)
        {
            _settings.XamlFile = xamlFileIn;
            _settings.ExpectedFilename = expectedFileIn;
            _settings.SuccessExpected = successExpectedIn;

            InitializeSteps += new TestStep(LoadXaml);
            InitializeSteps += new TestStep(LoadReferenceContent);

            RunSteps += new TestStep(CreateWindowContent);
            RunSteps += new TestStep(Capture);
            RunSteps += new TestStep(Verification);
        }

        /// <returns>Whether the image was loadable</returns>
        private TestResult LoadReferenceContent()
        {
            LogComment("Loading reference image [" + _settings.ExpectedFilename + "]");

            _expected = Snapshot.FromFile(_settings.ExpectedFilename);
            if (null == _expected)
            {
                LogComment("reference image failed to load!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult LoadXaml()
        {
            XmlDocument xamldoc = new XmlDocument();
            Status("Loading xaml from " + _settings.XamlFile);
            xamldoc.Load(_settings.XamlFile);
            Status("xaml loaded;");
            _xaml = xamldoc.OuterXml;
            return TestResult.Pass;
        }

        /// <summary>
        /// Put the xaml into a top left, borderless window and render it
        /// </summary> 
        private TestResult CreateWindowContent()
        {
            Status("Making Security-wrapped window ...");
            _myWindow = new Window();
            _myWindow.WindowStyle = WindowStyle.None;
            _myWindow.AllowsTransparency = true;
            RenderOptions.SetBitmapScalingMode(_myWindow, BitmapScalingMode.Linear);
            _myWindow.Show();

            Status("Rendering ...");
            Stream stream = IOHelper.ConvertTextToStream(_xaml);
            ParserContext pc = new ParserContext();
            pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
            _myWindow.Content = XamlReader.Load(stream,pc) as FrameworkElement;
            _myWindow.Height = _windowHeight;
            _myWindow.Width = _windowWidth;
            _myWindow.Left = 0;
            _myWindow.Top = 0;

            DispatcherHelper.DoEvents(100);
            return TestResult.Pass;
        }

        /// <summary>
        /// Capture test image 
        /// </summary>
        private TestResult Capture()
        {
            // capture the image
            LogComment("Waiting for render...");
            DispatcherHelper.DoEvents(1000);
            DispatcherHelper.DoEvents(0, System.Windows.Threading.DispatcherPriority.Render);

            _captured = GrabWindowSnapshot(_windowWidth, _windowHeight);
            
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify renderings before and after
        /// </summary>
        private TestResult Verification()
        {
            LogComment("building histogram");
            Histogram tolerance = Histogram.FromFile(DefaultHistogram);

            LogComment("building verifier");
            SnapshotHistogramVerifier verifier = new SnapshotHistogramVerifier(tolerance);

            LogComment("comparing");
            Snapshot diff = _expected.CompareTo(_captured);

            LogComment("verifying");
            VerificationResult result = verifier.Verify(diff);

            bool imagesMatch = (result == VerificationResult.Pass);
          
            if (imagesMatch)
            {
                LogComment("Images match");
            }
            else
            {
                LogComment("Images do not match");
            }

            if (_settings.SuccessExpected)
            {
                LogComment("Images were expected to match");
            }
            else
            {
                LogComment("Images were not expected to match");
            }

            // unexpected results? log everything and fail.
            if (imagesMatch != _settings.SuccessExpected)
            {
                string loggedCaptureFile = _settings.ExpectedFilename + "-captured.png";
                string loggedDiffFile = _settings.ExpectedFilename + "-diff.png";
                string expectedFile = _settings.ExpectedFilename + "-expected.png";

                _captured.ToFile(loggedCaptureFile, ImageFormat.Png);
                diff.ToFile(loggedDiffFile, ImageFormat.Png);
                _expected.ToFile(expectedFile, ImageFormat.Png);
                GlobalLog.LogFile(loggedCaptureFile);
                GlobalLog.LogFile(loggedDiffFile);
                GlobalLog.LogFile(expectedFile);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #region Helpers
        /// <summary>
        /// Grab a rect from the topleft of the screen, where our window is rendering
        /// </summary>
        /// <returns>A snapshot of the window.</returns>
        /// <param name="width"> width of the rect to grab</param>
        /// <param name="height"> height of the rect to grab</param> 
        private Snapshot GrabWindowSnapshot(int width, int height)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(
                                                (int)0,
                                                (int)0,
                                                (int)width,
                                                (int)height);
            return Snapshot.FromRectangle(rect);
        }
        #endregion

        /// <summary>
        /// Each variation will have settings for the members of this struct,
        /// these are the test vectors for this test engine.
        /// </summary>
        internal struct TestSettings
        {
            public string XamlFile; // the source xaml file to render 
            public bool SuccessExpected; // whether this case is expected to match or not
            public string ExpectedFilename; // the known good image to compare this render to
        }
        private TestSettings _settings; // test vector values instance

        private Window _myWindow; // have to make our own because windowTest fails in PT
        private int _windowWidth = 800; // hardcoded for this set of tests
        private int _windowHeight = 480; // hardcoded for this set of tests
        private Snapshot _expected,_captured; // known good and rendered images
        private string _xaml; // the loaded xaml

        private const string DefaultHistogram = "TestAPIDefaultProfile.xml"; // taken from the ancient 'tyjonesdefaultprofile' and converted
    }
}

