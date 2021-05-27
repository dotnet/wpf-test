// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Microsoft.Test.Animation
{

//  AnimationValidator returns an expected value for an animation (or set of composed animations)
//
        /// <summary>
        /// Animation Validator 
        /// </summary>
    public class AnimationValidator
    {
    
        /// <summary>
        /// The different types of animation supported by the validator
        /// </summary>
        private enum animationTypeEnum
        {
            from,
            to,
            BooleanDiscrete,
            CharDiscrete,
            MatrixDiscrete,
            StringDiscrete,
            KeyFramed,
            by,
            fromTo,
            fromBy,
            invalid
        }

        /// <summary>
        /// Results returned by the Validator
        /// </summary>
        public struct VerifierResults
        {
            /// <summary>
            /// result information
            /// </summary>
            public string resultInformation;
            /// <summary>
            /// actual result
            /// </summary>
            public bool result;
        }

        private struct AniProps
        {
            public object from;
            public object to;
            public object by;
            public object keyFrames;
            public bool isAdditive;
            public bool isCumulative;
            public bool isActive;
            public bool isAutoReverse;
            public int begin;
            public int duration;
            public double totalDuration;
            public double totalAutoRevDuration;
            public double repeatCount;
            public string fill;
            public animationTypeEnum animType;
        }

        // constructors

        /// <summary>
        /// Empty constructor
        /// </summary>
        public AnimationValidator() { }


//     Single AnimationClock entryPoint
 
        /// <summary>
        /// single animationClock entrypoint
        /// </summary>
        public object Verify(AnimationClock animationClock,object myBaseValue, double curTime)
        {
            object[] animationList = new object[1];
            animationList[0] = animationClock;
        
            return Verify(animationList,myBaseValue,curTime);
        }

// main private Verify entryPoint

        private object Verify(object[] animations, object myBaseValue, double curTime)
        {
            if (myBaseValue == null)
            {
                myBaseValue = ZeroObject((animations[0] as AnimationClock).Timeline.TargetPropertyType);
            }

            object finalValue = null;
            object tempValue = myBaseValue;
            string typeAsString = null;
            for (int i = 0; i < animations.Length; i++)
            { 
                object animation = animations[i];
                AniProps objProps = fillProperties((AnimationClock)animation);
                
                typeAsString = objProps.animType.ToString();
                bool firstPass = true;
                        
                if (objProps.isActive)              
                { 
                    // if a first calculation, use the zero value               
                    // or, if the element is not composing, we need to throw out previously calculated values
                    if ((!firstPass) && (!objProps.isAdditive))
                    {
                        tempValue = ZeroObject((animations[0] as AnimationClock).Timeline.TargetPropertyType);
                        firstPass = false;
                    }

                    // get a temporary calculation value
                    tempValue = CalculateAnimation(animation,myBaseValue,objProps,curTime,tempValue);

                    // else, if To,FromTo,FromBy,FromToDiscrete,FromByDiscrete,ToDiscrete and composing the final value is the calculated value

                    if ((typeAsString == "BooleanDiscrete") || (typeAsString == "StringDiscrete") || (typeAsString == "CharDiscrete") || (typeAsString == "MatrixDiscrete") || (typeAsString == "by") || (typeAsString == "to") || (typeAsString == "fromTo") || (typeAsString == "fromBy"))
                    {
                        finalValue = tempValue;
                    }
                    // for all other types, return the summation of previously calculated values and the temporary calculation
                    else
                    {
                        finalValue = SumObjects(finalValue,tempValue);
                    }
                }
                else
                {
                    finalValue = myBaseValue;
                }
            }
            return RoundObject(finalValue,3);
        }


        private object CalculateAnimation(object curAnimation, object baseValue, AniProps objProps, double curTime, object tempValue)
        {
            object returnCalc = null;
            object holder = null;
            float  numberOfRepeats = 0;

            object currentReturnValue = baseValue;
            double currentClosestTime = 0;

// *** ASSUMPTION ** for interactive and static animations, we will calculate the begin time based on the object's 
// current progress this allows for the handling of interactive methods. We are no longer calcuating progress directly 
// off of the animations properties (as this is tested by the timing code)

            objProps.begin = (int)(curTime - ((Clock)curAnimation).CurrentProgress * (double)(((Clock)curAnimation).Timeline).Duration.TimeSpan.TotalMilliseconds);


            float progress = (float)((Clock)curAnimation).CurrentProgress;

            // if the animation repeats, but does not accumulate, we need to ensure that the
            // progress is <=1 for the calculations. If the animation is active, but has
            // a TimeFill set to hold, we need to calculate the expected hold
            // value. This value is affected by Accumulation. So if it is accumulating, 
            // our progress used to calculate should be the repeatCount (eg 2) if the animation
            // is beyond its expected duration. If the animation has not reached it's expected
            // total duration, the progress remains what was calculated above. For non cumulative,
            // assign the progress to 1 (which would be 1 iteration of the animation)


            if (((objProps.fill == "HoldEnd")) && (progress >= 1) && (objProps.totalDuration <= curTime) && (!objProps.isAutoReverse))
            {    
                if ((!objProps.isCumulative))
                {
                    progress = 1f;
                }
                else if (objProps.totalDuration <= curTime)
                {
                    progress = (float)objProps.repeatCount;
                }
            }
            else 
            {
                while (progress >= 1)
                { 
                    progress = progress - 1;
                    numberOfRepeats ++;
                }
            }


           // autoreverse - if we are outside our active duration, we are reversing


            if ((objProps.isAutoReverse) && (curTime <= objProps.totalAutoRevDuration))
            {
                while (progress >= 1)
                { 
                    progress = progress - 1;
                }

                double autoReverseFactor = (curTime - objProps.begin);
                autoReverseFactor = autoReverseFactor / objProps.duration;
                autoReverseFactor = Math.Floor(autoReverseFactor) % 2;

                if (autoReverseFactor == 1)
                {
                    progress = 1 - progress;
                }
            }
            else if ((objProps.isAutoReverse) && (curTime > objProps.totalAutoRevDuration))
            {
                progress = 0;
            }

            switch(objProps.animType)
            {
                case animationTypeEnum.fromTo:
                    // FromTo = (from +((to - from) * progress)) ;

                    if (objProps.isCumulative) 
                    {
                        holder = MultObjects(objProps.to,numberOfRepeats);
                        holder = SumObjects(tempValue,holder);
                    }
                    object from = SumObjects(objProps.from,holder);
                    object to = SumObjects(objProps.to,holder);

                    returnCalc = DiffObjects(to,from);
                    returnCalc = MultObjects(returnCalc,progress);
                    returnCalc = SumObjects(from,returnCalc);
                    break;

                case animationTypeEnum.fromBy:
                    // FromBy = by * progress + from;

                    from = SumObjects(objProps.from,tempValue);

                    if (objProps.isCumulative)
                    {
                        progress += numberOfRepeats;
                    }
                    returnCalc = MultObjects(objProps.by,progress);
                    returnCalc = SumObjects(returnCalc,from);

                    if (objProps.isCumulative) 
                    { 
                       holder = MultObjects(from,numberOfRepeats);             
                       returnCalc = SumObjects(returnCalc,holder); 
                    }
                    break;

                case animationTypeEnum.to:
                    //To = to * progress;

                    if (objProps.isCumulative)
                    {
                        progress += numberOfRepeats;
                    }

                    returnCalc = DiffObjects(objProps.to,tempValue);
                    returnCalc = MultObjects(returnCalc,progress);
                    returnCalc = SumObjects(returnCalc,tempValue);

                    // todo: assess cumulative to logic
                    break;

                case animationTypeEnum.by:
                    //By = by * progress;

                    if (objProps.isCumulative)
                    {
                        progress += numberOfRepeats;
                    }
                    returnCalc = MultObjects(objProps.by,progress);
                    returnCalc = SumObjects(returnCalc,tempValue);
                    break;
                
                case animationTypeEnum.StringDiscrete:
                    for (int i = 0; i < ((StringKeyFrameCollection)objProps.keyFrames).Count; i++)
                    {
                        StringKeyFrame curKeyFrame = ((StringKeyFrameCollection)objProps.keyFrames)[i];
                        double keyTimeTime = getAbsoluteKeyTimeTime(curKeyFrame.KeyTime,objProps.begin,objProps.duration);

                        if ((keyTimeTime < curTime) && (keyTimeTime > currentClosestTime))
                        {
                            currentReturnValue = curKeyFrame.Value; 
                            currentClosestTime = keyTimeTime;
                        }
                    }
                    returnCalc = currentReturnValue;                        
                    break;

                case animationTypeEnum.BooleanDiscrete:
                    for (int i = 0; i < ((BooleanKeyFrameCollection)objProps.keyFrames).Count; i++)
                    {
                        BooleanKeyFrame curKeyFrame = ((BooleanKeyFrameCollection)objProps.keyFrames)[i];
                        double keyTimeTime = getAbsoluteKeyTimeTime(curKeyFrame.KeyTime,objProps.begin,objProps.duration);

                        if ((keyTimeTime < curTime) && (keyTimeTime > currentClosestTime))
                        {
                            currentReturnValue = curKeyFrame.Value; 
                            currentClosestTime = keyTimeTime;
                        }
                    }
                    returnCalc = currentReturnValue;                        
                    break;
                    
                case animationTypeEnum.CharDiscrete:
                    for (int i = 0; i < ((CharKeyFrameCollection)objProps.keyFrames).Count; i++)
                    {
                        CharKeyFrame curKeyFrame = ((CharKeyFrameCollection)objProps.keyFrames)[i];
                        double keyTimeTime = getAbsoluteKeyTimeTime(curKeyFrame.KeyTime,objProps.begin,objProps.duration);

                        if ((keyTimeTime < curTime) && (keyTimeTime > currentClosestTime))
                        {
                            currentReturnValue = curKeyFrame.Value; 
                            currentClosestTime = keyTimeTime;
                        }
                    }
                    returnCalc = currentReturnValue;                        
                    break;
                    
                case animationTypeEnum.MatrixDiscrete:
                    for (int i = 0; i < ((MatrixKeyFrameCollection)objProps.keyFrames).Count; i++)
                    {
                        MatrixKeyFrame curKeyFrame = ((MatrixKeyFrameCollection)objProps.keyFrames)[i];
                        double keyTimeTime = getAbsoluteKeyTimeTime(curKeyFrame.KeyTime,objProps.begin,objProps.duration);

                        if ((keyTimeTime < curTime) && (keyTimeTime > currentClosestTime))
                        {
                            currentReturnValue = curKeyFrame.Value; 
                            currentClosestTime = keyTimeTime;
                        }
                    }
                    returnCalc = currentReturnValue;                        
                    break;

            }
            return returnCalc;
        }

        private double getAbsoluteKeyTimeTime(KeyTime keyTime, double beginTime, double duration)
        {
            if (keyTime.Type == KeyTimeType.TimeSpan)
            {
                return (double)keyTime.TimeSpan.TotalMilliseconds + beginTime;
            }
            else if (keyTime.Type == KeyTimeType.Percent)
            {
                return keyTime.Percent * duration + beginTime;
            }
            return 0;
        }

        private AniProps fillProperties(AnimationClock animClock)
        {

            AniProps returnProps = new AniProps();
            returnProps.animType = animationTypeEnum.invalid;
            bool isKeyFramed = false;   

            // no reflection in SEE, assign object based on type
            switch (animClock.Timeline.GetType().ToString())
            {
                case "System.Windows.Media.Animation.ColorAnimation":
                    if (((ColorAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Color)(((ColorAnimation)animClock.Timeline).From); }
                    if (((ColorAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Color)(((ColorAnimation)animClock.Timeline).To); }
                    if (((ColorAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Color)(((ColorAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((ColorAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((ColorAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.DoubleAnimation":
                    if (((DoubleAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (double)(((DoubleAnimation)animClock.Timeline).From); }
                    if (((DoubleAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (double)(((DoubleAnimation)animClock.Timeline).To); }
                    if (((DoubleAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (double)(((DoubleAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((DoubleAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((DoubleAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.PointAnimation":
                    if (((PointAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Point)(((PointAnimation)animClock.Timeline).From); }
                    if (((PointAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Point)(((PointAnimation)animClock.Timeline).To); }
                    if (((PointAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Point)(((PointAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((PointAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((PointAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.RectAnimation":
                    if (((RectAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Rect)(((RectAnimation)animClock.Timeline).From); }
                    if (((RectAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Rect)(((RectAnimation)animClock.Timeline).To); }
                    if (((RectAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Rect)(((RectAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((RectAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((RectAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.SizeAnimation":
                    if (((SizeAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Size)(((SizeAnimation)animClock.Timeline).From); }
                    if (((SizeAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Size)(((SizeAnimation)animClock.Timeline).To); }
                    if (((SizeAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Size)(((SizeAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((SizeAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((SizeAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.VectorAnimation":
                    if (((VectorAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Vector)(((VectorAnimation)animClock.Timeline).From); }
                    if (((VectorAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Vector)(((VectorAnimation)animClock.Timeline).To); }
                    if (((VectorAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Vector)(((VectorAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((VectorAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((VectorAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.DecimalAnimation":
                    if (((DecimalAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Decimal)(((DecimalAnimation)animClock.Timeline).From); }
                    if (((DecimalAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Decimal)(((DecimalAnimation)animClock.Timeline).To); }
                    if (((DecimalAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Decimal)(((DecimalAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((DecimalAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((DecimalAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.ByteAnimation":
                    if (((ByteAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Byte)(((ByteAnimation)animClock.Timeline).From); }
                    if (((ByteAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Byte)(((ByteAnimation)animClock.Timeline).To); }
                    if (((ByteAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Byte)(((ByteAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((ByteAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((ByteAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.Int16Animation":
                    if (((Int16Animation)animClock.Timeline).From.HasValue) { returnProps.from = (Int16)(((Int16Animation)animClock.Timeline).From); }
                    if (((Int16Animation)animClock.Timeline).To.HasValue) { returnProps.to = (Int16)(((Int16Animation)animClock.Timeline).To); }
                    if (((Int16Animation)animClock.Timeline).By.HasValue) { returnProps.by = (Int16)(((Int16Animation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((Int16Animation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((Int16Animation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.Int32Animation":
                    if (((Int32Animation)animClock.Timeline).From.HasValue) { returnProps.from = (Int32)(((Int32Animation)animClock.Timeline).From); }
                    if (((Int32Animation)animClock.Timeline).To.HasValue) { returnProps.to = (Int32)(((Int32Animation)animClock.Timeline).To); }
                    if (((Int32Animation)animClock.Timeline).By.HasValue) { returnProps.by = (Int32)(((Int32Animation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((Int32Animation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((Int32Animation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.Int64Animation":
                    if (((Int64Animation)animClock.Timeline).From.HasValue) { returnProps.from = (Int64)(((Int64Animation)animClock.Timeline).From); }
                    if (((Int64Animation)animClock.Timeline).To.HasValue) { returnProps.to = (Int64)(((Int64Animation)animClock.Timeline).To); }
                    if (((Int64Animation)animClock.Timeline).By.HasValue) { returnProps.by = (Int64)(((Int64Animation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((Int64Animation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((Int64Animation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.Point3DAnimation":
                    if (((Point3DAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Point3D)(((Point3DAnimation)animClock.Timeline).From); }
                    if (((Point3DAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Point3D)(((Point3DAnimation)animClock.Timeline).To); }
                    if (((Point3DAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Point3D)(((Point3DAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((Point3DAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((Point3DAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.SingleAnimation":
                    if (((SingleAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Single)(((SingleAnimation)animClock.Timeline).From); }
                    if (((SingleAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Single)(((SingleAnimation)animClock.Timeline).To); }
                    if (((SingleAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Single)(((SingleAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((SingleAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((SingleAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.Vector3DAnimation":
                    if (((Vector3DAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Vector3D)(((Vector3DAnimation)animClock.Timeline).From); }
                    if (((Vector3DAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Vector3D)(((Vector3DAnimation)animClock.Timeline).To); }
                    if (((Vector3DAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Vector3D)(((Vector3DAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((Vector3DAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((Vector3DAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.ThicknessAnimation":
                    if (((ThicknessAnimation)animClock.Timeline).From.HasValue) { returnProps.from = (Thickness)(((ThicknessAnimation)animClock.Timeline).From); }
                    if (((ThicknessAnimation)animClock.Timeline).To.HasValue) { returnProps.to = (Thickness)(((ThicknessAnimation)animClock.Timeline).To); }
                    if (((ThicknessAnimation)animClock.Timeline).By.HasValue) { returnProps.by = (Thickness)(((ThicknessAnimation)animClock.Timeline).By); }
                    returnProps.isAdditive = (bool)((ThicknessAnimation)animClock.Timeline).IsAdditive;
                    returnProps.isCumulative = (bool)((ThicknessAnimation)animClock.Timeline).IsCumulative;
                    break;
                case "System.Windows.Media.Animation.StringAnimationUsingKeyFrames":
                    returnProps.keyFrames = ((StringAnimationUsingKeyFrames)animClock.Timeline).KeyFrames;
                    returnProps.animType = animationTypeEnum.StringDiscrete;
                    break;
                case "System.Windows.Media.Animation.BooleanAnimationUsingKeyFrames":
                    returnProps.keyFrames = ((BooleanAnimationUsingKeyFrames)animClock.Timeline).KeyFrames;
                    returnProps.animType = animationTypeEnum.BooleanDiscrete;
                    break;
                case "System.Windows.Media.Animation.CharAnimationUsingKeyFrames":
                    returnProps.keyFrames = ((CharAnimationUsingKeyFrames)animClock.Timeline).KeyFrames;
                    returnProps.animType = animationTypeEnum.CharDiscrete;
                    break;
                case "System.Windows.Media.Animation.MatrixAnimationUsingKeyFrames":
                    returnProps.keyFrames = ((MatrixAnimationUsingKeyFrames)animClock.Timeline).KeyFrames;
                    returnProps.animType = animationTypeEnum.MatrixDiscrete;
                    break;
                default:
                    return returnProps;
            }

            returnProps.isActive = (animClock.CurrentState == ClockState.Active) || (animClock.CurrentState == ClockState.Filling);


            returnProps.begin = (int)((TimeSpan)((Timeline)animClock.Timeline).BeginTime.Value).TotalMilliseconds;
            returnProps.duration = (int) ((Timeline)animClock.Timeline).Duration.TimeSpan.TotalMilliseconds;
            returnProps.repeatCount = (double) ((RepeatBehavior)((Timeline)animClock.Timeline).RepeatBehavior).Count;
            returnProps.fill = (string)((Timeline)animClock.Timeline).FillBehavior.ToString();
            returnProps.isAutoReverse = (bool)((Timeline)animClock.Timeline).AutoReverse;

            if (isKeyFramed)
            {
                returnProps.animType = animationTypeEnum.KeyFramed;
            }
            else if (returnProps.animType == animationTypeEnum.invalid)
            {
                returnProps.animType = GetAnimationType(returnProps);
            }

            if (returnProps.repeatCount == 0)
            {
                returnProps.totalDuration = returnProps.duration + returnProps.begin;
            }
            else
            {
                returnProps.totalDuration = returnProps.duration * returnProps.repeatCount + returnProps.begin;
            }

            if (returnProps.isAutoReverse)               
            { 
                if (returnProps.repeatCount == 0)
                {
                    returnProps.totalAutoRevDuration = returnProps.totalDuration + returnProps.duration;
                }
                else
                {
                    returnProps.totalAutoRevDuration = returnProps.totalDuration + returnProps.duration * returnProps.repeatCount;
                }
            }


            return returnProps;
        }


        private animationTypeEnum GetAnimationType(AniProps props)
        {
            animationTypeEnum returnResult;

            if ((props.from != null) && (props.to != null)) { returnResult = animationTypeEnum.fromTo; }
            else if ((props.from != null) && (props.by != null)) { returnResult = animationTypeEnum.fromBy; }
            else if (props.to != null) { returnResult = animationTypeEnum.to; }
            else if (props.by != null) { returnResult = animationTypeEnum.by; }
            else { returnResult = animationTypeEnum.invalid; }

            /*   key spline logic to be worked out
            else if ((props.from != null) && (props.to != null) && (props.interpMethod.Equals("Spline")) && (props.keytimes != null) && (props.keysplines != null)) { returnResult = animationTypeEnum.fromToSpline; }
            else if ((props.keyvalues != null) && (keyvaluenum == 1)) {returnResult = animationTypeEnum.OneKeyValue;}
            else if ((props.keyvalues != null) && (props.keytimes == null) && ((props.interpMethod.Equals("Linear")) || (props.interpMethod.Equals("Paced")) || (props.interpMethod.Equals("Discrete")))) { returnResult = animationTypeEnum.keyValues; }
            else if ((props.keyvalues != null) && (props.keytimes != null) && ((props.interpMethod.Equals("Linear")) || (props.interpMethod.Equals("Paced")) || (props.interpMethod.Equals("Discrete")))) { returnResult = animationTypeEnum.keyValuesKeytimes; }
            else if ((props.keyvalues != null) && (props.keytimes == null) && (props.interpMethod.Equals("Discrete"))) { returnResult = animationTypeEnum.keyValuesDiscrete; }
            else if ((props.keyvalues != null) && (props.keytimes != null) && (props.interpMethod.Equals("Discrete"))) { returnResult = animationTypeEnum.keyValuesKeytimesDiscrete; }
            else if ((props.keyvalues != null) && (props.interpMethod.Equals("Spline")) && (props.keytimes != null) && (props.keysplines != null)) { returnResult = animationTypeEnum.keyValuesSpline; }
            */
            return returnResult;
        }
        
// object animation calculators

        private object ZeroObject(Type currValueType)
        {
            switch (currValueType.ToString())
            {
                case "System.Double":
                    return 0d;
                case "System.Windows.Media.Color":
                    return Colors.Black;
                case "System.Windows.Point":
                    return new Point(0,0);
                case "System.Windows.Rect":
                    return new Rect(0,0,0,0);
                case "System.Windows.Size":
                    return new Size(0,0);
                case "System.Windows.Vectorn":
                    return new Vector(0,0);
                case "System.Byte":
                    return (Byte)0;
                case "System.Decimal":
                    return 0M;
                case "System.Int16":
                    return (Int16)0;
                case "System.Int32":
                    return (Int32)0;
                case "System.Int64":
                    return (Int64)0;
                case "System.Windows.Media.Media3D.Point3D":
                    return new Point3D(0,0,0);
                case "System.Windows.Media.Media3D.Quaternion":
                    return new Quaternion(0,0,0,0);
                case "System.Windows.Media.Media3D.Rect3D":
                    return new Rect3D(0,0,0,0,0,0);
                case "System.Single":
                    return (Single)0;
                case "System.Windows.Media.Media3D.Size3D":
                    return new Size3D(0,0,0);
                case "System.Windows.Media.Media3D.Vector3D":
                    return new Vector3D(0,0,0);
                case "System.Windows.Thickness":
                    return new Thickness(0,0,0,0);
                default:
                    return null;
            }
        }

        private object SumObjects(object operand1,object operand2)
        {
            if (operand1 == null)
            {
                return operand2;
            }
            else if (operand2 == null)
            {
                return operand1;
            }  

            switch (operand1.GetType().ToString())
            {
                case "System.Double":
                    return Convert.ToDouble(operand1) + Convert.ToDouble(operand2);
                case "System.Windows.Media.Color":
                    Color tempC = Color.Add((Color)operand1,(Color)operand2);
                    if (((Color)operand1).A == ((Color)operand2).A) { tempC.A = ((Color)operand1).A; }
                    return tempC;
                case "System.Windows.Point":
                    return new Point(((Point)operand1).X + ((Point)operand2).X,((Point)operand1).Y + ((Point)operand2).Y);
                case "System.Windows.Rect":
                    return new Rect(((Rect)operand1).X + ((Rect)operand2).X,((Rect)operand1).Y + ((Rect)operand2).Y,((Rect)operand1).Width + ((Rect)operand2).Width,((Rect)operand1).Height + ((Rect)operand2).Height);
                case "System.Windows.Size":
                    return new Size(((Size)operand1).Width + ((Size)operand2).Width,((Size)operand1).Height + ((Size)operand2).Height);
                case "System.Windows.Vector":
                    return new Vector(((Vector)operand1).X + ((Vector)operand2).X,((Vector)operand1).Y + ((Vector)operand2).Y);
                //case "System.Windows.Length":
                    //  return new Length( ((Length)operand1).Value + ((Length)operand2).Value, ((Length)operand1).UnitType);
                case "System.Byte":
                    return (Byte)((Byte)operand1 + (Byte)operand2);
                case "System.Decimal":
                    return (Decimal)operand1 + (Decimal)operand2;
                case "System.Int16":
                    return (Int16)((Int16)operand1 + (Int16)operand2);
                case "System.Int32":
                    return (Int32)((Int32)operand1 + (Int32)operand2);
                case "System.Int64":
                    return (Int64)((Int64)operand1 + (Int64)operand2);
                case "System.Windows.Media.Media3D.Point3D":
                    return new Point3D(((Point3D)operand1).X + ((Point3D)operand2).X,((Point3D)operand1).Y + ((Point3D)operand2).Y,((Point3D)operand1).Z + ((Point3D)operand2).Z);
                case "System.Windows.Media.Media3D.Quaternion":
                    return Quaternion.Add( (Quaternion)operand1, (Quaternion)operand2 );
                case "System.Windows.Media.Media3D.Rect3D":
                    return new Rect3D(((Rect3D)operand1).X + ((Rect3D)operand2).X,((Rect3D)operand1).Y + ((Rect3D)operand2).Y,((Rect3D)operand1).Z + ((Rect3D)operand2).Z, ((Rect3D)operand1).SizeX + ((Rect3D)operand2).SizeX, ((Rect3D)operand1).SizeY + ((Rect3D)operand2).SizeY, ((Rect3D)operand1).SizeZ + ((Rect3D)operand2).SizeZ );
                case "System.Single":
                    return Convert.ToSingle(operand1) + Convert.ToSingle(operand2);
                case "System.Windows.Media.Media3D.Size3D":
                    return new Size3D(((Size3D)operand1).X + ((Size3D)operand2).X,((Size3D)operand1).Y + ((Size3D)operand2).Y,((Size3D)operand1).Z + ((Size3D)operand2).Z);
                case "System.Windows.Media.Media3D.Vector3D":
                    return new Vector3D(((Vector3D)operand1).X + ((Vector3D)operand2).X,((Vector3D)operand1).Y + ((Vector3D)operand2).Y,((Vector3D)operand1).Z + ((Vector3D)operand2).Z);
                //case "System.Windows.Thickness":
                    //  return new Thickness( new Length(((Length)((Thickness)operand1).Left).Value + ((Length)((Thickness)operand2).Left).Value),
                    //                        new Length(((Length)((Thickness)operand1).Top).Value + ((Length)((Thickness)operand2).Top).Value),
                    //                        new Length(((Length)((Thickness)operand1).Right).Value + ((Length)((Thickness)operand2).Right).Value),
                    //                        new Length(((Length)((Thickness)operand1).Bottom).Value + ((Length)((Thickness)operand2).Bottom).Value) );
                case "System.Windows.Thickness":
                    return new Thickness((((Thickness)operand1).Left)+ (((Thickness)operand2).Left),
                    (((Thickness)operand1).Top) + (((Thickness)operand2).Top),
                    (((Thickness)operand1).Right) + (((Thickness)operand2).Right),
                    (((Thickness)operand1).Bottom) + (((Thickness)operand2).Bottom));
                default:
                    return null;
            }
        }

        private object DiffObjects(object operand1,object operand2)
        {

            if (operand1 == null)
            {
                return operand2;
            }
            else if (operand2 == null)
            {
                return operand1;
            }  

            switch (operand1.GetType().ToString())
            {
                case "System.Double":
                    return Convert.ToDouble(operand1) - Convert.ToDouble(operand2);
                case "System.Windows.Media.Color":
                    Color tempC = Color.Subtract((Color)operand1,(Color)operand2);
                if (((Color)operand1).A == ((Color)operand2).A) { tempC.A = ((Color)operand1).A; }
                    return tempC;
                case "System.Windows.Point":
                    return new Point(((Point)operand1).X - ((Point)operand2).X,((Point)operand1).Y - ((Point)operand2).Y);
                case "System.Windows.Rect":
                    return new Rect(((Rect)operand1).X - ((Rect)operand2).X,((Rect)operand1).Y - ((Rect)operand2).Y,((Rect)operand1).Width - ((Rect)operand2).Width,((Rect)operand1).Height - ((Rect)operand2).Height);
                case "System.Windows.Size":
                    return new Size(((Size)operand1).Width - ((Size)operand2).Width,((Size)operand1).Height - ((Size)operand2).Height);
                case "System.Windows.Vector":
                    return new Vector(((Vector)operand1).X - ((Vector)operand2).X,((Vector)operand1).Y - ((Vector)operand2).Y);
                case "System.Byte":
                    return (Byte)((Byte)operand1 - (Byte)operand2);
                case "System.Decimal":
                    return (Decimal)operand1 - (Decimal)operand2;
                case "System.Int16":
                    return (Int16)((Int16)operand1 - (Int16)operand2);
                case "System.Int32":
                    return (Int32)((Int32)operand1 - (Int32)operand2);
                case "System.Int64":
                    return (Int64)((Int64)operand1 - (Int64)operand2);
                case "System.Windows.Media.Media3D.Point3D":
                    return new Point3D(((Point3D)operand1).X - ((Point3D)operand2).X,((Point3D)operand1).Y - ((Point3D)operand2).Y,((Point3D)operand1).Z - ((Point3D)operand2).Z);
                case "System.Windows.Media.Media3D.Quaternion":
                    return Quaternion.Subtract( (Quaternion)operand1, (Quaternion)operand2 );
                case "System.Windows.Media.Media3D.Rect3D":
                    return new Rect3D(((Rect3D)operand1).X - ((Rect3D)operand2).X,((Rect3D)operand1).Y - ((Rect3D)operand2).Y,((Rect3D)operand1).Z - ((Rect3D)operand2).Z, ((Rect3D)operand1).SizeX - ((Rect3D)operand2).SizeX, ((Rect3D)operand1).SizeY - ((Rect3D)operand2).SizeY, ((Rect3D)operand1).SizeZ - ((Rect3D)operand2).SizeZ );
                case "System.Single":
                    return Convert.ToSingle(operand1) - Convert.ToSingle(operand2);
                case "System.Windows.Media.Media3D.Size3D":
                    return new Size3D(((Size3D)operand1).X - ((Size3D)operand2).X,((Size3D)operand1).Y - ((Size3D)operand2).Y,((Size3D)operand1).Z - ((Size3D)operand2).Z);
                case "System.Windows.Media.Media3D.Vector3D":
                    return new Vector3D(((Vector3D)operand1).X - ((Vector3D)operand2).X,((Vector3D)operand1).Y - ((Vector3D)operand2).Y,((Vector3D)operand1).Z - ((Vector3D)operand2).Z);
                case "System.Windows.Thickness":
                    return new Thickness( (((Thickness)operand1).Left) - (((Thickness)operand2).Left),
                    (((Thickness)operand1).Top) - (((Thickness)operand2).Top),
                    (((Thickness)operand1).Right) - (((Thickness)operand2).Right),
                    (((Thickness)operand1).Bottom) - (((Thickness)operand2).Bottom));
                default:
                    return null;
            }
        }

        private object MultObjects(object operand1,float factor)
        {

            if (operand1 == null)
            {
                return null;
            }
            switch (operand1.GetType().ToString())
            {
                case "System.Double":
                    return Convert.ToDouble(operand1) * factor;
                case "System.Windows.Media.Color":
                    Color tempC = Color.Multiply((Color)operand1,factor);
                    tempC.A = ((Color)operand1).A; 
                    return tempC;
                case "System.Windows.Point":
                    return new Point(((Point)operand1).X * factor,((Point)operand1).Y * factor);
                case "System.Windows.Rect":
                    return new Rect(((Rect)operand1).X * factor,((Rect)operand1).Y * factor,((Rect)operand1).Width * factor,((Rect)operand1).Height * factor);
                case "System.Windows.Size":
                    return new Size(((Size)operand1).Width * factor,((Size)operand1).Height * factor);
                case "System.Windows.Vector":
                    return new Vector(((Vector)operand1).X * factor,((Vector)operand1).Y * factor);
                    //case "System.Windows.Length":
                    //  return new Length( ((Length)operand1).Value * factor, ((Length)operand1).UnitType);
                case "System.Byte":
                    return (Byte)Math.Floor((Convert.ToDouble(operand1) * (double)factor));
                case "System.Decimal":
                    return (Decimal)((Decimal)operand1 * (Decimal)factor);
                case "System.Int16":
                    return (Int16)Math.Floor((Convert.ToDouble(operand1) * (double)factor));
                case "System.Int32":
                    return (Int32)Math.Floor((Convert.ToDouble(operand1) * (double)factor));
                case "System.Int64":
                    return (Int64)Math.Floor((Convert.ToDouble(operand1) * (double)factor));
                case "System.Windows.Media.Media3D.Point3D":
                    return new Point3D(((Point3D)operand1).X * factor,((Point3D)operand1).Y * factor,((Point3D)operand1).Z * factor);
                case "System.Windows.Media.Media3D.Quaternion":
                    return new Quaternion( ((Quaternion)operand1).X * factor, ((Quaternion)operand1).Y * factor, ((Quaternion)operand1).Z * factor, ((Quaternion)operand1).W * factor);
                case "System.Windows.Media.Media3D.Rect3D":
                    return new Rect3D(((Rect3D)operand1).X * factor,((Rect3D)operand1).Y * factor,((Rect3D)operand1).Z * factor, ((Rect3D)operand1).SizeX * factor, ((Rect3D)operand1).SizeY * factor, ((Rect3D)operand1).SizeZ * factor );
                case "System.Single":
                    return Convert.ToSingle(operand1) * factor;
                case "System.Windows.Media.Media3D.Size3D":
                    return new Size3D(((Size3D)operand1).X * factor,((Size3D)operand1).Y * factor,((Size3D)operand1).Z * factor);
                case "System.Windows.Media.Media3D.Vector3D":
                    return new Vector3D(((Vector3D)operand1).X * factor,((Vector3D)operand1).Y * factor,((Vector3D)operand1).Z * factor);
                case "System.Windows.Thickness":
                    return new Thickness( (((Thickness)operand1).Left) * factor,
                    (((Thickness)operand1).Top) * factor,
                    (((Thickness)operand1).Right) * factor,
                    (((Thickness)operand1).Bottom) * factor );
                default:
                    return null;
            }
        }
    
        private object RoundObject(object operand1,int sig)
        {
            if (operand1 == null)
            {
                return null;
            }

            switch (operand1.GetType().ToString())
            {
                case "System.Double":
                    return Convert.ToDouble(Math.Round((double)operand1,sig));
                case "System.Windows.Media.Color":
                    return operand1;
                case "System.Windows.Point":
                    return new Point( Math.Round(((Point)operand1).X,sig) ,Math.Round(((Point)operand1).Y,sig));
                case "System.Windows.Rect":
                    return new Rect(Math.Round(((Rect)operand1).X,sig),Math.Round(((Rect)operand1).Y,sig),Math.Round(((Rect)operand1).Width,sig),Math.Round(((Rect)operand1).Height,sig));
                case "System.Windows.Size":
                    return new Size(Math.Round(((Size)operand1).Width,sig),Math.Round(((Size)operand1).Height,sig));
                case "System.Windows.Vector":
                    return new Vector(Math.Round(((Vector)operand1).X,sig),Math.Round(((Vector)operand1).Y,sig));
                case "System.Byte":
                    return (Byte)operand1;
                case "System.Decimal":
                    return (Decimal)Math.Round((Decimal)operand1,sig);
                case "System.Int16":
                    return (Int16)operand1;
                case "System.Int32":
                    return (Int32)operand1;
                case "System.Int64":
                    return (Int64)operand1;
                case "System.Windows.Media.Media3D.Point3D":
                    return new Point3D( Math.Round(((Point3D)operand1).X,sig) ,Math.Round(((Point3D)operand1).Y,sig),Math.Round(((Point3D)operand1).Z,sig));
                case "System.Windows.Media.Media3D.Quaternion":
                    return new Quaternion( Math.Round(((Quaternion)operand1).X,sig) ,Math.Round(((Quaternion)operand1).Y,sig),Math.Round(((Quaternion)operand1).Z,sig), Math.Round(((Quaternion)operand1).W,sig));
                case "System.Windows.Media.Media3D.Rect3D":
                    return new Rect3D( Math.Round(((Rect3D)operand1).X,sig) ,Math.Round(((Rect3D)operand1).Y,sig),Math.Round(((Rect3D)operand1).Z,sig), Math.Round(((Rect3D)operand1).SizeX,sig) ,Math.Round(((Rect3D)operand1).SizeY,sig),Math.Round(((Rect3D)operand1).SizeZ,sig));
                case "System.Single":
                    return Convert.ToSingle(Math.Round((Single)operand1,sig));
                case "System.Windows.Media.Media3D.Size3D":
                    return new Size3D( Math.Round(((Size3D)operand1).X,sig) ,Math.Round(((Size3D)operand1).Y,sig),Math.Round(((Size3D)operand1).Z,sig));
                case "System.Windows.Media.Media3D.Vector3D":
                    return new Vector3D( Math.Round(((Vector3D)operand1).X,sig) ,Math.Round(((Vector3D)operand1).Y,sig),Math.Round(((Vector3D)operand1).Z,sig));
                    case "System.Windows.Thickness":
                return new Thickness( ( Math.Round((((Thickness)operand1).Left),sig) ),
                    ( Math.Round((((Thickness)operand1).Top),sig) ),
                    ( Math.Round((((Thickness)operand1).Right),sig) ),
                    ( Math.Round((((Thickness)operand1).Bottom),sig) ) );
                default:
                    return operand1;
            }
        }

        /// <summary>
        /// are the two values within the supplied tolerance
        /// </summary>
        public bool WithinTolerance(object operand1,object operand2,double tolerance)
        {
            switch (operand1.GetType().ToString())
            {
                case "System.Double":
                    double diff = Convert.ToDouble(operand1) - Convert.ToDouble(operand2);

                    if (!(Convert.ToDouble(operand1) == 0))
                    {
                        diff = diff/Convert.ToDouble(operand1);
                    }
                    if (Math.Abs(diff) <= tolerance) { return true; }
                    else { return false; }

                case "System.Windows.Media.Color":
                    double diffA = ((Color)operand1).A - ((Color)operand2).A;
                    double diffR = ((Color)operand1).R - ((Color)operand2).R;
                    double diffG = ((Color)operand1).G - ((Color)operand2).G;
                    double diffB = ((Color)operand1).B - ((Color)operand2).B;

                    if (!(((Color)operand1).A == 0)) { diffA = diffA / ((Color)operand1).A; }
                    if (!(((Color)operand1).R == 0)) { diffR = diffR / ((Color)operand1).R; }
                    if (!(((Color)operand1).G == 0)) { diffG = diffG / ((Color)operand1).G; }
                    if (!(((Color)operand1).B == 0)) { diffB = diffB / ((Color)operand1).B; }

                    if ( (Math.Abs(diffA) <= tolerance) && (Math.Abs(diffR) <= tolerance) && (Math.Abs(diffG) <= tolerance) && (Math.Abs(diffB) <= tolerance) ){ return true; }
                    else { return false; }

                case "System.Windows.Point":
                    double diffX = ((Point)operand1).X - ((Point)operand2).X;
                    double diffY = ((Point)operand1).Y - ((Point)operand2).Y;

                    if (!(((Point)operand1).X == 0)) { diffX = diffX / ((Point)operand1).X; }
                    if (!(((Point)operand1).Y == 0)) { diffY = diffY / ((Point)operand1).Y; }

                    if ( (Math.Abs(diffX) <= tolerance) && (Math.Abs(diffY) <= tolerance) ) { return true; }
                    else { return false; }

                case "System.Windows.Rect":
                    double diffHeight = ((Rect)operand1).Height - ((Rect)operand2).Height;
                    double diffWidth = ((Rect)operand1).Width - ((Rect)operand2).Width;
                    double diffX2 = ((Rect)operand1).X - ((Rect)operand2).X;
                    double diffY2 = ((Rect)operand1).Y - ((Rect)operand2).Y;

                    if (!(((Rect)operand1).Height == 0)) { diffHeight = diffHeight / ((Rect)operand1).Height; }
                    if (!(((Rect)operand1).Width == 0)) { diffWidth = diffWidth / ((Rect)operand1).Width; }
                    if (!(((Rect)operand1).X == 0)) { diffX2 = diffX2 / ((Rect)operand1).X; }
                    if (!(((Rect)operand1).Y == 0)) { diffY2 = diffY2 / ((Rect)operand1).Y; }

                    if ( (Math.Abs(diffHeight) <= tolerance) && (Math.Abs(diffWidth) <= tolerance) && (Math.Abs(diffX2) <= tolerance) && (Math.Abs(diffY2) <= tolerance) ) { return true; }
                    else { return false; }

                case "System.Windows.Size":
                    double diffHeight2 = ((Size)operand1).Height - ((Size)operand2).Height;
                    double diffWidth2 = ((Size)operand1).Width - ((Size)operand2).Width;

                    if (!(((Size)operand1).Height == 0)) { diffHeight2 = diffHeight2 / ((Size)operand1).Height; }
                    if (!(((Size)operand1).Width == 0)) { diffWidth2 = diffWidth2 / ((Size)operand1).Width; }

                    if ( (Math.Abs(diffHeight2) <= tolerance) && (Math.Abs(diffWidth2) <= tolerance) ) { return true; }
                    else { return false; }

                case "System.Windows.Vector":
                    double diffX3 = ((Vector)operand1).X - ((Vector)operand2).X;
                    double diffY3 = ((Vector)operand1).Y - ((Vector)operand2).Y;

                    if (!(((Vector)operand1).X == 0)) { diffX3 = diffX3 / ((Vector)operand1).X; }
                    if (!(((Vector)operand1).Y == 0)) { diffY3 = diffY3 / ((Vector)operand1).Y; }

                    if ( (Math.Abs(diffX3) <= tolerance) && (Math.Abs(diffY3) <= tolerance) ) { return true; }
                    else { return false; }
                    //case "System.Windows.Length":
                    //              double diff2 = ((Length)operand1).Value - ((Length)operand2).Value;

                    //              if (!(((Length)operand1).Value == 0)) { diff2 = diff2/((Length)operand1).Value; }
                    //              if (Math.Abs(diff2) <= tolerance) { return true; }
                    //              else { return false; }
                
                case "System.Byte":
                    Double diffB2 = (Double)((Byte)operand1 - (Byte)operand2);
                    if (!(Convert.ToByte(operand1) == 0)) { diffB2 = diffB2/Convert.ToByte(operand1); }
                    if (Math.Abs(diffB2) <= (Double)tolerance) { return true; }
                    else { return false; }
                
                case "System.Decimal":
                    Decimal diffD = Convert.ToDecimal(operand1) - Convert.ToDecimal(operand2);
                    if (!(Convert.ToDecimal(operand1) == 0)) { diffD = diffD/Convert.ToDecimal(operand1); }
                    if (Math.Abs(diffD) <= (Decimal)tolerance) { return true; }
                    else { return false; }
                
                case "System.Int16":
                    Double diffI = (Double)((Int16)operand1 - (Int16)operand2);
                    if (!(Convert.ToInt16(operand1) == 0)) { diffI = diffI/Convert.ToInt16(operand1); }
                    if (Math.Abs(diffI) <= (Double)tolerance) { return true; }
                    else { return false; }
                
                case "System.Int32":
                    Double diffI2 = (Double)((Int32)operand1 - (Int32)operand2);
                    if (!(Convert.ToInt32(operand1) == 0)) { diffI2 = diffI2/Convert.ToInt32(operand1); }
                    if (Math.Abs(diffI2) <= (Double)tolerance) { return true; }
                    else { return false; }
                
                case "System.Int64":
                    Double diffI3 = (Double)((Int64)operand1 - (Int64)operand2);
                    if (!(Convert.ToInt64(operand1) == 0)) { diffI3 = diffI3/Convert.ToInt64(operand1); }
                    if (Math.Abs(diffI3) <= (Double)tolerance) { return true; }
                    else { return false; }
                    case "System.Windows.Media.Media3D.Point3D":
                    double diffP31 = ((Point3D)operand1).X - ((Point3D)operand2).X;
                    double diffP32 = ((Point3D)operand1).Y - ((Point3D)operand2).Y;
                    double diffP33 = ((Point3D)operand1).Z - ((Point3D)operand2).Z;

                    if (!(((Point3D)operand1).X == 0)) { diffP31 = diffP31 / ((Point3D)operand1).X; }
                    if (!(((Point3D)operand1).Y == 0)) { diffP32 = diffP32 / ((Point3D)operand1).Y; }
                    if (!(((Point3D)operand1).Z == 0)) { diffP33 = diffP33 / ((Point3D)operand1).Z; }

                    if ( (Math.Abs(diffP31) <= tolerance) && (Math.Abs(diffP32) <= tolerance) && (Math.Abs(diffP33) <= tolerance) ) { return true; }
                    else { return false; }

                case "System.Windows.Media.Media3D.Quaternion":
                    double diffQ1 = ((Quaternion)operand1).X - ((Quaternion)operand2).X;
                    double diffQ2 = ((Quaternion)operand1).Y - ((Quaternion)operand2).Y;
                    double diffQ3 = ((Quaternion)operand1).Z - ((Quaternion)operand2).Z;
                    double diffQ4 = ((Quaternion)operand1).W - ((Quaternion)operand2).W;

                    if (!(((Quaternion)operand1).X == 0)) { diffQ1 = diffQ1 / ((Quaternion)operand1).X; }
                    if (!(((Quaternion)operand1).Y == 0)) { diffQ2 = diffQ2 / ((Quaternion)operand1).Y; }
                    if (!(((Quaternion)operand1).Z == 0)) { diffQ3 = diffQ3 / ((Quaternion)operand1).Z; }
                    if (!(((Quaternion)operand1).W == 0)) { diffQ4 = diffQ4 / ((Quaternion)operand1).W; }

                    if ( (Math.Abs(diffQ1) <= tolerance) && (Math.Abs(diffQ2) <= tolerance) && (Math.Abs(diffQ3) <= tolerance) && (Math.Abs(diffQ4) <= tolerance) ) { return true; }
                    else { return false; }
                    case "System.Windows.Media.Media3D.Rect3D":
                    double diffR31 = ((Rect3D)operand1).X - ((Rect3D)operand2).X;
                    double diffR32 = ((Rect3D)operand1).Y - ((Rect3D)operand2).Y;
                    double diffR33 = ((Rect3D)operand1).Z - ((Rect3D)operand2).Z;
                    double diffR34 = ((Rect3D)operand1).SizeX - ((Rect3D)operand2).SizeX;
                    double diffR35 = ((Rect3D)operand1).SizeY - ((Rect3D)operand2).SizeY;
                    double diffR36 = ((Rect3D)operand1).SizeZ - ((Rect3D)operand2).SizeZ;

                    if (!(((Rect3D)operand1).X == 0)) { diffR31 = diffR31 / ((Rect3D)operand1).X; }
                    if (!(((Rect3D)operand1).Y == 0)) { diffR32 = diffR32 / ((Rect3D)operand1).Y; }
                    if (!(((Rect3D)operand1).Z == 0)) { diffR33 = diffR33 / ((Rect3D)operand1).Z; }
                    if (!(((Rect3D)operand1).SizeX == 0)) { diffR34 = diffR34 / ((Rect3D)operand1).SizeX; }
                    if (!(((Rect3D)operand1).SizeY == 0)) { diffR35 = diffR35 / ((Rect3D)operand1).SizeY; }
                    if (!(((Rect3D)operand1).SizeZ == 0)) { diffR36 = diffR36 / ((Rect3D)operand1).SizeZ; }

                    if ( (Math.Abs(diffR31) <= tolerance) && (Math.Abs(diffR32) <= tolerance) && (Math.Abs(diffR33) <= tolerance) && (Math.Abs(diffR34) <= tolerance) && (Math.Abs(diffR35) <= tolerance) && (Math.Abs(diffR36) <= tolerance)) { return true; }
                    else { return false; }

                case "System.Single":
                    double diffS = Convert.ToSingle(operand1) - Convert.ToSingle(operand2);

                    if (!(Convert.ToSingle(operand1) == 0)) { diffS = diffS/Convert.ToSingle(operand1); }
                    if (Math.Abs(diffS) <= tolerance) { return true; }
                    else { return false; }
                    case "System.Windows.Media.Media3D.Size3D":
                    double diffS31 = ((Size3D)operand1).X - ((Size3D)operand2).X;
                    double diffS32 = ((Size3D)operand1).Y - ((Size3D)operand2).Y;
                    double diffS33 = ((Size3D)operand1).Z - ((Size3D)operand2).Z;

                    if (!(((Size3D)operand1).X == 0)) { diffS31 = diffS31 / ((Size3D)operand1).X; }
                    if (!(((Size3D)operand1).Y == 0)) { diffS32 = diffS32 / ((Size3D)operand1).Y; }
                    if (!(((Size3D)operand1).Z == 0)) { diffS33 = diffS33 / ((Size3D)operand1).Z; }

                    if ( (Math.Abs(diffS31) <= tolerance) && (Math.Abs(diffS32) <= tolerance) && (Math.Abs(diffS33) <= tolerance) ) { return true; }
                    else { return false; }

                case "System.Windows.Media.Media3D.Vector3D":
                    double diffV31 = ((Vector3D)operand1).X - ((Vector3D)operand2).X;
                    double diffV32 = ((Vector3D)operand1).Y - ((Vector3D)operand2).Y;
                    double diffV33 = ((Vector3D)operand1).Z - ((Vector3D)operand2).Z;

                    if (!(((Vector3D)operand1).X == 0)) { diffV31 = diffV31 / ((Vector3D)operand1).X; }
                    if (!(((Vector3D)operand1).Y == 0)) { diffV32 = diffV32 / ((Vector3D)operand1).Y; }
                    if (!(((Vector3D)operand1).Z == 0)) { diffV33 = diffV33 / ((Vector3D)operand1).Z; }

                    if ( (Math.Abs(diffV31) <= tolerance) && (Math.Abs(diffV32) <= tolerance) && (Math.Abs(diffV33) <= tolerance) ) { return true; }
                    else { return false; }

                case "System.Windows.Thickness":
                    double diffT1 = (((Thickness)operand1).Left) - (((Thickness)operand2).Left);
                    double diffT2 = (((Thickness)operand1).Top) - (((Thickness)operand2).Top);
                    double diffT3 = (((Thickness)operand1).Right) - (((Thickness)operand2).Right);
                    double diffT4 = (((Thickness)operand1).Bottom) - (((Thickness)operand2).Bottom);


                    if (!((((Thickness)operand1).Left) == 0)) { diffT1 = diffT1 / (((Thickness)operand1).Left); }
                    if (!((((Thickness)operand1).Top) == 0)) { diffT2 = diffT2 / (((Thickness)operand1).Top); }
                    if (!((((Thickness)operand1).Right) == 0)) { diffT3 = diffT3 / (((Thickness)operand1).Right); }
                    if (!((((Thickness)operand1).Bottom) == 0)) { diffT4 = diffT4 / (((Thickness)operand1).Bottom); }

                    if ( (Math.Abs(diffT1) <= tolerance) && (Math.Abs(diffT2) <= tolerance) && (Math.Abs(diffT3) <= tolerance) && (Math.Abs(diffT4) <= tolerance) ) { return true; }
                    else { return false; }
                case "System.Boolean":
                    if ((bool)operand1 == (bool)operand2) { return true; }
                    else { return false; }
                    case "System.Char":
                    if ((char)operand1 == (char)operand2) { return true; }
                    else { return false; }
                case "System.Windows.Media.Matrix":
                if ((Matrix)operand1 == (Matrix)operand2) { return true; }
                    else { return false; }
                case "System.String":
                    if (((String)operand1).Equals((String)operand2)) { return true; }
                    else { return false; }

                default:
                    return false;
            }
        }
    }
}
