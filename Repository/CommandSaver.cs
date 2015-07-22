using System;
using System.Web.Script.Serialization;
using Repository.Enums;
using Repository.Models;

namespace Repository
{
    public interface ICommandSaver
    {
        Command CreateCommand(BaseEntity entity, CommandTypes commandType, string user);
    }

    public class CommandSaver : ICommandSaver
    {
        public Command CreateCommand(BaseEntity entity, CommandTypes commandType, string user)
        {
            var command = new Command
            {
                CommandType = commandType.ToString(),
                Date = DateTime.Now,
                EntityType = entity.GetType().Name,
                User = user,
                CommandContent = new JavaScriptSerializer().Serialize(entity),
                Id = Guid.NewGuid()
            };
            return command;
        }
    }
}