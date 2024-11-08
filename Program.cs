using System;
using System.Linq;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Usage: <width> <height> <output filename> <mode> [<random seed>]");
            return;
        }

        // Parse command-line arguments
        int width = int.Parse(args[0]);
        int height = int.Parse(args[1]);
        string filename = args[2];
        string mode = args[3];
        int seed = args.Length > 4 ? int.Parse(args[4]) : 0;

        // Ensure the image size is large enough
        if (width * height < 256 * 256 * 256)
        {
            Console.WriteLine("Error: Image size must be at least 16 million pixels.");
            return;
        }

        // Create color set for all RGB values
        List<Rgb24> colors = new List<Rgb24>();
        for (int r = 0; r < 256; r++)
            for (int g = 0; g < 256; g++)
                for (int b = 0; b < 256; b++)
                    colors.Add(new Rgb24((byte)r, (byte)g, (byte)b));

        // Choose mode
        switch (mode.ToLower())
        {
            case "trivial":
                TrivialMode(width, height, filename, colors);
                break;
            case "random":
                RandomMode(width, height, filename, colors, seed);
                break;
            case "pattern":
                PatternMode(width, height, filename, colors);
                break;
            default:
                Console.WriteLine("Invalid mode. Choose 'trivial', 'random', or 'pattern'.");
                break;
        }
    }

    // Trivial Mode: Place colors in sequential order
    static void TrivialMode(int width, int height, string filename, List<Rgb24> colors)
    {
        using var image = new Image<Rgb24>(width, height);
        int index = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                image[x, y] = colors[index];
                index = (index + 1) % colors.Count;
            }
        }
        image.Save(filename);
        Console.WriteLine($"Trivial mode image saved as {filename}");
    }

    // Random Mode: More structured randomization to create visual variety
    static void RandomMode(int width, int height, string filename, List<Rgb24> colors, int seed)
    {
        Random random = new Random(seed);
        using var image = new Image<Rgb24>(width, height);

        // Fill the image with colors in a structured random manner
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Randomly select a color from the color list
                int colorIndex = random.Next(colors.Count);
                image[x, y] = colors[colorIndex];
            }
        }

        // Save the image
        image.Save(filename);
        Console.WriteLine($"Random mode image saved as {filename}");
    }

    // Pattern Mode: Simple mandala-like pattern with mirrored colors
    static void PatternMode(int width, int height, string filename, List<Rgb24> colors)
    {
        using var image = new Image<Rgb24>(width, height);
        int halfWidth = width / 2;
        int halfHeight = height / 2;
        int index = 0;

        for (int y = 0; y < halfHeight; y++)
        {
            for (int x = 0; x < halfWidth; x++)
            {
                Rgb24 color = colors[index];
                image[x, y] = color;
                image[width - x - 1, y] = color;
                image[x, height - y - 1] = color;
                image[width - x - 1, height - y - 1] = color;
                index = (index + 1) % colors.Count;
            }
        }

        image.Save(filename);
        Console.WriteLine($"Pattern mode image saved as {filename}");
    }
}
