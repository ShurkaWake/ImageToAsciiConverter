using AsciiImage;

if(args.Length < 3){
    Console.Error.WriteLine("You need at least 3 arguments: sourceImage, "
            + "width of result in symbols, height of result in symbols");
    return;
}

// Characters used to encode image
List<char> codeChars = new List<char>() {' ', '\'', '.', '^', '*', 'a', '#', '%'};

int width = 0;
int height = 0;
try
{
    width = int.Parse(args[1]);
    height = int.Parse(args[2]);
}
catch
{
    Console.Error.WriteLine("Width and Height of output must be positive integer numbers");
    return;
}

if (width > 2000 || height > 2000)
{
    Console.Error.WriteLine("Width and Height of output must be less than 2000");
    return;
}


AsciiConverter ac;
try
{
    ac = new AsciiConverter(args[0], width / 2, height, codeChars);
}
catch
{
    Console.Error.WriteLine("Something wrong with file. Make sure that path is correct or try another image.");
    return;
}
Console.Clear();

var chars = ac.CharsMatrix;
var colors = ac.ColorsMatrix;
for(int i = 0; i < chars.GetLength(1); i++){
    for(int j = 0; j < chars.GetLength(0); j++){
        Console.ForegroundColor = colors[j, i];
        Console.Write(chars[j, i]);
        Console.Write(chars[j, i]);
    }
    Console.WriteLine();
}

Console.ForegroundColor = ConsoleColor.White;
