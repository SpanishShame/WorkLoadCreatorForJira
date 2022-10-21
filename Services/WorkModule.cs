using System.Net.Http.Headers;
using JiraWorkloadReportCreator.Entity;
using Newtonsoft.Json;

namespace JiraWorkloadReportCreator
{
    public class WorkModule
    {
        private readonly HttpClient _httpClient;
        private JiraConfig _config;
        public WorkModule(HttpClient httpClient, JiraConfig config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<(DateTime date, string author, double workTime, string comment, string taskId)>> getAllLogFromJira()
        {
            var result = new List<(DateTime, string, double, string, string)>();
            var tasks = await getTasks();

            var requests = tasks.Select(GetWorkloadFromTask);
            var subResults = await Task.WhenAll(requests);
            foreach (var workloads in subResults)
            {
                result.AddRange(workloads);
            }
           
            return result;
        }
        private async Task<List<(DateTime date, string author, double workTime, string comment, string taskId)>>  GetWorkloadFromTask(string item)
        {
            string JiraUrl = String.Format(_config.JiraTaskUrl, _config.JiraUrl, item);
            var dict = GetAllWorkLogFromDynamic(await GetRequest(JiraUrl));
            return dict.Select(c => (c.Key.date, c.Key.author, c.Value.spendTime, c.Value.comment, item)).ToList();
        }
        private Dictionary<(DateTime date, string author), (double spendTime, string comment)> GetAllWorkLogFromDynamic(dynamic json)
        {
            var tempList = new Dictionary<(DateTime date, string author), (double spendTime, string comment)>();
            dynamic data = JsonConvert.DeserializeObject(json);
            var worklogs = data.worklogs;
            foreach (var item in worklogs)
            {
                
                double timeDyn = item.timeSpentSeconds / 60.0 / 60.0;
                string dateDyn = Convert.ToString(item.started);
                var newDate = DateTime.Parse(dateDyn).Date;
                if (tempList.ContainsKey((newDate, item.author.displayName)))
                {
                    var currentItem = tempList[(newDate, item.author.displayName)];
                    tempList[(newDate, item.author.displayName)] = (currentItem.spendTime + timeDyn, currentItem.comment + Environment.NewLine +(string)item.comment);
                }
                else
                {
                    tempList.Add((newDate, item.author.displayName), (timeDyn, (string)item.comment));
                }
            }
            return tempList;
        }

        public async Task<IEnumerable<string>> getTasks()
        {
            var jiraUrlIssues = String.Format(_config.JiraIssueUrl, _config.JiraUrl, _config.ReportStartDate.ToString("yyyy-MM-dd"));
            var response = await GetRequest(jiraUrlIssues);
            return GetTasksFromResponse(response);
        }

        private IEnumerable<string> GetTasksFromResponse(string response)
        {
            dynamic data = JsonConvert.DeserializeObject(response);
            foreach (var issue in data.issues)
            {
                yield return issue.key;
            }
        }

        private async Task<string> GetRequest(string URL)
        {
            string result = "";
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                                                                                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                                                                                                       $"{_config.JiraLogin}:{_config.JiraToken}")));
            var response = await _httpClient.GetAsync(URL); 
            response.EnsureSuccessStatusCode();
            result = await response.Content.ReadAsStringAsync();  //Make sure to add a reference to System.Net.Http.Formatting.dll
            return result;
        }
    }
}
