﻿using Domain;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using SystemWrapper.IO;

namespace Services.Disk
{
    public interface IBackupDirectoryValidator
    {
        ValidationResult CanAdd(string backupDirectory);
    }

    public enum ValidationResult
    {
        Success,
        InvalidPath,
        PathAlreadyExists
    }

    public class BackupDirectoryValidator : IBackupDirectoryValidator
    {
        private IDirectoryWrap _directoryWrap;
        private IBackupDirectoryModel _backupDirectoryModel;

        public BackupDirectoryValidator(IDirectoryWrap directoryWrap, IBackupDirectoryModel model) 
        {
            _directoryWrap = directoryWrap;
            _backupDirectoryModel = model;
        }

        public ValidationResult CanAdd(string backupDirectory)
        {
            if(backupDirectory == null
                ||!_directoryWrap.Exists(backupDirectory)
                || backupDirectory.Contains(".")
                || (!backupDirectory.Contains("/")
                && !backupDirectory.Contains(@"\")))
                    return ValidationResult.InvalidPath;

            return _backupDirectoryModel.IsSubdirectoryOfExisting(backupDirectory)
                ? ValidationResult.PathAlreadyExists
                : ValidationResult.Success;
        }
    }
}
