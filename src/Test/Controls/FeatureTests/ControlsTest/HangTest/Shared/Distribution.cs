using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;

namespace HangTest
{
    [TypeConverter(typeof(DistributionConverter))]
    public abstract class Distribution
    {
        public Random BaseRandom { get; set; }

        public abstract double NextDouble();

        public virtual int NextInt()
        {
            return (int)NextDouble();
        }

        // specifying min and max changes the distribution F(z) to
        // G(z) = (z<=min) ? 0 : (z>=max) ? 1 : [F(z)-F(min)]/[F(max)-F(min)].
        // This is OK when 
        //   (a) min and max exclude an insignificant tail, i.e. the denominator
        //       is very nearly 1.0, and
        //   (b) you don't care about the exact distribution, but merely its
        //       general shape.
        // For example, if you want a random large set, choosing its size by
        // sampling Normal(10000,100) using NextInt(1) to guarantee non-empty
        // will give a density that's not quite a bell curve with mean 10000 and
        // std dev 100, but close enough for most practical purposes.
        public double NextDouble(double min=Double.NegativeInfinity, double max=Double.PositiveInfinity)
        {
            double result = NextDouble();
            while (result < min || result >= max)
            {
                result = NextDouble();
            }
            return result;
        }

        public int NextInt(int lower = Int32.MinValue, int upper = Int32.MaxValue)
        {
            int result = NextInt();
            while (result < lower || result >= upper)
            {
                result = NextInt();
            }
            return result;
        }
    }

    // constant distribution
    // i.e. F(z) = (z<v) ? 0 : 1
    public class ConstantDistribution : Distribution
    {
        double _value;

        public ConstantDistribution(double v)
        {
            _value = v;
        }

        public override double NextDouble()
        {
            return _value;
        }
    }

    // uniform distribution on [min, max)
    // i.e. F(z) = z for z in [0,1), transformed to [min, max)
    public class UniformDistribution : Distribution
    {
        double _lower, _range;

        public UniformDistribution(double lower=0.0, double upper=1.0)
        {
            _lower = lower;
            _range = upper - lower;
        }

        public override double NextDouble()
        {
            return BaseRandom.NextDouble() * _range + _lower;
        }
    }

    // "Coin-flip" distribution - Next returns 1 with probability p
    // and 0 with probability (1-p)
    public class ProbabilityDistribution : Distribution
    {
        double _p;

        public ProbabilityDistribution(double p)
        {
            _p = p;
        }

        public override double NextDouble()
        {
            return (BaseRandom.NextDouble() <= _p) ? 1.0 : 0.0;
        }
    }

    // Discrete weighted distribution - for weights w0, w1, ... wn
    // that sum to W, Next returns i with probability wi/W
    public class WeightDistribution : Distribution
    {
        double[] _p;

        public WeightDistribution(double[] w)
        {
            int n = w.Length;
            double W = 0;
            for (int i = 0; i < n; ++i)
                W += w[i];

            _p = new double[n];
            for (int i = 0; i < n; ++i)
                _p[i] = w[i] / W;
        }

        public override double NextDouble()
        {
            double x = BaseRandom.NextDouble();
            for (int i=0, n=_p.Length; i<n;  ++i)
            {
                x -= _p[i];
                if (x <= 0.0)
                    return i;
            }

            // the _p[i]'s sum to 1.0, so we shouldn't get here,
            // but it can happen due to rounding errors.  Return
            // an arbitrary result, just for safety
            return 0.0;
        }
    }

    // monomial distribution with exponent t on [min, max)
    // i.e. F(z) = z^t for z in [0,1), transformed to [min, max)
    public class MonomialDistribution : Distribution
    {
        double _lower, _range;
        int _t;

        public MonomialDistribution(int t, double lower=0.0, double upper=1.0)
        {
            _lower = lower;
            _range = upper - lower;
            _t = t;
        }

        // ACP 3.4.1.B
        public override double NextDouble()
        {
            double result = Double.NegativeInfinity;
            for (int i=0; i<_t; ++i)
            {
                double x = BaseRandom.NextDouble() * _range + _lower;
                result = Math.Max(result, x);
            }
            return result;
        }
    }

    // exponential distribution with mean mu and minimum x
    // i.e. F(z) = 1 - e^(-z/mu), suitably shifted
    public class ExponentialDistribution : Distribution
    {
        double _min, _minusMu;

        public ExponentialDistribution(double mu=1.0, double min=0.0)
        {
            _min = min;
            _minusMu = min - mu;
            if (_minusMu > 0.0)
                throw new ArgumentException("mu cannot be less than min");
        }

        // ACP 3.4.1.D
        public override double NextDouble()
        {
            while (true)
            {
                double u = _minusMu * Math.Log(BaseRandom.NextDouble()) + _min;
                if (!Double.IsNaN(u) && !Double.IsInfinity(u))
                    return u;
            }
        }
    }

    // normal distribution with mean mu and standard deviation sigma
    // i.e. F(z) = (2*pi)^(-1/2) * Int[-inf, z]{e^(-t^2/2)} dt, suitably transformed
    public class NormalDistribution : Distribution
    {
        static double c1 = Math.Sqrt(8.0 / Math.E);
        double _mu, _sigma;

        public NormalDistribution(double mu=0.0, double sigma=1.0)
        {
            _mu = mu;
            _sigma = sigma;
        }

        // ACP 3.4.1.C, Algorithm R
        public override double NextDouble()
        {
            double u = 0.0, v, x = 0.0;

            while (u == 0.0 || x * x > -4.0 * Math.Log(u))
            {
                u = BaseRandom.NextDouble();
                v = BaseRandom.NextDouble();

                if (u == 0.0) continue;     // don't divide by 0
                x = c1 * (v - 0.5) / u;
            }

            return _mu + _sigma * x;
        }
    }

    // bi-normal distribution, with probability p choose from N(mu1,sigma1) otherwise
    // choose from N(mu2,sigma2).
    // Useful for interspersing "small" and "large" values
    public class BiNormalDistribution: Distribution
    {
        Random _currentBaseRandom;
        double _p;
        NormalDistribution _n1, _n2;

        public BiNormalDistribution(double p, double mu1, double sigma1, double mu2, double sigma2)
        {
            _p = p;
            _n1 = new NormalDistribution(mu1, sigma1);
            _n2 = new NormalDistribution(mu2, sigma2);
        }

        public override double NextDouble()
        {
            if (BaseRandom != _currentBaseRandom)
            {
                _currentBaseRandom = BaseRandom;
                _n1.BaseRandom = _currentBaseRandom;
                _n2.BaseRandom = _currentBaseRandom;
            }

            return (BaseRandom.NextDouble() <= _p) ? _n1.NextDouble() : _n2.NextDouble();
        }
    }

    public class DistributionConverter : TypeConverter
    {
        static char[] Separators = { ' ', ',' };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text == null)
                return base.ConvertFrom(context, culture, value);

            int leftParen = text.IndexOf('('), rightParen = text.LastIndexOf(')');
            if (leftParen < 0)
            {
                leftParen = text.Length;
            }
            if (rightParen <= leftParen)
            {
                rightParen = leftParen + 1;
            }

            string name = text.Substring(0, leftParen).Trim();
            string argText = (leftParen < text.Length) ? text.Substring(leftParen + 1, rightParen - leftParen - 1) : String.Empty;
            string[] args = argText.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            double[] dargs;

            switch (name)
            {
                case "C":
                    if (args.Length == 1)
                    {
                        dargs = ConvertArgs(args, 0, 1);
                        return new ConstantDistribution(dargs[0]);
                    }
                    else
                        throw new FormatException("C takes 1 argument.");

                case "U":
                    switch (args.Length)
                    {
                        case 0:
                            return new UniformDistribution();
                        case 2:
                            dargs = ConvertArgs(args, 0, 2);
                            return new UniformDistribution(dargs[0], dargs[1]);
                        default:
                            throw new FormatException("U takes 0 or 2 arguments.");
                    }

                case "P":
                    if (args.Length == 1)
                    {
                        dargs = ConvertArgs(args, 0, 1);
                        double p = dargs[0];
                        // interpret arg as a probability or as a percentage
                        if (p <= 1.0) return new ProbabilityDistribution(p);
                        if (p <= 100.0) return new ProbabilityDistribution(p/100.0);
                        throw new ArgumentException("P argument out of range: " + args[0]);
                    }
                    else
                        throw new FormatException("P takes 1 argument.");

                case "W":
                    dargs = ConvertArgs(args, 0, args.Length);
                    return new WeightDistribution(dargs);

                case "M":
                    if (args.Length > 0)
                    {
                        int t = Int32.Parse(args[0], CultureInfo.InvariantCulture);
                        switch (args.Length)
                        {
                            case 1:
                                return new MonomialDistribution(t);
                            case 3:
                                dargs = ConvertArgs(args, 1, 2);
                                return new MonomialDistribution(t, dargs[0], dargs[1]);
                            default:
                                throw new FormatException("M takes 1 or 3 arguments.");
                        }
                    }
                    else
                        throw new FormatException("M is missing required argument.");

                case "E":
                    switch (args.Length)
                    {
                        case 0:
                            return new ExponentialDistribution();
                        case 1:
                            dargs = ConvertArgs(args, 0, 1);
                            return new ExponentialDistribution(dargs[0]);
                        case 2:
                            dargs = ConvertArgs(args, 0, 2);
                            return new ExponentialDistribution(dargs[0], dargs[1]);
                        default:
                            throw new FormatException("E takes 0-2 arguments.");
                    }

                case "N":
                    switch (args.Length)
                    {
                        case 0:
                            return new NormalDistribution();
                        case 1:
                            dargs = ConvertArgs(args, 0, 1);
                            return new NormalDistribution(dargs[0]);
                        case 2:
                            dargs = ConvertArgs(args, 0, 2);
                            return new NormalDistribution(dargs[0], dargs[1]);
                        default:
                            throw new FormatException("D takes 0-2 arguments.");
                    }

                case "BN":
                    switch (args.Length)
                    {
                        case 5:
                            dargs = ConvertArgs(args, 0, 5);
                            return new BiNormalDistribution(dargs[0], dargs[1], dargs[2], dargs[3], dargs[4]);
                        default:
                            throw new FormatException("BN takes 5 arguments.");
                    }

                default:
                    throw new FormatException("Unknown distribution: " + name);
            }
        }

        double[] ConvertArgs(string[] args, int start, int length)
        {
            double[] result = new double[length];
            for (int i=0; i<length; ++i)
            {
                result[i] = Double.Parse(args[start + i].Trim(), CultureInfo.InvariantCulture);
            }
            return result;
        }
    }

}
