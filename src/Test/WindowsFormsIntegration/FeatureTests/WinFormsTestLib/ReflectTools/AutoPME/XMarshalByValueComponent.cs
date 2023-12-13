// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using WFCTestLib.Log;
using WFCTestLib.Util;

namespace ReflectTools.AutoPME {
    public abstract class XMarshalByValueComponent : XObject {
        public XMarshalByValueComponent(String[] args) : base(args) { }

        /* BETA2: DesignMode no longer externally accessible
        protected virtual ScenarioResult get_DesignMode(TParams p) {
            MarshalByValueComponent comp = (MarshalByValueComponent)p.target;

            if (comp == null) {
                p.log.WriteLine("returned MarshalByValueComponent is null");
                return ScenarioResult.Fail;
            }

            bool bDesignMode = comp.DesignMode;

            p.log.WriteLine("DesignMode: " + bDesignMode.ToString());
            return ScenarioResult.Pass;
        }

        protected virtual ScenarioResult GetServiceObject(TParams p, Type t) {
            return ScenarioResult.Pass;
        }
*/
        protected virtual ScenarioResult set_Site(TParams p, ISite value) {
            return get_Site(p);
        }
        
        protected virtual ScenarioResult get_Site(TParams p) {
            return ScenarioResult.Pass;
        }

        protected override ScenarioResult ToString(TParams p) {
            return base.ToString(p);
        }

        protected virtual ScenarioResult Dispose(TParams p) {
            return ScenarioResult.Pass;
        }
        protected virtual ScenarioResult get_Container(TParams p) {
            return ScenarioResult.Pass;
        }
    }
}
