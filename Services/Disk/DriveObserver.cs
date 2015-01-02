using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    class DiskObserver : IObserver<IDriveInfoWrap>
    {
        private List<IDriveInfoWrap> _previouslySeenDrives =
            new List<IDriveInfoWrap>();

        private Action<IDriveInfoWrap> _action;

        public DiskObserver(Action<IDriveInfoWrap> action, IEnumerable<IDriveInfoWrap> initialDrives)
        {
            _action = action;
            _previouslySeenDrives.AddRange(initialDrives);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(IDriveInfoWrap value)
        {
            if (!_previouslySeenDrives.Contains(value))
            {
                _previouslySeenDrives.Add(value);
                _action(value);
            }

        }
    }
}
