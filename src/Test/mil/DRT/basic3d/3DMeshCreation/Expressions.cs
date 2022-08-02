// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Tests
{
    //--------------------------------------------------------------

    public interface IExpression
    {
        double Evaluate( double u, double v );
        IExpression Differentiate( string byVar );
        IExpression Simplify();
        string ToString();
    }

    /// <summary>
    /// ConstantExpression - An expression which always resolves to a constant 
    /// value.
    /// </summary>
    public sealed class ConstantExpression : IExpression
    {
        public ConstantExpression( double value )
        {
            this.value = value;
        }

        /// <summary>
        /// A constant always evaluates to its value. 
        /// </summary>
        public double Evaluate( double u, double v )
        {
            return value;
        }

        /// <summary>
        /// The derivative of a constant is 0.
        /// </summary>
        public IExpression Differentiate( string byVar )
        {
            //      f(x) = c;
            // d( f(x) ) = c * d( c );
            //    d( c ) = 0;
            //     f'(x) = 0;
            return new ConstantExpression( 0 );
        }

        public IExpression Simplify()
        {
            return new ConstantExpression( value );
        }

        string IExpression.ToString()
        {
            return value.ToString();
        }

        public double Value { get { return this.value; } }

        private double value;
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Expression based on a single variable.
    /// </summary>
    public sealed class VariableExpression : IExpression
    {
        public VariableExpression( string identifier )
        {
            this.identifier = identifier;
        }

        /// <summary>
        /// Current variables understood are: u, v, and pi.  They each return
        /// their respective value.
        /// </summary>
        public double Evaluate( double u, double v )
        {
            if ( identifier == "u" )
            {
                return u;
            }
            else if ( identifier == "v" )
            {
                return v;
            }
            else if ( identifier.ToLower() == "pi" )
            {
                return Math.PI;
            }
            else
            {
                throw new UndefinedVariableException( "I can't evaluate this variable: " + identifier );
            }
        }

        /// <summary>
        /// Differentiate the Variable by input variable.
        /// </summary>
        public IExpression Differentiate( string byVar )
        {
            if ( byVar == identifier )
            {
                //      f(x) = x;
                // d( f(x) ) = 1 * d( x );
                //    d( x ) = 1;
                //     f'(x) = 1;
                return new ConstantExpression( 1 );
            }
            //      f(x) = c;
            // d( f(x) ) = c * d( c );
            //    d( c ) = 0;
            //     f'(x) = 0;
            return new ConstantExpression( 0 );
        }

        public IExpression Simplify()
        {
            if ( identifier == "pi" )
            {
                return new ConstantExpression( Math.PI );
            }
            return new VariableExpression( identifier );
        }

        string IExpression.ToString()
        {
            return identifier;
        }

        private string identifier;
    }

    #region Unary Expressions

    //--------------------------------------------------------------

    /// <summary>
    /// An expression which holds onto a single child expression.  All of its 
    /// functions call into the child functions.
    /// </summary>
    public abstract class UnaryExpression : IExpression
    {
        public UnaryExpression( IExpression child )
        {
            this.child = child;
        }

        public double Evaluate( double u, double v )
        {
            return Operate( child.Evaluate( u, v ) );
        }

        string IExpression.ToString()
        {
            return this.ToString();
        }

        protected abstract double Operate( double d );
        public abstract IExpression Differentiate( string byVar );
        public abstract IExpression Simplify();
        public abstract override string ToString();

        public IExpression Child { get { return child; } }

        protected IExpression child;
    }

    //--------------------------------------------------------------

    /// <summary>
    /// An expression which holds onto and negates a single child expression.  
    /// All of its functions call into the child functions and return the
    /// negation.
    /// </summary>
    public sealed class NegateExpression : UnaryExpression
    {
        public NegateExpression( IExpression child ) : base( child )
        {
        }

        protected override double Operate( double d )
        {
            return -d;
        }

        public override IExpression Differentiate( string byVar )
        {
            //      f(x) = -g(x);
            // d( f(x) ) = -( d( g(x) ) );
            // d( g(x) ) = g'(x)
            //     f'(x) = -g'(x);
            return new NegateExpression( child.Differentiate( byVar ) );
        }

        public override IExpression Simplify()
        {
            IExpression newChild = child.Simplify();
            ConstantExpression childConst = newChild as ConstantExpression;

            if ( childConst != null )
            {
                // child is constant;  just evaluate it;
                return new ConstantExpression( -childConst.Value );
            }
            return new NegateExpression( newChild );
        }

        public override string ToString()
        {
            return "(-" + child.ToString() + ")";
        }
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Evaluate the sine of an expression.
    /// </summary>
    public sealed class SineExpression : UnaryExpression
    {
        public SineExpression( IExpression child ) : base( child )
        {
        }

        protected override double Operate( double d )
        {
            return Math.Sin( d );
        }

        public override IExpression Differentiate( string byVar )
        {
            //      f(x) = sin( g(x) );
            // d( f(x) ) = cos( g(x) ) * d( g(x) );
            // d( g(x) ) = g'(x)
            //     f'(x) = cos( g(x) ) * g'(x);
            return new MultExpression( new CosineExpression( child ), child.Differentiate( byVar ) );
        }

        public override IExpression Simplify()
        {
            IExpression newChild = child.Simplify();
            ConstantExpression childConst = newChild as ConstantExpression;

            if ( childConst != null )
            {
                // child is constant;  just evaluate it;
                return new ConstantExpression( Math.Sin( childConst.Value ) );
            }
            return new SineExpression( newChild );
        }

        public override string ToString()
        {
            return "sin(" + child.ToString() + ")";
        }
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Evaluate the cosine of an expression.
    /// </summary>
    public sealed class CosineExpression : UnaryExpression
    {
        public CosineExpression( IExpression child ) : base( child )
        {
        }

        protected override double Operate( double d )
        {
            return Math.Cos( d );
        }

        public override IExpression Differentiate( string byVar )
        {
            //      f(x) = cos( g(x) );
            // d( f(x) ) = -sin( g(x) ) * d( g(x) );
            // d( g(x) ) = g'(x)
            //     f'(x) = -sin( g(x) ) * g'(x);
            return new MultExpression( new NegateExpression( new SineExpression( child ) ),
                                        child.Differentiate( byVar ) );
        }

        public override IExpression Simplify()
        {
            IExpression newChild = child.Simplify();
            ConstantExpression childConst = newChild as ConstantExpression;

            if ( childConst != null )
            {
                // child is constant;  just evaluate it;
                return new ConstantExpression( Math.Cos( childConst.Value ) );
            }
            return new CosineExpression( newChild );
        }

        public override string ToString()
        {
            return "cos(" + child.ToString() + ")";
        }
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Evaluate the tangent of an expression.
    /// </summary>
    public sealed class TangentExpression : UnaryExpression
    {
        public TangentExpression( IExpression child ) : base( child )
        {
        }

        protected override double Operate( double d )
        {
            return Math.Tan( d );
        }

        public override IExpression Differentiate( string byVar )
        {
            //      f(x) = tan( g(x) );
            // d( f(x) ) = cos^-2( g(x) ) * d( g(x) );
            // d( g(x) ) = g'(x)
            //     f'(x) = cos^-2( g(x) ) * g'(x);
            return new MultExpression( new ExpExpression( new CosineExpression( child ),
                                                          new ConstantExpression( -2 ) ),
                                       child.Differentiate( byVar ) );
        }

        public override IExpression Simplify()
        {
            IExpression newChild = child.Simplify();
            ConstantExpression childConst = newChild as ConstantExpression;

            if ( childConst != null )
            {
                // child is constant;  just evaluate it;
                return new ConstantExpression( Math.Tan( childConst.Value ) );
            }
            return new TangentExpression( newChild );
        }

        public override string ToString()
        {
            return "tan(" + child.ToString() + ")";
        }
    }

    #endregion

    #region Binary Expressions

    //--------------------------------------------------------------

    /// <summary>
    /// Combines the results of 2 expressions, with the binary operation.
    /// </summary>
    public abstract class BinaryExpression : IExpression
    {
        public BinaryExpression( IExpression left, IExpression right )
        {
            this.left = left;
            this.right = right;
        }

        public double Evaluate( double u, double v )
        {
            return Operate( left.Evaluate( u, v ), right.Evaluate( u, v ) );
        }

        string IExpression.ToString()
        {
            return this.ToString();
        }

        protected abstract double Operate( double d1, double d2 );
        public abstract IExpression Differentiate( string byVar );
        public abstract IExpression Simplify();
        public abstract override string ToString();

        public IExpression Left { get { return left; } }
        public IExpression Right { get { return right; } }

        protected IExpression left;
        protected IExpression right;
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Adds 2 expressions
    /// </summary>
    public sealed class AddExpression : BinaryExpression
    {
        public AddExpression( IExpression left, IExpression right ) : base( left, right )
        {
        }

        protected override double Operate( double d1, double d2 )
        {
            return d1 + d2;
        }

        public override IExpression Differentiate( string byVar )
        {
            //      f(x) = g(x) + h(x);
            // d( f(x) ) = d( g(x) ) + d( h(x) );
            //     f'(x) = g'(x) + h'(x);
            return new AddExpression( left.Differentiate( byVar ), right.Differentiate( byVar ) );
        }

        public override IExpression Simplify()
        {
            IExpression newLeft = left.Simplify();
            IExpression newRight = right.Simplify();

            ConstantExpression leftConst = newLeft as ConstantExpression;
            ConstantExpression rightConst = newRight as ConstantExpression;
            NegateExpression leftNegate = newLeft as NegateExpression;
            NegateExpression rightNegate = newRight as NegateExpression;

            if ( leftConst != null && rightConst != null )
            {
                // two constants;  just evaluate it;
                return new ConstantExpression( leftConst.Value + rightConst.Value );
            }
            else if ( leftConst != null && leftConst.Value == 0 )
            {
                // 0 + y;  return y;
                return newRight;
            }
            else if ( rightConst != null && rightConst.Value == 0 )
            {
                // x + 0;  return x;
                return newLeft;
            }
            else if ( rightNegate != null )
            {
                // x + -y;  return x - y;  (this covers -x + -y case too)
                return new SubExpression( newLeft, rightNegate.Child );
            }
            else if ( leftNegate != null )
            {
                // -x + y;  return y - x;
                return new SubExpression( newRight, leftNegate.Child );
            }
            // x + y;  no simplification
            return new AddExpression( newLeft, newRight );
        }

        public override string ToString()
        {
            return "(" + left.ToString() + "+" + right.ToString() + ")";
        }
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Subtracts 2 expressions.
    /// </summary>
    public sealed class SubExpression : BinaryExpression
    {
        public SubExpression( IExpression left, IExpression right ) : base( left, right )
        {
        }

        protected override double Operate( double d1, double d2 )
        {
            return d1 - d2;
        }

        public override IExpression Differentiate( string byVar )
        {
            //      f(x) = g(x) - h(x);
            // d( f(x) ) = d( g(x) ) - d( h(x) );
            //     f'(x) = g'(x) - h'(x);
            return new SubExpression( left.Differentiate( byVar ), right.Differentiate( byVar ) );
        }

        public override IExpression Simplify()
        {
            IExpression newLeft = left.Simplify();
            IExpression newRight = right.Simplify();

            ConstantExpression leftConst = newLeft as ConstantExpression;
            ConstantExpression rightConst = newRight as ConstantExpression;
            NegateExpression rightNegate = newRight as NegateExpression;

            if ( leftConst != null && rightConst != null )
            {
                // two constants;  just evaluate it;
                return new ConstantExpression( leftConst.Value - rightConst.Value );
            }
            else if ( leftConst != null && leftConst.Value == 0 )
            {
                // 0 - y;  return -y;
                if ( rightNegate != null )
                {
                    // y = -u (--u);  return u;
                    return rightNegate.Child;
                }
                return new NegateExpression( newRight );
            }
            else if ( rightConst != null && rightConst.Value == 0 )
            {
                // x - 0;  return x;
                return newLeft;
            }
            else if ( rightNegate != null )
            {
                // x - -y;  return x + y;
                return new AddExpression( newLeft, rightNegate.Child );
            }
            // x - y;  no simplification
            return new SubExpression( newLeft, newRight );
        }

        public override string ToString()
        {
            return "(" + left.ToString() + "-" + right.ToString() + ")";
        }
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Multiplies 2 expressions.
    /// </summary>
    public sealed class MultExpression : BinaryExpression
    {
        public MultExpression( IExpression left, IExpression right ) : base( left, right )
        {
        }

        protected override double Operate( double d1, double d2 )
        {
            return d1 * d2;
        }

        public override IExpression Differentiate( string byVar )
        {
            //      f(x) = g(x)*h(x);
            //     f'(x) = g(x)*h'(x) + g'(x)*h(x);
            return new AddExpression( new MultExpression( left, right.Differentiate( byVar ) ),
                                      new MultExpression( left.Differentiate( byVar ), right ) );
        }

        public override IExpression Simplify()
        {
            IExpression newLeft = left.Simplify();
            IExpression newRight = right.Simplify();

            ConstantExpression leftConst = newLeft as ConstantExpression;
            ConstantExpression rightConst = newRight as ConstantExpression;
            NegateExpression leftNegate = newLeft as NegateExpression;
            NegateExpression rightNegate = newRight as NegateExpression;

            if ( leftConst != null && rightConst != null )
            {
                // two constants;  just evaluate it;
                return new ConstantExpression( leftConst.Value * rightConst.Value );
            }
            else if ( leftConst != null )
            {
                if ( leftConst.Value == 0 )
                {
                    // 0 * y;  return 0;
                    return new ConstantExpression( 0 );
                }
                if ( leftConst.Value == 1 )
                {
                    // 1 * y;  return y;
                    return newRight;
                }
                if ( leftConst.Value == -1 )
                {
                    // -1 * y;  return -y
                    if ( rightNegate != null )
                    {
                        // y = -u (-y = --u);  return u;
                        return rightNegate.Child;
                    }
                    return new NegateExpression( newRight );
                }
            }
            else if ( rightConst != null )
            {
                if ( rightConst.Value == 0 )
                {
                    // x * 0;  return 0;
                    return new ConstantExpression( 0 );
                }
                if ( rightConst.Value == 1 )
                {
                    // x * 1;  return x;
                    return newLeft;
                }
                if ( rightConst.Value == -1 )
                {
                    // x * -1;  return -x;
                    if ( leftNegate != null )
                    {
                        // x = -u (-x = --u);  return u;
                        return leftNegate.Child;
                    }
                    return new NegateExpression( newLeft );
                }
            }
            else if ( leftNegate != null && rightNegate != null )
            {
                // -x * -y;  return x * y;
                return new MultExpression( leftNegate.Child, rightNegate.Child );
            }
            // x * y;  no simplification
            return new MultExpression( newLeft, newRight );
        }

        public override string ToString()
        {
            return "(" + left.ToString() + "*" + right.ToString() + ")";
        }
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Divides first expression by the second.
    /// </summary>
    public sealed class DivExpression : BinaryExpression
    {
        public DivExpression( IExpression left, IExpression right ) : base( left, right )
        {
        }

        protected override double Operate( double d1, double d2 )
        {
            return d1 / d2;
        }

        public override IExpression Differentiate( string byVar )
        {
            //      f(x) = g(x)/h(x);
            //     f'(x) = ( g'(x)*h(x) - g(x)*h'(x) )/( h(x)^2 ) ;
            return new DivExpression( new SubExpression( new MultExpression( left.Differentiate( byVar ), right ),
                                                         new MultExpression( left, right.Differentiate( byVar ) ) ),
                                      new ExpExpression( right, new ConstantExpression( 2 ) ) );
        }

        public override IExpression Simplify()
        {
            IExpression newLeft = left.Simplify();
            IExpression newRight = right.Simplify();

            ConstantExpression leftConst = newLeft as ConstantExpression;
            ConstantExpression rightConst = newRight as ConstantExpression;
            NegateExpression leftNegate = newLeft as NegateExpression;
            NegateExpression rightNegate = newRight as NegateExpression;

            if ( leftConst != null && rightConst != null )
            {
                // two constants;  just evaluate it;
                if ( rightConst.Value == 0 )
                {
                    throw new InvalidExpressionException( "Divide by zero detected in function" );
                }
                return new ConstantExpression( leftConst.Value / rightConst.Value );
            }
            else if ( leftConst != null && leftConst.Value == 0 )
            {
                // 0 / y;  return 0;
                if ( rightConst != null && rightConst.Value == 0 )
                {
                    throw new InvalidExpressionException( "Divide by zero detected in function" );
                }
                return new ConstantExpression( 0 );
            }
            else if ( rightConst != null )
            {
                if ( rightConst.Value == 0 )
                {
                    // x / 0;
                    throw new InvalidExpressionException( "Divide by zero detected in function" );
                }
                if ( rightConst.Value == 1 )
                {
                    // x / 1;  return x;
                    return newLeft;
                }
                if ( rightConst.Value == -1 )
                {
                    // x / -1;  return -x;
                    if ( leftNegate != null )
                    {
                        // x = -u (-x = --u);  return u;
                        return leftNegate.Child;
                    }
                    return new NegateExpression( newLeft );
                }
            }
            else if ( leftNegate != null && rightNegate != null )
            {
                // -x / -y;  return x / y;
                return new DivExpression( leftNegate.Child, rightNegate.Child );
            }
            // x / y;  no simplification
            return new DivExpression( newLeft, newRight );
        }

        public override string ToString()
        {
            return "(" + left.ToString() + "/" + right.ToString() + ")";
        }
    }

    //--------------------------------------------------------------

    /// <summary>
    /// Raise the first expression to the power of the second.
    /// </summary>
    public sealed class ExpExpression : BinaryExpression
    {
        public ExpExpression( IExpression left, IExpression right ) : base( left, right )
        {
        }

        protected override double Operate( double d1, double d2 )
        {
            return Math.Pow( d1, d2 );
        }

        public override IExpression Differentiate( string byVar )
        {
            if ( right is ConstantExpression )
            {
                //      f(x) = g(x)^n;
                //     f'(x) = n * g'(x)^(n-1) * d( g(x) );
                return new MultExpression( new MultExpression( right, left.Differentiate( byVar ) ),
                                           new ExpExpression( left,
                                                              new SubExpression( right,
                                                                                 new ConstantExpression( 1 ) ) ) );
            }
            throw new CannotDifferentiateException( "I do not support complex exponent differentiation" );
        }

        public override IExpression Simplify()
        {
            IExpression newLeft = left.Simplify();
            IExpression newRight = right.Simplify();

            ConstantExpression leftConst = newLeft as ConstantExpression;
            ConstantExpression rightConst = newRight as ConstantExpression;

            if ( leftConst != null && rightConst != null )
            {
                // two constants;  just evaluate it;
                return new ConstantExpression( Math.Pow( leftConst.Value, rightConst.Value ) );
            }
            else if ( rightConst != null )
            {
                if ( rightConst.Value == 0 )
                {
                    // x ^ 0;  return 1;
                    return new ConstantExpression( 1 );
                }
                else if ( rightConst.Value == 1 )
                {
                    // x ^ 1;  return x;
                    return newLeft;
                }
            }
            else if ( leftConst != null && leftConst.Value == 0 )
            {
                // 0 ^ y;  return 0;
                return new ConstantExpression( 0 );
            }
            // x ^ y;  no simplification
            return new ExpExpression( newLeft, newRight );
        }

        public override string ToString()
        {
            return "(" + left.ToString() + "^" + right.ToString() + ")";
        }
    }

    #endregion

    // 
}
