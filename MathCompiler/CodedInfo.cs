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
    /// Coded information for classes MathCompiler
    /// </summary>
    public enum CodedInfo : int
    {
        /// <summary>
        /// Formula contains forbidden character.
        /// </summary>
        ForbiddenCharacter = 0,
        /// <summary>
        /// First token ' {0} ' is invalid.
        /// </summary>
        InvalidFirstToken = 1,
        /// <summary>
        /// Token ' {1} ' is not allowed to follow token ' {0} '.
        /// </summary>
        InvalidFollowToken = 2,
        /// <summary>
        /// Function ' {0} ' has {1} argument(s).
        /// </summary>
        InvalidFunctionArgumentCount = 3,
        /// <summary>
        /// Last token ' {0} ' is invalid.
        /// </summary>
        InvalidLastToken = 4,
        /// <summary>
        /// Invalid token: ' {0} '.
        /// </summary>
        InvalidToken = 5,
        /// <summary>
        /// Vector expression ' {0} ' is only allowed as an argument of a vector function.
        /// </summary>
        InvalidVectorExpression = 6,
        /// <summary>
        /// Closing bracket is missing.
        /// </summary>
        MissingClosingBracket = 7,
        /// <summary>
        /// Function ' {0} ' does not have a closing bracket.
        /// </summary>
        MissingFunctionClosingBracket = 8,
        /// <summary>
        /// No Formula defined.
        /// </summary>
        NoFormula = 9,
        /// <summary>
        /// Formula is null/empty. 
        /// </summary>
        FormulaNullEmpty = 10,
        /// <summary>
        /// Formula is successfully compiled. 
        /// </summary>
        SuccessfullyCompiled = 11,
        /// <summary>
        /// Unequal number of brackets: Open/Close = {0} / {1}.
        /// </summary>
        UnequalNumberOfBrackets = 12,
        /// <summary>
        /// Argument {0} of vector function ' {1} ' is a vector argument.
        /// </summary>
        MissingVectorArgument = 13,
        /// <summary>
        /// Argument {0} of vector function ' {1} ' is a scalar argument.
        /// </summary>
        MissingScalarArgument = 14,
        /// <summary>
        /// Not used
        /// </summary>
        NotUsed1 = 15,
        /// <summary>
        /// Invalid repeating quotation marks. 
        /// </summary>
        InvalidRepeatingQuotationMarks = 16,
        /// <summary>
        /// Not used
        /// </summary>
        NotUsed2 = 17,
        /// <summary>
        /// Conditional IF has 3 arguments.
        /// </summary>
        InvalidIfArgumentCount = 18,
        /// <summary>
        /// Conditional IF does not have a closing bracket.
        /// </summary>
        MissingIfClosingBracket = 19,
        /// <summary>
        /// Invalid token outside multi-parameter function: ' {0} '.
        /// </summary>
        InvalidTokenOutsideFormula = 20,
        /// <summary>
        /// Not used
        /// </summary>
        NotUsed3 = 21,
        /// <summary>
        /// Not used
        /// </summary>
        NotUsed4 = 22,
        /// <summary>
        /// Not used
        /// </summary>
        NotUsed5 = 23,
        /// <summary>
        /// Not used
        /// </summary>
        NotUsed6 = 24,
        /// <summary>
        /// Not used
        /// </summary>
        NotUsed7 = 25,
        /// <summary>
        ///  Not used
        /// </summary>
        NotUsed8 = 26,
        /// <summary>
        /// Invalid vector: Curly closing bracket is missing.
        /// </summary>
        InvalidVector = 27,
        /// <summary>
        /// Unequal number of curly brackets: Open/Close = {0} / {1}.
        /// </summary>
        UnequalNumberOfCurlyBrackets = 28,
        /// <summary>
        /// Illegal custom item.
        /// </summary>
        IllegalCustomItem = 29,
        /// <summary>
        /// Custom items successfully set.
        /// </summary>
        CustomItemsSuccessfullySet = 30,
        /// <summary>
        /// Illegal nested vector.
        /// </summary>
        IllegalNestedVector = 31
    }
}
