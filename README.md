# Memory Leaks on iOS

See the [Xamarin.iOS history on this](https://stackoverflow.com/questions/13058521/is-this-a-bug-in-monotouch-gc/13059140#13059140).

## Case 1: View.Parent

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

## Case 2: C# Events

This creates a cycle:

```csharp
class MyView : UIView
{
    public UIDatePicker Picker { get; } = new UIDatePicker();
    
    public MyView()
    {
        AddSubview(Picker);
        Picker.ValueChanged += OnValueChanged;
    }
    
    // Use this instead and it doesn't leak!
    //static void OnValueChanged(object? sender, EventArgs e) { }
    
    void OnValueChanged(object? sender, EventArgs e) { }
}
```

`MyView` -> `UIDatePicker` -> `EventHandler` -> `MyView`

You can solve this one by making the `OnValueChanged` method `static`.

Try the [`event-example`](https://github.com/jonathanpeppers/MemoryLeaksOniOS/tree/event-example) branch for further details.
