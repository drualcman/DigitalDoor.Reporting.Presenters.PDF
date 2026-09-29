using DigitalDoor.Reporting.Presenters.PDF.Samples;

string outputDirectory = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "output");
await SampleConsoleRunner.RunAsync(outputDirectory, "skia", includePlainBase64Strings: true);
