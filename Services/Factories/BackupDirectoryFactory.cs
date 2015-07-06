using Domain;
using Registrar;
using Services.Disk.FileSystem;
using SystemWrapper.IO;

namespace Services.Factories
{
    public interface IDirectoryFactory
    {
        BackupDirectory GetBackupDirectoryFor(string path);

        BackupRootDirectory GetBackupRootDirectoryForDrive(IDriveInfoWrap drive);
    }

    // Why on earth do I have have 3 types for essentially the same thing (a wrapper around a directory)??
    // What is this madness?!
    //
    // Well, BackupDirectory, MirroredDirectory and BackupRootDirectory are non-interchangable
    // domain concepts. Having these types of directory represented in a type-safe way reduces the
    // possibilities of errors around using the wrong Directory type, and also makes the contract
    // of consumers more explicit. These are not hidden behind interfaces because they are domain objects
    // and I will never need to provide test dummies (I can provide dummy underlying IDirectoryInfoWraps
    // if I need to manipulate the behaviour for tests).
    //
    // Also, there is some duplicated code between BackupDirectory, MirroredDirectory and
    // BackupRootDirectory. This is fine in this case - I don't want to use inheritence as I want
    // to stress that they're distinct non-interchangable concepts. I could factor out the common code
    // into a helper class, but there's only three types. Maybe if I add more, I'll consider it...
    // Whew...

    //TODO: This class is a target for future refactoring.
    [Register(LifeTime.Transient)]
    public class BackupFactory : IDirectoryFactory
    {
        private IFileSystemRootProvider _rootProvider;

        public BackupFactory(IFileSystemRootProvider rootProvider)
        {
            _rootProvider = rootProvider;
        }

        public BackupDirectory GetBackupDirectoryFor(string path)
        {
            return new BackupDirectory(Sanitize(path));
        }

        /// <summary>
        /// Gets the BackupRootDirectory for the drive.
        /// </summary>
        /// <param name="drive">The drive</param>
        /// <returns>The BackupRootDirectory</returns>
        public BackupRootDirectory GetBackupRootDirectoryForDrive(IDriveInfoWrap drive)
        {
            var roo = _rootProvider.GetFileSystemRoot(drive);
            return new BackupRootDirectory(new DirectoryInfoWrap(roo));
        }

        private DirectoryInfoWrap Sanitize(string path)
        {
            var sanitisedPath = path.Replace('/', '\\');
            var directoryInfo = new DirectoryInfoWrap(sanitisedPath);
            return directoryInfo;
        }
    }
}