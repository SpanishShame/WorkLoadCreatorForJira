using JiraWorkloadReportCreator;
using JiraWorkloadReportCreator.Entity;
using JiraWorkloadReportCreator.Helpers;
using System.Text.Json;

Console.ForegroundColor = ConsoleColor.Red;
JiraConfig? config = null;
try
{
    config = JsonSerializer.Deserialize<JiraConfig>(File.ReadAllText("settings.json"));
}catch(Exception e)
{
    Console.WriteLine($"cannot get settings:{e.Message}");
    Console.ReadKey();
    return;
}

List<Report> reportList = new List<Report>();
try
{
    var jiraModule = new WorkModule(config);
    reportList = (await jiraModule.getAllLogFromJira())
        .GroupBy(c => c.author).Select(x =>
                new Report
                {
                    Author = x.Key,
                    Sum = x.Sum(c => c.workTime),
                    Tasks = x.Select(x => new WorkTask { TaskId = x.taskId, WorkTime = x.workTime, Date = x.date, Comment = x.comment }).ToArray()
                }).ToList();
}
catch (Exception e)
{
    Console.WriteLine($"cannot get report object from jira:{e.Message}");
    Console.ReadKey();
    return;
}


try
{
    
    StaticExcelHelper.CreateExcel(reportList, config.ReportPath);
}
catch (Exception e)
{
    Console.WriteLine($"cannot get create report with openXml:{e.Message}");
    Console.ReadKey();
    return;
}
Console.ForegroundColor= ConsoleColor.Green;
Console.WriteLine($"report was created in {Path.GetFullPath(config.ReportPath)}");
Console.ReadLine();
return;
