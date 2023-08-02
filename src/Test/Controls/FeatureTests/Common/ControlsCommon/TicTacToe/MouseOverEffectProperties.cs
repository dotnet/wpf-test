using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Test.Controls
{
    public class MouseOverEffectProperties
    {
        public static DependencyProperty MouseOverEffectProperty;

        static MouseOverEffectProperties()
        {
            OuterGlowBitmapEffect defaultEffect = new OuterGlowBitmapEffect();
            defaultEffect.GlowColor = Colors.Yellow;
            defaultEffect.GlowSize = 10;

            MouseOverEffectProperty = DependencyProperty.RegisterAttached(
                "MouseOverEffect",
                typeof(BitmapEffect),
                typeof(MouseOverEffectProperties),
                new PropertyMetadata(defaultEffect));
        }

        public static BitmapEffect GetMouseOverEffect(DependencyObject target)
        {
            return (BitmapEffect)target.GetValue(MouseOverEffectProperty);
        }

        public static void SetMouseOverEffect(DependencyObject target, BitmapEffect value)
        {
            target.SetValue(MouseOverEffectProperty, value);
        }
    }
}
