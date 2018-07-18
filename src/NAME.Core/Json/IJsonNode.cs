using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.Json
{
    /// <summary>
    /// Provides a mechanism to acess json elements.
    /// </summary>
    public interface IJsonNode
    {
        /// <summary>
        /// Gets a child json element using the index.
        /// </summary>
        /// <param name="aIndex">The index.</param>
        IJsonNode this[int aIndex]
        {
            get;
        }

        /// <summary>
        /// Gets a child json element using the key.
        /// </summary>
        /// <param name="aKey">The key.</param>
        IJsonNode this[string aKey]
        {
            get;
        }

        /// <summary>
        /// Gets the element value as string.
        /// </summary>
        string Value
        {
            get;
        }
        
        /// <summary>
        /// Gets the element tag.
        /// </summary>
        JsonBinaryTag Tag
        {
            get;
        }

        /// <summary>
        /// Gets the element value as int.
        /// </summary>
        int AsInt
        {
            get;
        }

        /// <summary>
        /// Gets the element value as long.
        /// </summary>
        long AsLong
        {
            get;
        }

        /// <summary>
        /// Gets the element value as float.
        /// </summary>
        float AsFloat
        {
            get;
        }

        /// <summary>
        /// Gets the element value as double.
        /// </summary>
        double AsDouble
        {
            get;
        }

#pragma warning disable SA1623 // Property summary documentation must match accessors
                              /// <summary>
                              /// Gets the element value as bool.
                              /// </summary>
        bool AsBool
#pragma warning restore SA1623 // Property summary documentation must match accessors
        {
            get;
        }
    }
}
