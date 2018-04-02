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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MathCompiler
{
    #region Overview: Tokens, commands, operator precedence

    // ----------------------------------------------------------------------------------------------------------------
    //  Value               Symbol to parse          Tokens                   Commands
    //  -----------         ----------------------   ------------------       -----------------------------------------
    //
    // ...
    // myMaxVectorSubtermIndex                       Vector Subterm Set 5     VectorStack.Push(VectorSubterm[5] = VectorStackSupportList.ToArray)
    // -----
    // myMaxVectorSubtermIndex - 1                   Vector Subterm ...       VectorStack.Push(VectorSubterm[...])
    // ...
    // myMaxSubtermConstantSetIndex                  Vector Subterm 5         VectorStack.Push(VectorSubterm[5])
    // -----
    // myMaxSubtermConstantSetIndex - 1              SubtermConstant Set ...  SubtermConstant[...] = V
    // ...
    // myMaxSubtermConstantIndex                     SubtermConstant Set 5    SubtermConstant[5] = V
    // -----
    // myMaxSubtermConstantIndex - 1                 SubtermConstant ...      V = SubtermConstant[...] (myCommandsPush[...] = true: & Stack.Push)
    // ...
    // myMaxCalculatedConstantIndex                  SubtermConstant 5        V = SubtermConstant[0] (myCommandsPush[myMaxCalculatedConstantIndex] = true: & Stack.Push)
    // -----
    // myMaxCalculatedConstantIndex - 1              CalculatedConstant ...   V = CalculatedConstant[...] (myCommandsPush[...] = true: & Stack.Push)
    // ...
    // myMaxVectorArgumentIndex                      CalculatedConstant 5     V = CalculatedConstant[5] (myCommandsPush[myMaxVectorArgumentIndex] = true: & Stack.Push)
    // -----
    // myMaxVectorArgumentIndex - 1                  VectorArgument ...       VectorStack.Push(VectorArgument[...])
    // ...
    // myMaxVectorFunctionIndex                      VectorArgument 5         VectorStack.Push(VectorArgument[5])
    // -----
    // myMaxVectorFunctionIndex - 1                  VectorFunction ...       V = VectorFunction[...] (myCommandsPush[...] = true: & Stack.Push)
    // ...
    // myMaxFunctionIndex                            VectorFunction 10        V = VectorFunction[10] (myCommandsPush[myMaxFunctionIndex] = true: & Stack.Push)
    // -----
    // myMaxFunctionIndex - 1                        Function ...             V = Function[...] (myCommandsPush[...] = true: & Stack.Push)
    // ...
    // myMaxVectorConstantIndex                      Function 25              V = Function[15] (myCommandsPush[myMaxConstantIndex] = true: & Stack.Push)
    // -----
    // myMaxVectorConstantIndex - 1                  VectorConstant ...       VectorStack.Push(VectorConstant[...])
    // ...
    // myMaxConstantIndex                            VectorConstant 5         VectorStack.Push(VectorConstant[5])
    // -----
    // myMaxConstantIndex - 1                        Constant ...             V = Constant[...] (myCommandsPush[...] = true: & Stack.Push)
    // ...
    // myMaxScalarArgumentIndex                      Constant 15              V = Constant[15] (myCommandsPush[myMaxScalarArgumentIndex] = true: & Stack.Push)
    // -----
    // myMaxScalarArgumentIndex - 1                  X...                     V = X[...] (myCommandsPush[...] = true: & Stack.Push)
    // ...
    // 206 = myVariableScalarArgumentStartToken      X12                      V = X[12] (myCommandsPush[196] = true: & Stack.Push)
    // -----
    // 205 = myFixedVectorSubtermSetEndToken         Vector Subterm Set 4     VectorStack.Push(VectorSubterm[4] = VectorStackSupportList.ToArray)
    // 204                                           Vector Subterm Set 3     VectorStack.Push(VectorSubterm[3] = VectorStackSupportList.ToArray)
    // ... myFixedVectorSubtermSetCount = 5
    // 202                                           Vector Subterm Set 1     VectorStack.Push(VectorSubterm[1] = VectorStackSupportList.ToArray)
    // 201 = myFixedVectorSubtermSetStartToken       Vector Subterm Set 0     VectorStack.Push(VectorSubterm[0] = VectorStackSupportList.ToArray)
    // -----
    // 200 = myFixedVectorSubtermEndToken            Vector Subterm 4         VectorStack.Push(VectorSubterm[4])
    // 199                                           Vector Subterm 3         VectorStack.Push(VectorSubterm[3])
    // ... myFixedVectorSubtermCount = 5
    // 197                                           Vector Subterm 1         VectorStack.Push(VectorSubterm[1])
    // 196 = myFixedVectorSubtermStartToken          Vector Subterm 0         VectorStack.Push(VectorSubterm[0])
    // -----
    // 195 = myFixedVectorConstantEndToken           VectorConstant 4         VectorStack.Push(VectorConstant[4])
    // 194                                           VectorConstant 3         VectorStack.Push(VectorConstant[3])
    // ... myFixedVectorConstantCount = 5
    // 192                                           VectorConstant 1         VectorStack.Push(VectorConstant[1])
    // 191 = myFixedVectorConstantStartToken         VectorConstant 0         VectorStack.Push(VectorConstant[0])
    // -----
    // 190 = myFixedVectorArgumentEndToken           VectorArgument 4         VectorStack.Push(VectorArgument[4])
    // 189                                           VectorArgument 3         VectorStack.Push(VectorArgument[3])
    // ... myFixedVectorArgumentCount = 5
    // 187                                           VectorArgument 1         VectorStack.Push(VectorArgument[2])
    // 186 = myFixedVectorArgumentStartToken         VectorArgument 0         VectorStack.Push(VectorArgument[0])
    // -----
    // 185 = myFixedVectorFunctionEndToken           VectorFunction 9         V = VectorFunctionValue[9] & Stack.Push
    // 184                                           VectorFunction 9         V = VectorFunctionValue[9]
    // ... myFixedVectorFunctionCount = 10
    // 167                                           VectorFunction 0         V = VectorFunctionValue[0] & Stack.Push
    // 166 = myFixedVectorFunctionStartToken         VectorFunction 0         V = VectorFunctionValue[0]
    // -----
    // 165 = myFixedCalculatedConstantEndToken       CalculatedConstant 4     V = CalculatedConstant[4] & Stack.Push
    // 164                                           CalculatedConstant 4     V = CalculatedConstant[4]
    // ... myFixedCalculatedConstantCount = 5
    // 157                                           CalculatedConstant 0     V = CalculatedConstant[0] & Stack.Push
    // 156 = myFixedCalculatedConstantStartToken     CalculatedConstant 0     V = CalculatedConstant[0]
    // -----
    // 155 = myFixedSubtermConstantSetEndToken       Subterm Constant Set 4   SubtermConstant[4] = V
    // 154                                           Subterm Constant Set 3   SubtermConstant[3] = V
    // ... myFixedSubtermConstantSetCount = 5
    // 152                                           Subterm Constant Set 1   SubtermConstant[1] = V
    // 151 = myFixedSubtermConstantSetStartToken     Subterm Constant Set 0   SubtermConstant[0] = V
    // -----
    // 150 = myFixedSubtermConstantEndToken          Subterm Constant 4       V = SubtermConstant[4] & Stack.Push
    // 149                                           Subterm Constant 4       V = SubtermConstant[4]
    // ... myFixedSubtermConstantCount = 5
    // 142                                           Subterm Constant 0       V = SubtermConstant[0] & Stack.Push
    // 141 = myFixedSubtermConstantStartToken        Subterm Constant 0       V = SubtermConstant[0]
    // -----
    // 140 = myFixedFunctionEndToken                 Function 24              V = FunctionValue[24] & Stack.Push
    // 139                                           Function 24              V = FunctionValue[24]
    // ... myFixedFunctionCount = 25
    // 92                                            Function 0               V = FunctionValue[0] & Stack.Push
    // 91 = myFixedFunctionStartToken                Function 0               V = FunctionValue[0]
    // -----
    // 90 = myFixedConstantEndToken                  Constant 14              V = Constant[14] & Stack.Push
    // 89                                            Constant 14              V = Constant[14]
    // ... myFixedConstantCount = 15
    // 62                                            Constant 0               V = Constant[0] & Stack.Push
    // 61 = myFixedConstantStartToken                Constant 0               V = Constant[0]
    // -----
    // 60 = myFixedScalarArgumentEndToken            X11                      V = X[11] & Stack.Push
    // 59                                            X11                      V = X[11]
    // ...  myFixedScalarArgumentCount = 12
    // 34                                            X0                       V = X[0] & Stack.Push
    // 37 = myFixedScalarArgumentStartToken          X0                       V = X[0]
    // -----
    // 36                                            Vector evaluation related command: PushVectorStackSupportListToVectorStack:          VectorStack.Push(VectorStackSupportList.ToArray)
    // 35                                            Vector evaluation related command: AddVectorStackComponentsToVectorStackSupportList: VectorStackSupportList.Add(VectorStack.Pop)
    // 34                                            Vector evaluation related command: AddScalarValueToVectorStackSupportList:           VectorStackSupportList.Add(V)
    // 33                                            Vector evaluation related command: ClearNewVectorStackSupportList:                   VectorStackSupportList.Clear
    // 32                                            Jump                     Jump
    // 31                                            FalseJump                IF V = false THEN Jump
    // 30                                            Push                     Stack.Push
    // 29                                            ChangeSignPush           Stack.Push(-V)
    // 28                                            ChangeSign               V = -V
    // 27                                            NotPush                  Stack.Push(!Stack.Pop)
    // 26                   NOT                      Not                      V = !Stack.Pop
    // 25                                            PowerPush                Stack.Push(Math.Pow(Stack.Pop , V))
    // 24                   ^                        Power                    V = Math.Pow(Stack.Pop , V) 
    // 23                    	                     DividePush               Stack.Push(Stack.Pop / V)
    // 22                   /	                     Divide                   V = Stack.Pop / V
    // 21                                            MultiplyPush             Stack.Push(Stack.Pop * V)
    // 20                   *                        Multiply                 V = Stack.Pop * V
    // 19                                            SubtractPush             Stack.Push(Stack.Pop - V)
    // 18                   -                        Subtract                 V = Stack.Pop - V
    // 17                    	                     AddPush                  Stack.Push(Stack.Pop + V)
    // 16                   +	                     Add                      V = Stack.Pop + V
    // 15                     	                     GreaterPush              Stack.Push(Stack.Pop > V)
    // 14                   > 	                     Greater                  V = Stack.Pop > V
    // 13                    	                     GreaterEqualPush         Stack.Push(Stack.Pop >= V)
    // 12                   >=	                     GreaterEqual             V = Stack.Pop >= V
    // 11                    	                     LessEqualPush            Stack.Push(Stack.Pop <= V)
    // 10                   <=	                     LessEqual                V = Stack.Pop <= V
    // 9                    	                     LessPush                 Stack.Push(Stack.Pop < V)
    // 8                    <	                     Less                     V = Stack.Pop < V
    // 7                      	                     UnequalPush              Stack.Push(Stack.Pop != V)
    // 6                    <>	                     Unequal                  V = Stack.Pop != V
    // 5                     	                     EqualPush                Stack.Push(Stack.Pop = V)
    // 4                    =	                     Equal                    V = Stack.Pop = V
    // 3                                             OrPush                   Stack.Push(Stack.Pop || V)
    // 2                    OR                       Or                       V = Stack.Pop || V
    // 1                        	                 AndPush                  Stack.Push(Stack.Pop && V)
    // 0                    AND 	                 And                      V = Stack.Pop && V
    // -----
    // -1                   Indicator for variable scalar argument
    // -2                   Indicator for constant
    // -3                   Indicator for vector constant
    // -4                   Indicator for variable vector argument
    // -5                   Indicator for variable function
    // -6                   Indicator for variable vector function
    // -7                   (                        BracketOpen
    // -8                   )                        BracketClose
    // -9                   {                        CurlyBracketOpen
    // -10                  }                        CurlyBracketClose
    // -11                  ,                        Comma
    // -12                  IF                       IF
    // -13                                           FalseJumpEntry
    // -14                                           JumpEntry
    // 
    // ----------------------------------------------------------------------------------------------------------------
    // Boolean values:
    // ----------------------------------------------------------------------------------------------------------------
    // true  = 1
    // false = 0
    //
    // ----------------------------------------------------------------------------------------------------------------
    // Operator precedence:
    // ----------------------------------------------------------------------------------------------------------------
    // () or {}
    // function
    // Unary NOT,+,-
    // ^
    // *,/
    // +,-
    // <,<=,>=,>
    // =,<>
    // AND
    // OR
    // ----------------------------------------------------------------------------------------------------------------

    #endregion


    /// <summary>
    /// Mathematical function compiler
    /// </summary>
    public class MathCompiler
    {
        #region Private class variables

        #region Private constants

        #region Symbols

        /// <summary>
        /// Symbol
        /// </summary>
        private const String myVectorArgumentSymbol = "{}";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myBracketOpenSymbol = "(";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myBracketCloseSymbol = ")";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myCurlyBracketOpenSymbol = "{";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myCurlyBracketCloseSymbol = "}";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myLessSymbol = "<";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myLessEqualSymbol = "<=";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myGreaterSymbol = ">";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myGreaterEqualSymbol = ">=";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myEqualSymbol = "=";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myUnequalSymbol = "<>";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myAndSymbol = "AND";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myOrSymbol = "OR";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myNotSymbol = "NOT";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myAddSymbol = "+";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String mySubtractSymbol = "-";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myDivideSymbol = "/";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myMultiplySymbol = "*";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myPowerSymbol = "^";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myCommaSymbol = ",";
        /// <summary>
        /// Symbol
        /// </summary>
        private const String myIfSymbol = "IF";

        #endregion

        #region Symbol integer representations / commands

        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myJumpEntry = -14;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myFalseJumpEntry = -13;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myIf = -12;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myComma = -11;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myCurlyBracketClose = -10;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myCurlyBracketOpen = -9;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myBracketClose = -8;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myBracketOpen = -7;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myIndicatorVariableVectorFunction = -6;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myIndicatorVariableFunction = -5;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myIndicatorVariableVectorArgument = -4;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myIndicatorVectorConstant = -3;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myIndicatorConstant = -2;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myIndicatorVariableScalarArgument = -1;

        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myAnd = 0;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myAndPush = 1;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myOr = 2;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myOrPush = 3;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myEqual = 4;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myEqualPush = 5;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myUnequal = 6;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myUnequalPush = 7;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myLess = 8;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myLessPush = 9;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myLessEqual = 10;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myLessEqualPush = 11;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myGreaterEqual = 12;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myGreaterEqualPush = 13;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myGreater = 14;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myGreaterPush = 15;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myAdd = 16;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myAddPush = 17;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 mySubtract = 18;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 mySubtractPush = 19;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myMultiply = 20;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myMultiplyPush = 21;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myDivide = 22;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myDividePush = 23;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myPower = 24;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myPowerPush = 25;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myNot = 26;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myNotPush = 27;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myChangeSign = 28;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myChangeSignPush = 29;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myPush = 30;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myFalseJump = 31;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myJump = 32;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myClearNewVectorStackSupportList = 33;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myAddScalarValueToVectorStackSupportList = 34;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myAddVectorStackComponentsToVectorStackSupportList = 35;
        /// <summary>
        /// Integer representation / command
        /// </summary>
        private const Int32 myPushVectorStackSupportListToVectorStack = 36;

        #endregion

        #region Other constants

        /// <summary>
        /// Number of fixed scalar arguments
        /// </summary>
        private const Int32 myFixedScalarArgumentCount = 12;
        /// <summary>
        /// First token for fixed scalar arguments
        /// </summary>
        private const Int32 myFixedScalarArgumentStartToken = 37;
        /// <summary>
        /// Last token for fixed scalar arguments
        /// </summary>
        private const Int32 myFixedScalarArgumentEndToken = 60;

        /// <summary>
        /// Number of fixed constants
        /// </summary>
        private const Int32 myFixedConstantCount = 15;
        /// <summary>
        /// First token for fixed constants
        /// </summary>
        private const Int32 myFixedConstantStartToken = 61;
        /// <summary>
        /// Last token for fixed constants
        /// </summary>
        private const Int32 myFixedConstantEndToken = 90;

        /// <summary>
        /// Number of fixed functions
        /// </summary>
        private const Int32 myFixedFunctionCount = 25;
        /// <summary>
        /// First token for fixed functions
        /// </summary>
        private const Int32 myFixedFunctionStartToken = 91;
        /// <summary>
        /// Last token for fixed functions
        /// </summary>
        private const Int32 myFixedFunctionEndToken = 140;

        /// <summary>
        /// Number of fixed subterm constants
        /// </summary>
        private const Int32 myFixedSubtermConstantCount = 5;
        /// <summary>
        /// First token for fixed subterm constants
        /// </summary>
        private const Int32 myFixedSubtermConstantStartToken = 141;
        /// <summary>
        /// Last token for fixed subterm constants
        /// </summary>
        private const Int32 myFixedSubtermConstantEndToken = 150;

        /// <summary>
        /// Number of fixed subterm constants to be set
        /// </summary>
        private const Int32 myFixedSubtermConstantSetCount = 5;
        /// <summary>
        /// First token for fixed subterm constants to be set
        /// </summary>
        private const Int32 myFixedSubtermConstantSetStartToken = 151;
        /// <summary>
        /// Last token for fixed subterm constants to be set
        /// </summary>
        private const Int32 myFixedSubtermConstantSetEndToken = 155;

        /// <summary>
        /// Number of fixed calculated constants
        /// </summary>
        private const Int32 myFixedCalculatedConstantCount = 5;
        /// <summary>
        /// First token for fixed calculated constants
        /// </summary>
        private const Int32 myFixedCalculatedConstantStartToken = 156;
        /// <summary>
        /// Last token for fixed calculated constants
        /// </summary>
        private const Int32 myFixedCalculatedConstantEndToken = 165;

        /// <summary>
        /// Number of fixed vector functions
        /// </summary>
        private const Int32 myFixedVectorFunctionCount = 10;
        /// <summary>
        /// First token for fixed vector functions
        /// </summary>
        private const Int32 myFixedVectorFunctionStartToken = 166;
        /// <summary>
        /// Last token for fixed vector functions
        /// </summary>
        private const Int32 myFixedVectorFunctionEndToken = 185;

        /// <summary>
        /// Number of fixed vector arguments
        /// </summary>
        private const Int32 myFixedVectorArgumentCount = 5;
        /// <summary>
        /// First token for fixed vector arguments
        /// </summary>
        private const Int32 myFixedVectorArgumentStartToken = 186;
        /// <summary>
        /// Last token for fixed vector arguments
        /// </summary>
        private const Int32 myFixedVectorArgumentEndToken = 190;

        /// <summary>
        /// Number of fixed vector constants
        /// </summary>
        private const Int32 myFixedVectorConstantCount = 5;
        /// <summary>
        /// First token for fixed vector constants
        /// </summary>
        private const Int32 myFixedVectorConstantStartToken = 191;
        /// <summary>
        /// Last token for fixed vector constants
        /// </summary>
        private const Int32 myFixedVectorConstantEndToken = 195;

        /// <summary>
        /// Number of fixed vector subterms
        /// </summary>
        private const Int32 myFixedVectorSubtermCount = 5;
        /// <summary>
        /// First token for fixed vector subterms
        /// </summary>
        private const Int32 myFixedVectorSubtermStartToken = 196;
        /// <summary>
        /// Last token for fixed vector subterms
        /// </summary>
        private const Int32 myFixedVectorSubtermEndToken = 200;

        /// <summary>
        /// Number of fixed vector subterms to be set
        /// </summary>
        private const Int32 myFixedVectorSubtermSetCount = 5;
        /// <summary>
        /// First token for fixed vector subterms to be set
        /// </summary>
        private const Int32 myFixedVectorSubtermSetStartToken = 201;
        /// <summary>
        /// Last token for fixed vector subterms to be set
        /// </summary>
        private const Int32 myFixedVectorSubtermSetEndToken = 205;

        /// <summary>
        /// Start token for variable scalar arguments
        /// </summary>
        private const Int32 myVariableScalarArgumentStartToken = 206;

        /// <summary>
        /// Double representation for true
        /// </summary>
        private const Double myTrue = 1;
        /// <summary>
        /// Double representation for false
        /// </summary>
        private const Double myFalse = 0;

        #endregion

        #endregion

        #region Private class variables

        #region Reserved names

        /// <summary>
        /// Dictionary for reserved names
        /// </summary>
        private Dictionary<String, String> myReservedNameDictionary;

        #endregion

        #region Formula related class variables

        #region Basic formula related class variables

        /// <summary>
        /// Original passed formula to compile
        /// </summary>
        private String myFormula;
        /// <summary>
        /// True: Formula is successfully compiled; 
        /// false: Otherwise
        /// </summary>
        private Boolean myIsCompiled;
        /// <summary>
        /// Array with tokens of formula
        /// </summary>
        private Int32[] myTokens;
        /// <summary>
        /// Index for myTokens
        /// </summary>
        private Int32 myTokenIndex;

        /// <summary>
        /// Index for myCommands and myCommandRepresentations
        /// </summary>
        private Int32 myCommandIndex;
        /// <summary>
        /// Jump identifier;
        /// </summary>
        private Int32 myJumpIdentifier;
        /// <summary>
        /// True: Formula contains jump due to IF, 
        /// false: Otherwise
        /// </summary>
        private Boolean myHasJump;
        /// <summary>
        /// True: Formula contains vector, 
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVector;
        /// <summary>
        /// True: Formula contains nested vector, 
        /// false: Otherwise
        /// </summary>
        private Boolean myHasNestedVector;

        #endregion

        #region Compiler optimization related variables

        /// <summary>
        /// True: Internal stack operations will be optimized, 
        /// false: Otherwise
        /// </summary>
        private Boolean myIsStackPushOptimization;
        /// <summary>
        /// True: Identical subterms will be eliminated, 
        /// false: Otherwise
        /// </summary>
        private Boolean myIsIdenticalSubtermRecognition;
        /// <summary>
        /// True: Subterms that only consist of a priori constant values that 
        /// can a priori be evaluated to a resulting constant value will 
        /// be evalutated in advance, 
        /// false: Otherwise
        /// </summary>
        private Boolean myIsConstantSubExpressionRecognition;
        /// <summary>
        /// True: Identical vectors will be eliminated, 
        /// false: Otherwise
        /// </summary>
        private Boolean myIsIdenticalVectorRecognition;

        #endregion

        #region Related/corresponding arrays for final command execution and information

        /// <summary>
        /// Array with commands for formula
        /// </summary>
        private Int32[] myCommands;
        /// <summary>
        /// Jump offsets for command. 
        /// NOTE: myJumpOffsets[i] corresponds to myCommands[i]
        /// </summary>
        private Int32[] myJumpOffsets;
        /// <summary>
        /// Corrected command values for index access. 
        /// NOTE: myCommandsCorrectIndex[i] corresponds to myCommands[i]
        /// </summary>
        private Int32[] myCorrectIndexCommands;
        /// <summary>
        /// Array with Stack.Push() flags for commands. 
        /// NOTE: myCommandsPush[i] corresponds to myCommands[i]
        /// </summary>
        private Boolean[] myCommandsPush;
        /// <summary>
        /// Array with command representations. 
        /// NOTE: myCommandRepresentations[i] corresponds to myCommands[i]
        /// </summary>
        private String[] myCommandRepresentations;

        #endregion

        #region Related/corresponding arrays for creating final command execution

        /// <summary>
        /// Set of command arrays after identical subterm elimination
        /// </summary>
        private Int32[][] myCommandSet;
        /// <summary>
        /// Set of jump offset arrays. 
        /// NOTE: myJumpOffsetSet[i][j] corresponds to myCommandSet[i][j]
        /// </summary>
        private Int32[][] myJumpOffsetSet;
        /// <summary>
        /// Set of corrected command value arrays for index access after identical subterm elimination. 
        /// NOTE: myCorrectIndexCommandSet[i][j] corresponds to myCommandSet[i][j]
        /// </summary>
        private Int32[][] myCorrectIndexCommandSet;
        /// <summary>
        /// Set of arrays with Stack.Push() flags for commands after identical subterm elimination. 
        /// NOTE: myCommandPushSet[i][j] corresponds to myCommandSet[i][j]
        /// </summary>
        private Boolean[][] myCommandPushSet;
        /// <summary>
        /// Set of arrays with command representations after identical subterm elimination. 
        /// NOTE: myCommandRepresentationSet[i][j] corresponds to myCommandSet[i][j]
        /// </summary>
        private String[][] myCommandRepresentationSet;

        #endregion

        #endregion

        #region Comment related class variables

        /// <summary>
        /// Comment with details about success/failure of private operations
        /// </summary>
        private String myComment;
        /// <summary>
        /// Coded comment with details about success/failure of private operations
        /// </summary>
        private CodedInfoItem myCodedComment;
        /// <summary>
        /// Comment array
        /// </summary>
        private static String[] myComments = new String[] {
                // Formula comments:
                //  0 = Formula contains forbidden character.
                "Formula contains forbidden character.",
                //  1  = First token ' {0} ' is invalid.
                "First token ' {0} ' is invalid.",
                //  2 = Token ' {1} ' is not allowed to follow token ' {0} '.
                "Token ' {1} ' is not allowed to follow token ' {0} '.",
                //  3 = Function ' {0} ' has {1} argument(s).
                "Function ' {0} ' has {1} argument(s).",
                //  4 = Last token ' {0} ' is invalid.
                "Last token ' {0} ' is invalid.",
                //  5 = Invalid token: ' {0} '.
                "Invalid token: ' {0} '.",
                //  6 = Vector expression ' {0} ' is only allowed as an argument of a vector function.
                "Vector expression ' {0} ' is only allowed as an argument of a vector function.",
                //  7 = Closing bracket is missing.
                "Closing bracket is missing.",
                //  8 = Function ' {0} ' does not have a closing bracket.
                "Function ' {0} ' does not have a closing bracket.",
                //  9 = No Formula defined.
                "No Formula defined.",
                // 10 = Formula is null/empty.
                "Formula is null/empty.",
                // 11 = Formula is successfully compiled.
                "Formula is successfully compiled.",
                // 12 = Unequal number of brackets: Open/Close = {0} / {1}.
                "Unequal number of brackets: Open/Close = {0} / {1}.",
                // 13 = Argument {0} of vector function ' {1} ' is a vector argument.
                "Argument {0} of vector function ' {1} ' is a vector argument.",
                // 14 = Argument {0} of vector function ' {1} ' is a scalar argument.
                "Argument {0} of vector function ' {1} ' is a scalar argument.",
                // 15 = Not used
                "Not used",
                // 16 = Invalid repeating quotation marks.
                "Invalid repeating quotation marks.",
                // 17 = Not used
                "Not used",
                // 18 = Conditional IF has 3 arguments.
                "Conditional IF has 3 comma separated arguments.",
                // 19 = Conditional IF does not have a closing bracket.
                "Conditional IF does not have a closing bracket.",
                // 20 = Invalid token outside multi-parameter function: ' {0} '.
                "Invalid token outside multi-parameter function: ' {0} '.",
                // 21 = Invalid vector: Curly closing bracket is missing.
                "Invalid vector: Curly closing bracket is missing",
                // 22 = Unequal number of curly brackets: Open/Close = {0} / {1}.
                "Unequal number of curly brackets: Open/Close = {0} / {1}.",
                // 23 = Illegal custom item.
                "Illegal custom item.",
                // 24 = Custom items successfully set.
                "Custom items successfully set.",
                // 25 = Illegal nested vector.
                "Illegal nested vector."
            };

        #endregion

        #region Operator and reserved character related class variables

        /// <summary>
        /// Array with operator in ascending precedence
        /// </summary>
        // Operator precedence:
        // ()
        // function
        // Unary NOT,+,-
        // ^
        // *,/
        // +,-
        // <,<=,>=,>
        // =,<>
        // AND, OR
        private static String[] myOperatorSymbols = new String[] {
            myOrSymbol,
            myAndSymbol,
            myEqualSymbol,
            myUnequalSymbol,
            myLessSymbol,
            myLessEqualSymbol,
            myGreaterEqualSymbol,
            myGreaterSymbol,
            myAddSymbol,
            mySubtractSymbol,
            myMultiplySymbol,
            myDivideSymbol,
            myPowerSymbol,
            myNotSymbol};
        /// <summary>
        /// Array with reserved characters
        /// </summary>
        private static String[] myReservedSymbols = new String[] {
            myBracketOpenSymbol,
            myBracketCloseSymbol,
            myVectorArgumentSymbol};

        #endregion

        #region Scalar argument related class variables

        /// <summary>
        /// True: Formula has scalar arguments;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasScalarArguments;
        /// <summary>
        /// True: Formula has variable scalar arguments;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVariableScalarArguments;
        /// <summary>
        /// Number of scalar arguments
        /// </summary>
        private Int32 myNumberOfScalarArguments;

        #endregion

        #region Vector argument related class variables

        /// <summary>
        /// True: Formula has vector arguments;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVectorArguments;
        /// <summary>
        /// True: Formula has variable vector arguments;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVariableVectorArguments;
        /// <summary>
        /// Number of vector arguments
        /// </summary>
        private Int32 myNumberOfVectorArguments;

        #endregion

        #region Predefined constant related class variables
        /// <summary>
        /// Predefined constants
        /// </summary>
        private IConstant[] myPredefinedConstants;
        /// <summary>
        /// Predefined constant names
        /// </summary>
        private String[] myPredefinedConstantNames;
        /// <summary>
        /// Predefined constant descriptions
        /// </summary>
        private String[] myPredefinedConstantDescriptions;
        /// <summary>
        /// Predefined extended constant descriptions
        /// </summary>
        private String[] myPredefinedConstantExtendedDescriptions;
        /// <summary>
        /// Dictionary for predefined constant names
        /// </summary>
        private List<String> myPredefinedConstantNamesList;
        /// <summary>
        /// Dictionary with predefined constant names (keys) and corresponding values
        /// </summary>
        private Dictionary<String, Double> myPredefinedConstantValueDictionary;
        #endregion

        #region Constant related class variables

        /// <summary>
        /// Constant array
        /// </summary>
        private Double[] myConstants;
        /// <summary>
        /// True: Formula has constants;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasConstants;
        /// <summary>
        /// True: Formula has variable constants;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVariableConstants;

        #endregion

        #region Vector constant related class variables

        /// <summary>
        /// Array of arrays with vector constant
        /// </summary>
        private Double[][] myVectorConstants;
        /// <summary>
        /// Array of vector constant representations. 
        /// NOTE: myVectorConstantRepresentations[i] corresponds to myVectorConstants[i].
        /// </summary>
        private String[] myVectorConstantRepresentations;
        /// <summary>
        /// True: Formula has vector constants;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVectorConstants;
        /// <summary>
        /// True: Formula has variable vector constants;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVariableVectorConstants;

        #endregion

        #region Calculated constant related class variables

        /// <summary>
        /// Calculated constant array
        /// </summary>
        private Double[] myCalculatedConstants;
        /// <summary>
        /// True: Formula has variable calculated constants;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVariableCalculatedConstants;
        /// <summary>
        /// Contains representation of commands that were the basis for calculated constants: 
        /// myCalculatedConstantCommandsRepresentationSet[i] corresponds to detected calculated constant i. 
        /// NOTE: This dimension may not be equal to the dimension of myCalculatedConstants since 
        /// "different" calculated constants may have the same value!
        /// </summary>
        private String[][] myCalculatedConstantCommandsRepresentationSet;

        #endregion

        #region Subterm constant related class variables

        /// <summary>
        /// Subterm constants
        /// </summary>
        private Double[] mySubtermConstants;

        #endregion

        #region Vector subterm related class variables

        /// <summary>
        /// Vector subterms
        /// </summary>
        private Double[][] myVectorSubterms;

        #endregion

        #region Scalar function related class variables

        /// <summary>
        /// Functions
        /// </summary>
        private IScalarFunction[] myScalarFunctions;
        /// <summary>
        /// Scalar function variable for function calculation
        /// </summary>
        private IScalarFunction myScalarFunction;
        /// <summary>
        /// Dictionary with scalar function names (keys) and corresponding number of arguments
        /// </summary>
        private Dictionary<String, Int32> myScalarFunctionToNumberOfArgumentsDictionary;
        /// <summary>
        /// Dictionary with scalar function names (keys) and corresponding token value
        /// </summary>
        private Dictionary<String, Int32> myScalarFunctionTokenDictionary;
        /// <summary>
        /// Array with arguments for scalar functions
        /// </summary>
        private Double[] myScalarFunctionArguments;
        /// <summary>
        /// Array with names of scalar functions
        /// </summary>
        private String[] myScalarFunctionNames;
        /// <summary>
        /// Array with descriptions of scalar functions
        /// </summary>
        private String[] myScalarFunctionDescriptions;
        /// <summary>
        /// Array with extended descriptions of scalar functions
        /// </summary>
        private String[] myScalarFunctionExtendedDescriptions;
        /// <summary>
        /// True: Formula has scalar functions;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasScalarFunctions;
        /// <summary>
        /// True: Formula has variable scalar functions;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVariableScalarFunctions;

        #endregion

        #region Vector function related class variables

        /// <summary>
        /// Vector functions
        /// </summary>
        private IVectorFunction[] myVectorFunctions;
        /// <summary>
        /// Vector function variable for vector function calculation
        /// </summary>
        private IVectorFunction myVectorFunction;
        /// <summary>
        /// Dictionary with vector function names (keys) and corresponding number of arguments
        /// </summary>
        private Dictionary<String, Int32> myVectorFunctionToNumberOfArgumentsDictionary;
        /// <summary>
        /// Dictionary with vector function names (keys) and corresponding token value
        /// </summary>
        private Dictionary<String, Int32> myVectorFunctionTokenDictionary;
        /// <summary>
        /// Array with arguments for vector functions
        /// </summary>
        private Double[][] myVectorFunctionArguments;
        /// <summary>
        /// Array with names of arrayfunctions
        /// </summary>
        private String[] myVectorFunctionNames;
        /// <summary>
        /// Array with descriptions of vector functions
        /// </summary>
        private String[] myVectorFunctionDescriptions;
        /// <summary>
        /// Array with extended descriptions of vector functions
        /// </summary>
        private String[] myVectorFunctionExtendedDescriptions;
        /// <summary>
        /// True: Formula has vector functions;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVectorFunctions;
        /// <summary>
        /// True: Formula has variable vector functions;
        /// false: Otherwise
        /// </summary>
        private Boolean myHasVariableVectorFunctions;

        #endregion

        #region Calculation related class variables

        /// <summary>
        /// Calculation variable
        /// </summary>
        private Double myV;
        /// <summary>
        /// Stack for intermediate calculation results
        /// </summary>
        private Double[] myStack;
        /// <summary>
        /// Index for myStack
        /// </summary>
        private Int32 myStackIndex;
        /// <summary>
        /// Stack for intermediate vector arguments
        /// </summary>
        private Double[][] myVectorStack;
        /// <summary>
        /// Index for myVectorStack
        /// </summary>
        private Int32 myVectorStackIndex;
        /// <summary>
        /// Support array for evaluation of vectors
        /// </summary>
        private List<Double>[] myVectorStackSupportListArray;
        /// <summary>
        /// Index for myVectorStackSupportListArray
        /// </summary>
        private Int32 myVectorStackSupportListArrayIndex;
        /// <summary>
        /// Support index for vector evaluation
        /// </summary>
        private Int32 myVectorStackSupportIndex;
        /// <summary>
        /// Support index for vector evaluation
        /// </summary>
        private Int32 myVectorStackSupportArraysIndex;
        /// <summary>
        /// Arguments for vector stack
        /// </summary>
        private Double[][] myVectorStackArguments;
        /// <summary>
        /// Support arrays for vector stack for formulas with vectors but not nested vector
        /// </summary>
        private Double[][] myVectorStackSupportArrays;
        /// <summary>
        /// Maximum boundary for command token for variable scalar arguments
        /// </summary>
        private Int32 myMaxScalarArgumentIndex;
        /// <summary>
        /// Maximum boundary for command token for variable constants
        /// </summary>
        private Int32 myMaxConstantIndex;
        /// <summary>
        /// Maximum boundary for command token for variable vector constants
        /// </summary>
        private Int32 myMaxVectorConstantIndex;
        /// <summary>
        /// Maximum boundary for command token for variable functions
        /// </summary>
        private Int32 myMaxFunctionIndex;
        /// <summary>
        /// Maximum boundary for command token for variable vector functions
        /// </summary>
        private Int32 myMaxVectorFunctionIndex;
        /// <summary>
        /// Maximum boundary for command token for variable vector arguments
        /// </summary>
        private Int32 myMaxVectorArgumentIndex;
        /// <summary>
        /// Maximum boundary for command token for variable calculated constants
        /// </summary>
        private Int32 myMaxCalculatedConstantIndex;
        /// <summary>
        /// Maximum boundary for command token for variable subterm constants
        /// </summary>
        private Int32 myMaxSubtermConstantIndex;
        /// <summary>
        /// Maximum boundary for command token for variable subterm constants to be set
        /// </summary>
        private Int32 myMaxSubtermConstantSetIndex;
        /// <summary>
        /// Maximum boundary for command token for variable vector subterms
        /// </summary>
        private Int32 myMaxVectorSubtermIndex;
        /// <summary>
        /// Support array for transformation of commant token to correct array index
        /// </summary>
        private Int32[] myCorrectIndex;

        #endregion

        #endregion

        #region Private static regular expressions

        /// <summary>
        /// Regex for operator AND
        /// </summary>
        private static Regex myOperatorAndReplaceRegex = new Regex(
            @"\WAND\W", RegexOptions.Compiled);
        /// <summary>
        /// Regex for operator replacement correction
        /// </summary>
        // Correct for | at beginning and end
        private static Regex myOperatorCorrectRegex1 = new Regex(
            @"^\||\|$", RegexOptions.Compiled);
        /// <summary>
        /// Regex for operator replacement correction
        /// </summary>
        // Correct for multiple |
        private static Regex myOperatorCorrectRegex2 = new Regex(
            @"\|[|\s]+", RegexOptions.Compiled);
        /// <summary>
        /// Regex for operator replacement correction
        /// </summary>
        // Correct for <|=
        private static Regex myOperatorCorrectRegex3 = new Regex(
            @"\<\|+\=", RegexOptions.Compiled);
        /// <summary>
        /// Regex for operator replacement correction
        /// </summary>
        // Correct for >|=
        private static Regex myOperatorCorrectRegex4 = new Regex(
            @"\>\|+\=", RegexOptions.Compiled);
        /// <summary>
        /// Regex for operator replacement correction
        /// </summary>
        // Correct for <|>
        private static Regex myOperatorCorrectRegex5 = new Regex(
            @"\<\|+\>", RegexOptions.Compiled);
        /// <summary>
        /// Regex for curly bracket correction for vector arguments
        /// </summary>
        // Correct for |{||}|
        private static Regex myCurlyBracketCorrectRegex = new Regex(
            @"\|{\|\|}\|", RegexOptions.Compiled);
        /// <summary>
        /// Regex for scientific number correction
        /// </summary>
        // Correct for E|-|
        private static Regex myScientificNumberCorrectRegex1 = new Regex(
            @"E\|\-\|", RegexOptions.Compiled);
        /// <summary>
        /// Regex for scientific number correction
        /// </summary>
        // Correct for E|+|
        private static Regex myScientificNumberCorrectRegex2 = new Regex(
            @"E\|\+\|", RegexOptions.Compiled);
        /// <summary>
        /// Regex for operator NOT
        /// </summary>
        private static Regex myOperatorNotReplaceRegex = new Regex(
            @"^NOT\W|\WNOT\W", RegexOptions.Compiled);
        /// <summary>
        /// Regex for operator OR
        /// </summary>
        private static Regex myOperatorOrReplaceRegex = new Regex(
            @"\WOR\W", RegexOptions.Compiled);
        /// <summary>
        /// Regex for "whitespace" characters in formulas 
        /// (must correspond to myForbiddenCharactersForFormulaRegex plus whitespace and '#')
        /// </summary>
        private static Regex myWhitespaceCharatersRegex = new Regex(
            @"[^a-zA-Z\d\(\)\{\}\+\-\*\/\^\,\<\>\=\.\s#]", RegexOptions.Compiled);
        /// <summary>
        /// Regex for scalar argument detection: X0, ...
        /// </summary>
        private static Regex myScalarArgumentDetectionRegex = new Regex(
            @"^(x|X)\d+$", RegexOptions.Compiled);
        /// <summary>
        /// Regex for vector argument detection: X0{}, ...
        /// </summary>
        private static Regex myVectorArgumentDetectionRegex = new Regex(
            @"^(x|X)\d+\{\}$", RegexOptions.Compiled);
        /// <summary>
        /// Regex for advanced scalar argument or vector argument detection: X0, X0{} ...
        /// </summary>
        private static Regex myAdvancedArgumentDetectionRegex = new Regex(
            @"^(x|X)\d+\W|\W(x|X)\d+\W|\W(x|X)\d+\W*$", RegexOptions.Compiled);
        /// <summary>
        /// Regex for detection of vector constants
        /// </summary>
        private static Regex myVectorConstantDetectionRegex = new Regex(
            @"{([^{}]+)}", RegexOptions.Compiled);
        /// <summary>
        /// Regex for detection of transformed vector constants: #0, #1, ...
        /// </summary>
        private static Regex myTransformedVectorConstantDetectionRegex = new Regex(
            @"^#\d+$", RegexOptions.Compiled);
        /// <summary>
        /// Regex for detection of forbidden characters in formulas
        /// </summary>
        // Allowed characters are: 
        // a-z, A-Z, digits, '(', ')', '+', '-', '*', '/', '^', '\<', '>', '=', '.', \s (whitespace)
        private static Regex myForbiddenCharactersForFormulaRegex = new Regex(
            @"[^a-zA-Z\d\(\)\{\}\+\-\*\/\^\,\<\>\=\.\s]",
            RegexOptions.Compiled
        );

        #endregion

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Constructors

        /// <summary>
        /// Constructor with standard constants and scalar/vector functions plus all active optimization options
        /// </summary>
        public MathCompiler() : this(
            StandardScalarFunctions.GetStandardScalarFunctions(),
            StandardVectorFunctions.GetStandardVectorFunctions(),
            StandardConstants.GetStandardConstants(),
            true,
            true,
            true,
            true
        )
        {}
        /// <summary>
        /// Constructor with standard constants and scalar/vector functions
        /// </summary>
        /// <param name="isConstantSubExpressionRecognition">
        /// True: Constant subterms will be evalutated in advance, false: Otherwise
        /// </param>
        /// <param name="isIdenticalSubtermRecognition">
        /// True: Identical subterms will be eliminated, false: Otherwise
        /// </param>
        /// <param name="isStackPushOptimization">
        /// True: Internal stack operations will be optimized, false: Otherwise
        /// </param>
        /// <param name="isIdenticalVectorRecognition">
        /// True: Identical vectors will be eliminated, false: Otherwise
        /// </param>
        public MathCompiler(
            Boolean isConstantSubExpressionRecognition,
            Boolean isIdenticalSubtermRecognition,
            Boolean isStackPushOptimization,
            Boolean isIdenticalVectorRecognition
        ) : this(
            StandardScalarFunctions.GetStandardScalarFunctions(),
            StandardVectorFunctions.GetStandardVectorFunctions(),
            StandardConstants.GetStandardConstants(),
            isConstantSubExpressionRecognition,
            isIdenticalSubtermRecognition,
            isStackPushOptimization,
            isIdenticalVectorRecognition
        )
        {}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scalarFunctions">
        /// Functions with at least one scalar argument
        /// </param>
        /// <param name="vectorFunctions">
        /// Functions with at least one vector argument
        /// </param>
        /// <param name="constants">
        /// Constants
        /// </param>
        /// <param name="isConstantSubExpressionRecognition">
        /// True: Constant subterms will be evalutated in advance, false: Otherwise
        /// </param>
        /// <param name="isIdenticalSubtermRecognition">
        /// True: Identical subterms will be eliminated, false: Otherwise
        /// </param>
        /// <param name="isStackPushOptimization">
        /// True: Internal stack operations will be optimized, false: Otherwise
        /// </param>
        /// <param name="isIdenticalVectorRecognition">
        /// True: Identical vectors will be eliminated, false: Otherwise
        /// </param>
        public MathCompiler(
            IScalarFunction[] scalarFunctions,
            IVectorFunction[] vectorFunctions,
            IConstant[] constants,
            Boolean isConstantSubExpressionRecognition,
            Boolean isIdenticalSubtermRecognition,
            Boolean isStackPushOptimization,
            Boolean isIdenticalVectorRecognition
        )
        {

            #region Clear reserved names

            // NOTE: This is NOT to be changed in Initialize() method
            this.myReservedNameDictionary = new Dictionary<String, String>(4);
            // Add AND, OR, NOT, IF
            this.myReservedNameDictionary.Add(myAndSymbol, null);
            this.myReservedNameDictionary.Add(myOrSymbol, null);
            this.myReservedNameDictionary.Add(myNotSymbol, null);
            this.myReservedNameDictionary.Add(myIfSymbol, null);

            #endregion

            #region Activate all optimization options

            // NOTE: This is NOT to be changed in Initialize() method
            this.myIsConstantSubExpressionRecognition = isConstantSubExpressionRecognition;
            this.myIsIdenticalSubtermRecognition = isIdenticalSubtermRecognition;
            this.myIsStackPushOptimization = isStackPushOptimization;
            this.myIsIdenticalVectorRecognition = isIdenticalVectorRecognition;

            #endregion

            #region Initialize with standard constants and functions

            this.SetConstantsAndFunctions(
                scalarFunctions,
                vectorFunctions,
                constants
            );

            #endregion

        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Public methods

        #region Calculate

        /// <summary>
        /// Calculates function value of formula WITHOUT any arguments. Existence 
        /// of arguments can be checked with property HasArguments.
        /// </summary>
        /// <remarks>
        /// Unsafe implementation with pointers. There is no exception handling. Formula must already be successfully compiled 
        /// (to be checked with property IsCompiled).
        /// </remarks>
        /// <returns>
        /// Function value
        /// </returns>
        unsafe public Double Calculate()
        {
            return this.Calculate(null, null);
        }
        /// <summary>
        /// Calculates function value of formula WITHOUT vector arguments. Existence 
        /// of vector arguments can be checked with property HasVectorArguments.
        /// </summary>
        /// <remarks>
        /// Unsafe implementation with pointers. There is no exception handling. Formula must already be successfully compiled 
        /// (to be checked with property IsCompiled).
        /// </remarks>
        /// <param name="arguments">
        /// Array with arguments which must have at least dimension ScalarArgumentCount 
        /// (may be null: Then formula is NOT allowed to contain any scalar arguments, only constants)
        /// </param>
        /// <returns>
        /// Function value
        /// </returns>
        unsafe public Double Calculate(Double[] arguments)
        {
            return this.Calculate(arguments, null);
        }
        /// <summary>
        /// Calculates function value of formula
        /// </summary>
        /// <remarks>
        /// Unsafe implementation with pointers. There is no exception handling. Formula must already be successfully compiled 
        /// (to be checked with property IsCompiled).
        /// </remarks>
        /// <param name="arguments">
        /// Array with arguments which must have at least dimension ScalarArgumentCount 
        /// (may be null: Then formula is NOT allowed to contain any scalar arguments)
        /// </param>
        /// <param name="vectorArguments">
        /// Array with vector arguments which must have at least dimension VectorArgumentCount 
        /// (may be null: Then formula is NOT allowed to contain any vector arguments)
        /// </param>
        /// <returns>
        /// Function value
        /// </returns>
        unsafe public Double Calculate(Double[] arguments, Double[][] vectorArguments)
        {
            fixed (
                Double* pArguments = arguments,
                pConstants = this.myConstants,
                pCalculatedConstants = this.myCalculatedConstants,
                pSubtermConstants = this.mySubtermConstants,
                pFunctionArguments = this.myScalarFunctionArguments)
            {
                fixed (
                    Boolean* pCommandsPush = this.myCommandsPush)
                {
                    fixed (
                        Int32* pCommands = this.myCommands,
                        pCorrectIndexCommands = this.myCorrectIndexCommands,
                        pJumpOffsets = this.myJumpOffsets)
                    {
                        // Clear pointers and allocate stack memory
                        Boolean* pCurrentCommandPush = pCommandsPush;
                        Int32* pCurrentJumpOffset = pJumpOffsets;
                        Int32* pCurrentCorrectIndexCommand = pCorrectIndexCommands;
                        Int32* pCurrentCommand = pCommands;
                        Int32* pCommandLimit = pCommands + this.myCommands.Length;
                        // Allocate stack memory
                        Double* pStack = stackalloc Double[this.myStack.Length];
                        // IMPORTANT: Decrement stack pointer since stack operations start with stack increment
                        pStack--;
                        while (pCurrentCommand < pCommandLimit)
                        {
                            switch (*pCurrentCommand)
                            {
                                #region Execute commands

                                #region AND, OR (0-3)

                                case 0:
                                    // AND
                                    if (*pStack != myFalse && this.myV != myFalse)
                                    {
                                        this.myV = myTrue;
                                    }
                                    else
                                    {
                                        this.myV = myFalse;
                                    }
                                    pStack--;
                                    break;
                                case 1:
                                    // AND & Stack.Push
                                    if (*pStack != myFalse && this.myV != myFalse)
                                    {
                                        *pStack = myTrue;
                                    }
                                    else
                                    {
                                        *pStack = myFalse;
                                    }
                                    break;
                                case 2:
                                    // OR
                                    if (*pStack != myFalse || this.myV != myFalse)
                                    {
                                        this.myV = myTrue;
                                    }
                                    else
                                    {
                                        this.myV = myFalse;
                                    }
                                    pStack--;
                                    break;
                                case 3:
                                    // OR & Stack.Push
                                    if (*pStack != myFalse || this.myV != myFalse)
                                    {
                                        *pStack = myTrue;
                                    }
                                    else
                                    {
                                        *pStack = myFalse;
                                    }
                                    break;

                                #endregion

                                #region =, <> (4-7)

                                case 4:
                                    // Equal
                                    if (Double.IsNaN(*pStack) && Double.IsNaN(this.myV))
                                    {
                                        this.myV = myTrue;
                                        pStack--;
                                    }
                                    else
                                    {
                                        if (*pStack == this.myV)
                                        {
                                            this.myV = myTrue;
                                        }
                                        else
                                        {
                                            this.myV = myFalse;
                                        }
                                        pStack--;
                                    }
                                    break;
                                case 5:
                                    // Equal & Stack.Push
                                    if (Double.IsNaN(*pStack) && Double.IsNaN(this.myV))
                                    {
                                        *pStack = myTrue;
                                    }
                                    else
                                    {
                                        if (*pStack == this.myV)
                                        {
                                            *pStack = myTrue;
                                        }
                                        else
                                        {
                                            *pStack = myFalse;
                                        }
                                    }
                                    break;
                                case 6:
                                    // Unequal
                                    if (Double.IsNaN(*pStack) && Double.IsNaN(this.myV))
                                    {
                                        this.myV = myFalse;
                                        pStack--;
                                    }
                                    else
                                    {
                                        if (*pStack != this.myV)
                                        {
                                            this.myV = myTrue;
                                        }
                                        else
                                        {
                                            this.myV = myFalse;
                                        }
                                        pStack--;
                                    }
                                    break;
                                case 7:
                                    // Unequal & Stack.Push
                                    if (Double.IsNaN(*pStack) && Double.IsNaN(this.myV))
                                    {
                                        *pStack = myFalse;
                                    }
                                    else
                                    {
                                        if (*pStack != this.myV)
                                        {
                                            *pStack = myTrue;
                                        }
                                        else
                                        {
                                            *pStack = myFalse;
                                        }
                                    }
                                    break;

                                #endregion

                                #region <, <=, >=, > (8-15)

                                case 8:
                                    // Less
                                    if (*pStack < this.myV)
                                    {
                                        this.myV = myTrue;
                                    }
                                    else
                                    {
                                        this.myV = myFalse;
                                    }
                                    pStack--;
                                    break;
                                case 9:
                                    // Less & Stack.Push
                                    if (*pStack < this.myV)
                                    {
                                        *pStack = myTrue;
                                    }
                                    else
                                    {
                                        *pStack = myFalse;
                                    }
                                    break;
                                case 10:
                                    // LessEqual
                                    if (Double.IsNaN(*pStack) && Double.IsNaN(this.myV))
                                    {
                                        this.myV = myTrue;
                                        pStack--;
                                    }
                                    else
                                    {
                                        if (*pStack <= this.myV)
                                        {
                                            this.myV = myTrue;
                                        }
                                        else
                                        {
                                            this.myV = myFalse;
                                        }
                                        pStack--;
                                    }
                                    break;
                                case 11:
                                    // LessEqual & Stack.Push
                                    if (Double.IsNaN(*pStack) && Double.IsNaN(this.myV))
                                    {
                                        *pStack = myTrue;
                                    }
                                    else
                                    {
                                        if (*pStack <= this.myV)
                                        {
                                            *pStack = myTrue;
                                        }
                                        else
                                        {
                                            *pStack = myFalse;
                                        }
                                    }
                                    break;
                                case 12:
                                    // GreaterEqual
                                    if (Double.IsNaN(*pStack) && Double.IsNaN(this.myV))
                                    {
                                        this.myV = myTrue;
                                        pStack--;
                                    }
                                    else
                                    {
                                        if (*pStack >= this.myV)
                                        {
                                            this.myV = myTrue;
                                        }
                                        else
                                        {
                                            this.myV = myFalse;
                                        }
                                        pStack--;
                                    }
                                    break;
                                case 13:
                                    // GreaterEqual & Stack.Push
                                    if (Double.IsNaN(*pStack) && Double.IsNaN(this.myV))
                                    {
                                        *pStack = myTrue;
                                    }
                                    else
                                    {
                                        if (*pStack >= this.myV)
                                        {
                                            *pStack = myTrue;
                                        }
                                        else
                                        {
                                            *pStack = myFalse;
                                        }
                                    }
                                    break;
                                case 14:
                                    // Greater
                                    if (*pStack > this.myV)
                                    {
                                        this.myV = myTrue;
                                    }
                                    else
                                    {
                                        this.myV = myFalse;
                                    }
                                    pStack--;
                                    break;
                                case 15:
                                    // Greater & Stack.Push
                                    if (*pStack > this.myV)
                                    {
                                        *pStack = myTrue;
                                    }
                                    else
                                    {
                                        *pStack = myFalse;
                                    }
                                    break;

                                #endregion

                                #region +, - (16-19)

                                case 16:
                                    // Add
                                    this.myV += *pStack;
                                    pStack--;
                                    break;
                                case 17:
                                    // Add & Stack.Push
                                    *pStack += this.myV;
                                    break;
                                case 18:
                                    // Subtract
                                    this.myV = *pStack - this.myV;
                                    pStack--;
                                    break;
                                case 19:
                                    // Subtract & Stack.Push
                                    *pStack -= this.myV;
                                    break;

                                #endregion

                                #region *, / (20-23)

                                case 20:
                                    // Multiply
                                    this.myV *= *pStack;
                                    pStack--;
                                    break;
                                case 21:
                                    // Multiply & Stack.Push
                                    *pStack *= this.myV;
                                    break;
                                case 22:
                                    // Divide
                                    this.myV = *pStack / this.myV;
                                    pStack--;
                                    break;
                                case 23:
                                    // Divide & Stack.Push
                                    *pStack /= this.myV;
                                    break;

                                #endregion

                                #region ^ (24-25)

                                case 24:
                                    // Power
                                    this.myV = Math.Pow(*pStack, this.myV);
                                    pStack--;
                                    break;
                                case 25:
                                    // Power & Stack.Push
                                    *pStack = Math.Pow(*pStack, this.myV);
                                    break;

                                #endregion

                                #region Unary NOT, - (26-29)

                                case 26:
                                    // NOT
                                    if (this.myV == myFalse)
                                    {
                                        this.myV = myTrue;
                                    }
                                    else
                                    {
                                        this.myV = myFalse;
                                    }
                                    break;
                                case 27:
                                    // NOT & Stack.Push
                                    if (this.myV == myFalse)
                                    {
                                        pStack++; *pStack = myTrue;
                                    }
                                    else
                                    {
                                        pStack++; *pStack = myFalse;
                                    }
                                    break;
                                case 28:
                                    // ChangeSign
                                    this.myV = -this.myV;
                                    break;
                                case 29:
                                    // ChangeSign & Stack.Push
                                    pStack++; *pStack = -this.myV;
                                    break;

                                #endregion

                                #region Stack.Push (30)

                                case 30:
                                    // Stack.Push
                                    pStack++; *pStack = this.myV;
                                    break;

                                #endregion

                                #region FalseJump, Jump (31,32)

                                case 31:
                                    // FalseJump: IF myV == false THEN Jump
                                    if (this.myV == myFalse)
                                    {
                                        // Pointer arithmetics
                                        pCurrentCommand += *pCurrentJumpOffset;
                                        pCurrentCommandPush += *pCurrentJumpOffset;
                                        pCurrentCorrectIndexCommand += *pCurrentJumpOffset;
                                        pCurrentJumpOffset += *pCurrentJumpOffset;
                                    }
                                    break;
                                case 32:
                                    // Jump
                                    // Pointer arithmetics
                                    pCurrentCommand += *pCurrentJumpOffset;
                                    pCurrentCommandPush += *pCurrentJumpOffset;
                                    pCurrentCorrectIndexCommand += *pCurrentJumpOffset;
                                    pCurrentJumpOffset += *pCurrentJumpOffset;
                                    break;

                                #endregion

                                #region Vector evaluation related commands (33-36)

                                case 33:
                                    // VectorStackSupportList.Clear
                                    if (this.myHasNestedVector)
                                    {
                                        // Clear list for myVectorStackSupportListArray
                                        this.myVectorStackSupportListArray[++this.myVectorStackSupportListArrayIndex].Clear();
                                    }
                                    else
                                    {
                                        this.myVectorStackSupportIndex = 0;
                                        this.myVectorStackSupportArraysIndex++;
                                    }
                                    break;
                                case 34:
                                    // VectorStackSupportList.Add(V)
                                    if (this.myHasNestedVector)
                                    {
                                        // Add scalar component to list of myVectorStackSupportListArray
                                        this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex].Add(this.myV);
                                    }
                                    else
                                    {
                                        this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex][this.myVectorStackSupportIndex++] = this.myV;
                                    }
                                    break;
                                case 35:
                                    // VectorStackSupportList.Add(VectorStack.Pop):
                                    // Add vector stack components to list of myVectorStackSupportListArray
                                    for (Int32 supportIndex = 0; supportIndex < this.myVectorStack[this.myVectorStackIndex].Length; supportIndex++)
                                    {
                                        this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex].Add(
                                            this.myVectorStack[this.myVectorStackIndex][supportIndex]);
                                    }
                                    this.myVectorStackIndex--;
                                    break;
                                case 36:
                                    // VectorStack.Push(VectorStackSupportList.ToArray): 
                                    // Push list of myVectorStackSupportListArray to vector stack
                                    if (this.myHasNestedVector)
                                    {
                                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                                    }
                                    else
                                    {
                                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                                    }
                                    break;

                                #endregion

                                #region Fixed scalar argument X0-X11 (37-60)

                                case 37:
                                    // scalar argument
                                    this.myV = pArguments[0];
                                    break;
                                case 38:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[0];
                                    break;
                                case 39:
                                    // scalar argument
                                    this.myV = pArguments[1];
                                    break;
                                case 40:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[1];
                                    break;
                                case 41:
                                    // scalar argument
                                    this.myV = pArguments[2];
                                    break;
                                case 42:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[2];
                                    break;
                                case 43:
                                    // scalar argument
                                    this.myV = pArguments[3];
                                    break;
                                case 44:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[3];
                                    break;
                                case 45:
                                    // scalar argument
                                    this.myV = pArguments[4];
                                    break;
                                case 46:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[4];
                                    break;
                                case 47:
                                    // scalar argument
                                    this.myV = pArguments[5];
                                    break;
                                case 48:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[5];
                                    break;
                                case 49:
                                    // scalar argument
                                    this.myV = pArguments[6];
                                    break;
                                case 50:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[6];
                                    break;
                                case 51:
                                    // scalar argument
                                    this.myV = pArguments[7];
                                    break;
                                case 52:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[7];
                                    break;
                                case 53:
                                    // scalar argument
                                    this.myV = pArguments[8];
                                    break;
                                case 54:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[8];
                                    break;
                                case 55:
                                    // scalar argument
                                    this.myV = pArguments[9];
                                    break;
                                case 56:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[9];
                                    break;
                                case 57:
                                    // scalar argument
                                    this.myV = pArguments[10];
                                    break;
                                case 58:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[10];
                                    break;
                                case 59:
                                    // scalar argument
                                    this.myV = pArguments[11];
                                    break;
                                case 60:
                                    // scalar argument & Stack.Push
                                    pStack++; *pStack = pArguments[11];
                                    break;

                                #endregion

                                #region Fixed constant 0-14 (61-90)

                                case 61:
                                    // Constant
                                    this.myV = pConstants[0];
                                    break;
                                case 62:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[0];
                                    break;
                                case 63:
                                    // Constant
                                    this.myV = pConstants[1];
                                    break;
                                case 64:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[1];
                                    break;
                                case 65:
                                    // Constant
                                    this.myV = pConstants[2];
                                    break;
                                case 66:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[2];
                                    break;
                                case 67:
                                    // Constant
                                    this.myV = pConstants[3];
                                    break;
                                case 68:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[3];
                                    break;
                                case 69:
                                    // Constant
                                    this.myV = pConstants[4];
                                    break;
                                case 70:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[4];
                                    break;
                                case 71:
                                    // Constant
                                    this.myV = pConstants[5];
                                    break;
                                case 72:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[5];
                                    break;
                                case 73:
                                    // Constant
                                    this.myV = pConstants[6];
                                    break;
                                case 74:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[6];
                                    break;
                                case 75:
                                    // Constant
                                    this.myV = pConstants[7];
                                    break;
                                case 76:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[7];
                                    break;
                                case 77:
                                    // Constant
                                    this.myV = pConstants[8];
                                    break;
                                case 78:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[8];
                                    break;
                                case 79:
                                    // Constant
                                    this.myV = pConstants[9];
                                    break;
                                case 80:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[9];
                                    break;
                                case 81:
                                    // Constant
                                    this.myV = pConstants[10];
                                    break;
                                case 82:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[10];
                                    break;
                                case 83:
                                    // Constant
                                    this.myV = pConstants[11];
                                    break;
                                case 84:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[11];
                                    break;
                                case 85:
                                    // Constant
                                    this.myV = pConstants[12];
                                    break;
                                case 86:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[12];
                                    break;
                                case 87:
                                    // Constant
                                    this.myV = pConstants[13];
                                    break;
                                case 88:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[13];
                                    break;
                                case 89:
                                    // Constant
                                    this.myV = pConstants[14];
                                    break;
                                case 90:
                                    // Constant & Stack.Push
                                    pStack++; *pStack = pConstants[14];
                                    break;

                                #endregion

                                #region Fixed function 0-24 (91-140)

                                case 91:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[0].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[0].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 92:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[0].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[0].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 93:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[1].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[1].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 94:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[1].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[1].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 95:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[2].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[2].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 96:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[2].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[2].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 97:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[3].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[3].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 98:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[3].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[3].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 99:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[4].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[4].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 100:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[4].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[4].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 101:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[5].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[5].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 102:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[5].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[5].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 103:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[6].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[6].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 104:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[6].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[6].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 105:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[7].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[7].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 106:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[7].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[7].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 107:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[8].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[8].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 108:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[8].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[8].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 109:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[9].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[9].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 110:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[9].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[9].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 111:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[10].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[10].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 112:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[10].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[10].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 113:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[11].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[11].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 114:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[11].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[11].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 115:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[12].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[12].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 116:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[12].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[12].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 117:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[13].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[13].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 118:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[13].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[13].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 119:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[14].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[14].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 120:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[14].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[14].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 121:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[15].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[15].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 122:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[15].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[15].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 123:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[16].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[16].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 124:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[16].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[16].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 125:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[17].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[17].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 126:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[17].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[17].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 127:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[18].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[18].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 128:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[18].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[18].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 129:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[19].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[19].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 130:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[19].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[19].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 131:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[20].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[20].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 132:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[20].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[20].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 133:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[21].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[21].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 134:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[21].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[21].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 135:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[22].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[22].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 136:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[22].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[22].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 137:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[23].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[23].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 138:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[23].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[23].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 139:
                                    // Function
                                    for (Int32 k = this.myScalarFunctions[24].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    this.myV = this.myScalarFunctions[24].Calculate(this.myScalarFunctionArguments);
                                    break;
                                case 140:
                                    // Function & Stack.Push
                                    for (Int32 k = this.myScalarFunctions[24].NumberOfArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    pStack++; *pStack = this.myScalarFunctions[24].Calculate(this.myScalarFunctionArguments);
                                    break;

                                #endregion

                                #region Fixed subterm constant 0-4 (141-150)

                                case 141:
                                    // Subterm constant
                                    this.myV = pSubtermConstants[0];
                                    break;
                                case 142:
                                    // Subterm constant & Stack.Push
                                    pStack++; *pStack = pSubtermConstants[0];
                                    break;
                                case 143:
                                    // Subterm constant
                                    this.myV = pSubtermConstants[1];
                                    break;
                                case 144:
                                    // Subterm constant & Stack.Push
                                    pStack++; *pStack = pSubtermConstants[1];
                                    break;
                                case 145:
                                    // Subterm constant
                                    this.myV = pSubtermConstants[2];
                                    break;
                                case 146:
                                    // Subterm constant & Stack.Push
                                    pStack++; *pStack = pSubtermConstants[2];
                                    break;
                                case 147:
                                    // Subterm constant
                                    this.myV = pSubtermConstants[3];
                                    break;
                                case 148:
                                    // Subterm constant & Stack.Push
                                    pStack++; *pStack = pSubtermConstants[3];
                                    break;
                                case 149:
                                    // Subterm constant
                                    this.myV = pSubtermConstants[4];
                                    break;
                                case 150:
                                    // Subterm constant & Stack.Push
                                    pStack++; *pStack = pSubtermConstants[4];
                                    break;

                                #endregion

                                #region Fixed subterm constant to be set 0-4 (151-155)

                                case 151:
                                    // Subterm constant to be set
                                    pSubtermConstants[0] = this.myV;
                                    break;
                                case 152:
                                    // Subterm constant to be set
                                    pSubtermConstants[1] = this.myV;
                                    break;
                                case 153:
                                    // Subterm constant to be set
                                    pSubtermConstants[2] = this.myV;
                                    break;
                                case 154:
                                    // Subterm constant to be set
                                    pSubtermConstants[3] = this.myV;
                                    break;
                                case 155:
                                    // Subterm constant to be set
                                    pSubtermConstants[4] = this.myV;
                                    break;

                                #endregion

                                #region Fixed calculated constant 0-4 (156-165)

                                case 156:
                                    // Calculated constant
                                    this.myV = pCalculatedConstants[0];
                                    break;
                                case 157:
                                    // Calculated constant & Stack.Push
                                    pStack++; *pStack = pCalculatedConstants[0];
                                    break;
                                case 158:
                                    // Calculated constant
                                    this.myV = pCalculatedConstants[1];
                                    break;
                                case 159:
                                    // Calculated constant & Stack.Push
                                    pStack++; *pStack = pCalculatedConstants[1];
                                    break;
                                case 160:
                                    // Calculated constant
                                    this.myV = pCalculatedConstants[2];
                                    break;
                                case 161:
                                    // Calculated constant & Stack.Push
                                    pStack++; *pStack = pCalculatedConstants[2];
                                    break;
                                case 162:
                                    // Calculated constant
                                    this.myV = pCalculatedConstants[3];
                                    break;
                                case 163:
                                    // Calculated constant & Stack.Push
                                    pStack++; *pStack = pCalculatedConstants[3];
                                    break;
                                case 164:
                                    // Calculated constant
                                    this.myV = pCalculatedConstants[4];
                                    break;
                                case 165:
                                    // Calculated constant & Stack.Push
                                    pStack++; *pStack = pCalculatedConstants[4];
                                    break;

                                #endregion

                                #region Fixed vector function 0-9 (166-185)

                                case 166:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[0].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[0].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[0].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 167:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[0].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[0].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[0].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 168:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[1].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[1].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[1].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 169:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[1].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[1].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[1].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 170:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[2].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[2].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[2].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 171:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[2].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[2].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[2].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 172:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[3].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[3].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[3].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 173:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[3].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[3].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[3].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 174:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[4].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[4].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[4].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 175:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[4].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[4].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[4].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 176:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[5].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[5].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[5].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 177:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[5].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[5].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[5].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 178:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[6].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[6].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[6].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 179:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[6].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[6].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[6].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 180:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[7].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[7].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[7].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 181:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[7].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[7].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[7].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 182:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[8].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[8].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[8].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 183:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[8].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[8].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[8].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 184:
                                    // Vector function
                                    for (Int32 k = this.myVectorFunctions[9].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[9].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    this.myV = this.myVectorFunctions[9].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;
                                case 185:
                                    // Vector function & Stack.Push
                                    for (Int32 k = this.myVectorFunctions[9].NumberOfScalarArguments - 1; k >= 0; k--)
                                    { pFunctionArguments[k] = *pStack; pStack--; }
                                    for (Int32 k = this.myVectorFunctions[9].NumberOfVectorArguments - 1; k >= 0; k--)
                                        this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                    pStack++; *pStack = this.myVectorFunctions[9].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                    break;

                                #endregion

                                #region Fixed vector argument 0-4 (186-190)

                                case 186:
                                    // Vector argument (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[0];
                                    break;
                                case 187:
                                    // Vector argument (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[1];
                                    break;
                                case 188:
                                    // Vector argument (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[2];
                                    break;
                                case 189:
                                    // Vector argument (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[3];
                                    break;
                                case 190:
                                    // Vector argument (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[4];
                                    break;

                                #endregion

                                #region Fixed vector constant 0-4 (191-195)

                                case 191:
                                    // Vector constant (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[0];
                                    break;
                                case 192:
                                    // Vector constant (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[1];
                                    break;
                                case 193:
                                    // Vector constant (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[2];
                                    break;
                                case 194:
                                    // Vector constant (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[3];
                                    break;
                                case 195:
                                    // Vector constant (always pushed on VectorStack)
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[4];
                                    break;

                                #endregion

                                #region Fixed vector subterm 0-4 (196-200)

                                case 196:
                                    // VectorStack.Push(VectorSubterm[...])
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[0];
                                    break;
                                case 197:
                                    // VectorStack.Push(VectorSubterm[...])
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[1];
                                    break;
                                case 198:
                                    // VectorStack.Push(VectorSubterm[...])
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[2];
                                    break;
                                case 199:
                                    // VectorStack.Push(VectorSubterm[...])
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[3];
                                    break;
                                case 200:
                                    // VectorStack.Push(VectorSubterm[...])
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[4];
                                    break;

                                #endregion

                                #region Fixed vector subterm to be set 0-4 (201-205)

                                case 201:
                                    // VectorStack.Push(VectorSubterm[0] = VectorStackSupportList.ToArray)
                                    if (this.myHasNestedVector)
                                    {
                                        this.myVectorSubterms[0] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                                    }
                                    else
                                    {
                                        this.myVectorSubterms[0] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                                    }
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[0];
                                    break;
                                case 202:
                                    if (this.myHasNestedVector)
                                    {
                                        this.myVectorSubterms[1] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                                    }
                                    else
                                    {
                                        this.myVectorSubterms[1] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                                    }
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[1];
                                    break;
                                case 203:
                                    if (this.myHasNestedVector)
                                    {
                                        this.myVectorSubterms[2] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                                    }
                                    else
                                    {
                                        this.myVectorSubterms[2] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                                    }
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[2];
                                    break;
                                case 204:
                                    if (this.myHasNestedVector)
                                    {
                                        this.myVectorSubterms[3] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                                    }
                                    else
                                    {
                                        this.myVectorSubterms[3] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                                    }
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[3];
                                    break;
                                case 205:
                                    if (this.myHasNestedVector)
                                    {
                                        this.myVectorSubterms[4] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                                    }
                                    else
                                    {
                                        this.myVectorSubterms[4] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                                    }
                                    this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[4];
                                    break;

                                #endregion

                                default:

                                    if (*pCurrentCommand < this.myMaxScalarArgumentIndex)
                                    {
                                        #region Variable scalar argument

                                        if (*pCurrentCommandPush)
                                        {
                                            pStack++; *pStack = pArguments[*pCurrentCorrectIndexCommand];
                                        }
                                        else
                                        {
                                            this.myV = pArguments[*pCurrentCorrectIndexCommand];
                                        }

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxConstantIndex)
                                    {
                                        #region Variable constant

                                        if (*pCurrentCommandPush)
                                        {
                                            pStack++; *pStack = pConstants[*pCurrentCorrectIndexCommand];
                                        }
                                        else
                                        {
                                            this.myV = pConstants[*pCurrentCorrectIndexCommand];
                                        }

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxVectorConstantIndex)
                                    {
                                        #region Variable vector constant

                                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[*pCurrentCorrectIndexCommand];

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxFunctionIndex)
                                    {
                                        #region Variable function

                                        this.myScalarFunction = this.myScalarFunctions[*pCurrentCorrectIndexCommand];
                                        // Get function arguments from stack ...
                                        for (Int32 k = this.myScalarFunction.NumberOfArguments - 1; k >= 0; k--)
                                        {
                                            pFunctionArguments[k] = *pStack; pStack--;
                                        }
                                        // ... and calculate function value
                                        if (*pCurrentCommandPush)
                                        {
                                            pStack++; *pStack = this.myScalarFunction.Calculate(this.myScalarFunctionArguments);
                                        }
                                        else
                                        {
                                            this.myV = this.myScalarFunction.Calculate(this.myScalarFunctionArguments);
                                        }

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxVectorFunctionIndex)
                                    {
                                        #region Variable vector function

                                        this.myVectorFunction = this.myVectorFunctions[*pCurrentCorrectIndexCommand];
                                        // Get vector function arguments from stack ...
                                        for (Int32 k = this.myVectorFunction.NumberOfScalarArguments - 1; k >= 0; k--)
                                        { pFunctionArguments[k] = *pStack; pStack--; }
                                        for (Int32 k = this.myVectorFunction.NumberOfVectorArguments - 1; k >= 0; k--)
                                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                                        // ... and calculate function value
                                        if (*pCurrentCommandPush)
                                        {
                                            pStack++; *pStack = this.myVectorFunction.Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                        }
                                        else
                                        {
                                            this.myV = this.myVectorFunction.Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                                        }

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxVectorArgumentIndex)
                                    {
                                        #region Variable vector argument

                                        this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[*pCurrentCorrectIndexCommand];

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxCalculatedConstantIndex)
                                    {
                                        #region Variable calculated constant

                                        if (*pCurrentCommandPush)
                                        {
                                            pStack++; *pStack = pCalculatedConstants[*pCurrentCorrectIndexCommand];
                                        }
                                        else
                                        {
                                            this.myV = pCalculatedConstants[*pCurrentCorrectIndexCommand];
                                        }

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxSubtermConstantIndex)
                                    {
                                        #region Variable subterm constant

                                        if (*pCurrentCommandPush)
                                        {
                                            pStack++; *pStack = pSubtermConstants[*pCurrentCorrectIndexCommand];
                                        }
                                        else
                                        {
                                            this.myV = pSubtermConstants[*pCurrentCorrectIndexCommand];
                                        }

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxSubtermConstantSetIndex)
                                    {
                                        #region Variable subterm constant to be set

                                        pSubtermConstants[*pCurrentCorrectIndexCommand] = this.myV;

                                        #endregion
                                    }
                                    else if (*pCurrentCommand < this.myMaxVectorSubtermIndex)
                                    {
                                        #region Variable vector subterm

                                        // VectorStack.Push(VectorSubterm[...])
                                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[*pCurrentCorrectIndexCommand];

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Variable vector subterm to be set

                                        // VectorStack.Push(VectorSubterm[0] = VectorStackSupportList.ToArray)
                                        if (this.myHasNestedVector)
                                        {
                                            this.myVectorSubterms[*pCurrentCorrectIndexCommand] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                                        }
                                        else
                                        {
                                            this.myVectorSubterms[*pCurrentCorrectIndexCommand] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                                        }
                                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[*pCurrentCorrectIndexCommand];

                                        #endregion
                                    }
                                    break;

                                #endregion
                            }
                            // Increment pointers
                            pCurrentCommand++;
                            pCurrentCommandPush++;
                            pCurrentJumpOffset++;
                            pCurrentCorrectIndexCommand++;
                        }
                    }
                }
            }

            #region Re-initialize vector evaluation related index variables

            if (this.myHasVector)
            {
                // Initialize to 0 because there is NO increment at start
                this.myVectorStackSupportIndex = 0;
                // Initializes this.myVectorStackSupportArraysIndex to -1 since operations start with increment
                this.myVectorStackSupportArraysIndex = -1;
            }

            #endregion

            return this.myV;
        }
        /// <summary>
        /// Calculates function value of formula WITHOUT any arguments. Existence 
        /// of arguments can be checked with property HasArguments.
        /// </summary>
        /// <remarks>
        /// Safe implementation without pointers. There is no exception handling. Formula must already be successfully compiled 
        /// (to be checked with property IsCompiled).
        /// </remarks>
        /// <returns>
        /// Function value
        /// </returns>
        public Double CalculateSafe()
        {
            return this.CalculateSafe(null, null);
        }
        /// <summary>
        /// Calculates function value of formula WITHOUT vector arguments. Existence 
        /// of vector arguments can be checked with property HasVectorArguments.
        /// </summary>
        /// <remarks>
        /// Safe implementation without pointers. There is no exception handling. Formula must already be successfully compiled 
        /// (to be checked with property IsCompiled).
        /// </remarks>
        /// <param name="arguments">
        /// Array with arguments which must have at least dimension ScalarArgumentCount 
        /// (may be null: Then formula is NOT allowed to contain any scalar arguments, only constants)
        /// </param>
        /// <returns>
        /// Function value
        /// </returns>
        public Double CalculateSafe(Double[] arguments)
        {
            return this.CalculateSafe(arguments, null);
        }
        /// <summary>
        /// Calculates function value of formula
        /// </summary>
        /// <remarks>
        /// Safe implementation without pointers. There is no exception handling. Formula must already be successfully compiled 
        /// (to be checked with property IsCompiled).
        /// </remarks>
        /// <param name="arguments">
        /// Array with arguments which must have at least dimension ScalarArgumentCount 
        /// (may be null: Then formula is NOT allowed to contain any scalar arguments)
        /// </param>
        /// <param name="vectorArguments">
        /// Array with vector arguments which must have at least dimension VectorArgumentCount 
        /// (may be null: Then formula is NOT allowed to contain any vector arguments)
        /// </param>
        /// <returns>
        /// Function value
        /// </returns>
        public Double CalculateSafe(Double[] arguments, Double[][] vectorArguments)
        {
            Int32 i;

            i = 0;
            while (i < this.myCommands.Length)
            {
                switch (this.myCommands[i])
                {
                    #region Execute commands

                    #region AND, OR (0-3)

                    case 0:
                        // AND
                        if (this.myStack[this.myStackIndex--] != myFalse && this.myV != myFalse)
                        {
                            this.myV = myTrue;
                        }
                        else
                        {
                            this.myV = myFalse;
                        }
                        break;
                    case 1:
                        // AND & Stack.Push
                        if (this.myStack[this.myStackIndex] != myFalse && this.myV != myFalse)
                        {
                            this.myStack[this.myStackIndex] = myTrue;
                        }
                        else
                        {
                            this.myStack[this.myStackIndex] = myFalse;
                        }
                        break;
                    case 2:
                        // OR
                        if (this.myStack[this.myStackIndex--] != myFalse || this.myV != myFalse)
                        {
                            this.myV = myTrue;
                        }
                        else
                        {
                            this.myV = myFalse;
                        }
                        break;
                    case 3:
                        // OR & Stack.Push
                        if (this.myStack[this.myStackIndex] != myFalse || this.myV != myFalse)
                        {
                            this.myStack[this.myStackIndex] = myTrue;
                        }
                        else
                        {
                            this.myStack[this.myStackIndex] = myFalse;
                        }
                        break;

                    #endregion

                    #region =, <> (4-7)

                    case 4:
                        // Equal
                        if (Double.IsNaN(this.myStack[this.myStackIndex]) && Double.IsNaN(this.myV))
                        {
                            this.myV = myTrue;
                            this.myStackIndex--;
                        }
                        else
                        {
                            if (this.myStack[this.myStackIndex--] == this.myV)
                            {
                                this.myV = myTrue;
                            }
                            else
                            {
                                this.myV = myFalse;
                            }
                        }
                        break;
                    case 5:
                        // Equal & Stack.Push
                        if (Double.IsNaN(this.myStack[this.myStackIndex]) && Double.IsNaN(this.myV))
                        {
                            this.myStack[this.myStackIndex] = myTrue;
                        }
                        else
                        {
                            if (this.myStack[this.myStackIndex] == this.myV)
                            {
                                this.myStack[this.myStackIndex] = myTrue;
                            }
                            else
                            {
                                this.myStack[this.myStackIndex] = myFalse;
                            }
                        }
                        break;
                    case 6:
                        // Unequal
                        if (Double.IsNaN(this.myStack[this.myStackIndex]) && Double.IsNaN(this.myV))
                        {
                            this.myV = myFalse;
                            this.myStackIndex--;
                        }
                        else
                        {
                            if (this.myStack[this.myStackIndex--] != this.myV)
                            {
                                this.myV = myTrue;
                            }
                            else
                            {
                                this.myV = myFalse;
                            }
                        }
                        break;
                    case 7:
                        // Unequal & Stack.Push
                        if (Double.IsNaN(this.myStack[this.myStackIndex]) && Double.IsNaN(this.myV))
                        {
                            this.myStack[this.myStackIndex] = myFalse;
                        }
                        else
                        {
                            if (this.myStack[this.myStackIndex] != this.myV)
                            {
                                this.myStack[this.myStackIndex] = myTrue;
                            }
                            else
                            {
                                this.myStack[this.myStackIndex] = myFalse;
                            }
                        }
                        break;

                    #endregion

                    #region <, <=, >=, > (8-15)

                    case 8:
                        // Less
                        if (this.myStack[this.myStackIndex--] < this.myV)
                        {
                            this.myV = myTrue;
                        }
                        else
                        {
                            this.myV = myFalse;
                        }
                        break;
                    case 9:
                        // Less & Stack.Push
                        if (this.myStack[this.myStackIndex] < this.myV)
                        {
                            this.myStack[this.myStackIndex] = myTrue;
                        }
                        else
                        {
                            this.myStack[this.myStackIndex] = myFalse;
                        }
                        break;
                    case 10:
                        // LessEqual
                        if (this.myStack[this.myStackIndex--] <= this.myV)
                        {
                            this.myV = myTrue;
                        }
                        else
                        {
                            this.myV = myFalse;
                        }
                        break;
                    case 11:
                        // LessEqual & Stack.Push
                        if (this.myStack[this.myStackIndex] <= this.myV)
                        {
                            this.myStack[this.myStackIndex] = myTrue;
                        }
                        else
                        {
                            this.myStack[this.myStackIndex] = myFalse;
                        }
                        break;
                    case 12:
                        // GreaterEqual
                        if (this.myStack[this.myStackIndex--] >= this.myV)
                        {
                            this.myV = myTrue;
                        }
                        else
                        {
                            this.myV = myFalse;
                        }
                        break;
                    case 13:
                        // GreaterEqual & Stack.Push
                        if (this.myStack[this.myStackIndex] >= this.myV)
                        {
                            this.myStack[this.myStackIndex] = myTrue;
                        }
                        else
                        {
                            this.myStack[this.myStackIndex] = myFalse;
                        }
                        break;
                    case 14:
                        // Greater
                        if (this.myStack[this.myStackIndex--] > this.myV)
                        {
                            this.myV = myTrue;
                        }
                        else
                        {
                            this.myV = myFalse;
                        }
                        break;
                    case 15:
                        // Greater & Stack.Push
                        if (this.myStack[this.myStackIndex] > this.myV)
                        {
                            this.myStack[this.myStackIndex] = myTrue;
                        }
                        else
                        {
                            this.myStack[this.myStackIndex] = myFalse;
                        }
                        break;

                    #endregion

                    #region +, - (16-19)

                    case 16:
                        // Add
                        this.myV = this.myStack[this.myStackIndex--] + this.myV;
                        break;
                    case 17:
                        // Add & Stack.Push
                        this.myStack[this.myStackIndex] = this.myStack[this.myStackIndex] + this.myV;
                        break;
                    case 18:
                        // Subtract
                        this.myV = this.myStack[this.myStackIndex--] - this.myV;
                        break;
                    case 19:
                        // Subtract & Stack.Push
                        this.myStack[this.myStackIndex] = this.myStack[this.myStackIndex] - this.myV;
                        break;

                    #endregion

                    #region *, / (20-23)

                    case 20:
                        // Multiply
                        this.myV = this.myStack[this.myStackIndex--] * this.myV;
                        break;
                    case 21:
                        // Multiply & Stack.Push
                        this.myStack[this.myStackIndex] = this.myStack[this.myStackIndex] * this.myV;
                        break;
                    case 22:
                        // Divide
                        this.myV = this.myStack[this.myStackIndex--] / this.myV;
                        break;
                    case 23:
                        // Divide & Stack.Push
                        this.myStack[this.myStackIndex] = this.myStack[this.myStackIndex] / this.myV;
                        break;

                    #endregion

                    #region ^ (24-25)

                    case 24:
                        // Power
                        this.myV = Math.Pow(this.myStack[this.myStackIndex--], this.myV);
                        break;
                    case 25:
                        // Power & Stack.Push
                        this.myStack[this.myStackIndex] = Math.Pow(this.myStack[this.myStackIndex], this.myV);
                        break;

                    #endregion

                    #region Unary NOT, - (26-29)

                    case 26:
                        // NOT
                        if (this.myV == myFalse)
                        {
                            this.myV = myTrue;
                        }
                        else
                        {
                            this.myV = myFalse;
                        }
                        break;
                    case 27:
                        // NOT & Stack.Push
                        if (this.myV == myFalse)
                        {
                            this.myStack[++this.myStackIndex] = myTrue;
                        }
                        else
                        {
                            this.myStack[++this.myStackIndex] = myFalse;
                        }
                        break;
                    case 28:
                        // ChangeSign
                        this.myV = -this.myV;
                        break;
                    case 29:
                        // ChangeSign & Stack.Push
                        this.myStack[++this.myStackIndex] = -this.myV;
                        break;

                    #endregion

                    #region Stack.Push (30)

                    case 30:
                        // Stack.Push
                        this.myStack[++this.myStackIndex] = this.myV;
                        break;

                    #endregion

                    #region FalseJump, Jump (31,32)

                    case 31:
                        // FalseJump: IF myV == false THEN Jump
                        if (this.myV == myFalse)
                        {
                            i += this.myJumpOffsets[i];
                        }
                        break;
                    case 32:
                        // Jump
                        i += this.myJumpOffsets[i];
                        break;

                    #endregion

                    #region Vector evaluation related commands (33-36)

                    case 33:
                        // VectorStackSupportList.Clear
                        if (this.myHasNestedVector)
                        {
                            // Clear list for myVectorStackSupportListArray
                            this.myVectorStackSupportListArray[++this.myVectorStackSupportListArrayIndex].Clear();
                        }
                        else
                        {
                            this.myVectorStackSupportIndex = 0;
                            this.myVectorStackSupportArraysIndex++;
                        }
                        break;
                    case 34:
                        // VectorStackSupportList.Add(V)
                        if (this.myHasNestedVector)
                        {
                            // Add scalar component to list of myVectorStackSupportListArray
                            this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex].Add(this.myV);
                        }
                        else
                        {
                            this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex][this.myVectorStackSupportIndex++] = this.myV;
                        }
                        break;
                    case 35:
                        // VectorStackSupportList.Add(VectorStack.Pop):
                        // Add vector stack components to list of myVectorStackSupportListArray
                        for (Int32 supportIndex = 0; supportIndex < this.myVectorStack[this.myVectorStackIndex].Length; supportIndex++)
                        {
                            this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex].Add(
                                this.myVectorStack[this.myVectorStackIndex][supportIndex]);
                        }
                        this.myVectorStackIndex--;
                        break;
                    case 36:
                        // VectorStack.Push(VectorStackSupportList.ToArray): 
                        // Push list of myVectorStackSupportListArray to vector stack
                        if (this.myHasNestedVector)
                        {
                            this.myVectorStack[++this.myVectorStackIndex] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                        }
                        else
                        {
                            this.myVectorStack[++this.myVectorStackIndex] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                        }
                        break;

                    #endregion

                    #region Fixed scalar argument X0-X11 (37-60)

                    case 37:
                        // scalar argument
                        this.myV = arguments[0];
                        break;
                    case 38:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[0];
                        break;
                    case 39:
                        // scalar argument
                        this.myV = arguments[1];
                        break;
                    case 40:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[1];
                        break;
                    case 41:
                        // scalar argument
                        this.myV = arguments[2];
                        break;
                    case 42:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[2];
                        break;
                    case 43:
                        // scalar argument
                        this.myV = arguments[3];
                        break;
                    case 44:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[3];
                        break;
                    case 45:
                        // scalar argument
                        this.myV = arguments[4];
                        break;
                    case 46:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[4];
                        break;
                    case 47:
                        // scalar argument
                        this.myV = arguments[5];
                        break;
                    case 48:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[5];
                        break;
                    case 49:
                        // scalar argument
                        this.myV = arguments[6];
                        break;
                    case 50:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[6];
                        break;
                    case 51:
                        // scalar argument
                        this.myV = arguments[7];
                        break;
                    case 52:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[7];
                        break;
                    case 53:
                        // scalar argument
                        this.myV = arguments[8];
                        break;
                    case 54:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[8];
                        break;
                    case 55:
                        // scalar argument
                        this.myV = arguments[9];
                        break;
                    case 56:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[9];
                        break;
                    case 57:
                        // scalar argument
                        this.myV = arguments[10];
                        break;
                    case 58:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[10];
                        break;
                    case 59:
                        // scalar argument
                        this.myV = arguments[11];
                        break;
                    case 60:
                        // scalar argument & Stack.Push
                        this.myStack[++this.myStackIndex] = arguments[11];
                        break;

                    #endregion

                    #region Fixed constant 0-14 (61-90)

                    case 61:
                        // Constant
                        this.myV = this.myConstants[0];
                        break;
                    case 62:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[0];
                        break;
                    case 63:
                        // Constant
                        this.myV = this.myConstants[1];
                        break;
                    case 64:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[1];
                        break;
                    case 65:
                        // Constant
                        this.myV = this.myConstants[2];
                        break;
                    case 66:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[2];
                        break;
                    case 67:
                        // Constant
                        this.myV = this.myConstants[3];
                        break;
                    case 68:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[3];
                        break;
                    case 69:
                        // Constant
                        this.myV = this.myConstants[4];
                        break;
                    case 70:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[4];
                        break;
                    case 71:
                        // Constant
                        this.myV = this.myConstants[5];
                        break;
                    case 72:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[5];
                        break;
                    case 73:
                        // Constant
                        this.myV = this.myConstants[6];
                        break;
                    case 74:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[6];
                        break;
                    case 75:
                        // Constant
                        this.myV = this.myConstants[7];
                        break;
                    case 76:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[7];
                        break;
                    case 77:
                        // Constant
                        this.myV = this.myConstants[8];
                        break;
                    case 78:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[8];
                        break;
                    case 79:
                        // Constant
                        this.myV = this.myConstants[9];
                        break;
                    case 80:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[9];
                        break;
                    case 81:
                        // Constant
                        this.myV = this.myConstants[10];
                        break;
                    case 82:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[10];
                        break;
                    case 83:
                        // Constant
                        this.myV = this.myConstants[11];
                        break;
                    case 84:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[11];
                        break;
                    case 85:
                        // Constant
                        this.myV = this.myConstants[12];
                        break;
                    case 86:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[12];
                        break;
                    case 87:
                        // Constant
                        this.myV = this.myConstants[13];
                        break;
                    case 88:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[13];
                        break;
                    case 89:
                        // Constant
                        this.myV = this.myConstants[14];
                        break;
                    case 90:
                        // Constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myConstants[14];
                        break;

                    #endregion

                    #region Fixed function 0-24 (91-140)

                    case 91:
                        // Function
                        for (Int32 k = this.myScalarFunctions[0].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[0].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 92:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[0].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[0].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 93:
                        // Function
                        for (Int32 k = this.myScalarFunctions[1].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[1].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 94:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[1].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[1].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 95:
                        // Function
                        for (Int32 k = this.myScalarFunctions[2].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[2].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 96:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[2].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[2].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 97:
                        // Function
                        for (Int32 k = this.myScalarFunctions[3].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[3].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 98:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[3].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[3].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 99:
                        // Function
                        for (Int32 k = this.myScalarFunctions[4].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[4].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 100:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[4].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[4].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 101:
                        // Function
                        for (Int32 k = this.myScalarFunctions[5].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[5].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 102:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[5].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[5].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 103:
                        // Function
                        for (Int32 k = this.myScalarFunctions[6].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[6].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 104:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[6].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[6].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 105:
                        // Function
                        for (Int32 k = this.myScalarFunctions[7].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[7].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 106:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[7].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[7].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 107:
                        // Function
                        for (Int32 k = this.myScalarFunctions[8].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[8].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 108:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[8].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[8].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 109:
                        // Function
                        for (Int32 k = this.myScalarFunctions[9].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[9].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 110:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[9].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[9].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 111:
                        // Function
                        for (Int32 k = this.myScalarFunctions[10].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[10].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 112:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[10].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[10].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 113:
                        // Function
                        for (Int32 k = this.myScalarFunctions[11].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[11].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 114:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[11].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[11].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 115:
                        // Function
                        for (Int32 k = this.myScalarFunctions[12].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[12].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 116:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[12].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[12].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 117:
                        // Function
                        for (Int32 k = this.myScalarFunctions[13].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[13].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 118:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[13].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[13].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 119:
                        // Function
                        for (Int32 k = this.myScalarFunctions[14].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[14].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 120:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[14].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[14].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 121:
                        // Function
                        for (Int32 k = this.myScalarFunctions[15].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[15].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 122:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[15].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[15].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 123:
                        // Function
                        for (Int32 k = this.myScalarFunctions[16].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[16].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 124:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[16].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[16].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 125:
                        // Function
                        for (Int32 k = this.myScalarFunctions[17].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[17].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 126:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[17].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[17].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 127:
                        // Function
                        for (Int32 k = this.myScalarFunctions[18].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[18].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 128:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[18].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[18].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 129:
                        // Function
                        for (Int32 k = this.myScalarFunctions[19].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[19].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 130:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[19].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[19].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 131:
                        // Function
                        for (Int32 k = this.myScalarFunctions[20].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[20].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 132:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[20].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[20].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 133:
                        // Function
                        for (Int32 k = this.myScalarFunctions[21].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[21].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 134:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[21].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[21].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 135:
                        // Function
                        for (Int32 k = this.myScalarFunctions[22].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[22].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 136:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[22].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[22].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 137:
                        // Function
                        for (Int32 k = this.myScalarFunctions[23].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[23].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 138:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[23].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[23].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 139:
                        // Function
                        for (Int32 k = this.myScalarFunctions[24].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myV = this.myScalarFunctions[24].Calculate(this.myScalarFunctionArguments);
                        break;
                    case 140:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[24].NumberOfArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myScalarFunctions[24].Calculate(this.myScalarFunctionArguments);
                        break;

                    #endregion

                    #region Fixed subterm constant 0-4 (141-150)

                    case 141:
                        // Subterm constant
                        this.myV = this.mySubtermConstants[0];
                        break;
                    case 142:
                        // Subterm constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.mySubtermConstants[0];
                        break;
                    case 143:
                        // Subterm constant
                        this.myV = this.mySubtermConstants[1];
                        break;
                    case 144:
                        // Subterm constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.mySubtermConstants[1];
                        break;
                    case 145:
                        // Subterm constant
                        this.myV = this.mySubtermConstants[2];
                        break;
                    case 146:
                        // Subterm constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.mySubtermConstants[2];
                        break;
                    case 147:
                        // Subterm constant
                        this.myV = this.mySubtermConstants[3];
                        break;
                    case 148:
                        // Subterm constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.mySubtermConstants[3];
                        break;
                    case 149:
                        // Subterm constant
                        this.myV = this.mySubtermConstants[4];
                        break;
                    case 150:
                        // Subterm constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.mySubtermConstants[4];
                        break;

                    #endregion

                    #region Fixed subterm constant to be set 0-4 (151-155)

                    case 151:
                        // Subterm constant to be set
                        this.mySubtermConstants[0] = this.myV;
                        break;
                    case 152:
                        // Subterm constant to be set
                        this.mySubtermConstants[1] = this.myV;
                        break;
                    case 153:
                        // Subterm constant to be set
                        this.mySubtermConstants[2] = this.myV;
                        break;
                    case 154:
                        // Subterm constant to be set
                        this.mySubtermConstants[3] = this.myV;
                        break;
                    case 155:
                        // Subterm constant to be set
                        this.mySubtermConstants[4] = this.myV;
                        break;

                    #endregion

                    #region Fixed calculated constant 0-4 (156-165)

                    case 156:
                        // Calculated constant
                        this.myV = this.myCalculatedConstants[0];
                        break;
                    case 157:
                        // Calculated constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myCalculatedConstants[0];
                        break;
                    case 158:
                        // Calculated constant
                        this.myV = this.myCalculatedConstants[1];
                        break;
                    case 159:
                        // Calculated constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myCalculatedConstants[1];
                        break;
                    case 160:
                        // Calculated constant
                        this.myV = this.myCalculatedConstants[2];
                        break;
                    case 161:
                        // Calculated constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myCalculatedConstants[2];
                        break;
                    case 162:
                        // Calculated constant
                        this.myV = this.myCalculatedConstants[3];
                        break;
                    case 163:
                        // Calculated constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myCalculatedConstants[3];
                        break;
                    case 164:
                        // Calculated constant
                        this.myV = this.myCalculatedConstants[4];
                        break;
                    case 165:
                        // Calculated constant & Stack.Push
                        this.myStack[++this.myStackIndex] = this.myCalculatedConstants[4];
                        break;

                    #endregion

                    #region Fixed vector function 0-9 (166-185)

                    case 166:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[0].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[0].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[0].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 167:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[0].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[0].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[0].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 168:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[1].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[1].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[1].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 169:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[1].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[1].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[1].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 170:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[2].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[2].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[2].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 171:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[2].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[2].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[2].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 172:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[3].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[3].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[3].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 173:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[3].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[3].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[3].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 174:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[4].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[4].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[4].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 175:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[4].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[4].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[4].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 176:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[5].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[5].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[5].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 177:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[5].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[5].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[5].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 178:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[6].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[6].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[6].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 179:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[6].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[6].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[6].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 180:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[7].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[7].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[7].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 181:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[7].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[7].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[7].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 182:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[8].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[8].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[8].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 183:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[8].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[8].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[8].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 184:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[9].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[9].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myV = this.myVectorFunctions[9].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;
                    case 185:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[9].NumberOfScalarArguments - 1; k >= 0; k--)
                            this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                        for (Int32 k = this.myVectorFunctions[9].NumberOfVectorArguments - 1; k >= 0; k--)
                            this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                        this.myStack[++this.myStackIndex] = this.myVectorFunctions[9].Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                        break;

                    #endregion

                    #region Fixed vector argument 0-4 (186-190)

                    case 186:
                        // Vector argument (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[0];
                        break;
                    case 187:
                        // Vector argument (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[1];
                        break;
                    case 188:
                        // Vector argument (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[2];
                        break;
                    case 189:
                        // Vector argument (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[3];
                        break;
                    case 190:
                        // Vector argument (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[4];
                        break;

                    #endregion

                    #region Fixed vector constant 0-4 (191-195)

                    case 191:
                        // Vector constant (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[0];
                        break;
                    case 192:
                        // Vector constant (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[1];
                        break;
                    case 193:
                        // Vector constant (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[2];
                        break;
                    case 194:
                        // Vector constant (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[3];
                        break;
                    case 195:
                        // Vector constant (always pushed on VectorStack)
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[4];
                        break;

                    #endregion

                    #region Fixed vector subterm 0-4 (196-200)

                    case 196:
                        // VectorStack.Push(VectorSubterm[...])
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[0];
                        break;
                    case 197:
                        // VectorStack.Push(VectorSubterm[...])
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[1];
                        break;
                    case 198:
                        // VectorStack.Push(VectorSubterm[...])
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[2];
                        break;
                    case 199:
                        // VectorStack.Push(VectorSubterm[...])
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[3];
                        break;
                    case 200:
                        // VectorStack.Push(VectorSubterm[...])
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[4];
                        break;

                    #endregion

                    #region Fixed vector subterm to be set 0-4 (201-205)

                    case 201:
                        // VectorStack.Push(VectorSubterm[0] = VectorStackSupportList.ToArray)
                        if (this.myHasNestedVector)
                        {
                            this.myVectorSubterms[0] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                        }
                        else
                        {
                            this.myVectorSubterms[0] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                        }
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[0];
                        break;
                    case 202:
                        if (this.myHasNestedVector)
                        {
                            this.myVectorSubterms[1] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                        }
                        else
                        {
                            this.myVectorSubterms[1] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                        }
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[1];
                        break;
                    case 203:
                        if (this.myHasNestedVector)
                        {
                            this.myVectorSubterms[2] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                        }
                        else
                        {
                            this.myVectorSubterms[2] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                        }
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[2];
                        break;
                    case 204:
                        if (this.myHasNestedVector)
                        {
                            this.myVectorSubterms[3] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                        }
                        else
                        {
                            this.myVectorSubterms[3] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                        }
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[3];
                        break;
                    case 205:
                        if (this.myHasNestedVector)
                        {
                            this.myVectorSubterms[4] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                        }
                        else
                        {
                            this.myVectorSubterms[4] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                        }
                        this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[4];
                        break;

                    #endregion

                    default:

                        if (this.myCommands[i] < this.myMaxScalarArgumentIndex)
                        {
                            #region Variable scalar argument

                            if (this.myCommandsPush[i])
                            {
                                this.myStack[++this.myStackIndex] = arguments[this.myCorrectIndexCommands[i]];
                            }
                            else
                            {
                                this.myV = arguments[this.myCorrectIndexCommands[i]];
                            }

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxConstantIndex)
                        {
                            #region Variable constant

                            if (this.myCommandsPush[i])
                            {
                                this.myStack[++this.myStackIndex] = this.myConstants[this.myCorrectIndexCommands[i]];
                            }
                            else
                            {
                                this.myV = this.myConstants[this.myCorrectIndexCommands[i]];
                            }

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxVectorConstantIndex)
                        {
                            #region Variable vector constant

                            this.myVectorStack[++this.myVectorStackIndex] = this.myVectorConstants[this.myCorrectIndexCommands[i]];

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxFunctionIndex)
                        {
                            #region Variable function

                            this.myScalarFunction = this.myScalarFunctions[this.myCorrectIndexCommands[i]];
                            // Get function arguments from stack ...
                            for (Int32 k = this.myScalarFunction.NumberOfArguments - 1; k >= 0; k--)
                            {
                                this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                            }
                            // ... and calculate function value
                            if (this.myCommandsPush[i])
                            {
                                this.myStack[++this.myStackIndex] = this.myScalarFunction.Calculate(this.myScalarFunctionArguments);
                            }
                            else
                            {
                                this.myV = this.myScalarFunction.Calculate(this.myScalarFunctionArguments);
                            }

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxVectorFunctionIndex)
                        {
                            #region Variable vector function

                            this.myVectorFunction = this.myVectorFunctions[this.myCorrectIndexCommands[i]];
                            // Get vector function arguments from stack ...
                            for (Int32 k = this.myVectorFunction.NumberOfScalarArguments - 1; k >= 0; k--)
                                this.myScalarFunctionArguments[k] = this.myStack[this.myStackIndex--];
                            for (Int32 k = this.myVectorFunction.NumberOfVectorArguments - 1; k >= 0; k--)
                                this.myVectorFunctionArguments[k] = this.myVectorStack[this.myVectorStackIndex--];
                            // ... and calculate function value
                            if (this.myCommandsPush[i])
                            {
                                this.myStack[++this.myStackIndex] = this.myVectorFunction.Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                            }
                            else
                            {
                                this.myV = this.myVectorFunction.Calculate(this.myScalarFunctionArguments, this.myVectorFunctionArguments);
                            }

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxVectorArgumentIndex)
                        {
                            #region Variable vector argument

                            this.myVectorStack[++this.myVectorStackIndex] = vectorArguments[this.myCorrectIndexCommands[i]];

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxCalculatedConstantIndex)
                        {
                            #region Variable calculated constant

                            if (this.myCommandsPush[i])
                            {
                                this.myStack[++this.myStackIndex] = this.myCalculatedConstants[this.myCorrectIndexCommands[i]];
                            }
                            else
                            {
                                this.myV = this.myCalculatedConstants[this.myCorrectIndexCommands[i]];
                            }

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxSubtermConstantIndex)
                        {
                            #region Variable subterm constant

                            if (this.myCommandsPush[i])
                            {
                                this.myStack[++this.myStackIndex] = this.mySubtermConstants[this.myCorrectIndexCommands[i]];
                            }
                            else
                            {
                                this.myV = this.mySubtermConstants[this.myCorrectIndexCommands[i]];
                            }

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxSubtermConstantSetIndex)
                        {
                            #region Variable subterm constant to be set

                            this.mySubtermConstants[this.myCorrectIndexCommands[i]] = this.myV;

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxVectorSubtermIndex)
                        {
                            #region Variable vector subterm

                            // VectorStack.Push(VectorSubterm[...])
                            this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[this.myCorrectIndexCommands[i]];

                            #endregion
                        }
                        else
                        {
                            #region Variable vector subterm to be set

                            // VectorStack.Push(VectorSubterm[0] = VectorStackSupportList.ToArray)
                            if (this.myHasNestedVector)
                            {
                                this.myVectorSubterms[this.myCorrectIndexCommands[i]] = this.myVectorStackSupportListArray[this.myVectorStackSupportListArrayIndex--].ToArray();
                            }
                            else
                            {
                                this.myVectorSubterms[this.myCorrectIndexCommands[i]] = this.myVectorStackSupportArrays[this.myVectorStackSupportArraysIndex];
                            }
                            this.myVectorStack[++this.myVectorStackIndex] = this.myVectorSubterms[this.myCorrectIndexCommands[i]];

                            #endregion
                        }
                        break;

                    #endregion
                }
                i++;
            }

            #region Re-initialize vector evaluation related index variables

            if (this.myHasVector)
            {
                // Initialize to 0 because there is NO increment at start
                this.myVectorStackSupportIndex = 0;
                // Initializes this.myVectorStackSupportArraysIndex to -1 since operations start with increment
                this.myVectorStackSupportArraysIndex = -1;
            }

            #endregion

            return this.myV;
        }

        #endregion

        #region GetNumberOfArguments

        /// <summary>
        /// Returns number of arguments of specified scalar function
        /// </summary>
        /// <param name="nameOfScalarFunction">
        /// Name of scalar function
        /// </param>
        /// <returns>
        /// Number of arguments of specified scalar function (>= 0) 
        /// or -1 if function was not found
        /// </returns>
        public Int32 GetNumberOfArgumentsOfScalarFunction(String nameOfScalarFunction)
        {
            #region Checks

            if (String.IsNullOrEmpty(nameOfScalarFunction))
            {
                return -1;
            }

            #endregion

            if (this.myScalarFunctionToNumberOfArgumentsDictionary.ContainsKey(nameOfScalarFunction))
            {
                return this.myScalarFunctionToNumberOfArgumentsDictionary[nameOfScalarFunction];
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// Returns number of arguments of specified vector function
        /// </summary>
        /// <param name="nameOfVectorFunction">
        /// Name of vector function
        /// </param>
        /// <returns>
        /// Number of arguments of specified vector function (>= 0) 
        /// or -1 if function was not found
        /// </returns>
        public Int32 GetNumberOfArgumentsOfVectorFunction(String nameOfVectorFunction)
        {
            #region Checks

            if (String.IsNullOrEmpty(nameOfVectorFunction))
            {
                return -1;
            }

            #endregion

            if (this.myVectorFunctionToNumberOfArgumentsDictionary.ContainsKey(nameOfVectorFunction))
            {
                return this.myVectorFunctionToNumberOfArgumentsDictionary[nameOfVectorFunction];
            }
            else
            {
                return -1;
            }
        }

        #endregion

        #region SetFormula

        /// <summary>
        /// Sets formula
        /// </summary>
        /// <remarks>
        /// Details of possible problems related to the compilation process can be 
        /// accessed with properties Comment and CodedComment.
        /// </remarks>
        /// <param name="formula">
        /// Formula
        /// </param>
        /// <returns>
        /// True: Formula could successfully be compiled, 
        /// false: Otherwise. 
        /// See properties Comment and CodedComment for details.
        /// </returns>
        public Boolean SetFormula(String formula)
        {
            #region Checks

            if (String.IsNullOrEmpty(formula))
            {
                this.Initialize();
                return false;
            }

            #endregion

            #region Initialize

            this.Initialize();

            #endregion

            if (this.ConvertAndCompile(formula))
            {
                this.myIsCompiled = true;
                return true;
            }
            else
            {
                this.myIsCompiled = false;
                this.myFormula = null;
                return false;
            }
        }

        #endregion

        #endregion

        #region Public static methods

        #region ContainsForbiddenCharacter

        /// <summary>
        /// Checks if specified formula contains forbidden character.
        /// </summary>
        /// <remarks>
        /// Qutotation mark '"' is treated as a forbidden character. 
        /// This has to be taken into account if math compiler formulas are checked.
        /// </remarks>
        /// <param name="formula">
        /// Formula
        /// </param>
        /// <returns>
        /// True: Specified formula contains at least one forbidden character, 
        /// false: Otherwise
        /// </returns>
        public static Boolean ContainsForbiddenCharacter(string formula)
        {
            #region Checks

            if (String.IsNullOrEmpty(formula))
            {
                return false;
            }

            #endregion

            return MathCompiler.myForbiddenCharactersForFormulaRegex.IsMatch(formula);
        }

        #endregion

        #endregion

        #region Public properties (get)

        #region IsConstantSubExpressionRecognition (transform a priori constant subterms to calculated constants)

        /// <summary>
        /// True: Constant subterms will be evalutated in advance, false: Otherwise
        /// </summary>
        /// <value>
        /// true/false
        /// </value>
        public Boolean IsConstantSubExpressionRecognition
        {
            get
            {
                return this.myIsConstantSubExpressionRecognition;
            }
        }

        #endregion

        #region IsIdenticalSubtermRecognition (identical subterm elimination)

        /// <summary>
        /// True: Identical subterms will be eliminated, 
        /// false: Otherwise
        /// </summary>
        /// <value>
        /// true/false
        /// </value>
        public Boolean IsIdenticalSubtermRecognition
        {
            get
            {
                return this.myIsIdenticalSubtermRecognition;
            }
        }

        #endregion

        #region IsStackPushOptimization (optimization of Stack.Push() operations)

        /// <summary>
        /// True: Internal stack operations will be optimized, false: Otherwise
        /// </summary>
        /// <value>
        /// true/false
        /// </value>
        public Boolean IsStackPushOptimization
        {
            get
            {
                return this.myIsStackPushOptimization;
            }
        }

        #endregion

        #region IsIdenticalVectorRecognition (identical vector elimination)

        /// <summary>
        /// True: Identical vectors will be eliminated, false: Otherwise
        /// </summary>
        /// <value>
        /// true/false
        /// </value>
        public Boolean IsIdenticalVectorRecognition
        {
            get
            {
                return this.myIsIdenticalVectorRecognition;
            }
        }

        #endregion

        #region Comment

        #region CodedComment

        /// <summary>
        /// Coded comment with details about success/failure of private operations
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public CodedInfoItem CodedComment
        {
            get
            {
                return this.myCodedComment;
            }
        }

        #endregion

        #region Comment

        /// <summary>
        /// Comment with details about success/failure of private operations
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String Comment
        {
            get
            {
                return this.myComment;
            }
        }

        #endregion

        #endregion

        #region Names and descriptions

        #region ConstantDescriptions

        /// <summary>
        /// Constant descriptions (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] ConstantDescriptions
        {
            get
            {
                return this.myPredefinedConstantDescriptions;
            }
        }

        #endregion

        #region ConstantExtendedDescriptions

        /// <summary>
        /// Extended constant descriptions (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] ConstantExtendedDescriptions
        {
            get
            {
                return this.myPredefinedConstantExtendedDescriptions;
            }
        }

        #endregion

        #region ConstantNames

        /// <summary>
        /// Constant names (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] ConstantNames
        {
            get
            {
                return this.myPredefinedConstantNames;
            }
        }

        #endregion

        #region ScalarFunctionDescriptions

        /// <summary>
        /// Scalar function descriptions (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] ScalarFunctionDescriptions
        {
            get
            {
                return this.myScalarFunctionDescriptions;
            }
        }

        #endregion

        #region ScalarFunctionExtendedDescriptions

        /// <summary>
        /// Extended scalar function descriptions (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] ScalarFunctionExtendedDescriptions
        {
            get
            {
                return this.myScalarFunctionExtendedDescriptions;
            }
        }

        #endregion

        #region ScalarFunctionNames

        /// <summary>
        /// Scalar function names (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] ScalarFunctionNames
        {
            get
            {
                return this.myScalarFunctionNames;
            }
        }

        #endregion

        #region VectorFunctionDescriptions

        /// <summary>
        /// Vector function descriptions (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] VectorFunctionDescriptions
        {
            get
            {
                return this.myVectorFunctionDescriptions;
            }
        }

        #endregion

        #region VectorFunctionExtendedDescriptions

        /// <summary>
        /// Extended vector function descriptions (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] VectorFunctionExtendedDescriptions
        {
            get
            {
                return this.myVectorFunctionExtendedDescriptions;
            }
        }

        #endregion

        #region VectorFunctionNames

        /// <summary>
        /// Vector function names (not sorted)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public String[] VectorFunctionNames
        {
            get
            {
                return this.myVectorFunctionNames;
            }
        }

        #endregion

        #endregion

        #region Arguments

        #region HasArguments

        /// <summary>
        /// True: MathCompiler instance contains formula with scalar or vector arguments, 
        /// false: Otherwise
        /// </summary>
        /// <value>
        /// true/false
        /// </value>
        public Boolean HasArguments
        {
            get
            {
                return this.myHasScalarArguments || this.myHasVectorArguments;
            }
        }

        #endregion

        #region HasVectorArguments

        /// <summary>
        /// True: MathCompiler instance contains formula with vector arguments, 
        /// false: Otherwise
        /// </summary>
        /// <value>
        /// true/false
        /// </value>
        public Boolean HasVectorArguments
        {
            get
            {
                return this.myHasVectorArguments;
            }
        }

        #endregion

        #region HasScalarArguments

        /// <summary>
        /// True: MathCompiler instance contains formula with scalar arguments, 
        /// false: Otherwise
        /// </summary>
        /// <value>
        /// true/false
        /// </value>
        public Boolean HasScalarArguments
        {
            get
            {
                return this.myHasScalarArguments;
            }
        }

        #endregion

        #region ScalarArgumentCount

        /// <summary>
        /// Number of scalar arguments: X0,...,X'ScalarArgumentCount-1'
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 ScalarArgumentCount
        {
            get
            {
                return this.myNumberOfScalarArguments;
            }
        }

        #endregion

        #region VectorArgumentCount

        /// <summary>
        /// Number of vector arguments: X0{},...,X'VectorArgumentCount-1'{}
        /// </summary>
        /// <value>
        /// Greater or equal to 0
        /// </value>
        public Int32 VectorArgumentCount
        {
            get
            {
                return this.myNumberOfVectorArguments;
            }
        }

        #endregion

        #endregion

        #region Others

        #region Formula

        /// <summary>
        /// Formula of compiler
        /// </summary>
        /// <value>
        /// May be null
        /// </value>
        public String Formula
        {
            get
            {
                return this.myFormula;
            }
        }

        #endregion

        #region IsCompiled

        /// <summary>
        /// True: MathCompiler instance contains a successfully compiled formula, 
        /// false: Otherwise
        /// </summary>
        /// <value>
        /// true/false
        /// </value>
        public Boolean IsCompiled
        {
            get
            {
                return this.myIsCompiled;
            }
        }

        #endregion

        #endregion

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region NOTE
        //  Some private methods and properties are declared public for testing purposes
        #endregion

        #region Private methods

        #region Initialize

        /// <summary>
        /// Initializes math compiler
        /// </summary>
        /// <remarks>
        /// This method does NOT affect function or formula comment definitions.
        /// </remarks>
        private void Initialize()
        {
            #region Formula related class variables

            this.myFormula = null;

            //  9 = "No Formula defined.";
            this.myComment = MathCompiler.myComments[9];
            this.myCodedComment = new CodedInfoItem(CodedInfo.NoFormula, null);

            this.myIsCompiled = false;
            this.myTokens = null;
            this.myTokenIndex = 0;

            this.myCommandIndex = 0;
            this.myJumpIdentifier = -1;
            this.myHasJump = false;
            this.myHasVector = false;
            this.myHasNestedVector = false;

            this.myCommands = null;
            this.myJumpOffsets = null;
            this.myCorrectIndexCommands = null;
            this.myCommandsPush = null;
            this.myCommandRepresentations = null;

            this.myCommandSet = null;
            this.myJumpOffsetSet = null;
            this.myCorrectIndexCommandSet = null;
            this.myCommandPushSet = null;
            this.myCommandRepresentationSet = null;

            #endregion

            #region Scalar and vector function related class variables

            this.myHasScalarFunctions = false;
            this.myHasVariableScalarFunctions = false;
            this.myHasVectorFunctions = false;
            this.myHasVariableVectorFunctions = false;

            #endregion

            #region Constant related class variables

            this.myConstants = null;
            this.myHasConstants = false;
            this.myHasVariableConstants = false;

            #endregion

            #region Vector constant related class variables

            this.myVectorConstants = null;
            this.myVectorConstantRepresentations = null;
            this.myHasVectorConstants = false;
            this.myHasVariableVectorConstants = false;

            #endregion

            #region Calculated constant related class variables

            this.myCalculatedConstants = null;
            this.myHasVariableCalculatedConstants = false;
            this.myCalculatedConstantCommandsRepresentationSet = null;

            #endregion

            #region Scalar argument related class variables

            this.myHasScalarArguments = false;
            this.myHasVariableScalarArguments = false;
            this.myNumberOfScalarArguments = 0;

            #endregion

            #region Vector argument related class variables

            this.myHasVectorArguments = false;
            this.myHasVariableVectorArguments = false;
            this.myNumberOfVectorArguments = 0;

            #endregion

            #region Subterm constant related class variables

            this.mySubtermConstants = null;

            #endregion

            #region Vector subterm related class variables

            this.myVectorSubterms = null;

            #endregion
        }

        #endregion

        #region SetConstantsAndFunctions

        /// <summary>
        /// Sets constants and functions
        /// </summary>
        /// <param name="scalarFunctions">
        /// Functions with at least one scalar argument (not allowed to be null)
        /// </param>
        /// <param name="vectorFunctions">
        /// Functions with at least one vector argument (not allowed to be null)
        /// </param>
        /// <param name="constants">
        /// Constants (not allowed to be null)
        /// </param>
        /// <returns>
        /// True: Operation was successful, 
        /// false: Otherwise
        /// </returns>
        private Boolean SetConstantsAndFunctions(
            IScalarFunction[] scalarFunctions,
            IVectorFunction[] vectorFunctions,
            IConstant[] constants
        )
        {
            #region Checks

            if (constants == null ||
                scalarFunctions == null ||
                vectorFunctions == null)
            {
                return false;
            }

            #endregion

            try
            {
                // NOTE: These settings are NOT to be changed in Initialize() method
                this.myScalarFunctions = scalarFunctions;
                this.myVectorFunctions = vectorFunctions;
                this.SetFunctionRelatedVariables();
                this.myPredefinedConstants = constants;
                this.SetPredefinedConstantRelatedVariables();
                this.Initialize();
                // 24 = Custom items successfully set.
                this.myComment = MathCompiler.myComments[24];
                this.myCodedComment = new CodedInfoItem(CodedInfo.CustomItemsSuccessfullySet, null);
                return true;

            }
            catch (Exception)
            {
                this.Initialize();
                // 23 = Illegal custom item.
                this.myComment = MathCompiler.myComments[23];
                this.myCodedComment = new CodedInfoItem(CodedInfo.IllegalCustomItem, null);
                return false;
            }
        }

        #endregion

        #region Compilation related methods

        /// <summary>
        /// Converts and compiles formula
        /// </summary>
        /// <remarks>
        /// Details of possible problems related to the compilation process can be 
        /// accessed with properties Comment and CodedComment.
        /// </remarks>
        /// <param name="formula">
        /// Formula
        /// </param>
        /// <returns>
        /// True: Formula could successfully be compiled, 
        /// false: Otherwise. 
        /// See properties Comment and CodedComment for details.
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if formula is null/empty
        /// </exception>
        private Boolean ConvertAndCompile(String formula)
        {
            String[] tokenRepresentations;

            #region Checks

            if (String.IsNullOrEmpty(formula))
            {
                throw new MathCompilerException("Formula is null/empty.");
            }
            if (MathCompiler.myForbiddenCharactersForFormulaRegex.IsMatch(formula))
            {
                //  0 = "Formula contains forbidden character.";
                this.myComment = MathCompiler.myComments[0];
                this.myCodedComment = new CodedInfoItem(CodedInfo.ForbiddenCharacter, null);
                return false;
            }

            #endregion

            #region Set formula

            this.myFormula = formula;

            #endregion

            #region Evaluate vector constants if necessary

            String vectorConstantCorrectedFormula = this.myFormula;
            // Match vector constants
            MatchCollection matches = MathCompiler.myVectorConstantDetectionRegex.Matches(vectorConstantCorrectedFormula);
            if (matches.Count > 0)
            {
                List<Double[]> vectorConstantList = new List<double[]>(matches.Count);
                List<String> vectorConstantRepresentationList = new List<String>(matches.Count);
                for (Int32 i = 0; i < matches.Count; i++)
                {
                    Double[] vectorConstant = this.EvaluateVectorConstantRepresentation(matches[i].Groups[1].Value);
                    if (vectorConstant != null)
                    {
                        Int32 indexOfVectorConstant = vectorConstantList.Count;

                        #region Check dublettes

                        if (vectorConstantList.Count > 0)
                        {
                            for (Int32 k = 0; k < vectorConstantList.Count; k++)
                            {
                                if (vectorConstant.Length == vectorConstantList[k].Length)
                                {
                                    Boolean isDublette = true;
                                    for (Int32 j = 0; j < vectorConstant.Length; j++)
                                    {
                                        if (vectorConstant[j] != vectorConstantList[k][j])
                                        {
                                            isDublette = false;
                                            break;
                                        }
                                    }
                                    if (isDublette)
                                    {
                                        indexOfVectorConstant = k;
                                    }
                                }
                            }
                        }

                        #endregion

                        // IMPORTANT: Replace vector constant representation by forbidden (!) character '#' and index
                        vectorConstantCorrectedFormula = vectorConstantCorrectedFormula.Replace(
                            matches[i].Groups[0].Value,
                            "#" + indexOfVectorConstant.ToString().Trim());
                        if (indexOfVectorConstant == vectorConstantList.Count)
                        {
                            vectorConstantList.Add(vectorConstant);
                            vectorConstantRepresentationList.Add(matches[i].Groups[0].Value);
                        }
                    }
                    else
                    {
                        // Vector constant is not valid (may be a vector)
                        vectorConstantList = null;
                        break;
                    }
                }
                if (vectorConstantList == null)
                {
                    vectorConstantCorrectedFormula = this.myFormula;
                }
                else
                {
                    this.myVectorConstants = new Double[vectorConstantList.Count][];
                    vectorConstantList.CopyTo(this.myVectorConstants, 0);
                    this.myVectorConstantRepresentations = new String[vectorConstantRepresentationList.Count];
                    vectorConstantRepresentationList.CopyTo(this.myVectorConstantRepresentations, 0);
                }
            }

            #endregion

            // NOTE: Evaluation of vector constants must be performed BEFORE 
            //       conversion to token representations!

            #region Convert to token representations

            tokenRepresentations = this.SplitToTokenRepresentations(vectorConstantCorrectedFormula);
            if (tokenRepresentations == null || tokenRepresentations.Length == 0)
            {
                return false;
            }

            #endregion

            #region Convert token representations to integer tokens

            this.myTokens = this.ConvertToIntegerTokens(tokenRepresentations);
            if (this.myTokens == null)
            {
                return false;
            }

            #endregion

            #region Check integer tokens (i.e. validity of formula)

            if (!this.CheckTokens(this.myTokens))
            {
                return false;
            }

            #endregion

            #region Compile

            if (!this.Compile())
            {
                return false;
            }

            #endregion

            // Successful compilation
            return true;
        }
        /// <summary>
        /// Compiles myTokens into myCommands
        /// </summary>
        /// <remarks>
        /// myComment is changed if necessary
        /// </remarks>
        /// <returns>
        /// True: Formula could successfully be compiled, 
        /// false: Otherwise (details are given in myComment)
        /// </returns>
        private Boolean Compile()
        {
            Int32 initialSize;
            List<Int32> finalCommandList;
            List<Int32> finalJumpOffsetList;
            List<Int32> finalCorrectIndexCommandList;
            List<Boolean> finalCommandPushList;
            List<String> finalCommandRepresentationList;

            #region Checks

            if (this.myTokens == null || this.myTokens.Length == 0)
            {
                return false;
            }

            #endregion

            #region Intialize command related variables

            initialSize = 100;
            this.myCommandIndex = 0;
            this.myJumpIdentifier = -1;
            this.myHasJump = false;
            this.myHasVector = false;
            this.myHasNestedVector = false;
            this.myCommands = new Int32[initialSize];
            this.myJumpOffsets = new Int32[initialSize];
            this.myCorrectIndexCommands = new Int32[initialSize];
            this.myCommandsPush = new Boolean[initialSize];
            this.myCommandRepresentations = new String[initialSize];
            this.myTokenIndex = 0;

            #endregion

            #region Compile

            try
            {
                // Compile formula
                this.Compile_Or();
                
                #region Check if last detected token is a comma: Syntax error (i.e. return false)

                if (this.myTokenIndex < this.myTokens.Length && this.myTokens[this.myTokenIndex] == myComma)
                {
                    // 35 = "Invalid token outside multi-parameter function: ' {0} '.";
                    this.myComment = String.Format(CultureInfo.InvariantCulture, MathCompiler.myComments[20], myCommaSymbol);
                    this.myCodedComment = new CodedInfoItem(CodedInfo.InvalidTokenOutsideFormula, new String[] { myCommaSymbol });
                    return false;
                }

                #endregion
            }
            catch (MathCompilerException)
            {
                // Syntax error was detected: Details are given in myComment
                return false;
            }

            #endregion

            if (this.myCommandIndex > 0)
            {
                #region Fit myCommands and related arrays to necessary size

                MathCompiler.ShrinkArraySize<Int32>(ref this.myCommands, this.myCommandIndex);
                MathCompiler.ShrinkArraySize<Int32>(ref this.myJumpOffsets, this.myCommandIndex);
                MathCompiler.ShrinkArraySize<Int32>(ref this.myCorrectIndexCommands, this.myCommandIndex);
                MathCompiler.ShrinkArraySize<Boolean>(ref this.myCommandsPush, this.myCommandIndex);
                MathCompiler.ShrinkArraySize<String>(ref this.myCommandRepresentations, this.myCommandIndex);

                #endregion

                #region Optimization option 1: Evaluate constant subterms (calculated constants)

                if (this.myIsConstantSubExpressionRecognition)
                {
                    this.EvaluateAllConstantSubterms();
                }

                #endregion

                #region Create myCommandSet and related sets

                this.myCommandSet = new Int32[1][];
                this.myCommandSet[0] = this.myCommands;

                this.myJumpOffsetSet = new Int32[1][];
                this.myJumpOffsetSet[0] = this.myJumpOffsets;

                this.myCorrectIndexCommandSet = new Int32[1][];
                this.myCorrectIndexCommandSet[0] = this.myCorrectIndexCommands;

                this.myCommandPushSet = new Boolean[1][];
                this.myCommandPushSet[0] = this.myCommandsPush;

                this.myCommandRepresentationSet = new String[1][];
                this.myCommandRepresentationSet[0] = this.myCommandRepresentations;

                #endregion

                // NOTE: Elimination of identical subterms is ONLY to be performed AFTER 
                //       evaluation of constant subterms

                #region Optimization option 2: Eliminate identical subterms

                if (this.myIsIdenticalSubtermRecognition)
                {
                    this.EliminateIdenticalSubterms();
                    this.EliminateSingleUsedSubterms();
                }

                #endregion

                // NOTE: Optimization of Stack.Push() commands is ONLY to be performed AFTER 
                //       elimination of identical subterms

                #region Optimization option 3: Optimize Stack.Push() commands

                if (this.myIsStackPushOptimization)
                {
                    this.OptimizeStackPushCommands();
                }

                #endregion

                #region Create final single command array and related arrays

                // Capacity of 100 will be sufficient in most cases
                finalCommandList = new List<Int32>(100);
                finalJumpOffsetList = new List<Int32>(100);
                finalCorrectIndexCommandList = new List<Int32>(100);
                finalCommandPushList = new List<Boolean>(100);
                finalCommandRepresentationList = new List<String>(100);

                for (Int32 i = 0; i < this.myCommandSet.Length; i++)
                {
                    if (this.myHasJump)
                    {
                        #region Jump treatment necessary

                        for (Int32 k = 0; k < this.myCommandSet[i].Length; k++)
                        {
                            if (this.myJumpOffsetSet[i][k] < 0)
                            {
                                if (this.myCommandSet[i][k] == myFalseJump ||
                                    this.myCommandSet[i][k] == myJump)
                                {
                                    Int32 j = k + 1;
                                    Int32 counter = 0;
                                    while (this.myJumpOffsetSet[i][j] != this.myJumpOffsetSet[i][k])
                                    {
                                        counter++;
                                        j++;
                                    }
                                    // Ensure correct counting with regard to automatic 
                                    // incrementing in Calculate() method
                                    if (this.myCommandSet[i][k] == myJump)
                                    {
                                        finalJumpOffsetList.Add(counter - 1);
                                        finalCommandRepresentationList.Add(
                                            "Jump: Offset = " +
                                            finalJumpOffsetList[finalJumpOffsetList.Count - 1].ToString(CultureInfo.InvariantCulture.NumberFormat));
                                    }
                                    else
                                    {
                                        finalJumpOffsetList.Add(counter);
                                        finalCommandRepresentationList.Add(
                                            "FalseJump: Offset = " +
                                            finalJumpOffsetList[finalJumpOffsetList.Count - 1].ToString(CultureInfo.InvariantCulture.NumberFormat));
                                    }
                                    finalCommandList.Add(this.myCommandSet[i][k]);
                                    finalCorrectIndexCommandList.Add(this.myCorrectIndexCommandSet[i][k]);
                                    finalCommandPushList.Add(this.myCommandPushSet[i][k]);
                                }
                            }
                            else
                            {
                                finalCommandList.Add(this.myCommandSet[i][k]);
                                finalJumpOffsetList.Add(this.myJumpOffsetSet[i][k]);
                                finalCorrectIndexCommandList.Add(this.myCorrectIndexCommandSet[i][k]);
                                finalCommandPushList.Add(this.myCommandPushSet[i][k]);
                                finalCommandRepresentationList.Add(this.myCommandRepresentationSet[i][k]);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region No jump treatment

                        for (Int32 k = 0; k < this.myCommandSet[i].Length; k++)
                        {
                            finalCommandList.Add(this.myCommandSet[i][k]);
                            finalJumpOffsetList.Add(this.myJumpOffsetSet[i][k]);
                            finalCorrectIndexCommandList.Add(this.myCorrectIndexCommandSet[i][k]);
                            finalCommandPushList.Add(this.myCommandPushSet[i][k]);
                            finalCommandRepresentationList.Add(this.myCommandRepresentationSet[i][k]);
                        }

                        #endregion
                    }

                    #region Set subterm constant if necessary

                    if (i < this.myCommandSet.Length - 1)
                    {
                        if (i < myFixedSubtermConstantSetCount)
                        {
                            #region Fixed subterm constant to be set

                            Int32 currentCommand = myFixedSubtermConstantSetStartToken + i;
                            finalCommandList.Add(currentCommand);
                            finalJumpOffsetList.Add(0);
                            finalCorrectIndexCommandList.Add(-1);
                            finalCommandPushList.Add(false);
                            finalCommandRepresentationList.Add(
                                "SubtermConstant[" +
                                i.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                                "] = V" + " (" + currentCommand.ToString() + ")");

                            #endregion
                        }
                        else
                        {
                            #region Variable subterm constant to be set

                            this.myMaxSubtermConstantSetIndex = this.myMaxSubtermConstantIndex - myFixedSubtermConstantSetCount + i + 1;
                            this.myMaxVectorSubtermIndex = this.myMaxSubtermConstantSetIndex;
                            Int32 currentCommand = this.myMaxSubtermConstantIndex - myFixedSubtermConstantSetCount + i;
                            finalCommandList.Add(currentCommand);
                            finalJumpOffsetList.Add(0);
                            finalCorrectIndexCommandList.Add(i);
                            finalCommandPushList.Add(false);
                            finalCommandRepresentationList.Add(
                                "SubtermConstant[" +
                                i.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                                "] = V" + " (" + currentCommand.ToString() + ")");

                            #endregion
                        }
                    }

                    #endregion
                }
                this.myCommands = finalCommandList.ToArray();
                this.myJumpOffsets = finalJumpOffsetList.ToArray();
                this.myCorrectIndexCommands = finalCorrectIndexCommandList.ToArray();
                this.myCommandsPush = finalCommandPushList.ToArray();
                this.myCommandRepresentations = finalCommandRepresentationList.ToArray();

                #endregion

                // NOTE: Elimination of identical vectors must be the LAST optimization operation and
                //       is only to be performed if constant subexpression evaluation and identical subterm
                //       elimination are ALREADY performed. This last optimization step operates on the 
                //       final command array.

                #region Optimization option 4: Eliminate identical vectors

                if (this.myHasVector &&
                    this.myIsConstantSubExpressionRecognition &&
                    this.myIsIdenticalSubtermRecognition &&                    
                    this.myIsIdenticalVectorRecognition)
                {
                    this.EliminateIdenticalVectors();
                }

                #endregion

                #region Clear myStack, myVectorStack and myVectorStackArguments

                this.InitializeStack();

                #endregion

                #region Set final comment

                // Set final myComment: 
                // 11 = "Formula is successfully compiled.";
                this.myComment = MathCompiler.myComments[11];
                this.myCodedComment = new CodedInfoItem(CodedInfo.SuccessfullyCompiled, null);

                #endregion

                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Compiler method: Operation OR
        /// </summary>
        private void Compile_Or()
        {
            // Get first argument
            this.Compile_And();
            while (this.myTokenIndex < this.myTokens.Length - 1)
            {
                if (this.myTokens[this.myTokenIndex] == myOr)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of OR-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of OR-operation (= V)
                    this.Compile_And();
                    // V = <first argument from stack> OR V
                    this.AddCommand(myOr, "V = Stack.Pop OR V");
                }
                else
                {
                    // Break while-loop and return
                    break;
                }
            }
        }
        /// <summary>
        /// Compiler method: Operation AND
        /// </summary>
        private void Compile_And()
        {
            // Get first argument
            this.Compile_Equal_Unequal();
            while (this.myTokenIndex < this.myTokens.Length - 1)
            {
                if (this.myTokens[this.myTokenIndex] == myAnd)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of AND-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of AND-operation (= V)
                    this.Compile_Equal_Unequal();
                    // V = <first argument from stack> AND V
                    this.AddCommand(myAnd, "V = Stack.Pop AND V");
                }
                else
                {
                    // Break while-loop and return
                    break;
                }
            }
        }
        /// <summary>
        /// Compiler method: Operations Equal, Unequal
        /// </summary>
        private void Compile_Equal_Unequal()
        {
            // Get first argument
            this.Compile_Less_Greater();
            while (this.myTokenIndex < this.myTokens.Length - 1)
            {
                if (this.myTokens[this.myTokenIndex] == myEqual)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '='-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '='-operation (= V)
                    this.Compile_Less_Greater();
                    // V = <first argument from stack> == V
                    this.AddCommand(myEqual, "V = Stack.Pop == V");
                }
                else if (this.myTokens[this.myTokenIndex] == myUnequal)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '<>'-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '<>'-operation (= V)
                    this.Compile_Less_Greater();
                    // V = <first argument from stack> <> V
                    this.AddCommand(myUnequal, "V = Stack.Pop <> V");
                }
                else
                {
                    // Break while-loop and return
                    break;
                }
            }
        }
        /// <summary>
        /// Compiler method: Operations Less, Greater
        /// </summary>
        private void Compile_Less_Greater()
        {
            // Get first argument
            this.Compile_Add_Subtract();
            while (this.myTokenIndex < this.myTokens.Length - 1)
            {
                if (this.myTokens[this.myTokenIndex] == myLess)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '<'-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '<'-operation (= V)
                    this.Compile_Add_Subtract();
                    // V = <first argument from stack> < V
                    this.AddCommand(myLess, "V = Stack.Pop < V");
                }
                else if (this.myTokens[this.myTokenIndex] == myLessEqual)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '<='-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '<='-operation (= V)
                    this.Compile_Add_Subtract();
                    // V = <first argument from stack> <= V
                    this.AddCommand(myLessEqual, "V = Stack.Pop <= V");
                }
                else if (this.myTokens[this.myTokenIndex] == myGreaterEqual)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '>='-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '>='-operation (= V)
                    this.Compile_Add_Subtract();
                    // V = <first argument from stack> >= V
                    this.AddCommand(myGreaterEqual, "V = Stack.Pop >= V");
                }
                else if (this.myTokens[this.myTokenIndex] == myGreater)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '>'-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '>'-operation (= V)
                    this.Compile_Add_Subtract();
                    // V = <first argument from stack> > V
                    this.AddCommand(myGreater, "V = Stack.Pop > V");
                }
                else
                {
                    // Break while-loop and return
                    break;
                }
            }
        }
        /// <summary>
        /// Compiler method: Operations Add, Subtract
        /// </summary>
        private void Compile_Add_Subtract()
        {
            // Get first argument
            this.Compile_Multiply_Divide();
            while (this.myTokenIndex < this.myTokens.Length - 1)
            {
                if (this.myTokens[this.myTokenIndex] == myAdd)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '+'-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '+'-operation (= V)
                    this.Compile_Multiply_Divide();
                    // V = <first argument from stack> + V
                    this.AddCommand(myAdd, "V = Stack.Pop + V");
                }
                else if (this.myTokens[this.myTokenIndex] == mySubtract)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '-'-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '-'-operation (= V)
                    this.Compile_Multiply_Divide();
                    // V = <first argument from stack> - V
                    this.AddCommand(mySubtract, "V = Stack.Pop - V");
                }
                else
                {
                    // Break while-loop and return
                    break;
                }
            }
        }
        /// <summary>
        /// Compiler method: Operations Multiply, Divide
        /// </summary>
        private void Compile_Multiply_Divide()
        {
            // Get first argument
            this.Compile_Power();
            while (this.myTokenIndex < this.myTokens.Length - 1)
            {
                if (this.myTokens[this.myTokenIndex] == myMultiply)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '*'-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '*'-operation (= V)
                    this.Compile_Power();
                    // V = <first argument from stack> * V
                    this.AddCommand(myMultiply, "V = Stack.Pop * V");
                }
                else if (this.myTokens[this.myTokenIndex] == myDivide)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '/'-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '/'-operation (= V)
                    this.Compile_Power();
                    // V = <first argument from stack> / V
                    this.AddCommand(myDivide, "V = Stack.Pop / V");
                }
                else
                {
                    // Break while-loop and return
                    break;
                }
            }
        }
        /// <summary>
        /// Compiler method: Operation Power
        /// </summary>
        private void Compile_Power()
        {
            // Get first argument
            this.Compile_UnaryOperations();
            while (this.myTokenIndex < this.myTokens.Length - 1)
            {
                if (this.myTokens[this.myTokenIndex] == myPower)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                    // Push first argument of '^'-operation on stack
                    this.AddCommand(myPush, "Stack.Push(V)");
                    // Get second argument of '^'-operation (= V)
                    this.Compile_UnaryOperations();
                    // V = <first argument from stack> ^ V
                    this.AddCommand(myPower, "V = Stack.Pop ^ V");
                }
                else
                {
                    // Break while-loop and return
                    break;
                }
            }
        }
        /// <summary>
        /// Compiler method: Unary operations
        /// </summary>
        private void Compile_UnaryOperations()
        {
            if (this.myTokens[this.myTokenIndex] == myNot)
            {
                // Increment for next token
                this.myTokenIndex++;
                // Get argument
                this.Compile_Value();
                // V = NOT V
                this.AddCommand(myNot, "V = NOT V");
            }
            else if (this.myTokens[this.myTokenIndex] == mySubtract)
            {
                // Increment for next token
                this.myTokenIndex++;
                // Get argument
                this.Compile_Value();
                // V = -V
                this.AddCommand(myChangeSign, "V = -V");
            }
            else if (this.myTokens[this.myTokenIndex] == myAdd)
            {
                // Increment for next token
                this.myTokenIndex++;
                // Get argument
                this.Compile_Value();
                // V = +V: Do nothing
            }
            else
            {
                this.Compile_Value();
            }
        }
        /// <summary>
        /// Compiler method: Values
        /// </summary>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if syntax error is detected
        /// </exception>
        private void Compile_Value()
        {
            if (this.IsConstant(this.myTokens[this.myTokenIndex]))
            {
                #region Constant

                // V = double value
                if (this.IsVariableConstant(this.myTokens[this.myTokenIndex]))
                {
                    this.AddCorrectIndexCommand(
                        this.myTokens[this.myTokenIndex],
                        this.myCorrectIndex[this.myTokenIndex],
                        "V = " + this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex));
                }
                else
                {
                    this.AddCommand(
                        this.myTokens[this.myTokenIndex],
                        "V = " + this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex));
                }
                // Increment for next token
                this.myTokenIndex++;

                #endregion
            }
            else if (this.IsScalarArgument(this.myTokens[this.myTokenIndex]))
            {
                #region Scalar argument

                // V = double value
                if (this.IsVariableScalarArgument(this.myTokens[this.myTokenIndex]))
                {
                    this.AddCorrectIndexCommand(
                        this.myTokens[this.myTokenIndex],
                        this.myCorrectIndex[this.myTokenIndex],
                        "V = " + this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex));
                }
                else
                {
                    this.AddCommand(
                        this.myTokens[this.myTokenIndex],
                        "V = " + this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex));
                }
                // Increment for next token
                this.myTokenIndex++;

                #endregion
            }
            else if (this.IsVectorArgument(this.myTokens[this.myTokenIndex]))
            {
                #region Vector argument

                // VectorStack.Push(vector argument)
                if (this.IsVariableVectorArgument(this.myTokens[this.myTokenIndex]))
                {
                    this.AddCorrectIndexCommand(
                        this.myTokens[this.myTokenIndex],
                        this.myCorrectIndex[this.myTokenIndex],
                        "VectorStack.Push(" + this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex) + ")");
                }
                else
                {
                    this.AddCommand(
                        this.myTokens[this.myTokenIndex],
                        "VectorStack.Push(" + this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex) + ")");
                }
                // Increment for next token
                this.myTokenIndex++;

                #endregion
            }
            else if (this.IsVectorConstant(this.myTokens[this.myTokenIndex]))
            {
                #region Vector constant

                // VectorStack.Push(vector argument)
                if (this.IsVariableVectorConstant(this.myTokens[this.myTokenIndex]))
                {
                    this.AddCorrectIndexCommand(
                        this.myTokens[this.myTokenIndex],
                        this.myCorrectIndex[this.myTokenIndex],
                        "VectorStack.Push(" + this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex) + ")");
                }
                else
                {
                    this.AddCommand(
                        this.myTokens[this.myTokenIndex],
                        "VectorStack.Push(" + this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex) + ")");
                }
                // Increment for next token
                this.myTokenIndex++;

                #endregion
            }
            else if (this.myTokens[this.myTokenIndex] == myBracketOpen)
            {
                #region Bracket open

                // Increment for next token
                this.myTokenIndex++;
                // Recursive evaluation
                this.Compile_Or();
                // Check for closing bracket
                if (this.myTokens[this.myTokenIndex] != myBracketClose)
                {
                    //  7 = "Closing bracket is missing.";
                    this.myComment = MathCompiler.myComments[7];
                    this.myCodedComment = new CodedInfoItem(CodedInfo.MissingClosingBracket, null);
                    throw new MathCompilerException();
                }
                if (this.myTokenIndex < this.myTokens.Length - 1)
                {
                    // Increment for next token
                    this.myTokenIndex++;
                }

                #endregion
            }
            else
            {
                this.Compile_If_Function_VectorFunction_Vector();
            }
        }
        /// <summary>
        /// Compiler method: IF, functions, vector functions, vectors
        /// </summary>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if syntax error is detected
        /// </exception>
        private void Compile_If_Function_VectorFunction_Vector()
        {
            Int32 functionToken;
            Int32 functionTokenIndex;
            Int32 numberOfArguments;
            Boolean[] isVectorArgumentArray;
            Int32 falseJumpIdentifier;
            Int32 jumpIdentifier;

            if (this.myTokens[this.myTokenIndex] == myIf)
            {
                #region IF

                // Formula caused jumps
                this.myHasJump = true;
                // Increment for next token after IF token
                this.myTokenIndex++;
                // Increment for next token after opening bracket
                this.myTokenIndex++;

                #region Compile first (conditional) argument: Recursive evaluation

                this.Compile_Or();

                #region Check argument

                if (IsVectorArgument(this.myTokens[this.myTokenIndex - 1]))
                {
                    //  6 = "Vector expression ' {0} ' is only allowed as an argument of a vector function.";
                    this.myComment =
                        String.Format(
                        CultureInfo.InvariantCulture,
                        MathCompiler.myComments[6],
                        this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1));
                    this.myCodedComment = new CodedInfoItem(
                        CodedInfo.InvalidVectorExpression,
                        new String[] { this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1) });
                    throw new MathCompilerException();
                }

                #endregion

                // Add false-jump command
                falseJumpIdentifier = this.myJumpIdentifier--;
                this.AddJumpCommand(myFalseJump, falseJumpIdentifier, "FalseJump if V == false");

                #endregion

                if (this.myTokens[this.myTokenIndex] == myComma)
                {
                    #region Compile second (true) argument: Recursive evaluation

                    // Increment for next token after comma
                    this.myTokenIndex++;
                    this.Compile_Or();

                    #region Check argument

                    if (IsVectorArgument(this.myTokens[this.myTokenIndex - 1]))
                    {
                        //  6 = "Vector expression ' {0} ' is only allowed as an argument of a vector function.";
                        this.myComment =
                            String.Format(
                            CultureInfo.InvariantCulture,
                            MathCompiler.myComments[6],
                            this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1));
                        this.myCodedComment = new CodedInfoItem(
                            CodedInfo.InvalidVectorExpression,
                            new String[] { this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1) });
                        throw new MathCompilerException();
                    }

                    #endregion

                    // Add jump command
                    jumpIdentifier = this.myJumpIdentifier--;
                    this.AddJumpCommand(myJump, jumpIdentifier, "Jump");
                    // Add condition FALSE jump entry command
                    this.AddJumpCommand(myFalseJumpEntry, falseJumpIdentifier, "FalseJump entry");

                    #endregion
                }
                else
                {
                    // 18 = Conditional IF has 3 arguments.
                    this.myComment = MathCompiler.myComments[18];
                    this.myCodedComment = new CodedInfoItem(CodedInfo.InvalidIfArgumentCount, new String[] { });
                    throw new MathCompilerException();
                }

                if (this.myTokens[this.myTokenIndex] == myComma)
                {
                    #region Compile third (false) argument: Recursive evaluation

                    // Increment for next token after comma
                    this.myTokenIndex++;
                    this.Compile_Or();

                    #region Check argument

                    if (IsVectorArgument(this.myTokens[this.myTokenIndex - 1]))
                    {
                        //  6 = "Vector expression ' {0} ' is only allowed as an argument of a vector function.";
                        this.myComment =
                            String.Format(
                            CultureInfo.InvariantCulture,
                            MathCompiler.myComments[6],
                            this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1));
                        this.myCodedComment = new CodedInfoItem(
                            CodedInfo.InvalidVectorExpression,
                            new String[] { this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1) });
                        throw new MathCompilerException();
                    }

                    #endregion

                    // Add condition TRUE jump entry command
                    this.AddJumpCommand(myJumpEntry, jumpIdentifier, "Jump entry");

                    #endregion
                }
                else
                {
                    // 18 = Conditional IF has 3 arguments.
                    this.myComment = MathCompiler.myComments[18];
                    this.myCodedComment = new CodedInfoItem(CodedInfo.InvalidIfArgumentCount, new String[] { });
                    throw new MathCompilerException();
                }

                #region Checks

                if (this.myTokens[this.myTokenIndex] == myComma)
                {
                    // 18 = Conditional IF has 3 arguments.
                    this.myComment = MathCompiler.myComments[18];
                    this.myCodedComment = new CodedInfoItem(CodedInfo.InvalidIfArgumentCount, new String[] { });
                    throw new MathCompilerException();
                }
                if (this.myTokens[this.myTokenIndex] != myBracketClose)
                {
                    // 19 = Conditional IF does not have a closing bracket.
                    this.myComment = MathCompiler.myComments[19];
                    this.myCodedComment = new CodedInfoItem(CodedInfo.MissingIfClosingBracket, new String[] { });
                    throw new MathCompilerException();
                }

                #endregion

                // Increment for next token after closing bracket if possible
                if (this.myTokenIndex < this.myTokens.Length - 1)
                {
                    this.myTokenIndex++;
                }

                #endregion
            }
            else if (this.IsScalarFunction(this.myTokens[this.myTokenIndex]))
            {
                #region Function

                // Save function token and number of arguments
                functionToken = this.myTokens[this.myTokenIndex];
                functionTokenIndex = this.myTokenIndex;
                if (this.IsFixedScalarFunction(functionToken))
                {
                    #region numberOfArguments for fixed function

                    numberOfArguments = this.myScalarFunctions[this.CorrectIndex(functionToken, myFixedFunctionStartToken)].NumberOfArguments;

                    #endregion
                }
                else
                {
                    #region numberOfArguments for variable function

                    numberOfArguments = this.myScalarFunctions[this.myCorrectIndex[functionToken]].NumberOfArguments;

                    #endregion
                }
                // Increment for next token after function token
                this.myTokenIndex++;
                // Increment for next token after opening bracket
                this.myTokenIndex++;

                #region Compile first argument: Recursive evaluation

                this.Compile_Or();

                #region Check argument

                if (this.IsVectorArgument(this.myTokens[this.myTokenIndex - 1]) ||
                    this.IsVectorConstant(this.myTokens[this.myTokenIndex - 1]) ||
                    this.myTokens[this.myTokenIndex - 1] == myCurlyBracketClose)
                {
                    //  6 = "Vector expression ' {0} ' is only allowed as an argument of a vector function.";
                    this.myComment =
                        String.Format(
                        CultureInfo.InvariantCulture,
                        MathCompiler.myComments[6],
                        this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1));
                    this.myCodedComment = new CodedInfoItem(
                        CodedInfo.InvalidVectorExpression,
                        new String[] { this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1) });
                    throw new MathCompilerException();
                }

                #endregion

                // Push on stack
                this.AddCommand(myPush, "Stack.Push(V)");

                #endregion

                #region Compile remaining arguments

                for (Int32 i = 1; i < numberOfArguments; i++)
                {
                    if (this.myTokens[this.myTokenIndex] == myComma)
                    {
                        // Increment for next token after comma
                        this.myTokenIndex++;
                        // Compile next argument: Recursive evaluation
                        this.Compile_Or();

                        #region Check argument

                        if (this.IsVectorArgument(this.myTokens[this.myTokenIndex - 1]) ||
                            this.IsVectorConstant(this.myTokens[this.myTokenIndex - 1]) ||
                            this.myTokens[this.myTokenIndex - 1] == myCurlyBracketClose)
                        {
                            //  6 = "Vector expression ' {0} ' is only allowed as an argument of a vector function.";
                            this.myComment =
                                String.Format(
                                CultureInfo.InvariantCulture,
                                MathCompiler.myComments[6],
                                this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1));
                            this.myCodedComment = new CodedInfoItem(
                                CodedInfo.InvalidVectorExpression,
                                new String[] { this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex - 1], this.myTokenIndex - 1) });
                            throw new MathCompilerException();
                        }

                        #endregion

                        // Push on stack
                        this.AddCommand(myPush, "Stack.Push(V)");
                    }
                    else
                    {
                        //  3 = "Function ' {0} ' has {1} argument(s).";
                        this.myComment =
                            String.Format(
                            CultureInfo.InvariantCulture,
                            MathCompiler.myComments[3],
                            this.ConvertToTokenRepresentation(functionToken, functionTokenIndex),
                            numberOfArguments.ToString(CultureInfo.InvariantCulture.NumberFormat));
                        this.myCodedComment = new CodedInfoItem(
                            CodedInfo.InvalidFunctionArgumentCount,
                            new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex),
                            numberOfArguments.ToString(CultureInfo.InvariantCulture.NumberFormat) });
                        throw new MathCompilerException();
                    }
                }

                #endregion

                #region Checks

                if (this.myTokens[this.myTokenIndex] == myComma)
                {
                    //  3 = "Function ' {0} ' has {1} argument(s).";
                    this.myComment =
                        String.Format(
                        CultureInfo.InvariantCulture,
                        MathCompiler.myComments[3],
                        this.ConvertToTokenRepresentation(functionToken, functionTokenIndex),
                        numberOfArguments.ToString(CultureInfo.InvariantCulture.NumberFormat));
                    this.myCodedComment = new CodedInfoItem(
                        CodedInfo.InvalidFunctionArgumentCount,
                        new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex),
                        numberOfArguments.ToString(CultureInfo.InvariantCulture.NumberFormat) });
                    throw new MathCompilerException();
                }
                if (this.myTokens[this.myTokenIndex] != myBracketClose)
                {
                    //  8 = "Function ' {0} ' does not have a closing bracket.";
                    this.myComment =
                        String.Format(
                        CultureInfo.InvariantCulture,
                        MathCompiler.myComments[8],
                        this.ConvertToTokenRepresentation(functionToken, functionTokenIndex));
                    this.myCodedComment = new CodedInfoItem(
                        CodedInfo.MissingFunctionClosingBracket,
                        new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) });
                    throw new MathCompilerException();
                }

                #endregion

                // V = function
                if (this.IsVariableScalarFunction(this.myTokens[this.myTokenIndex]))
                {
                    this.AddCorrectIndexCommand(
                        functionToken,
                        this.myCorrectIndex[functionTokenIndex],
                        "V = " + this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) + "(Stack.Pop, ...)");
                }
                else
                {
                    this.AddCommand(
                        functionToken,
                        "V = " + this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) + "(Stack.Pop, ...)");
                }
                // Increment for next token after closing bracket if possible
                if (this.myTokenIndex < this.myTokens.Length - 1)
                {
                    this.myTokenIndex++;
                }

                #endregion
            }
            else if (this.IsVectorFunction(this.myTokens[this.myTokenIndex]))
            {
                #region Vector function

                // Save function token and number of arguments
                functionToken = this.myTokens[this.myTokenIndex];
                functionTokenIndex = this.myTokenIndex;
                // Save number of arguments of vector function
                if (this.IsFixedVectorFunction(functionToken))
                {
                    #region Number and type of arguments for fixed vectorfunction

                    numberOfArguments = this.myVectorFunctions[this.CorrectIndex(functionToken, myFixedVectorFunctionStartToken)].NumberOfArguments;
                    // Get information whether argument i is a vector argument
                    isVectorArgumentArray = new Boolean[numberOfArguments];
                    for (Int32 i = 0; i < isVectorArgumentArray.Length; i++)
                    {
                        isVectorArgumentArray[i] =
                            this.myVectorFunctions[this.CorrectIndex(functionToken, myFixedVectorFunctionStartToken)].IsVectorArgument(i);
                    }

                    #endregion
                }
                else
                {
                    #region Number and type of arguments for variable variable function

                    numberOfArguments = this.myVectorFunctions[this.myCorrectIndex[functionToken]].NumberOfArguments;
                    // Get information whether argument i is a vector argument
                    isVectorArgumentArray = new Boolean[numberOfArguments];
                    for (Int32 i = 0; i < isVectorArgumentArray.Length; i++)
                    {
                        isVectorArgumentArray[i] =
                            this.myVectorFunctions[this.myCorrectIndex[functionToken]].IsVectorArgument(i);
                    }

                    #endregion
                }
                // Increment for next token after function token
                this.myTokenIndex++;
                // Increment for next token after opening bracket
                this.myTokenIndex++;

                #region Compile first argument: Recursive evaluation

                this.Compile_Or();

                #region Check argument

                if (this.IsVectorArgument(this.myTokens[this.myTokenIndex - 1]) ||
                    this.IsVectorConstant(this.myTokens[this.myTokenIndex - 1]) ||
                    this.myTokens[this.myTokenIndex - 1] == myCurlyBracketClose)
                {
                    #region Last token was vector argument

                    #region Check if argument type is correct

                    if (!isVectorArgumentArray[0])
                    {
                        // 14 = "Argument {0} of vector function ' {1} ' is a scalar argument.";
                        this.myComment =
                            String.Format(
                            CultureInfo.InvariantCulture,
                            MathCompiler.myComments[14],
                            "0",
                            this.ConvertToTokenRepresentation(functionToken, functionTokenIndex));
                        this.myCodedComment = new CodedInfoItem(
                            CodedInfo.MissingScalarArgument,
                            new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) });
                        throw new MathCompilerException();
                    }

                    #endregion

                    // NOTE: If last token was a vector argument is was automatically pushed on vector stack

                    #endregion
                }
                else
                {
                    #region Last token was NO vector argument: Check and push on stack

                    #region Check if argument type is correct

                    if (isVectorArgumentArray[0])
                    {
                        // 13 = "Argument {0} of vector function ' {1} ' is a vector argument.";
                        this.myComment =
                            String.Format(
                            CultureInfo.InvariantCulture,
                            MathCompiler.myComments[13],
                            "0",
                            this.ConvertToTokenRepresentation(functionToken, functionTokenIndex));
                        this.myCodedComment = new CodedInfoItem(
                            CodedInfo.MissingVectorArgument,
                            new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) });
                        throw new MathCompilerException();
                    }

                    #endregion

                    // Push on stack
                    this.AddCommand(myPush, "Stack.Push(V)");

                    #endregion
                }
                #endregion

                #endregion

                #region Compile remaining arguments

                for (Int32 i = 1; i < numberOfArguments; i++)
                {
                    if (this.myTokens[this.myTokenIndex] == myComma)
                    {
                        // Increment for next token after comma
                        this.myTokenIndex++;
                        // Compile next argument: Recursive evaluation
                        this.Compile_Or();

                        #region Check argument

                        if (this.IsVectorArgument(this.myTokens[this.myTokenIndex - 1]) ||
                            this.IsVectorConstant(this.myTokens[this.myTokenIndex - 1]) ||
                            this.myTokens[this.myTokenIndex - 1] == myCurlyBracketClose)
                        {
                            #region Last token was vector argument

                            #region Check if argument type is correct

                            if (!isVectorArgumentArray[i])
                            {
                                // 14 = "Argument {0} of vector function ' {1} ' is a scalar argument.";
                                this.myComment =
                                    String.Format(
                                    CultureInfo.InvariantCulture,
                                    MathCompiler.myComments[14],
                                    i.ToString(CultureInfo.InvariantCulture.NumberFormat),
                                    this.ConvertToTokenRepresentation(functionToken, functionTokenIndex));
                                this.myCodedComment = new CodedInfoItem(
                                    CodedInfo.MissingScalarArgument,
                                    new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) });
                                throw new MathCompilerException();
                            }

                            #endregion

                            // NOTE: If last token was a vector argument is was automatically pushed on vector stack

                            #endregion
                        }
                        else
                        {
                            #region Last token was NO vector argument: Check and push on stack

                            #region Check if argument type is correct

                            if (isVectorArgumentArray[i])
                            {
                                // 13 = "Argument {0} of vector function ' {1} ' is a vector argument.";
                                this.myComment =
                                    String.Format(
                                    CultureInfo.InvariantCulture,
                                    MathCompiler.myComments[13],
                                    i.ToString(CultureInfo.InvariantCulture.NumberFormat),
                                    this.ConvertToTokenRepresentation(functionToken, functionTokenIndex));
                                this.myCodedComment = new CodedInfoItem(
                                    CodedInfo.MissingVectorArgument,
                                    new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) });
                                throw new MathCompilerException();
                            }

                            #endregion

                            // Push on stack
                            this.AddCommand(myPush, "Stack.Push(V)");

                            #endregion
                        }
                        #endregion

                    }
                    else
                    {
                        #region No comma found: Exception

                        //  3 = "Function ' {0} ' has {1} argument(s).";
                        this.myComment =
                            String.Format(
                            CultureInfo.InvariantCulture,
                            MathCompiler.myComments[3],
                            this.ConvertToTokenRepresentation(functionToken, functionTokenIndex),
                            numberOfArguments.ToString(CultureInfo.InvariantCulture.NumberFormat));
                        this.myCodedComment = new CodedInfoItem(
                            CodedInfo.InvalidFunctionArgumentCount,
                            new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex),
                            numberOfArguments.ToString(CultureInfo.InvariantCulture.NumberFormat) });
                        throw new MathCompilerException();

                        #endregion
                    }
                }

                #endregion

                #region Checks

                if (this.myTokens[this.myTokenIndex] == myComma)
                {
                    //  3 = "Function ' {0} ' has {1} argument(s).";
                    this.myComment =
                        String.Format(
                        CultureInfo.InvariantCulture,
                        MathCompiler.myComments[3],
                        this.ConvertToTokenRepresentation(functionToken, functionTokenIndex),
                        numberOfArguments.ToString(CultureInfo.InvariantCulture.NumberFormat));
                    this.myCodedComment = new CodedInfoItem(
                        CodedInfo.InvalidFunctionArgumentCount,
                        new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex),
                        numberOfArguments.ToString(CultureInfo.InvariantCulture.NumberFormat) });
                    throw new MathCompilerException();
                }
                if (this.myTokens[this.myTokenIndex] != myBracketClose)
                {
                    //  8 = "Function ' {0} ' does not have a closing bracket.";
                    this.myComment =
                        String.Format(
                        CultureInfo.InvariantCulture,
                        MathCompiler.myComments[8],
                        this.ConvertToTokenRepresentation(functionToken, functionTokenIndex));
                    this.myCodedComment = new CodedInfoItem(
                        CodedInfo.MissingFunctionClosingBracket,
                        new String[] { this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) });
                    throw new MathCompilerException();
                }

                #endregion

                // V = function
                if (this.IsVariableScalarFunction(this.myTokens[this.myTokenIndex]))
                {
                    this.AddCorrectIndexCommand(
                        functionToken,
                        this.myCorrectIndex[functionTokenIndex],
                        "V = " + this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) + "(VectorStack.Pop, ...)");
                }
                else
                {
                    this.AddCommand(
                        functionToken,
                        "V = " + this.ConvertToTokenRepresentation(functionToken, functionTokenIndex) + "(VectorStack.Pop, ...)");
                }
                // Increment for next token after closing bracket if possible
                if (this.myTokenIndex < this.myTokens.Length - 1)
                {
                    this.myTokenIndex++;
                }

                #endregion
            }
            else if (this.myTokens[this.myTokenIndex] == myCurlyBracketOpen)
            {
                #region Vector

                // Formula has vectors
                this.myHasVector = true;
                // Compile vector components: Recursive evaluation
                Boolean isFirstComponent = true;
                do
                {
                    // Increment for next token after curly bracket open or comma
                    this.myTokenIndex++;
                    // Evaluate component expression
                    this.Compile_Or();
                    // Check if first component
                    if (isFirstComponent)
                    {
                        #region Create new vector stack support list

                        this.AddCommand(
                            myClearNewVectorStackSupportList,
                            "VectorStackSupportList.Clear");
                        isFirstComponent = false;

                        #endregion
                    }
                    // Check component
                    if (this.myTokens[this.myTokenIndex - 1] == myCurlyBracketClose)
                    {
                        #region Illegal nested vector

                        // 25 = Illegal nested vector.
                        this.myComment = MathCompiler.myComments[25];
                        this.myCodedComment = new CodedInfoItem(CodedInfo.IllegalNestedVector, null);
                        throw new MathCompilerException();

                        #endregion
                    }
                    if (this.IsVectorArgument(this.myTokens[this.myTokenIndex - 1]) ||
                        this.IsVectorConstant(this.myTokens[this.myTokenIndex - 1]))
                    {
                        #region Vector component

                        // Formula has nested vectors
                        this.myHasNestedVector = true;
                        this.AddCommand(
                            myAddVectorStackComponentsToVectorStackSupportList,
                            "VectorStackSupportList.Add(VectorStack.Pop)");

                        #endregion
                    }
                    else
                    {
                        #region Scalar component

                        this.AddCommand(
                            myAddScalarValueToVectorStackSupportList,
                            "VectorStackSupportList.Add(V)");

                        #endregion
                    }
                }
                while (this.myTokens[this.myTokenIndex] == myComma);

                #region Check curly bracket close

                if (this.myTokens[this.myTokenIndex] != myCurlyBracketClose)
                {
                    // 21 = Invalid vector: Curly closing bracket is missing.
                    this.myComment = MathCompiler.myComments[21];
                    this.myCodedComment = new CodedInfoItem(CodedInfo.InvalidVector, null);
                    throw new MathCompilerException();
                }

                #endregion

                // Push vector stack support list to vector stack 
                this.AddCommand(
                    myPushVectorStackSupportListToVectorStack,
                    "VectorStack.Push(VectorStackSupportList.ToArray)");
                // Increment for next token after closing curly bracket
                this.myTokenIndex++;

                #endregion
            }
            else
            {
                #region Invalid token

                //  5 = "Invalid token: ' {0} '.";
                this.myComment =
                    String.Format(
                    CultureInfo.InvariantCulture,
                    MathCompiler.myComments[5],
                    this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex));
                this.myCodedComment = new CodedInfoItem(
                    CodedInfo.InvalidToken,
                    new String[] { this.ConvertToTokenRepresentation(this.myTokens[this.myTokenIndex], this.myTokenIndex) });
                throw new MathCompilerException(this.myComment);

                #endregion
            }
        }

        #endregion

        #region Token conversion methods

        /// <summary>
        /// Splits formula into token representations
        /// </summary>
        /// <remarks>
        /// myComment is changed if necessary
        /// </remarks>
        /// <param name="formula">
        /// Formula
        /// </param>
        /// <returns>
        /// Array of token representations or null if formula could not be split
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if formula is null/empty
        /// </exception>
        public String[] SplitToTokenRepresentations(String formula)
        {
            StringBuilder buffer;
            String result;

            #region Checks

            if (String.IsNullOrEmpty(formula))
            {
                throw new MathCompilerException("Parameter formula is null/empty.");
            }

            #endregion

            // NOTE: Evaluation of vector constants must be performed BEFORE 
            //       conversion to token representations!

            #region Remove whitespaces and separate token representations by character '|'

            result = myWhitespaceCharatersRegex.Replace(formula, String.Empty);
            buffer = new StringBuilder(result, result.Length * 2);
            buffer.Replace("+", "|+|");
            buffer.Replace("-", "|-|");
            buffer.Replace("*", "|*|");
            buffer.Replace("/", "|/|");
            buffer.Replace("^", "|^|");
            buffer.Replace("(", "|(|");
            buffer.Replace(")", "|)|");
            buffer.Replace("{", "|{|");
            buffer.Replace("}", "|}|");
            buffer.Replace("<", "|<|");
            buffer.Replace(">", "|>|");
            buffer.Replace("=", "|=|");
            buffer.Replace(",", "|,|");
            result = myOperatorAndReplaceRegex.Replace(buffer.ToString(), "|AND|");
            result = myOperatorOrReplaceRegex.Replace(result, "|OR|");
            result = myOperatorNotReplaceRegex.Replace(result, "|NOT|");

            #endregion

            #region Correct separated token representations

            // Correct for | at beginning and end
            result = myOperatorCorrectRegex1.Replace(result, String.Empty);
            // Correct for |{||}| for vector arguments.
            // NOTE: This correction has to be performed BEFORE multiple | are corrected
            result = myCurlyBracketCorrectRegex.Replace(result, @"{}");
            // Correct for E|-| in scientific numbers
            result = myScientificNumberCorrectRegex1.Replace(result, "E-");
            // Correct for E|+| in scientific numbers
            result = myScientificNumberCorrectRegex2.Replace(result, "E+");
            // Correct for multiple |
            result = myOperatorCorrectRegex2.Replace(result, @"|");
            // Correct for <|=
            result = myOperatorCorrectRegex3.Replace(result, @"<=");
            // Correct for >|=
            result = myOperatorCorrectRegex4.Replace(result, @">=");
            // Correct for <|>
            result = myOperatorCorrectRegex5.Replace(result, @"<>");

            #endregion

            #region Tokenize with character '|', trim tokenList and return

            return MathCompiler.TokenizeString(result, '|', true);

            #endregion
        }
        /// <summary>
        /// Converts array with token representations into integer token list
        /// </summary>
        /// <remarks>
        /// myComment is changed if necessary
        /// </remarks>
        /// <param name="tokenRepresentations">
        /// Array with token representations 
        /// </param>
        /// <returns>
        /// Array with integer tokens or null if conversion failed
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if tokenRepresentations is null or has length 0
        /// </exception>
        public Int32[] ConvertToIntegerTokens(String[] tokenRepresentations)
        {
            #region Local variable declarations

            Double constantValue;
            List<Double> constantValueList;
            List<Int32> vectorConstantIndexList;
            List<Int32> variableFunctionTokenList;
            List<Int32> variableVectorFunctionTokenList;
            List<Int32> variableScalarArgumentIndexList;
            List<Int32> variableVectorArgumentTokenList;
            Int32 indexOfScalarArgument;
            Int32 indexOfVectorArgument;
            Int32 indexOfVectorConstant;
            Int32[] tokens;

            #endregion

            #region Checks

            if (tokenRepresentations == null || tokenRepresentations.Length == 0)
            {
                throw new MathCompilerException("tokenRepresentations is null or has length 0.");
            }

            #endregion

            #region Variable initialization

            tokens = new Int32[tokenRepresentations.Length];
            this.myCorrectIndex = new Int32[tokenRepresentations.Length];
            // Clear myCorrectIndex with -1
            for (Int32 i = 0; i < this.myCorrectIndex.Length; i++) this.myCorrectIndex[i] = -1;
            // Capacities will be sufficient in most cases
            constantValueList = new List<Double>(20);
            vectorConstantIndexList = new List<Int32>(20);
            variableFunctionTokenList = new List<Int32>(20);
            variableVectorFunctionTokenList = new List<Int32>(20);
            variableScalarArgumentIndexList = new List<Int32>(20);
            variableVectorArgumentTokenList = new List<Int32>(20);
            this.myNumberOfScalarArguments = 0;
            this.myNumberOfVectorArguments = 0;

            this.myHasScalarArguments = false;
            this.myHasConstants = false;
            this.myHasVectorConstants = false;
            this.myHasVectorArguments = false;
            this.myHasScalarFunctions = false;
            this.myHasVectorFunctions = false;

            this.myHasVariableScalarArguments = false;
            this.myHasVariableConstants = false;
            this.myHasVariableVectorConstants = false;
            this.myHasVariableVectorArguments = false;
            this.myHasVariableScalarFunctions = false;
            this.myHasVariableVectorFunctions = false;

            #endregion

            for (Int32 i = 0; i < tokenRepresentations.Length; i++)
            {
                switch (tokenRepresentations[i])
                {
                    #region (, )

                    case myBracketOpenSymbol:
                        tokens[i] = myBracketOpen;
                        break;
                    case myBracketCloseSymbol:
                        tokens[i] = myBracketClose;
                        break;

                    #endregion

                    #region {, }

                    case myCurlyBracketOpenSymbol:
                        tokens[i] = myCurlyBracketOpen;
                        break;
                    case myCurlyBracketCloseSymbol:
                        tokens[i] = myCurlyBracketClose;
                        break;

                    #endregion

                    #region <, <=, =, <>, >=, >

                    case myLessSymbol:
                        tokens[i] = myLess;
                        break;
                    case myLessEqualSymbol:
                        tokens[i] = myLessEqual;
                        break;
                    case myEqualSymbol:
                        tokens[i] = myEqual;
                        break;
                    case myUnequalSymbol:
                        tokens[i] = myUnequal;
                        break;
                    case myGreaterEqualSymbol:
                        tokens[i] = myGreaterEqual;
                        break;
                    case myGreaterSymbol:
                        tokens[i] = myGreater;
                        break;

                    #endregion

                    #region AND, OR, NOT

                    case myAndSymbol:
                        tokens[i] = myAnd;
                        break;
                    case myOrSymbol:
                        tokens[i] = myOr;
                        break;
                    case myNotSymbol:
                        tokens[i] = myNot;
                        break;

                    #endregion

                    #region +,-,*,/,^

                    case myAddSymbol:
                        tokens[i] = myAdd;
                        break;
                    case mySubtractSymbol:
                        tokens[i] = mySubtract;
                        break;
                    case myMultiplySymbol:
                        tokens[i] = myMultiply;
                        break;
                    case myDivideSymbol:
                        tokens[i] = myDivide;
                        break;
                    case myPowerSymbol:
                        tokens[i] = myPower;
                        break;

                    #endregion

                    #region ,

                    case myCommaSymbol:
                        tokens[i] = myComma;
                        break;

                    #endregion

                    #region IF

                    case myIfSymbol:
                        tokens[i] = myIf;
                        break;

                    #endregion

                    default:

                        if (MathCompiler.myScalarArgumentDetectionRegex.IsMatch(tokenRepresentations[i]))
                        {
                            #region Scalar argument

                            indexOfScalarArgument = Int32.Parse(tokenRepresentations[i].Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                            if (this.myNumberOfScalarArguments < indexOfScalarArgument + 1)
                            {
                                this.myNumberOfScalarArguments = indexOfScalarArgument + 1;
                            }
                            if (indexOfScalarArgument < myFixedScalarArgumentCount)
                            {
                                #region Fixed scalar argument

                                // '* 2': Every fixed scalar argument token is always followed by scalar argument-push token
                                tokens[i] = myFixedScalarArgumentStartToken + indexOfScalarArgument * 2;

                                #endregion
                            }
                            else
                            {
                                #region Variable scalar argument

                                variableScalarArgumentIndexList.Add(indexOfScalarArgument);
                                tokens[i] = myIndicatorVariableScalarArgument;
                                this.myHasVariableScalarArguments = true;

                                #endregion
                            }
                            this.myHasScalarArguments = true;

                            #endregion
                        }
                        else if (this.myPredefinedConstantValueDictionary != null && 
                            this.myPredefinedConstantValueDictionary.ContainsKey(tokenRepresentations[i]))
                        {
                            #region Predefined Constant

                            constantValueList.Add(this.myPredefinedConstantValueDictionary[tokenRepresentations[i]]);
                            tokens[i] = myIndicatorConstant;
                            this.myHasConstants = true;

                            #endregion
                        }
                        else if (this.myScalarFunctionTokenDictionary != null && 
                            this.myScalarFunctionTokenDictionary.ContainsKey(tokenRepresentations[i]))
                        {
                            #region Scalar function

                            if (this.myScalarFunctionTokenDictionary[tokenRepresentations[i]] < myFixedFunctionCount)
                            {
                                #region Fixed scalar function

                                // *2: Every fixed function token is always followed by function-push token
                                tokens[i] = myFixedFunctionStartToken + this.myScalarFunctionTokenDictionary[tokenRepresentations[i]] * 2;

                                #endregion
                            }
                            else
                            {
                                #region Variable scalar function

                                variableFunctionTokenList.Add(this.myScalarFunctionTokenDictionary[tokenRepresentations[i]]);
                                tokens[i] = myIndicatorVariableFunction;
                                this.myHasVariableScalarFunctions = true;

                                #endregion
                            }
                            this.myHasScalarFunctions = true;

                            #endregion
                        }
                        else if (this.myVectorFunctionTokenDictionary != null && 
                            this.myVectorFunctionTokenDictionary.ContainsKey(tokenRepresentations[i]))
                        {
                            #region Vector function

                            if (this.myVectorFunctionTokenDictionary[tokenRepresentations[i]] < myFixedVectorFunctionCount)
                            {
                                #region Fixed vector function

                                // *2: Every fixed vector function token is always followed by vector-function-push token
                                tokens[i] = myFixedVectorFunctionStartToken + this.myVectorFunctionTokenDictionary[tokenRepresentations[i]] * 2;

                                #endregion
                            }
                            else
                            {
                                #region Variable vector function

                                variableVectorFunctionTokenList.Add(this.myVectorFunctionTokenDictionary[tokenRepresentations[i]]);
                                tokens[i] = myIndicatorVariableVectorFunction;
                                this.myHasVariableVectorFunctions = true;

                                #endregion
                            }
                            this.myHasVectorFunctions = true;

                            #endregion
                        }
                        else if (MathCompiler.myVectorArgumentDetectionRegex.IsMatch(tokenRepresentations[i]))
                        {
                            #region Vector argument

                            indexOfVectorArgument = Int32.Parse(
                                tokenRepresentations[i].Substring(1, tokenRepresentations[i].Length - 3),
                                CultureInfo.InvariantCulture.NumberFormat);
                            if (this.myNumberOfVectorArguments < indexOfVectorArgument + 1)
                            {
                                this.myNumberOfVectorArguments = indexOfVectorArgument + 1;
                            }
                            if (indexOfVectorArgument < myFixedVectorArgumentCount)
                            {
                                #region Fixed vector argument

                                // NOTE: NO '* 2' since every vector argument is automatically pushed on vector stack
                                tokens[i] = myFixedVectorArgumentStartToken + indexOfVectorArgument;

                                #endregion
                            }
                            else
                            {
                                #region Variable vector argument

                                variableVectorArgumentTokenList.Add(indexOfVectorArgument);
                                tokens[i] = myIndicatorVariableVectorArgument;
                                this.myHasVariableVectorArguments = true;

                                #endregion
                            }
                            this.myHasVectorArguments = true;

                            #endregion
                        }
                        else if (MathCompiler.myTransformedVectorConstantDetectionRegex.IsMatch(tokenRepresentations[i]))
                        {
                            #region Vector constant

                            indexOfVectorConstant = Int32.Parse(tokenRepresentations[i].Substring(1),CultureInfo.InvariantCulture.NumberFormat);
                            vectorConstantIndexList.Add(indexOfVectorConstant);
                            tokens[i] = myIndicatorVectorConstant;
                            this.myHasVectorConstants = true;

                            #endregion
                        }
                        else if (MathCompiler.ConvertToDouble(tokenRepresentations[i], out constantValue))
                        {
                            #region Constant

                            constantValueList.Add(constantValue);
                            tokens[i] = myIndicatorConstant;
                            this.myHasConstants = true;

                            #endregion
                        }
                        else
                        {
                            #region Invalid token representation
                            //  5 = "Invalid token: ' {0} '.";
                            this.myComment =
                                String.Format(
                                CultureInfo.InvariantCulture,
                                MathCompiler.myComments[5],
                                tokenRepresentations[i]);
                            this.myCodedComment = new CodedInfoItem(
                                CodedInfo.InvalidToken,
                                new String[] { tokenRepresentations[i] });
                            return null;
                            #endregion
                        }
                        break;
                }
            }

            // NOTE: Do NOT change sequence of checks since every following check depends on dependent one

            #region Check variable scalar arguments

            if (this.myHasVariableScalarArguments)
            {
                Int32 k;
                Int32 j;

                this.myMaxScalarArgumentIndex = myVariableScalarArgumentStartToken + this.myNumberOfScalarArguments - myFixedScalarArgumentCount;
                k = myVariableScalarArgumentStartToken;
                j = 0;
                for (Int32 i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i] == myIndicatorVariableScalarArgument)
                    {
                        this.myCorrectIndex[i] = variableScalarArgumentIndexList[j++];
                        tokens[i] = k++;
                    }
                }
                // Check: k == this.myMaxScalarArgumentIndex
            }
            else
            {
                this.myMaxScalarArgumentIndex = myVariableScalarArgumentStartToken;
            }

            #endregion

            #region Check constants

            if (this.myHasConstants)
            {
                // NOTE: There are dublettes in constantValueList which must be eliminated

                #region Elimination of dublettes in constantValueList

                List<Double> newConstantValueList = new List<Double>(constantValueList.Count);
                List<Int32> indexList = new List<Int32>(constantValueList.Count);
                newConstantValueList.Add(constantValueList[0]);
                indexList.Add(0);
                for (Int32 i = 1; i < constantValueList.Count; i++)
                {
                    Boolean isDublette = false;
                    for (Int32 k = 0; k < newConstantValueList.Count; k++)
                    {
                        if (constantValueList[i] == newConstantValueList[k])
                        {
                            indexList.Add(k);
                            isDublette = true;
                            break;
                        }
                    }
                    if (!isDublette)
                    {
                        indexList.Add(newConstantValueList.Count);
                        newConstantValueList.Add(constantValueList[i]);
                    }
                }
                this.myConstants = new Double[newConstantValueList.Count];
                newConstantValueList.CopyTo(this.myConstants, 0);

                #endregion

                if (this.myConstants.Length > myFixedConstantCount)
                {
                    Int32 k;
                    Int32 j;

                    #region Fixed and variable constants

                    this.myHasVariableConstants = true;
                    this.myMaxConstantIndex = this.myMaxScalarArgumentIndex + this.myConstants.Length - myFixedConstantCount;

                    // Correct tokens for fixed constants
                    k = 0;
                    j = 0;
                    for (Int32 i = 0; i < tokens.Length; i++)
                    {
                        if (tokens[i] == myIndicatorConstant)
                        {
                            if (indexList[k] < myFixedConstantCount)
                            {
                                #region Tokens for fixed constants

                                // *2: Every fixed constant token is always followed by constant-push token
                                tokens[i] = myFixedConstantStartToken + indexList[k++] * 2;

                                #endregion
                            }
                            else
                            {
                                #region Tokens and myCorrectIndex for variable constants

                                this.myCorrectIndex[i] = indexList[k++];
                                tokens[i] = this.myMaxScalarArgumentIndex + j++;

                                #endregion
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region Fixed constants only

                    // Correct tokens
                    int k = 0;
                    for (Int32 i = 0; i < tokens.Length; i++)
                    {
                        if (tokens[i] == myIndicatorConstant)
                        {
                            // *2: Every fixed constant token is always followed by constant-push token
                            tokens[i] = myFixedConstantStartToken + indexList[k++] * 2;
                        }
                    }
                    this.myMaxConstantIndex = this.myMaxScalarArgumentIndex;

                    #endregion
                }
            }
            else
            {
                this.myMaxConstantIndex = this.myMaxScalarArgumentIndex;
            }
            #endregion

            #region Check vector constants

            if (this.myHasVectorConstants)
            {
                if (vectorConstantIndexList.Count > myFixedVectorConstantCount)
                {
                    #region Fixed and variable vector constants

                    this.myHasVariableVectorConstants = true;
                    this.myMaxVectorConstantIndex = this.myMaxConstantIndex + this.myVectorConstants.Length - myFixedVectorConstantCount;

                    Int32 k = 0;
                    Int32 j = 0;
                    for (Int32 i = 0; i < tokens.Length; i++)
                    {
                        if (tokens[i] == myIndicatorVectorConstant)
                        {
                            if (vectorConstantIndexList[k] < myFixedVectorConstantCount)
                            {
                                #region Tokens for fixed vector constants

                                // NOTE: NO '* 2' since every vector constant is automatically pushed on vector stack
                                tokens[i] = myFixedVectorConstantStartToken + vectorConstantIndexList[k++];

                                #endregion
                            }
                            else
                            {
                                #region Tokens and myCorrectIndex for variable vector constants

                                this.myCorrectIndex[i] = vectorConstantIndexList[k++];
                                tokens[i] = this.myMaxConstantIndex + j++;

                                #endregion
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region Fixed vector constants only

                    int k = 0;
                    for (Int32 i = 0; i < tokens.Length; i++)
                    {
                        if (tokens[i] == myIndicatorVectorConstant)
                        {
                            // NOTE: NO '* 2' since every vector constant is automatically pushed on vector stack
                            tokens[i] = myFixedVectorConstantStartToken + vectorConstantIndexList[k++];
                        }
                    }
                    this.myMaxVectorConstantIndex = this.myMaxConstantIndex;

                    #endregion
                }
            }
            else
            {
                this.myMaxVectorConstantIndex = this.myMaxConstantIndex;
            }

            #endregion

            #region Check variable functions

            if (this.myHasVariableScalarFunctions)
            {
                this.myMaxFunctionIndex = this.myMaxVectorConstantIndex + variableFunctionTokenList.Count;
                // Correct tokens
                Int32 k = this.myMaxConstantIndex;
                Int32 j = 0;
                for (Int32 i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i] == myIndicatorVariableFunction)
                    {
                        tokens[i] = k++;
                        this.myCorrectIndex[i] = variableFunctionTokenList[j];
                    }
                }
                // Check: k == this.myMaxFunctionIndex
            }
            else
            {
                this.myMaxFunctionIndex = this.myMaxVectorConstantIndex;
            }

            #endregion

            #region Check variable vector functions

            if (this.myHasVariableVectorFunctions)
            {
                this.myMaxVectorFunctionIndex = this.myMaxFunctionIndex + variableVectorFunctionTokenList.Count;
                // Correct tokens
                Int32 k = this.myMaxFunctionIndex;
                Int32 j = 0;
                for (Int32 i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i] == myIndicatorVariableVectorFunction)
                    {
                        tokens[i] = k++;
                        this.myCorrectIndex[i] = variableVectorFunctionTokenList[j];
                    }
                }
                // Check: k == this.myMaxVectorFunctionIndex
            }
            else
            {
                this.myMaxVectorFunctionIndex = this.myMaxFunctionIndex;
            }

            #endregion

            #region Check variable vector arguments

            if (this.myHasVariableVectorArguments)
            {
                this.myMaxVectorArgumentIndex = this.myMaxVectorFunctionIndex + variableVectorArgumentTokenList.Count;
                // Correct tokens
                Int32 k = this.myMaxVectorFunctionIndex;
                Int32 j = 0;
                for (Int32 i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i] == myIndicatorVariableVectorArgument)
                    {
                        tokens[i] = k++;
                        this.myCorrectIndex[i] = variableVectorArgumentTokenList[j];
                    }
                }
            }
            else
            {
                this.myMaxVectorArgumentIndex = this.myMaxVectorFunctionIndex;
            }
            // Higher indices myMaxCalculatedConstantIndex, myMaxSubtermConstantIndex, myMaxSubtermConstantSetIndex, myMaxVectorSubtermIndex 
            // are not defined at this stage of evaluation so set to myMaxVectorArgumentIndex
            this.myMaxCalculatedConstantIndex = this.myMaxVectorArgumentIndex;
            this.myMaxSubtermConstantIndex = this.myMaxVectorArgumentIndex;
            this.myMaxSubtermConstantSetIndex = this.myMaxVectorArgumentIndex;
            this.myMaxVectorSubtermIndex = this.myMaxVectorArgumentIndex;

            #endregion

            // Return tokens
            return tokens;
        }
        /// <summary>
        /// Checks tokens
        /// </summary>
        /// <remarks>
        /// myComment is changed if necessary
        /// </remarks>
        /// <param name="tokens">
        /// Array with tokens
        /// </param>
        /// <returns>
        /// True: Tokens represent a valid formula, 
        /// false: Otherwise
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if tokens is null or has length 0
        /// </exception>
        public Boolean CheckTokens(Int32[] tokens)
        {
            Int32 numberBracketsOpen;
            Int32 numberBracketsClose;
            Int32 numberCurlyBracketsOpen;
            Int32 numberCurlyBracketsClose;

            #region Checks

            if (tokens == null || tokens.Length == 0)
            {
                throw new MathCompilerException("Parameter tokens is null or has length 0.");
            }

            #endregion

            #region Check first token

            // First token: (, +, -, NOT, scalar argument, constant, function, vector function, IF
            if (tokens[0] != myBracketOpen &&
                tokens[0] != myAdd &&
                tokens[0] != mySubtract &&
                tokens[0] != myNot &&
                tokens[0] != myIf &&
                !IsScalarArgument(tokens[0]) &&
                !IsConstant(tokens[0]) &&
                !IsScalarFunction(tokens[0]) &&
                !IsVectorFunction(tokens[0]))
            {
                //  1 = "First token ' {0} ' is invalid.";
                this.myComment =
                    String.Format(
                    CultureInfo.InvariantCulture, 
                    MathCompiler.myComments[1], 
                    this.ConvertToTokenRepresentation(tokens[0], 0));
                this.myCodedComment = new CodedInfoItem(
                    CodedInfo.InvalidFirstToken,
                    new String[] { this.ConvertToTokenRepresentation(tokens[0], 0) });
                return false;
            }

            #endregion

            #region Check intermediate tokens

            // --------------------------------------------------------------------
            // Allowed token combinations:
            // --------------------------------------------------------------------
            // First token (FT):
            // -----------------
            // FTClass1 = (, {, \,
            // FTClass2 = ), scalar argument, constant
            // FTClass3 = <, <=, =, <>, >=, >, AND, OR, NOT, +, -, *, /, ^
            //
            // Second token (ST):
            // ------------------
            // STClass1 = (, NOT, scalar argument, constant, function, vector function, IF
            // STClass2 = ), <, <=, =, <>, >=, >, AND, OR, +, -, *, /, ^, \,
            // 
            // First token (FT)               Second Token (ST)
            // ----------------               -----------------
            // FTClass1                       STClass 1, +, -, vector argument/constant, {
            // FTClass2                       STClass 2, }
            // FTClass3                       STClass 1
            // function, vector function, IF  (
            // vector argument/constant, }    ), }, \,
            // --------------------------------------------------------------------
            if (tokens.Length > 1)
            {
                for (Int32 i = 0; i < tokens.Length - 1; i++)
                {
                    if (MathCompiler.IsInFTClass1(tokens[i]))
                    {
                        #region FTClass1
                        if (!this.IsInSTClass1(tokens[i + 1]) &&
                            tokens[i + 1] != myAdd &&
                            tokens[i + 1] != mySubtract &&
                            !this.IsVectorArgument(tokens[i + 1]) &&
                            !this.IsVectorConstant(tokens[i + 1]) &&
                            tokens[i + 1] != myCurlyBracketOpen)
                        {
                            //  2 = "Token ' {1} ' is not allowed to follow token ' {0} '.";
                            this.myComment =
                                String.Format(
                                CultureInfo.InvariantCulture,
                                MathCompiler.myComments[2],
                                this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1));
                            this.myCodedComment = new CodedInfoItem(
                                CodedInfo.InvalidFollowToken,
                                new String[] { this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1) });
                            return false;
                        }
                        #endregion
                    }
                    else if (this.IsInFTClass2(tokens[i]))
                    {
                        #region FTClass2
                        if (!MathCompiler.IsInSTClass2(tokens[i + 1]) &&
                            tokens[i + 1] != myCurlyBracketClose)
                        {
                            //  2 = "Token ' {1} ' is not allowed to follow token ' {0} '.";
                            this.myComment =
                                String.Format(
                                CultureInfo.InvariantCulture,
                                MathCompiler.myComments[2],
                                this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1],  i + 1));
                            this.myCodedComment = new CodedInfoItem(
                                CodedInfo.InvalidFollowToken,
                                new String[] { this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1) });
                            return false;
                        }
                        #endregion
                    }
                    else if (MathCompiler.IsInFTClass3(tokens[i]))
                    {
                        #region FTClass3
                        if (!this.IsInSTClass1(tokens[i + 1]))
                        {
                            //  2 = "Token ' {1} ' is not allowed to follow token ' {0} '.";
                            this.myComment =
                                String.Format(
                                CultureInfo.InvariantCulture,
                                MathCompiler.myComments[2],
                                this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1));
                            this.myCodedComment = new CodedInfoItem(
                                CodedInfo.InvalidFollowToken,
                                new String[] { this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1) });
                            return false;
                        }
                        #endregion
                    }
                    else if (this.IsScalarFunction(tokens[i]) || 
                        this.IsVectorFunction(tokens[i]) ||
                        tokens[i] == myIf)
                    {
                        #region Function, vector function or IF
                        if (tokens[i + 1] != myBracketOpen)
                        {
                            //  2 = "Token ' {1} ' is not allowed to follow token ' {0} '.";
                            this.myComment =
                                String.Format(
                                CultureInfo.InvariantCulture,
                                MathCompiler.myComments[2],
                                this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1));
                            this.myCodedComment = new CodedInfoItem(
                                CodedInfo.InvalidFollowToken,
                                new String[] { this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1) });
                            return false;
                        }
                        #endregion
                    }
                    else if (this.IsVectorArgument(tokens[i]) ||
                        this.IsVectorConstant(tokens[i]) ||
                        tokens[i] == myCurlyBracketClose)
                    {
                        #region Vector argument/constant

                        if (tokens[i + 1] != myBracketClose &&
                            tokens[i + 1] != myCurlyBracketClose &&
                            tokens[i + 1] != myComma)
                        {
                            //  2 = "Token ' {1} ' is not allowed to follow token ' {0} '.";
                            this.myComment =
                                String.Format(
                                CultureInfo.InvariantCulture,
                                MathCompiler.myComments[2],
                                this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1));
                            this.myCodedComment = new CodedInfoItem(
                                CodedInfo.InvalidFollowToken,
                                new String[] { this.ConvertToTokenRepresentation(tokens[i], i),
                                this.ConvertToTokenRepresentation(tokens[i + 1], i + 1) });
                            return false;
                        }

                        #endregion
                    }
                }
            }

            #endregion

            #region Check last token

            // Last token: ), scalar argument, constant
            if (tokens[tokens.Length - 1] != myBracketClose &&
                !IsScalarArgument(tokens[tokens.Length - 1]) &&
                !IsConstant(tokens[tokens.Length - 1]))
            {
                //  4 = "Last token ' {0} ' is invalid.";
                this.myComment =
                    String.Format(
                    CultureInfo.InvariantCulture,
                    MathCompiler.myComments[4],
                    this.ConvertToTokenRepresentation(tokens[tokens.Length - 1], tokens.Length - 1));
                    this.myCodedComment = new CodedInfoItem(
                        CodedInfo.InvalidLastToken,
                        new String[] { this.ConvertToTokenRepresentation(tokens[tokens.Length - 1], tokens.Length - 1) });
                return false;
            }

            #endregion

            #region Check normal and curly brackets

            numberBracketsOpen = 0;
            numberBracketsClose = 0;
            numberCurlyBracketsOpen = 0;
            numberCurlyBracketsClose = 0;
            for (Int32 i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == myBracketOpen)
                {
                    numberBracketsOpen++;
                }
                if (tokens[i] == myBracketClose)
                {
                    numberBracketsClose++;
                }
                if (tokens[i] == myCurlyBracketOpen)
                {
                    numberCurlyBracketsOpen++;
                }
                if (tokens[i] == myCurlyBracketClose)
                {
                    numberCurlyBracketsClose++;
                }
            }
            if (numberBracketsOpen != numberBracketsClose)
            {
                // 12 = "Unequal number of brackets: Open/Close = {0} / {1}.";
                this.myComment =
                    String.Format(
                    CultureInfo.InvariantCulture,
                    MathCompiler.myComments[12],
                    numberBracketsOpen.ToString(CultureInfo.InvariantCulture.NumberFormat),
                    numberBracketsClose.ToString(CultureInfo.InvariantCulture.NumberFormat));
                this.myCodedComment = new CodedInfoItem(
                    CodedInfo.UnequalNumberOfBrackets,
                    new String[] { numberBracketsOpen.ToString(CultureInfo.InvariantCulture.NumberFormat),
                    numberBracketsClose.ToString(CultureInfo.InvariantCulture.NumberFormat) });
                return false;
            }
            if (numberCurlyBracketsOpen != numberCurlyBracketsClose)
            {
                // 22 = Unequal number of curly brackets: Open/Close = {0} / {1}.
                this.myComment =
                    String.Format(
                    CultureInfo.InvariantCulture,
                    MathCompiler.myComments[22],
                    numberCurlyBracketsOpen.ToString(CultureInfo.InvariantCulture.NumberFormat),
                    numberCurlyBracketsClose.ToString(CultureInfo.InvariantCulture.NumberFormat));
                this.myCodedComment = new CodedInfoItem(
                    CodedInfo.UnequalNumberOfCurlyBrackets,
                    new String[] { numberCurlyBracketsOpen.ToString(CultureInfo.InvariantCulture.NumberFormat),
                    numberCurlyBracketsClose.ToString(CultureInfo.InvariantCulture.NumberFormat) });
                return false;
            }

            #endregion

            #region Check vector arguments

            for (Int32 i = 0; i < tokens.Length; i++)
            {
                #region Check vector argument/constant and vector

                if ((i < 2 &&
                    (this.IsVectorArgument(tokens[i]) || this.IsVectorConstant(tokens[i]) || tokens[i] == myCurlyBracketOpen))
                    ||
                    ((this.IsVectorArgument(tokens[i]) || this.IsVectorConstant(tokens[i]) || tokens[i] == myCurlyBracketOpen) &&
                    !((tokens[i - 1] == myBracketOpen && this.IsVectorFunction(tokens[i - 2])) || tokens[i - 1] == myComma || tokens[i - 1] == myCurlyBracketOpen)))
                {
                    //  6 = "Vector expression ' {0} ' is only allowed as an argument of a vector function.";
                    this.myComment =
                        String.Format(
                        CultureInfo.InvariantCulture,
                        MathCompiler.myComments[6],
                        this.ConvertToTokenRepresentation(tokens[i], i));
                    this.myCodedComment = new CodedInfoItem(
                        CodedInfo.InvalidVectorExpression,
                        new String[] { this.ConvertToTokenRepresentation(tokens[i], i) });
                    return false;
                }

                #endregion
            }

            #endregion

            return true;
        }
        /// <summary>
        /// Converts token to token representation
        /// </summary>
        /// <remarks>
        /// This method must NOT be optimized for speed so cases in switch statement may not be in integer sequence
        /// </remarks>
        /// <param name="token">
        /// Token
        /// </param>
        /// <param name="index">
        /// Index of token
        /// </param>
        /// <returns>
        /// Token representation or null if conversion failed
        /// </returns>
        public String ConvertToTokenRepresentation(Int32 token, Int32 index)
        {
            switch (token)
            {
                #region ,

                case myComma:
                    return myCommaSymbol;

                #endregion

                #region (, )

                case myBracketOpen:
                    return myBracketOpenSymbol;
                case myBracketClose:
                    return myBracketCloseSymbol;

                #endregion

                #region {, }

                case myCurlyBracketOpen:
                    return myCurlyBracketOpenSymbol;
                case myCurlyBracketClose:
                    return myCurlyBracketCloseSymbol;

                #endregion

                #region <,<=,=,<>,>=,>

                case myLess:
                    return myLessSymbol;
                case myLessEqual:
                    return myLessEqualSymbol;
                case myEqual:
                    return myEqualSymbol;
                case myUnequal:
                    return myUnequalSymbol;
                case myGreaterEqual:
                    return myGreaterEqualSymbol;
                case myGreater:
                    return myGreaterSymbol;

                #endregion

                #region AND,OR,NOT

                case myAnd:
                    return myAndSymbol;
                case myOr:
                    return myOrSymbol;
                case myNot:
                    return myNotSymbol;

                #endregion

                #region +,-,*,/,^

                case myAdd:
                    return myAddSymbol;
                case mySubtract:
                    return mySubtractSymbol;
                case myMultiply:
                    return myMultiplySymbol;
                case myDivide:
                    return myDivideSymbol;
                case myPower:
                    return myPowerSymbol;

                #endregion

                #region IF

                case myIf:
                    return myIfSymbol;

                #endregion

                default:

                    // NOTE: Fixed and variable calculated constants are not taken into account:
                    //       They are NOT necessary for this stage of evaluation.

                    if (this.IsFixedScalarFunction(token))
                    {
                        #region Fixed functions

                        return this.myScalarFunctions[this.CorrectIndex(token, myFixedFunctionStartToken)].Name;

                        #endregion
                    }
                    else if (this.IsVariableScalarFunction(token))
                    {
                        #region Variable functions

                        return this.myScalarFunctions[this.myCorrectIndex[index]].Name;

                        #endregion
                    }
                    else if (this.IsFixedVectorFunction(token))
                    {
                        #region Fixed vector function

                        return this.myVectorFunctions[this.CorrectIndex(token, myFixedVectorFunctionStartToken)].Name;

                        #endregion
                    }
                    else if (this.IsVariableVectorFunction(token))
                    {
                        #region Variable vector function

                        return this.myVectorFunctions[this.myCorrectIndex[index]].Name;

                        #endregion
                    }
                    else if (this.IsFixedScalarArgument(token))
                    {
                        #region Fixed scalar argument

                        // MathCompiler instance has normal formula: Return Xi
                        return "X" + this.CorrectIndex(token, myFixedScalarArgumentStartToken).ToString(CultureInfo.InvariantCulture.NumberFormat);

                        #endregion
                    }
                    else if (this.IsVariableScalarArgument(token))
                    {
                        #region Variable scalar argument

                        // MathCompiler instance has normal formula: Return Xi
                        return "X" + this.myCorrectIndex[index].ToString(CultureInfo.InvariantCulture.NumberFormat);

                        #endregion
                    }
                    else if (this.IsFixedConstant(token))
                    {
                        #region Fixed constants

                        return this.myConstants[this.CorrectIndex(token, myFixedConstantStartToken)].ToString(CultureInfo.InvariantCulture.NumberFormat);

                        #endregion
                    }
                    else if (this.IsVariableConstant(token))
                    {
                        #region Variable constants

                        return this.myConstants[this.myCorrectIndex[index]].ToString(CultureInfo.InvariantCulture.NumberFormat);

                        #endregion
                    }
                    else if (this.IsFixedVectorConstant(token))
                    {
                        #region Fixed vector constant

                        return this.myVectorConstantRepresentations[this.CorrectIndex(token, myFixedVectorConstantStartToken)];
                        
                        #endregion
                    }
                    else if (this.IsVariableVectorConstant(token))
                    {
                        #region Variable vector constant

                        return this.myVectorConstantRepresentations[this.myCorrectIndex[index]];

                        #endregion
                    }
                    else if (this.IsFixedVectorArgument(token))
                    {
                        #region Fixed vector arguments

                        // MathCompiler instance has normal formula: Return Xi{}
                        return "X" + this.CorrectIndex(token, myFixedVectorArgumentStartToken).ToString(CultureInfo.InvariantCulture.NumberFormat) + "{}";

                        #endregion
                    }
                    else if (this.IsVariableVectorArgument(token))
                    {
                        #region Variable vector arguments

                        // MathCompiler instance has normal formula: Return Xi{}
                        return "X" + this.myCorrectIndex[index].ToString(CultureInfo.InvariantCulture.NumberFormat) + "{}";

                        #endregion
                    }
                    // This should never happen: All tokens are to be known.
                    return "Unknown token with integer representation = '" + token.ToString(CultureInfo.InvariantCulture.NumberFormat) + "'.";
            }
        }

        #endregion

        #region Stack initialization method

        /// <summary>
        /// Initializes myStack with necessary size according to myCommands
        /// </summary>
        /// <remarks>
        /// This method must not be optimized for speed, so case values in switch-statement must not be in integer sequence
        /// </remarks>
        private void InitializeStack()
        {
            Int32 netStackPushCount;
            Int32 maximumStackSize;
            Int32 netVectorStackPushCount;
            Int32 maximumVectorStackSize;
            Int32 netVectorStackSupportListArrayPushCount;
            Int32 maximumVectorStackSupportListArraySize;
            Int32[] vectorStackSupportArrayInformationForNonNestedVectorFormula;
            Boolean isSuccess;

            this.CountStackPushOperations(
                this.myCommands,
                this.myCommandsPush,
                out netStackPushCount,
                out maximumStackSize,
                out netVectorStackPushCount,
                out maximumVectorStackSize,
                out netVectorStackSupportListArrayPushCount,
                out maximumVectorStackSupportListArraySize,
                out vectorStackSupportArrayInformationForNonNestedVectorFormula,
                out isSuccess);

            #region Checks

            if (!isSuccess)
            {
                return;
            }

            #endregion

            // Clear myStack
            this.myStack = new Double[maximumStackSize];
            // IMPORTANT: Clear to -1 because first Stack.Push will increment in advance
            this.myStackIndex = -1;
            // Clear myVectorStack
            this.myVectorStack = new Double[maximumVectorStackSize][];
            // IMPORTANT: Clear to -1 because first Stack.Push will increment in advance
            this.myVectorStackIndex = -1;
            // Clear myVectorStackArguments
            this.myVectorStackArguments = new Double[maximumVectorStackSize][];
            for (Int32 i = 0; i < this.myVectorStackArguments.Length; i++)
            {
                this.myVectorStackArguments[i] = new Double[] { 0 };
            }
            // Clear myVectorStackSupportListArray
            this.myVectorStackSupportListArray = new List<Double>[maximumVectorStackSupportListArraySize];
            for (Int32 i = 0; i < this.myVectorStackSupportListArray.Length; i++)
            {
                // Capacity of 100 will be sufficient in most cases
                this.myVectorStackSupportListArray[i] = new List<Double>(100);
            }
            // IMPORTANT: Clear to -1 because first Stack.Push will increment in advance
            this.myVectorStackSupportListArrayIndex = -1;
            // Check if formula does not contain nested vector and info exists
            if (!this.myHasNestedVector && vectorStackSupportArrayInformationForNonNestedVectorFormula != null)
            {
                this.myVectorStackSupportArrays = new Double[vectorStackSupportArrayInformationForNonNestedVectorFormula.Length][];
                for (Int32 i = 0; i < vectorStackSupportArrayInformationForNonNestedVectorFormula.Length; i++)
                {
                    this.myVectorStackSupportArrays[i] = new Double[vectorStackSupportArrayInformationForNonNestedVectorFormula[i]];
                }
            }
            // Initialize to 0 because there is NO increment at start
            this.myVectorStackSupportIndex = 0;
            // Initializes this.myVectorStackSupportArraysIndex to -1 since operations start with increment
            this.myVectorStackSupportArraysIndex = -1;
        }

        #endregion

        #region Add-command methods

        /// <summary>
        /// Adds command to myCommands and related arrays
        /// </summary>
        /// <param name="commandToken">
        /// Command token
        /// </param>
        /// <param name="commandRepresentation">
        /// Command representation
        /// </param>
        private void AddCommand(Int32 commandToken, String commandRepresentation)
        {
            #region Resize command related arrays if necessary

            if (this.myCommandIndex == this.myCommands.Length)
            {
                this.DoubleSizeCommandRelatedArrays();
            }

            #endregion

            this.myCommands[this.myCommandIndex] = commandToken;
            this.myJumpOffsets[this.myCommandIndex] = 0;
            this.myCorrectIndexCommands[this.myCommandIndex] = -1;
            this.myCommandsPush[this.myCommandIndex] = false;
            this.myCommandRepresentations[this.myCommandIndex++] = commandRepresentation + " (" + commandToken.ToString() + ")";
        }
        /// <summary>
        /// Adds corrected command to myCommands and related arrays
        /// </summary>
        /// <param name="commandToken">
        /// Command token
        /// </param>
        /// <param name="correctIndexCommand">
        /// Corrected command value for index access. 
        /// </param>
        /// <param name="commandRepresentation">
        /// Command representation
        /// </param>
        private void AddCorrectIndexCommand(Int32 commandToken, Int32 correctIndexCommand, String commandRepresentation)
        {
            #region Resize command related arrays if necessary

            if (this.myCommandIndex == this.myCommands.Length)
            {
                this.DoubleSizeCommandRelatedArrays();
            }

            #endregion

            this.myCommands[this.myCommandIndex] = commandToken;
            this.myJumpOffsets[this.myCommandIndex] = 0;
            this.myCorrectIndexCommands[this.myCommandIndex] = correctIndexCommand;
            this.myCommandsPush[this.myCommandIndex] = false;
            this.myCommandRepresentations[this.myCommandIndex++] = commandRepresentation + " (" + commandToken.ToString() + ")";
        }
        /// <summary>
        /// Adds jump command to myCommands and related arrays
        /// </summary>
        /// <param name="commandToken">
        /// Command token
        /// </param>
        /// <param name="jumpIdentifier">
        /// Jump identifier
        /// </param>
        /// <param name="commandRepresentation">
        /// Command representation
        /// </param>
        private void AddJumpCommand(Int32 commandToken, Int32 jumpIdentifier, String commandRepresentation)
        {
            #region Resize command related arrays if necessary

            if (this.myCommandIndex == this.myCommands.Length)
            {
                this.DoubleSizeCommandRelatedArrays();
            }

            #endregion

            this.myCommands[this.myCommandIndex] = commandToken;
            this.myJumpOffsets[this.myCommandIndex] = jumpIdentifier;
            this.myCorrectIndexCommands[this.myCommandIndex] = -1;
            this.myCommandsPush[this.myCommandIndex] = false;
            this.myCommandRepresentations[this.myCommandIndex++] = commandRepresentation + " (" + commandToken.ToString() + ")";
        }

        #endregion

        #region Evaluation of constant subterms (calculated constants) related methods

        /// <summary>
        /// Evaluates all constant subterms
        /// </summary>
        private void EvaluateAllConstantSubterms()
        {
            List<List<Int32>> constantSubtermIndexLists = this.GetAllConstantSubterms(this.myCommands, this.myCommandsPush);
            if (constantSubtermIndexLists != null)
            {
                #region Build myCalculatedConstantCommandsRepresentationSet for debug purposes

                this.myCalculatedConstantCommandsRepresentationSet = new String[constantSubtermIndexLists.Count][];
                for (Int32 i = 0; i < constantSubtermIndexLists.Count; i++)
                {
                    this.myCalculatedConstantCommandsRepresentationSet[i] = new String[constantSubtermIndexLists[i].Count];
                    for (Int32 k = 0; k < constantSubtermIndexLists[i].Count; k++)
                    {
                        this.myCalculatedConstantCommandsRepresentationSet[i][k] = this.myCommandRepresentations[constantSubtermIndexLists[i][k]];
                    }
                }

                #endregion

                #region Save all command related arrays

                Int32[] commandsSaved = new Int32[this.myCommands.Length];
                this.myCommands.CopyTo(commandsSaved, 0);

                Int32[] jumpOffsetsSaved = new Int32[this.myJumpOffsets.Length];
                this.myJumpOffsets.CopyTo(jumpOffsetsSaved, 0);

                Int32[] correctIndexCommandsSaved = new Int32[this.myCorrectIndexCommands.Length];
                this.myCorrectIndexCommands.CopyTo(correctIndexCommandsSaved, 0);

                Boolean[] commandsPushSaved = new Boolean[this.myCommandsPush.Length];
                this.myCommandsPush.CopyTo(commandsPushSaved, 0);

                String[] commandRepresentationsSaved = new String[this.myCommandRepresentations.Length];
                this.myCommandRepresentations.CopyTo(commandRepresentationsSaved, 0);

                #endregion

                #region Calculate constant values

                List<Double> newConstants = new List<double>(constantSubtermIndexLists.Count);
                foreach (List<Int32> constantSubtermIndexList in constantSubtermIndexLists)
                {
                    #region Copy to command arrays for constant value calculation

                    this.myCommands = new Int32[constantSubtermIndexList.Count];
                    this.myJumpOffsets = new Int32[constantSubtermIndexList.Count];
                    this.myCorrectIndexCommands = new Int32[constantSubtermIndexList.Count];
                    this.myCommandsPush = new Boolean[constantSubtermIndexList.Count];
                    for (Int32 i = 0; i < constantSubtermIndexList.Count; i++)
                    {
                        Int32 currentIndex = constantSubtermIndexList[i];
                        this.myCommands[i] = commandsSaved[currentIndex];
                        this.myJumpOffsets[i] = jumpOffsetsSaved[currentIndex];
                        this.myCorrectIndexCommands[i] = correctIndexCommandsSaved[currentIndex];
                        this.myCommandsPush[i] = commandsPushSaved[currentIndex];
                    }

                    #endregion

                    #region Clear myStack, myVectorStack and myVectorStackArguments

                    this.InitializeStack();

                    #endregion

                    #region Calculate constant value

                    newConstants.Add(this.CalculateSafe());

                    #endregion
                }

                #endregion

                #region Check for dublette constant values and set map

                List<Double> newDubletteFreeConstants = new List<double>(constantSubtermIndexLists.Count);
                Int32[] mapToDubletteFreeConstants = new Int32[constantSubtermIndexLists.Count];
                newDubletteFreeConstants.Add(newConstants[0]);
                mapToDubletteFreeConstants[0] = 0;
                if (constantSubtermIndexLists.Count > 1)
                {
                    for (Int32 i = 1; i < constantSubtermIndexLists.Count; i++)
                    {
                        Int32 dubletteIndex = -1;
                        for (Int32 k = 0; k < newDubletteFreeConstants.Count; k++)
                        {
                            if (newDubletteFreeConstants[k] == newConstants[i])
                            {
                                dubletteIndex = k;
                            }
                        }

                        if (dubletteIndex != -1)
                        {
                            // Dublette
                            mapToDubletteFreeConstants[i] = dubletteIndex;
                        }
                        else
                        {
                            // No dublette
                            newDubletteFreeConstants.Add(newConstants[i]);
                            mapToDubletteFreeConstants[i] = newDubletteFreeConstants.Count - 1;
                        }
                    }
                }

                #endregion

                #region Set calculated constants and all command related arrays

                // Capacity of 100 will be sufficient in most cases
                List<Int32> newCommandList = new List<Int32>(100);
                List<Int32> newJumpOffsetList = new List<Int32>(100);
                List<Int32> newCorrectIndexCommandList = new List<Int32>(100);
                List<Boolean> newCommandsPushList = new List<Boolean>(100);
                List<String> newCommandRepresentationList = new List<String>(100);

                this.myCalculatedConstants = new Double[newDubletteFreeConstants.Count];

                for (Int32 i = 0; i < commandsSaved.Length; i++)
                {
                    for (Int32 k = 0; k < constantSubtermIndexLists.Count; k++)
                    {
                        if (i == constantSubtermIndexLists[k][0])
                        {
                            #region Insert calculated constant

                            this.myCalculatedConstants[mapToDubletteFreeConstants[k]] = newDubletteFreeConstants[mapToDubletteFreeConstants[k]];
                            if (mapToDubletteFreeConstants[k] < myFixedCalculatedConstantCount)
                            {
                                #region Fixed calculated constants

                                // NOTE: *2 since every fixed calculated constant token is always followed by calculated-constant-push token
                                newCommandList.Add(myFixedCalculatedConstantStartToken + mapToDubletteFreeConstants[k] * 2);
                                newCorrectIndexCommandList.Add(-1);

                                #endregion
                            }
                            else
                            {
                                #region Variable calculated constants

                                this.myHasVariableCalculatedConstants = true;
                                Int32 token = this.myMaxVectorArgumentIndex + mapToDubletteFreeConstants[k] - myFixedCalculatedConstantCount;
                                this.myMaxCalculatedConstantIndex = token + 1;
                                this.myMaxSubtermConstantIndex = this.myMaxCalculatedConstantIndex;
                                this.myMaxSubtermConstantSetIndex = this.myMaxCalculatedConstantIndex;
                                this.myMaxVectorSubtermIndex = this.myMaxCalculatedConstantIndex;
                                newCommandList.Add(token);
                                newCorrectIndexCommandList.Add(k);

                                #endregion
                            }
                            newJumpOffsetList.Add(0);
                            newCommandsPushList.Add(false);
                            newCommandRepresentationList.Add(
                                "V = " +
                                this.myCalculatedConstants[mapToDubletteFreeConstants[k]].ToString(CultureInfo.InvariantCulture.NumberFormat) +
                                " (Calculated Constant " +
                                mapToDubletteFreeConstants[k].ToString(CultureInfo.InvariantCulture.NumberFormat) +
                                ")");

                            #endregion
                        }
                        else if (k == 0 && i < constantSubtermIndexLists[k][0] ||
                            k == constantSubtermIndexLists.Count - 1 && i > constantSubtermIndexLists[k][constantSubtermIndexLists[k].Count - 1] ||
                            k < constantSubtermIndexLists.Count - 1 && i > constantSubtermIndexLists[k][constantSubtermIndexLists[k].Count - 1] && i < constantSubtermIndexLists[k + 1][0])
                        {
                            newCommandList.Add(commandsSaved[i]);
                            newJumpOffsetList.Add(jumpOffsetsSaved[i]);
                            newCorrectIndexCommandList.Add(correctIndexCommandsSaved[i]);
                            newCommandsPushList.Add(commandsPushSaved[i]);
                            newCommandRepresentationList.Add(commandRepresentationsSaved[i]);
                        }
                    }
                }

                this.myCommands = new Int32[newCommandList.Count];
                newCommandList.CopyTo(this.myCommands);
                this.myJumpOffsets = new Int32[newJumpOffsetList.Count];
                newJumpOffsetList.CopyTo(this.myJumpOffsets);
                this.myCorrectIndexCommands = new Int32[newCorrectIndexCommandList.Count];
                newCorrectIndexCommandList.CopyTo(this.myCorrectIndexCommands);
                this.myCommandsPush = new Boolean[newCommandsPushList.Count];
                newCommandsPushList.CopyTo(this.myCommandsPush);
                this.myCommandRepresentations = new String[newCommandRepresentationList.Count];
                newCommandRepresentationList.CopyTo(this.myCommandRepresentations);

                #endregion
            }
        }
        /// <summary>
        /// Returns array of index lists with valid constant subterms
        /// </summary>
        /// <param name="commands">
        /// Command array
        /// </param>
        /// <param name="commandsPush">
        /// CommandPush array that corresponds to commands
        /// </param>
        /// <returns>
        /// Array of index lists with valid constant subterms 
        /// or null if there are no constant subterm
        /// </returns>
        private List<List<Int32>> GetAllConstantSubterms(Int32[] commands, Boolean[] commandsPush)
        {
            // Capacity of 5 will be sufficient in most cases
            List<List<Int32>> constantSubtermIndexLists = new List<List<Int32>>(5);
            Int32 startIndex = 0;
            while (startIndex < this.myCommands.Length)
            {
                List<Int32> newConstantSubtermIndexList = this.GetValidConstantSubtermFromIndex(commands, commandsPush, startIndex);
                if (newConstantSubtermIndexList != null)
                {
                    constantSubtermIndexLists.Add(newConstantSubtermIndexList);
                    startIndex = newConstantSubtermIndexList[newConstantSubtermIndexList.Count - 1] + 1;
                }
                else
                {
                    startIndex++;
                }
            }
            if (constantSubtermIndexLists.Count > 0)
            {
                return constantSubtermIndexLists;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Returns index list with valid constant subterm from index
        /// </summary>
        /// <param name="commands">
        /// Command array
        /// </param>
        /// <param name="commandsPush">
        /// CommandPush array that corresponds to commands
        /// </param>
        /// <param name="index">
        /// Index for start of search
        /// </param>
        /// <returns>
        /// Index list with valid constant subterm from index
        /// or null if there are no subterm matches
        /// </returns>
        private List<Int32> GetValidConstantSubtermFromIndex(Int32[] commands, Boolean[] commandsPush, Int32 index)
        {
            Int32 position;
            List<Int32> constantSubtermIndexList;

            #region Checks

            if ((!this.myHasConstants && !this.myHasVectorConstants) ||
                commands == null || 
                commandsPush == null ||
                index < 0 || 
                index >= commands.Length)
            {
                return null;
            }

            #endregion

            #region Find first constant value from index

            position = -1;
            for (Int32 i = index; i < commands.Length - 1; i++)
            {
                // Find constant plus Stack.Push or vector constant (is automatically pushed on vector stack)
                if ((this.IsConstant(commands[i]) && commands[i + 1] == myPush) ||
                    (this.IsConstant(commands[i]) && commands[i + 1] == myClearNewVectorStackSupportList) ||
                    this.IsVectorConstant(commands[i]))
                {
                    position = i;
                    break;
                }
            }
            if (position == -1)
            {
                // No constant plus Stack.Push found
                return null;
            }

            #endregion

            #region Find constant subterm

            // Capacity of 10 will be sufficient in most cases
            constantSubtermIndexList = new List<Int32>(10);
            if ((this.IsConstant(commands[position]) && commands[position + 1] == myPush) ||
                (this.IsConstant(commands[position]) && commands[position + 1] == myClearNewVectorStackSupportList))
            {
                // Add constant and Stack.Push: Count = 2
                constantSubtermIndexList.Add(position++);
                constantSubtermIndexList.Add(position++);
            }
            else
            {
                // Add vector constant (is automatically pushed on vector stack)
                constantSubtermIndexList.Add(position++);
            }
            Int32 lastCorrectCount = -1;
            while (position < commands.Length)
            {
                if (this.IsScalarArgument(commands[position]) ||
                    this.IsVectorArgument(commands[position]) ||
                    this.IsJumpRelatedCommand(commands[position]))
                {
                    // No valid constant subterm found up to next scalar/vector argument or jump
                    break;
                }
                if (this.IsSubtermListConstantValue(commands, commandsPush, constantSubtermIndexList))
                {
                    lastCorrectCount = constantSubtermIndexList.Count;
                }
                constantSubtermIndexList.Add(position++);
            }
            if (this.IsSubtermListConstantValue(commands, commandsPush, constantSubtermIndexList))
            {
                lastCorrectCount = constantSubtermIndexList.Count;
            }
            // Check if valid constant subterm was found
            if (lastCorrectCount <= 2)
            {
                return null;
            }
            else
            {
                if (lastCorrectCount == constantSubtermIndexList.Count)
                {
                    return constantSubtermIndexList;
                }
                else
                {
                    constantSubtermIndexList.RemoveRange(lastCorrectCount, constantSubtermIndexList.Count - lastCorrectCount);
                    return constantSubtermIndexList;
                }
            }

            #endregion
        }

        #endregion

        #region Identical subterm elimination related methods

        /// <summary>
        /// Enlarges command sets by identical subterm elimination if necessary 
        /// (i.e. if identical subterms exist). The command array with the highest index 
        /// is treated, i.e. this.myCommandSet[this.myCommandSet.Length - 1]
        /// </summary>
        /// <remarks>
        /// The smallest identical subterms are eliminated with precedence but in a successive 
        /// manner, e.g. (X0/(X1 + 1)) + 3/(X0/(X1 + 1)) first eliminates (X1 + 1) and in the 
        /// next step (X0/(X1 + 1)). This might cause overhead for subterms that are only used 
        /// once: Method EliminateSingleUsedSubterms() solves this problem afterwards.
        /// </remarks>
        private void EliminateIdenticalSubterms()
        {
            List<Int32>[] winnerLists;
            Boolean isSuccessfulElimination;

            isSuccessfulElimination = true;
            while (isSuccessfulElimination)
            {
                // Normlize arguments of symmetric operations of command set 
                this.NormalizeSymmetricOperands(
                    this.myCommandSet[this.myCommandSet.Length - 1],
                    this.myCommandRepresentationSet[this.myCommandRepresentationSet.Length - 1]);
                // Elimination process is performed for the command array with the highest index only 
                winnerLists = null;
                for (Int32 i = 0; i < this.myCommandSet[this.myCommandSet.Length - 1].Length; i++)
                {
                    List<Int32>[] subtermLists = this.GetMaximumMatchOfSubtermsFromIndex(
                        this.myCommandSet[this.myCommandSet.Length - 1],
                        this.myCommandPushSet[this.myCommandPushSet.Length - 1],
                        i);
                    if (subtermLists != null)
                    {
                        // Get smallest (!) winner
                        if (winnerLists == null || subtermLists[0].Count < winnerLists[0].Count)
                        {
                            winnerLists = subtermLists;
                        }
                    }
                }
                if (winnerLists != null)
                {
                    // Subterm list can be evaluated to a constant value
                    this.AddToCommandSet(winnerLists);
                    isSuccessfulElimination = true;
                }
                else
                {
                    isSuccessfulElimination = false;
                }
            }
        }
        /// <summary>
        /// Adds new command array for identical subterm and related arrays to command set and related sets
        /// </summary>
        /// <param name="subtermLists">
        /// Subterm lists with indices of identical subterms in command array
        /// </param>
        private void AddToCommandSet(List<Int32>[] subtermLists)
        {
            #region Local variables

            Int32 newCommandForSubterm;
            Int32 newJumpOffsetForSubterm;
            Int32 newCorrectIndexCommandForSubterm;
            Boolean newCommandPushForSubterm;
            String newCommandRepresentationForSubterm;

            Int32[] newSubtermCommands;
            Int32[] newSubtermJumpOffsets;
            Int32[] newSubtermCorrectIndexCommands;
            Boolean[] newSubtermCommandsPush;
            String[] newSubtermCommandRepresentations;

            List<Int32> newCommandList;
            List<Int32> newJumpOffsetList;
            List<Int32> newCorrectIndexCommandList;
            List<Boolean> newCommandsPushList;
            List<String> newCommandRepresentationList;

            Int32[] newCommands;
            Int32[] newJumpOffsets;
            Int32[] newCorrectIndexCommands;
            Boolean[] newCommandsPush;
            String[] newCommandRepresentations;

            #endregion

            #region Set array size of subterm constants and new command for subterm and related variables

            if (this.mySubtermConstants == null)
            {
                #region Fixed subterm constant

                newCommandForSubterm = myFixedSubtermConstantStartToken;
                newJumpOffsetForSubterm = 0;
                newCorrectIndexCommandForSubterm = -1;
                newCommandPushForSubterm = false;
                newCommandRepresentationForSubterm =
                    "V = SubtermConstants[0]" + " (" + newCommandForSubterm.ToString() + ")";
                this.mySubtermConstants = new Double[1];

                #endregion
            }
            else
            {
                if (this.mySubtermConstants.Length < myFixedSubtermConstantCount)
                {
                    #region Fixed subterm constant

                    // Factor 2 since newCommandForSubterm is no push command
                    newCommandForSubterm = myFixedSubtermConstantStartToken + this.mySubtermConstants.Length * 2;
                    newJumpOffsetForSubterm = 0;
                    newCorrectIndexCommandForSubterm = -1;
                    newCommandPushForSubterm = false;

                    #endregion
                }
                else
                {
                    #region Variable subterm constant

                    newCommandForSubterm = this.myMaxCalculatedConstantIndex + this.mySubtermConstants.Length - myFixedSubtermConstantCount;
                    this.myMaxSubtermConstantIndex = newCommandForSubterm + 1;
                    this.myMaxSubtermConstantSetIndex = this.myMaxSubtermConstantIndex;
                    this.myMaxVectorSubtermIndex = this.myMaxSubtermConstantIndex;
                    newCommandPushForSubterm = false;
                    newCorrectIndexCommandForSubterm = this.mySubtermConstants.Length;
                    newJumpOffsetForSubterm = 0;

                    #endregion
                }
                newCommandRepresentationForSubterm = 
                    "V = SubtermConstants[" + 
                    this.mySubtermConstants.Length.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                    "]" + " (" + newCommandForSubterm.ToString() + ")";
                MathCompiler.IncrementArraySize<Double>(ref this.mySubtermConstants);
            }

            #endregion

            #region Set new subterm commands and related arrays

            newSubtermCommands = new Int32[subtermLists[0].Count];
            newSubtermJumpOffsets = new Int32[subtermLists[0].Count];
            newSubtermCorrectIndexCommands = new Int32[subtermLists[0].Count];
            newSubtermCommandsPush = new Boolean[subtermLists[0].Count];
            newSubtermCommandRepresentations = new String[subtermLists[0].Count];
            for (Int32 i = 0; i < subtermLists[0].Count; i++)
            {
                newSubtermCommands[i] = this.myCommandSet[this.myCommandSet.Length - 1][subtermLists[0][i]];
                newSubtermJumpOffsets[i] = this.myJumpOffsetSet[this.myJumpOffsetSet.Length - 1][subtermLists[0][i]];
                newSubtermCorrectIndexCommands[i] = this.myCorrectIndexCommandSet[this.myCorrectIndexCommandSet.Length - 1][subtermLists[0][i]];
                newSubtermCommandsPush[i] = this.myCommandPushSet[this.myCommandPushSet.Length - 1][subtermLists[0][i]];
                newSubtermCommandRepresentations[i] = this.myCommandRepresentationSet[this.myCommandRepresentationSet.Length - 1][subtermLists[0][i]];
            }

            #endregion

            #region Set new commands and related arrays

            // Capacity of 100 will be sufficient in most cases
            newCommandList = new List<Int32>(100);
            newJumpOffsetList = new List<Int32>(100);
            newCorrectIndexCommandList = new List<Int32>(100);
            newCommandsPushList = new List<Boolean>(100);
            newCommandRepresentationList = new List<String>(100);

            int k = 0;
            int j = 0;
            Boolean newFlag = true;
            Boolean lastCommandsFlag = false;
            for (Int32 i = 0; i < this.myCommandSet[this.myCommandSet.Length - 1].Length; i++)
            {
                if (lastCommandsFlag)
                {
                    newCommandList.Add(this.myCommandSet[this.myCommandSet.Length - 1][i]);
                    newJumpOffsetList.Add(this.myJumpOffsetSet[this.myJumpOffsetSet.Length - 1][i]);
                    newCorrectIndexCommandList.Add(this.myCorrectIndexCommandSet[this.myCorrectIndexCommandSet.Length - 1][i]);
                    newCommandsPushList.Add(this.myCommandPushSet[this.myCommandPushSet.Length - 1][i]);
                    newCommandRepresentationList.Add(this.myCommandRepresentationSet[this.myCommandRepresentationSet.Length - 1][i]);
                }
                else if (i < subtermLists[k][j])
                {
                    newCommandList.Add(this.myCommandSet[this.myCommandSet.Length - 1][i]);
                    newJumpOffsetList.Add(this.myJumpOffsetSet[this.myJumpOffsetSet.Length - 1][i]);
                    newCorrectIndexCommandList.Add(this.myCorrectIndexCommandSet[this.myCorrectIndexCommandSet.Length - 1][i]);
                    newCommandsPushList.Add(this.myCommandPushSet[this.myCommandPushSet.Length - 1][i]);
                    newCommandRepresentationList.Add(this.myCommandRepresentationSet[this.myCommandRepresentationSet.Length - 1][i]);
                }
                else if (i == subtermLists[k][j])
                {
                    if (newFlag)
                    {
                        newCommandList.Add(newCommandForSubterm);
                        newJumpOffsetList.Add(newJumpOffsetForSubterm);
                        newCorrectIndexCommandList.Add(newCorrectIndexCommandForSubterm);
                        newCommandsPushList.Add(newCommandPushForSubterm);
                        newCommandRepresentationList.Add(newCommandRepresentationForSubterm);
                        newFlag = false;
                    }
                    j++;
                    if (j == subtermLists[k].Count)
                    {
                        k++;
                        if (k == subtermLists.Length)
                        {
                            lastCommandsFlag = true;
                        }
                        else
                        {
                            newFlag = true;
                            j = 0;
                        }
                    }
                }
            }

            #endregion

            #region Increment myCommandSet and related sets

            MathCompiler.IncrementSetSize<Int32>(ref this.myCommandSet);
            MathCompiler.IncrementSetSize<Int32>(ref this.myJumpOffsetSet);
            MathCompiler.IncrementSetSize<Int32>(ref this.myCorrectIndexCommandSet);
            MathCompiler.IncrementSetSize<Boolean>(ref this.myCommandPushSet);
            MathCompiler.IncrementSetSize<String>(ref this.myCommandRepresentationSet);

            #endregion

            #region Assign to incremented sets

            this.myCommandSet[this.myCommandSet.Length - 2] = newSubtermCommands;
            this.myJumpOffsetSet[this.myJumpOffsetSet.Length - 2] = newSubtermJumpOffsets;
            this.myCorrectIndexCommandSet[this.myCorrectIndexCommandSet.Length - 2] = newSubtermCorrectIndexCommands;
            this.myCommandPushSet[this.myCommandPushSet.Length - 2] = newSubtermCommandsPush;
            this.myCommandRepresentationSet[this.myCommandRepresentationSet.Length - 2] = newSubtermCommandRepresentations;

            newCommands = new Int32[newCommandList.Count];
            newCommandList.CopyTo(newCommands);
            newJumpOffsets = new Int32[newJumpOffsetList.Count];
            newJumpOffsetList.CopyTo(newJumpOffsets);
            newCorrectIndexCommands = new Int32[newCorrectIndexCommandList.Count];
            newCorrectIndexCommandList.CopyTo(newCorrectIndexCommands);
            newCommandsPush = new Boolean[newCommandsPushList.Count];
            newCommandsPushList.CopyTo(newCommandsPush);
            newCommandRepresentations = new String[newCommandRepresentationList.Count];
            newCommandRepresentationList.CopyTo(newCommandRepresentations);

            this.myCommandSet[this.myCommandSet.Length - 1] = newCommands;
            this.myJumpOffsetSet[this.myJumpOffsetSet.Length - 1] = newJumpOffsets;
            this.myCorrectIndexCommandSet[this.myCorrectIndexCommandSet.Length - 1] = newCorrectIndexCommands;
            this.myCommandPushSet[this.myCommandPushSet.Length - 1] = newCommandsPush;
            this.myCommandRepresentationSet[this.myCommandRepresentationSet.Length - 1] = newCommandRepresentations;

            #endregion
        }
        /// <summary>
        /// Returns array of lists with maximum valid (!) subterm matches of size greater than 1 in commands from index
        /// </summary>
        /// <param name="commands">
        /// Command array
        /// </param>
        /// <param name="commandsPush">
        /// CommandPush array that corresponds to commands
        /// </param>
        /// <param name="index">
        /// Index for start of search
        /// </param>
        /// <returns>
        /// Array of lists with maximum valid (!) subterm matches of size greater than 1 in commands from index
        /// or null if there are no subterm matches
        /// </returns>
        private List<Int32>[] GetMaximumMatchOfSubtermsFromIndex(Int32[] commands, Boolean[] commandsPush, Int32 index)
        {
            Int32 position;
            List<List<Int32>> overlapFreeLists;

            #region Checks

            if (commands == null ||
                commandsPush == null ||
                index < 0 || 
                index >= commands.Length)
            {
                return null;
            }

            #endregion

            List<Int32> indexList = this.GetIndexListOfStartCommand(commands, index);
            if (indexList == null || indexList.Count < 2)
            {
                #region Start command index list could NOT be created

                return null;

                #endregion
            }
            else
            {
                #region Build subterm lists

                List<Int32>[] subtermLists = new List<Int32>[indexList.Count];
                for (Int32 i = 0; i < subtermLists.Length; i++)
                {
                    // Capacity of 10 will be sufficient in most cases
                    subtermLists[i] = new List<Int32>(10);
                    subtermLists[i].Add(indexList[i]);
                }
                // Build minimum valid subtermlist[0]
                position = subtermLists[0][subtermLists[0].Count - 1] + 1;
                while (position < commands.Length)
                {
                    subtermLists[0].Add(position);
                    if (commands[position] != myPush &&
                        this.IsSubtermListConstantValue(commands, commandsPush, subtermLists[0]))
                    {
                        break;
                    }
                    else
                    {
                        position++;
                    }
                }
                // Check if valid minimum subterm was found
                if (subtermLists[0].Count == 1)
                {
                    return null;
                }
                // Enlarge other subterm lists
                for (Int32 i = 1; i < subtermLists.Length; i++)
                {
                    for (Int32 k = 0; k < subtermLists[0].Count - 1; k++)
                    {
                        if (subtermLists[i][k] + 1 < commands.Length &&
                            commands[subtermLists[i][k] + 1] == commands[subtermLists[0][k] + 1])
                        {
                            subtermLists[i].Add(subtermLists[i][k] + 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                #endregion

                #region Remove smaller lists

                subtermLists = this.RemoveSmallerLists(subtermLists);
                if (subtermLists == null)
                {
                    return null;
                }

                #endregion

                #region Remove overlapping lists

                overlapFreeLists = new List<List<Int32>>(subtermLists.Length);
                overlapFreeLists.Add(subtermLists[0]);
                for (Int32 i = 1; i < subtermLists.Length; i++)
                {
                    if (subtermLists[i][0] > subtermLists[i - 1][subtermLists[i - 1].Count - 1])
                    {
                        overlapFreeLists.Add(subtermLists[i]);
                    }
                }
                if (overlapFreeLists.Count == 1)
                {
                    return null;
                }
                else if (overlapFreeLists.Count < subtermLists.Length)
                {
                    subtermLists = new List<Int32>[overlapFreeLists.Count];
                    overlapFreeLists.CopyTo(subtermLists);
                }

                #endregion

                #region Check if subtermLists[0] has more than 1 command index and return

                if (subtermLists[0].Count == 1)
                {
                    return null;
                }
                else
                {
                    return subtermLists;
                }

                #endregion
            }
        }
        /// <summary>
        /// Returns list with occurences of same start command from index to end of commands
        /// </summary>
        /// <param name="commands">
        /// Command array
        /// </param>
        /// <param name="index">
        /// index for start of search
        /// </param>
        /// <returns>
        /// List with occurences of same start command from index to end of commands 
        /// or null if there are none of these occurences
        /// </returns>
        private List<Int32> GetIndexListOfStartCommand(Int32[] commands, Int32 index)
        {
            #region Checks

            if (commands == null || index < 0 || index >= commands.Length)
            {
                return null;
            }

            #endregion

            List<Int32> indexList = null;
            for (Int32 i = index; i < commands.Length; i++)
            {
                if (this.IsStartCommand(commands[i]))
                {
                    // Capacity of 10 will be sufficient in most cases
                    indexList = new List<Int32>(10);
                    indexList.Add(i);
                    for (Int32 k = i + 1; k < commands.Length; k++)
                    {
                        if (commands[k] == commands[i])
                        {
                            indexList.Add(k);
                        }
                    }
                    if (indexList.Count > 1)
                    {
                        break;
                    }
                    else
                    {
                        indexList = null;
                    }
                }
            }
            return indexList;
        }
        /// <summary>
        /// Checks if command is a start command for subterm detection
        /// </summary>
        /// <param name="command">
        /// Command
        /// </param>
        /// <returns>
        /// True: Command is a start command, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsStartCommand(Int32 command)
        {
            return 
                this.IsScalarArgument(command) || 
                this.IsConstant(command) ||
                this.IsVectorConstant(command) ||
                this.IsVectorArgument(command) || 
                this.IsSubtermConstant(command);
        }
        /// <summary>
        /// Returns array of subterm lists with all lists (including list 0) that have the same size as list with index 0
        /// </summary>
        /// <param name="subtermLists">
        /// Array of subterm lists
        /// </param>
        /// <returns>
        /// Array of subterm lists with all lists (including list 0) that have the same size as list with index 0 
        /// or null if subtermLists is empty or does not contain another list with size of list 0
        /// </returns>
        private List<Int32>[] RemoveSmallerLists(List<Int32>[] subtermLists)
        {
            #region Checks

            if (subtermLists == null || subtermLists.Length < 2)
            {
                return null;
            }

            #endregion

            List<List<Int32>> listOfSubtermLists = new List<List<Int32>>(subtermLists.Length);
            listOfSubtermLists.Add(subtermLists[0]);
            for (Int32 i = 1; i < subtermLists.Length; i++)
            {
                if (subtermLists[i].Count == subtermLists[0].Count)
                {
                    listOfSubtermLists.Add(subtermLists[i]);
                }
            }
            if (listOfSubtermLists.Count == 1)
            {
                return null;
            }
            else
            {
                List<Int32>[] newSubtermLists = new List<Int32>[listOfSubtermLists.Count];
                listOfSubtermLists.CopyTo(newSubtermLists, 0);
                return newSubtermLists;
            }
        }
        /// <summary>
        /// Checks if subterm list can be evaluated to a constant value
        /// </summary>
        /// <param name="commands">
        /// Command array
        /// </param>
        /// <param name="commandsPush">
        /// CommandPush array that corresponds to commands
        /// </param>
        /// <param name="subtermList">
        /// Subterm list
        /// </param>
        /// <returns>
        /// True: Subterm list can be evaluated to a constant value, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsSubtermListConstantValue(Int32[] commands, Boolean[] commandsPush, List<Int32> subtermList)
        {
            Int32[] subCommands;
            Boolean[] subCommandsPush;
            Int32 netStackPushCount;
            Int32 maximumStackSize;
            Int32 netVectorStackPushCount;
            Int32 maximumVectorStackSize;
            Int32 netVectorStackSupportListArrayPushCount;
            Int32 maximumVectorStackSupportListArraySize;
            Int32[] dummy;
            Boolean isSuccess;
            Int32 completeCounter;
            Boolean hasVectorFunction;

            #region Checks

            // Subterm list must at least contain 2 command indizes
            if (commands == null || subtermList == null || commands.Length < 2 || subtermList.Count < 2)
            {
                return false;
            }

            #endregion

            #region Build array with sub commands

            subCommands = new Int32[subtermList.Count];
            subCommandsPush = new Boolean[subtermList.Count];
            for (Int32 i = 0; i < subCommands.Length; i++)
            {
                subCommands[i] = commands[subtermList[i]];
                subCommandsPush[i] = commandsPush[subtermList[i]];
            }

            #endregion

            #region Check jumps

            if (this.myHasJump)
            {
                completeCounter = 0;
                foreach (Int32 subCommand in subCommands)
                {
                    if (subCommand == myFalseJump)
                    {
                        completeCounter++;
                    }
                    else if (subCommand == myJumpEntry)
                    {
                        completeCounter++;
                    }
                    else if (subCommand == myFalseJumpEntry)
                    {
                        completeCounter++;
                    }
                    else if (subCommand == myJump)
                    {
                        completeCounter++;
                    }
                    if (completeCounter == 4)
                    {
                        completeCounter = 0;
                    }
                }
                if (completeCounter != 0)
                {
                    // Not a valid subterm
                    return false;
                }
            }

            #endregion

            #region Check vector arguments

            if (this.myHasVectorArguments)
            {
                // Set hasVectorFunction to true for initialization only
                hasVectorFunction = true;
                foreach (Int32 subCommand in subCommands)
                {
                    if (this.IsVectorArgument(subCommand))
                    {
                        hasVectorFunction = false;
                    }
                    else if (this.IsVectorFunction(subCommand))
                    {
                        hasVectorFunction = true;
                    }
                }
                if (!hasVectorFunction)
                {
                    // Not a valid subterm
                    return false;
                }
            }

            #endregion

            #region Check vector constants

            if (this.myHasVectorConstants)
            {
                // Set hasVectorFunction to true for initialization only
                hasVectorFunction = true;
                foreach (Int32 subCommand in subCommands)
                {
                    if (this.IsVectorConstant(subCommand))
                    {
                        hasVectorFunction = false;
                    }
                    else if (this.IsVectorFunction(subCommand))
                    {
                        hasVectorFunction = true;
                    }
                }
                if (!hasVectorFunction)
                {
                    // Not a valid subterm
                    return false;
                }
            }

            #endregion

            #region Check vectors

            if (this.myHasVector)
            {
                completeCounter = 0;
                // Set hasVectorFunction to true for initialization only
                hasVectorFunction = true;
                foreach (Int32 subCommand in subCommands)
                {
                    if (subCommand == myClearNewVectorStackSupportList)
                    {
                        completeCounter++;
                        hasVectorFunction = false;
                    }
                    else if (subCommand == myAddScalarValueToVectorStackSupportList)
                    {
                        if (completeCounter <= 0)
                        {
                            // Not a valid subterm
                            return false;
                        }
                        hasVectorFunction = false;
                    }
                    else if (subCommand == myPushVectorStackSupportListToVectorStack)
                    {
                        completeCounter--;
                        hasVectorFunction = false;
                    }
                    else if (this.IsVectorFunction(subCommand))
                    {
                        hasVectorFunction = true;
                    }
                }
                if (completeCounter != 0 || !hasVectorFunction)
                {
                    // Not a valid subterm
                    return false;
                }
            }

            #endregion

            #region Count push and pop operations for subCommands

            this.CountStackPushOperations(
                subCommands,
                subCommandsPush,
                out netStackPushCount,
                out maximumStackSize,
                out netVectorStackPushCount,
                out maximumVectorStackSize,
                out netVectorStackSupportListArrayPushCount,
                out maximumVectorStackSupportListArraySize,
                out dummy,
                out isSuccess);

            #region Checks

            if (!isSuccess)
            {
                return false;
            }

            #endregion

            #endregion

            #region Check net counts and return result

            return netStackPushCount == 0 && netVectorStackPushCount == 0 && netVectorStackSupportListArrayPushCount == 0;

            #endregion
        }
        /// <summary>
        /// Normalizes (sorts) the arguments of symmetric operations (+, *, equal, unequal) due to their token value
        /// </summary>
        /// <remarks>
        /// NOTE: All commandsPush that correspond to commands must be FALSE at this point of evaluation.
        /// </remarks>
        /// <param name="commands">
        /// Commands
        /// </param>
        /// <param name="commandRepresentations">
        /// Command representations that correspond to commands
        /// </param>
        private void NormalizeSymmetricOperands(Int32[] commands, String[] commandRepresentations)
        {
            this.NormalizeSymmetricOperands(commands, commandRepresentations, myAdd);
            this.NormalizeSymmetricOperands(commands, commandRepresentations, myMultiply);
            this.NormalizeSymmetricOperands(commands, commandRepresentations, myEqual);
            this.NormalizeSymmetricOperands(commands, commandRepresentations, myUnequal);
        }
        /// <summary>
        /// Normalizes (sorts) the arguments of symmetric operations (+, *, equal, unequal) due to their token value
        /// </summary>
        /// <remarks>
        /// NOTE: All commandsPush that correspond to commands must be FALSE at this point of evaluation.
        /// </remarks>
        /// <param name="commands">
        /// Commands
        /// </param>
        /// <param name="symmetricCommand">
        /// Symmetric command (+, *, equal, unequal)
        /// </param>
        /// <param name="commandRepresentations">
        /// Command representations that correspond to commands
        /// </param>
        private void NormalizeSymmetricOperands(Int32[] commands, String[] commandRepresentations, Int32 symmetricCommand)
        {
            Int32 startIndex;
            Int32 endIndex;
            List<Int32> argumentList;
            Int32 i;
            Int32 j;
            Dictionary<Int32, String> commandToRepresentationDictionary;

            #region Checks

            // Command array must at least have a length of 4 commands
            if (commands == null || 
                commands.Length < 4 ||
                !(symmetricCommand == myAdd || 
                symmetricCommand == myMultiply || 
                symmetricCommand == myEqual ||
                symmetricCommand == myUnequal))
            {
                return;
            }

            #endregion

            // Capacity of 15 will be sufficient in most cases
            commandToRepresentationDictionary = new Dictionary<Int32, String>(15);
            i = 0;
            while (i < commands.Length)
            {
                if (this.IsScalarArgument(commands[i]) ||
                    this.IsConstant(commands[i]) ||
                    this.IsSubtermConstant(commands[i]))
                {
                    startIndex = i;
                    endIndex = i;
                    if (i + 3 < commands.Length)
                    {
                        while (commands[endIndex + 1] == myPush &&
                            (this.IsScalarArgument(commands[endIndex + 2]) ||
                            this.IsConstant(commands[endIndex + 2]) ||
                            this.IsSubtermConstant(commands[endIndex + 2])) &&
                            commands[endIndex + 3] == symmetricCommand)
                        {
                            endIndex = endIndex + 3;
                            if (endIndex + 3 >= commands.Length)
                            {
                                break;
                            }
                        }
                        argumentList = new List<int>(endIndex - startIndex + 1);
                        for (Int32 k = startIndex; k <= endIndex; k++)
                        {
                            if (this.IsScalarArgument(commands[k]) ||
                                this.IsConstant(commands[k]) ||
                                this.IsSubtermConstant(commands[k]))
                            {
                                argumentList.Add(commands[k]);
                                if (!commandToRepresentationDictionary.ContainsKey(commands[k]))
                                {
                                    commandToRepresentationDictionary.Add(commands[k], commandRepresentations[k]);
                                }
                            }
                        }
                        argumentList.Sort();
                        j = 0;
                        for (Int32 k = startIndex; k <= endIndex; k++)
                        {
                            if (this.IsScalarArgument(commands[k]) ||
                                this.IsConstant(commands[k]) ||
                                this.IsSubtermConstant(commands[k]))
                            {
                                commands[k] = argumentList[j];
                                commandRepresentations[k] = commandToRepresentationDictionary[argumentList[j++]];
                            }
                        }
                    }
                    i = endIndex + 1;
                }
                else
                {
                    i++;
                }
            }
        }

        #endregion

        #region Single subterm elimination related methods

        /// <summary>
        /// Eliminates subterms that are only used once
        /// </summary>
        private void EliminateSingleUsedSubterms()
        {
            #region Local variables

            Dictionary<Int32, Int32> subtermIndexToCommandSetIndexDictionary;
            Dictionary<Int32, Int32> oldToNewSubtermIndexDictionary;
            Dictionary<Int32, Int32> newToOldCommandSetIndexDictionary;
            Int32[] subtermIndices;
            List<Int32> commandList;
            List<Int32> jumpOffsetList;
            List<Int32> correctIndexCommandList;
            List<Boolean> commandPushList;
            List<String> commandRepresentationList;
            Int32 j;

            #endregion

            #region Checks

            if (this.mySubtermConstants == null)
            {
                // Command array has no subterms
                return;
            }

            #endregion

            // Get subterm indices that only occur once
            subtermIndexToCommandSetIndexDictionary = this.GetCountOneSubterms();
            if (subtermIndexToCommandSetIndexDictionary != null)
            {
                #region Sort (ascending) subterm indices that only occur once

                subtermIndices = new Int32[subtermIndexToCommandSetIndexDictionary.Count];
                subtermIndexToCommandSetIndexDictionary.Keys.CopyTo(subtermIndices, 0);
                Array.Sort(subtermIndices);

                #endregion

                #region Create mapping from old to new subterm indices

                oldToNewSubtermIndexDictionary = new Dictionary<Int32,Int32>(this.mySubtermConstants.Length);
                j = 0;
                for (Int32 i = 0; i < this.mySubtermConstants.Length; i++)
                {
                    if (j == subtermIndices.Length)
                    {
                        // i > subtermIndices[maxIndex]
                        oldToNewSubtermIndexDictionary.Add(i, i - j);
                    }
                    else if (i < subtermIndices[j])
                    {
                        oldToNewSubtermIndexDictionary.Add(i, i - j);
                    }
                    else if (i == subtermIndices[j])
                    {
                        oldToNewSubtermIndexDictionary.Add(i, -1);
                        j++;
                    }
                }

                #endregion

                #region Create mapping from old to new myCommandSet indices

                newToOldCommandSetIndexDictionary = new Dictionary<Int32, Int32>(this.myCommandSet.Length - subtermIndices.Length);
                j = 0;
                for (Int32 i = 0; i < this.myCommandSet.Length; i++)
                {
                    if (j == subtermIndices.Length)
                    {
                        // i > subtermIndices[maxIndex]
                        newToOldCommandSetIndexDictionary.Add(i - j, i);
                    }
                    else if (i < subtermIndices[j])
                    {
                        newToOldCommandSetIndexDictionary.Add(i - j, i);
                    }
                    else if (i == subtermIndices[j])
                    {
                        j++;
                    }
                }

                #endregion

                // Loop over subterm indices that only occur once to eliminate them
                for (Int32 i = 0; i < subtermIndices.Length; i++)
                {
                    Int32 subtermIndex = subtermIndices[i];
                    Int32 commandSetIndex = subtermIndexToCommandSetIndexDictionary[subtermIndex];

                    #region Clear command list and related lists

                    // Capacity of 50 will be sufficient in most cases
                    commandList = new List<Int32>(50);
                    jumpOffsetList = new List<Int32>(50);
                    correctIndexCommandList = new List<Int32>(50);
                    commandPushList = new List<Boolean>(50);
                    commandRepresentationList = new List<String>(50);

                    #endregion

                    for (Int32 k = 0; k < this.myCommandSet[commandSetIndex].Length; k++)
                    {
                        if (this.IsSubtermConstant(this.myCommandSet[commandSetIndex][k]) &&
                            this.GetIndexOfSubterm(this.myCommandSet[commandSetIndex][k]) == subtermIndex)
                        {
                            #region Insert myCommandSet[subtermIndex] for subterm that only occurs once

                            for (Int32 l = 0; l < this.myCommandSet[subtermIndex].Length; l++)
                            {
                                commandList.Add(this.myCommandSet[subtermIndex][l]);
                                jumpOffsetList.Add(this.myJumpOffsetSet[subtermIndex][l]);
                                correctIndexCommandList.Add(this.myCorrectIndexCommandSet[subtermIndex][l]);
                                commandPushList.Add(this.myCommandPushSet[subtermIndex][l]);
                                commandRepresentationList.Add(this.myCommandRepresentationSet[subtermIndex][l]);
                            }
                            // this.myCommandSet[subtermIndex] is no longer used:
                            // Set to null to mark for easier debugging purposes
                            this.myCommandSet[subtermIndex] = null;
                            this.myCommandRepresentationSet[subtermIndex] = null;

                            #endregion
                        }
                        else
                        {
                            #region Add existing commands and related values to their lists

                            commandList.Add(this.myCommandSet[commandSetIndex][k]);
                            jumpOffsetList.Add(this.myJumpOffsetSet[commandSetIndex][k]);
                            correctIndexCommandList.Add(this.myCorrectIndexCommandSet[commandSetIndex][k]);
                            commandPushList.Add(this.myCommandPushSet[commandSetIndex][k]);
                            commandRepresentationList.Add(this.myCommandRepresentationSet[commandSetIndex][k]);

                            #endregion
                        }
                    }

                    #region Copy command list and related lists to myCommandSet and related arrays

                    this.myCommandSet[commandSetIndex] = new Int32[commandList.Count];
                    commandList.CopyTo(this.myCommandSet[commandSetIndex]);
                    this.myJumpOffsetSet[commandSetIndex] = new Int32[jumpOffsetList.Count] ;
                    jumpOffsetList.CopyTo(this.myJumpOffsetSet[commandSetIndex]);
                    this.myCorrectIndexCommandSet[commandSetIndex] = new Int32[correctIndexCommandList.Count];
                    correctIndexCommandList.CopyTo(this.myCorrectIndexCommandSet[commandSetIndex]);
                    this.myCommandPushSet[commandSetIndex] = new Boolean[commandPushList.Count];
                    commandPushList.CopyTo(this.myCommandPushSet[commandSetIndex]);
                    this.myCommandRepresentationSet[commandSetIndex] = new String[commandRepresentationList.Count];
                    commandRepresentationList.CopyTo(this.myCommandRepresentationSet[commandSetIndex]);

                    #endregion
                }

                #region Shrink myCommandSet and related arrays to new size after copying

                for (Int32 i = 0; i < newToOldCommandSetIndexDictionary.Count; i++)
                {
                    Int32 oldIndex = newToOldCommandSetIndexDictionary[i];
                    this.myCommandSet[i] = this.myCommandSet[oldIndex];
                    this.myJumpOffsetSet[i] = this.myJumpOffsetSet[oldIndex];
                    this.myCorrectIndexCommandSet[i] = this.myCorrectIndexCommandSet[oldIndex];
                    this.myCommandPushSet[i] = this.myCommandPushSet[oldIndex];
                    this.myCommandRepresentationSet[i] = this.myCommandRepresentationSet[oldIndex];
                }
                MathCompiler.ShrinkArraySize<Int32[]>(ref this.myCommandSet, newToOldCommandSetIndexDictionary.Count);
                MathCompiler.ShrinkArraySize<Int32[]>(ref this.myJumpOffsetSet, newToOldCommandSetIndexDictionary.Count);
                MathCompiler.ShrinkArraySize<Int32[]>(ref this.myCorrectIndexCommandSet, newToOldCommandSetIndexDictionary.Count);
                MathCompiler.ShrinkArraySize<Boolean[]>(ref this.myCommandPushSet, newToOldCommandSetIndexDictionary.Count);
                MathCompiler.ShrinkArraySize<String[]>(ref this.myCommandRepresentationSet, newToOldCommandSetIndexDictionary.Count);

                #endregion

                #region Shrink mySubtermConstants to reduced size

                MathCompiler.ShrinkArraySize<Double>(ref this.mySubtermConstants, this.mySubtermConstants.Length - subtermIndices.Length);

                #endregion

                #region Substitute old subterm indices that occur more than once with their new values

                if (this.myCommandSet.Length > 1)
                {
                    for (Int32 i = 1; i < this.myCommandSet.Length; i++)
                    {
                        for (Int32 k = 0; k < this.myCommandSet[i].Length; k++)
                        {
                            if (this.IsSubtermConstant(this.myCommandSet[i][k]))
                            {
                                Int32 oldIndexOfSubterm = this.GetIndexOfSubterm(this.myCommandSet[i][k]);
                                Int32 newIndexOfSubterm = oldToNewSubtermIndexDictionary[oldIndexOfSubterm];

                                // myCommandSet is NOT allowed to contain push commands at this point
                                this.myCommandSet[i][k] =
                                    this.GetSubtermCommandToken(newIndexOfSubterm);
                                // this.myJumpOffsetSet[k][j] is not to be changed
                                // this.myCorrectIndexCommandSet[k][j] is not to be changed
                                // this.myCommandPushSet[k][j] is not to be changed
                                this.myCommandRepresentationSet[i][k] =
                                    "V = SubtermConstants[" +
                                    newIndexOfSubterm.ToString(CultureInfo.InvariantCulture.NumberFormat) +
                                    "]" + " (" + this.myCommandSet[i][k].ToString() + ")";
                            }
                        }
                    }
                }

                #endregion
            }
        }
        /// <summary>
        /// Returns subterms that only occur once in myCommandSet
        /// </summary>
        /// <returns>
        /// Dictionary with subterm indices and corresponding index of myCommandSet 
        /// that only occur once or null if no subterm occurs only once
        /// </returns>
        private Dictionary<Int32, Int32> GetCountOneSubterms()
        {
            Dictionary<Int32, Int32> subtermIndexToCommandSetIndexDictionary;

            subtermIndexToCommandSetIndexDictionary = new Dictionary<Int32, Int32>(this.mySubtermConstants.Length);
            for (Int32 i = 0; i < this.mySubtermConstants.Length; i++)
            {
                // Set default index for myCommandSet to -1 (i.e. "impossible")
                subtermIndexToCommandSetIndexDictionary.Add(i, -1);
            }
            for (Int32 i = 0; i < this.myCommandSet.Length; i++)
            {
                for (Int32 k = 0; k < this.myCommandSet[i].Length; k++)
                {
                    if (this.IsSubtermConstant(this.myCommandSet[i][k]))
                    {
                        Int32 value;
                        Int32 index = this.GetIndexOfSubterm(this.myCommandSet[i][k]);
                        if (subtermIndexToCommandSetIndexDictionary.TryGetValue(index, out value))
                        {
                            if (value == -1)
                            {
                                // Subterm occures for the first time
                                subtermIndexToCommandSetIndexDictionary[index] = i;
                            }
                            else
                            {
                                // Subterm already occured and now occurs for the second time: Remove
                                subtermIndexToCommandSetIndexDictionary.Remove(index);
                            }
                        }
                        // Else case means: Subterm index is already removed, i.e. occured more than once
                    }
                }
            }
            if (subtermIndexToCommandSetIndexDictionary.Count > 0)
            {
                return subtermIndexToCommandSetIndexDictionary;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Return index of subterm in mySubtermConstants for commandToken that represents 
        /// subterm constant command
        /// </summary>
        /// <param name="commandToken">
        /// Subterm command
        /// </param>
        /// <returns>
        /// Index of subterm in mySubtermConstants for commandToken
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if invalid commandToken is passed
        /// </exception>
        private Int32 GetIndexOfSubterm(Int32 commandToken)
        {
            if (this.IsFixedSubtermConstant(commandToken))
            {
                #region Fixed subterm constant

                if (!MathCompiler.IsEven(myFixedSubtermConstantStartToken))
                {
                    if (MathCompiler.IsEven(commandToken))
                    {
                        #region Push Command

                        return (commandToken - myFixedSubtermConstantStartToken - 1) / 2;

                        #endregion
                    }
                    else
                    {
                        #region Normal Command

                        return (commandToken - myFixedSubtermConstantStartToken) / 2;

                        #endregion
                    }
                }
                else
                {
                    if (!MathCompiler.IsEven(commandToken))
                    {
                        #region Push Command

                        return (commandToken - myFixedSubtermConstantStartToken - 1) / 2;

                        #endregion
                    }
                    else
                    {
                        #region Normal Command

                        return (commandToken - myFixedSubtermConstantStartToken) / 2;

                        #endregion
                    }
                }

                #endregion
            }
            else if (this.IsVariableSubtermConstant(commandToken))
            {
                #region Variable subterm constant

                return commandToken - this.myMaxCalculatedConstantIndex + myFixedSubtermConstantCount;

                #endregion
            }
            else
            {
                throw new MathCompilerException(
                    "Invalid command token: " + 
                    commandToken.ToString(CultureInfo.InvariantCulture.NumberFormat));
            }
        }
        /// <summary>
        /// Return command token for subterm with specified index in mySubtermConstants
        /// </summary>
        /// <param name="subtermIndex">
        /// Index of subterm
        /// </param>
        /// <returns>
        /// Command token for subterm with specified index in mySubtermConstants
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if invalid subtermIndex is passed
        /// </exception>
        private Int32 GetSubtermCommandToken(Int32 subtermIndex)
        {
            #region Checks

            if (subtermIndex < 0 || subtermIndex >= this.mySubtermConstants.Length)
            {
                throw new MathCompilerException(
                    "Invalid subterm index: " +
                    subtermIndex.ToString(CultureInfo.InvariantCulture.NumberFormat));
            }

            #endregion

            if (subtermIndex < myFixedSubtermConstantCount)
            {
                #region Fixed subterm constant

                // Faktor 2 because of command is no push command
                return myFixedSubtermConstantStartToken + subtermIndex * 2;

                #endregion
            }
            else
            {
                #region Variable subterm constant

                return this.myMaxCalculatedConstantIndex + subtermIndex - myFixedSubtermConstantCount;
                // Note: myMaxSubtermConstantIndex, myMaxSubtermConstantSetIndex, myMaxVectorSubtermIndex is NOT changed!

                #endregion
            }
        }

        #endregion

        #region Optimize Stack.Push() commands related method

        /// <summary>
        /// Optimizes stack push commands
        /// </summary>
        private void OptimizeStackPushCommands()
        {
            List<Int32> newCommandList;
            List<Int32> newJumpOffsetList;
            List<Int32> newCorrectIndexCommandList;
            List<Boolean> newCommandsPushList;
            List<String> newCommandRepresentationList;

            Int32[] newCommands;
            Int32[] newJumpOffsets;
            Int32[] newCorrectIndexCommands;
            Boolean[] newCommandsPush;
            String[] newCommandRepresentations;

            // Loop over all command arrays in the set
            for (Int32 k = 0; k < this.myCommandSet.Length; k++)
            {
                newCommandList = new List<Int32>(this.myCommandSet.Length);
                newJumpOffsetList = new List<Int32>(this.myCommandSet.Length);
                newCorrectIndexCommandList = new List<Int32>(this.myCommandSet.Length);
                newCommandsPushList = new List<Boolean>(this.myCommandSet.Length);
                newCommandRepresentationList = new List<String>(this.myCommandSet.Length);

                // Loop over all commands in an array
                for (Int32 i = 0; i < this.myCommandSet[k].Length; i++)
                {
                    if (this.myCommandSet[k][i] == myPush &&
                        !this.IsJumpRelatedCommand(this.myCommandSet[k][i - 1]))
                    {
                        #region Stack.Push() to optimize

                        if (this.IsVariableScalarArgument(this.myCommandSet[k][i - 1]) ||
                            this.IsVariableConstant(this.myCommandSet[k][i - 1]) ||
                            this.IsVariableVectorArgument(this.myCommandSet[k][i - 1]) ||
                            this.IsVariableVectorConstant(this.myCommandSet[k][i - 1]) ||
                            this.IsVariableScalarFunction(this.myCommandSet[k][i - 1]) ||
                            this.IsVariableVectorFunction(this.myCommandSet[k][i - 1]) ||
                            this.IsVariableSubtermConstant(this.myCommandSet[k][i - 1]) ||
                            this.IsVariableCalculatedConstant(this.myCommandSet[k][i - 1]))
                        {
                            newCommandsPushList[newCommandsPushList.Count - 1] = true;
                        }
                        else
                        {
                            // Push command is always above command
                            newCommandList[newCommandList.Count - 1] += 1;
                        }
                        newCommandRepresentationList[newCommandRepresentationList.Count - 1] =
                            newCommandRepresentationList[newCommandRepresentationList.Count - 1] +
                            "   &   Stack.Push(V)";

                        #endregion
                    }
                    else
                    {
                        #region No Stack.Push() to optimize

                        newCommandList.Add(this.myCommandSet[k][i]);
                        newJumpOffsetList.Add(this.myJumpOffsetSet[k][i]);
                        newCorrectIndexCommandList.Add(this.myCorrectIndexCommandSet[k][i]);
                        newCommandsPushList.Add(this.myCommandPushSet[k][i]);
                        newCommandRepresentationList.Add(this.myCommandRepresentationSet[k][i]);

                        #endregion
                    }
                }

                #region Copy new commands and related arrays to sets

                newCommands = new Int32[newCommandList.Count];
                newCommandList.CopyTo(newCommands);
                this.myCommandSet[k] = newCommands;

                newJumpOffsets = new Int32[newJumpOffsetList.Count];
                newJumpOffsetList.CopyTo(newJumpOffsets);
                this.myJumpOffsetSet[k] = newJumpOffsets;

                newCorrectIndexCommands = new Int32[newCorrectIndexCommandList.Count];
                newCorrectIndexCommandList.CopyTo(newCorrectIndexCommands);
                this.myCorrectIndexCommandSet[k] = newCorrectIndexCommands;

                newCommandsPush = new Boolean[newCommandsPushList.Count];
                newCommandsPushList.CopyTo(newCommandsPush);
                this.myCommandPushSet[k] = newCommandsPush;

                newCommandRepresentations = new String[newCommandRepresentationList.Count];
                newCommandRepresentationList.CopyTo(newCommandRepresentations);
                this.myCommandRepresentationSet[k] = newCommandRepresentations;

                #endregion
            }
        }

        #endregion

        #region Identical vector elimination related methods

        /// <summary>
        /// Eliminates identical vectors from final command array(s)
        /// </summary>
        private void EliminateIdenticalVectors()
        {
            List<Int32> finalCommandList;
            List<Int32> finalJumpOffsetList;
            List<Int32> finalCorrectIndexCommandList;
            List<Boolean> finalCommandPushList;
            List<String> finalCommandRepresentationList;

            // myCommands and related arrays will shrink by identical vector elimination
            finalCommandList = new List<Int32>(this.myCommands.Length);
            finalJumpOffsetList = new List<Int32>(this.myCommands.Length);
            finalCorrectIndexCommandList = new List<Int32>(this.myCommands.Length);
            finalCommandPushList = new List<Boolean>(this.myCommands.Length);
            finalCommandRepresentationList = new List<String>(this.myCommands.Length);

            Int32[] vectorEliminationStartIndices = this.GetIdenticalVectorRecognitionStartIndices(0);
            if (vectorEliminationStartIndices == null)
            {
                return;
            }
            Int32 variableVectorSubstitutionCommand = 1;
            while (vectorEliminationStartIndices != null)
            {
                Int32 i = 0;
                Int32 vectorIndex = 0;
                while (i < this.myCommands.Length)
                {
                    #region Initial commands

                    if (vectorIndex == 0 &&
                        i < vectorEliminationStartIndices[0])
                    {
                        finalCommandList.Add(this.myCommands[i]);
                        finalJumpOffsetList.Add(this.myJumpOffsets[i]);
                        finalCorrectIndexCommandList.Add(this.myCorrectIndexCommands[i]);
                        finalCommandPushList.Add(this.myCommandsPush[i]);
                        finalCommandRepresentationList.Add(this.myCommandRepresentations[i]);
                    }

                    #endregion

                    #region First vector with vectorIndex == 0

                    if (vectorIndex == 0 &&
                        i >= vectorEliminationStartIndices[0])
                    {
                        if (this.myCommands[i] != myPushVectorStackSupportListToVectorStack)
                        {
                            finalCommandList.Add(this.myCommands[i]);
                            finalJumpOffsetList.Add(this.myJumpOffsets[i]);
                            finalCorrectIndexCommandList.Add(this.myCorrectIndexCommands[i]);
                            finalCommandPushList.Add(this.myCommandsPush[i]);
                            finalCommandRepresentationList.Add(this.myCommandRepresentations[i]);
                        }
                        else
                        {
                            Int32 currentCommand;
                            Int32 currentCorrectIndexCommand;
                            if (this.myVectorSubterms == null)
                            {
                                #region First vector subterm

                                this.myVectorSubterms = new Double[1][];
                                currentCommand = myFixedVectorSubtermSetStartToken;
                                currentCorrectIndexCommand = -1;

                                #endregion
                            }
                            else
                            {
                                MathCompiler.IncrementJaggedArraySize<Double>(ref this.myVectorSubterms);
                                if (this.myVectorSubterms.Length <= myFixedVectorSubtermSetCount)
                                {
                                    #region Fixed vector subterm

                                    currentCommand = myFixedVectorSubtermSetStartToken + this.myVectorSubterms.Length - 1;
                                    currentCorrectIndexCommand = -1;

                                    #endregion
                                }
                                else
                                {
                                    #region Variable vector subterm

                                    // NOTE: Minus sign (necessary for indication since commands start with 0)
                                    currentCommand = -variableVectorSubstitutionCommand;
                                    currentCorrectIndexCommand = this.myVectorSubterms.Length - 1;

                                    #endregion
                                }
                            }
                            finalCommandList.Add(currentCommand);
                            finalJumpOffsetList.Add(0);
                            finalCorrectIndexCommandList.Add(currentCorrectIndexCommand);
                            finalCommandPushList.Add(false);
                            if (currentCommand == -variableVectorSubstitutionCommand)
                            {
                                finalCommandRepresentationList.Add(
                                    "VectorStack.Push(VectorSubterm[0] = VectorStackSupportList.ToArray)");
                            }
                            else
                            {
                                finalCommandRepresentationList.Add(
                                    "VectorStack.Push(VectorSubterm[0] = VectorStackSupportList.ToArray)" + " (" + currentCommand.ToString() + ")");
                            }
                            vectorIndex = 1;
                            i++;
                        }
                    }

                    #endregion

                    #region Intermediate commands

                    if (vectorIndex > 0 &&
                        vectorIndex < vectorEliminationStartIndices.Length &&
                        i < vectorEliminationStartIndices[vectorIndex])
                    {
                        finalCommandList.Add(this.myCommands[i]);
                        finalJumpOffsetList.Add(this.myJumpOffsets[i]);
                        finalCorrectIndexCommandList.Add(this.myCorrectIndexCommands[i]);
                        finalCommandPushList.Add(this.myCommandsPush[i]);
                        finalCommandRepresentationList.Add(this.myCommandRepresentations[i]);
                    }

                    #endregion

                    #region Other identical vectors

                    if (vectorIndex > 0 &&
                        vectorIndex < vectorEliminationStartIndices.Length &&
                        i >= vectorEliminationStartIndices[vectorIndex] &&
                        this.myCommands[i] == myPushVectorStackSupportListToVectorStack)
                    {
                        Int32 currentCommand;
                        Int32 currentCorrectIndexCommand;
                        if (this.myVectorSubterms.Length <= myFixedVectorSubtermCount)
                        {
                            #region Fixed vector subterm

                            currentCommand = myFixedVectorSubtermStartToken + this.myVectorSubterms.Length - 1;
                            currentCorrectIndexCommand = -1;

                            #endregion
                        }
                        else
                        {
                            #region Variable vector subterm

                            // NOTE: Minus sign (necessary for indication since commands start with 0)
                            currentCommand = -variableVectorSubstitutionCommand;
                            currentCorrectIndexCommand = this.myVectorSubterms.Length - 1;

                            #endregion
                        }
                        Int32 currentVectorSubtermIndex = this.myVectorSubterms.Length - 1;
                        finalCommandList.Add(currentCommand);
                        finalJumpOffsetList.Add(0);
                        finalCorrectIndexCommandList.Add(currentCorrectIndexCommand);
                        finalCommandPushList.Add(false);
                        if (currentCommand == -variableVectorSubstitutionCommand)
                        {
                            finalCommandRepresentationList.Add(
                                "VectorStack.Push(VectorSubterm[" + currentVectorSubtermIndex.ToString() + "])");
                        }
                        else
                        {
                            finalCommandRepresentationList.Add(
                                "VectorStack.Push(VectorSubterm[" + currentVectorSubtermIndex.ToString() + "])" + " (" + currentCommand.ToString() + ")");
                        }
                        vectorIndex++;
                        if (vectorIndex == vectorEliminationStartIndices.Length)
                        {
                            i++;
                        }
                    }

                    #endregion

                    #region Final commands

                    if (vectorIndex == vectorEliminationStartIndices.Length)
                    {
                        finalCommandList.Add(this.myCommands[i]);
                        finalJumpOffsetList.Add(this.myJumpOffsets[i]);
                        finalCorrectIndexCommandList.Add(this.myCorrectIndexCommands[i]);
                        finalCommandPushList.Add(this.myCommandsPush[i]);
                        finalCommandRepresentationList.Add(this.myCommandRepresentations[i]);
                    }

                    #endregion

                    i++;
                }
                this.myCommands = finalCommandList.ToArray();
                this.myJumpOffsets = finalJumpOffsetList.ToArray();
                this.myCorrectIndexCommands = finalCorrectIndexCommandList.ToArray();
                this.myCommandsPush = finalCommandPushList.ToArray();
                this.myCommandRepresentations = finalCommandRepresentationList.ToArray();

                #region Replace variable vector subterm substitution commands

                this.myMaxVectorSubtermIndex = myMaxSubtermConstantSetIndex + this.myVectorSubterms.Length;
                for (Int32 k = 0; k < this.myCommands.Length; k++)
                {
                    if (this.myCommands[k] < 0)
                    {
                        this.myCommands[k] = this.myMaxVectorSubtermIndex - this.myCommands[k] - 1;
                        this.myCommandRepresentations[k] += " (" + this.myCommands[k] + ")";
                    }
                }

                #endregion

                variableVectorSubstitutionCommand++;
                // NOTE: + 2 since vector starts with V = ... BEFORE VectorStackSupportList.Clear: + 2 ensures index after VectorStackSupportList.Clear
                vectorEliminationStartIndices = this.GetIdenticalVectorRecognitionStartIndices(vectorEliminationStartIndices[0] + 2);
            }
        }
        /// <summary>
        /// Returns start indices for identical vector elimination
        /// </summary>
        /// <param name="startIndex">
        /// Start index for detection in myCommands
        /// </param>
        /// <returns>
        /// Array with start indices for identical vector elimination or null if none exist
        /// </returns>
        private Int32[] GetIdenticalVectorRecognitionStartIndices(Int32 startIndex)
        {
            List<Int32> startIndexList = new List<Int32>();
            for (Int32 i = startIndex; i < this.myCommands.Length; i++)
            {
                if (this.myCommands[i] == myClearNewVectorStackSupportList)
                {
                    // NOTE: Vector starts with V = ... BEFORE VectorStackSupportList.Clear
                    startIndexList.Add(i - 1);
                }
            }
            if (startIndexList.Count < 2)
            {
                return null;
            }
            LinkedList<Int32> finalStartIndexList = new LinkedList<Int32>();
            for (Int32 i = 0; i < startIndexList.Count - 1; i++)
            {
                Boolean isCompleteVector = false;
                for (Int32 k = i + 1; k < startIndexList.Count; k++)
                {
                    Int32 offset = 0;
                    while (this.myCommands[startIndexList[i] + offset] == this.myCommands[startIndexList[k] + offset])
                    {
                        if (this.myCommands[startIndexList[i] + offset] == myPushVectorStackSupportListToVectorStack)
                        {
                            isCompleteVector = true;
                            break;
                        }
                        offset++;
                    }
                    if (isCompleteVector)
                    {
                        if (finalStartIndexList.Count == 0)
                        {
                            finalStartIndexList.AddLast(startIndexList[i]);
                        }
                        finalStartIndexList.AddLast(startIndexList[k]);
                    }
                }
                if (isCompleteVector)
                {
                    break;
                }
            }
            if (finalStartIndexList.Count == 0)
            {
                return null;
            }
            else
            {
                Int32[] resultIndexArray = new Int32[finalStartIndexList.Count];
                finalStartIndexList.CopyTo(resultIndexArray, 0);
                return resultIndexArray;
            }
        }

        #endregion

        #region Utility is-methods

        /// <summary>
        /// Checks if token is scalar argument: X0, ...
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is scalar argument, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsScalarArgument(Int32 token)
        {
            return this.IsFixedScalarArgument(token) || this.IsVariableScalarArgument(token);
        }
        /// <summary>
        /// Checks if token is fixed scalar argument
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is fixed scalar argument, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsFixedScalarArgument(Int32 token)
        {
            if (this.myHasScalarArguments &&
                token >= myFixedScalarArgumentStartToken &&
                token <= myFixedScalarArgumentEndToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if token is variable scalar argument
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is variable scalar argument, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVariableScalarArgument(Int32 token)
        {
            if (this.myHasVariableScalarArguments &&
                token >= myVariableScalarArgumentStartToken &&
                token < this.myMaxScalarArgumentIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if token is constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsConstant(Int32 token)
        {
            return this.IsFixedConstant(token) || this.IsVariableConstant(token);
        }
        /// <summary>
        /// Checks if token is fixed constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is fixed constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsFixedConstant(Int32 token)
        {
            if (this.myHasConstants &&
                token >= myFixedConstantStartToken &&
                token <= myFixedConstantEndToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if token is variable constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is variable constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVariableConstant(Int32 token)
        {
            if (this.myHasVariableConstants &&
                token >= this.myMaxScalarArgumentIndex &&
                token < this.myMaxConstantIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if token is vector constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is vector constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVectorConstant(Int32 token)
        {
            return this.IsFixedVectorConstant(token) || this.IsVariableVectorConstant(token);
        }
        /// <summary>
        /// Checks if token is fixed vector constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is fixed vector constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsFixedVectorConstant(Int32 token)
        {
            if (this.myHasVectorConstants &&
                token >= myFixedVectorConstantStartToken &&
                token <= myFixedVectorConstantEndToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if token is variable vector constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is variable vector constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVariableVectorConstant(Int32 token)
        {
            if (this.myHasVariableVectorConstants &&
                token >= this.myMaxConstantIndex &&
                token < this.myMaxVectorConstantIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if token is calculated constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is calculated constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsCalculatedConstant(Int32 token)
        {
            return this.IsFixedCalculatedConstant(token) || this.IsVariableCalculatedConstant(token);
        }
        /// <summary>
        /// Checks if token is fixed calculated constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is fixed calculated constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsFixedCalculatedConstant(Int32 token)
        {
            if (this.myHasConstants &&
                token >= myFixedCalculatedConstantStartToken &&
                token <= myFixedCalculatedConstantEndToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if token is variable calculated constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is variable calculated constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVariableCalculatedConstant(Int32 token)
        {
            if (this.myHasVariableCalculatedConstants &&
                token >= this.myMaxVectorArgumentIndex &&
                token < this.myMaxCalculatedConstantIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if token is scalar function
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is function, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsScalarFunction(Int32 token)
        {
            return this.IsFixedScalarFunction(token) || this.IsVariableScalarFunction(token);
        }
        /// <summary>
        /// Checks if token is fixed scalar function
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is fixed function, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsFixedScalarFunction(Int32 token)
        {
            if (this.myHasScalarFunctions &&
                token >= myFixedFunctionStartToken &&
                token <= myFixedFunctionEndToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if token is variable scalar function
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is variable function, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVariableScalarFunction(Int32 token)
        {
            if (this.myHasVariableScalarFunctions &&
                token >= this.myMaxConstantIndex &&
                token < this.myMaxFunctionIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if token is vector function
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is vector function, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVectorFunction(Int32 token)
        {
            return this.IsFixedVectorFunction(token) || this.IsVariableVectorFunction(token);
        }
        /// <summary>
        /// Checks if token is fixed vector function
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is fixed vector function, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsFixedVectorFunction(Int32 token)
        {
            if (this.myHasVectorFunctions &&
                token >= myFixedVectorFunctionStartToken &&
                token <= myFixedVectorFunctionEndToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if token is variable vector function
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is variable vector function, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVariableVectorFunction(Int32 token)
        {
            if (this.myHasVariableVectorFunctions &&
                token >= this.myMaxFunctionIndex &&
                token < this.myMaxVectorFunctionIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if token is vector argument: X0{}, ...
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is vector argument, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVectorArgument(Int32 token)
        {
            {
                return this.IsFixedVectorArgument(token) || this.IsVariableVectorArgument(token);
            }
        }
        /// <summary>
        /// Checks if token is fixed vector argument
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is fixed vector argument, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsFixedVectorArgument(Int32 token)
        {
            if (this.myHasVectorArguments &&
                token >= myFixedVectorArgumentStartToken &&
                token <= myFixedVectorArgumentEndToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if token is variable vector argument
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is variable vector argument, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVariableVectorArgument(Int32 token)
        {
            if (this.myHasVariableVectorArguments &&
                token >= this.myMaxVectorFunctionIndex &&
                token < this.myMaxVectorArgumentIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if token is subterm constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is subterm constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsSubtermConstant(Int32 token)
        {
            return this.IsFixedSubtermConstant(token) || this.IsVariableSubtermConstant(token);
        }
        /// <summary>
        /// Checks if token is fixed subterm constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is fixed subterm constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsFixedSubtermConstant(Int32 token)
        {
            if (this.mySubtermConstants != null &&
                token >= myFixedSubtermConstantStartToken &&
                token <= myFixedSubtermConstantEndToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if token is variable subterm constant
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is variable subterm constant, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsVariableSubtermConstant(Int32 token)
        {
            if (this.mySubtermConstants != null &&
                token >= this.myMaxCalculatedConstantIndex &&
                token < this.myMaxSubtermConstantIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if token is jump related constant/command
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is jump related constant/command, 
        /// false: Otherwise
        /// </returns>
        private Boolean IsJumpRelatedCommand(Int32 token)
        {
            return token == myFalseJump ||
                token == myFalseJumpEntry ||
                token == myJump ||
                token == myJumpEntry;
        }

        /// <summary>
        /// Checks if token is in FTClass1
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is in FTClass1, 
        /// false: Otherwise
        /// </returns>
        // FTClass1 = (, \,
        private static Boolean IsInFTClass1(Int32 token)
        {
            // FTClass1 = (, {, \,
            switch (token)
            {
                case myBracketOpen:
                    return true;
                case myCurlyBracketOpen:
                    return true;
                case myComma:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Checks if token is in FTClass2
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is in FTClass2, 
        /// false: Otherwise
        /// </returns>
        // FTClass2 = ), scalar argument, constant
        private Boolean IsInFTClass2(Int32 token)
        {
            // FTClass2 = ), scalar argument, constant
            switch (token)
            {
                case myBracketClose:
                    return true;
                default:
                    if (this.IsScalarArgument(token) || this.IsConstant(token))
                    {
                        return true;
                    }
                    return false;
            }
        }
        /// <summary>
        /// Checks if token is in FTClass3
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is in FTClass3, 
        /// false: Otherwise
        /// </returns>
        // FTClass3 = <, <=, =, <>, >=, >, AND, OR, NOT, +, -, *, /, ^
        private static Boolean IsInFTClass3(Int32 token)
        {
            // FTClass3 = <, <=, =, <>, >=, >, AND, OR, NOT, +, -, *, /, ^
            switch (token)
            {
                case myLess:
                    return true;
                case myLessEqual:
                    return true;
                case myEqual:
                    return true;
                case myUnequal:
                    return true;
                case myGreater:
                    return true;
                case myGreaterEqual:
                    return true;
                case myAnd:
                    return true;
                case myOr:
                    return true;
                case myNot:
                    return true;
                case myAdd:
                    return true;
                case mySubtract:
                    return true;
                case myMultiply:
                    return true;
                case myDivide:
                    return true;
                case myPower:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Checks if token is in STClass1
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is in STClass1, 
        /// false: Otherwise
        /// </returns>
        // STClass1 = (, NOT, scalar argument, constant, function, vector function, IF
        private Boolean IsInSTClass1(Int32 token)
        {
            // STClass1 = (, NOT, scalar argument, constant, function, vector function, IF
            switch (token)
            {
                case myBracketOpen:
                    return true;
                case myNot:
                    return true;
                case myIf:
                    return true;
                default:
                    if (
                        this.IsScalarArgument(token) ||
                        this.IsConstant(token) ||
                        this.IsScalarFunction(token) ||
                        this.IsVectorFunction(token))
                    {
                        return true;
                    }
                    return false;
            }
        }
        /// <summary>
        /// Checks if token is in STClass2
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// True: Token is in STClass2, 
        /// false: Otherwise
        /// </returns>
        // STClass2 = ), <, <=, =, <>, >=, >, AND, OR, +, -, *, /, ^, \,
        private static Boolean IsInSTClass2(Int32 token)
        {
            // STClass2 = ), <, <=, =, <>, >=, >, AND, OR, +, -, *, /, ^, \,
            switch (token)
            {
                case myBracketClose:
                    return true;
                case myLess:
                    return true;
                case myLessEqual:
                    return true;
                case myEqual:
                    return true;
                case myUnequal:
                    return true;
                case myGreater:
                    return true;
                case myGreaterEqual:
                    return true;
                case myAnd:
                    return true;
                case myOr:
                    return true;
                case myAdd:
                    return true;
                case mySubtract:
                    return true;
                case myMultiply:
                    return true;
                case myDivide:
                    return true;
                case myPower:
                    return true;
                case myComma:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Corrects command token for ambiguity between command and push-command
        /// </summary>
        /// <param name="commandToken">
        /// Command token
        /// </param>
        /// <param name="startToken">
        /// Start token
        /// </param>
        /// <returns>
        /// Corrected index
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if invalid commandToken is passed
        /// </exception>
        private Int32 CorrectIndex(Int32 commandToken, Int32 startToken)
        {
            if (this.IsFixedScalarArgument(commandToken) ||
                this.IsFixedConstant(commandToken) ||
                this.IsFixedScalarFunction(commandToken) ||
                this.IsFixedSubtermConstant(commandToken) ||
                this.IsFixedVectorFunction(commandToken) ||
                this.IsFixedCalculatedConstant(commandToken))
            {
                #region Normal and push commands

                if (!MathCompiler.IsEven(startToken))
                {
                    if (MathCompiler.IsEven(commandToken))
                    {
                        #region Push Command

                        return (commandToken - startToken - 1) / 2;

                        #endregion
                    }
                    else
                    {
                        #region Normal Command

                        return (commandToken - startToken) / 2;

                        #endregion
                    }
                }
                else
                {
                    if (!MathCompiler.IsEven(commandToken))
                    {
                        #region Push Command

                        return (commandToken - startToken - 1) / 2;

                        #endregion
                    }
                    else
                    {
                        #region Normal Command

                        return (commandToken - startToken) / 2;

                        #endregion
                    }
                }

                #endregion
            }
            else if (this.IsFixedVectorArgument(commandToken) ||
                this.IsFixedVectorConstant(commandToken))
            {
                #region Push commands only

                return commandToken - startToken;

                #endregion
            }
            else
            {
                throw new MathCompilerException("Invalid command token: " + commandToken.ToString(CultureInfo.InvariantCulture.NumberFormat));
            }
        }
        /// <summary>
        /// Converts double representation to double value.
        /// </summary>
        /// <remarks>
        /// Decimal point is a priori converted to NumberDecimalSeparator of current culture.
        /// </remarks>
        /// <param name="doubleRepresentation">
        /// Double representation
        /// </param>
        /// <param name="doubleValue">
        /// Out parameter double value
        /// </param>
        /// <returns>
        /// True: Conversion successful, 
        /// false: Conversion failed
        /// </returns>
        private static Boolean ConvertToDouble(String doubleRepresentation, out Double doubleValue)
        {
            // IMPORTANT: Replace decimal point by current culture decimal separator
            return Double.TryParse(doubleRepresentation.Replace(".", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator), out doubleValue);
        }
        /// <summary>
        /// Counts stack push operations
        /// </summary>
        /// <remarks>
        /// This method must not be optimized for speed, so case values in switch-statement must not be in integer sequence
        /// </remarks>
        /// <param name="commands">
        /// Commands
        /// </param>
        /// <param name="commandsPush">
        /// CommandsPush that correspond to commands
        /// </param>
        /// <param name="netStackPushCount">
        /// Net number of stack push operations
        /// </param>
        /// <param name="maximumStackSize">
        /// Maximum stack size
        /// </param>
        /// <param name="netVectorStackPushCount">
        /// Net number of vector stack push operations
        /// </param>
        /// <param name="maximumVectorStackSize">
        /// Maximum vector stack size
        /// </param>
        /// <param name="netVectorStackSupportListArrayPushCount">
        /// Net number of vector stack support list push operations
        /// </param>
        /// <param name="maximumVectorStackSupportListArraySize">
        /// Maximum vector stack support list array size
        /// </param>
        /// <param name="vectorStackSupportArrayInformationForNonNestedVectorFormula">
        /// Vector stack support array information for formulas without nested vector
        /// </param>
        /// <param name="isSuccess">
        /// True: Count operation could be performed with success, 
        /// false: Otherwise
        /// </param>
        private void CountStackPushOperations(
            Int32[] commands,
            Boolean[] commandsPush,
            out Int32 netStackPushCount, 
            out Int32 maximumStackSize, 
            out Int32 netVectorStackPushCount,
            out Int32 maximumVectorStackSize,
            out Int32 netVectorStackSupportListArrayPushCount,
            out Int32 maximumVectorStackSupportListArraySize,
            out Int32[] vectorStackSupportArrayInformationForNonNestedVectorFormula,
            out Boolean isSuccess)
        {

            #region Checks
            if (commands == null || commands.Length == 0 || 
                commandsPush == null || commands.Length != commandsPush.Length)
            {
                netStackPushCount = Int32.MinValue;
                maximumStackSize = Int32.MinValue;
                netVectorStackPushCount = Int32.MinValue;
                maximumVectorStackSize = Int32.MinValue;
                netVectorStackSupportListArrayPushCount = Int32.MinValue;
                maximumVectorStackSupportListArraySize = Int32.MinValue;
                vectorStackSupportArrayInformationForNonNestedVectorFormula = null;
                isSuccess = false;
                return;
            }
            #endregion

            netStackPushCount = 0;
            maximumStackSize = 0;
            netVectorStackPushCount = 0;
            maximumVectorStackSize = 0;
            netVectorStackSupportListArrayPushCount = 0;
            maximumVectorStackSupportListArraySize = 0;
            List<Int32> vectorStackSupportArrayListForNonNestedVectorFormulas = new List<Int32>();
            vectorStackSupportArrayInformationForNonNestedVectorFormula = null;
            for (Int32 i = 0; i < commands.Length; i++)
            {
                switch (commands[i])
                {
                    #region Push only operations (0-32)

                    case myNotPush:
                    case myChangeSignPush:
                    case myPush:
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;

                    #endregion

                    #region Pop only operations (0-32)

                    case myAnd:
                    case myOr:
                    case myEqual:
                    case myUnequal:
                    case myLess:
                    case myLessEqual:
                    case myGreaterEqual:
                    case myGreater:
                    case myAdd:
                    case mySubtract:
                    case myMultiply:
                    case myDivide:
                    case myPower:
                        netStackPushCount--;
                        if (netStackPushCount < 0)
                        {
                            isSuccess = false;
                            return;
                        }
                        break;

                    #endregion

                    #region No Push/Pop operations (0-32)

                    case myAndPush:
                    case myOrPush:
                    case myEqualPush:
                    case myUnequalPush:
                    case myLessPush:
                    case myLessEqualPush:
                    case myGreaterEqualPush:
                    case myGreaterPush:
                    case myAddPush:
                    case mySubtractPush:
                    case myMultiplyPush:
                    case myDividePush:
                    case myPowerPush:
                    case myNot:
                    case myChangeSign:
                    case myFalseJump:
                    case myJump:
                        break;

                    #endregion

                    #region Vector evaluation related commands (33-36)

                    case 33:
                        // Clear List for myVectorStackSupportListArray
                        netVectorStackSupportListArrayPushCount++;
                        if (netVectorStackSupportListArrayPushCount > maximumVectorStackSupportListArraySize)
                        {
                            maximumVectorStackSupportListArraySize = netVectorStackSupportListArrayPushCount;
                        }
                        if (!this.myHasNestedVector)
                        {
                            // Initialize to 0
                            vectorStackSupportArrayListForNonNestedVectorFormulas.Add(0);
                        }
                        break;
                    case 34:
                        // Add scalar component to List of myVectorStackSupportListArray
                        if (!this.myHasNestedVector)
                        {
                            vectorStackSupportArrayListForNonNestedVectorFormulas[vectorStackSupportArrayListForNonNestedVectorFormulas.Count - 1] += 1;
                        }
                        break;
                    case 35:
                        // Add vector stack components to List of myVectorStackSupportListArray
                        netVectorStackPushCount--;
                        if (netVectorStackPushCount < 0)
                        {
                            isSuccess = false;
                            return;
                        }
                        break;
                    case 36:
                        // Push List of myVectorStackSupportListArray to vector stack
                        netVectorStackPushCount++;
                        if (netVectorStackPushCount > maximumVectorStackSize)
                        {
                            maximumVectorStackSize = netVectorStackPushCount;
                        }
                        netVectorStackSupportListArrayPushCount--;
                        if (netVectorStackSupportListArrayPushCount < 0)
                        {
                            isSuccess = false;
                            return;
                        }
                        break;

                    #endregion

                    #region Fixed scalar argument X0-X11 (37-60)

                    case 37:
                    case 39:
                    case 41:
                    case 43:
                    case 45:
                    case 47:
                    case 49:
                    case 51:
                    case 53:
                    case 55:
                    case 57:
                    case 59:
                        break;

                    case 38:
                    case 40:
                    case 42:
                    case 44:
                    case 46:
                    case 48:
                    case 50:
                    case 52:
                    case 54:
                    case 56:
                    case 58:
                    case 60:
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;

                    #endregion

                    #region Fixed constant 0-14 (61-90)

                    case 61:
                    case 63:
                    case 65:
                    case 67:
                    case 69:
                    case 71:
                    case 73:
                    case 75:
                    case 77:
                    case 79:
                    case 81:
                    case 83:
                    case 85:
                    case 87:
                    case 89:
                        break;

                    case 62:
                    case 64:
                    case 66:
                    case 68:
                    case 70:
                    case 72:
                    case 74:
                    case 76:
                    case 78:
                    case 80:
                    case 82:
                    case 84:
                    case 86:
                    case 88:
                    case 90:
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;

                    #endregion

                    #region Fixed function 0-24 (91-140)

                    case 91:
                        // Function
                        for (Int32 k = this.myScalarFunctions[0].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 92:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[0].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 93:
                        // Function
                        for (Int32 k = this.myScalarFunctions[1].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 94:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[1].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 95:
                        // Function
                        for (Int32 k = this.myScalarFunctions[2].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 96:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[2].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 97:
                        // Function
                        for (Int32 k = this.myScalarFunctions[3].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 98:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[3].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 99:
                        // Function
                        for (Int32 k = this.myScalarFunctions[4].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 100:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[4].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 101:
                        // Function
                        for (Int32 k = this.myScalarFunctions[5].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 102:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[5].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 103:
                        // Function
                        for (Int32 k = this.myScalarFunctions[6].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 104:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[6].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 105:
                        // Function
                        for (Int32 k = this.myScalarFunctions[7].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 106:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[7].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 107:
                        // Function
                        for (Int32 k = this.myScalarFunctions[8].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 108:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[8].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 109:
                        // Function
                        for (Int32 k = this.myScalarFunctions[9].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 110:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[9].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 111:
                        // Function
                        for (Int32 k = this.myScalarFunctions[10].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 112:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[10].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 113:
                        // Function
                        for (Int32 k = this.myScalarFunctions[11].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 114:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[11].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 115:
                        // Function
                        for (Int32 k = this.myScalarFunctions[12].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 116:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[12].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 117:
                        // Function
                        for (Int32 k = this.myScalarFunctions[13].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 118:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[13].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 119:
                        // Function
                        for (Int32 k = this.myScalarFunctions[14].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 120:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[14].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 121:
                        // Function
                        for (Int32 k = this.myScalarFunctions[15].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 122:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[15].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 123:
                        // Function
                        for (Int32 k = this.myScalarFunctions[16].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 124:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[16].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 125:
                        // Function
                        for (Int32 k = this.myScalarFunctions[17].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 126:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[17].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 127:
                        // Function
                        for (Int32 k = this.myScalarFunctions[18].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 128:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[18].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 129:
                        // Function
                        for (Int32 k = this.myScalarFunctions[19].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 130:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[19].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 131:
                        // Function
                        for (Int32 k = this.myScalarFunctions[20].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 132:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[20].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 133:
                        // Function
                        for (Int32 k = this.myScalarFunctions[21].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 134:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[21].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 135:
                        // Function
                        for (Int32 k = this.myScalarFunctions[22].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 136:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[22].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 137:
                        // Function
                        for (Int32 k = this.myScalarFunctions[23].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 138:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[23].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;
                    case 139:
                        // Function
                        for (Int32 k = this.myScalarFunctions[24].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        break;
                    case 140:
                        // Function & Stack.Push
                        for (Int32 k = this.myScalarFunctions[24].NumberOfArguments - 1; k >= 0; k--)
                        {
                            netStackPushCount--;
                            if (netStackPushCount < 0)
                            {
                                isSuccess = false;
                                return;
                            }
                        }
                        netStackPushCount++;
                        break;

                    #endregion

                    #region Fixed subterm constant 0-4 (141-150)

                    case 141:
                    case 143:
                    case 145:
                    case 147:
                    case 149:
                        break;

                    case 142:
                    case 144:
                    case 146:
                    case 148:
                    case 150:
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;

                    #endregion

                    #region Fixed subterm constant to be set 0-4 (151-155)

                    case 151:
                    case 152:
                    case 153:
                    case 154:
                    case 155:
                        break;

                    #endregion

                    #region Fixed calculated constant 0-4 (156-165)

                    case 156:
                    case 158:
                    case 160:
                    case 162:
                    case 164:
                        break;

                    case 157:
                    case 159:
                    case 161:
                    case 163:
                    case 165:
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;

                    #endregion

                    #region Fixed vector function 0-9 (166-185)

                    case 166:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[0].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[0].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 167:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[0].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[0].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 168:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[1].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[1].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 169:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[1].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[1].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 170:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[2].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[2].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 171:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[2].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[2].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 172:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[3].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[3].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 173:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[3].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[3].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 174:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[4].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[4].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 175:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[4].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[4].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 176:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[5].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[5].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 177:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[5].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[5].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 178:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[6].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[6].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 179:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[6].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[6].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 180:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[7].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[7].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 181:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[7].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[7].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 182:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[8].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[8].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 183:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[8].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[8].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;
                    case 184:
                        // Vector function
                        for (Int32 k = this.myVectorFunctions[9].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[9].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        break;
                    case 185:
                        // Vector function & Stack.Push
                        for (Int32 k = this.myVectorFunctions[9].NumberOfArguments - 1; k >= 0; k--)
                        {
                            if (this.myVectorFunctions[9].IsVectorArgument(k))
                            {
                                netVectorStackPushCount--;
                                if (netVectorStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            else
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                        }
                        netStackPushCount++;
                        if (netStackPushCount > maximumStackSize)
                        {
                            maximumStackSize = netStackPushCount;
                        }
                        break;

                    #endregion

                    #region Fixed vector argument 0-4 (186-190)

                    // Vector argument is always pushed on vector stack
                    case 186:
                    case 187:
                    case 188:
                    case 189:
                    case 190:
                        netVectorStackPushCount++;
                        if (netVectorStackPushCount > maximumVectorStackSize)
                        {
                            maximumVectorStackSize = netVectorStackPushCount;
                        }
                        break;

                    #endregion

                    #region Fixed vector constant 0-4 (191-195)

                    // Vector constant is always pushed on vector stack
                    case 191:
                    case 192:
                    case 193:
                    case 194:
                    case 195:
                        netVectorStackPushCount++;
                        if (netVectorStackPushCount > maximumVectorStackSize)
                        {
                            maximumVectorStackSize = netVectorStackPushCount;
                        }
                        break;

                    #endregion

                    #region Fixed vector subterm 0-4 (196-200)

                    case 196:
                    case 197:
                    case 198:
                    case 199:
                    case 200:
                        netVectorStackPushCount++;
                        if (netVectorStackPushCount > maximumVectorStackSize)
                        {
                            maximumVectorStackSize = netVectorStackPushCount;
                        }
                        break;

                    #endregion

                    #region Fixed vector subterm to be set 0-4 (201-205)

                    case 201:
                    case 202:
                    case 203:
                    case 204:
                    case 205:
                        netVectorStackPushCount++;
                        if (netVectorStackPushCount > maximumVectorStackSize)
                        {
                            maximumVectorStackSize = netVectorStackPushCount;
                        }
                        break;

                    #endregion

                    default:

                        if (commands[i] < this.myMaxScalarArgumentIndex)
                        {
                            #region Variable scalar argument

                            if (commandsPush[i])
                            {
                                netStackPushCount++;
                                if (netStackPushCount > maximumStackSize)
                                {
                                    maximumStackSize = netStackPushCount;
                                }
                            }

                            #endregion
                        }
                        else if (commands[i] < this.myMaxConstantIndex)
                        {
                            #region Variable constant

                            if (commandsPush[i])
                            {
                                netStackPushCount++;
                                if (netStackPushCount > maximumStackSize)
                                {
                                    maximumStackSize = netStackPushCount;
                                }
                            }

                            #endregion
                        }
                        else if (commands[i] < this.myMaxVectorConstantIndex)
                        {
                            #region Variable vector constant

                            // Vector constant is always pushed on vector stack
                            netVectorStackPushCount++;
                            if (netVectorStackPushCount > maximumVectorStackSize)
                            {
                                maximumVectorStackSize = netVectorStackPushCount;
                            }

                            #endregion
                        }
                        else if (commands[i] < this.myMaxFunctionIndex)
                        {
                            #region Variable function

                            this.myScalarFunction = this.myScalarFunctions[this.myCorrectIndexCommands[i]];
                            // Get function arguments from stack ...
                            for (Int32 k = this.myScalarFunction.NumberOfArguments - 1; k >= 0; k--)
                            {
                                netStackPushCount--;
                                if (netStackPushCount < 0)
                                {
                                    isSuccess = false;
                                    return;
                                }
                            }
                            if (commandsPush[i])
                            {
                                netStackPushCount++;
                            }

                            #endregion
                        }
                        else if (commands[i] < this.myMaxVectorFunctionIndex)
                        {
                            #region Variable vector function

                            this.myVectorFunction = this.myVectorFunctions[this.myCorrectIndexCommands[i]];
                            // Get vector function arguments from vector stack ...
                            for (Int32 k = this.myVectorFunction.NumberOfArguments - 1; k >= 0; k--)
                            {
                                if (this.myVectorFunction.IsVectorArgument(k))
                                {
                                    netVectorStackPushCount--;
                                    if (netVectorStackPushCount < 0)
                                    {
                                        isSuccess = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    netStackPushCount--;
                                    if (netStackPushCount < 0)
                                    {
                                        isSuccess = false;
                                        return;
                                    }
                                }
                            }
                            if (commandsPush[i])
                            {
                                netStackPushCount++;
                                if (netStackPushCount > maximumStackSize)
                                {
                                    maximumStackSize = netStackPushCount;
                                }
                            }

                            #endregion
                        }
                        else if (commands[i] < this.myMaxVectorArgumentIndex)
                        {
                            #region Variable vector argument

                            // Vector argument is always pushed on vector stack
                            netVectorStackPushCount++;
                            if (netVectorStackPushCount > maximumVectorStackSize)
                            {
                                maximumVectorStackSize = netVectorStackPushCount;
                            }

                            #endregion
                        }
                        else if (commands[i] < this.myMaxCalculatedConstantIndex)
                        {
                            #region Variable calculated constant

                            if (commandsPush[i])
                            {
                                netStackPushCount++;
                                if (netStackPushCount > maximumStackSize)
                                {
                                    maximumStackSize = netStackPushCount;
                                }
                            }

                            #endregion
                        }
                        else if (commands[i] < this.myMaxSubtermConstantIndex)
                        {
                            #region Variable subterm constant

                            if (commandsPush[i])
                            {
                                netStackPushCount++;
                                if (netStackPushCount > maximumStackSize)
                                {
                                    maximumStackSize = netStackPushCount;
                                }
                            }

                            #endregion
                        }
                        else if (commands[i] < this.myMaxSubtermConstantSetIndex)
                        {
                            #region Variable subterm constant to be set

                            // No influence on stack

                            #endregion
                        }
                        else if (this.myCommands[i] < this.myMaxVectorSubtermIndex)
                        {
                            #region Variable vector subterm

                            netVectorStackPushCount++;
                            if (netVectorStackPushCount > maximumVectorStackSize)
                            {
                                maximumVectorStackSize = netVectorStackPushCount;
                            }

                            #endregion
                        }
                        else
                        {
                            #region Variable vector subterm to be set

                            netVectorStackPushCount++;
                            if (netVectorStackPushCount > maximumVectorStackSize)
                            {
                                maximumVectorStackSize = netVectorStackPushCount;
                            }

                            #endregion
                        }
                        break;
                }
            }

            #region Vector stack support array treatment for formulas without nested vector

            if (!this.myHasNestedVector)
            {
                if (vectorStackSupportArrayListForNonNestedVectorFormulas.Count > 0)
                {
                    vectorStackSupportArrayInformationForNonNestedVectorFormula = new Int32[vectorStackSupportArrayListForNonNestedVectorFormulas.Count];
                    vectorStackSupportArrayListForNonNestedVectorFormulas.CopyTo(vectorStackSupportArrayInformationForNonNestedVectorFormula, 0);
                }
            }

            #endregion

            isSuccess = true;
        }
        /// <summary>
        /// Doubles the size of command related arrays
        /// </summary>
        private void DoubleSizeCommandRelatedArrays()
        {
            MathCompiler.DoubleArraySize<Int32>(ref this.myCommands);
            MathCompiler.DoubleArraySize<Int32>(ref this.myJumpOffsets);
            MathCompiler.DoubleArraySize<Int32>(ref this.myCorrectIndexCommands);
            MathCompiler.DoubleArraySize<Boolean>(ref this.myCommandsPush);
            MathCompiler.DoubleArraySize<String>(ref this.myCommandRepresentations);
        }
        /// <summary>
        /// Evaluates vector constant representation (e.g. {2.4, 3.1, 5.7}) to double array
        /// </summary>
        /// <param name="vectorConstantRepresentation">
        /// Vector constant representation
        /// </param>
        /// <returns>
        /// Array of double values for vector constant or null if vectorConstantRepresentation could not be evaluated
        /// </returns>
        private Double[] EvaluateVectorConstantRepresentation(String vectorConstantRepresentation)
        {
            #region Checks

            if (String.IsNullOrEmpty(vectorConstantRepresentation))
            {
                return null;
            }

            #endregion

            // Tokenize with trim-Operation
            String[] valueRepresentations = vectorConstantRepresentation.Split(',');
            Double[] values = new Double[valueRepresentations.Length];
            for (Int32 i = 0; i < values.Length; i++)
            {
                if (String.IsNullOrEmpty(valueRepresentations[i].Trim()))
                {
                    return null;
                }
                Double result;
                // IMPORTANT: Replace decimal point by current culture decimal separator
                if (!MathCompiler.ConvertToDouble(valueRepresentations[i].Trim(), out result))
                {
                    return null;
                }
                values[i] = result;
            }
            return values;
        }
        /// <summary>
        /// Returns vector argument count
        /// </summary>
        /// <param name="token">
        /// Token
        /// </param>
        /// <returns>
        /// Returns vector argument count if token is vector function or -1 if token is not
        /// </returns>
        private Int32 GetVectorArgumentCount(Int32 token)
        {
            #region Checks

            if (!this.IsVectorFunction(token))
            {
                return -1;
            }

            #endregion

            if (this.IsFixedVectorFunction(token))
            {
                return this.myVectorFunctions[this.CorrectIndex(token, myFixedVectorFunctionStartToken)].NumberOfVectorArguments;
            }
            else
            {
                return this.myVectorFunctions[this.myCorrectIndex[token]].NumberOfVectorArguments;
            }
        }
        /// <summary>
        /// Returns vector function description
        /// </summary>
        /// <param name="vectorFunction">
        /// Vector function
        /// </param>
        /// <returns>
        /// Vector function description
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if vectorFunction is null
        /// </exception>
        private static String GetVectorFunctionDescription(IVectorFunction vectorFunction)
        {
            StringBuilder buffer;

            #region Checks

            if (vectorFunction == null)
            {
                throw new MathCompilerException("vectorFunction is null.");
            }

            #endregion

            buffer = new StringBuilder();
            buffer.Append(vectorFunction.Name);
            buffer.Append("(");
            for (Int32 i = 0; i < vectorFunction.NumberOfArguments; i++)
            {
                if (i == 0)
                {
                    if (vectorFunction.IsVectorArgument(i))
                    {
                        buffer.Append("arg" + i.ToString(CultureInfo.InvariantCulture.NumberFormat) + "{}");
                    }
                    else
                    {
                        buffer.Append("arg" + i.ToString(CultureInfo.InvariantCulture.NumberFormat));
                    }
                }
                else
                {
                    if (vectorFunction.IsVectorArgument(i))
                    {
                        buffer.Append(", arg" + i.ToString(CultureInfo.InvariantCulture.NumberFormat) + "{}");
                    }
                    else
                    {
                        buffer.Append(", arg" + i.ToString(CultureInfo.InvariantCulture.NumberFormat));
                    }
                }
            }
            buffer.Append(") - ");
            buffer.Append(vectorFunction.Description);
            return buffer.ToString();
        }
        /// <summary>
        /// Returns scalar function description
        /// </summary>
        /// <param name="scalarFunction">
        /// Function
        /// </param>
        /// <returns>
        /// Scalar function description
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if function is null
        /// </exception>
        private static String GetScalarFunctionDescription(IScalarFunction scalarFunction)
        {
            StringBuilder buffer;

            #region Checks

            if (scalarFunction == null)
            {
                throw new MathCompilerException("function is null.");
            }

            #endregion

            buffer = new StringBuilder();
            buffer.Append(scalarFunction.Name);
            buffer.Append("(");
            for (Int32 i = 0; i < scalarFunction.NumberOfArguments; i++)
            {
                if (i == 0)
                {
                    buffer.Append("arg" + i.ToString(CultureInfo.InvariantCulture.NumberFormat));
                }
                else
                {
                    buffer.Append(", arg" + i.ToString(CultureInfo.InvariantCulture.NumberFormat));
                }
            }
            buffer.Append(") - ");
            buffer.Append(scalarFunction.Description);
            return buffer.ToString();
        }
        /// <summary>
        /// Returns predefined constant description
        /// </summary>
        /// <param name="predefinedConstant">
        /// Predefined constant
        /// </param>
        /// <returns>
        /// Predefined constant description of null if predefinedConstant was null
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if predefinedConstant is null
        /// </exception>
        private static String GetPredefinedConstantDescription(IConstant predefinedConstant)
        {
            StringBuilder buffer;

            #region Checks

            if (predefinedConstant == null)
            {
                throw new MathCompilerException("predefinedConstant is null.");
            }

            #endregion

            buffer = new StringBuilder();
            buffer.Append(predefinedConstant.Name);
            buffer.Append(" - ");
            buffer.Append(predefinedConstant.Description);
            return buffer.ToString();
        }
        /// <summary>
        /// Returns whether formula contains scalar arguments or vector arguments: X0, X0{}, ...
        /// </summary>
        /// <param name="formula">
        /// Formula
        /// </param>
        /// <returns>
        /// True: Formula contains scalar arguments or vector arguments, 
        /// false: Otherwise
        /// </returns>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if formula is null/empty
        /// </exception>
        private static Boolean HasScalarOrVectorArguments(String formula)
        {
            #region Checks

            if (String.IsNullOrEmpty(formula))
            {
                throw new MathCompilerException("Parameter formula is null/empty.");
            }

            #endregion

            return MathCompiler.myAdvancedArgumentDetectionRegex.IsMatch(formula);
        }
        /// <summary>
        /// Checks if integer number is even
        /// </summary>
        /// <param name="integerNumber">
        /// Integer number to be checked
        /// </param>
        /// <returns>
        /// True: Integer number is even, 
        /// false: Integer number is odd
        /// </returns>
        private static Boolean IsEven(Int32 integerNumber)
        {
            Int32 remainder;

            Math.DivRem(integerNumber, 2, out remainder);
            return remainder == 0;
        }
        /// <summary>
        /// Tests if integerRepresentation represents a positive integer (without '+' sign)
        /// </summary>
        /// <param name="integerRepresentation">
        /// String to be tested
        /// </param>
        /// <returns>
        /// True: integerRepresentation represents a positive integer number, 
        /// false: integerRepresentation does not represent a positive integer number
        /// </returns>
        private static Boolean IsPositiveInteger(String integerRepresentation)
        {
            Char[] chars;

            #region Checks
            if (String.IsNullOrEmpty(integerRepresentation))
            {
                return false;
            }
            #endregion

            chars = integerRepresentation.ToCharArray();
            for (Int32 i = 0; i < chars.Length; i++)
            {
                if (!Char.IsDigit(chars[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Sets predefined constant related variables
        /// </summary>
        private void SetPredefinedConstantRelatedVariables()
        {
            if (this.myPredefinedConstants != null && this.myPredefinedConstants.Length > 0)
            {
                this.myPredefinedConstantValueDictionary = new Dictionary<string, double>(this.myPredefinedConstants.Length);
                this.myPredefinedConstantNames = new String[this.myPredefinedConstants.Length];
                this.myPredefinedConstantDescriptions = new String[this.myPredefinedConstants.Length];
                this.myPredefinedConstantExtendedDescriptions = new String[this.myPredefinedConstants.Length];
                this.myPredefinedConstantNamesList = new List<String>(this.myPredefinedConstants.Length);
                for (Int32 i = 0; i < this.myPredefinedConstants.Length; i++)
                {
                    this.myPredefinedConstantValueDictionary.Add(this.myPredefinedConstants[i].Name, this.myPredefinedConstants[i].Value);
                    this.myPredefinedConstantNames[i] = this.myPredefinedConstants[i].Name;
                    this.myPredefinedConstantDescriptions[i] = this.myPredefinedConstants[i].Description;
                    this.myPredefinedConstantExtendedDescriptions[i] =
                        MathCompiler.GetPredefinedConstantDescription(this.myPredefinedConstants[i]);
                    this.myPredefinedConstantNamesList.Add(this.myPredefinedConstants[i].Name);
                }
            }
        }
        /// <summary>
        /// Sets function related variables
        /// </summary>
        private void SetFunctionRelatedVariables()
        {
            Int32 maximumNumberOfArguments;
            Int32 maximumNumberOfVectorArguments;
            Int32 maximumNumberOfScalarArguments;

            #region Scalar functions

            // NOTE: There are predefined functions
            this.myScalarFunctionToNumberOfArgumentsDictionary = new Dictionary<string, int>(this.myScalarFunctions.Length);
            this.myScalarFunctionTokenDictionary = new Dictionary<string, int>(this.myScalarFunctions.Length);
            this.myScalarFunctionNames = new String[this.myScalarFunctions.Length];
            this.myScalarFunctionDescriptions = new String[this.myScalarFunctions.Length];
            this.myScalarFunctionExtendedDescriptions = new String[this.myScalarFunctions.Length];
            maximumNumberOfArguments = 0;
            for (Int32 i = 0; i < this.myScalarFunctions.Length; i++)
            {
                this.myScalarFunctionNames[i] = this.myScalarFunctions[i].Name;
                this.myScalarFunctionDescriptions[i] = this.myScalarFunctions[i].Description;
                this.myScalarFunctionExtendedDescriptions[i] = MathCompiler.GetScalarFunctionDescription(this.myScalarFunctions[i]);
                this.myScalarFunctions[i].Token = i;
                this.myScalarFunctionToNumberOfArgumentsDictionary.Add(this.myScalarFunctions[i].Name, this.myScalarFunctions[i].NumberOfArguments);
                this.myScalarFunctionTokenDictionary.Add(this.myScalarFunctions[i].Name, this.myScalarFunctions[i].Token);
                if (this.myScalarFunctions[i].NumberOfArguments > maximumNumberOfArguments)
                {
                    maximumNumberOfArguments = this.myScalarFunctions[i].NumberOfArguments;
                }
            }
            this.myScalarFunctionArguments = new Double[maximumNumberOfArguments];

            #endregion

            #region Vector functions

            // NOTE: There are predefined vector functions
            this.myVectorFunctionToNumberOfArgumentsDictionary = new Dictionary<string, int>(this.myVectorFunctions.Length);
            this.myVectorFunctionTokenDictionary = new Dictionary<string, int>(this.myVectorFunctions.Length);
            this.myVectorFunctionNames = new String[this.myVectorFunctions.Length];
            this.myVectorFunctionDescriptions = new String[this.myVectorFunctions.Length];
            this.myVectorFunctionExtendedDescriptions = new String[this.myVectorFunctions.Length];
            maximumNumberOfVectorArguments = 0;
            maximumNumberOfScalarArguments = 0;
            for (Int32 i = 0; i < this.myVectorFunctions.Length; i++)
            {
                this.myVectorFunctionNames[i] = this.myVectorFunctions[i].Name;
                this.myVectorFunctionDescriptions[i] = this.myVectorFunctions[i].Description;
                this.myVectorFunctionExtendedDescriptions[i] = MathCompiler.GetVectorFunctionDescription(this.myVectorFunctions[i]);
                this.myVectorFunctions[i].Token = i;
                this.myVectorFunctionToNumberOfArgumentsDictionary.Add(this.myVectorFunctions[i].Name, this.myVectorFunctions[i].NumberOfArguments);
                this.myVectorFunctionTokenDictionary.Add(this.myVectorFunctions[i].Name, this.myVectorFunctions[i].Token);
                if (this.myVectorFunctions[i].NumberOfVectorArguments > maximumNumberOfVectorArguments)
                {
                    maximumNumberOfVectorArguments = this.myVectorFunctions[i].NumberOfVectorArguments;
                }
                if (this.myVectorFunctions[i].NumberOfScalarArguments > maximumNumberOfScalarArguments)
                {
                    maximumNumberOfScalarArguments = this.myVectorFunctions[i].NumberOfScalarArguments;
                }
            }
            this.myVectorFunctionArguments = new Double[maximumNumberOfVectorArguments][];
            // Set myFunctionArguments if necessary since vector functions also may need scalar arguments
            if (maximumNumberOfScalarArguments > 0)
            {
                if (this.myScalarFunctionArguments == null || this.myScalarFunctionArguments.Length < maximumNumberOfScalarArguments)
                {
                    this.myScalarFunctionArguments = new Double[maximumNumberOfScalarArguments];
                }
            }

            #endregion
        }
        /// <summary>
        /// Tokenizes string according to separation character (separator) into string tokenList 
        /// </summary>
        /// <param name="stringToBeTokenized">
        /// String to be tokenized
        /// </param>
        /// <param name="separator">
        /// Separation character
        /// </param>
        /// <param name="trimFlag">
        /// True : Tokens are trimmed (spaces at the beginning and the end of the token strings are removed), 
        /// false: Tokens are not trimmed
        /// </param>
        /// <returns>
        /// String array with tokenList or null if stringToBeTokenized was null/empty
        /// </returns>
        private static String[] TokenizeString(String stringToBeTokenized, Char separator, Boolean trimFlag)
        {
            Int32 endPosition;
            String[] tokens;
            Int32 startPosition;
            String subString;
            StringCollection tokenList;

            #region Checks
            if (String.IsNullOrEmpty(stringToBeTokenized))
            {
                return null;
            }
            #endregion

            tokenList = new StringCollection();
            startPosition = 0;
            endPosition = 0;
            do
            {
                endPosition = stringToBeTokenized.IndexOf(separator, startPosition);
                if (endPosition == -1)
                {
                    endPosition = stringToBeTokenized.Length;
                }
                if (endPosition != startPosition)
                {
                    if (trimFlag)
                    {
                        subString = stringToBeTokenized.Substring(startPosition, endPosition - startPosition).Trim();
                        if (subString.Length != 0)
                        {
                            tokenList.Add(subString);
                        }
                    }
                    else
                    {
                        tokenList.Add(stringToBeTokenized.Substring(startPosition, endPosition - startPosition));
                    }
                }
                startPosition = endPosition + 1;
            } while (startPosition < stringToBeTokenized.Length);
            if (tokenList.Count > 0)
            {
                tokens = new String[tokenList.Count];
                tokenList.CopyTo(tokens, 0);
                return tokens;
            }
            else
            {
                return new String[] { stringToBeTokenized };
            }
        }

        #endregion

        #endregion

        #region Private static methods

        /// <summary>
        /// Copies array into a new array with double size
        /// </summary>
        /// <typeparam name="T">
        /// Genery type
        /// </typeparam>
        /// <param name="array">
        /// Array to be copied into new array with double size
        /// </param>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if array is its default 
        /// </exception>
        private static void DoubleArraySize<T>(ref T[] array)
        {
            #region Checks
            if (array == default(T[]))
            {
                throw new MathCompilerException("Array is its default.");
            }
            #endregion

            T[] helperArray = new T[array.Length * 2];
            array.CopyTo(helperArray, 0);
            array = helperArray;
        }
        /// <summary>
        /// Copies array into a new array with incremented size
        /// </summary>
        /// <typeparam name="T">
        /// Genery type
        /// </typeparam>
        /// <param name="array">
        /// Array to be copied into new array with incremented size
        /// </param>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if array is its default 
        /// </exception>
        private static void IncrementArraySize<T>(ref T[] array)
        {
            #region Checks
            if (array == default(T[]))
            {
                throw new MathCompilerException("Array is its default.");
            }
            #endregion

            T[] helperArray = new T[array.Length + 1];
            array.CopyTo(helperArray, 0);
            array = helperArray;
        }
        /// <summary>
        /// Copies jagged array into a new jagged array with incremented size
        /// </summary>
        /// <typeparam name="T">
        /// Genery type
        /// </typeparam>
        /// <param name="array">
        /// Array to be copied into new array with incremented size
        /// </param>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if array is its default 
        /// </exception>
        private static void IncrementJaggedArraySize<T>(ref T[][] array)
        {
            #region Checks

            if (array == default(T[][]))
            {
                throw new MathCompilerException("Array is its default.");
            }

            #endregion

            T[][] helperArray = new T[array.Length + 1][];
            array.CopyTo(helperArray, 0);
            array = helperArray;
        }
        /// <summary>
        /// Copies set into a new set with incremented size
        /// </summary>
        /// <typeparam name="T">
        /// Genery type
        /// </typeparam>
        /// <param name="set">
        /// set to be copied into new set with incremented size
        /// </param>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if set is its default 
        /// </exception>
        private static void IncrementSetSize<T>(ref T[][] set)
        {
            #region Checks
            if (set == default(T[][]))
            {
                throw new MathCompilerException("Set is its default.");
            }
            #endregion

            T[][] helperSet = new T[set.Length + 1][];
            set.CopyTo(helperSet, 0);
            set = helperSet;
        }
        /// <summary>
        /// Copies array into a new array with shrinked size according to new length
        /// </summary>
        /// <typeparam name="T">
        /// Genery type
        /// </typeparam>
        /// <param name="array">
        /// Array to be copied into new array with shrinked size according to new length
        /// </param>
        /// <param name="newLength">
        /// New (shrinked) length of array (>0)
        /// </param>
        /// <exception cref="MathCompiler.MathCompilerException">
        /// Thrown if array is its default 
        /// </exception>
        private static void ShrinkArraySize<T>(ref T[] array, Int32 newLength)
        {
            #region Checks
            if (newLength <= 0)
            {
                throw new MathCompilerException("NewLength is less or equal to 0.");
            }
            if (array == default(T[]))
            {
                throw new MathCompilerException("Array is its default.");
            }
            #endregion

            T[] helperArray = new T[newLength];
            Array.Copy(array, helperArray, newLength);
            array = helperArray;
        }

        #endregion

        #region Private properties (get)

        #region CalculatedConstantCommandsRepresentationSet

        /// <summary>
        /// Contains representation of commands that were the basis for calculated constants: 
        /// myCalculatedConstantCommandsRepresentationSet[i] corresponds to detected calculated constant i. 
        /// NOTE: This dimension may not be equal to the dimension of myCalculatedConstants since 
        /// "different" calculated constants may have the same value!
        /// </summary>
        public String[][] CalculatedConstantCommandsRepresentationSet
        {
            get
            {
                return this.myCalculatedConstantCommandsRepresentationSet;
            }
        }

        #endregion

        #region CommandRepresentations

        /// <summary>
        /// Command representations
        /// </summary>
        public String[] CommandRepresentations
        {
            get
            {
                return this.myCommandRepresentations;
            }
        }

        #endregion

        #region CommandRepresentationSet

        /// <summary>
        /// Command representation set
        /// </summary>
        public String[][] CommandRepresentationSet
        {
            get
            {
                return this.myCommandRepresentationSet;
            }
        }

        #endregion

        #region HasJump

        /// <summary>
        /// True: Formula contains jump due to IF, 
        /// false: Otherwise
        /// </summary>
        public Boolean HasJump
        {
            get
            {
                return this.myHasJump;
            }
        }

        #endregion

        #region HasVector

        /// <summary>
        /// True: Formula contains vector, 
        /// false: Otherwise
        /// </summary>
        public Boolean HasVector
        {
            get
            {
                return this.myHasVector;
            }
        }

        #endregion

        #region HasNestedVector

        /// <summary>
        /// True: Formula contains nested vector, 
        /// false: Otherwise
        /// </summary>
        public Boolean HasNestedVector
        {
            get
            {
                return this.myHasNestedVector;
            }
        }

        #endregion

        #region NumberOfConstants

        /// <summary>
        /// Number of constants
        /// </summary>
        public Int32 NumberOfConstants
        {
            get
            {
                if (this.myConstants == null)
                {
                    return 0;
                }
                else
                {
                    return this.myConstants.Length;
                }
            }
        }

        #endregion

        #region NumberOfVectorConstants

        /// <summary>
        /// Number of vector constants
        /// </summary>
        public Int32 NumberOfVectorConstants
        {
            get
            {
                if (this.myVectorConstants == null)
                {
                    return 0;
                }
                else
                {
                    return this.myVectorConstants.Length;
                }
            }
        }

        #endregion

        #region NumberOfCalculatedConstants

        /// <summary>
        /// Number of calculated constants
        /// </summary>
        public Int32 NumberOfCalculatedConstants
        {
            get
            {
                if (this.myCalculatedConstants == null)
                {
                    return 0;
                }
                else
                {
                    return this.myCalculatedConstants.Length;
                }
            }
        }

        #endregion

        #region NumberOfSubtermConstants

        /// <summary>
        /// Number of subterm constants
        /// </summary>
        public Int32 NumberOfSubtermConstants
        {
            get
            {
                if (this.mySubtermConstants == null)
                {
                    return 0;
                }
                else
                {
                    return this.mySubtermConstants.Length;
                }
            }
        }

        #endregion

        #region NumberOfVectorSubterms

        /// <summary>
        /// Number of vector subterms
        /// </summary>
        private Int32 NumberOfVectorSubterms
        {
            get
            {
                if (this.myVectorSubterms == null)
                {
                    return 0;
                }
                else
                {
                    return this.myVectorSubterms.Length;
                }
            }
        }

        #endregion

        #endregion

        #region Private static properties (get)

        #region AllComments

        /// <summary>
        /// All comments
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        public static String[] AllComments
        {
            get
            {
                return MathCompiler.myComments;
            }
        }

        #endregion

        #region False

        /// <summary>
        /// Double representation of boolean false
        /// </summary>
        /// <value>
        /// 0
        /// </value>
        public static Double False
        {
            get
            {
                return myFalse;
            }
        }

        #endregion

        #region True

        /// <summary>
        /// Double representation of boolean true
        /// </summary>
        /// <value>
        /// 1
        /// </value>
        public static Double True
        {
            get
            {
                return myTrue;
            }
        }

        #endregion

        #region OperatorSymbols

        /// <summary>
        /// Operator representations (in ascending precedence)
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        private static String[] OperatorSymbols
        {
            get
            {
                return MathCompiler.myOperatorSymbols;
            }
        }

        #endregion

        #region ReservedSymbols

        /// <summary>
        /// Reserved characters
        /// </summary>
        /// <value>
        /// Not null
        /// </value>
        private static String[] ReservedSymbols
        {
            get
            {
                return MathCompiler.myReservedSymbols;
            }
        }

        #endregion

        #endregion

    }
}
