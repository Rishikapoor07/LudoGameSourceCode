using System;
using System.Threading;
using VivoxUnity;

public delegate bool LoopDone();

public delegate void RunLoop(ref bool didWork);


public class Waiter
{
    private WaitHandle _waitHandle;
    private DateTime _until;
    public Waiter(WaitHandle handle, TimeSpan until)
    {
        _waitHandle = handle;
        _until = DateTime.Now + until;
    }

    public bool IsDone()
    {
        if (_waitHandle != null)
        {
            if (_waitHandle.WaitOne(0))
                return true;
        }
        if (DateTime.Now >= _until)
            return true;
        return false;
    }
}

public class MessagePump
{
    private static MessagePump _instance;
    MessagePump()
    {
    }

    public event RunLoop MainLoopRun;

    public static MessagePump Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MessagePump();
            return _instance;
        }
    }

    public void RunUntil(LoopDone done)
    {
        for (; ; )
        {
            RunOnce();
            if (!done())
            {
                Thread.Sleep(20);
            }
            else
            {
                break;
            }
        }
    }

    public void RunOnce()
    {
        for (; ; )
        {
            bool didWork = false;
            MainLoopRun?.Invoke(ref didWork);
            if (didWork)
                continue;
            break;
        }
    }

    public static bool IsDone(WaitHandle handle, DateTime until)
    {
        if (handle != null)
        {
            if (handle.WaitOne(0))
                return true;
        }
        if (DateTime.Now >= until)
            return true;
        return false;
    }

    public static bool Run(WaitHandle handle, TimeSpan until)
    {
        var then = DateTime.Now + until;
        MessagePump.Instance.RunUntil(new LoopDone(() => MessagePump.IsDone(handle, then)));
        if (handle != null) return handle.WaitOne(0);
        return false;
    }

    public delegate bool DoneDelegate();
}
