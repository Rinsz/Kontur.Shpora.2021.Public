using System;
using System.Threading;

namespace ReaderWriterLock
{
    public class RwLock : IRwLock
    {

        private long writers;

        private long readers;

        private object locker = new();

        private SpinWait spinWait = new();
		
        public void ReadLocked(Action action)
        {
            while (Interlocked.Read(ref writers) > 0)
                spinWait.SpinOnce();
			
            Interlocked.Increment(ref readers);
			
            action.Invoke();
			
            Interlocked.Decrement(ref readers);
        }

        public void WriteLocked(Action action)
        {

            Interlocked.Increment(ref writers);

            while (Interlocked.Read(ref readers) > 0)
                spinWait.SpinOnce();
			
            lock(locker)
                action.Invoke();
			
            Interlocked.Decrement(ref writers);
        }
    }
}