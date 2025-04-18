using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace JustPlatform.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Получение значения ключа если он существует в словаре. Если не существует то добавляем новое.
    /// </summary>
    /// <param name="dictionary">Искомый словарь.</param>
    /// <param name="key">Ключ для поиска в словаре.</param>
    /// <param name="value">Значение для добавления.</param>
    /// <typeparam name="TKey">Тип ключа в словаре.</typeparam>
    /// <typeparam name="TValue">Тип значения в словаре.</typeparam>
    /// <returns>Значение в словаре.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ref var slot = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);
        if (exists) return slot!;

        slot = value;
        return value;
    }

    /// <summary>
    /// Попытка обновить значение ключа в словаре.
    /// </summary>
    /// <param name="dictionary">Искомый словарь.</param>
    /// <param name="key">Ключ для поиска в словаре.</param>
    /// <param name="value">Значение для обновления.</param>
    /// <typeparam name="TKey">Тип ключа в словаре.</typeparam>
    /// <typeparam name="TValue">Тип значения в словаре.</typeparam>
    /// <returns>true - если обновление прошло успешно. false - если ключ не найден.</returns>
    public static bool TryUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ref var slot = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        if (Unsafe.IsNullRef(ref slot)) return false;

        slot = value;
        return true;
    }

    /// <summary>
    /// Удаляет ключ из словаря, если он доступен.
    /// </summary>
    /// <param name="dictionary">Искомый словарь.</param>
    /// <param name="key">Ключ для поиска в словаре.</param>
    /// <typeparam name="TKey">Тип ключа в словаре.</typeparam>
    /// <typeparam name="TValue">Тип значения в словаре.</typeparam>
    /// <returns>true - если ключ был удален. false - если ключ не был найден</returns>
    public static bool RemoveIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        // Получаем ссылку на значение по ключу или null, если ключа нет
        ref var slot = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);

        // Если ссылка не является null, значит ключ существует
        if (Unsafe.IsNullRef(ref slot)) return false; // Ключ не существует, ничего не делаем

        dictionary.Remove(key); // Удаляем ключ
        return true; // Возвращаем true, так как ключ был удален
    }

    /// <summary>
    /// Обновляет значение для существующего ключа или добавляет новую связку ключ-значение.
    /// </summary>
    /// <param name="dictionary">Искомый словарь.</param>
    /// <param name="key">Ключ для поиска в словаре.</param>
    /// <param name="value">Значение для добавления / обновления.</param>
    /// <typeparam name="TKey">Тип ключа в словаре.</typeparam>
    /// <typeparam name="TValue">Тип значения в словаре.</typeparam>
    public static void Upsert<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        // Получаем ссылку на значение по ключу или default(TValue), если ключа нет
        ref var slot = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out _);

        // Если ключ существует, обновляем значение
        // Если же не существует, то добавляем значение
        slot = value;
    }
}