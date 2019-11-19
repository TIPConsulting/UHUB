using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHub.CoreLib.Entities.Posts.DataInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UHub.CoreLib.Tests;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Posts.DataInterop.Tests
{
    public partial class PostReaderTests
    {
        /*
            Synchronous:
            Iterations: 50
            Total Time: 3602.6452ms
            ItemCount: 11508

            Min: 68.463ms
            Max: 172.457ms
            Avg: 72.051536ms
            Median: 69.3937ms


            -------------------------------------------------------


            Synchronous (threaded):
            Iterations: 50
            Total Time: 1509.9419ms
            ItemCount: 11508

            Min: 95.0701ms
            Max: 1270.5817ms
            Avg: 312.75165ms
            Median: 245.60905ms


            -------------------------------------------------------


            Async:
            Iterations: 50
            Total Time: 2731.1464ms
            ItemCount: 11508

            Min: 608.9696ms
            Max: 2294.3514ms
            Avg: 1529.740062ms
            Median: 948.85225ms
        */

        //useful metric, but dont include in tests (too time consuming)
        //[TestMethod]
        public async Task GetPostStatTest()
        {
            TestGlobal.TestInit();
            var iterCount = 50;

            {
                var samples1 = new List<double>();
                var countSet = new List<int>(iterCount);
                var itmCount = 0;

                var start_outer = FailoverDateTimeOffset.UtcNow;


                for (int i = 0; i < iterCount; i++)
                {
                    var start = FailoverDateTimeOffset.UtcNow;

                    var set = SqlWorker.ExecEntityQuery<Post>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[Posts_GetAll]",
                    (cmd) => { })
                    .ToList();

                    var end = FailoverDateTimeOffset.UtcNow;


                    countSet.Add(set.Count);
                    itmCount = set.Count;
                    double sample = (end - start).TotalMilliseconds;
                    samples1.Add(sample);
                }

                var end_outer = FailoverDateTimeOffset.UtcNow;
                var totalTime = (end_outer - start_outer).TotalMilliseconds;


                Console.WriteLine($"Synchronous:");
                Console.WriteLine($"Iterations: {countSet.Count}");
                Console.WriteLine($"Total Time: {totalTime}ms");
                Console.WriteLine($"ItemCount: {itmCount}");
                Console.WriteLine();
                Console.WriteLine($"Min: {samples1.Min()}ms");
                Console.WriteLine($"Max: {samples1.Max()}ms");
                Console.WriteLine($"Avg: {samples1.Average()}ms");
                Console.WriteLine($"Median: {samples1.Median()}ms");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
            }
            {
                var samples1 = new List<double>();
                var countSet = new List<int>(iterCount);
                var itmCount = 0;

                var start_outer = FailoverDateTimeOffset.UtcNow;


                Parallel.For(0, iterCount, (x) =>
                {
                    var start = FailoverDateTimeOffset.UtcNow;

                    var set = SqlWorker.ExecEntityQuery<Post>(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[Posts_GetAll]",
                    (cmd) => { })
                    .ToList();

                    var end = FailoverDateTimeOffset.UtcNow;

                    countSet.Add(set.Count);
                    itmCount = set.Count;
                    double sample = (end - start).TotalMilliseconds;
                    samples1.Add(sample);
                });

                var end_outer = FailoverDateTimeOffset.UtcNow;
                var totalTime = (end_outer - start_outer).TotalMilliseconds;


                Console.WriteLine($"Synchronous (threaded):");
                Console.WriteLine($"Iterations: {countSet.Count}");
                Console.WriteLine($"Total Time: {totalTime}ms");
                Console.WriteLine($"ItemCount: {itmCount}");
                Console.WriteLine();
                Console.WriteLine($"Min: {samples1.Min()}ms");
                Console.WriteLine($"Max: {samples1.Max()}ms");
                Console.WriteLine($"Avg: {samples1.Average()}ms");
                Console.WriteLine($"Median: {samples1.Median()}ms");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
            }


            {
                var samples1 = new ConcurrentBag<double>();
                var countSet = new List<int>(iterCount);
                var itmCount = 0;


                async Task worker()
                {
                    var start = FailoverDateTimeOffset.UtcNow;

                    var set = await SqlWorker.ExecEntityQueryAsync<Post>(
                        CoreFactory.Singleton.Properties.CmsDBConfig,
                        "[dbo].[Posts_GetAll]",
                        (cmd) => { });



                    var end = FailoverDateTimeOffset.UtcNow;

                    countSet.Add(set.ToList().Count);
                    itmCount = set.Count();
                    double sample = (end - start).TotalMilliseconds;
                    samples1.Add(sample);
                }


                var taskSet = new List<Task>();
                var start_outer = FailoverDateTimeOffset.UtcNow;
                for (int i = 0; i < iterCount; i++)
                {
                    taskSet.Add(worker());
                }
                await Task.WhenAll(taskSet);


                var end_outer = FailoverDateTimeOffset.UtcNow;
                var totalTime = (end_outer - start_outer).TotalMilliseconds;


                Console.WriteLine($"Async:");
                Console.WriteLine($"Iterations: {countSet.Count}");
                Console.WriteLine($"Total Time: {totalTime}ms");
                Console.WriteLine($"ItemCount: {itmCount}");
                Console.WriteLine();
                Console.WriteLine($"Min: {samples1.Min()}ms");
                Console.WriteLine($"Max: {samples1.Max()}ms");
                Console.WriteLine($"Avg: {samples1.Average()}ms");
                Console.WriteLine($"Median: {samples1.Median()}ms");

            }

        }

    }
}