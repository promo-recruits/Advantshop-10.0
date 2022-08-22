//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Mails
{
    public abstract class BaseTaskMailTemplate : MailTemplate
    {
        private readonly Task _task;

        public BaseTaskMailTemplate(Task task)
        {
            _task = task;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = formatedStr.Replace("#TASK_ID#", _task.Id.ToString());
            formatedStr = formatedStr.Replace("#TASK_PROJECT#", _task.TaskGroup != null ? _task.TaskGroup.Name : string.Empty);
            formatedStr = formatedStr.Replace("#TASK_NAME#", _task.Name);
            formatedStr = formatedStr.Replace("#TASK_DESCRIPTION#", _task.Description);
            formatedStr = formatedStr.Replace("#TASK_STATUS#", _task.Status.Localize());
            formatedStr = formatedStr.Replace("#TASK_PRIORITY#", _task.Priority.Localize());
            formatedStr = formatedStr.Replace("#DUEDATE#", _task.DueDate.HasValue ? _task.DueDate.Value.ToString("dd.MM.yyyy HH:mm") : "-");
            formatedStr = formatedStr.Replace("#DATE_CREATED#", _task.DateAppointed.ToString("dd.MM.yyyy HH:mm"));
            formatedStr = formatedStr.Replace("#MANAGER_NAME#", _task.TaskGroup.IsPrivateComments ? "-" : _task.Managers.Select(x => x.FullName).AggregateString(", "));
            formatedStr = formatedStr.Replace("#APPOINTEDMANAGER#", _task.AppointedManager != null ? _task.AppointedManager.FullName : string.Empty);
            formatedStr = formatedStr.Replace("#TASK_URL#", UrlService.GetAdminUrl("tasks/view/" + _task.Id));
            formatedStr = formatedStr.Replace("#TASK_ATTACHMENTS#", _task.Attachments.Select(x => GetLinkHTML(x.Path, x.FileName)).DefaultIfEmpty("-").AggregateString(", "));
            return formatedStr;
        }

        private static string GetLinkHTML(string url, string name)
        {
            return string.Format("<a href=\"{0}\">{1}</a>", url, name);
        }
    }

    public class TaskCreatedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnTaskCreated; }
        }

        public TaskCreatedMailTemplate(Task task) : base(task)
        {
        }
    }


    public class TaskAssignedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnTaskAssigned; }
        }

        public TaskAssignedMailTemplate(Task task) : base(task)
        {
        }
    }

    public class TaskDeletedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnTaskDeleted; }
        }

        private readonly string _modifier;

        public TaskDeletedMailTemplate(Task task, string modifier) : base(task)
        {
            _modifier = modifier;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = base.FormatString(formatedStr);
            formatedStr = formatedStr.Replace("#MODIFIER#", _modifier);
            return formatedStr;
        }
    }

    public class TaskCommentAddedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnTaskCommentAdded; }
        }

        private readonly string _author;
        private readonly string _comment;

        public TaskCommentAddedMailTemplate(Task task, string author, string comment) : base(task)
        {
            _author = author;
            _comment = comment;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = base.FormatString(formatedStr);
            formatedStr = formatedStr.Replace("#AUTHOR#", _author);
            formatedStr = formatedStr.Replace("#COMMENT#", _comment);
            return formatedStr;
        }
    }

    public class TaskChangedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnTaskChanged; }
        }

        private readonly string _modifier;
        private readonly string _changesTable;
        private readonly Task _taskPrev;

        public TaskChangedMailTemplate(Task task, string changesTable, string modifier, Task taskPrev) : base(task)
        {
            _modifier = modifier;
            _changesTable = changesTable;
            _taskPrev = taskPrev;
        }

        protected override string FormatString(string formatedStr)
        {
            // task name from previous state
            formatedStr = formatedStr.Replace("#TASK_NAME#", _taskPrev.Name);

            formatedStr = base.FormatString(formatedStr);

            formatedStr = formatedStr.Replace("#MODIFIER#", _modifier);
            formatedStr = formatedStr.Replace("#CHANGES_TABLE#", _changesTable);
            return formatedStr;
        }
    }

    public class TaskReminderMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnTaskReminder; }
        }

        public TaskReminderMailTemplate(Task task) : base(task)
        {

        }
    }

    public class TaskObserverAddedMailTempate : BaseTaskMailTemplate
    {
        public override MailType Type
        {
            get { return MailType.OnTaskObserverAdded; }
        }

        private readonly string _newObserver;
        
        public TaskObserverAddedMailTempate(Task task, string newObserver) : base(task)
        {
            _newObserver = newObserver;
        }

        protected override string FormatString(string formatedStr)
        {
            formatedStr = base.FormatString(formatedStr);

            formatedStr = formatedStr.Replace("#OBSERVER#", _newObserver);

            return formatedStr;
        }
    }
}