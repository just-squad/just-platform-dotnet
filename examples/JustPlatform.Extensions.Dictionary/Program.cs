using JustPlatform.Extensions;

var dictionary = new Dictionary<int, string>
{
    [1] = "2",
    [2] = "2",
    [3] = "2",
    [4] = "2"
};

dictionary.TryAdd(5, "3");

if (!dictionary.TryGetValue(5, out var value))
{
    dictionary.Add(5, "3");
}

var getOrAddValue5 = dictionary.GetOrAdd(5, "3");
var getOrAddValue1 = dictionary.GetOrAdd(1, "3");
Console.WriteLine(getOrAddValue5);
Console.WriteLine(getOrAddValue1);

var tryUpdate1 = dictionary.TryUpdate(1, "3");
Console.WriteLine($"{tryUpdate1} {dictionary[1]}");

var tryUpdate2 = dictionary.TryUpdate(6, "3");
Console.WriteLine($"{tryUpdate2}");
