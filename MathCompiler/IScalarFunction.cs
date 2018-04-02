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
    /// Interface for math compiler function with scalar arguments only
    /// </summary>
    public interface IScalarFunction
    {

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
        Double Calculate(Double[] arguments);

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Properties

        /// <summary>
        /// Description of function (not allowed to be null/empty)
        /// </summary>
        /// <value>
        /// Not null/empty
        /// </value>
        String Description
        {
            get;
        }
        /// <summary>
        /// Name of function (not allowed to be null/empty)
        /// </summary>
        /// <value>
        /// Not null/empty
        /// </value>
        String Name
        {
            get;
        }
        /// <summary>
        /// Number of arguments
        /// </summary>
        /// <value>
        /// Greater 0
        /// </value>
        Int32 NumberOfArguments
        {
            get;
        }
        /// <summary>
        /// Token of function (internal variable)
        /// </summary>
        /// <value>
        /// Not null/empty
        /// </value>
        Int32 Token
        {
            get;
            set;
        }

        #endregion

    }
}
