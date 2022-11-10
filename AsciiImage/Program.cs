// See https://aka.ms/new-console-template for more information
using AsciiImage;

if(args.Length < 3){
    throw new ArgumentException("You need at least 3 arguments: sourceImage, "
    + "width of result in symbols, height of result in symbols");
}

List<char> codeChars = new List<char>() {' ', '\'', '.', '^', '*', 'a', '#', '%'};

int width = int.Parse(args[1]);
int height = int.Parse(args[2]);
AsciiConverter ac = new AsciiConverter(args[0], width / 2, height, codeChars);
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