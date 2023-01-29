using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.TaskManager
{
    public class TaskManager : ITaskManager
    {
        private Dictionary<string, Task> tasksById = new Dictionary<string, Task>();
        private Dictionary<string, List<Task>> tasksByDomain = new Dictionary<string, List<Task>>();
        private List<string> executionQueue = new List<string>();
        private Dictionary<string, Task> executed = new Dictionary<string, Task>();

        public void AddTask(Task task)
        {
            executionQueue.Add(task.Id);
            tasksById.Add(task.Id, task);
            if (!tasksByDomain.ContainsKey(task.Domain))
            {
                tasksByDomain.Add(task.Domain, new List<Task>());
            }
            tasksByDomain[task.Domain].Add(task);
        }

        public bool Contains(Task task)
            => tasksById.ContainsKey(task.Id);

        public void DeleteTask(string taskId)
        {
            if (!tasksById.ContainsKey(taskId))
            {
                throw new ArgumentException();
            }

            var task = tasksById[taskId];
            tasksById.Remove(taskId);
            executionQueue.Remove(task.Id);
            tasksByDomain[task.Domain].Remove(task);
            executed.Remove(task.Id);
        }

        public Task ExecuteTask()
        {
            if (executionQueue.Count == 0)
            {
                throw new ArgumentException();
            }

            var taskId = executionQueue[0];
            executionQueue.RemoveAt(0);
            executed.Add(taskId, tasksById[taskId]);

            return tasksById[taskId];
        }

        public IEnumerable<Task> GetAllTasksOrderedByEETThenByName()
            => tasksById.Values
            .OrderByDescending(t => t.EstimatedExecutionTime)
            .ThenBy(t => t.Name.Length);

        public IEnumerable<Task> GetDomainTasks(string domain)
        {
            if (!tasksByDomain.ContainsKey(domain))
            {
                throw new ArgumentException();
            }

            return tasksByDomain[domain].Where(t => executionQueue.Contains(t.Id));
        }

        public Task GetTask(string taskId)
        {
            if (!tasksById.ContainsKey(taskId))
            {
                throw new ArgumentException();
            }

            return tasksById[taskId];
        }

        public IEnumerable<Task> GetTasksInEETRange(int lowerBound, int upperBound)
            => tasksById.Values
            .Where(t => executionQueue.Contains(t.Id))
            .Where(t => t.EstimatedExecutionTime >= lowerBound && t.EstimatedExecutionTime <= upperBound)
            .OrderBy(t => executionQueue.IndexOf(t.Id));

        public void RescheduleTask(string taskId)
        {
            if (!executed.ContainsKey(taskId))
            {
                throw new ArgumentException();
            }

            executionQueue.Add(taskId);
            executed.Remove(taskId);
        }

        public int Size()
            => executionQueue.Count;
    }
}
