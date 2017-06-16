using System;
using System.Globalization;
using System.Net;

//Based on the project in https://github.com/samus/mongodb-csharp under the Apache-2.0 license

namespace NAME.MongoDb
{
    /// <summary>
    /// Represents a mongodb server with host and port.
    /// </summary>
    internal sealed class MongoServerEndPoint : EndPoint
    {
        /// <summary>
        /// The mongo default host name.
        /// </summary>
        public const string DefaultHost = "localhost";
        
        /// <summary>
        /// The mongo default server port.
        /// </summary>
        public const int DefaultPort = 27017;

        /// <summary>
        /// The default MongoServerEndPoint.
        /// </summary>
        public static readonly MongoServerEndPoint Default = new MongoServerEndPoint();

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoServerEndPoint"/> class.
        /// </summary>
        public MongoServerEndPoint()
            : this(DefaultHost, DefaultPort)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoServerEndPoint"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public MongoServerEndPoint(string host)
            : this(host, DefaultPort)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoServerEndPoint"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public MongoServerEndPoint(int port)
            : this(DefaultHost, port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoServerEndPoint"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public MongoServerEndPoint(string host, int port)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            this.Host = host;
            this.Port = port;
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; private set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; private set; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}:{1}", this.Host, this.Port);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(MongoServerEndPoint other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other.Host, this.Host) && other.Port == this.Port;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == typeof(MongoServerEndPoint) && this.Equals((MongoServerEndPoint)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Host != null ? this.Host.GetHashCode() : 0) * 397) ^ this.Port;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MongoServerEndPoint left, MongoServerEndPoint right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(MongoServerEndPoint left, MongoServerEndPoint right)
        {
            return !Equals(left, right);
        }
    }
}