// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.CompilerServices;


// We are making sure that you cann perform any assert or elevation of privilage.

[assembly: SecurityCritical(SecurityCriticalScope.Everything)]
