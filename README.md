# C# Utilities

A collection of utilities for C# and Unity. This package contains pure C# code only. Even though it is formatted as Unity package, it *should* be usable in any C# project. An additional package will be released *soon(tm)* which contains Unity-specific utilities.

## Installation

### Install via Git URL

1. Navigate to your project's Packages folder and open the `manifest.json` file.
2. Add this line:
	-	```json
		"com.kryzarel.c-sharp-utilities": "https://github.com/Kryzarel/c-sharp-utilities.git",
		```

### Install manually

1. Clone or download the [C# Utilities](https://github.com/Kryzarel/c-sharp-utilities) repository.
2. Copy/paste or move the whole repository folder directly into your project's Packages folder or into the Assets folder.

## Useful Utilities

### TypeExtensions

#### C# doesn't give you base class private members even if you use BindingFlags.NonPublic. These methods manually search the base classes to find ALL members. Additionally, they receive an `IList` provided by the user to fill with the results. The user can choose to reuse this list, to reduce memory allocations.
```csharp
public static void GetAllFields(this Type type, BindingFlags bindingFlags, IList<FieldInfo> fieldInfos);
public static void GetAllProperties(this Type type, BindingFlags bindingFlags, IList<PropertyInfo> propertyInfos);
public static void GetAllMethods(this Type type, BindingFlags bindingFlags, IList<MethodInfo> methodInfos)
```

#### Same as the above methods, but these will only return members that are decorated with the specified attribute:
```csharp
public static void GetAllFieldsWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, IList<FieldInfo> fieldInfos);
public static void GetAllPropertiesWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, IList<PropertyInfo> propertyInfos);
public static void GetAllMethodsWithAttribute(this Type type, BindingFlags bindingFlags, Type attributeType, IList<MethodInfo> methodInfos);
```

### ListExtensions

#### Checks if a list's current capacity is less than `capacity`, and increases the current capacity if so. The capacity increases to the highest value between `currentCapacity * 2` and `capacity`.
```csharp
public static void EnsureCapacity<T>(this List<T> list, int capacity);
```

The default `AddRange()` method only accepts `IEnumerable<T>`. This causes allocations when iterating over it in a `foreach` loop, due to boxing of the enumerator struct. These methods use a `for` loop and indexing implemented by `IList<T>` and `IReadOnlyList<T>`, avoiding iterator allocations. Since `IList<T>` and `IReadOnlyList<T>` also implement a `Count` property, they also call `EnsureCapacity<T>(toAdd.Count)` before adding to the list, potentially preventing multiple resizes.
```csharp
public static void AddRangeNonAlloc<T, TAdd>(this List<T> list, IList<T> toAdd);
public static void AddRangeNonAlloc<T, TAdd>(this List<T> list, IReadOnlyList<T> toAdd);
```

### CollectionExtensions

Same as above, but works for `IList<T>` interface rather than just concrete `List<T>`.
```csharp
public static void AddRangeNonAlloc<T>(this IList<T> list, IList<T> toAdd);
public static void AddRangeNonAlloc<T>(this IList<T> list, IReadOnlyList<T> toAdd);
```

`IndexOf` and `Contains` implementations for `IReadOnlyList<T>`. Optional overloads to allow using a custom `IEqualityComparer<T>`.
```csharp
public static int IndexOf<T, TList>(this TList list, T value) where TList : IReadOnlyList<T>;
public static int IndexOf<T, TList, TComp>(this TList list, T value, TComp comparer) where TList : IReadOnlyList<T> where TComp : IEqualityComparer<T>;

public static bool Contains<T, TList>(this TList list, T value) where TList : IReadOnlyList<T>;
public static bool Contains<T, TList, TComp>(this TList list, TComp comp, T value) where TList : IReadOnlyList<T> where TComp : IEqualityComparer<T>;
```

### BinarySearchExtensions

Binary search for `IReadOnlyList<T>`. Fully generic so you can specify any types you want, even structs, without boxing (when using the right generic version). Implementing `IComparer<T>` in particular as a struct is nice for fast and allocation free comparisons.
```csharp
public static int BinarySearch<T, TList>(this TList list, T value) where TList : IReadOnlyList<T>;
public static int BinarySearch<T, TList>(this TList list, T value, int index, int count) where TList : IReadOnlyList<T>;
public static int BinarySearch<T, TList, TComp>(this TList list, T value, TComp comparer) where TList : IReadOnlyList<T> where TComp : IComparer<T>;
public static int BinarySearch<T, TList, TComp>(this TList list, T value, int index, int count, TComp comparer) where TList : IReadOnlyList<T> where TComp : IComparer<T>;
```

### ExactSizeArrayPool

An implementation of ArrayPool that returns arrays of the exact size specified, rather than arrays that *may be larger* than the requested size.
```csharp
int[] array = ExactSizeArrayPool<int>.Shared.Rent(size); // Returns an array of length `size`
```

### PooledList

Reimplementation of `List<T>` that uses `ArrayPool<T>` for its internal arrays, rather than allocating new ones. The implementation was mostly copied directly from [Microsoft's official `List<T>` source code](https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/List.cs).
```csharp
PooledList<int> list = new PooledList<T>(size); // Returns a list with capacity larger than or equal to `size`

list.AddRange(collection); // When resizing the list, new arrays will be rented from `ArrayPool<T>` rather than allocated using new T[];
```

`PooledList<T>` objects can also themselves be pooled for further reduced memory pressure.
```csharp
PooledList<int> list = PooledList<T>().Rent(size); // Returns a list with capacity larger than or equal to `size`
```

## Author

[Kryzarel](https://www.youtube.com/@Kryzarel)

## License

MIT