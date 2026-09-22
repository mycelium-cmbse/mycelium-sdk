// ------------------------------------------------------------------------------------------------
//  <copyright file="DataFormatterResolver.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.MessagePack.Helpers
{
    using global::MessagePack;
    using global::MessagePack.Formatters;

    /// <summary>
    /// Resolves cached MessagePack formatters for exact generated DTO types.
    /// </summary>
    public sealed class DataFormatterResolver : IFormatterResolver
    {
        /// <summary>
        /// The shared formatter resolver instance.
        /// </summary>
        public static readonly IFormatterResolver Instance = new DataFormatterResolver();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormatterResolver" /> class.
        /// </summary>
        private DataFormatterResolver()
        {
        }

        /// <summary>
        /// Gets the cached MessagePack formatter registered for the exact requested type.
        /// </summary>
        /// <typeparam name="T">
        /// The exact type to serialize or deserialize.
        /// </typeparam>
        /// <returns>
        /// The cached formatter registered for <typeparamref name="T" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when no formatter is registered for the exact requested type.
        /// </exception>
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            var formatter = FormatterCache<T>.Formatter;

            if (formatter == null)
            {
                throw new NotSupportedException($"No MessagePack formatter is registered for exact type '{typeof(T).FullName}'.");
            }

            return formatter;
        }

        /// <summary>
        /// Caches the formatter lookup result for one exact requested type.
        /// </summary>
        /// <typeparam name="T">
        /// The exact type for which the formatter is cached.
        /// </typeparam>
        private static class FormatterCache<T>
        {
            /// <summary>
            /// The cached formatter, or <see langword="null" /> when the exact type is unsupported.
            /// </summary>
            public static readonly IMessagePackFormatter<T>? Formatter = (IMessagePackFormatter<T>?)DataResolverGetFormatterHelper.GetFormatter(typeof(T));
        }
    }
}
