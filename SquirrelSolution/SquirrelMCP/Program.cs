using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Squirrel;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();

[McpServerToolType]
public static class EchoTool
{
    static Table data = new EmptyTable();
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"hello {message}";
    [McpServerTool, Description("Length of the message.")]
    public static string Length(string message) => $"{message} is {message.Length} characters long.";

    [McpServerTool, Description("Load the CSV")]
    public static string LoadCsv(string path)
    {
        data = DataAcquisition.LoadCsv(path);
        return $"Loaded the table with {data.RowCount} rows";
    }

    [McpServerTool, Description("Show me only female students")]
    public static string FilterFemale() =>
        data.Filter("Gender", "F")
            .Rows
            .Select(row =>
            {
                return row["Name"] + row["Age"];
            }).Aggregate((a, b) => a + "," + b);
    
    [McpServerTool, Description("Show me only students who are {gender}")]
    public static string FilterGender(string gender) =>
        data.Filter("Gender", gender)
            .Rows
            .Select(row =>
            {
                return row["Name"] + row["Age"];
            }).Aggregate((a, b) => a + "," + b);

    [McpServerTool, Description("Show me only students who took course {course}")]
    public static string FilterCourse(string course) =>
        data.Filter("Course", course)
            .Rows
            .Select(row =>
            {
                return row["Name"] + row["Age"];
            }).Aggregate((a, b) => a + "," + b);

}