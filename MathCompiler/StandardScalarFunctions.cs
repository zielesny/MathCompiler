// MathCompiler - Mathematical function compiler for calculation of function 
// values of arbitrary complex single-line mathematical formulas
// Copyright (C) 2018  Achim Zielesny (achim.zielesny@googlemail.com)
// 
// Source code is available at <https://github.com/zielesny/MathCompiler>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;

namespace MathCompiler
{

    /// <summary>
    /// Standard scalar functions
    /// </summary>
    public static class StandardScalarFunctions
    {

        /// <summary>
        /// Returns standard scalar functions
        /// </summary>
        /// <returns>Standard scalar functions</returns>
        public static IScalarFunction[] GetStandardScalarFunctions()
        {
            return new IScalarFunction[]
            {
                new Abs(),
                new Acos(),
                new Asin(),
                new Atan(),
                new Cos(),
                new Cosh(),
                new Exp(),
                new Lg(),
                new Ln(),
                new Log(),
                new Min(),
                new Max(),
                new Round(),
                new Sin(),
                new Sinh(),
                new Sqrt(),
                new Tan(),
                new Tanh()
            };
        }

    }

    #region Standard scalar function classes

    // Standard scalar functions with at least one scalar argument

    #region Abs

    /// <summary>
    /// Absolute value of the specified number
    /// </summary>
    internal class Abs : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Abs(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "abs";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "absolute value of the specified number.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Acos

    /// <summary>
    /// Angle measured in radians whose cosine is the specified number
    /// </summary>
    internal class Acos : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Acos(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "acos";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "angle measured in radians whose cosine is the specified number.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Asin

    /// <summary>
    /// Angle measured in radians whose sine is the specified number
    /// </summary>
    internal class Asin : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Asin(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "asin";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "angle measured in radians whose sine is the specified number.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Atan

    /// <summary>
    /// Angle measured in radians whose tangent is the specified number
    /// </summary>
    internal class Atan : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Atan(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "atan";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "angle measured in radians whose tangent is the specified number.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Cos

    /// <summary>
    /// Cosine of an angle in radians
    /// </summary>
    internal class Cos : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Cos(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "cos";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "cosine of an angle in radians.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Cosh

    /// <summary>
    /// Hyperbolic cosine of an angle in radians
    /// </summary>
    internal class Cosh : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Cosh(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "cosh";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "hyperbolic cosine of an angle in radians.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Exp

    /// <summary>
    /// e raised to the specified power
    /// </summary>
    internal class Exp : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Exp(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "exp";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "e raised to the specified power.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Lg

    /// <summary>
    /// Base 10 logarithm of a specified number
    /// </summary>
    internal class Lg : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Log10(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "lg";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "base 10 logarithm of a specified number.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Ln

    /// <summary>
    /// Natural logarithm of a specified number
    /// </summary>
    internal class Ln : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Log(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "ln";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "natural logarithm of a specified number.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Log

    /// <summary>
    /// Logarithm of a specified number (first argument) in a specified base (second argument)
    /// </summary>
    internal class Log : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Log(arguments[0], arguments[1]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 2;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "log";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "logarithm of a specified number (first argument) in a specified base (second argument).";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Min

    /// <summary>
    /// Smaller of two numbers
    /// </summary>
    internal class Min : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Min(arguments[0], arguments[1]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 2;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "min";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "smaller of two numbers.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Max

    /// <summary>
    /// Larger of two numbers
    /// </summary>
    internal class Max : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Max(arguments[0], arguments[1]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 2;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "max";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "larger of two numbers.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Round

    /// <summary>
    /// Rounds a value to the specified precision: Second argument is number of digits
    /// </summary>
    internal class Round : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Round(arguments[0], Convert.ToInt32(arguments[1]));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 2;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "round";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "rounded value to the specified precision: Second argument is the number of digits.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Sin

    /// <summary>
    /// Sine of an angle in radians
    /// </summary>
    internal class Sin : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Sin(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "sin";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "sine of an angle in radians.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Sinh

    /// <summary>
    /// Hyperbolic sine of an angle in radians
    /// </summary>
    internal class Sinh : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Sinh(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "sinh";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "hyperbolic sine of an angle in radians.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Sqrt

    /// <summary>
    /// Square root of a specified number
    /// </summary>
    internal class Sqrt : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Sqrt(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "sqrt";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "square root of a specified number.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Tan

    /// <summary>
    /// Tangent of an angle in radians
    /// </summary>
    internal class Tan : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Tan(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "tan";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "tangent of an angle in radians.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #region Tanh

    /// <summary>
    /// Hyperbolic tangent of an angle in radians
    /// </summary>
    internal class Tanh : IScalarFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates function value
        /// </summary>
        /// <param name="arguments">
        /// Array with arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments)
        {
            return Math.Tanh(arguments[0]);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number of arguments
        /// </summary>
        public Int32 NumberOfArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Name of function
        /// </summary>
        public String Name
        {
            get
            {
                return "tanh";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "hyperbolic tangent of an angle in radians.";
            }
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        public Int32 Token
        {
            get
            {
                return this.myToken;
            }
            set
            {
                this.myToken = value;
            }
        }

        #endregion
    }

    #endregion

    #endregion

}
