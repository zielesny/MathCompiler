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
    /// Interface for math compiler constant
    /// </summary>
    public interface IConstant
    {

        #region Properties

        /// <summary>
        /// Description of constant (not allowed to be null/empty)
        /// </summary>
        /// <value>
        /// Not null/empty
        /// </value>
        String Description
        {
            get;
        }
        /// <summary>
        /// Name of constant (not allowed to be null/empty)
        /// </summary>
        /// <value>
        /// Not null/empty
        /// </value>
        String Name
        {
            get;
        }
        /// <summary>
        /// Value of constant
        /// </summary>
        /// <value>
        /// Arbitrary
        /// </value>
        Double Value
        {
            get;
        }

        #endregion

    }
}
