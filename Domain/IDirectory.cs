using SystemWrapper.IO;

namespace Domain
{
    public interface IDirectory
    {
        IDirectoryInfoWrap Directory { get; }
    }
}