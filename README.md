# Memory Leaks on iOS

See the [Xamarin.iOS history on this](https://stackoverflow.com/questions/13058521/is-this-a-bug-in-monotouch-gc/13059140#13059140).

The core issue was doing something like this:

```csharp
class MyViewSubclass : UIView
{
    public UIView? Parent { get; set; }

    public void Add(MyViewSubclass subview)
    {
        subview.Parent = this;
        AddSubview(subview);
    }
}

//...

var parent = new MyViewSubclass();
var view = new MyViewSubclass();
parent.Add(view);
```

Appears to leak unless you do one of:

```csharp
view.Parent = null;
view.RemoveFromSuperview();
```

This is a problem in .NET MAUI, because they would have to explicitly unset many values to solve issues.

To repro, click the button in the sample:

![screenshot of a popup](screenshots/screenshot.png)
