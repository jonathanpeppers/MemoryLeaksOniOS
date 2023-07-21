namespace MemoryLeaksOniOS;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		var button = new UIButton(Window.Frame);
		button.SetTitle("Click Me", UIControlState.Normal);
		button.TouchUpInside += OnClicked;

		var vc = new UIViewController ();
		vc.View!.AddSubview (button);
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}

	async void OnClicked(object? sender, EventArgs e)
	{
		WeakReference objReference, viewReference;

		{
			var view = new MyView();
			objReference = new WeakReference(new object());
			viewReference = new WeakReference(view);
		}

		// 10 GCs to just be thorough
		for (int i = 0; i < 10; i++)
		{
			await Task.Yield();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		new UIAlertView("Results",
$"""
new object() is alive: {objReference.IsAlive}
new MyViewSubclass() is alive: {viewReference.IsAlive}
"""
			,
			null, "Ok").Show();
	}

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
}
