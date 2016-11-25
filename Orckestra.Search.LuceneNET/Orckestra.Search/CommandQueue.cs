using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using Composite.Core;
using Composite.Core.Extensions;
using Composite.Core.IO;
using System.Web.Script.Serialization;
using Composite;
using Orckestra.Search.Commands;

namespace Orckestra.Search
{
    static class CommandQueue
    {
        public const string QueueFolderRelativePath = "~/App_Data/Search/queue";

        const string MessageExtension = "command";
        const string CorruptedMessageExtension = "bad";

        private static readonly string LogTitle = typeof (CommandQueue).FullName;
        private static readonly string QueueFolder;

        public static AutoResetEvent NewMessages = new AutoResetEvent(false);

        static CommandQueue()
        {
            try
            {
                QueueFolder = PathUtil.Resolve(QueueFolderRelativePath);
            }
            catch(Exception ex)
            {
                Log.LogError(typeof(CommandQueue).FullName, ex);
            }
        }

        public static void Queue(IIndexUpdateCommand command)
        {
            Verify.ArgumentNotNull(command, nameof(command));

            PersistCommand(command);

            NewMessages.Set();
        }

        public static IIndexUpdateCommand Dequeue()
        {
            var files = GetFiles();

            foreach (var file in files)
            {
                string serializedCommand = File.ReadAllText(file);

                try
                {
                    var serializer = new JavaScriptSerializer(new SimpleTypeResolver());

                    var command = serializer.Deserialize<IIndexUpdateCommand>(serializedCommand);
                    return command;
                }
                catch (Exception ex)
                {
                    Log.LogError(LogTitle, ex);
                    
                    File.WriteAllText(file.Replace(MessageExtension, CorruptedMessageExtension), serializedCommand);
                }
                finally
                {
                    File.Delete(file);
                }
            }

            return null;
        }

        private static IEnumerable<string> GetFiles()
        {
            return Directory.Exists(QueueFolder) 
                ? Directory.GetFiles(QueueFolder, "*." + MessageExtension).OrderBy(f => f).ToList()
                : Enumerable.Empty<string>();
        } 

        public static void ExecuteCommand(IIndexUpdateCommand command)
        {
            try
            {
                var index = ServiceLocator.GetService<ISearchIndex>();
                command.Execute(index);
            }
            catch (ThreadAbortException)
            {
                if (IsRestarting())
                {
                    // Saving the command if the thread was aborted
                    PersistCommand(command);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(LogTitle, ex);
            }
        }

        private static void PersistCommand(IIndexUpdateCommand command)
        {
            var serializedCommand = new JavaScriptSerializer(new SimpleTypeResolver()).Serialize(command);

            var fileName = DateTime.Now.ToString("yyMMddHHmmssffffff") + "." + MessageExtension;

            Directory.CreateDirectory(QueueFolder);
            File.WriteAllText(QueueFolder + "/" + fileName, serializedCommand);
        }

        private static bool IsRestarting()
        {
            return HostingEnvironment.ApplicationHost.ShutdownInitiated();
        }

        public static void ClearCommands()
        {
            try
            {
                var files = GetFiles();
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
            catch { }
        }
    }
}
