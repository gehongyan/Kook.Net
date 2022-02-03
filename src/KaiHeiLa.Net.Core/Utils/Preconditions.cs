using System;

namespace KaiHeiLa;

public class Preconditions
{
        #region Objects
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <see langword="null"/>.</exception>
        public static void NotNull<T>(T obj, string name, string msg = null) where T : class { if (obj == null) throw CreateNotNullException(name, msg); }
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <see langword="null"/>.</exception>
        public static void NotNull<T>(Optional<T> obj, string name, string msg = null) where T : class { if (obj.IsSpecified && obj.Value == null) throw CreateNotNullException(name, msg); }

        private static ArgumentNullException CreateNotNullException(string name, string msg)
        {
            if (msg == null) return new ArgumentNullException(paramName: name);
            else return new ArgumentNullException(paramName: name, message: msg);
        }
        #endregion

        #region Strings
        /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
        public static void NotEmpty(string obj, string name, string msg = null) { if (obj.Length == 0) throw CreateNotEmptyException(name, msg); }
        /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
        public static void NotEmpty(Optional<string> obj, string name, string msg = null) { if (obj.IsSpecified && obj.Value.Length == 0) throw CreateNotEmptyException(name, msg); }
        /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <see langword="null"/>.</exception>
        public static void NotNullOrEmpty(string obj, string name, string msg = null)
        {
            if (obj == null) throw CreateNotNullException(name, msg);
            if (obj.Length == 0) throw CreateNotEmptyException(name, msg);
        }
        /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <see langword="null"/>.</exception>
        public static void NotNullOrEmpty(Optional<string> obj, string name, string msg = null)
        {
            if (obj.IsSpecified)
            {
                if (obj.Value == null) throw CreateNotNullException(name, msg);
                if (obj.Value.Length == 0) throw CreateNotEmptyException(name, msg);
            }
        }
        /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <see langword="null"/>.</exception>
        public static void NotNullOrWhitespace(string obj, string name, string msg = null)
        {
            if (obj == null) throw CreateNotNullException(name, msg);
            if (obj.Trim().Length == 0) throw CreateNotEmptyException(name, msg);
        }
        /// <exception cref="ArgumentException"><paramref name="obj"/> cannot be blank.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <see langword="null"/>.</exception>
        public static void NotNullOrWhitespace(Optional<string> obj, string name, string msg = null)
        {
            if (obj.IsSpecified)
            {
                if (obj.Value == null) throw CreateNotNullException(name, msg);
                if (obj.Value.Trim().Length == 0) throw CreateNotEmptyException(name, msg);
            }
        }        

        private static ArgumentException CreateNotEmptyException(string name, string msg)
            => new ArgumentException(message: msg ?? "Argument cannot be blank.", paramName: name);
        #endregion
}