// ------------------------------------------------------------------------------------------------
//  <copyright file="IDao.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Mycelium.SDK.DTO;

    using Npgsql;

    /// <summary>
    /// Defines the create, read, update and delete operations that a data access object performs for one concrete DTO type
    /// against the PostgreSQL schema delivered in <c>0001_initial.sql</c>.
    /// </summary>
    /// <typeparam name="TDto">
    /// The DTO interface of the concrete class whose objects the data access object stores.
    /// </typeparam>
    /// <remarks>
    /// Every operation runs on the connection of the supplied transaction. A data access object never opens, commits or rolls back
    /// a connection or transaction; the caller owns both.
    /// </remarks>
    public interface IDao<TDto> where TDto : class, IThing
    {
        /// <summary>
        /// Reads every stored object of the data access object's exact concrete class, ordered by identifier.
        /// </summary>
        /// <param name="transaction">
        /// The caller's open transaction, whose connection executes the query.
        /// </param>
        /// <param name="cancellationToken">
        /// The token that cancels the operation.
        /// </param>
        /// <returns>
        /// The stored objects, or an empty list when none exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="transaction" /> is <see langword="null" />.
        /// </exception>
        Task<IReadOnlyList<TDto>> ReadAllAsync(NpgsqlTransaction transaction, CancellationToken cancellationToken);

        /// <summary>
        /// Reads the stored object of the data access object's exact concrete class that has the specified identifier.
        /// </summary>
        /// <param name="transaction">
        /// The caller's open transaction, whose connection executes the query.
        /// </param>
        /// <param name="id">
        /// The identifier of the object to read.
        /// </param>
        /// <param name="cancellationToken">
        /// The token that cancels the operation.
        /// </param>
        /// <returns>
        /// The stored object, or <see langword="null" /> when no object of this concrete class has <paramref name="id" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="transaction" /> is <see langword="null" />.
        /// </exception>
        Task<TDto> ReadByIdAsync(NpgsqlTransaction transaction, Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Stores a new object: its serialized JSON document, one row per class in its inheritance chain with the scalar
        /// reference columns declared there, and the link-table rows of its stored collections.
        /// </summary>
        /// <param name="transaction">
        /// The caller's open transaction, whose connection executes the statements.
        /// </param>
        /// <param name="dto">
        /// The object to store, identified by its application-supplied <see cref="IThing.Id" />.
        /// </param>
        /// <param name="cancellationToken">
        /// The token that cancels the operation.
        /// </param>
        /// <returns>
        /// A task that completes when every statement has been executed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="transaction" /> or <paramref name="dto" /> is <see langword="null" />.
        /// </exception>
        Task CreateAsync(NpgsqlTransaction transaction, TDto dto, CancellationToken cancellationToken);

        /// <summary>
        /// Replaces a stored object's JSON document, scalar reference columns and link-table rows with the values of
        /// <paramref name="dto" />.
        /// </summary>
        /// <param name="transaction">
        /// The caller's open transaction, whose connection executes the statements.
        /// </param>
        /// <param name="dto">
        /// The new state of the object, identified by its <see cref="IThing.Id" />.
        /// </param>
        /// <param name="cancellationToken">
        /// The token that cancels the operation.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the object was updated; <see langword="false" /> when no object of this concrete class
        /// has the identifier, in which case nothing is written.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="transaction" /> or <paramref name="dto" /> is <see langword="null" />.
        /// </exception>
        Task<bool> UpdateAsync(NpgsqlTransaction transaction, TDto dto, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a stored object; the database's cascades and triggers remove its inheritance rows, link rows and contained
        /// objects.
        /// </summary>
        /// <param name="transaction">
        /// The caller's open transaction, whose connection executes the statement.
        /// </param>
        /// <param name="id">
        /// The identifier of the object to delete.
        /// </param>
        /// <param name="cancellationToken">
        /// The token that cancels the operation.
        /// </param>
        /// <returns>
        /// <see langword="true" /> when the object was deleted; <see langword="false" /> when no object of this concrete class
        /// has <paramref name="id" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="transaction" /> is <see langword="null" />.
        /// </exception>
        Task<bool> DeleteAsync(NpgsqlTransaction transaction, Guid id, CancellationToken cancellationToken);
    }
}
