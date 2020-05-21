# ICSharpCode.TextEditor for .NET Core 3.1

We're adding a new feature to [Visual ReCode](https://visualrecode.com) to
automatically migrate .NET 4.x projects to .NET Core 3.1. We expect this to
be helpful for people with projects that use WPF and Windows Forms now that
those are supported in Core, and also for projects like console applications,
Windows Services and so on which have Core equivalents.

I reached out on Twitter and asked for open-source WinForms or WPF projects,
and somebody pointed me at [GitExtensions](https://github.com/gitextensions/gitextensions).
That's an insanely big and complicated project and I really wanted to start smaller,
but I noticed it referenced [ICSharpCode.TextEditor](https://github.com/gitextensions/ICSharpCode.TextEditor)
and that's much more like it. The library project has already been converted to the
new SDK project format, but targeting .NET 4.6.1, and there's a Windows Forms
sample that's still using the old-style project format.

After a few fixes and improvements, Visual ReCode was able to cleanly migrate
both projects to .NET Core 3.1, using the `Microsoft.NET.Sdk.WindowsDesktop` SDK,
and the solution would build both in Visual Studio and from the CLI.

Unfortunately it threw an exception when I tried to run it, which I tracked down
to the [Ime class](https://github.com/VisualReCode/ICSharpCode.TextEditor/blob/master/NetCore31/src/ICSharpCode.TextEditor/Src/Gui/Ime.cs).
I don't know what IME is, but apparently it was causing crashes in WOW64 processes
and .NET 4.0 didn't like it, so there was some code to disable it in those environments:

```csharp
// For unknown reasons, the IME support is causing crashes when used in a WOW64 process
// or when used in .NET 4.0. We'll disable IME support in those cases.
var PROCESSOR_ARCHITEW6432 = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
if (PROCESSOR_ARCHITEW6432 == "IA64" || PROCESSOR_ARCHITEW6432 == "AMD64" || Environment.OSVersion.Platform == PlatformID.Unix || Environment.Version >= new Version(major: 4, minor: 0))
    disableIME = true;
else
    hIMEWnd = ImmGetDefaultIMEWnd(hWnd);
```

None of the conditions were `true` when running in .NET Core 3.1, so I changed it to set
`disableIME = true` unconditionally. [See the change here](https://github.com/VisualReCode/ICSharpCode.TextEditor/blob/master/NetCore31/src/ICSharpCode.TextEditor/Src/Gui/Ime.cs#L42).

With that one change, the sample project builds and runs under .NET Core 3.1.

Next job is to get the Test project to migrate cleanly as well.
