# Memory Leaks on iOS

*UPDATE* it appears two GCs solves the issue, more investigation needed:

```csharp
await Task.Yield();
GC.Collect();
GC.WaitForPendingFinalizers();
```

The core issue was doing something like this:

```csharp
class MyViewSubclass : UIView
{
    public override void LayoutSubviews()
    {
        Console.WriteLine("LayoutSubviews");
        base.LayoutSubviews();
    }
}

//...

var parent = new MyViewSubclass();
var view = new MyViewSubclass();
parent.AddSubview(view);
```

Appears to leak unless you do:

```csharp
view.RemoveFromSuperview();
```

This is a problem in .NET MAUI, because they would have to explicitly call `RemoveFromSuperview()` many places to solve issues.

To repro, click the button in the sample:

![screenshot of a popup](screenshots/screenshot.png)
