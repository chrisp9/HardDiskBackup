using System;

namespace Domain
{
    public interface IEnvironmentWrap
    {
        string AppDataPath { get; }
    }

    public class EnvironmentWrap : IEnvironmentWrap
    {
        public string AppDataPath
        {
            get
            {
                return Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);
            }
        }
    }
}