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
			objReference = new WeakReference(new object());

			var parent = new MyViewSubclass();
			var view = new MyViewSubclass();
			parent.AddSubview(view);
			// Uncomment to solve the leak
			//view.RemoveFromSuperview();
			viewReference = new WeakReference(view);

			// NOTE: the examples below work fine

			/* Just one view works fine */
			// viewReference = new WeakReference(new MyViewSubclass());

			/* Using a UIView directly works fine */
			// var parent = new UIView();
			// var view = new UIView();
			// parent.AddSubview(view);
			// viewReference = new WeakReference(view);
		}

		await Task.Yield();
		GC.Collect();
		GC.WaitForPendingFinalizers();

		new UIAlertView("Results",
$"""
new object() is alive: {objReference.IsAlive}
new MyViewSubclass() is alive: {viewReference.IsAlive}
"""
			,
			null, "Ok").Show();
	}

	class MyViewSubclass : UIView
	{
		public override void LayoutSubviews()
		{
			Console.WriteLine("LayoutSubviews");
			base.LayoutSubviews();
		}
	}
}
