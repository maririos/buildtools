﻿using System;
using System.Collections.Generic;

namespace Microsoft.DotNet.Execute
{
    public class Setup
    {
        public Dictionary<string, Setting> Settings { get; set; }
        public Dictionary<string, Command> Commands { get; set; }
        public Dictionary<string, Process> Processes { get; set; }


        public void ProvideHelpSettings()
        {
            Console.WriteLine("========Settings========");

            foreach (KeyValuePair<string, Setting> settingInfo in Settings)
            {
                Console.WriteLine("* {0} - {1}", settingInfo.Key, settingInfo.Value.Description);
                if (settingInfo.Value.Values.Count > 0)
                {
                    Console.WriteLine("    The allowed values are: {0}", string.Join(", ", settingInfo.Value.Values));
                }
                if (!string.IsNullOrEmpty(settingInfo.Value.DefaultValue))
                {
                    Console.WriteLine("    The default value is: {0}", settingInfo.Value.DefaultValue);
                }
            }
        }

        private string FindSettingValue(string valueToFind)
        {
            if (Settings.ContainsKey(valueToFind))
            {
                return Settings[valueToFind].DefaultValue;
            }
            return null;
        }

        private string FindSettingType(string valueToFind)
        {
            if (Settings.ContainsKey(valueToFind))
            {
                return Settings[valueToFind].ValueType;
            }
            return null;
        }

        public bool BuildCommand(Command commandToExecute, string os, Dictionary<string, string> settingParameters)
        {
            string toolName = string.Empty;
            if (Processes.ContainsKey(commandToExecute.ToolName))
            {
                toolName = (os.Equals("windows")) ? Processes[commandToExecute.ToolName].Run["windows"] : Processes[commandToExecute.ToolName].Run["non-windows"];
            }
            else
            {
                Console.WriteLine("Error: The process {0} is not specified in the Json file.", commandToExecute.ToolName);
                return false;
            }

            if (BuildRequiredValueSettingsForCommand(commandToExecute.LockedSettings, settingParameters) &&
                    BuildOptionalValueSettingsForCommand(commandToExecute.Settings, settingParameters))
            {
                string commandParameters = BuildParametersForCommand(settingParameters, commandToExecute.ToolName);
                Run runCommand = new Run();
                return Convert.ToBoolean(runCommand.ExecuteProcess(toolName, commandParameters));
            }

            return false;
        }
        
        private string BuildParametersForCommand(Dictionary<string, string> settingParameters, string toolName)
        {
            string commandSetting = string.Empty;
            foreach (KeyValuePair<string, string> parameters in settingParameters)
            {
                if (!string.IsNullOrEmpty(parameters.Value))
                {
                    string settingType = FindSettingType(parameters.Key);
                    if (settingType.Equals("passThrough"))
                    {
                        commandSetting += string.Format(" {0}", parameters.Value);
                    }
                    else
                    {
                        commandSetting += string.Format(" {0}", FormatSetting(parameters.Key, parameters.Value, FindSettingType(parameters.Key), toolName));
                    }
                }
            }
            return commandSetting;
        }

        private bool BuildRequiredValueSettingsForCommand(Dictionary<string, string> requiredSettings, Dictionary<string, string> commandValues)
        {
            foreach (KeyValuePair<string, string> reqSetting in requiredSettings)
            {
                string value = string.IsNullOrEmpty(reqSetting.Value) || reqSetting.Value.Equals("default") ? FindSettingValue(reqSetting.Key) : reqSetting.Value;
                if (value != null && string.IsNullOrEmpty(commandValues[reqSetting.Key]))
                {
                    commandValues[reqSetting.Key] = string.IsNullOrEmpty(value) ? "True" : value;
                }
                else
                {
                    if (!string.IsNullOrEmpty(value) && !value.Equals(commandValues[reqSetting.Key]))
                    {
                        Console.WriteLine("Error: The value for setting {0} can't be overwriten.", reqSetting.Key);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool BuildOptionalValueSettingsForCommand(Dictionary<string, string> optionalSettings, Dictionary<string, string> commandValues)
        {
            foreach (KeyValuePair<string, string> optSetting in optionalSettings)
            {
                if (string.IsNullOrEmpty(commandValues[optSetting.Key]))
                {
                    string value = string.IsNullOrEmpty(optSetting.Value) || optSetting.Value.Equals("default") ? FindSettingValue(optSetting.Key) : optSetting.Value;
                    if (value != null)
                    {
                        commandValues[optSetting.Key] = value;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string FormatSetting(string option, string value, string type, string toolName)
        {
            if (Processes.ContainsKey(toolName) && !string.IsNullOrEmpty(type))
            {
                string commandOption = Processes[toolName].ValueTypes[type];
                commandOption = commandOption.Replace("{name}", option).Replace("{value}", value);
                return commandOption;
            }
            return null;
        }
    }

    public class Command
    {
        public string Description { get; set; }
        public string ToolName { get; set; }
        public Dictionary<string, string> LockedSettings { get; set; }
        public Dictionary<string, string> Settings { get; set; }

    }

    public class Process
    {
        public Dictionary<string, string> Run { get; set; }
        public Dictionary<string, string> ValueTypes { get; set; }
    }

    public class Setting
    {
        public string Description { get; set; }
        public string ValueType { get; set; }
        public List<string> Values { get; set; }
        public string DefaultValue { get; set; }
    }
}