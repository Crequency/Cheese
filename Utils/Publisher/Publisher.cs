using Cheese.Options;
using Cheese.Utils.Publisher;
using Common.BasicHelper.Utils.Extensions;
using System.Diagnostics;
using System.IO.Compression;

internal class Publisher
{
    private string? BaseSlnDir { get; set; }

    private bool FindSolutionUpward(string startLocation, string solutionFileName)
    {
        var currentDirInfo = new DirectoryInfo(startLocation);

        if (!currentDirInfo.Exists || currentDirInfo.Parent is null || CheckForFile(currentDirInfo, solutionFileName))
        {
            var path = Path.Combine(currentDirInfo.FullName, solutionFileName);
            if (File.Exists(path))
            {
                BaseSlnDir = currentDirInfo.FullName;
                return true;
            }
            else return false;
        }

        return FindSolutionUpward(currentDirInfo.Parent.FullName, solutionFileName);
    }

    private static bool CheckForFile(DirectoryInfo directory, string fileNameIgnoreCase) => directory.GetFiles().Any(
        x => x.Name.Equals(fileNameIgnoreCase, StringComparison.OrdinalIgnoreCase)
    );

    internal void Execute(PublishOptions options)
    {
        Console.WriteLine(
            """
            Running Cheese Publisher
            """
        );

        var location = Environment.CurrentDirectory.GetFullPath();

        if (!FindSolutionUpward(location, "KitX.sln"))
        {
            Console.WriteLine("! You're not in a KitX repo.");
            return;
        }

        var publishDir = $"{BaseSlnDir}/KitX Publish".GetFullPath();

        if (publishDir is not null && Directory.Exists(publishDir) && !options.SkipGenerating)
            foreach (var dir in new DirectoryInfo(publishDir).GetDirectories())
                Directory.Delete(dir.FullName, true);

        var path = Path.GetFullPath("../../KitX Clients/KitX Dashboard/KitX Dashboard/");
        var pro = "Properties/";
        var pub = "PublishProfiles/";
        var ab_pub_path = Path.GetFullPath($"{path}{pro}{pub}");
        var files = Directory.GetFiles(
            ab_pub_path,
            "*.pubxml",
            SearchOption.AllDirectories
        );

        var finished_threads = 0;
        var executing_thread_index = 0;

        var update_finished_threads_lock = new object();
        var single_thread_update_lock = new object();

        var random = new Random();

        var thread_output_colors = new Dictionary<int, ConsoleColor>();
        var used_colors_count = 0;
        var default_color = Console.ForegroundColor;

        int get_random_index(int max) => random.Next(0, max);

        ConsoleColor get_random_color()
        {
            var cc = Defines.AvailableColors[get_random_index(Defines.AvailableColors.Count)];
            if (used_colors_count < Defines.AvailableColors.Count)
            {
                while (thread_output_colors.Values.ToList().Contains((ConsoleColor)cc))
                    cc = Defines.AvailableColors[get_random_index(Defines.AvailableColors.Count)];
            }
            ++used_colors_count;
            return (ConsoleColor)cc;
        }

        var tasks = new List<Action>();

        foreach (var item in files)
        {
            var index = executing_thread_index++;
            var color = get_random_color();
            thread_output_colors.Add(index, color);
            var filename = Path.GetFileName(item);

            void print(string msg)
            {
                Console.ForegroundColor = thread_output_colors[index];
                Console.WriteLine(msg);
                Console.ForegroundColor = default_color;
            }

            tasks.Add(() =>
            {
                var cmd = "dotnet";
                var arg = $"publish \"{Path.GetFullPath(path + "/KitX.Dashboard.csproj")}\" \"/p:PublishProfile={item}\"";
                lock (single_thread_update_lock)
                {
                    print(
                        $"""
                >>> On task_{index}:
                    Task file: {filename}
                    Executing: {cmd} {arg}
                    Output:

                """
                    );
                }
                var process = new Process();
                var psi = new ProcessStartInfo()
                {
                    FileName = cmd,
                    Arguments = arg,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                process.StartInfo = psi;
                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    Console.WriteLine($"            {line}");
                }

                process.WaitForExit();

                lock (update_finished_threads_lock)
                {
                    ++finished_threads;
                    print($">>> Finished task_{index}, still {files.Length - finished_threads} tasks running.");
                }
            });

            print($">>> New task: task_{index}\t->   {filename}");
        }

        if (!options.SkipGenerating)
            foreach (var task in tasks)
                task.Invoke();

        if (!options.SkipGenerating)
            while (finished_threads != files.Length) ;  //  Wait until all tasks done.
                                                        //Task.WhenAll(tasks); // If you want to use async/await, you can use this.

        Console.WriteLine($">>> All tasks done.");

        if (!options.SkipPacking && publishDir is not null)
        {
            Console.WriteLine(">>> Begin packing.");

            var folders = new DirectoryInfo(publishDir).GetDirectories();

            foreach (var folder in folders)
            {
                var name = folder.Name;
                var zipFileName = $"{publishDir}/{name}.zip";

                Console.WriteLine($">>> Packing {name}");

                if (File.Exists(zipFileName))
                    File.Delete(zipFileName);

                ZipFile.CreateFromDirectory(
                    folder.FullName,
                    zipFileName,
                    CompressionLevel.SmallestSize,
                    true
                );
            }

            Console.WriteLine(">>> Packing done.");
        }
    }
}
