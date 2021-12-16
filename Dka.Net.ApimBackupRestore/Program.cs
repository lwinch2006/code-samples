using Dka.Net.ApimBackupRestore.Utils;

var configuration = ConfigurationUtils.BuildConfiguration();
var httpClient = new HttpClient();

var displayIndex = 0;

while (true)
{
    DrawDisplay(displayIndex);
    var key = Console.ReadKey();
    await TakeAction(displayIndex, key);
}

void DrawDisplay(int displayIndex)
{
    switch (displayIndex)
    {
        case 0:
            DrawMainDisplay();
            break;
    }
}

async Task TakeAction(int displayIndex, ConsoleKeyInfo key)
{
    Console.WriteLine();
    
    switch (displayIndex)
    {
        case 0:
            await TakeActionAtMainDisplay(key);
            break;
    }
}

void DrawMainDisplay()
{
    Console.WriteLine("(1) Backup APIM configuration");
    Console.WriteLine("(2) Restore APIM configuration");
    Console.WriteLine("(3) Exit");
}

async Task TakeActionAtMainDisplay(ConsoleKeyInfo key)
{
    switch (key.KeyChar)
    {
        case '1':
            Console.WriteLine("Backing up APIM configuration...");
            await ApimUtils.Backup(configuration, httpClient);
            Console.WriteLine("Backup of APIM configuration done");
            Console.WriteLine();
            break;
        case '2':
            Console.WriteLine("Restoring APIM configuration...");
            await ApimUtils.Restore(configuration, httpClient);
            Console.WriteLine("Restore of APIM configuration done");
            Console.WriteLine();
            break;
        case '3':
            Console.WriteLine("Exiting...");
            Environment.Exit(1);
            break;
    }    
}