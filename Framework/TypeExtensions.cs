/* Copyright 2017 Fairfield Tek, L.L.C.
* Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
* to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute,
* sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
* OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
* OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Outlook365.SpamAssassin.Framework
{
    /// <summary>
    ///     Extension methods for the Type class
    /// </summary>
    public static class TypeExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Loads the configuration from assembly attributes
        /// </summary>
        /// <typeparam name="T">
        /// The type of the custom attribute to find.
        /// </typeparam>
        /// <param name="typeWithAttributes">
        /// The calling assembly to search.
        /// </param>
        /// <returns>
        /// The custom attribute of type T, if found.
        /// </returns>
        public static T GetAttribute<T>(this Type typeWithAttributes) where T : Attribute
        {
            return GetAttributes<T>(typeWithAttributes).FirstOrDefault();
        }

        /// <summary>
        /// Loads the configuration from assembly attributes
        /// </summary>
        /// <typeparam name="T">
        /// The type of the custom attribute to find.
        /// </typeparam>
        /// <param name="typeWithAttributes">
        /// The calling assembly to search.
        /// </param>
        /// <returns>
        /// An enumeration of attributes of type T that were found.
        /// </returns>
        public static IEnumerable<T> GetAttributes<T>(this Type typeWithAttributes) where T : Attribute
        {
            // Try to find the configuration attribute for the default logger if it exists
            Attribute[] configAttributes = Attribute.GetCustomAttributes(typeWithAttributes, typeof(T), false);

            // get just the first one
            if (configAttributes.Length <= 0) yield break;
            foreach (Attribute attribute1 in configAttributes)
            {
                T attribute = (T) attribute1;
                yield return attribute;
            }
        }

        #endregion
    }
}