using System.Drawing;

namespace AsciiImage;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416")]
public class AsciiConverter
{
    Bitmap sourceImage;
    double[,] intensityMatrix;
    ConsoleColor[,] colorsMatrix;
    char[,] result;
    double tileWidth;
    double tileHeight;
    Dictionary<ConsoleColor, Color> consoleColorsRGB = new Dictionary<ConsoleColor, Color>();
    IEnumerable<char> codeChars;

    public AsciiConverter(string sourcePath, int width, int height, in IEnumerable<char> charsToUse)
    {
        FillConsoleColorsDoctionary();

        sourceImage = new Bitmap(sourcePath);
        intensityMatrix = new double[width, height];
        colorsMatrix = new ConsoleColor[width, height];
        result = new char[width, height];
        tileWidth = (int) Math.Ceiling(sourceImage.Width / (double) width);
        tileHeight = (int) Math.Ceiling(sourceImage.Height / (double) height);
        codeChars = charsToUse;
        
        Parse(sourceImage);
    }

    public ConsoleColor[,] ColorsMatrix
    {
        get
        {
            return colorsMatrix;
        }
    }

    public char[,] CharsMatrix
    {
        get
        {
            return result;
        }
    }

    private void Parse(Bitmap image)
    {
        for(int i = 0; i < intensityMatrix.GetLength(0); i++)
        {
            for(int j = 0; j < intensityMatrix.GetLength(1); j++)
            {
                ProcessTile(i, j, image);
                char t;
                try
                {
                    t = codeChars.ElementAt((int) intensityMatrix[i,j] / (256 / codeChars.Count()));
                }
                catch
                {
                    t = ' ';
                }
                result[i, j] = t;
            }
        }
    }

    private void ProcessTile(int x, int y, Bitmap image)
    {
        long R = 0;
        long G = 0;
        long B = 0;
        double intensity = 0;
        double pixelsProcessed = 0;

        for(int i = 0; i < tileWidth; i++)
        {
            for(int j = 0; j < tileHeight; j++)
            {
                int currentX = (int) Math.Truncate(x * tileWidth) + i;
                int currentY = (int) Math.Truncate(y * tileHeight) + j;

                if (IsInBounds(currentX, currentY, image))
                {
                    var pixel = image.GetPixel(currentX, currentY);
                    R += pixel.R;
                    G += pixel.G;
                    B += pixel.B;
                    intensity += GetPixelIntensivity(currentX, currentY, image);
                    pixelsProcessed++;
                }
            }
        }

        R = ConvertIntToByteBounds((int) (R / pixelsProcessed));
        G = ConvertIntToByteBounds((int) (G / pixelsProcessed));
        B = ConvertIntToByteBounds((int) (B / pixelsProcessed));

        intensityMatrix[x, y] = intensity / pixelsProcessed;
        colorsMatrix[x, y] = GetConsoleColor(Color.FromArgb((int) R, (int) G, (int) B));

        byte ConvertIntToByteBounds(int number)
        {
            byte result = 0;
            if (number < 0)
            {
                result = 0;
            }
            else if (number > 255)
            {
                result = 255;
            }
            else 
            {
                result = (byte) number;
            }
            return result;
        }
    }

    private bool IsInBounds(int x, int y, Bitmap image)
    {
        return x >= 0 && x < image.Width && y >= 0 && y < image.Height;
    }
    
    private double GetPixelIntensivity(int x, int y, Bitmap image)
    {
        var pixel = image.GetPixel(x, y);
        return Math.Sqrt(
            (   
                Math.Pow(pixel.R, 2) 
                + Math.Pow(pixel.G, 2) 
                + Math.Pow(pixel.B, 2)
            )
            / 3
        );
    }

    private void FillConsoleColorsDoctionary()
    {
        consoleColorsRGB.Add(ConsoleColor.Black, Color.FromArgb(0x00, 0x00, 0x00));
        consoleColorsRGB.Add(ConsoleColor.DarkBlue, Color.FromArgb(0x00, 0x00, 0x80));
        consoleColorsRGB.Add(ConsoleColor.DarkGreen, Color.FromArgb(0x00, 0x80, 0x00));
        consoleColorsRGB.Add(ConsoleColor.DarkCyan, Color.FromArgb(0x00, 0x80, 0x80));
        consoleColorsRGB.Add(ConsoleColor.DarkRed, Color.FromArgb(0x80, 0x00, 0x00));
        consoleColorsRGB.Add(ConsoleColor.DarkMagenta, Color.FromArgb(0x80, 0x00, 0x80));
        consoleColorsRGB.Add(ConsoleColor.DarkYellow, Color.FromArgb(0x80, 0x80, 0x00));
        consoleColorsRGB.Add(ConsoleColor.DarkGray, Color.FromArgb(0x80, 0x80, 0x80));
        consoleColorsRGB.Add(ConsoleColor.Blue, Color.FromArgb(0x00, 0x00, 0xFF));
        consoleColorsRGB.Add(ConsoleColor.Green, Color.FromArgb(0x00, 0xFF, 0x00));
        consoleColorsRGB.Add(ConsoleColor.Cyan, Color.FromArgb(0x00, 0xFF, 0xFF));
        consoleColorsRGB.Add(ConsoleColor.Red, Color.FromArgb(0xFF, 0x00, 0x00));
        consoleColorsRGB.Add(ConsoleColor.Magenta, Color.FromArgb(0xFF, 0x00, 0xFF));
        consoleColorsRGB.Add(ConsoleColor.Yellow, Color.FromArgb(0xFF, 0xFF, 0x00));
        consoleColorsRGB.Add(ConsoleColor.Gray, Color.FromArgb(0xC0, 0xC0, 0xC0));
        consoleColorsRGB.Add(ConsoleColor.White, Color.FromArgb(0xFF, 0xFF, 0xFF));
    }

    private ConsoleColor GetConsoleColor(Color color)
    {
        double minDist = double.MaxValue;
        ConsoleColor result = ConsoleColor.White;

        foreach (var elem in consoleColorsRGB)
        {
            double distance = GetDistanceRGB(elem.Value, color);
            if (minDist > distance)
            {
                minDist = distance;
                result = elem.Key;
            }
        }

        return result;
    }

    private double GetDistanceRGB(Color a, Color b) 
    {
        return Math.Sqrt (
            Math.Pow(a.R - b.R, 2)
            + Math.Pow(a.G - b.G, 2)
            + Math.Pow(a.B - b.B, 2)
        );
    }
}
