# Memory Leaks on iOS

The core issue is doing something like this:

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
