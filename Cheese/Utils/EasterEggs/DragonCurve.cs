using Spectre.Console;

namespace Cheese.Utils.EasterEggs;

public class DragonCurve : IEasterEgg
{
    private static int Width { get; set; }

    private static int Height { get; set; }

    private Canvas Panel { get; }

    public DragonCurve()
    {
        Width = Console.WindowWidth;
        Height = Console.WindowHeight * 2;

        Panel = new(Width, Height);
    }

    public void Run()
    {
        MainProcess();

        Thread.Sleep(500);

        Exit();
    }

    private void MainProcess()
    {
        const int maxDepth = 8;

        var currentDepth = 0;

        var beginX = Width / 2 - 13;
        var beginY = Height / 2 - 16;

        int rotateX;
        int rotateY;

        var paintedPoints = new List<(int, int)>();

        Paint(beginX, beginY);
        Paint(beginX + 2, beginY);
        Paint(beginX + 2, beginY + 2);

        while (currentDepth < maxDepth)
        {
            ++currentDepth;

            var copiedPoints = new List<(int, int)>(paintedPoints);
            var r = (rotateX, rotateY);

            foreach (var rotated in copiedPoints.Select(p => Rotate(p.ToTuple(), r.ToTuple())))
                Paint(rotated.Item1 + 1, rotated.Item2 + 2);

            AnsiConsole.Write(Panel);

            Thread.Sleep(500);

            if (currentDepth != maxDepth)
                Clean(Panel.Height);
        }

        return;

        void Paint(int x, int y)
        {
            Draw(x, y);
            Draw(x + 1, y);
            Draw(x, y + 1);
            Draw(x + 1, y + 1);

            paintedPoints.Add((x, y));

            rotateX = x;
            rotateY = y;

            return;

            void Draw(int innerX, int innerY)
            {
                if (innerX < Width && innerX >= 0 && innerY < Height && innerY >= 0)
                    Panel.SetPixel(innerX, innerY, Color.White);
            }
        }

        (int, int) Rotate(Tuple<int, int> p, Tuple<int, int> r)
        {
            var x = p.Item1;
            var y = p.Item2;
            var rx = r.Item1;
            var ry = r.Item2;

            return (-(y - ry) + rx, x - rx + ry);
        }

        void Clean(int count)
        {
            if (count <= 0) return;
            Console.Write("\x1b[2K");
            for (var i = 1; i < count; ++i)
            {
                Console.Write("\x1b[1A");
                Console.Write("\x1b[2K");
            }
        }
    }

    private void Exit()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[blue]Good bye ~[/]");
    }
}