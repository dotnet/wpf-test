using System;
using System.Drawing;
using System.Windows.Forms;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;

namespace ReflectTools.AutoPME
{
// System.ValueType AutoPME Test
//
public abstract class XValueType : XObject 
{
  public XValueType(String[] args) : base(args) { }

  protected override Type Class 
  {
    get { return typeof(ValueType); }
  }

  //========================================
  // Test Methods
  //========================================
  protected override ScenarioResult ToString(TParams p) 
  {
      return base.ToString(p);
  }

  protected override ScenarioResult Equals(TParams p, Object obj) 
  {
    return base.Equals(p, obj);
  }

  protected override ScenarioResult GetHashCode(TParams p) 
  {
      return base.GetHashCode(p);
  }
}
}