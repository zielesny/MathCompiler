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
    /// Standard constants
    /// </summary>
    public static class StandardConstants
    {

        /// <summary>
        /// Returns standard constants
        /// </summary>
        /// <returns>Standard constants</returns>
        public static IConstant[] GetStandardConstants()
        {
            return new IConstant[]
            {
                new E(),
                new LogicalFalse(),
                new PI(),
                new LogicalTrue(),
                new Undefined()
            };
        }

    }

    #region Standard constant classes

    // Standard constants

    #region E

    /// <summary>
    /// Natural logarithmic base
    /// </summary>
    internal class E : IConstant
    {
        #region Properties

        /// <summary>
        /// Description of constant
        /// </summary>
        public String Description
        {
            get
            {
                return "natural logarithmic base.";
            }
        }
        /// <summary>
        /// Name of constant
        /// </summary>
        public String Name
        {
            get
            {
                return "e";
            }
        }
        /// <summary>
        /// Value of constant
        /// </summary>
        public Double Value
        {
            get
            {
                return Math.E;
            }
        }

        #endregion
    }

    #endregion

    #region LogicalFalse

    /// <summary>
    /// Double value representation of logical false
    /// </summary>
    internal class LogicalFalse : IConstant
    {
        #region Properties

        /// <summary>
        /// Description of constant
        /// </summary>
        public String Description
        {
            get
            {
                return "double value representation of logical false.";
            }
        }
        /// <summary>
        /// Name of constant
        /// </summary>
        public String Name
        {
            get
            {
                return "false";
            }
        }
        /// <summary>
        /// Value of constant
        /// </summary>
        public Double Value
        {
            get
            {
                return 0;
            }
        }

        #endregion
    }

    #endregion

    #region PI

    /// <summary>
    /// Mathematical constant pi (greek)
    /// </summary>
    internal class PI : IConstant
    {
        #region Properties

        /// <summary>
        /// Description of constant
        /// </summary>
        public String Description
        {
            get
            {
                return "mathematical constant pi (greek).";
            }
        }
        /// <summary>
        /// Name of constant
        /// </summary>
        public String Name
        {
            get
            {
                return "pi";
            }
        }
        /// <summary>
        /// Value of constant
        /// </summary>
        public Double Value
        {
            get
            {
                return Math.PI;
            }
        }

        #endregion
    }

    #endregion

    #region LogicalTrue

    /// <summary>
    /// Double value representation of logical true
    /// </summary>
    internal class LogicalTrue : IConstant
    {
        #region Properties

        /// <summary>
        /// Description of constant
        /// </summary>
        public String Description
        {
            get
            {
                return "double value representation of logical true.";
            }
        }
        /// <summary>
        /// Name of constant
        /// </summary>
        public String Name
        {
            get
            {
                return "true";
            }
        }
        /// <summary>
        /// Value of constant
        /// </summary>
        public Double Value
        {
            get
            {
                return 1;
            }
        }

        #endregion
    }

    #endregion

    #region Undefined

    /// <summary>
    /// Undefined value: Double.NaN
    /// </summary>
    internal class Undefined : IConstant
    {
        #region Properties

        /// <summary>
        /// Description of constant
        /// </summary>
        public String Description
        {
            get
            {
                return "double value representation of an undefined value.";
            }
        }
        /// <summary>
        /// Name of constant
        /// </summary>
        public String Name
        {
            get
            {
                return "undefined";
            }
        }
        /// <summary>
        /// Value of constant
        /// </summary>
        public Double Value
        {
            get
            {
                return Double.NaN;
            }
        }

        #endregion
    }

    #endregion

    #endregion

}
