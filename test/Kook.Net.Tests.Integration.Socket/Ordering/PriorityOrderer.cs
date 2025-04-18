using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace Kook;

public class PriorityOrderer : ITestCaseOrderer
{
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        SortedDictionary<int, List<TTestCase>> sortedMethods = new();
        foreach (TTestCase testCase in testCases)
        {
            int priority = (testCase.TestMethod as XunitTestMethod)?
                .Method?
                .GetCustomAttribute<TestPriorityAttribute>()?.Priority
                ?? 0;

            GetOrCreate(sortedMethods, priority).Add(testCase);
        }

        return sortedMethods.Keys
            .SelectMany(priority => sortedMethods[priority]
                .OrderBy(testCase => testCase.TestMethod?.MethodName))
            .ToArray();
    }

    private static TValue GetOrCreate<TKey, TValue>(
        SortedDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : struct
        where TValue : new()
    {
        if (dictionary.TryGetValue(key, out TValue? result))
            return result;
        return dictionary[key] = new TValue();
    }
}
