// ------------------------------------------------------------------------------------------------
//  <copyright file="Program.cs" company="Starion Group S.A.">
//
//    Copyright 2026 Starion Group S.A.
//    SPDX-License-Identifier: Apache-2.0
//
//  </copyright>
//  ------------------------------------------------------------------------------------------------

namespace Mycelium.SDK.Serializer.Json.Benchmarks
{
    using System.Threading.Tasks;

    using BenchmarkDotNet.Running;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides the benchmark executable entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Runs the benchmark suite or the separate peak-process-memory measurement.
        /// </summary>
        /// <param name="args">
        /// The command-line arguments.
        /// </param>
        /// <returns>
        /// A task representing the executable operation.
        /// </returns>
        public static async Task Main(string[] args)
        {
            if (PeakMemoryMeasurement.IsRequested(args))
            {
                using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var logger = loggerFactory.CreateLogger(nameof(PeakMemoryMeasurement));

                await PeakMemoryMeasurement.RunAsync(args, logger);
                return;
            }

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                .Run(args);
        }
    }
}
