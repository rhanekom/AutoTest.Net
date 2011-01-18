﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Errors;

namespace AutoTest.TestRunners
{
    class TestRunner : MarshalByRefObject, ITestRunner
    {
        private List<TestResult> _results = new List<TestResult>();

        public IEnumerable<TestResult> Run(Plugin plugin, RunOptions options)
        {
            _results = new List<TestResult>();
            var runner = getRunner(plugin);
            if (runner == null)
                return _results;
            return runTests(options, runner);
        }

        private IEnumerable<TestResult> runTests(RunOptions options, IAutoTestNetTestRunner runner)
        {
            foreach (var run in options.TestRuns)
                if (runner.Handles(run.ID))
                    _results.AddRange(runner.Run(run));
            return _results;
        }

        private IAutoTestNetTestRunner getRunner(Plugin plugin)
        {
            try
            {
                return plugin.New();
            }
            catch (Exception ex)
            {
                _results.Add(ErrorHandler.GetError(plugin.Type, ex));
            }
            return null;
        }
    }
}
