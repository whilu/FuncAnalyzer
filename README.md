# FuncAnalyzer

A method analyzer for Unity 3D. **Easy** to use and **Not invade** the source code.

## How to use

### Step 1

Import FuncAnalyzer package, purchase from [Asset Store](https://assetstore.unity.com/packages/slug/164221).

### Step 2

Add `[Analyze]` attribute to the method you want to analyze:

```csharp
[Analyze]
private int AnalyzeSampleFunction(string msg, int level)
{
    int i = 8;
    return i;
}
```

Now, run your game and the console will print analysis logs for the method.

## Analyze Property

There are some useful properties for `Analyze` attribute:

| Name  | Type  | Default value  | Description  |
| ------------ | ------------ | ------------ | ------------ |
| AnalyzingFlags  | [Flags](#Flags)  | `Flags.Default`  | Analyze flags, let FuncAnalyzer know which kind of analysis code you want to inject.  |
| Enable  | `bool`  | `true`  | Enable or disable analyze for this method.  |

Use `Enable` disable method analysis:

```csharp
[Analyze(Enable = false)]
private int AnalyzeSampleFunction(string msg, int level)
{
    int i = 8;
    return i;
}
```

If disabled, this method will not print logs.

## <span id="Flags">Analyze Flags</span>

The `co.lujun.funcanalyzer.Flags` class provides some analysis indicators:

| Name  | Description  |
| ------------ | ------------ |
| Args  | The method input parameters.  |
| Ret  | The method return value.  |
| Time  | Method execution time.  |
| Memory  | Memory information during method execution.  |
| Default  | This will provide the above four types of analysis.  |

Use `|` to indicate that some need to be analyzed at the same time, for example:

```csharp
[Analyze(AnalyzingFlags = Flags.Args | Flags.Time)]
private void AnalyzeSampleFunction(string msg, double price)
{
    int i = 8;
}
```

The above code indicates that the method input parameters and execution time need to be analyzed at the same time.

## Menu Tool

After successfully importing this package, the Unity menu bar will have a 'FuncAnalyzer' menu:

<img src="https://raw.githubusercontent.com/whilu/lujun.co-storge/master/image/funca_menu.png" width="20%" height="20%" />

Use the menu tool, you can do something like below:

| Menu item  | Description  |
| ------------ | ------------ |
| Inject analysis code  | Inject analysis code manually.  |
| Auto Inject  | Enable or disable automatic injecting. Once enabled, FuncAnalyzer will automatically inject analysis code after reload scripts(Default enable).  |

## Change logs

### 1.0.1(2020-03-27)
- Fix bugs

### 1.0.0(2020-03-06)
- First release

## About

If you have any questions, contact me: [lujun.byte#gmail.com](mailto:lujun.byte@gmail.com).

## License

[Apache License 2.0](https://github.com/whilu/FuncAnalyzer/blob/master/LICENSE)
