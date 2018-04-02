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
    /// Coded information item
    /// </summary>
    public class CodedInfoItem
    {

        #region Private class variables

        /// <summary>
        /// Coded information
        /// </summary>
        private CodedInfo myCodedInfo;
        /// <summary>
        /// Coded information arguments
        /// </summary>
        private String[] myCodedInfoArguments;

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="codedInfo">
        /// Coded information
        /// </param>
        /// <param name="codedInfoArguments">
        /// Coded information arguments
        /// </param>
        public CodedInfoItem(CodedInfo codedInfo, String[] codedInfoArguments)
        {
            this.myCodedInfo = codedInfo;
            this.myCodedInfoArguments = codedInfoArguments;
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Public properties (get)

        /// <summary>
        /// Coded information arguments
        /// </summary>
        /// <value>
        /// May be null
        /// </value>
        public String[] CodedInfoArguments
        {
            get
            {
                return this.myCodedInfoArguments;
            }
        }
        /// <summary>
        /// Coded information
        /// </summary>
        public CodedInfo CodedInfo
        {
            get
            {
                return this.myCodedInfo;
            }
        }

        #endregion

    }
}
