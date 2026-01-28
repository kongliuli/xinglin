using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xinglin.ReportTemplateEditor.WPF.Commands;

namespace Xinglin.ReportTemplateEditor.WPF.Services
{
    /// <summary>
    /// 命令管理器服务
    /// </summary>
    public class CommandManagerService
    {
        private readonly List<ICommand> _commands;
        private readonly Dictionary<string, ICommand> _commandMap;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandManagerService()
        {
            _commands = new List<ICommand>();
            _commandMap = new Dictionary<string, ICommand>();
        }
        
        /// <summary>
        /// 注册命令
        /// </summary>
        /// <param name="commandName">命令名称</param>
        /// <param name="command">命令</param>
        public void RegisterCommand(string commandName, ICommand command)
        {
            _commands.Add(command);
            _commandMap[commandName] = command;
        }
        
        /// <summary>
        /// 获取命令
        /// </summary>
        /// <param name="commandName">命令名称</param>
        /// <returns>命令</returns>
        public ICommand GetCommand(string commandName)
        {
            if (_commandMap.TryGetValue(commandName, out var command))
            {
                return command;
            }
            return null;
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandName">命令名称</param>
        /// <param name="parameter">命令参数</param>
        public void ExecuteCommand(string commandName, object parameter = null)
        {
            var command = GetCommand(commandName);
            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
        
        /// <summary>
        /// 重置所有命令的可执行状态
        /// </summary>
        public void InvalidateCommands()
        {
            foreach (var command in _commands)
            {
                if (command is RelayCommand relayCommand)
                {
                    relayCommand.RaiseCanExecuteChanged();
                }
                else if (command is RelayCommand<object> genericRelayCommand)
                {
                    genericRelayCommand.RaiseCanExecuteChanged();
                }
            }
        }
    }
}
