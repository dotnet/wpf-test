using System;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;

//
// System.Windows.UIElement AutoPME Test
//
public class XUIElement : XVisual {
    public XUIElement(String[] args) : base(args) { }

    UIElement GetUIElement(TParams p) {
        if ( p.target is UIElement )
            return (UIElement)p.target;
        else {
            p.log.WriteLine("target isn't type UIElement");
            return null;
        }
    }

    //========================================
    // Test Methods
    //========================================
    protected ScenarioResult get_PersistId(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_AllowDrop(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_AllowDrop(TParams p, Boolean value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_DesiredSize(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsMeasureValid(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsArrangeValid(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult InvalidateMeasure(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult InvalidateArrange(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult InvalidateVisual(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult Measure(TParams p, Size availableSize) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult Arrange(TParams p, Rect finalRect) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_RenderSize(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_RenderSize(TParams p, Size value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_RenderTransform(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_RenderTransform(TParams p, Transform value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_RenderTransformOrigin(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_RenderTransformOrigin(TParams p, Point value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult UpdateLayout(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult TranslatePoint(TParams p, Point point, UIElement relativeTo) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult InputHitTest(TParams p, Point point) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsMouseDirectlyOver(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsMouseOver(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsStylusOver(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsStylusCaptureWithin(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsKeyboardFocusWithin(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsMouseCaptured(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsMouseCaptureWithin(TParams p)
    {
        return new ScenarioResult(true);
    }

    protected ScenarioResult CaptureMouse(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ReleaseMouseCapture(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsStylusDirectlyOver(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsStylusCaptured(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult CaptureStylus(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ReleaseStylusCapture(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsKeyboardFocused(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult Focus(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult MoveFocus(TParams p, TraversalRequest request) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult PredictFocus(TParams p, FocusNavigationDirection direction) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsInputMethodEnabled(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Opacity(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Opacity(TParams p, Double value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_OpacityMask(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_OpacityMask(TParams p, Brush value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_BitmapEffect(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_BitmapEffect(TParams p, BitmapEffect value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_BitmapEffectInput(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_BitmapEffectInput(TParams p, BitmapEffectInput value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Visibility(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Visibility(TParams p, Visibility value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_ClipToBounds(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_ClipToBounds(TParams p, Boolean value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Clip(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Clip(TParams p, Geometry value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_SnapsToDevicePixels(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_SnapsToDevicePixels(TParams p, Boolean value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsFocused(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsEnabled(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_IsEnabled(TParams p, Boolean value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsHitTestVisible(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_IsHitTestVisible(TParams p, Boolean value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_IsVisible(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_Focusable(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult set_Focusable(TParams p, Boolean value) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ApplyAnimationClock(TParams p, DependencyProperty dp, AnimationClock clock) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ApplyAnimationClock(TParams p, DependencyProperty dp, AnimationClock clock, HandoffBehavior handoffBehavior) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult BeginAnimation(TParams p, DependencyProperty dp, AnimationTimeline animation) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult BeginAnimation(TParams p, DependencyProperty dp, AnimationTimeline animation, HandoffBehavior handoffBehavior) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_HasAnimatedProperties(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult GetAnimationBaseValue(TParams p, DependencyProperty dp) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_InputBindings(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ShouldSerializeInputBindings(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult get_CommandBindings(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult ShouldSerializeCommandBindings(TParams p) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult RaiseEvent(TParams p, RoutedEventArgs e) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult AddHandler(TParams p, RoutedEvent routedEvent, Delegate handler) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult AddHandler(TParams p, RoutedEvent routedEvent, Delegate handler, Boolean handledEventsToo) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult RemoveHandler(TParams p, RoutedEvent routedEvent, Delegate handler) {
        return new ScenarioResult(true);
    }

    protected ScenarioResult AddToEventRoute(TParams p, EventRoute route, RoutedEventArgs e) {
        return new ScenarioResult(true);
    }

}
