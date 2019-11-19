using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    public class TaskList : List<Task>
    {

        //Wraps action of adding a new task
        public void Add(Action task)
        {
            base.Add(new Task(task));
        }


        /// <summary>
        /// Starts all tasks in queue and returns control to caller after all have completed
        /// </summary>
        public void ExecuteAll()
        {
            ForEach(x => { x.Start(); });
            Task.WaitAll(this.ToArray());
        }


        /// <summary>
        /// Starts all tasks in queue (async) and returns task
        /// </summary>
        public Task ExecuteAllAsync()
        {
            ForEach(x => { x.Start(); });
            return Task.WhenAll(this.ToArray());
        }
    }
}
