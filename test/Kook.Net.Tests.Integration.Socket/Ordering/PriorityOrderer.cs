using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kook;

public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
        IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        string assemblyName = typeof(TestPriorityAttribute).AssemblyQualifiedName!;
        SortedDictionary<int, List<TTestCase>> sortedMethods = new();
        foreach (TTestCase testCase in testCases)
        {
            int priority = testCase.TestMethod.Method
                .GetCustomAttributes(assemblyName)
                .FirstOrDefault()
                ?.GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority)) ?? 0;

            GetOrCreate(sortedMethods, priority).Add(testCase);
        }

        return sortedMethods.Keys
            .SelectMany(priority => sortedMethods[priority]
                .OrderBy(testCase => testCase.TestMethod.Method.Name));
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
