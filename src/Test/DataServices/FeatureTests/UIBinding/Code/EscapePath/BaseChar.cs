// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.DataServices
{
    class BaseChar : RandomStringProvider
    {
        public BaseChar(Random random) : base(random)
        {
            #region Add ranges
            this.ranges.Add(new Range('\u0041', '\u005A'));
            this.ranges.Add(new Range('\u0061', '\u007A'));
            this.ranges.Add(new Range('\u00C0', '\u00D6'));
            this.ranges.Add(new Range('\u00D8', '\u00F6'));
            this.ranges.Add(new Range('\u00F8', '\u00FF'));

            this.ranges.Add(new Range('\u0100', '\u0131'));
            this.ranges.Add(new Range('\u0134', '\u013E'));
            this.ranges.Add(new Range('\u0141', '\u0148'));
            this.ranges.Add(new Range('\u014A', '\u017E'));
            this.ranges.Add(new Range('\u0180', '\u01C3'));

            this.ranges.Add(new Range('\u0180', '\u01C3'));
            this.ranges.Add(new Range('\u0180', '\u01C3'));
            this.ranges.Add(new Range('\u0180', '\u01C3'));
            this.ranges.Add(new Range('\u0180', '\u01C3'));
            this.ranges.Add(new Range('\u0180', '\u01C3'));

            this.ranges.Add(new Range('\u01CD', '\u01F0'));
            this.ranges.Add(new Range('\u01F4', '\u01F5'));
            this.ranges.Add(new Range('\u01FA', '\u0217'));
            this.ranges.Add(new Range('\u0250', '\u02A8'));
            this.ranges.Add(new Range('\u02BB', '\u02C1'));

            this.ranges.Add(new Range('\u0386', '\u0386'));
            this.ranges.Add(new Range('\u0388', '\u038A'));
            this.ranges.Add(new Range('\u038C', '\u038C'));
            this.ranges.Add(new Range('\u038E', '\u03A1'));
            this.ranges.Add(new Range('\u03A3', '\u03CE'));

            this.ranges.Add(new Range('\u03D0', '\u03D6'));
            this.ranges.Add(new Range('\u03DA', '\u03DA'));
            this.ranges.Add(new Range('\u03DC', '\u03DC'));
            this.ranges.Add(new Range('\u03DE', '\u03DE'));
            this.ranges.Add(new Range('\u03E0', '\u03E0'));

            this.ranges.Add(new Range('\u03E2', '\u03F3'));
            this.ranges.Add(new Range('\u0401', '\u040C'));
            this.ranges.Add(new Range('\u040E', '\u044F'));
            this.ranges.Add(new Range('\u0451', '\u045C'));
            this.ranges.Add(new Range('\u045E', '\u0481'));

            this.ranges.Add(new Range('\u0490', '\u04C4'));
            this.ranges.Add(new Range('\u04C7', '\u04C8'));
            this.ranges.Add(new Range('\u04CB', '\u04CC'));
            this.ranges.Add(new Range('\u04D0', '\u04EB'));
            this.ranges.Add(new Range('\u04EE', '\u04F5'));

            this.ranges.Add(new Range('\u04F8', '\u04F9'));
            this.ranges.Add(new Range('\u0531', '\u0556'));
            this.ranges.Add(new Range('\u0559', '\u0559'));
            this.ranges.Add(new Range('\u0561', '\u0586'));
            this.ranges.Add(new Range('\u05D0', '\u05EA'));

            this.ranges.Add(new Range('\u05F0', '\u05F2'));
            this.ranges.Add(new Range('\u0621', '\u063A'));
            this.ranges.Add(new Range('\u0641', '\u064A'));
            this.ranges.Add(new Range('\u0671', '\u06B7'));
            this.ranges.Add(new Range('\u06BA', '\u06BE'));

            this.ranges.Add(new Range('\u06C0', '\u06CE'));
            this.ranges.Add(new Range('\u06D0', '\u06D3'));
            this.ranges.Add(new Range('\u06D5', '\u06D5'));
            this.ranges.Add(new Range('\u06E5', '\u06E6'));
            this.ranges.Add(new Range('\u0905', '\u0939'));

            this.ranges.Add(new Range('\u093D', '\u093D'));
            this.ranges.Add(new Range('\u0958', '\u0961'));
            this.ranges.Add(new Range('\u0985', '\u098C'));
            this.ranges.Add(new Range('\u098F', '\u0990'));
            this.ranges.Add(new Range('\u0993', '\u09A8'));

            this.ranges.Add(new Range('\u09AA', '\u09B0'));
            this.ranges.Add(new Range('\u09B2', '\u09B2'));
            this.ranges.Add(new Range('\u09B6', '\u09B9'));
            this.ranges.Add(new Range('\u09DC', '\u09DD'));
            this.ranges.Add(new Range('\u09DF', '\u09E1'));

            this.ranges.Add(new Range('\u09F0', '\u09F1'));
            this.ranges.Add(new Range('\u0A05', '\u0A0A'));
            this.ranges.Add(new Range('\u0A0F', '\u0A10'));
            this.ranges.Add(new Range('\u0A13', '\u0A28'));
            this.ranges.Add(new Range('\u0A2A', '\u0A30'));

            this.ranges.Add(new Range('\u0A32', '\u0A33'));
            this.ranges.Add(new Range('\u0A35', '\u0A36'));
            this.ranges.Add(new Range('\u0A38', '\u0A39'));
            this.ranges.Add(new Range('\u0A59', '\u0A5C'));
            this.ranges.Add(new Range('\u0A5E', '\u0A5E'));

            this.ranges.Add(new Range('\u0A72', '\u0A74'));
            this.ranges.Add(new Range('\u0A85', '\u0A8B'));
            this.ranges.Add(new Range('\u0A8D', '\u0A8D'));
            this.ranges.Add(new Range('\u0A8F', '\u0A91'));
            this.ranges.Add(new Range('\u0A93', '\u0AA8'));

            this.ranges.Add(new Range('\u0AAA', '\u0AB0'));
            this.ranges.Add(new Range('\u0AB2', '\u0AB3'));
            this.ranges.Add(new Range('\u0AB5', '\u0AB9'));
            this.ranges.Add(new Range('\u0ABD', '\u0ABD'));
            this.ranges.Add(new Range('\u0AE0', '\u0AE0'));

            this.ranges.Add(new Range('\u0B05', '\u0B0C'));
            this.ranges.Add(new Range('\u0B0F', '\u0B10'));
            this.ranges.Add(new Range('\u0B13', '\u0B28'));
            this.ranges.Add(new Range('\u0B2A', '\u0B30'));
            this.ranges.Add(new Range('\u0B32', '\u0B33'));

            this.ranges.Add(new Range('\u0B36', '\u0B39'));
            this.ranges.Add(new Range('\u0B3D', '\u0B3D'));
            this.ranges.Add(new Range('\u0B5C', '\u0B5D'));
            this.ranges.Add(new Range('\u0B5F', '\u0B61'));
            this.ranges.Add(new Range('\u0B85', '\u0B8A'));

            this.ranges.Add(new Range('\u0B8E', '\u0B90'));
            this.ranges.Add(new Range('\u0B92', '\u0B95'));
            this.ranges.Add(new Range('\u0B99', '\u0B9A'));
            this.ranges.Add(new Range('\u0B9C', '\u0B9C'));
            this.ranges.Add(new Range('\u0B9E', '\u0B9F'));

            this.ranges.Add(new Range('\u0BA3', '\u0BA4'));
            this.ranges.Add(new Range('\u0BA8', '\u0BAA'));
            this.ranges.Add(new Range('\u0BAE', '\u0BB5'));
            this.ranges.Add(new Range('\u0BB7', '\u0BB9'));
            this.ranges.Add(new Range('\u0C05', '\u0C0C'));

            this.ranges.Add(new Range('\u0C0E', '\u0C10'));
            this.ranges.Add(new Range('\u0C12', '\u0C28'));
            this.ranges.Add(new Range('\u0C2A', '\u0C33'));
            this.ranges.Add(new Range('\u0C35', '\u0C39'));
            this.ranges.Add(new Range('\u0C60', '\u0C61'));

            this.ranges.Add(new Range('\u0C85', '\u0C8C'));
            this.ranges.Add(new Range('\u0C8E', '\u0C90'));
            this.ranges.Add(new Range('\u0C92', '\u0CA8'));
            this.ranges.Add(new Range('\u0CAA', '\u0CB3'));
            this.ranges.Add(new Range('\u0CB5', '\u0CB9'));

            this.ranges.Add(new Range('\u0CDE', '\u0CDE'));
            this.ranges.Add(new Range('\u0CE0', '\u0CE1'));
            this.ranges.Add(new Range('\u0D05', '\u0D0C'));
            this.ranges.Add(new Range('\u0D0E', '\u0D10'));
            this.ranges.Add(new Range('\u0D12', '\u0D28'));

            this.ranges.Add(new Range('\u0D2A', '\u0D39'));
            this.ranges.Add(new Range('\u0D60', '\u0D61'));
            this.ranges.Add(new Range('\u0E01', '\u0E2E'));
            this.ranges.Add(new Range('\u0E30', '\u0E30'));
            this.ranges.Add(new Range('\u0E32', '\u0E33'));

            this.ranges.Add(new Range('\u0E40', '\u0E45'));
            this.ranges.Add(new Range('\u0E81', '\u0E82'));
            this.ranges.Add(new Range('\u0E84', '\u0E84'));
            this.ranges.Add(new Range('\u0E87', '\u0E88'));
            this.ranges.Add(new Range('\u0E8A', '\u0E8A'));

            this.ranges.Add(new Range('\u0E8D', '\u0E8D'));
            this.ranges.Add(new Range('\u0E94', '\u0E97'));
            this.ranges.Add(new Range('\u0E99', '\u0E9F'));
            this.ranges.Add(new Range('\u0EA1', '\u0EA3'));
            this.ranges.Add(new Range('\u0EA5', '\u0EA5'));

            this.ranges.Add(new Range('\u0EA7', '\u0EA7'));
            this.ranges.Add(new Range('\u0EAA', '\u0EAB'));
            this.ranges.Add(new Range('\u0EAD', '\u0EAE'));
            this.ranges.Add(new Range('\u0EB0', '\u0EB0'));
            this.ranges.Add(new Range('\u0EB2', '\u0EB3'));

            this.ranges.Add(new Range('\u0EBD', '\u0EBD'));
            this.ranges.Add(new Range('\u0EC0', '\u0EC4'));
            this.ranges.Add(new Range('\u0F40', '\u0F47'));
            this.ranges.Add(new Range('\u0F49', '\u0F69'));
            this.ranges.Add(new Range('\u10A0', '\u10C5'));

            this.ranges.Add(new Range('\u10D0', '\u10F6'));
            this.ranges.Add(new Range('\u1100', '\u1100'));
            this.ranges.Add(new Range('\u1102', '\u1103'));
            this.ranges.Add(new Range('\u1105', '\u1107'));
            this.ranges.Add(new Range('\u1109', '\u1109'));

            this.ranges.Add(new Range('\u110B', '\u110C'));
            this.ranges.Add(new Range('\u110E', '\u1112'));
            this.ranges.Add(new Range('\u113C', '\u113C'));
            this.ranges.Add(new Range('\u113E', '\u113E'));
            this.ranges.Add(new Range('\u1140', '\u1140'));

            this.ranges.Add(new Range('\u114C', '\u114C'));
            this.ranges.Add(new Range('\u114E', '\u114E'));
            this.ranges.Add(new Range('\u1150', '\u1150'));
            this.ranges.Add(new Range('\u1154', '\u1155'));
            this.ranges.Add(new Range('\u1159', '\u1159'));

            this.ranges.Add(new Range('\u115F', '\u1161'));
            this.ranges.Add(new Range('\u1163', '\u1163'));
            this.ranges.Add(new Range('\u1165', '\u1165'));
            this.ranges.Add(new Range('\u1167', '\u1167'));
            this.ranges.Add(new Range('\u1169', '\u1169'));

            this.ranges.Add(new Range('\u116D', '\u116E'));
            this.ranges.Add(new Range('\u1172', '\u1173'));
            this.ranges.Add(new Range('\u1175', '\u1175'));
            this.ranges.Add(new Range('\u119E', '\u119E'));
            this.ranges.Add(new Range('\u11A8', '\u11A8'));

            this.ranges.Add(new Range('\u11AB', '\u11AB'));
            this.ranges.Add(new Range('\u11AE', '\u11AF'));
            this.ranges.Add(new Range('\u11B7', '\u11B8'));
            this.ranges.Add(new Range('\u11BA', '\u11BA'));
            this.ranges.Add(new Range('\u11BC', '\u11C2'));

            this.ranges.Add(new Range('\u11EB', '\u11EB'));
            this.ranges.Add(new Range('\u11F0', '\u11F0'));
            this.ranges.Add(new Range('\u11F9', '\u11F9'));
            this.ranges.Add(new Range('\u1E00', '\u1E9B'));
            this.ranges.Add(new Range('\u1EA0', '\u1EF9'));

            this.ranges.Add(new Range('\u1F00', '\u1F15'));
            this.ranges.Add(new Range('\u1F18', '\u1F1D'));
            this.ranges.Add(new Range('\u1F20', '\u1F45'));
            this.ranges.Add(new Range('\u1F48', '\u1F4D'));
            this.ranges.Add(new Range('\u1F50', '\u1F57'));

            this.ranges.Add(new Range('\u1F59', '\u1F59'));
            this.ranges.Add(new Range('\u1F5B', '\u1F5B'));
            this.ranges.Add(new Range('\u1F5D', '\u1F5D'));
            this.ranges.Add(new Range('\u1F5F', '\u1F7D'));
            this.ranges.Add(new Range('\u1F80', '\u1FB4'));

            this.ranges.Add(new Range('\u1FB6', '\u1FBC'));
            this.ranges.Add(new Range('\u1FBE', '\u1FBE'));
            this.ranges.Add(new Range('\u1FC2', '\u1FC4'));
            this.ranges.Add(new Range('\u1FC6', '\u1FCC'));
            this.ranges.Add(new Range('\u1FD0', '\u1FD3'));

            this.ranges.Add(new Range('\u1FD6', '\u1FDB'));
            this.ranges.Add(new Range('\u1FE0', '\u1FEC'));
            this.ranges.Add(new Range('\u1FF2', '\u1FF4'));
            this.ranges.Add(new Range('\u1FF6', '\u1FFC'));
            this.ranges.Add(new Range('\u2126', '\u2126'));

            this.ranges.Add(new Range('\u212A', '\u212B'));
            this.ranges.Add(new Range('\u212E', '\u212E'));
            this.ranges.Add(new Range('\u2180', '\u2182'));
            this.ranges.Add(new Range('\u3041', '\u3094'));
            this.ranges.Add(new Range('\u30A1', '\u30FA'));

            this.ranges.Add(new Range('\u3105', '\u312C'));
            this.ranges.Add(new Range('\uAC00', '\uD7A3'));
            #endregion

            this.overallCount = this.GetOverallCount();
        }
    }
}
