using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class BenchmarkISearchableQuery {
        struct Searchable {
            public string str;
            public Searchable (string s) {
                str = s;
            }
        }

        private void TestMethod (Action<List<Searchable>, string> method) {
            List<Searchable> matching = new List<Searchable> ();
            string matchString = "test";
            Measure.Method (() => method (matching, matchString))
                .WarmupCount (10)
                .IterationsPerMeasurement (10)
                .Run ();
        }

        [PerformanceTest]
        public void LinearForEach () {
            TestMethod ((matching, matchString) => {
                matching.Clear ();
                var assetPaths = AssetDatabase.GetAllAssetPaths ();
                foreach (var x in assetPaths) {
                    // matching.Add (new Searchable.Asset (path));
                    if (x.IndexOf (matchString, StringComparison.OrdinalIgnoreCase) >= 0)
                        matching.Add (new Searchable (x));
                }
            });
        }

        [PerformanceTest]
        public void LinearLinqForEach () {
            TestMethod ((matching, matchString) => {
                matching.Clear ();
                var assetPaths = AssetDatabase.GetAllAssetPaths ()
                    .Where (x => x.IndexOf (matchString, StringComparison.OrdinalIgnoreCase) >= 0);
                foreach (var path in assetPaths) {
                    // matching.Add (new Searchable.Asset (path));
                    matching.Add (new Searchable (path));
                }
            });
        }

        [PerformanceTest]
        public void LinearListForEach () {
            TestMethod ((matching, matchString) => {
                matching.Clear ();
                AssetDatabase.GetAllAssetPaths ()
                    .Where (x => x.IndexOf (matchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList ()
                    .ForEach (x => matching.Add (new Searchable (x)));
            });
        }

        [PerformanceTest]
        public void SystemParallel () {
            TestMethod ((matching, matchString) => {
                matching.Clear ();
                Parallel.ForEach (AssetDatabase.GetAllAssetPaths (), (x) => {
                    if (x.IndexOf (matchString, StringComparison.OrdinalIgnoreCase) >= 0) {
                        matching.Add (new Searchable (x));
                    }
                });
            });
        }

        [PerformanceTest]
        public void LinqParallel () {
            TestMethod ((matching, matchString) => {
                matching.Clear ();
                AssetDatabase.GetAllAssetPaths ()
                    .AsParallel ()
                    .Where (x => x.IndexOf (matchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ForAll (x => matching.Add (new Searchable (x)));
            });
        }
    }
}