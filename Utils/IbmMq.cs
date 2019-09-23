using IBM.WMQ;
using System.Collections;
using System.Collections.Generic;

namespace WM.Common.Utils
{
    public class IbmMq
    {
        private readonly MQQueueManager _mqQueueManager;
        public IbmMq(string queueManagerName)
        {
            Hashtable connectionProperties = new Hashtable();
            //connectionProperties.Add(MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED);
            connectionProperties.Add(MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_XACLIENT);

            _mqQueueManager = new MQQueueManager(queueManagerName, connectionProperties);
        }
        public void Disconnect()
        {
            _mqQueueManager.Disconnect();
        }
        public List<MQMessage> BrowseMessages(string queueName)
        {
            int totalMessages = MessageCount(queueName);
            MQQueue mqQueue = _mqQueueManager.AccessQueue(queueName, MQC.MQOO_BROWSE);
            MQGetMessageOptions messageOptions = new MQGetMessageOptions { Options = MQC.MQGMO_BROWSE_FIRST };
            if (totalMessages > 0)
            {
                List<MQMessage> messages = new List<MQMessage>();
                for (int i = 1; i <= totalMessages; i++)
                {
                    // Gettin an IBM MQ message from the queue.
                    MQMessage message = new MQMessage();                    
                    mqQueue.Get(message, messageOptions);
                    messages.Add(message);

                    // get next message
                    messageOptions.Options = MQC.MQGMO_BROWSE_NEXT;
                }

                mqQueue.Close();
                return messages;
            }

            mqQueue.Close();
            return null;
        }
        public MQMessage Get(string queueName)
        {
            MQQueue mqQueue = _mqQueueManager.AccessQueue(queueName, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);
            MQGetMessageOptions getMessageOptions = new MQGetMessageOptions();
            getMessageOptions.Options += MQC.MQGMO_WAIT + MQC.MQGMO_SYNCPOINT;
            getMessageOptions.WaitInterval = 20000; //20 Seconds

            MQMessage mqMessage = new MQMessage();
            mqQueue.Get(mqMessage, getMessageOptions);

            mqQueue.Close();

            return mqMessage;
        }
        public void Put(string queueName, MQMessage message)
        {
            MQQueue mqQueue = _mqQueueManager.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);
            MQPutMessageOptions putMessageOptions = new MQPutMessageOptions();
            putMessageOptions.Options += MQC.MQGMO_SYNCPOINT;

            mqQueue.Put(message, putMessageOptions);

            mqQueue.Close();
        }
        public int MessageCount(string queueName)
        {
            return _mqQueueManager.AccessQueue(queueName, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING + MQC.MQOO_INQUIRE).CurrentDepth;
        }
    }
}
