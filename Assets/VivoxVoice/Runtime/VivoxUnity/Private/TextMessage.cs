using System;
using System.ComponentModel;

namespace VivoxUnity.Private
{
    internal class DirectedTextMessage : IDirectedTextMessage
    {
        private Exception _exception;
        public event PropertyChangedEventHandler PropertyChanged;
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
                }
            }
        }
        public string Key { get; set; }

        public DateTime ReceivedTime { get; set; }
        public string Message { get; set; }
        public string Language { get; set; }

        public ILoginSession LoginSession { get; set; }
        public AccountId Sender { get; set; }
        public string ApplicationStanzaNamespace { get; set; }
        public string ApplicationStanzaBody { get; set; }
    }

    internal class FailedDirectedTextMessage : IFailedDirectedTextMessage
    {
        private Exception _exception;
        public event PropertyChangedEventHandler PropertyChanged;
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
                }
            }
        }

        public AccountId Sender { get; set; }
        public string RequestId { get; set; }
        public int StatusCode { get; set; }
    }

    internal class ChannelTextMessage : IChannelTextMessage
    {
        private Exception _exception;
        public event PropertyChangedEventHandler PropertyChanged;
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
                }
            }
        }
        public string Key { get; set; }

        public DateTime ReceivedTime { get; set; }
        public string Message { get; set; }
        public string Language { get; set; }

        public IChannelSession ChannelSession { get; set; }
        public AccountId Sender { get; set; }
        public bool FromSelf { get; set; }
        public string ApplicationStanzaNamespace { get; set; }
        public string ApplicationStanzaBody { get; set; }
     }

    internal class SessionArchiveMessage : ISessionArchiveMessage
    {
        private Exception _exception;
        public event PropertyChangedEventHandler PropertyChanged;
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
                }
            }
        }
        public string Key { get; set; }

        public DateTime ReceivedTime { get; set; }
        public string Message { get; set; }
        public string Language { get; set; }

        public IChannelSession ChannelSession { get; set; }
        public AccountId Sender { get; set; }
        public bool FromSelf { get; set; }

        public string QueryId { get; set; }
        public string MessageId { get; set; }
    }

    public class AccountArchiveMessage : IAccountArchiveMessage
    {
        private Exception _exception;
        public event PropertyChangedEventHandler PropertyChanged;
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
                }
            }
        }
        public string Key { get; set; }

        public DateTime ReceivedTime { get; set; }
        public string Message { get; set; }
        public string Language { get; set; }

        public ILoginSession LoginSession { get; set; }
        public string QueryId { get; set; }
        public AccountId RemoteParticipant { get; set; }
        public ChannelId Channel { get; set; }
        public bool Inbound { get; set; }
        public string MessageId { get; set; }
    }

    public class TranscribedMessage : ITranscribedMessage
    {
        private Exception _exception;

        public TranscribedMessage(AccountId sender, string message, string key, string language, IChannelSession channelSession,
            bool fromSelf, DateTime? receivedTime = null)
        {
            ReceivedTime = receivedTime ?? DateTime.Now;
            Sender = sender;
            Message = message;
            Key = key;
            Language = language;
            ChannelSession = channelSession;
            FromSelf = fromSelf;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
                }
            }
        }
        public string Key { get; set; }

        public DateTime ReceivedTime { get; private set; }
        public string Message { get; private set; }
        public string Language { get; private set; }

        public IChannelSession ChannelSession { get; private set; }
        public AccountId Sender { get; private set; }
        public bool FromSelf { get; private set; }
    }
}
