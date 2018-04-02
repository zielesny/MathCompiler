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
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using MathCompiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCompiler.Test
{
    /// <summary>
    /// Tests for MathCompiler
    /// </summary>
    [TestClass]
    public class MathCompilerTest
    {

        #region Tests
        /// <summary>
        /// Tests compiler optimizations
        /// </summary>
        [TestMethod]
        public void Test_CompilationOptimization_01()
        {
            String assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            String assemblyDirectoryPath = Path.GetDirectoryName(assemblyFilePath);
            String outputDirectoryPath = Path.GetFullPath(Path.Combine(assemblyDirectoryPath, @"..\..\..\TestOutput"));
            System.IO.Directory.CreateDirectory(outputDirectoryPath);
            String outputFilePathname = Path.Combine(outputDirectoryPath, "Test_CompilationOptimization_01.txt");
            File.Delete(outputFilePathname);

            MathCompiler mathCompiler;
            String formula;
            String[] commandRepresentations;
            Boolean isConstantSubExpressionRecognition;
            Boolean isIdenticalSubtermRecognition;
            Boolean isStackPushOptimization;
            Boolean isIdenticalVectorRecognition;
            LinkedList<String> outputList = new LinkedList<String>();

            formula = "(X0 + X1) * (2.34 - X2) * exp(X1 / (2.34 - X2)) + X3 - ((3 - 1.74) / ln(2)) / exp(X1 / (2.34 - X2)) + (X1 + X0)";

            isConstantSubExpressionRecognition = false;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = false;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentations = mathCompiler.CommandRepresentations;
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("Formula                            = " + formula);
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("isConstantSubExpressionRecognition = " + isConstantSubExpressionRecognition.ToString());
            outputList.AddLast("isIdenticalSubtermRecognition      = " + isIdenticalSubtermRecognition.ToString());
            outputList.AddLast("isStackPushOptimization            = " + isStackPushOptimization.ToString());
            outputList.AddLast("isIdenticalVectorRecognition       = " + isIdenticalVectorRecognition.ToString());
            outputList.AddLast("Number of commands                 = " + commandRepresentations.Length.ToString());
            outputList.AddLast("----------------------------------------------------------------------");
            foreach (String commandRepresentation in commandRepresentations)
            {
                outputList.AddLast(commandRepresentation);
            }
            outputList.AddLast("");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = false;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentations = mathCompiler.CommandRepresentations;
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("Formula                            = " + formula);
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("isConstantSubExpressionRecognition = " + isConstantSubExpressionRecognition.ToString());
            outputList.AddLast("isIdenticalSubtermRecognition      = " + isIdenticalSubtermRecognition.ToString());
            outputList.AddLast("isStackPushOptimization            = " + isStackPushOptimization.ToString());
            outputList.AddLast("isIdenticalVectorRecognition       = " + isIdenticalVectorRecognition.ToString());
            outputList.AddLast("Number of commands                 = " + commandRepresentations.Length.ToString());
            outputList.AddLast("----------------------------------------------------------------------");
            foreach (String commandRepresentation in commandRepresentations)
            {
                outputList.AddLast(commandRepresentation);
            }
            outputList.AddLast("");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = false;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentations = mathCompiler.CommandRepresentations;
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("Formula                            = " + formula);
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("isConstantSubExpressionRecognition = " + isConstantSubExpressionRecognition.ToString());
            outputList.AddLast("isIdenticalSubtermRecognition      = " + isIdenticalSubtermRecognition.ToString());
            outputList.AddLast("isStackPushOptimization            = " + isStackPushOptimization.ToString());
            outputList.AddLast("isIdenticalVectorRecognition       = " + isIdenticalVectorRecognition.ToString());
            outputList.AddLast("Number of commands                 = " + commandRepresentations.Length.ToString());
            outputList.AddLast("----------------------------------------------------------------------");
            foreach (String commandRepresentation in commandRepresentations)
            {
                outputList.AddLast(commandRepresentation);
            }
            outputList.AddLast("");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = false;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentations = mathCompiler.CommandRepresentations;
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("Formula                            = " + formula);
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("isConstantSubExpressionRecognition = " + isConstantSubExpressionRecognition.ToString());
            outputList.AddLast("isIdenticalSubtermRecognition      = " + isIdenticalSubtermRecognition.ToString());
            outputList.AddLast("isStackPushOptimization            = " + isStackPushOptimization.ToString());
            outputList.AddLast("isIdenticalVectorRecognition       = " + isIdenticalVectorRecognition.ToString());
            outputList.AddLast("Number of commands                 = " + commandRepresentations.Length.ToString());
            outputList.AddLast("----------------------------------------------------------------------");
            foreach (String commandRepresentation in commandRepresentations)
            {
                outputList.AddLast(commandRepresentation);
            }
            outputList.AddLast("");

            System.IO.File.WriteAllLines(outputFilePathname, outputList, Encoding.UTF8);
        }
        /// <summary>
        /// Tests compiler optimizations
        /// </summary>
        [TestMethod]
        public void Test_CompilationOptimization_02()
        {
            String assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            String assemblyDirectoryPath = Path.GetDirectoryName(assemblyFilePath);
            String outputDirectoryPath = Path.GetFullPath(Path.Combine(assemblyDirectoryPath, @"..\..\..\TestOutput"));
            System.IO.Directory.CreateDirectory(outputDirectoryPath);
            String outputFilePathname = Path.Combine(outputDirectoryPath, "Test_CompilationOptimization_02.txt");
            File.Delete(outputFilePathname);

            MathCompiler mathCompiler;
            String formula;
            String[] commandRepresentations;
            Boolean isConstantSubExpressionRecognition;
            Boolean isIdenticalSubtermRecognition;
            Boolean isStackPushOptimization;
            Boolean isIdenticalVectorRecognition;
            LinkedList<String> outputList = new LinkedList<String>();

            formula = "X0 + exp(X1/(X0 + X1))*exp(X1/(X2 + X0)) - 2.74/(exp(X1/(X0 + X2))*exp(X1/(X1 + X0))) + 1/(X0 + exp(X1/(X0 + X1))*exp(X1/(X2 + X0)) - 2.74/(exp(X1/(X0 + X2))*exp(X1/(X1 + X0)))) + 1/(X0 + exp(X1/(X0 + X1))*exp(X1/(X2 + X0)) - 2.74/(exp(X1/(X0 + X2))*exp(X1/(X1 + X0))) + 1/(X0 + exp(X1/(X0 + X1))*exp(X1/(X2 + X0)) - 2.74/(exp(X1/(X0 + X2))*exp(X1/(X1 + X0))))) - exp(X0 + exp(X1/(X0 + X1))*exp(X1/(X2 + X0)) - 2.74/(exp(X1/(X0 + X2))*exp(X1/(X1 + X0))) + 1/(X0 + exp(X1/(X0 + X1))*exp(X1/(X2 + X0)) - 2.74/(exp(X1/(X0 + X2))*exp(X1/(X1 + X0)))))";

            isConstantSubExpressionRecognition = false;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = false;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentations = mathCompiler.CommandRepresentations;
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("Formula                            = " + formula);
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("isConstantSubExpressionRecognition = " + isConstantSubExpressionRecognition.ToString());
            outputList.AddLast("isIdenticalSubtermRecognition      = " + isIdenticalSubtermRecognition.ToString());
            outputList.AddLast("isStackPushOptimization            = " + isStackPushOptimization.ToString());
            outputList.AddLast("isIdenticalVectorRecognition       = " + isIdenticalVectorRecognition.ToString());
            outputList.AddLast("Number of commands                 = " + commandRepresentations.Length.ToString());
            outputList.AddLast("----------------------------------------------------------------------");
            foreach (String commandRepresentation in commandRepresentations)
            {
                outputList.AddLast(commandRepresentation);
            }
            outputList.AddLast("");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = false;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentations = mathCompiler.CommandRepresentations;
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("Formula                            = " + formula);
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("isConstantSubExpressionRecognition = " + isConstantSubExpressionRecognition.ToString());
            outputList.AddLast("isIdenticalSubtermRecognition      = " + isIdenticalSubtermRecognition.ToString());
            outputList.AddLast("isStackPushOptimization            = " + isStackPushOptimization.ToString());
            outputList.AddLast("isIdenticalVectorRecognition       = " + isIdenticalVectorRecognition.ToString());
            outputList.AddLast("Number of commands                 = " + commandRepresentations.Length.ToString());
            outputList.AddLast("----------------------------------------------------------------------");
            foreach (String commandRepresentation in commandRepresentations)
            {
                outputList.AddLast(commandRepresentation);
            }
            outputList.AddLast("");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = false;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentations = mathCompiler.CommandRepresentations;
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("Formula                            = " + formula);
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("isConstantSubExpressionRecognition = " + isConstantSubExpressionRecognition.ToString());
            outputList.AddLast("isIdenticalSubtermRecognition      = " + isIdenticalSubtermRecognition.ToString());
            outputList.AddLast("isStackPushOptimization            = " + isStackPushOptimization.ToString());
            outputList.AddLast("isIdenticalVectorRecognition       = " + isIdenticalVectorRecognition.ToString());
            outputList.AddLast("Number of commands                 = " + commandRepresentations.Length.ToString());
            outputList.AddLast("----------------------------------------------------------------------");
            foreach (String commandRepresentation in commandRepresentations)
            {
                outputList.AddLast(commandRepresentation);
            }
            outputList.AddLast("");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = false;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentations = mathCompiler.CommandRepresentations;
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("Formula                            = " + formula);
            outputList.AddLast("----------------------------------------------------------------------");
            outputList.AddLast("isConstantSubExpressionRecognition = " + isConstantSubExpressionRecognition.ToString());
            outputList.AddLast("isIdenticalSubtermRecognition      = " + isIdenticalSubtermRecognition.ToString());
            outputList.AddLast("isStackPushOptimization            = " + isStackPushOptimization.ToString());
            outputList.AddLast("isIdenticalVectorRecognition       = " + isIdenticalVectorRecognition.ToString());
            outputList.AddLast("Number of commands                 = " + commandRepresentations.Length.ToString());
            outputList.AddLast("----------------------------------------------------------------------");
            foreach (String commandRepresentation in commandRepresentations)
            {
                outputList.AddLast(commandRepresentation);
            }
            outputList.AddLast("");

            System.IO.File.WriteAllLines(outputFilePathname, outputList, Encoding.UTF8);
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Systematic tests

        /// <summary>
        /// Test in systematic manner
        /// </summary>
        [TestMethod]
        public void Test_Systematic()
        {
            #region 0001 Formula "1 + 1"

            this.TestValidFormula(
                "1 + 1", // formula
                null, // scalarArguments
                null, // vectorArguments,
                2, // correctFunctionValue,
                false, // isOptimizationOption1,
                false, // isOptimizationOption2,
                false, // isOptimizationOption3,
                false, // isOptimizationOption4,
                0, // scalarArgumentCount,
                0, // vectorArgumentCount,
                false, // hasJump,
                false, // hasVector,
                false, // hasNestedVector,
                1, // numberOfConstants,
                0, // numberOfVectorConstants,
                0, // numberOfCalculatedConstants,
                0, // numberOfSubtermConstants,
                0, // numberOfVectorSubterms,
                "0001 Formula '1 + 1' : Test 1-"
            ); // testInfo
            this.TestValidFormula(
                "1 + 1", // formula
                null, // scalarArguments
                null, // vectorArguments,
                2, // correctFunctionValue,
                true, // isOptimizationOption1,
                true, // isOptimizationOption2,
                true, // isOptimizationOption3,
                true, // isOptimizationOption4,
                0, // scalarArgumentCount,
                0, // vectorArgumentCount,
                false, // hasJump,
                false, // hasVector,
                false, // hasNestedVector,
                1, // numberOfConstants,
                0, // numberOfVectorConstants,
                1, // numberOfCalculatedConstants,
                0, // numberOfSubtermConstants,
                0, // numberOfVectorSubterms,
                "0001 Formula '1 + 1' : Test 2-"
            ); // testInfo

            #endregion

            #region 0002 Formula "mean({X0,X1,X2}) - sum({X0,X1,X2})/count({X0,X1,X2})"

            this.TestValidFormula(
                "mean({X0,X1,X2}) - sum({X0,X1,X2})/count({X0,X1,X2})", // formula
                new Double[] { 1, 2, 3 }, // scalarArguments
                null, // vectorArguments,
                0, // correctFunctionValue,
                false, // isOptimizationOption1,
                false, // isOptimizationOption2,
                false, // isOptimizationOption3,
                false, // isOptimizationOption4,
                3, // scalarArgumentCount,
                0, // vectorArgumentCount,
                false, // hasJump,
                true, // hasVector,
                false, // hasNestedVector,
                0, // numberOfConstants,
                0, // numberOfVectorConstants,
                0, // numberOfCalculatedConstants,
                0, // numberOfSubtermConstants,
                0, // numberOfVectorSubterms,
                "0002 Formula 'mean({X0,X1,X2}) - sum({X0,X1,X2})/count({X0,X1,X2})' : Test 1-"
            ); // testInfo
            this.TestValidFormula(
                "mean({X0,X1,X2}) - sum({X0,X1,X2})/count({X0,X1,X2})", // formula
                new Double[] { 1, 2, 3 }, // scalarArguments
                null, // vectorArguments,
                0, // correctFunctionValue,
                true, // isOptimizationOption1,
                true, // isOptimizationOption2,
                true, // isOptimizationOption3,
                true, // isOptimizationOption4,
                3, // scalarArgumentCount,
                0, // vectorArgumentCount,
                false, // hasJump,
                true, // hasVector,
                false, // hasNestedVector,
                0, // numberOfConstants,
                0, // numberOfVectorConstants,
                0, // numberOfCalculatedConstants,
                0, // numberOfSubtermConstants,
                1, // numberOfVectorSubterms,
                "0002 Formula 'mean({X0,X1,X2}) - sum({X0,X1,X2})/count({X0,X1,X2})' : Test 2-"
            ); // testInfo

            #endregion
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Non-systematic tests

        #region Test ConvertToIntegerTokens()

        /// <summary>
        /// Test ConvertToIntegerTokens()
        /// </summary>
        [TestMethod]
        public void Test_ConvertToTokens()
        {
            MathCompiler mathCompiler;
            String formula;
            String[] tokenRepresentations;
            Int32[] tokens;
            String formulaComment;

            mathCompiler = new MathCompiler();

            #region Test1 : "X1*X2 + 1.54"

            formula = "X1*X2 + 1.54";
            tokenRepresentations = mathCompiler.SplitToTokenRepresentations(formula);
            Assert.IsTrue(tokenRepresentations[0].Trim() == "X1", "Test1");
            Assert.IsTrue(tokenRepresentations[1].Trim() == "*", "Test1");
            Assert.IsTrue(tokenRepresentations[2].Trim() == "X2", "Test1");
            Assert.IsTrue(tokenRepresentations[3].Trim() == "+", "Test1");
            Assert.IsTrue(tokenRepresentations[4].Trim() == "1.54", "Test1");
            tokens = mathCompiler.ConvertToIntegerTokens(tokenRepresentations);
            Assert.IsTrue(mathCompiler.CheckTokens(tokens), "Test1");

            #endregion

            #region Test2 : "(X1 + X2) <= (2*3/(X3 - X4)) + \n exp(x5/(x6 - 3.79))"

            formula = "(X1 + X2) <= (2*3/(X3 - X4)) + \n exp(x5/(x6 - 3.79))";
            tokenRepresentations = mathCompiler.SplitToTokenRepresentations(formula);
            Assert.IsTrue(tokenRepresentations[0].Trim() == "(", "Test2");
            Assert.IsTrue(tokenRepresentations[1].Trim() == "X1", "Test2");
            Assert.IsTrue(tokenRepresentations[2].Trim() == "+", "Test2");
            Assert.IsTrue(tokenRepresentations[3].Trim() == "X2", "Test2");
            Assert.IsTrue(tokenRepresentations[4].Trim() == ")", "Test2");
            Assert.IsTrue(tokenRepresentations[5].Trim() == "<=", "Test2");
            Assert.IsTrue(tokenRepresentations[6].Trim() == "(", "Test2");
            Assert.IsTrue(tokenRepresentations[7].Trim() == "2", "Test2");
            Assert.IsTrue(tokenRepresentations[8].Trim() == "*", "Test2");
            Assert.IsTrue(tokenRepresentations[9].Trim() == "3", "Test2");
            Assert.IsTrue(tokenRepresentations[10].Trim() == "/", "Test2");
            Assert.IsTrue(tokenRepresentations[11].Trim() == "(", "Test2");
            Assert.IsTrue(tokenRepresentations[12].Trim() == "X3", "Test2");
            Assert.IsTrue(tokenRepresentations[13].Trim() == "-", "Test2");
            Assert.IsTrue(tokenRepresentations[14].Trim() == "X4", "Test2");
            Assert.IsTrue(tokenRepresentations[15].Trim() == ")", "Test2");
            Assert.IsTrue(tokenRepresentations[16].Trim() == ")", "Test2");
            Assert.IsTrue(tokenRepresentations[17].Trim() == "+", "Test2");
            Assert.IsTrue(tokenRepresentations[18].Trim() == "exp", "Test2");
            Assert.IsTrue(tokenRepresentations[19].Trim() == "(", "Test2");
            Assert.IsTrue(tokenRepresentations[20].Trim() == "x5", "Test2");
            Assert.IsTrue(tokenRepresentations[21].Trim() == "/", "Test2");
            Assert.IsTrue(tokenRepresentations[22].Trim() == "(", "Test2");
            Assert.IsTrue(tokenRepresentations[23].Trim() == "x6", "Test2");
            Assert.IsTrue(tokenRepresentations[24].Trim() == "-", "Test2");
            Assert.IsTrue(tokenRepresentations[25].Trim() == "3.79", "Test2");
            Assert.IsTrue(tokenRepresentations[26].Trim() == ")", "Test2");
            Assert.IsTrue(tokenRepresentations[27].Trim() == ")", "Test2");
            tokens = mathCompiler.ConvertToIntegerTokens(tokenRepresentations);
            Assert.IsTrue(mathCompiler.CheckTokens(tokens), "Test2");

            #endregion

            #region Test4 : "(X1 + X2) AND NOT(X3 - X4) OR (x6 - 1)"

            formula = "(X1 + X2) AND NOT(X3 - X4) OR (x6 - 1)";
            tokenRepresentations = mathCompiler.SplitToTokenRepresentations(formula);
            Assert.IsTrue(tokenRepresentations[0].Trim() == "(", "Test4");
            Assert.IsTrue(tokenRepresentations[1].Trim() == "X1", "Test4");
            Assert.IsTrue(tokenRepresentations[2].Trim() == "+", "Test4");
            Assert.IsTrue(tokenRepresentations[3].Trim() == "X2", "Test4");
            Assert.IsTrue(tokenRepresentations[4].Trim() == ")", "Test4");
            Assert.IsTrue(tokenRepresentations[5].Trim() == "AND", "Test4");
            Assert.IsTrue(tokenRepresentations[6].Trim() == "NOT", "Test4");
            Assert.IsTrue(tokenRepresentations[7].Trim() == "(", "Test4");
            Assert.IsTrue(tokenRepresentations[8].Trim() == "X3", "Test4");
            Assert.IsTrue(tokenRepresentations[9].Trim() == "-", "Test4");
            Assert.IsTrue(tokenRepresentations[10].Trim() == "X4", "Test4");
            Assert.IsTrue(tokenRepresentations[11].Trim() == ")", "Test4");
            Assert.IsTrue(tokenRepresentations[12].Trim() == "OR", "Test4");
            Assert.IsTrue(tokenRepresentations[13].Trim() == "(", "Test4");
            Assert.IsTrue(tokenRepresentations[14].Trim() == "x6", "Test4");
            Assert.IsTrue(tokenRepresentations[15].Trim() == "-", "Test4");
            Assert.IsTrue(tokenRepresentations[16].Trim() == "1", "Test4");
            Assert.IsTrue(tokenRepresentations[17].Trim() == ")", "Test4");
            tokens = mathCompiler.ConvertToIntegerTokens(tokenRepresentations);
            Assert.IsTrue(mathCompiler.CheckTokens(tokens), "Test4");

            #endregion

            #region Test5 : "(X1 + X2) * unknownFunction(X3)"

            formula = "(X1 + X2) * unknownFunction(X3)";
            tokenRepresentations = mathCompiler.SplitToTokenRepresentations(formula);
            Assert.IsTrue(tokenRepresentations != null, "Test5");
            tokens = mathCompiler.ConvertToIntegerTokens(tokenRepresentations);
            Assert.IsTrue(tokens == null, "Test5");
            formulaComment = mathCompiler.Comment;

            #endregion

            #region Test6 : "(X + X2)"

            formula = "(X + X2)";
            tokenRepresentations = mathCompiler.SplitToTokenRepresentations(formula);
            Assert.IsTrue(tokenRepresentations != null, "Test6");
            tokens = mathCompiler.ConvertToIntegerTokens(tokenRepresentations);
            Assert.IsTrue(tokens == null, "Test6");
            formulaComment = mathCompiler.Comment;

            #endregion

            #region Test7 : "(X1 + X2) AND OR (x6 - 1)"

            formula = "(X1 + X2) AND OR (x6 - 1)";
            tokenRepresentations = mathCompiler.SplitToTokenRepresentations(formula);
            tokens = mathCompiler.ConvertToIntegerTokens(tokenRepresentations);
            Assert.IsTrue(!mathCompiler.CheckTokens(tokens), "Test7");
            formulaComment = mathCompiler.Comment;

            #endregion

            #region Test8 : "(X1 + X2)*sin+cos"

            formula = "(X1 + X2)*sin+cos";
            tokenRepresentations = mathCompiler.SplitToTokenRepresentations(formula);
            tokens = mathCompiler.ConvertToIntegerTokens(tokenRepresentations);
            Assert.IsFalse(mathCompiler.CheckTokens(tokens), "Test8");
            formulaComment = mathCompiler.Comment;

            #endregion
        }

        #endregion

        #region Test SetFormula()

        /// <summary>
        /// Test SetFormula()
        /// </summary>
        [TestMethod]
        public void Test_SetFormula()
        {
            MathCompiler mathCompiler;
            String formula;
            String[][] commandRepresentationSet1;
            String[][] commandRepresentationSet2;
            String[] commandRepresentations;
            String[][] calculatedConstantsDescriptionsSet;
            String comment;
            Boolean isConstantSubExpressionRecognition;
            Boolean isIdenticalSubtermRecognition;
            Boolean isStackPushOptimization;
            Boolean isIdenticalVectorRecognition;

            #region Test1 : "X1 * X2 + 1.54"

            formula = "X1 * X2 + 1.54";
            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // No identical subterms detected
            Assert.IsTrue(commandRepresentationSet1.Length == 1, "Test1");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test1");
            commandRepresentationSet2 = mathCompiler.CommandRepresentationSet;
            // No identical subterms detected
            Assert.IsTrue(commandRepresentationSet2.Length == commandRepresentationSet1.Length, "Test1");
            // Push-commands reduce number of necessary commands
            Assert.IsTrue(commandRepresentationSet2[0].Length < commandRepresentationSet1[0].Length, "Test1");

            #endregion

            #region Test2 : "(X1 + X2) <= (2*3/(X3 - X4)) + \n exp(x5/(x6 - 3.79))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            // \n is to be ignored
            formula = "(X1 + X2) <= (2 * 3/(X3 - X4)) + \n exp(x5/(x6 - 3.79))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test2");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test3 : "5.57^(X1 + X2)*log(10.23,pi) <= ln(2.36*X0/(X3 - X4)) + exp(X5/(X6 - 3.79))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "5.57^(X1 + X2)*log(10.23,pi) <= ln(2.36*X0/(X3 - X4)) + exp(X5/(X6 - 3.79))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test3");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test4 : "X"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X";
            mathCompiler.SetFormula(formula);
            //  5 = "Invalid token: ' {0} '.";
            Assert.IsTrue(mathCompiler.Comment == String.Format(MathCompiler.AllComments[5], "X"), "Test4");

            #endregion

            #region Test5 : "X0"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X0";
            mathCompiler.SetFormula(formula);
            // 11 = "Formula is successfully compiled.";
            Assert.IsTrue(mathCompiler.Comment == MathCompiler.AllComments[11], "Test5");

            #endregion

            #region Test6 : "X0 + "

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X0 + ";
            mathCompiler.SetFormula(formula);
            //  4 = "Last token ' {0} ' is invalid.";
            Assert.IsTrue(mathCompiler.Comment == String.Format(MathCompiler.AllComments[4], "+"), "Test6");

            #endregion

            #region Test7 : "X0 + X"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X0 + X";
            mathCompiler.SetFormula(formula);
            //  5 = "Invalid token: ' {0} '.";
            Assert.IsTrue(mathCompiler.Comment == String.Format(MathCompiler.AllComments[5], "X"), "Test7");

            #endregion

            #region Test8 : "X0 + X1 * unknown(X0)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X0 + X1 * unknown(X0)";
            mathCompiler.SetFormula(formula);
            //  5 = "Invalid token: ' {0} '.";
            Assert.IsTrue(mathCompiler.Comment == String.Format(MathCompiler.AllComments[5], "unknown"), "Test8");

            #endregion

            #region Test9 : "log(5.5)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "log(5.5)";
            mathCompiler.SetFormula(formula);
            //  3 = "Function ' {0} ' has {1} argument(s).";
            Assert.IsTrue(mathCompiler.Comment == String.Format(MathCompiler.AllComments[3], "log", "2"), "Test9");

            #endregion

            #region Test10 : "log(5.5, e, 2.2)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "log(5.5, e, 2.2)";
            mathCompiler.SetFormula(formula);
            //  3 = "Function ' {0} ' has {1} argument(s).";
            Assert.IsTrue(mathCompiler.Comment == String.Format(MathCompiler.AllComments[3], "log", "2"), "Test10");

            #endregion

            #region Test11 : "ln(5.5, e, 2.2)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "ln(5.5, e, 2.2)";
            mathCompiler.SetFormula(formula);
            //  3 = "Function ' {0} ' has {1} argument(s).";
            Assert.IsTrue(mathCompiler.Comment == String.Format(MathCompiler.AllComments[3], "ln", "1"), "Test11");

            #endregion

            #region Test12 : "ln((5.5)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "ln((5.5)";
            mathCompiler.SetFormula(formula);
            // 12 = "Unequal number of brackets: Open/Close = {0}/{1}.";
            Assert.IsTrue(mathCompiler.Comment == String.Format(MathCompiler.AllComments[12], "2", "1"), "Test12");

            #endregion

            #region Test13 : "ln(((((5.5)))))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "ln(((((5.5)))))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test13");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test14 : "+ln(+(+(+(+(+5.5)))))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "+ln(+(+(+(+(+5.5)))))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test14");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test15 : "mean(X1{}) + 1.6"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "mean(X1{}) + 1.6";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test15");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test16 : "x0+x1*(sin(x2*x3)^x4+exp(-x2*(x0^x6+x1)/x5))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "x0+x1*(sin(x2*x3)^x4+exp(-x2*(x0^x6+x1)/x5))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test16");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            // Same formula differently written
            formula = "(exp(-x2*(x0^x6+x1)/x5)+sin(x2*x3)^x4)*x1+x0";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test16");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test17 : "sqrt((X0*X1)^X2-sin(X3)/cos(X4)*X5 + exp(X6)-(X7*X8)^X9-sin(X10)/cos(X11)*X12 + exp(X13)+(X14*X15)^X16-sin(X17)/cos(X18)*X19 + exp(X20))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "sqrt((X0*X1)^X2-sin(X3)/cos(X4)*X5 + exp(X6)-(X7*X8)^X9-sin(X10)/cos(X11)*X12 + exp(X13)+(X14*X15)^X16-sin(X17)/cos(X18)*X19 + exp(X20))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test17");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test18 : "X0*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X0*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test18");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 2, "Test18");

            #endregion

            #region Test19 : "(X0 + X1)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "(X0 + X1)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test19");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 3, "Test19");

            #endregion

            #region Test20 : "(X0 + X1)*(2.34-X2)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "(X0 + X1)*(2.34-X2)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test3");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 4, "Test20");

            #endregion

            #region Test21 : "mean(X1{}) + 1.6^mean(X1{})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "mean(X1{}) + 1.6^mean(X1{})";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test21");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 2, "Test21");

            #endregion

            #region Test22 : "subTotal(X1{}, 0, 1) + 1.5"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "subTotal(X1{}, 0, 1) + 1.5";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test22");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test23 : "subTotal(X0{}, 0, count(X0{}) - 2) < sum(X0{})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "subTotal(0, X1{}, 1) + 1.5";
            Assert.IsFalse(mathCompiler.SetFormula(formula), "Test23");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;

            #endregion

            #region Test24 : "(X1 + X2) + (X1 + X2) * exp(X3 / (X1 + X2))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "(X1 + X2) + (X1 + X2) * exp(X3 / (X1 + X2))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test24");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 2, "Test24");

            #endregion

            #region Test25 : (X4-X3) + (X1-X2) / exp(sin(x6)) + (X4-X3) + (X1-X2) / exp(sin(x6))

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "(X4 - X3) + (X1 - X2) / exp(sin(x6)) + (X4 - X3) + (X1 - X2) / exp(sin(x6))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test25");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 3, "Test25");

            #endregion

            #region Test26 : "X1 + X0"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X1 + X0";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test26");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 1, "Test26");

            #endregion

            #region Test27 : "2.31 + X1 + X0 + X3 + X2"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "2.31 + X1 + X0 + X3 + X2";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test27");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 1, "Test27");

            #endregion

            #region Test28 : "(X1 + X0) + 3.4/(X0 + X1)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "(X1 + X0) + 3.4/(X0 + X1)";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test28");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 2, "Test28");

            #endregion

            #region Test29 : "X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0)))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0)))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test29");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 2, "Test29");

            #endregion

            #region Test30 : "X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0)))+1/(X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0))))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0)))+1/(X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0))))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test30");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 3, "Test30");

            #endregion

            #region Test31 : "1+1+1+1"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "1+1+1+1";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test31");
            comment = mathCompiler.Comment;
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 1, "Test31");

            #endregion

            #region Test32 : "IF(X0 < 4, 1, 2)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "IF(X0 < 4, 1, 2)";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test32");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            commandRepresentations = mathCompiler.CommandRepresentations;
            // Expression has no identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 1, "Test32");

            #endregion

            #region Test33 : "IF(X0 < 3, X0 + 2, X0 + 3) + (X0 + 3)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "IF(X0 < 3, X0 + 2, X0 + 3) + (X0 + 3)";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test33");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            commandRepresentations = mathCompiler.CommandRepresentations;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 2, "Test33");

            #endregion

            #region Test34 : "IF(X0 < 3, 1, 2) + IF(X0 < 3, 1, 2)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "IF(X0 < 3, 1, 2) + IF(X0 < 3, 1, 2)";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test34");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            commandRepresentations = mathCompiler.CommandRepresentations;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 2, "Test34");

            #endregion

            #region Test35 : "exp(1/(x0 + 1)) + 1/(exp(1/(x0 + 1)))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "exp(1/(x0 + 1)) + 1/(exp(1/(x0 + 1)))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test35");
            commandRepresentationSet1 = mathCompiler.CommandRepresentationSet;
            commandRepresentations = mathCompiler.CommandRepresentations;
            // Expression has identical subterms
            Assert.IsTrue(commandRepresentationSet1.Length == 2, "Test35");

            #endregion

            #region Test36 : "1,2345"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "1,2345";
            Assert.IsFalse(mathCompiler.SetFormula(formula), "Test36");

            #endregion

            #region Test37 : "(X0 + 3) - (4 + exp(5/3)*(X1 - 2)) - pi/2.34 + X0 - 4/2 + 1"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "(X0 + 3) - (4 + exp(5/3)*(X1 - 2)) - pi/2.34 + X0 - 4/2 + 1";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test37a");
            calculatedConstantsDescriptionsSet = mathCompiler.CalculatedConstantCommandsRepresentationSet;
            Assert.IsTrue(calculatedConstantsDescriptionsSet.Length == 3, "Test37b");

            #endregion

            #region Test38 : "(X0 + 3) - (4 + exp(5/3)*(X1 - 2)) - pi/2.34 + X0 - (4/2 + 1) + 1/((X0 + 3) - (4 + exp(5/3)*(X1 - 2)) - pi/2.34 + X0 - (4/2 + 1))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "(X0 + 3) - (4 + exp(5/3)*(X1 - 2)) - pi/2.34 + X0 - (4/2 + 1) + 1/((X0 + 3) - (4 + exp(5/3)*(X1 - 2)) - pi/2.34 + X0 - (4/2 + 1))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test38a");
            calculatedConstantsDescriptionsSet = mathCompiler.CalculatedConstantCommandsRepresentationSet;
            Assert.IsTrue(calculatedConstantsDescriptionsSet.Length == 6, "Test38b");

            #endregion

            #region Test39 : "exp(pi/2) + 1"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "exp(pi/2) + 1";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test39a");
            calculatedConstantsDescriptionsSet = mathCompiler.CalculatedConstantCommandsRepresentationSet;
            Assert.IsTrue(calculatedConstantsDescriptionsSet.Length == 1, "Test39b");

            #endregion

            #region Test40 : "mean({2.1, 4.8, 6.3}) + count({2.1, 4.8, 6.3})/sum({2.1, 4.9, 6.3}) - count({2.1,4.8,6.3})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "mean({2.1, 4.8, 6.3}) + count({2.1, 4.8, 6.3})/sum({2.1, 4.9, 6.3}) - count({2.1,4.8,6.3})";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test40a");

            #endregion

            #region Test41 : "mean({2.1, , 6.3}) + count({2.1, 4.8, 6.3})/sum({2.1, 4.9, 6.3}) - count({2.1,4.8,6.3})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "mean({2.1, , 6.3}) + count({2.1, 4.8, 6.3})/sum({2.1, 4.9, 6.3}) - count({2.1,4.8,6.3})";
            Assert.IsFalse(mathCompiler.SetFormula(formula), "Test41a");

            #endregion

            #region Test42 : "(X1 + X2) | X3"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "(X1 + X2) | X3";
            Assert.IsFalse(mathCompiler.SetFormula(formula), "Test42a");
            Assert.IsTrue(mathCompiler.CodedComment.CodedInfo == CodedInfo.ForbiddenCharacter, "Test42b");

            #endregion

            #region Test43 : "mean({2.3, X0, X1})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "mean({2.3, X0, X1})";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test43a");

            #endregion

            #region Test44 : "({1,2})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "({1,2})";
            Assert.IsFalse(mathCompiler.SetFormula(formula), "Test44a");
            Assert.IsTrue(mathCompiler.CodedComment.CodedInfo == CodedInfo.InvalidVectorExpression, "Test44b");

            #endregion

            #region Test45 : "mean({X0,X1,X2}) + count({X0,X1,X2})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "mean({X0,X1,X2}) + count({X0,X1,X2})";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test45a");
            Assert.IsTrue(mathCompiler.CommandRepresentationSet.Length == 1, "Test45b");

            #endregion

            #region Test46 : "mean({X1-1,X2,X1-1})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "mean({X1-1,X2,X1-1})";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test46a");
            Assert.IsTrue(mathCompiler.CommandRepresentationSet.Length == 2, "Test46b");

            #endregion

            #region Test47 : "IF(X0>1, X1-1, X2+1) - mean({X1-1, X2+1})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula = "IF(X0>1, X1-1, X2+1) - mean({X1-1, X2+1})";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test47a");
            Assert.IsTrue(mathCompiler.CommandRepresentationSet.Length == 3, "Test47b");

            #endregion
        }

        #endregion

        #region Test Calculate()

        /// <summary>
        /// Test Calculate()
        /// </summary>
        [TestMethod]
        public void Test_Calculate()
        {
            MathCompiler mathCompiler1;
            MathCompiler mathCompiler2;
            String formula1;
            String formula2;
            String[][] commandRepresentationSet1;
            String[][] commandRepresentationSet2;
            String[] commandRepresentations;
            Double result1;
            Double result2;
            Double result3;
            Double result4;
            Double result5;
            Double epsilon;
            Double[][] vectorArguments;
            Double[] X;
            String comment;
            Boolean isConstantSubExpressionRecognition;
            Boolean isIdenticalSubtermRecognition;
            Boolean isStackPushOptimization;
            Boolean isIdenticalVectorRecognition;

            epsilon = 1E-14;

            #region Test1 : "X0 * X1 + 1.54"

            formula1 = "X0 * X1 + 1.54";

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test1");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2, 3 }) == 7.54, "Test1");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test1");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2, 3 }) == 7.54, "Test1");

            #endregion

            #region Test2 : "X0*ln(5.5)" vs. "X0*log(5.5,e)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "X0*ln(5.5)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test2");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler2 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula2 = "X0*log(5.5,e)";
            Assert.IsTrue(mathCompiler2.SetFormula(formula2), "Test2");
            commandRepresentationSet2 = mathCompiler2.CommandRepresentationSet;

            result1 = mathCompiler1.Calculate(new Double[] { 2 });
            result2 = mathCompiler2.Calculate(new Double[] { 2 });

            // NOTE: result1 will differ from result2 because of roundoff errors, so define epsilon
            Assert.IsTrue(result1 - result2 < epsilon, "Test2");

            #endregion

            #region Test3 : "1.54"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "1.54";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test3");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null) == 1.54, "Test3");

            #endregion

            #region Test4 : "mean(X0{}) + 1.6"

            formula1 = "mean(X0{}) + 1.6";
            vectorArguments = new Double[1][];
            vectorArguments[0] = new Double[] { 1, 2, 3, 4 };

            isConstantSubExpressionRecognition = false;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test4");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == 4.1, "Test4");

            isConstantSubExpressionRecognition = false;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test4");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == 4.1, "Test4");

            #endregion

            #region Test5 : "5.57^(X1 + X2) * log(10.23, pi) - ln(2.36 * X0 / (X3 - X4)) + exp(X5 / (X6 - 3.79))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "5.57^(X1 + X2) * log(10.23, pi) - ln(2.36 * X0 / (X3 - X4)) + exp(X5 / (X6 - 3.79))";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test5");
            X = new Double[] { 7, 6, 5, 4, 3, 2, 1 };

            result1 = mathCompiler1.Calculate(X);
            result2 = Math.Pow(5.57, (X[1] + X[2])) * Math.Log(10.23, Math.PI) - Math.Log(2.36 * X[0] / (X[3] - X[4])) + Math.Exp(X[5] / (X[6] - 3.79));
            result3 = mathCompiler1.CalculateSafe(X);
            result4 = mathCompiler1.Calculate(X, null);
            result5 = mathCompiler1.CalculateSafe(X, null);
            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test5");
            Assert.IsTrue((result3 - result2) < result1 * epsilon, "Test5");
            Assert.IsTrue((result4 - result2) < result1 * epsilon, "Test5");
            Assert.IsTrue((result5 - result2) < result1 * epsilon, "Test5");

            #endregion

            #region Test6 : "sqrt((X0*X1)^X2-sin(X3)/cos(X4)*X5 + exp(X6)-(X7*X8)^X9-sin(X10)/cos(X11)*X12 + exp(X13)+(X14*X15)^X16-sin(X17)/cos(X18)*X19 + exp(X20))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "sqrt((X0*X1)^X2-sin(X3)/cos(X4)*X5 + exp(X6)-(X7*X8)^X9-sin(X10)/cos(X11)*X12 + exp(X13)+(X14*X15)^X16-sin(X17)/cos(X18)*X19 + exp(X20))";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test6");
            X = new Double[] { 1.6, 2.0, 3.6, 8.6, 1.2, 9.65, 0.95, 1.6, 2.0, 3.6, 8.6, 1.2, 9.65, 0.95, 1.61, 2.0, 3.6, 8.6, 1.2, 9.65, 0.95 };

            result1 = mathCompiler1.Calculate(X);
            result2 = Math.Sqrt(Math.Pow((X[0] * X[1]), X[2]) - Math.Sin(X[3]) / Math.Cos(X[4]) * X[5] + Math.Exp(X[6]) - Math.Pow((X[7] * X[8]), X[9]) - Math.Sin(X[10]) / Math.Cos(X[11]) * X[12] + Math.Exp(X[13]) + Math.Pow((X[14] * X[15]), X[16]) - Math.Sin(X[17]) / Math.Cos(X[18]) * X[19] + Math.Exp(X[20]));
            result3 = mathCompiler1.CalculateSafe(X);
            result4 = mathCompiler1.Calculate(X, null);
            result5 = mathCompiler1.CalculateSafe(X, null);
            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test6");
            Assert.IsTrue((result3 - result2) < result1 * epsilon, "Test6");
            Assert.IsTrue((result4 - result2) < result1 * epsilon, "Test6");
            Assert.IsTrue((result5 - result2) < result1 * epsilon, "Test5");

            #endregion

            #region Test7 : "x0+x1*(sin(x2*x3)^x4+exp(-x2*(x0^x6+x1)/x5))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "x0+x1*(sin(x2*x3)^x4+exp(-x2*(x0^x6+x1)/x5))";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test7");
            X = new Double[] { 1.6, 0.1, 0.11, 8.6, 1.2, 9.65, 0.95 };

            result1 = mathCompiler1.Calculate(X);
            result2 = X[0] + X[1] * (Math.Pow(Math.Sin(X[2] * X[3]), X[4]) + Math.Exp(-X[2] * (Math.Pow(X[0], X[6]) + X[1]) / X[5]));
            result3 = mathCompiler1.CalculateSafe(X);
            result4 = mathCompiler1.Calculate(X, null);
            result5 = mathCompiler1.CalculateSafe(X, null);
            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test7");
            Assert.IsTrue((result3 - result2) < result1 * epsilon, "Test7");
            Assert.IsTrue((result4 - result2) < result1 * epsilon, "Test7");
            Assert.IsTrue((result5 - result2) < result1 * epsilon, "Test5");

            #endregion

            #region Test8a : "X0*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))"

            // WITHOUT identical subterm elimination
            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "X0*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test8a");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(commandRepresentationSet1.Length == 1, "Test8a");
            X = new Double[] { 1.6, 0.1, 0.11, 8.6 };

            result1 = mathCompiler1.Calculate(X);
            result2 = X[0] * Math.Exp(X[1] / (2.34 - X[2])) + X[3] - 3 / Math.Exp(X[1] / (2.34 - X[2]));
            result3 = mathCompiler1.CalculateSafe(X);
            result4 = mathCompiler1.Calculate(X, null);
            result5 = mathCompiler1.CalculateSafe(X, null);
            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test8a");
            Assert.IsTrue((result3 - result2) < result1 * epsilon, "Test8a");
            Assert.IsTrue((result4 - result2) < result1 * epsilon, "Test8a");
            Assert.IsTrue((result5 - result2) < result1 * epsilon, "Test5");

            #endregion

            #region Test8b : "X0*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))"

            // WITH identical subterm elimination
            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "X0*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test8b");
            X = new Double[] { 1.6, 0.1, 0.11, 8.6 };

            result1 = mathCompiler1.Calculate(X);
            result2 = X[0] * Math.Exp(X[1] / (2.34 - X[2])) + X[3] - 3 / Math.Exp(X[1] / (2.34 - X[2]));
            result3 = mathCompiler1.CalculateSafe(X);
            result4 = mathCompiler1.Calculate(X, null);
            result5 = mathCompiler1.CalculateSafe(X, null);
            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test8b");
            Assert.IsTrue((result3 - result2) < result1 * epsilon, "Test8b");
            Assert.IsTrue((result4 - result2) < result1 * epsilon, "Test8b");
            Assert.IsTrue((result5 - result2) < result1 * epsilon, "Test5");

            #endregion

            #region Test9a : "(X0 + X1)*(2.34-X2)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)"

            // WITHOUT identical subterm elimination
            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "(X0 + X1)*(2.34-X2)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test9a");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(commandRepresentationSet1.Length == 1, "Test9a");
            X = new Double[] { 1.6, 0.1, 0.11, 8.6 };

            result1 = mathCompiler1.Calculate(X);
            result2 = (X[0] + X[1]) * (2.34 - X[2]) * Math.Exp(X[1] / (2.34 - X[2])) + X[3] - 3 / Math.Exp(X[1] / (2.34 - X[2])) + (X[0] + X[1]);
            result3 = mathCompiler1.CalculateSafe(X);
            result4 = mathCompiler1.Calculate(X, null);
            result5 = mathCompiler1.CalculateSafe(X, null);
            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test9a");
            Assert.IsTrue((result3 - result2) < result1 * epsilon, "Test9a");
            Assert.IsTrue((result4 - result2) < result1 * epsilon, "Test9a");
            Assert.IsTrue((result5 - result2) < result1 * epsilon, "Test5");

            #endregion

            #region Test9b : "(X0 + X1)*(2.34-X2)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)"

            // WITH identical subterm elimination
            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "(X0 + X1)*(2.34-X2)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test9b");
            X = new Double[] { 1.6, 0.1, 0.11, 8.6 };

            result1 = mathCompiler1.Calculate(X);
            result2 = (X[0] + X[1]) * (2.34 - X[2]) * Math.Exp(X[1] / (2.34 - X[2])) + X[3] - 3 / Math.Exp(X[1] / (2.34 - X[2])) + (X[0] + X[1]);
            result3 = mathCompiler1.CalculateSafe(X);
            result4 = mathCompiler1.Calculate(X, null);
            result5 = mathCompiler1.CalculateSafe(X, null);
            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test9b");
            Assert.IsTrue((result3 - result2) < result1 * epsilon, "Test9b");
            Assert.IsTrue((result4 - result2) < result1 * epsilon, "Test9b");
            Assert.IsTrue((result5 - result2) < result1 * epsilon, "Test5");

            #endregion

            #region Test10 : "subTotal(X0{}, 0, 2) + 1.5"

            formula1 = "subTotal(X0{}, 0, 2) + 1.5";
            vectorArguments = new Double[1][];
            vectorArguments[0] = new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test10");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == 7.5, "Test10");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test10");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == 7.5, "Test10");

            #endregion

            #region Test11 : "subTotal(X0{}, 7, 8) + 1.5"

            formula1 = "subTotal(X0{}, 7, 8) + 1.5";
            vectorArguments = new Double[1][];
            vectorArguments[0] = new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test11");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == 18.5, "Test11");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test11");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == 18.5, "Test11");

            #endregion

            #region Test12 : "subTotal(X0{}, 0, count(X0{}) - 1) = sum(X0{})"

            formula1 = "subTotal(X0{}, 0, count(X0{}) - 1) = sum(X0{})";
            vectorArguments = new Double[1][];
            vectorArguments[0] = new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test12");
            comment = mathCompiler1.Comment;
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == MathCompiler.True, "Test12");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test12");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == MathCompiler.True, "Test12");

            #endregion

            #region Test13 : "subTotal(X0{}, 0, count(X0{}) - 2) < sum(X0{})"

            formula1 = "subTotal(X0{}, 0, count(X0{}) - 2) < sum(X0{})";
            vectorArguments = new Double[1][];
            vectorArguments[0] = new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test13");
            comment = mathCompiler1.Comment;
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == MathCompiler.True, "Test13");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test13");
            commandRepresentationSet1 = mathCompiler1.CommandRepresentationSet;
            Assert.IsTrue(mathCompiler1.Calculate(null, vectorArguments) == MathCompiler.True, "Test13");

            #endregion

            #region Test14 : "X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0)))"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0)))";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test14");
            X = new Double[] { 7, 6, 5, };

            result1 = mathCompiler1.Calculate(X);
            result2 = X[0] + Math.Exp(X[1] / (X[0] + X[1])) * Math.Exp(X[1] / (X[2] + X[0])) - 2.74 / (Math.Exp(X[1] / (X[0] + X[2])) * Math.Exp(X[1] / (X[1] + X[0])));
            result3 = mathCompiler1.CalculateSafe(X);
            result4 = mathCompiler1.Calculate(X, null);
            result5 = mathCompiler1.CalculateSafe(X, null);
            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test14");
            Assert.IsTrue((result3 - result2) < result1 * epsilon, "Test14");
            Assert.IsTrue((result4 - result2) < result1 * epsilon, "Test14");
            Assert.IsTrue((result5 - result2) < result1 * epsilon, "Test14");

            #endregion

            #region Test15 : "IF(X0 < 4, 1, 2)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "IF(X0 < 4, 1, 2)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test15");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 2 }) == 1, "Test15");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 5 }) == 2, "Test15");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 2 }, null) == 1, "Test15");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 5 }, null) == 2, "Test15");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2 }) == 1, "Test15");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 5 }) == 2, "Test15");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2 }, null) == 1, "Test15");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 5 }, null) == 2, "Test15");

            #endregion

            #region Test16 : "IF(X0 < 3, X0 + 2, X0 + 3) + (X0 + 3)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "IF(X0 < 3, X0 + 2, X0 + 3) + (X0 + 3)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test16");
            commandRepresentations = mathCompiler1.CommandRepresentations;
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 2 }) == 9, "Test16");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 5 }) == 16, "Test16");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 2 }, null) == 9, "Test16");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 5 }, null) == 16, "Test16");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2 }) == 9, "Test16");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 5 }) == 16, "Test16");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2 }, null) == 9, "Test16");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 5 }, null) == 16, "Test16");

            #endregion

            #region Test17 : "IF(X0 < 3, 1, 2) + IF(X0 < 3, 1, 2)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "IF(X0 < 3, 1, 2) + IF(X0 < 3, 1, 2)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test17");
            commandRepresentations = mathCompiler1.CommandRepresentations;
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 2 }) == 2, "Test17");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 5 }) == 4, "Test17");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 2 }, null) == 2, "Test17");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 5 }, null) == 4, "Test17");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2 }) == 2, "Test17");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 5 }) == 4, "Test17");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2 }, null) == 2, "Test17");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 5 }, null) == 4, "Test17");

            #endregion

            #region Test18 : "IF(IF(X0 < 3, true, false), true, false)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "IF(IF(X0 < 3, true, false), true, false)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test18");
            commandRepresentations = mathCompiler1.CommandRepresentations;
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 2 }) == MathCompiler.True, "Test18");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 5 }) == MathCompiler.False, "Test18");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 2 }, null) == MathCompiler.True, "Test18");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 5 }, null) == MathCompiler.False, "Test18");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2 }) == MathCompiler.True, "Test18");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 5 }) == MathCompiler.False, "Test18");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 2 }, null) == MathCompiler.True, "Test18");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 5 }, null) == MathCompiler.False, "Test18");

            #endregion

            #region Test19 : "IF(X0 = 0, X0, 1/X0) + IF(X0 = 0, X0, 1/X0)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "IF(X0 = 0, X0, 2/X0) + IF(X0 = 0, X0, 2/X0)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test19");
            commandRepresentations = mathCompiler1.CommandRepresentations;
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 0 }) == 0, "Test19");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 1 }) == 4, "Test19");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 0 }, null) == 0, "Test19");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 1 }, null) == 4, "Test19");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 0 }) == 0, "Test19");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 1 }) == 4, "Test19");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 0 }, null) == 0, "Test19");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 1 }, null) == 4, "Test19");

            #endregion

            #region Test20 : "count(X1{})"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "count(X1{})";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test20");
            commandRepresentations = mathCompiler1.CommandRepresentations;
            Assert.IsTrue(mathCompiler1.CalculateSafe(null, new Double[][] { new double[] { 1, 2, 3 }, new double[] { 4, 5, 6, 7 } }) == 4, "Test20");
            Assert.IsTrue(mathCompiler1.Calculate(null, new Double[][] { new double[] { 1, 2, 3 }, new double[] { 4, 5, 6, 7 } }) == 4, "Test20");

            #endregion

            #region Test21 : "4 + exp(5/3) - pi/2.34 + X0 - (4/2 + 1)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "4 + exp(5/3) - pi/2.34 + X0 - (4/2 + 1)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test21");
            commandRepresentations = mathCompiler1.CommandRepresentations;
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 1.0 }) > 5.9 &&
                mathCompiler1.CalculateSafe(new Double[] { 1.0 }) < 6.0, "Test21");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 1.0 }) > 5.9 &&
                mathCompiler1.Calculate(new Double[] { 1.0 }) < 6.0, "Test21");

            #endregion

            #region Test22 : "exp(pi/2 + 4.37) + X0*exp(pi/2 + 4.37) - 1/exp(pi/2 + 4.37)"

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "exp(pi/2 + 4.37) + X0*exp(pi/2 + 4.37) - 1/exp(pi/2 + 4.37)";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test22");
            commandRepresentations = mathCompiler1.CommandRepresentations;
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 1.0 }) < 761, "Test22a");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 1.0 }) < 761, "Test22b");

            #endregion

            #region Test23 : "mean({2.0, X0, X1})"

            isConstantSubExpressionRecognition = false;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "mean({2.0, X0, X1})";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test23");
            Assert.IsTrue(mathCompiler1.CalculateSafe(new Double[] { 3.0, 4.0 }) == 3.0, "Test23a");
            Assert.IsTrue(mathCompiler1.Calculate(new Double[] { 3.0, 4.0 }) == 3.0, "Test23b");

            #endregion

            #region Test24 : "mean({1, 2, {3, 4}}) + 1/mean({1, 2, {3, 4}})"

            isConstantSubExpressionRecognition = false;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = false;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            formula1 = "mean({1, 2, {3, 4}}) + 1/mean({1, 2, {3, 4}})";
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test24");
            Assert.IsTrue(mathCompiler1.CalculateSafe() == 2.9, "Test24a");
            Assert.IsTrue(mathCompiler1.Calculate() == 2.9, "Test24b");

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler1 = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler1.SetFormula(formula1), "Test24c");
            Assert.IsTrue(mathCompiler1.CalculateSafe() == 2.9, "Test24d");
            Assert.IsTrue(mathCompiler1.Calculate() == 2.9, "Test24e");

            #endregion
        }

        #endregion

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Test performance

        /// <summary>
        /// Test performance of SetFormula()
        /// </summary>
        [TestMethod]
        public void Test_Performance_SetFormula_1()
        {
            MathCompiler[] mathCompilers;
            String formula;
            Int32 numberOfMathCompilers;
            Int32 startTime;
            Int32 endTime;
            Int32 difference;

            // ------------------------
            // Performance Quantity
            // ------------------------
            numberOfMathCompilers = 10;
            // ------------------------

            mathCompilers = new MathCompiler[numberOfMathCompilers];
            formula = "5.57^(X1 + X2) * log(10.23, pi) <= ln(2.36 * X0 / (X3 - X4)) + exp(X5 / (X6 - 3.79))";

            startTime = Environment.TickCount;
            for (Int32 i = 0; i < numberOfMathCompilers; i++)
            {
                mathCompilers[i] = new MathCompiler();
                mathCompilers[i].SetFormula(formula);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference = endTime - startTime;

            for (Int32 i = 0; i < numberOfMathCompilers; i++)
            {
                Assert.IsTrue(mathCompilers[i].IsCompiled, "Test1");
            }
        }
        /// <summary>
        /// Test performance of Calculate()
        /// </summary>
        [TestMethod]
        public void Test_Performance_Calculate_1()
        {
            MathCompiler mathCompiler;
            String formula;
            Int32 number;
            Int32 startTime;
            Int32 endTime;
            Int32 difference1;
            Int32 difference2;
            Double ratio;
            Double result1;
            Double result2;
            Double[] X;
            Double epsilon;

            // ------------------------
            // Performance Quantity
            // ------------------------
            number = 10;
            // ------------------------

            // epsilon is necessary because of roundoff errors
            epsilon = 1E-14;
            result1 = 0;
            result2 = 0;
            mathCompiler = new MathCompiler();

            formula = "5.57^(X1 + X2) * log(10.23, pi) - ln(2.36 * X0 / (X3 - X4)) + exp(X5 / (X6 - 3.79))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            X = new Double[] { 7, 6, 5, 4, 3, 2, 1 };

            #region Performance of MathCompiler

            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result1 = mathCompiler.Calculate(X);
                //result1 = mathCompiler.Calculate(X, null);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference1 = endTime - startTime;

            #endregion

            #region Performance of C# function evaluation

            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result2 = Math.Pow(5.57, (X[1] + X[2])) * Math.Log(10.23, Math.PI) - Math.Log(2.36 * X[0] / (X[3] - X[4])) + Math.Exp(X[5] / (X[6] - 3.79));
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference2 = endTime - startTime;

            #endregion

            // Performance ratio MathCompiler/C#
            ratio = System.Convert.ToDouble(difference1) / System.Convert.ToDouble(difference2);

            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test1");
        }
        /// <summary>
        /// Test performance of Calculate()
        /// </summary>
        [TestMethod]
        public void Test_Performance_Calculate_2()
        {
            MathCompiler mathCompiler;
            String formula;
            Int32 number;
            Int32 startTime;
            Int32 endTime;
            Int32 difference1;
            Int32 difference2;
            Double ratio;
            Double result1;
            Double result2;
            Double[] X;
            Double epsilon;

            // ------------------------
            // Performance Quantity
            // ------------------------
            number = 10;
            // ------------------------

            // epsilon is necessary because of roundoff errors
            epsilon = 1E-14;
            result1 = 0;
            result2 = 0;
            mathCompiler = new MathCompiler();

            formula = "sqrt((X0*X1)^X2-sin(X3)/cos(X4)*X5 + exp(X6)-(X7*X8)^X9-sin(X10)/cos(X11)*X12 + exp(X13)+(X14*X15)^X16-sin(X17)/cos(X18)*X19 + exp(X20))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            X = new Double[] { 1.6, 2.0, 3.6, 8.6, 1.2, 9.65, 0.95, 1.6, 2.0, 3.6, 8.6, 1.2, 9.65, 0.95, 1.61, 2.0, 3.6, 8.6, 1.2, 9.65, 0.95 };

            #region Performance of MathCompiler
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result1 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference1 = endTime - startTime;
            #endregion

            #region Performance of C# function evaluation
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result2 = Math.Sqrt(Math.Pow((X[0] * X[1]), X[2]) - Math.Sin(X[3]) / Math.Cos(X[4]) * X[5] + Math.Exp(X[6]) - Math.Pow((X[7] * X[8]), X[9]) - Math.Sin(X[10]) / Math.Cos(X[11]) * X[12] + Math.Exp(X[13]) + Math.Pow((X[14] * X[15]), X[16]) - Math.Sin(X[17]) / Math.Cos(X[18]) * X[19] + Math.Exp(X[20]));
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference2 = endTime - startTime;
            #endregion

            // Performance ratio MathCompiler/C#
            ratio = System.Convert.ToDouble(difference1) / System.Convert.ToDouble(difference2);

            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test1");
        }
        /// <summary>
        /// Test performance of Calculate()
        /// </summary>
        [TestMethod]
        public void Test_Performance_Calculate_3()
        {
            MathCompiler mathCompiler;
            String formula;
            Int32 number;
            Int32 startTime;
            Int32 endTime;
            Int32 difference1;
            Int32 difference2;
            Double ratio;
            Double result1;
            Double result2;
            Double[] X;
            Double epsilon;

            // ------------------------
            // Performance Quantity
            // ------------------------
            number = 10;
            // ------------------------

            // epsilon is necessary because of roundoff errors
            epsilon = 1E-14;
            result1 = 0;
            result2 = 0;
            mathCompiler = new MathCompiler();

            formula = "x0+x1*(sin(x2*x3)^x4+exp(-x2*(x0^x6+x1)/x5))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            X = new Double[] { 1.6, 0.1, 0.11, 8.6, 1.2, 9.65, 0.95 };

            #region Performance of MathCompiler
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result1 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference1 = endTime - startTime;
            #endregion

            #region Performance of C# function evaluation
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result2 = X[0] + X[1] * (Math.Pow(Math.Sin(X[2] * X[3]), X[4]) + Math.Exp(-X[2] * (Math.Pow(X[0], X[6]) + X[1]) / X[5]));
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference2 = endTime - startTime;
            #endregion

            // Performance ratio MathCompiler/C#
            ratio = System.Convert.ToDouble(difference1) / System.Convert.ToDouble(difference2);

            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test1");
        }
        /// <summary>
        /// Test performance of Calculate()
        /// </summary>
        [TestMethod]
        public void Test_Performance_Calculate_4()
        {
            MathCompiler mathCompiler;
            String formula;
            Int32 number;
            Int32 startTime;
            Int32 endTime;
            Int32 difference1;
            Int32 difference2;
            Double ratio;
            Double result1;
            Double result2;
            Double[] X;
            Double epsilon;
            String[][] commandRepresentationSet;

            // ------------------------
            // Performance Quantity
            // ------------------------
            number = 10;
            // ------------------------

            // epsilon is necessary because of roundoff errors
            epsilon = 1E-14;
            result1 = 0;
            result2 = 0;
            mathCompiler = new MathCompiler();

            formula = "X0*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            commandRepresentationSet = mathCompiler.CommandRepresentationSet;
            X = new Double[] { 1.6, 0.1, 0.11, 8.6 };

            #region Performance of MathCompiler
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result1 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference1 = endTime - startTime;
            #endregion

            #region Performance of C# function evaluation
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result2 = X[0] * Math.Exp(X[1] / (2.34 - X[2])) + X[3] - 3 / Math.Exp(X[1] / (2.34 - X[2]));
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference2 = endTime - startTime;
            #endregion

            // Performance ratio MathCompiler/C#
            ratio = System.Convert.ToDouble(difference1) / System.Convert.ToDouble(difference2);

            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test1");
        }
        /// <summary>
        /// Test performance of Calculate()
        /// </summary>
        [TestMethod]
        public void Test_Performance_Calculate_5()
        {
            MathCompiler mathCompiler;
            String formula;
            Int32 number;
            Int32 startTime;
            Int32 endTime;
            Int32 difference1;
            Int32 difference2;
            Double ratio;
            Double result1;
            Double result2;
            Double[] X;
            Double epsilon;
            String[][] commandRepresentationSet;
            Boolean isConstantSubExpressionRecognition;
            Boolean isIdenticalSubtermRecognition;
            Boolean isStackPushOptimization;
            Boolean isIdenticalVectorRecognition;

            // ------------------------
            // Performance Quantity
            // ------------------------
            number = 10;
            // ------------------------

            // epsilon is necessary because of roundoff errors
            epsilon = 1E-14;
            result1 = 0;
            result2 = 0;
            mathCompiler = new MathCompiler();

            formula = "X0*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))";
            X = new Double[] { 1.6, 0.1, 0.11, 8.6 };

            #region Performance of MathCompiler WITHOUT identical subterm elimination

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            commandRepresentationSet = mathCompiler.CommandRepresentationSet;
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result1 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference1 = endTime - startTime;

            #endregion

            #region Performance of MathCompiler WITH identical subterm elimination

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            commandRepresentationSet = mathCompiler.CommandRepresentationSet;
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result2 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference2 = endTime - startTime;

            #endregion

            // Performance ratio WITHOUT/WITH
            ratio = System.Convert.ToDouble(difference1) / System.Convert.ToDouble(difference2);

            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test1");
        }
        /// <summary>
        /// Test performance of Calculate()
        /// </summary>
        [TestMethod]
        public void Test_Performance_Calculate_6()
        {
            MathCompiler mathCompiler;
            String formula;
            Int32 number;
            Int32 startTime;
            Int32 endTime;
            Int32 difference1;
            Int32 difference2;
            Double ratio;
            Double result1;
            Double result2;
            Double[] X;
            Double epsilon;
            String[][] commandRepresentationSet;
            Boolean isConstantSubExpressionRecognition;
            Boolean isIdenticalSubtermRecognition;
            Boolean isStackPushOptimization;
            Boolean isIdenticalVectorRecognition;

            // ------------------------
            // Performance Quantity
            // ------------------------
            number = 10;
            // ------------------------

            // epsilon is necessary because of roundoff errors
            epsilon = 1E-14;
            result1 = 0;
            result2 = 0;
            mathCompiler = new MathCompiler();

            formula = "(X0 + X1)*(2.34-X2)*exp(X1/(2.34-X2))+X3-3/exp(X1/(2.34-X2))+(X0 + X1)";
            X = new Double[] { 1.6, 0.1, 0.11, 8.6 };

            #region Performance of MathCompiler WITHOUT identical subterm elimination

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            commandRepresentationSet = mathCompiler.CommandRepresentationSet;
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result1 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference1 = endTime - startTime;

            #endregion

            #region Performance of MathCompiler WITH identical subterm elimination

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            commandRepresentationSet = mathCompiler.CommandRepresentationSet;
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result2 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference2 = endTime - startTime;

            #endregion

            // Performance ratio WITHOUT/WITH
            ratio = System.Convert.ToDouble(difference1) / System.Convert.ToDouble(difference2);

            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test1");
        }
        /// <summary>
        /// Test performance of Calculate()
        /// </summary>
        [TestMethod]
        public void Test_Performance_Calculate_7()
        {
            MathCompiler mathCompiler;
            String formula;
            Int32 number;
            Int32 startTime;
            Int32 endTime;
            Int32 difference1;
            Int32 difference2;
            Double ratio;
            Double result1;
            Double result2;
            Double[] X;
            Double epsilon;
            String[][] commandRepresentationSet;
            Boolean isConstantSubExpressionRecognition;
            Boolean isIdenticalSubtermRecognition;
            Boolean isStackPushOptimization;
            Boolean isIdenticalVectorRecognition;

            // ------------------------
            // Performance Quantity
            // ------------------------
            number = 10;
            // ------------------------

            // epsilon is necessary because of roundoff errors
            epsilon = 1E-14;
            result1 = 0;
            result2 = 0;
            mathCompiler = new MathCompiler();

            formula = "X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0)))";
            X = new Double[] { 7, 6, 5, };

            #region Performance of MathCompiler WITHOUT identical subterm elimination

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = false;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            commandRepresentationSet = mathCompiler.CommandRepresentationSet;
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result1 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference1 = endTime - startTime;

            #endregion

            #region Performance of MathCompiler WITH identical subterm elimination

            isConstantSubExpressionRecognition = true;
            isIdenticalSubtermRecognition = true;
            isStackPushOptimization = true;
            isIdenticalVectorRecognition = true;
            mathCompiler = new MathCompiler(
                isConstantSubExpressionRecognition,
                isIdenticalSubtermRecognition,
                isStackPushOptimization,
                isIdenticalVectorRecognition
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            commandRepresentationSet = mathCompiler.CommandRepresentationSet;
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result2 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference2 = endTime - startTime;

            #endregion

            // Performance ratio WITHOUT/WITH
            ratio = System.Convert.ToDouble(difference1) / System.Convert.ToDouble(difference2);

            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test1");
        }
        [TestMethod]
        public void Test_Performance_Calculate_8()
        {
            MathCompiler mathCompiler;
            String formula;
            Int32 number;
            Int32 startTime;
            Int32 endTime;
            Int32 difference1;
            Int32 difference2;
            Double ratio;
            Double result1;
            Double result2;
            Double[] X;
            Double epsilon;
            String[][] commandRepresentationSet;

            // ------------------------
            // Performance Quantity
            // ------------------------
            number = 10;
            // ------------------------

            // epsilon is necessary because of roundoff errors
            epsilon = 1E-14;
            result1 = 0;
            result2 = 0;
            mathCompiler = new MathCompiler();

            formula = "X0+exp(X1/(X0+X1))*exp(X1/(X2+X0))-2.74/(exp(X1/(X0+X2))*exp(X1/(X1+X0)))";
            Assert.IsTrue(mathCompiler.SetFormula(formula), "Test0");
            commandRepresentationSet = mathCompiler.CommandRepresentationSet;
            X = new Double[] { 7, 6, 5, };

            #region Performance of MathCompiler
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result1 = mathCompiler.Calculate(X);
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference1 = endTime - startTime;
            #endregion

            #region Performance of C# function evaluation
            startTime = Environment.TickCount;
            for (Int32 i = 0; i < number; i++)
            {
                result2 = X[0] + Math.Exp(X[1] / (X[0] + X[1])) * Math.Exp(X[1] / (X[2] + X[0])) - 2.74 / (Math.Exp(X[1] / (X[0] + X[2])) * Math.Exp(X[1] / (X[1] + X[0])));
            }
            endTime = Environment.TickCount;
            // Difference in milliseconds
            difference2 = endTime - startTime;
            #endregion

            // Performance ratio MathCompiler/C#
            ratio = System.Convert.ToDouble(difference1) / System.Convert.ToDouble(difference2);

            // Check result
            Assert.IsTrue((result1 - result2) < result1 * epsilon, "Test1");
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Code snippets

        /// <summary>
        /// Code snippet
        /// </summary>
        [TestMethod]
        public void CodeSnippet1()
        {
            Double[] myArguments;
            Double myCalculatedValue;
            String formula;
            MathCompiler myMathCompiler;
            
            // Set formula
            formula = "X0 / (X1 - 3.5) * exp(-X2^2)";
            // Create MathCompiler instance
            myMathCompiler = new MathCompiler();
            // Set formula
            if (!myMathCompiler.SetFormula(formula))
            {
                // Something went wrong: See properties Comment ... 
                String detailedComment = myMathCompiler.Comment;
                // ... or CodedComment ...
                CodedInfoItem detailedInfoItem = 
                    myMathCompiler.CodedComment;
                // ... for detailed information ...
            }
            // Set Arguments:
            // myArguments[0] corresponds to X0, 
            // myArguments[1] corresponds to X1 etc.
            myArguments = new Double[] { 2.1, 6.3, 4.7 };
            // Calculate function value
            try
            {
                myCalculatedValue = 
                    myMathCompiler.Calculate(myArguments);
            }
            catch (Exception)
            {
                // Exception handling ...
            }
        }

        /// <summary>
        /// Code snippet
        /// </summary>
        [TestMethod]
        public void CodeSnippet2()
        {
            Double[] myArguments;
            Double[][] myVectorArguments;
            Double myCalculatedValue;
            String formula;
            MathCompiler myMathCompiler;

            // Set formula
            formula = "mean(X0{}) * (X0 + 2.5)";
            // Create MathCompiler instance
            myMathCompiler = new MathCompiler();
            // Set formula
            if (!myMathCompiler.SetFormula(formula))
            {
                // Something went wrong: See properties Comment ... 
                String detailedComment = myMathCompiler.Comment;
                // ... or CodedComment ...
                CodedInfoItem detailedInfoItem =
                    myMathCompiler.CodedComment;
                // ... for detailed information ...
            }
            // Set Arguments:
            // myArguments[0] corresponds to X0, 
            // myArguments[1] corresponds to X1 etc.
            myArguments = new Double[] { 3.5 };
            // myVectorArguments[0] corresponds to X0{}, 
            // myVectorArguments[1] corresponds to X1{} etc.
            myVectorArguments = new Double[1][];
            myVectorArguments[0] = new Double[] { 1,2,3,4 };
            // Calculate function value
            try
            {
                myCalculatedValue =
                    myMathCompiler.Calculate(myArguments, myVectorArguments);
            }
            catch (Exception)
            {
                // Exception handling ...
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        #region Private methods

        /// <summary>
        /// Test invalid formula
        /// </summary>
        /// <param name="formula">
        /// Parameter
        /// </param>
        /// <param name="codedInfo">
        /// Parameter
        /// </param>
        /// <param name="testInfo">
        /// Parameter
        /// </param>
        private void TestInvalidFormula(
            String formula,
            CodedInfo codedInfo,
            String testInfo)
        {
            MathCompiler mathCompiler = new MathCompiler();
            Assert.IsFalse(mathCompiler.SetFormula(formula), testInfo + "a");
            Assert.IsTrue(mathCompiler.CodedComment.CodedInfo == codedInfo, testInfo + "b");
        }
        /// <summary>
        /// Test valid formula
        /// </summary>
        /// <param name="formula">
        /// Parameter
        /// </param>
        /// <param name="scalarArguments">
        /// Parameter
        /// </param>
        /// <param name="vectorArguments">
        /// Parameter
        /// </param>
        /// <param name="correctFunctionValue">
        /// Parameter
        /// </param>
        /// <param name="isOptimizationOption1">
        /// Parameter
        /// </param>
        /// <param name="isOptimizationOption2">
        /// Parameter
        /// </param>
        /// <param name="isOptimizationOption3">
        /// Parameter
        /// </param>
        /// <param name="isOptimizationOption4">
        /// Parameter
        /// </param>
        /// <param name="scalarArgumentCount">
        /// Parameter
        /// </param>
        /// <param name="vectorArgumentCount">
        /// Parameter
        /// </param>
        /// <param name="hasJump">
        /// Parameter
        /// </param>
        /// <param name="hasVector">
        /// Parameter
        /// </param>
        /// <param name="hasNestedVector">
        /// Parameter
        /// </param>
        /// <param name="numberOfConstants">
        /// Parameter
        /// </param>
        /// <param name="numberOfVectorConstants">
        /// Parameter
        /// </param>
        /// <param name="numberOfCalculatedConstants">
        /// Parameter
        /// </param>
        /// <param name="numberOfSubtermConstants">
        /// Parameter
        /// </param>
        /// <param name="numberOfVectorSubterms">
        /// Parameter
        /// </param>
        /// <param name="testInfo">
        /// Parameter
        /// </param>
        private void TestValidFormula(
            String formula,
            Double[] scalarArguments,
            Double[][] vectorArguments,
            Double correctFunctionValue,
            Boolean isOptimizationOption1,
            Boolean isOptimizationOption2,
            Boolean isOptimizationOption3,
            Boolean isOptimizationOption4,
            Int32 scalarArgumentCount,
            Int32 vectorArgumentCount,
            Boolean hasJump,
            Boolean hasVector,
            Boolean hasNestedVector,
            Int32 numberOfConstants,
            Int32 numberOfVectorConstants,
            Int32 numberOfCalculatedConstants,
            Int32 numberOfSubtermConstants,
            Int32 numberOfVectorSubterms,
            String testInfo)
        {
            MathCompiler mathCompiler = new MathCompiler(
                isOptimizationOption1,
                isOptimizationOption2,
                isOptimizationOption3,
                isOptimizationOption4
            );
            Assert.IsTrue(mathCompiler.SetFormula(formula), testInfo + "a");
            Assert.IsTrue(this.AreNumericalValuesEqual(correctFunctionValue, mathCompiler.Calculate(scalarArguments, vectorArguments)), testInfo + "b");
            Assert.IsTrue(this.AreNumericalValuesEqual(correctFunctionValue, mathCompiler.CalculateSafe(scalarArguments, vectorArguments)), testInfo + "c");
            Assert.IsTrue(mathCompiler.ScalarArgumentCount == scalarArgumentCount, testInfo + "d");
            Assert.IsTrue(mathCompiler.VectorArgumentCount== vectorArgumentCount, testInfo + "e");
            Assert.IsTrue(mathCompiler.HasJump == hasJump, testInfo + "f");
            Assert.IsTrue(mathCompiler.HasVector == hasVector, testInfo + "g");
            Assert.IsTrue(mathCompiler.HasNestedVector == hasNestedVector, testInfo + "h");
            Assert.IsTrue(mathCompiler.NumberOfConstants == numberOfConstants, testInfo + "i");
            Assert.IsTrue(mathCompiler.NumberOfVectorConstants == numberOfVectorConstants, testInfo + "j");
            Assert.IsTrue(mathCompiler.NumberOfCalculatedConstants == numberOfCalculatedConstants, testInfo + "k");
            Assert.IsTrue(mathCompiler.NumberOfSubtermConstants == numberOfSubtermConstants, testInfo + "l");
            Assert.IsTrue(mathCompiler.NumberOfVectorConstants == numberOfVectorConstants, testInfo + "m");
        }
        /// <summary>
        /// Returns if values are equal in the range of roundoff errors
        /// </summary>
        /// <param name="value1">
        /// Value 1
        /// </param>
        /// <param name="value2">
        /// Value 2
        /// </param>
        /// <returns>
        /// True: Values are equal, false: Otherwise
        /// </returns>
        private Boolean AreNumericalValuesEqual(Double value1, Double value2)
        {
            if (value1 == 0.0 && value2 == 0.0)
            {
                return true;
            }
            return Math.Abs(value1 - value2) < Math.Abs(value1) * 1E-14;
        }

        #endregion
    }
}