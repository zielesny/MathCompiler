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
using System.Globalization;

namespace MathCompiler
{

    /// <summary>
    /// Standard scalar functions
    /// </summary>
    public static class StandardVectorFunctions
    {

        /// <summary>
        /// Returns standard vector functions
        /// </summary>
        /// <returns>Standard vector functions</returns>
        public static IVectorFunction[] GetStandardVectorFunctions()
        {
            return new IVectorFunction[]
            {
                new Component(),
                new Count(),
                new MaxComponent(),
                new Mean(),
                new MinComponent(),
                new SampleError(),
                new Subtotal(),
                new Sum()
            };
        }

    }

    #region Standard vector function classes

    // Standard vector functions with at least one vector argument

    #region Component

    /// <summary>
    /// Specified component of vector argument
    /// </summary>
    internal class Component : IVectorFunction
    {
        #region Class variables

        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;

        #endregion

        #region Methods

        /// <summary>
        /// Calculates vector function value
        /// </summary>
        /// <param name="arguments">
        /// Array of scalar arguments
        /// </param>
        /// <param name="vectorArguments">
        /// Array of arrays with arguments, i.e. array of vector arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            return vectorArguments[0][Convert.ToInt32(arguments[0])];
        }
        /// <summary>
        /// Returns if argument with specified index is a vector argument
        /// </summary>
        /// <param name="argumentIndex">
        /// Index of argument
        /// </param>
        /// <returns>
        /// True: Argument is a vector argument, 
        /// false: argument is a scalar argument
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if argumentIndex is invalid
        /// </exception>
        public Boolean IsVectorArgument(Int32 argumentIndex)
        {
            switch (argumentIndex)
            {
                case 0:
                    return true;
                case 1:
                    return false;
                default:
                    throw new MathCompilerException(
                        "Illegal argument index: '" +
                        argumentIndex.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                        "'.");
            }
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
        /// Number of scalar arguments
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 NumberOfScalarArguments
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        public Int32 NumberOfVectorArguments
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
                return "component";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "specified component of vector argument: Scalar argument = index of component.";
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

    #region Count

    /// <summary>
    /// Number of components of argument vector
    /// </summary>
    internal class Count : IVectorFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates vector function value
        /// </summary>
        /// <param name="arguments">
        /// Array of scalar arguments
        /// </param>
        /// <param name="vectorArguments">
        /// Array of arrays with arguments, i.e. array of vector arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            return vectorArguments[0].Length;
        }
        /// <summary>
        /// Returns if argument with specified index is a vector argument
        /// </summary>
        /// <param name="argumentIndex">
        /// Index of argument
        /// </param>
        /// <returns>
        /// True: Argument is a vector argument, 
        /// false: argument is a scalar argument
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if argumentIndex is invalid
        /// </exception>
        public Boolean IsVectorArgument(Int32 argumentIndex)
        {
            if (argumentIndex != 0)
            {
                throw new MathCompilerException(
                    "Illegal argument index: '" +
                    argumentIndex.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                    "'.");
            }
            else
            {
                return true;
            }
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
        /// Number of scalar arguments
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 NumberOfScalarArguments
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        public Int32 NumberOfVectorArguments
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
                return "count";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "number of components of argument vector.";
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

    #region MaxComponent

    /// <summary>
    /// Maximum component of vector argument
    /// </summary>
    internal class MaxComponent : IVectorFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates vector function value
        /// </summary>
        /// <param name="arguments">
        /// Array of scalar arguments
        /// </param>
        /// <param name="vectorArguments">
        /// Array of arrays with arguments, i.e. array of vector arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            Double max;

            max = vectorArguments[0][0];
            for (Int32 i = 1; i < vectorArguments[0].Length; i++)
            {
                if (max < vectorArguments[0][i])
                {
                    max = vectorArguments[0][i];
                }
            }
            return max;
        }
        /// <summary>
        /// Returns if argument with specified index is a vector argument
        /// </summary>
        /// <param name="argumentIndex">
        /// Index of argument
        /// </param>
        /// <returns>
        /// True: Argument is a vector argument, 
        /// false: argument is a scalar argument
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if argumentIndex is invalid
        /// </exception>
        public Boolean IsVectorArgument(Int32 argumentIndex)
        {
            if (argumentIndex != 0)
            {
                throw new MathCompilerException(
                    "Illegal argument index: '" +
                    argumentIndex.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                    "'.");
            }
            else
            {
                return true;
            }
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
        /// Number of scalar arguments
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 NumberOfScalarArguments
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        public Int32 NumberOfVectorArguments
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
                return "maxComponent";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "maximum component of vector argument.";
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

    #region Mean

    /// <summary>
    /// Arithmetic mean of components of vector argument
    /// </summary>
    internal class Mean : IVectorFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates vector function value
        /// </summary>
        /// <param name="arguments">
        /// Array of scalar arguments
        /// </param>
        /// <param name="vectorArguments">
        /// Array of arrays with arguments, i.e. array of vector arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            Double sum;

            sum = 0;
            for (Int32 i = 0; i < vectorArguments[0].Length; i++)
            {
                sum += vectorArguments[0][i];
            }
            return sum / Convert.ToDouble(vectorArguments[0].Length);
        }
        /// <summary>
        /// Returns if argument with specified index is a vector argument
        /// </summary>
        /// <param name="argumentIndex">
        /// Index of argument
        /// </param>
        /// <returns>
        /// True: Argument is a vector argument, 
        /// false: argument is a scalar argument
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if argumentIndex is invalid
        /// </exception>
        public Boolean IsVectorArgument(Int32 argumentIndex)
        {
            if (argumentIndex != 0)
            {
                throw new MathCompilerException(
                    "Illegal argument index: '" +
                    argumentIndex.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                    "'.");
            }
            else
            {
                return true;
            }
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
        /// Number of scalar arguments
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 NumberOfScalarArguments
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        public Int32 NumberOfVectorArguments
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
                return "mean";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "arithmetic mean of components of vector argument.";
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

    #region MinComponent

    /// <summary>
    /// Minimum component of vector argument
    /// </summary>
    internal class MinComponent : IVectorFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates vector function value
        /// </summary>
        /// <param name="arguments">
        /// Array of scalar arguments
        /// </param>
        /// <param name="vectorArguments">
        /// Array of arrays with arguments, i.e. array of vector arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            Double min;

            min = vectorArguments[0][0];
            for (Int32 i = 1; i < vectorArguments[0].Length; i++)
            {
                if (min > vectorArguments[0][i])
                {
                    min = vectorArguments[0][i];
                }
            }
            return min;
        }
        /// <summary>
        /// Returns if argument with specified index is a vector argument
        /// </summary>
        /// <param name="argumentIndex">
        /// Index of argument
        /// </param>
        /// <returns>
        /// True: Argument is a vector argument, 
        /// false: argument is a scalar argument
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if argumentIndex is invalid
        /// </exception>
        public Boolean IsVectorArgument(Int32 argumentIndex)
        {
            if (argumentIndex != 0)
            {
                throw new MathCompilerException(
                    "Illegal argument index: '" +
                    argumentIndex.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                    "'.");
            }
            else
            {
                return true;
            }
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
        /// Number of scalar arguments
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 NumberOfScalarArguments
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        public Int32 NumberOfVectorArguments
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
                return "minComponent";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "minimum component of vector argument.";
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

    #region SampleError

    /// <summary>
    /// Root mean square error (standard deviation) of components of vector argument (sample standard deviation: (N-1) in demoninator)
    /// </summary>
    internal class SampleError : IVectorFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates vector function value
        /// </summary>
        /// <param name="arguments">
        /// Array of scalar arguments
        /// </param>
        /// <param name="vectorArguments">
        /// Array of arrays with arguments, i.e. array of vector arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            Double sum;
            Double sumSquared;

            sum = 0;
            sumSquared = 0;
            for (Int32 i = 0; i < vectorArguments[0].Length; i++)
            {
                sum += vectorArguments[0][i];
                sumSquared += vectorArguments[0][i] * vectorArguments[0][i];
            }
            return Math.Sqrt((Convert.ToDouble(vectorArguments[0].Length) * sumSquared - sum * sum) / (Convert.ToDouble(vectorArguments[0].Length) * (Convert.ToDouble(vectorArguments[0].Length) - 1)));
        }
        /// <summary>
        /// Returns if argument with specified index is a vector argument
        /// </summary>
        /// <param name="argumentIndex">
        /// Index of argument
        /// </param>
        /// <returns>
        /// True: Argument is a vector argument, 
        /// false: argument is a scalar argument
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if argumentIndex is invalid
        /// </exception>
        public Boolean IsVectorArgument(Int32 argumentIndex)
        {
            if (argumentIndex != 0)
            {
                throw new MathCompilerException(
                    "Illegal argument index: '" +
                    argumentIndex.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                    "'.");
            }
            else
            {
                return true;
            }
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
        /// Number of scalar arguments
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 NumberOfScalarArguments
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        public Int32 NumberOfVectorArguments
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
                return "sampleError";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "sample standard deviation of components of vector argument.";
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

    #region Subtotal

    /// <summary>
    /// Sum of range of components of vector argument
    /// </summary>
    internal class Subtotal : IVectorFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates vector function value
        /// </summary>
        /// <param name="arguments">
        /// Array of scalar arguments
        /// </param>
        /// <param name="vectorArguments">
        /// Array of arrays with arguments, i.e. array of vector arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            Double sum;

            sum = 0;
            for (Int32 i = System.Convert.ToInt32(arguments[0]); i <= System.Convert.ToInt32(arguments[1]); i++)
            {
                sum += vectorArguments[0][i];
            }
            return sum;
        }
        /// <summary>
        /// Returns if argument with specified index is a vector argument
        /// </summary>
        /// <param name="argumentIndex">
        /// Index of argument
        /// </param>
        /// <returns>
        /// True: Argument is a vector argument, 
        /// false: argument is a scalar argument
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if argumentIndex is invalid
        /// </exception>
        public Boolean IsVectorArgument(Int32 argumentIndex)
        {
            switch (argumentIndex)
            {
                case 0:
                    return true;
                case 1:
                    return false;
                case 2:
                    return false;
                default:
                    throw new MathCompilerException(
                        "Illegal argument index: '" +
                        argumentIndex.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                        "'.");
            }
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
                return 3;
            }
        }
        /// <summary>
        /// Number of scalar arguments
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 NumberOfScalarArguments
        {
            get
            {
                return 2;
            }
        }
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        public Int32 NumberOfVectorArguments
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
                return "subTotal";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "sum of range of components of vector argument: Scalar argument 1 = first component, scalar argument 2 = last component.";
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

    #region Sum

    /// <summary>
    /// Sum of components of vector argument
    /// </summary>
    internal class Sum : IVectorFunction
    {
        #region Class variables
        /// <summary>
        /// Token
        /// </summary>
        private Int32 myToken;
        #endregion

        #region Methods

        /// <summary>
        /// Calculates vector function value
        /// </summary>
        /// <param name="arguments">
        /// Array of scalar arguments
        /// </param>
        /// <param name="vectorArguments">
        /// Array of arrays with arguments, i.e. array of vector arguments
        /// </param>
        /// <returns>
        /// Function value for specified arguments
        /// </returns>
        public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            Double sum;

            sum = 0;
            for (Int32 i = 0; i < vectorArguments[0].Length; i++)
            {
                sum += vectorArguments[0][i];
            }
            return sum;
        }
        /// <summary>
        /// Returns if argument with specified index is a vector argument
        /// </summary>
        /// <param name="argumentIndex">
        /// Index of argument
        /// </param>
        /// <returns>
        /// True: Argument is a vector argument, 
        /// false: argument is a scalar argument
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if argumentIndex is invalid
        /// </exception>
        public Boolean IsVectorArgument(Int32 argumentIndex)
        {
            if (argumentIndex != 0)
            {
                throw new MathCompilerException(
                    "Illegal argument index: '" +
                    argumentIndex.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                    "'.");
            }
            else
            {
                return true;
            }
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
        /// Number of scalar arguments
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 NumberOfScalarArguments
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        public Int32 NumberOfVectorArguments
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
                return "sum";
            }
        }
        /// <summary>
        /// Description of function
        /// </summary>
        public String Description
        {
            get
            {
                return "sum of components of vector argument.";
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
