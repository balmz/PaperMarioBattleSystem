﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem.Utilities
{
    /// <summary>
    /// Class for global utility functions
    /// </summary>
    public static class UtilityGlobals
    {
        public static readonly double TwoPI = (Math.PI * 2d);
        public static readonly double HalfPI = (Math.PI / 2d);

        public static int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;
        public static float Clamp(float value, float min, float max) => (value < min) ? min : (value > max) ? max : value;
        public static double Clamp(double value, double min, double max) => (value < min) ? min : (value > max) ? max : value;
        public static uint Clamp(uint value, uint min, uint max) => (value < min) ? min : (value > max) ? max : value;

        public static int Wrap(int value, int min, int max) => (value < min) ? max : (value > max) ? min : value;
        public static float Wrap(float value, float min, float max) => (value < min) ? max : (value > max) ? min : value;
        public static double Wrap(double value, double min, double max) => (value < min) ? max : (value > max) ? min : value;

        public static T Min<T>(T val1, T val2) where T : IComparable => (val1.CompareTo(val2) < 0) ? val1 : (val2.CompareTo(val1) < 0) ? val2 : val1;
        public static T Max<T>(T val1, T val2) where T : IComparable => (val1.CompareTo(val2) > 0) ? val1 : (val2.CompareTo(val1) > 0) ? val2 : val1;

        public static float ToDegrees(float radians) => Microsoft.Xna.Framework.MathHelper.ToDegrees(radians);
        public static float ToRadians(float degrees) => Microsoft.Xna.Framework.MathHelper.ToRadians(degrees);

        public static double ToDegrees(double radians) => (radians * (180d / Math.PI));
        public static double ToRadians(double degrees) => (degrees * (Math.PI / 180d));

        public static int Lerp(int value1, int value2, float amount) => value1 + (int)((value2 - value1) * amount);
        public static float Lerp(float value1, float value2, float amount) => value1 + ((value2 - value1) * amount);
        public static double Lerp(double value1, double value2, float amount) => value1 + ((value2 - value1) * amount);

        public static double LerpPrecise(double value1, double value2, float amount) => ((1 - amount) * value1) + (value2 * amount);
        public static float LerpPrecise(float value1, float value2, float amount) => ((1 - amount) * value1) + (value2 * amount);
        public static int LerpPrecise(int value1, int value2, float amount) => (int)(((1 - amount) * value1) + (value2 * amount));

        /// <summary>
        /// Bounces a value between 0 and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A double with a value between 0 and <paramref name="maxVal"/>.</returns>
        public static double PingPong(double time, double maxVal)
        {
            double lengthTimesTwo = maxVal * 2d;
            double timeMod = time % lengthTimesTwo;

            if (timeMod >= 0 && timeMod < maxVal)
                return timeMod;
            else
                return lengthTimesTwo - timeMod;
        }

        /// <summary>
        /// Bounces a value between a min and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="minVal">The min value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A float with a value between <paramref name="minVal"/> and <paramref name="maxVal"/>.</returns>
        public static double PingPong(double time, double minVal, double maxVal)
        {
            return PingPong(time, maxVal - minVal) + minVal;
        }

        /// <summary>
        /// Bounces a value between 0 and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A float with a value between 0 and <paramref name="maxVal"/>.</returns>
        public static float PingPong(double time, float maxVal) => (float)PingPong(time, (double)maxVal);

        /// <summary>
        /// Bounces a value between a min and a max value.
        /// </summary>
        /// <param name="time">The time value.</param>
        /// <param name="minVal">The min value.</param>
        /// <param name="maxVal">The max value.</param>
        /// <returns>A float with a value between <paramref name="minVal"/> and <paramref name="maxVal"/>.</returns>
        public static float PingPong(double time, float minVal, float maxVal) => (float)PingPong(time, (double)minVal, (double)maxVal);

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <remarks>This is a more verbose but readable version of MonoGame's Hermite.
        /// It returns a double rather than a float.</remarks>
        /// <param name="value1">The startpoint of the curve.</param>
        /// <param name="tangent1">Initial tangent; the direction and speed to how the curve leaves the startpoint.</param>
        /// <param name="value2">The endpoint of the curve.</param>
        /// <param name="tangent2">End tangent; the direction and speed to how the curve leaves the endpoint.</param>
        /// <param name="amount">Weighting factor; between 0 and 1.</param>
        /// <returns>A double representing the result of the Hermite spline interpolation.</returns>
        public static double Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            /*Hermite basis functions:
             * s = amount (or time)
             * h1 = 2s^3 - 3s^2 + 1
             * h2 = -2s^3 + 3s^2
             * h3 = s^3 - 2s^2 + s
             * h4 = s^3 - s^2
             * 
             * The values are multiplied by the basis functions and added together like so:
             * result = (h1 * val1) + (h2 * val2) + (h3 * tan1) + (h4 * tan2);
            */

            double val1 = value1;
            double val2 = value2;
            double tan1 = tangent1;
            double tan2 = tangent2;
            double amt = amount;
            double result = 0d;

            //Define cube and squares
            double amtCubed = amt * amt * amt;
            double amtSquared = amt * amt;

            //If 0, return the initial value
            if (amount == 0f)
            {
                result = value1;
            }
            //If 1, return the 
            else if (amount == 1f)
            {
                result = value2;
            }
            else
            {
                //Define hermite functions
                //double h1 = (2 * amtCubed) - (3 * amtSquared) + 1;
                //double h2 = (-2 * amtCubed) + (3 * amtSquared);
                //double h3 = amtCubed - (2 * amtSquared) + amt;
                //double h4 = amtCubed - amtSquared;

                //Multiply the results
                //result = (h1 * val1) + (h2 * val2) + (h3 * tan1) + (h4 * tan2);

                //Condensed
                result =
                    (((2 * val1) - (2 * val2) + tan2 + tan1) * amtCubed) +
                    (((3 * val2) - (3 * val1) - (2 * tan1) - tan2) * amtSquared) +
                    (tan1 * amt) + val1;
            }

            return result;
        }

        /// <summary>
        /// Interpolates a value by weighted average.
        /// The closer the value gets to the target, the slower it moves.
        /// </summary>
        /// <param name="curVal">The current value.</param>
        /// <param name="targetVal">The target value.</param>
        /// <param name="slowdownFactor">The slowdown factor. The higher this is, the slower <paramref name="curVal"/> will approach <paramref name="targetVal"/>.</param>
        /// <returns>A double representing the weighted average interpolation.</returns>
        public static double WeightedAverageInterpolation(double curVal, double targetVal, double slowdownFactor)
        {
            //Avoid division by 0
            if (slowdownFactor == 0)
            {
                return targetVal;
            }

            return ((curVal * (slowdownFactor - 1)) + targetVal) / slowdownFactor;
        }

        /// <summary>
        /// Swaps two references of the same Type.
        /// </summary>
        /// <typeparam name="T">The Type of the objects to swap.</typeparam>
        /// <param name="obj1">The first object to swap.</param>
        /// <param name="obj2">The second object to swap.</param>
        public static void Swap<T>(ref T obj1, ref T obj2)
        {
            T temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }

        /// <summary>
        /// Gets the tangent angle between two Vector2s in radians. This value is between -π and π. 
        /// </summary>
        /// <param name="vec1">The first vector2.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the tangent angle between the two vectors, in radians.</returns>
        public static double TangentAngle(Vector2 vec1, Vector2 vec2) => Math.Atan2(vec2.Y - vec1.Y, vec2.X - vec1.X);

        /// <summary>
        /// Gets the cosign angle between two Vector2s in radians.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the cosign angle between the two vectors, in radians.</returns>
        public static double CosignAngle(Vector2 vec1, Vector2 vec2)
        {
            //a · b = (a.X * b.X) + (a.Y * b.Y) = ||a|| * ||b|| * cos(θ)
            double dotProduct = Vector2.Dot(vec1, vec2);
            double mag1 = vec1.Length();
            double mag2 = vec2.Length();

            double magMult = mag1 * mag2;

            double div = dotProduct / magMult;

            double angleRadians = Math.Acos(div);
            
            return angleRadians;
        }

        /// <summary>
        /// Gets the cosign angle between two Vector2s in degrees.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the cosign angle between the two vectors, in degrees.</returns>
        public static double CosignAngleDegrees(Vector2 vec1, Vector2 vec2) => ToDegrees(CosignAngle(vec1, vec2));

        /// <summary>
        /// Obtains the 2D cross product result of two Vector2s.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>A float representing the 2D cross product result between the two Vectors.</returns>
        public static double Cross(Vector2 vector1, Vector2 vector2)
        {
            //a x b = ((a.y * b.z) - (a.z * b.y), (a.z * b.x) - (a.x * b.z), (a.x * b.y) - (a.y * b.x))
            //The Z component is the only one that remains since we're dealing with Vector2s
            return (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
        }

        /// <summary>
        /// Gets the sine angle between two Vector2s in radians.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the sine angle between the two vectors, in radians.</returns>
        public static double SineAngle(Vector2 vec1, Vector2 vec2)
        {
            //||a x b|| = ||a|| * ||b|| * sin(θ)
            double crossMag = Cross(vec1, vec2);

            double mag1 = vec1.Length();
            double mag2 = vec2.Length();

            double magMult = mag1 * mag2;

            double div = crossMag / magMult;

            double angleRadians = Math.Asin(div);

            return angleRadians;
        }

        /// <summary>
        /// Finds a point around a circle at a particular angle.
        /// </summary>
        /// <param name="circle">The Circle.</param>
        /// <param name="angle">The angle of the point.</param>
        /// <param name="angleInDegrees">Whether the angle passed in is in degrees or not.</param>
        /// <returns>A Vector2 with the X and Y components at the location around the circle.</returns>
        public static Vector2 GetPointAroundCircle(Circle circle, double angle, bool angleInDegrees)
        {
            //If the angle is in degrees, convert it to radians
            if (angleInDegrees == true)
            {
                angle = ToRadians(angle);
            }

            return circle.GetPointAround(angle);
        }

        /// <summary>
        /// Tests a random condition with two values.
        /// This is commonly used when calculating a total percentage of something happening.
        /// For example, this is used when testing whether a move will inflict a Status Effect on a BattleEntity.
        /// <para>Two values are multiplied by each other then divided by <see cref="RandomGlobals.RandomConditionVal"/>.
        /// A random value is then rolled; if it's less than the result, it returns true. This works for any non-negative values.</para>
        /// </summary>
        /// <param name="value1">The first value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <param name="value2">The second value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(double value1, double value2)
        {
            double value = (RandomGlobals.Randomizer.NextDouble() * RandomGlobals.RandomConditionVal);

            double percentageResult = ((value1 * value2) / (double)RandomGlobals.RandomConditionVal);

            return (value < percentageResult);
        }

        /// <summary>
        /// Tests a random condition with one value.
        /// </summary>
        /// <param name="value">The value to test, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(double value)
        {
            return TestRandomCondition(value, (double)RandomGlobals.RandomConditionVal);
        }

        /// <summary>
        /// Tests a random condition with two values. An int overload.
        /// This is commonly used when calculating a total percentage of something happening.
        /// For example, this is used when testing whether a move will inflict a Status Effect on a BattleEntity.
        /// <para>Two values are multiplied by each other then divided by <see cref="RandomGlobals.RandomConditionVal"/>.
        /// A random value is then rolled; if it's less than the result, it returns true. This works for any non-negative values.</para>
        /// </summary>
        /// <param name="value1">The first value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <param name="value2">The second value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(int value1, int value2)
        {
            int value = RandomGlobals.Randomizer.Next(RandomGlobals.RandomConditionVal);

            int percentageResult = ((value1 * value2) / RandomGlobals.RandomConditionVal);

            return (value < percentageResult);
        }

        /// <summary>
        /// Tests a random condition with one value. An int overload.
        /// </summary>
        /// <param name="value">The value to test, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(int value)
        {
            return TestRandomCondition(value, RandomGlobals.RandomConditionVal);
        }

        /// <summary>
        /// Chooses a random index in a list of percentages
        /// </summary>
        /// <param name="percentages">The container of percentages, each with positive values, with the sum adding up to 1</param>
        /// <returns>The index in the container of percentages that was chosen</returns>
        public static int ChoosePercentage(IList<double> percentages)
        {
            double randomVal = RandomGlobals.Randomizer.NextDouble();
            double value = 0d;

            for (int i = 0; i < percentages.Count; i++)
            {
                value += percentages[i];
                if (value > randomVal)
                {
                    return i;
                }
            }

            //Return the last one if it goes through
            return percentages.Count - 1;
        }

        /// <summary>
        /// Indicates whether an <see cref="IList{T}"/> is null or empty.
        /// </summary>
        /// <typeparam name="T">The Type of the elements in the IList.</typeparam>
        /// <param name="iList">The IList.</param>
        /// <returns>true if <paramref name="iList"/> is null or empty, otherwise false.</returns>
        public static bool IListIsNullOrEmpty<T>(IList<T> iList)
        {
            return (iList == null || iList.Count == 0);
        }

        #region Flag Check Utilities

        /* Adding flags: flag1 |= flag2            ; 10 | 01 = 11
         * Checking flags: (flag1 & flag2) != 0    ; 11 & 10 = 10
         * Removing flags: (flag1 & (~flag2))      ; 1111 & (~0010) = 1111 & 1101 = 1101
         * */

        /// <summary>
        /// Tells whether a set of DamageEffects has any of the flags in another DamageEffects set.
        /// </summary>
        /// <param name="damageEffects">The DamageEffects value.</param>
        /// <param name="damageEffectFlags">The flags to test.</param>
        /// <returns>true if any of the flags in damageEffectFlags are in damageEffects, otherwise false.</returns>
        public static bool DamageEffectHasFlag(Enumerations.DamageEffects damageEffects, Enumerations.DamageEffects damageEffectFlags)
        {
            Enumerations.DamageEffects flags = (damageEffects & damageEffectFlags);

            return (flags != 0);
        }

        /// <summary>
        /// Tells whether a set of DefensiveActionTypes has any of the flags in another DefensiveActionTypes set.
        /// </summary>
        /// <param name="defensiveOverrides">The DefensiveActionTypes value.</param>
        /// <param name="defensiveOverrideFlags">The flags to test.</param>
        /// <returns>true if any of the flags in defensiveOverrides are in defensiveOverrideFlags, otherwise false.</returns>
        public static bool DefensiveActionTypesHasFlag(Enumerations.DefensiveActionTypes defensiveOverrides,
            Enumerations.DefensiveActionTypes defensiveOverrideFlags)
        {
            Enumerations.DefensiveActionTypes flags = (defensiveOverrides & defensiveOverrideFlags);

            return (flags != 0);
        }

        /// <summary>
        /// Tells whether a set of MoveAffectionTypes has any of the flags in another MoveAffectionTypes set.
        /// </summary>
        /// <param name="moveAffectionTypes">The MoveAffectionTypes value.</param>
        /// <param name="moveAffectionTypesFlags">The flags to test.</param>
        /// <returns>true if any of the flags in moveAffectionTypes are in moveAffectionTypesFlags, otherwise false.</returns>
        public static bool MoveAffectionTypesHasFlag(Enumerations.MoveAffectionTypes moveAffectionTypes,
            Enumerations.MoveAffectionTypes moveAffectionTypesFlags)
        {
            Enumerations.MoveAffectionTypes flags = (moveAffectionTypes & moveAffectionTypesFlags);

            return (flags != 0);
        }

        /// <summary>
        /// Tells whether a set of ItemTypes has any of the flags in another ItemTypes set.
        /// </summary>
        /// <param name="itemTypes">The ItemTypes value.</param>
        /// <param name="itemTypesFlags">The flags to test.</param>
        /// <returns>true if any of the flags in itemTypes are in itemTypesFlags, otherwise false.</returns>
        public static bool ItemTypesHasFlag(Item.ItemTypes itemTypes, Item.ItemTypes itemTypesFlags)
        {
            Item.ItemTypes flags = (itemTypes & itemTypesFlags);

            return (flags != 0);
        }

        #endregion

        /// <summary>
        /// Initializes a jagged array with default values.
        /// <para>This should be used on null jagged arrays to easily initialize them.</para>
        /// </summary>
        /// <typeparam name="T">The type of the jagged array.</typeparam>
        /// <param name="jaggedArray">The jagged array of type T to initialize.</param>
        /// <param name="columns">The number of columns (first bracket) in the jagged array.</param>
        /// <param name="rows">The number of rows (second bracket) in the jagged array.</param>
        public static void InitializeJaggedArray<T>(ref T[][] jaggedArray, int columns, int rows)
        {
            jaggedArray = new T[columns][];
            for (int i = 0; i < jaggedArray.Length; i++)
            {
                jaggedArray[i] = new T[rows];
            }
        }

        /// <summary>
        /// Clears a jagged array by nulling out its outer arrays.
        /// </summary>
        /// <typeparam name="T">The type of the jagged array.</typeparam>
        /// <param name="jaggedArray">The jagged array of type T to clear.</param>
        public static void ClearJaggedArray<T>(ref T[][] jaggedArray)
        {
            if (jaggedArray != null)
            {
                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    jaggedArray[i] = null;
                }
            }
        }

        /// <summary>
        /// Subtracts a float from another float and divides the result by a dividing factor.
        /// </summary>
        /// <param name="value1">The float value which has its value subtracted by <paramref name="value2"/>.</param>
        /// <param name="value2">The float value used in the subtraction.</param>
        /// <returns>The difference between <paramref name="value2"/> and <paramref name="value1"/> divided by the
        /// <paramref name="dividingFactor"/>.
        /// If <paramref name="dividingFactor"/> is 0, then 0.
        /// </returns>
        public static float DifferenceDivided(float value1, float value2, float dividingFactor)
        {
            //Return 0 if we're trying to divide by 0
            if (dividingFactor == 0f)
            {
                return 0f;
            }

            //Return the difference over the division
            float diff = (value1 - value2);
            return diff / dividingFactor;
        }

        #region Collision Utilities

        /// <summary>
        /// Checks if a collision object collides with any other object in a set of collision objects.
        /// </summary>
        /// <typeparam name="T">A type that implements <see cref="ICollisionHandler"/>.</typeparam>
        /// <param name="collisionObj">The collision object to check collisions with.</param>
        /// <param name="collisionObjects">The objects to check collisions with.</param>
        /// <returns>A <see cref="CollisionResponseHolder"/> with the collision data if a collision was found, otherwise null.</returns>
        public static CollisionResponseHolder? GetCollisionForSet<T>(ICollisionHandler collisionObj, IList<T> collisionObjects) where T : ICollisionHandler
        {
            //Collision can't be resolved with null references, so return
            if (collisionObj == null || collisionObjects == null) return null;

            //Check collisions with objects
            for (int i = 0; i < collisionObjects.Count; i++)
            {
                //We found a collision, so return the response
                if (collisionObj.collisionShape.CollidesWith(collisionObjects[i].collisionShape) == true)
                {
                    return collisionObjects[i].GetCollisionResponse(collisionObj);
                }
            }

            return null;
        }

        #endregion
    }
}
