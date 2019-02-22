using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class BenchmarkGetAllAssetPaths {
        [PerformanceTest]
        public void GetAllAssetPaths_Performance () {
            Measure.Method (() => AssetDatabase.GetAllAssetPaths ())
                .WarmupCount (10)
                .IterationsPerMeasurement (10)
                .Run ();
        }

        [PerformanceTest]
        public void NewList_Performance () {
            Measure.Method (() => { var s = new string[500]; })
                .WarmupCount (10)
                .IterationsPerMeasurement (10)
                .Run ();
        }

        [PerformanceTest]
        public void LoadMainAsset_Performance () {
            var assetPath = AssetDatabase.GetAllAssetPaths () [0];
            Measure.Method (() => {
                    AssetDatabase.LoadMainAssetAtPath (assetPath);
                })
                .WarmupCount (10)
                .IterationsPerMeasurement (10)
                .Run ();
        }
    }
}